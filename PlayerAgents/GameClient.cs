using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using C = ClientPackets;
using S = ServerPackets;
using Shared;
using PlayerAgents.Map;

public sealed partial class GameClient
{
    private readonly Config _config;
    private readonly NpcMemoryBank _npcMemory;
    private readonly MapMovementMemoryBank _movementMemory;
    private readonly MapExpRateMemoryBank _expRateMemory;
    private readonly NavDataManager _navDataManager;
    private readonly IAgentLogger? _logger;
    private CancellationTokenSource? _movementSaveCts;
    private CancellationTokenSource? _movementDeleteCts;
    public event Action? MovementEntryRemoved;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private long _pingTime;
    private readonly byte[] _buffer = new byte[1024 * 8];
    private readonly MemoryStream _receiveStream = new();
    private readonly Random _random = new();
    private MirClass? _playerClass;
    private BaseStats? _baseStats;
    private readonly TaskCompletionSource<MirClass> _classTcs = new();
    private Point _currentLocation = Point.Empty;
    private Point? _pendingMoveTarget;
    private readonly List<Point> _pendingMovementAction = new();
    private string _playerName = string.Empty;
    private string _currentAction = string.Empty;
    private uint _objectId;
    private string _currentMapFile = string.Empty;
    private string _currentMapName = string.Empty;
    private PlayerAgents.Map.MapData? _mapData;
    private NavData? _navData;

    public string PlayerName => string.IsNullOrEmpty(_playerName) ? _config.CharacterName : _playerName;
    public string CurrentAction => _currentAction;
    public ushort Level => _level;
    public string CurrentMapFile => _currentMapFile;
    public string CurrentMapName => _currentMapName;
    public NavData? NavData => _navData;

    private LightSetting _timeOfDay = LightSetting.Normal;

    private MirGender _gender;
    private ushort _level;
    private long _experience;
    private int _hp;
    private int _mp;
    private UserItem[]? _inventory;
    private UserItem[]? _equipment;
    private UserItem? _lastPickedItem;
    private uint _gold;

    private int _maxBagWeight;
    private int _maxWearWeight;
    private int _maxHandWeight;
    private int _maxHP;
    private int _maxMP;
    private bool _statsDirty = true;

    private uint? _lastAttackTarget;
    private uint? _lastStruckAttacker;

    private bool _dead;

    private DateTime _lastMoveTime = DateTime.MinValue;
    private bool _canRun;

    private DateTime _mapStartTime = DateTime.MinValue;
    private long _mapStartExp;
    private ushort _mapStartLevel;
    private MirClass? _mapStartClass;
    private long _mapExpGained;

    private TimeSpan _mapElapsedBeforePause = TimeSpan.Zero;
    private bool _mapExpPaused;
    private string _trackedMapFile = string.Empty;

    private string _pausedMapFile = string.Empty;
    private long _pausedMapExpGained;
    private TimeSpan _pausedMapElapsed = TimeSpan.Zero;
    private long _pausedMapStartExp;
    private MirClass? _pausedMapClass;
    private ushort _pausedMapLevel;
    private bool _hasPausedMapSession;

    // store information on nearby objects
    private readonly ConcurrentDictionary<uint, TrackedObject> _trackedObjects = new();
    private readonly ConcurrentDictionary<System.Drawing.Point, int> _blockingCells = new();

    private static bool IsBlocking(TrackedObject obj) =>
        !obj.Dead && (obj.Type == ObjectType.Player || obj.Type == ObjectType.Monster || obj.Type == ObjectType.Merchant);

    private void AddTrackedObject(TrackedObject obj)
    {
        _trackedObjects[obj.Id] = obj;
        if (IsBlocking(obj))
            _blockingCells.AddOrUpdate(obj.Location, 1, (_, v) => v + 1);
    }

    private void UpdateTrackedObject(uint id, Point newLoc, MirDirection dir)
    {
        if (_trackedObjects.TryGetValue(id, out var obj))
        {
            var oldLoc = obj.Location;
            obj.Location = newLoc;
            obj.Direction = dir;
            if (IsBlocking(obj) && oldLoc != newLoc)
            {
                _blockingCells.AddOrUpdate(newLoc, 1, (_, v) => v + 1);
                if (_blockingCells.AddOrUpdate(oldLoc, 0, (_, v) => Math.Max(0, v - 1)) == 0)
                    _blockingCells.TryRemove(oldLoc, out _);
            }
        }
    }

    private void RemoveTrackedObject(uint id)
    {
        if (_trackedObjects.TryRemove(id, out var obj))
        {
            if (IsBlocking(obj))
            {
                var oldLoc = obj.Location;
                if (_blockingCells.AddOrUpdate(oldLoc, 0, (_, v) => Math.Max(0, v - 1)) == 0)
                    _blockingCells.TryRemove(oldLoc, out _);
            }
        }
    }

    private readonly ConcurrentDictionary<uint, NpcEntry> _npcEntries = new();
    private uint? _dialogNpcId;
    private readonly Queue<uint> _npcQueue = new();
    private readonly Queue<(string key, Func<Task> action)> _npcActionTasks = new();
    private bool _processingNpcAction;
    private DateTime _npcInteractionStart;
    private bool _skipNextGoods;
    public bool IsProcessingNpc => _dialogNpcId.HasValue;

    private NPCInteraction? _npcInteraction;

    private readonly Dictionary<ulong, (NpcEntry entry, ItemType type)> _pendingSellChecks = new();
    private readonly Dictionary<ulong, (NpcEntry entry, ItemType type)> _pendingRepairChecks = new();

    private TaskCompletionSource<S.NPCResponse>? _npcResponseTcs;
    private TaskCompletionSource<bool>? _npcGoodsTcs;
    private TaskCompletionSource<bool>? _npcSellTcs;
    private TaskCompletionSource<bool>? _npcRepairTcs;
    private readonly Dictionary<ulong, TaskCompletionSource<S.SellItem>> _sellItemTcs = new();
    private readonly Dictionary<ulong, TaskCompletionSource<bool>> _repairItemTcs = new();
    private const int NpcResponseDebounceMs = 100;
    private readonly Dictionary<(string name, string map, int x, int y), DateTime> _recentNpcInteractions = new();

    private List<UserItem>? _lastNpcGoods;
    private PanelType _lastNpcGoodsType;


    // Use a dictionary for faster lookups by item index
    // Shared across all agents; using ConcurrentDictionary for thread safety
    public static readonly ConcurrentDictionary<int, ItemInfo> ItemInfoDict = new();

    private static readonly HashSet<byte> AutoHarvestAIs = new() { 1, 2, 4, 5, 7, 9 };

    private bool _awaitingHarvest;
    private uint? _harvestTargetId;
    private bool _harvestComplete;
    private static readonly TimeSpan HarvestDelay = TimeSpan.FromMilliseconds(600);
    private DateTime _nextHarvestTime = DateTime.MinValue;

    public bool IsHarvesting => _awaitingHarvest;
    public bool MovementSavePending => _movementSaveCts != null;
    public bool IgnoreNpcInteractions { get; set; }

    private static void Bind(UserItem item)
    {
        if (ItemInfoDict.TryGetValue(item.ItemIndex, out var info))
        {
            item.Info = info;
            for (int i = 0; i < item.Slots.Length; i++)
            {
                if (item.Slots[i] != null)
                    Bind(item.Slots[i]!);
            }
        }
    }

    private static void BindAll(UserItem[]? items)
    {
        if (items == null) return;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                Bind(items[i]!);
        }
    }

    private static int DefaultItemScore(UserItem item, EquipmentSlot slot)
    {
        int score = 0;
        if (item.Info != null)
            score += item.Info.Stats.Count;
        if (item.AddedStats != null)
            score += item.AddedStats.Count;
        return score;
    }

    private int GetItemScore(UserItem item, EquipmentSlot slot)
    {
        if (ItemScoreFunc != null)
            return ItemScoreFunc(item, slot);
        return DefaultItemScore(item, slot);
    }

    private int GetBestItemScore(UserItem item)
    {
        if (item.Info == null) return 0;
        int best = 0;
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            if (!IsItemForSlot(item.Info, slot)) continue;
            if (!CanEquipItem(item, slot)) continue;
            int score = GetItemScore(item, slot);
            if (score > best) best = score;
        }
        return best;
    }

    private static bool MatchesDesiredItem(UserItem item, DesiredItem desired)
    {
        if (item.Info == null) return false;
        if (item.Info.Type != desired.Type) return false;
        if (desired.Shape.HasValue && item.Info.Shape != desired.Shape.Value) return false;
        if (desired.HpPotion.HasValue)
        {
            bool healsHP = item.Info.Stats[Stat.HP] > 0 || item.Info.Stats[Stat.HPRatePercent] > 0;
            bool healsMP = item.Info.Stats[Stat.MP] > 0 || item.Info.Stats[Stat.MPRatePercent] > 0;
            if (desired.HpPotion.Value && !healsHP) return false;
            if (!desired.HpPotion.Value && !healsMP) return false;
        }

        return true;
    }

    private bool NeedMoreOfDesiredItem(DesiredItem desired)
    {
        if (_inventory == null) return false;
        var matching = _inventory.Where(i => i != null && MatchesDesiredItem(i, desired)).ToList();

        if (desired.Count.HasValue)
            return matching.Count < desired.Count.Value;

        if (desired.WeightFraction > 0)
        {
            int requiredWeight = (int)Math.Ceiling(GetMaxBagWeight() * desired.WeightFraction);
            int currentWeight = matching.Sum(i => i.Weight);
            return currentWeight < requiredWeight;
        }

        return false;
    }

    private static bool CanBeEquipped(ItemInfo info)
    {
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            if (IsItemForSlot(info, slot))
                return true;
        }
        return false;
    }

    private async Task EquipIfBetterAsync(UserItem item)
    {
        if (_equipment == null || item.Info == null) return;

        int bestSlot = -1;
        int bestDiff = 0;

        for (int slot = 0; slot < _equipment.Length; slot++)
        {
            var equipSlot = (EquipmentSlot)slot;
            if (!IsItemForSlot(item.Info, equipSlot)) continue;
            if (!CanEquipItem(item, equipSlot)) continue;

            var current = _equipment[slot];
            int newScore = GetItemScore(item, equipSlot);
            int currentScore = current != null ? GetItemScore(current, equipSlot) : -1;
            int diff = newScore - currentScore;
            if (diff > bestDiff)
            {
                bestDiff = diff;
                bestSlot = slot;
            }
        }

        if (bestSlot >= 0 && bestDiff > 0)
        {
            await EquipItemAsync(item, (EquipmentSlot)bestSlot);
            await Task.Delay(200);
            _lastPickedItem = null;
        }
    }

    private async Task BuyNeededItemsFromGoodsAsync(List<UserItem> goods, PanelType type)
    {
        if (goods.Count == 0) return;

        var desired = DesiredItemsProvider?.Invoke();
        if (desired == null && _equipment == null) return;

        foreach (var g in goods)
            Bind(g);

        var orderedGoods = goods
            .OrderByDescending(g => GetBestItemScore(g))
            .ToList();

        int currentWeight = GetCurrentBagWeight();
        int maxWeight = GetMaxBagWeight();
        int freeSlots = _inventory?.Count(i => i == null) ?? int.MaxValue;

        foreach (var item in orderedGoods)
        {
            if (item.Info == null) continue;

            bool need = false;

            if (_equipment != null)
            {
                for (int slot = 0; slot < _equipment.Count(); slot++)
                {
                    var equipSlot = (EquipmentSlot)slot;
                    if (!IsItemForSlot(item.Info, equipSlot)) continue;
                    if (!CanEquipItem(item, equipSlot)) continue;
                    var current = _equipment[slot];
                    int newScore = GetItemScore(item, equipSlot);
                    int currentScore = current != null ? GetItemScore(current, equipSlot) : -1;
                    if (newScore > currentScore)
                    {
                        need = true;
                        break;
                    }
                }
            }

            if (!need && desired != null)
            {
                foreach (var d in desired)
                {
                    if (MatchesDesiredItem(item, d) && NeedMoreOfDesiredItem(d))
                    {
                        need = true;
                        break;
                    }
                }
            }

            if (need && _gold >= item.Info.Price)
            {
                if (freeSlots <= 0 || currentWeight + item.Weight > maxWeight)
                    continue;

                if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var npc))
                    Log($"I am buying {item.Info.FriendlyName} from {npc.Name} for {item.Info.Price} gold");
                await BuyItemAsync(item.UniqueID, 1, type);
                await Task.Delay(50);
                if (_lastPickedItem != null && _lastPickedItem.Info != null &&
                    _lastPickedItem.Info.Index == item.Info.Index && CanBeEquipped(_lastPickedItem.Info))
                {
                    await EquipIfBetterAsync(_lastPickedItem);
                }

                freeSlots--;
                currentWeight += item.Weight;
            }
        }
    }

    public IReadOnlyList<UserItem>? Inventory => _inventory;
    public IReadOnlyList<UserItem>? Equipment => _equipment;

    public bool Dead => _dead;

    public MirClass? PlayerClass => _playerClass;
    public Task<MirClass> WaitForClassAsync() => _classTcs.Task;
    public LightSetting TimeOfDay => _timeOfDay;
    public MapData? CurrentMap => _mapData;
    public IReadOnlyDictionary<uint, TrackedObject> TrackedObjects => _trackedObjects;
    public IEnumerable<Point> BlockingCells => _blockingCells.Keys;
    public bool IsMapLoaded => _mapData != null && _mapData.Width > 0 && _mapData.Height > 0;
    public Point CurrentLocation => _currentLocation;
    public long PingTime => _pingTime;
    public uint ObjectId => _objectId;
    public uint Gold => _gold;
    public UserItem? LastPickedItem => _lastPickedItem;
    public int HP => _hp;
    public int MP => _mp;
    public MapMovementMemoryBank MovementMemory => _movementMemory;
    public MapExpRateMemoryBank ExpRateMemory => _expRateMemory;
    public Func<UserItem, EquipmentSlot, int>? ItemScoreFunc { get; set; }
    public Func<IReadOnlyList<DesiredItem>>? DesiredItemsProvider { get; set; }

    private void ReportStatus()
    {
        var status = new AgentStatus
        {
            Level = _level,
            MapFile = _currentMapFile,
            MapName = _currentMapName,
            X = _currentLocation.X,
            Y = _currentLocation.Y,
            Action = _currentAction
        };
        _logger?.UpdateStatus(PlayerName, status);
    }

    public void UpdateAction(string action)
    {
        _currentAction = action;
        ReportStatus();
    }

    public GameClient(Config config, NpcMemoryBank npcMemory, MapMovementMemoryBank movementMemory, MapExpRateMemoryBank expRateMemory, NavDataManager navDataManager, IAgentLogger? logger = null)
    {
        _config = config;
        _npcMemory = npcMemory;
        _movementMemory = movementMemory;
        _expRateMemory = expRateMemory;
        _navDataManager = navDataManager;
        _logger = logger;
    }

    private void StartMapExpTracking(string mapFile)
    {
        // resume if we previously paused on this map at this level
        if (_hasPausedMapSession && _pausedMapFile == mapFile && _pausedMapLevel == _level)
        {
            _trackedMapFile = mapFile;
            _mapElapsedBeforePause = _pausedMapElapsed;
            _mapStartTime = DateTime.UtcNow;
            _mapStartExp = _pausedMapStartExp;
            _mapExpGained = _pausedMapExpGained;
            _mapStartLevel = _pausedMapLevel;
            _mapStartClass = _pausedMapClass;
            _hasPausedMapSession = false;
            return;
        }

        // finalize any existing active tracking
        if (!string.IsNullOrEmpty(_trackedMapFile))
            FinalizeMapExpRate();

        _trackedMapFile = mapFile;
        _mapElapsedBeforePause = TimeSpan.Zero;
        _mapStartTime = DateTime.UtcNow;
        _mapStartExp = _experience;
        _mapExpGained = 0;
        _mapStartLevel = _level;
        _mapStartClass = _playerClass;
        _mapExpPaused = false;
    }

    private void PauseMapExpTracking()
    {
        if (string.IsNullOrEmpty(_trackedMapFile))
            return;

        TimeSpan elapsed = _mapElapsedBeforePause;
        if (!_mapExpPaused && _mapStartTime != DateTime.MinValue)
            elapsed += DateTime.UtcNow - _mapStartTime;

        if (_hasPausedMapSession)
            FinalizePausedMapSession();

        _pausedMapFile = _trackedMapFile;
        _pausedMapExpGained = _mapExpGained;
        _pausedMapElapsed = elapsed;
        _pausedMapStartExp = _mapStartExp;
        _pausedMapClass = _mapStartClass;
        _pausedMapLevel = _mapStartLevel;
        _hasPausedMapSession = true;

        _trackedMapFile = string.Empty;
        _mapStartTime = DateTime.MinValue;
        _mapElapsedBeforePause = TimeSpan.Zero;
        _mapExpGained = 0;
        _mapExpPaused = false;
    }

    private void FinalizePausedMapSession()
    {
        if (!_hasPausedMapSession || string.IsNullOrEmpty(_pausedMapFile) || _pausedMapClass == null)
            return;

        if (_pausedMapElapsed >= TimeSpan.FromMinutes(15))
        {
            double rate = _pausedMapExpGained / _pausedMapElapsed.TotalHours;
            _expRateMemory.AddRate(_pausedMapFile, _pausedMapClass.Value, _pausedMapLevel, rate);
        }

        _hasPausedMapSession = false;
        _pausedMapFile = string.Empty;
        _pausedMapExpGained = 0;
        _pausedMapElapsed = TimeSpan.Zero;
        _pausedMapStartExp = 0;
        _pausedMapClass = null;
        _pausedMapLevel = 0;
    }

    private void FinalizeMapExpRate()
    {
        if (string.IsNullOrEmpty(_trackedMapFile)) return;

        TimeSpan elapsed = _mapElapsedBeforePause;
        if (!_mapExpPaused && _mapStartTime != DateTime.MinValue)
            elapsed += DateTime.UtcNow - _mapStartTime;

        if (elapsed >= TimeSpan.FromMinutes(15) && _mapStartClass != null)
        {
            double rate = _mapExpGained / elapsed.TotalHours;
            _expRateMemory.AddRate(_trackedMapFile, _mapStartClass.Value, _mapStartLevel, rate);
        }

        _mapElapsedBeforePause = TimeSpan.Zero;
        _mapExpPaused = false;
        _mapStartTime = DateTime.MinValue;
        _trackedMapFile = string.Empty;
    }

    public void ProcessMapExpRateInterval()
    {
        if (string.IsNullOrEmpty(_trackedMapFile)) return;
        if (_mapExpPaused || _mapStartTime == DateTime.MinValue) return;

        var elapsed = _mapElapsedBeforePause + (DateTime.UtcNow - _mapStartTime);
        if (elapsed >= TimeSpan.FromMinutes(15))
        {
            if (_mapStartClass != null)
            {
                double rate = _mapExpGained / elapsed.TotalHours;
                _expRateMemory.AddRate(_trackedMapFile, _mapStartClass.Value, _mapStartLevel, rate);
            }

            _mapElapsedBeforePause = TimeSpan.Zero;
            _mapStartTime = DateTime.UtcNow;
            _mapStartExp = _experience;
            _mapExpGained = 0;
            _mapStartLevel = _level;
            _mapStartClass = _playerClass;
        }
    }

    private bool IsKnownMovementCell(Point loc)
    {
        if (string.IsNullOrEmpty(_currentMapFile)) return false;
        var map = Path.GetFileNameWithoutExtension(_currentMapFile);
        return _movementMemory.GetAll().Any(e => e.SourceMap == map && e.SourceX == loc.X && e.SourceY == loc.Y);
    }

    private bool IsOnKnownMovementCell() => IsKnownMovementCell(_currentLocation);

    private void CancelMovementDeleteCheck()
    {
        _movementDeleteCts?.Cancel();
        _movementDeleteCts = null;
    }

    private void MaybeStartMovementDeleteCheck()
    {
        if (_movementDeleteCts != null) return;
        if (!IsOnKnownMovementCell()) return;

        var map = _currentMapFile;
        var loc = _currentLocation;

        var cts = new CancellationTokenSource();
        _movementDeleteCts = cts;
        FireAndForget(Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cts.Token);
                if (!cts.IsCancellationRequested &&
                    string.Equals(_currentMapFile, map, StringComparison.OrdinalIgnoreCase) &&
                    _currentLocation == loc)
                {
                    _movementMemory.RemoveMovements(map, loc);
                    MovementEntryRemoved?.Invoke();
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                if (ReferenceEquals(_movementDeleteCts, cts))
                    _movementDeleteCts = null;
            }
        }));
    }

    public string? GetBestMapForLevel()
    {
        if (_playerClass == null) return null;
        var best = _expRateMemory.GetBestMapFile(_playerClass.Value, _level);
        if (!string.IsNullOrEmpty(best))
            return best;

        var maps = _movementMemory.GetKnownMaps();
        if (maps.Count == 0) return null;
        return maps[_random.Next(maps.Count)];
    }

    public string? GetRandomExplorationMap()
    {
        if (_playerClass == null) return null;

        var entries = _expRateMemory.GetAll();
        var allMaps = new HashSet<string>(entries.Select(e => e.MapFile));
        foreach (var m in _movementMemory.GetKnownMaps())
            allMaps.Add(m);

        var known = new HashSet<string>(entries
            .Where(e => e.Class == _playerClass.Value && e.Level == _level)
            .Select(e => e.MapFile));

        var candidates = allMaps.Where(m => !known.Contains(m)).ToList();
        if (candidates.Count == 0) return null;
        return candidates[_random.Next(candidates.Count)];
    }

    private Task RandomStartupDelayAsync() => Task.Delay(_random.Next(1000, 3000));

    private void MarkStatsDirty() => _statsDirty = true;

    private void RecalculateStats()
    {
        if (_playerClass == null)
        {
            _maxBagWeight = int.MaxValue;
            _maxHP = int.MaxValue;
            _maxMP = int.MaxValue;
            _statsDirty = false;
            return;
        }

        _baseStats ??= new BaseStats(_playerClass.Value);

        int baseWeight = _baseStats.Stats.First(s => s.Type == Stat.BagWeight).Calculate(_playerClass.Value, _level);
        int baseWearWeight = _baseStats.Stats.First(s => s.Type == Stat.WearWeight).Calculate(_playerClass.Value, _level);
        int baseHandWeight = _baseStats.Stats.First(s => s.Type == Stat.HandWeight).Calculate(_playerClass.Value, _level);
        int extraWeight = 0;
        int extraWearWeight = 0;
        int extraHandWeight = 0;
        int baseHP = _baseStats.Stats.First(s => s.Type == Stat.HP).Calculate(_playerClass.Value, _level);
       int extraHP = 0;
        int hpPercent = 0;
        int baseMP = _baseStats.Stats.First(s => s.Type == Stat.MP).Calculate(_playerClass.Value, _level);
        int extraMP = 0;
        int mpPercent = 0;

        if (_equipment != null)
        {
            foreach (var item in _equipment)
            {
                if (item == null || item.Info == null) continue;
                extraWeight += item.Info.Stats[Stat.BagWeight];
                extraWeight += item.AddedStats[Stat.BagWeight];

                extraWearWeight += item.Info.Stats[Stat.WearWeight];
                extraWearWeight += item.AddedStats[Stat.WearWeight];
                extraHandWeight += item.Info.Stats[Stat.HandWeight];
                extraHandWeight += item.AddedStats[Stat.HandWeight];

                extraHP += item.Info.Stats[Stat.HP];
                extraHP += item.AddedStats[Stat.HP];
                hpPercent += item.Info.Stats[Stat.HPRatePercent];
                hpPercent += item.AddedStats[Stat.HPRatePercent];

                extraMP += item.Info.Stats[Stat.MP];
                extraMP += item.AddedStats[Stat.MP];
                mpPercent += item.Info.Stats[Stat.MPRatePercent];
                mpPercent += item.AddedStats[Stat.MPRatePercent];
            }
        }

        _maxBagWeight = baseWeight + extraWeight;
        _maxWearWeight = baseWearWeight + extraWearWeight;
        _maxHandWeight = baseHandWeight + extraHandWeight;

        _maxHP = baseHP + extraHP;
        if (hpPercent != 0)
            _maxHP += (_maxHP * hpPercent) / 100;

        _maxMP = baseMP + extraMP;
        if (mpPercent != 0)
            _maxMP += (_maxMP * mpPercent) / 100;

        _statsDirty = false;
    }

    public int GetCurrentBagWeight()
    {
        int weight = 0;
        if (_inventory != null)
        {
            foreach (var item in _inventory)
                if (item != null)
                    weight += item.Weight;
        }
        return weight;
    }

    public int GetCurrentWearWeight()
    {
        int weight = 0;
        if (_equipment != null)
        {
            foreach (var item in _equipment)
            {
                if (item?.Info == null) continue;
                if (item.Info.Type == ItemType.Weapon) continue;
                weight += item.Weight;
            }
        }
        return weight;
    }

    public int GetCurrentHandWeight()
    {
        int weight = 0;
        if (_equipment != null)
        {
            foreach (var item in _equipment)
            {
                if (item?.Info == null) continue;
                if (item.Info.Type == ItemType.Weapon)
                    weight += item.Weight;
            }
        }
        return weight;
    }

    public int GetMaxBagWeight()
    {
        if (_statsDirty) RecalculateStats();
        return _maxBagWeight;
    }

    public int GetMaxWearWeight()
    {
        if (_statsDirty) RecalculateStats();
        return _maxWearWeight;
    }

    public int GetMaxHandWeight()
    {
        if (_statsDirty) RecalculateStats();
        return _maxHandWeight;
    }

    public int GetMaxHP()
    {
        if (_statsDirty) RecalculateStats();
        return _maxHP;
    }

    public int GetMaxMP()
    {
        if (_statsDirty) RecalculateStats();
        return _maxMP;
    }

    public bool HasFreeBagSpace()
    {
        if (_inventory == null) return true;
        for (int i = 0; i < _inventory.Length; i++)
            if (_inventory[i] == null) return true;
        return false;
    }

    public UserItem? FindPotion(bool hpPotion)
    {
        if (_inventory == null) return null;
        foreach (var item in _inventory)
        {
            if (item?.Info == null) continue;
            if (item.Info.Type != ItemType.Potion) continue;

            bool healsHP = item.Info.Stats[Stat.HP] > 0 || item.Info.Stats[Stat.HPRatePercent] > 0;
            bool healsMP = item.Info.Stats[Stat.MP] > 0 || item.Info.Stats[Stat.MPRatePercent] > 0;

            if (hpPotion && healsHP) return item;
            if (!hpPotion && healsMP) return item;
        }
        return null;
    }

    public UserItem? FindTownTeleport()
    {
        if (_inventory == null) return null;
        foreach (var item in _inventory)
        {
            if (item?.Info == null) continue;
            if (item.Info.Type != ItemType.Scroll) continue;
            if (item.Info.Shape == 1) return item;
        }
        return null;
    }

    public int GetPotionRestoreAmount(UserItem item, bool hpPotion)
    {
        int max = hpPotion ? GetMaxHP() : GetMaxMP();
        int flat = item.GetTotal(hpPotion ? Stat.HP : Stat.MP);
        int percent = item.GetTotal(hpPotion ? Stat.HPRatePercent : Stat.MPRatePercent);
        return flat + (max * percent) / 100;
    }

    public int GetStatTotal(Stat stat)
    {
        int total = 0;

        if (_playerClass != null)
        {
            _baseStats ??= new BaseStats(_playerClass.Value);
            var baseStat = _baseStats.Stats.FirstOrDefault(s => s.Type == stat);
            if (baseStat != null)
                total += baseStat.Calculate(_playerClass.Value, _level);
        }

        if (_equipment != null)
        {
            foreach (var item in _equipment)
            {
                if (item == null || item.Info == null) continue;
                total += item.Info.Stats[stat];
                total += item.AddedStats[stat];
            }
        }

        return total;
    }

    private async Task HarvestLoopAsync(TrackedObject monster)
    {
        _awaitingHarvest = true;
        _harvestTargetId = monster.Id;
        _harvestComplete = false;

        while (!_harvestComplete)
        {
            if (!HasFreeBagSpace() || GetCurrentBagWeight() >= GetMaxBagWeight())
                break;

            if (_trackedObjects.Values.Any(o => o.Type == ObjectType.Monster && !o.Dead &&
                Functions.MaxDistance(_currentLocation, o.Location) <= 1))
            {
                await Task.Delay(HarvestDelay);
                continue;
            }

            if (DateTime.UtcNow < _nextHarvestTime)
                await Task.Delay(_nextHarvestTime - DateTime.UtcNow);

            var dir = Functions.DirectionFromPoint(_currentLocation, monster.Location);
            await HarvestAsync(dir);
            _nextHarvestTime = DateTime.UtcNow + HarvestDelay;

            await Task.Delay(HarvestDelay);
        }

        _awaitingHarvest = false;
        _harvestTargetId = null;
    }

    private async Task DetermineSellTypesAsync(NpcEntry entry)
    {
        if (_inventory == null) return;
        var seen = new HashSet<ItemType>();
        if (entry.SellItemTypes != null) seen.UnionWith(entry.SellItemTypes);
        if (entry.CannotSellItemTypes != null) seen.UnionWith(entry.CannotSellItemTypes);
        foreach (var item in _inventory)
        {
            if (item == null || item.Info == null) continue;
            if (seen.Contains(item.Info.Type)) continue;
            seen.Add(item.Info.Type);
            _pendingSellChecks[item.UniqueID] = (entry, item.Info.Type);
            Log($"I am selling {item.Info.FriendlyName} to {entry.Name}");
            using var cts = new CancellationTokenSource(2000);
            var waitTask = WaitForSellItemAsync(item.UniqueID, cts.Token);
            await SendAsync(new C.SellItem { UniqueID = item.UniqueID, Count = 1 });
            try
            {
                await waitTask;
            }
            catch (OperationCanceledException)
            {
            }
            await Task.Delay(200);
        }
    }

    private async Task DetermineRepairTypesAsync(NpcEntry entry, bool special = false)
    {
        if (_inventory == null && _equipment == null) return;
        var seen = new HashSet<ItemType>();
        if (special)
        {
            if (entry.SpecialRepairItemTypes != null) seen.UnionWith(entry.SpecialRepairItemTypes);
            if (entry.CannotSpecialRepairItemTypes != null) seen.UnionWith(entry.CannotSpecialRepairItemTypes);
        }
        else
        {
            if (entry.RepairItemTypes != null) seen.UnionWith(entry.RepairItemTypes);
            if (entry.CannotRepairItemTypes != null) seen.UnionWith(entry.CannotRepairItemTypes);
        }

        var items = new List<(UserItem item, EquipmentSlot? slot)>();

        if (_inventory != null)
        {
            foreach (var item in _inventory)
            {
                if (item == null) continue;
                items.Add((item, null));
            }
        }

        if (_equipment != null)
        {
            for (int i = 0; i < _equipment.Length; i++)
            {
                var item = _equipment[i];
                if (item == null) continue;
                items.Add((item, (EquipmentSlot)i));
            }
        }

        foreach (var (item, slot) in items)
        {
            if (item == null || item.Info == null) continue;
            if (item.CurrentDura == item.MaxDura) continue;
            if (seen.Contains(item.Info.Type)) continue;
            seen.Add(item.Info.Type);
            _pendingRepairChecks[item.UniqueID] = (entry, item.Info.Type);
            Log($"I am {(special ? "special repairing" : "repairing")} {item.Info.FriendlyName} at {entry.Name}");
            using var cts = new CancellationTokenSource(2000);
            var waitTask = WaitForRepairItemAsync(item.UniqueID, cts.Token);
            try
            {
                if (slot.HasValue)
                {
                    await UnequipItemAsync(slot.Value);
                    await Task.Delay(200);
                }

                if (special)
                    await SendAsync(new C.SRepairItem { UniqueID = item.UniqueID });
                else
                    await SendAsync(new C.RepairItem { UniqueID = item.UniqueID });
                var success = await waitTask;
                if (success)
                {
                    if (special)
                    {
                        entry.SpecialRepairItemTypes ??= new List<ItemType>();
                        if (!entry.SpecialRepairItemTypes.Contains(item.Info.Type))
                        {
                            entry.SpecialRepairItemTypes.Add(item.Info.Type);
                            _npcMemory.SaveChanges();
                        }
                    }
                    else
                    {
                        entry.RepairItemTypes ??= new List<ItemType>();
                        if (!entry.RepairItemTypes.Contains(item.Info.Type))
                        {
                            entry.RepairItemTypes.Add(item.Info.Type);
                            _npcMemory.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (special)
                    {
                        entry.CannotSpecialRepairItemTypes ??= new List<ItemType>();
                        if (!entry.CannotSpecialRepairItemTypes.Contains(item.Info.Type))
                        {
                            entry.CannotSpecialRepairItemTypes.Add(item.Info.Type);
                            _npcMemory.SaveChanges();
                        }
                    }
                    else
                    {
                        entry.CannotRepairItemTypes ??= new List<ItemType>();
                        if (!entry.CannotRepairItemTypes.Contains(item.Info.Type))
                        {
                            entry.CannotRepairItemTypes.Add(item.Info.Type);
                            _npcMemory.SaveChanges();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (slot.HasValue)
                {
                    await EquipItemAsync(item, slot.Value);
                    await Task.Delay(200);
                }
            }
            await Task.Delay(200);
        }
    }

    private async Task RepairNeededItemsAsync(NpcEntry entry, bool special = false)
    {
        if (_inventory == null || _equipment == null) return;
        var repairList = special ? entry.SpecialRepairItemTypes : entry.RepairItemTypes;
        if (repairList == null || repairList.Count == 0) return;

        var items = new List<(UserItem item, EquipmentSlot? slot)>();

        for (int i = 0; i < _equipment.Length; i++)
        {
            var item = _equipment[i];
            if (item?.Info == null) continue;
            if (item.CurrentDura == item.MaxDura) continue;
            if (!repairList.Contains(item.Info.Type)) continue;
            items.Add((item, (EquipmentSlot)i));
        }

        foreach (var (item, slot) in items)
        {
            if (slot.HasValue)
            {
                await UnequipItemAsync(slot.Value);
                await Task.Delay(200);
            }

            Log($"I am {(special ? "special repairing" : "repairing")} {item.Info?.FriendlyName ?? "item"} at {entry.Name}");
            using var cts = new CancellationTokenSource(2000);
            var waitTask = WaitForRepairItemAsync(item.UniqueID, cts.Token);
            try
            {
                if (special)
                    await SendAsync(new C.SRepairItem { UniqueID = item.UniqueID });
                else
                    await SendAsync(new C.RepairItem { UniqueID = item.UniqueID });
                await waitTask;
            }
            catch (OperationCanceledException)
            {
            }
            await Task.Delay(200);

            if (slot.HasValue)
            {
                await EquipItemAsync(item, slot.Value);
                await Task.Delay(200);
            }
        }
    }

    private bool HasUnknownSellTypes(NpcEntry entry)
    {
        if (_inventory == null) return false;
        var seen = new HashSet<ItemType>();
        if (entry.SellItemTypes != null) seen.UnionWith(entry.SellItemTypes);
        if (entry.CannotSellItemTypes != null) seen.UnionWith(entry.CannotSellItemTypes);
        foreach (var item in _inventory)
        {
            if (item?.Info == null) continue;
            if (!seen.Contains(item.Info.Type))
                return true;
        }
        return false;
    }

    private bool HasUnknownRepairTypes(NpcEntry entry, bool special = false)
    {
        if (_inventory == null && _equipment == null) return false;
        var seen = new HashSet<ItemType>();
        if (special)
        {
            if (entry.SpecialRepairItemTypes != null) seen.UnionWith(entry.SpecialRepairItemTypes);
            if (entry.CannotSpecialRepairItemTypes != null) seen.UnionWith(entry.CannotSpecialRepairItemTypes);
        }
        else
        {
            if (entry.RepairItemTypes != null) seen.UnionWith(entry.RepairItemTypes);
            if (entry.CannotRepairItemTypes != null) seen.UnionWith(entry.CannotRepairItemTypes);
        }

        IEnumerable<UserItem?> items = _inventory ?? Array.Empty<UserItem?>();
        if (_equipment != null)
            items = items.Concat(_equipment);

        foreach (var item in items)
        {
            if (item?.Info == null) continue;
            if (item.CurrentDura == item.MaxDura) continue;
            if (!seen.Contains(item.Info.Type))
                return true;
        }
        return false;
    }

    private bool NeedsNpcInteraction(NpcEntry entry)
    {
        if (!entry.CheckedMerchantKeys)
            return true;
        if (entry.CanBuy)
            return true;
        if (entry.CanSell && HasUnknownSellTypes(entry))
            return true;
        if (entry.CanRepair && HasUnknownRepairTypes(entry))
            return true;
        if (entry.CanSpecialRepair && HasUnknownRepairTypes(entry, true))
            return true;
        return false;
    }

    private async Task HandleNpcSellAsync(NpcEntry entry)
    {
        await DetermineSellTypesAsync(entry);
        _npcSellTcs?.TrySetResult(true);
        _npcSellTcs = null;
        ProcessNpcActionQueue();
    }

    private async Task HandleNpcRepairAsync(NpcEntry entry, bool special = false)
    {
        await DetermineRepairTypesAsync(entry, special);
        _npcRepairTcs?.TrySetResult(true);
        _npcRepairTcs = null;
        ProcessNpcActionQueue();
    }

    private void ProcessNpcGoods(IEnumerable<UserItem> goods, PanelType type)
    {
        if (!_dialogNpcId.HasValue) return;
        if (!_npcEntries.TryGetValue(_dialogNpcId.Value, out var entry)) return;

        if (_skipNextGoods)
        {
            _skipNextGoods = false;
            return;
        }

        if (type != PanelType.Buy && type != PanelType.BuySub)
            return;

        _lastNpcGoods = goods.Select(g =>
        {
            Bind(g);
            return g;
        }).ToList();
        _lastNpcGoodsType = type;

        entry.CanBuy = true;
        entry.BuyItemIndexes ??= new List<int>();
        foreach (var it in _lastNpcGoods)
        {
            int index = it.Info?.Index ?? it.ItemIndex;
            if (!entry.BuyItemIndexes.Contains(index))
                entry.BuyItemIndexes.Add(index);
        }

        _npcMemory.SaveChanges();
        _npcGoodsTcs?.TrySetResult(true);
        _npcGoodsTcs = null;
    }

    private void TryFinishNpcInteraction()
    {
        if (_dialogNpcId.HasValue &&
            _pendingSellChecks.Count == 0 &&
            _pendingRepairChecks.Count == 0 &&
            _npcActionTasks.Count == 0 &&
            !_processingNpcAction)
        {
            _dialogNpcId = null;
            _npcInteraction = null;
            ProcessNextNpcInQueue();
        }
    }

    private void ProcessNextNpcInQueue()
    {
        if (IgnoreNpcInteractions || _movementSaveCts != null) return;
        while (_npcQueue.Count > 0)
        {
            var id = _npcQueue.Dequeue();
            if (_npcEntries.TryGetValue(id, out var entry))
            {
                StartNpcInteraction(id, entry);
                break;
            }
        }
    }

    private async void ProcessNpcActionQueue()
    {
        if (_processingNpcAction || !_dialogNpcId.HasValue || _npcInteraction == null) return;
        if (_pendingSellChecks.Count > 0 || _pendingRepairChecks.Count > 0) return;

        if (_npcActionTasks.Count == 0)
        {
            TryFinishNpcInteraction();
            return;
        }

        var item = _npcActionTasks.Dequeue();
        _processingNpcAction = true;
        await item.action();
    }

    private async void StartNpcInteraction(uint id, NpcEntry entry)
    {
        _dialogNpcId = id;
        _npcInteractionStart = DateTime.UtcNow;
        _npcActionTasks.Clear();
        _processingNpcAction = false;
        _recentNpcInteractions[(entry.Name, entry.MapFile, entry.X, entry.Y)] = DateTime.UtcNow;
        Log($"I am speaking with NPC {entry.Name}");
        _npcInteraction = new NPCInteraction(this, id);
        var page = await _npcInteraction.BeginAsync();
        HandleNpcDialogPage(page, entry);
    }

    private Func<Task> CreateBuyTask(string key) => async () =>
    {
        if (_npcInteraction == null) return;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var waitTask = WaitForNpcGoodsAsync(cts.Token);
        try
        {
            if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var entry))
                Log($"I am looking at {entry.Name}'s goods list");
            await _npcInteraction.SelectFromMainAsync(key);
            await waitTask;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _processingNpcAction = false;
            ProcessNpcActionQueue();
        }
    };

    private Func<Task> CreateSellTask(string key) => async () =>
    {
        if (_npcInteraction == null) return;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var waitTask = WaitForNpcSellAsync(cts.Token);
        try
        {
            await _npcInteraction.SelectFromMainAsync(key);
            await waitTask;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _processingNpcAction = false;
            ProcessNpcActionQueue();
        }
    };

    private Func<Task> CreateRepairTask(string key) => async () =>
    {
        if (_npcInteraction == null) return;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var waitTask = WaitForNpcRepairAsync(cts.Token);
        try
        {
            await _npcInteraction.SelectFromMainAsync(key);
            await waitTask;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _processingNpcAction = false;
            ProcessNpcActionQueue();
        }
    };

    private Func<Task> CreateCheckBuyTask(string key) => async () =>
    {
        if (_npcInteraction == null) return;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var waitTask = WaitForNpcGoodsAsync(cts.Token);
        try
        {
            if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var entry))
                Log($"I am looking at {entry.Name}'s goods list");
            await _npcInteraction.SelectFromMainAsync(key);
            await waitTask;
            if (_lastNpcGoods != null)
                await BuyNeededItemsFromGoodsAsync(_lastNpcGoods, _lastNpcGoodsType);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _processingNpcAction = false;
            ProcessNpcActionQueue();
        }
    };

    private Func<Task> CreateCheckRepairTask(string key) => async () =>
    {
        if (_npcInteraction == null) return;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var waitTask = WaitForNpcRepairAsync(cts.Token);
        try
        {
            await _npcInteraction.SelectFromMainAsync(key);
            await waitTask;
            if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var entry))
            {
                bool special = key.Equals("@SREPAIR", StringComparison.OrdinalIgnoreCase);
                await RepairNeededItemsAsync(entry, special);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _processingNpcAction = false;
            ProcessNpcActionQueue();
        }
    };

    private void HandleNpcDialogPage(NpcDialogPage page, NpcEntry entry)
    {
        var keyList = page.Buttons.Select(b => b.Key).ToList();
        var keys = new HashSet<string>(keyList.Select(k => k.ToUpper()));

        bool changed = false;
        bool needBuyCheck = false;
        bool needSellCheck = false;
        bool needRepairCheck = false;

        bool hasBuy = keys.Overlaps(new[] { "@BUY", "@BUYSELL", "@BUYNEW", "@BUYSELLNEW", "@PEARLBUY" });
        bool hasSell = keys.Overlaps(new[] { "@SELL", "@BUYSELL", "@BUYSELLNEW" });
        bool hasRepair = keys.Overlaps(new[] { "@REPAIR", "@SREPAIR" });

        string? buyKey = null;
        string? sellKey = null;
        string? repairKey = null;

        if (hasBuy)
        {
            if (!entry.CanBuy)
            {
                entry.CanBuy = true;
                changed = true;
            }
            string[] buyKeys = { "@BUYSELLNEW", "@BUYSELL", "@BUYNEW", "@PEARLBUY", "@BUY" };
            buyKey = keyList.FirstOrDefault(k => buyKeys.Contains(k.ToUpper())) ?? "@BUY";
            if (entry.BuyItemIndexes == null)
            {
                needBuyCheck = true;
                if (buyKey.Equals("@BUYBACK", StringComparison.OrdinalIgnoreCase))
                {
                    _skipNextGoods = true;
                    buyKey = null;
                }
            }
        }

        if (hasSell)
        {
            if (!entry.CanSell)
            {
                entry.CanSell = true;
                changed = true;
            }
            needSellCheck = HasUnknownSellTypes(entry);
            if (needSellCheck)
            {
                string[] sellKeys = { "@BUYSELLNEW", "@BUYSELL", "@SELL" };
                sellKey = keyList.FirstOrDefault(k => sellKeys.Contains(k.ToUpper())) ?? "@SELL";
                if (sellKey.Equals("@BUYBACK", StringComparison.OrdinalIgnoreCase))
                {
                    _skipNextGoods = true;
                    sellKey = null;
                }
            }
        }

        if (hasRepair)
        {
            if (!entry.CanRepair)
            {
                entry.CanRepair = true;
                changed = true;
            }
            string[] repairKeys = { "@SREPAIR", "@REPAIR" };
            repairKey = keyList.FirstOrDefault(k => repairKeys.Contains(k.ToUpper())) ?? "@REPAIR";
            bool specialRepair = repairKey.Equals("@SREPAIR", StringComparison.OrdinalIgnoreCase);
            if (specialRepair && !entry.CanSpecialRepair)
            {
                entry.CanSpecialRepair = true;
                changed = true;
            }
            needRepairCheck = HasUnknownRepairTypes(entry, specialRepair);
            if (needRepairCheck)
            {
                if (repairKey.Equals("@BUYBACK", StringComparison.OrdinalIgnoreCase))
                {
                    _skipNextGoods = true;
                    repairKey = null;
                }
            }
        }

        if (needBuyCheck && buyKey != null)
        {
            _npcActionTasks.Enqueue((buyKey, CreateBuyTask(buyKey)));
        }
        if (sellKey != null)
        {
            _npcActionTasks.Enqueue((sellKey, CreateSellTask(sellKey)));
        }
        if (repairKey != null)
        {
            _npcActionTasks.Enqueue((repairKey, CreateRepairTask(repairKey)));
            _npcActionTasks.Enqueue((repairKey, CreateCheckRepairTask(repairKey)));
        }
        if (buyKey != null)
        {
            _npcActionTasks.Enqueue((buyKey, CreateCheckBuyTask(buyKey)));
        }

        if (!entry.CheckedMerchantKeys)
        {
            entry.CheckedMerchantKeys = true;
            changed = true;
        }

        if (changed)
            _npcMemory.SaveChanges();

        ProcessNpcActionQueue();
    }

    private void CheckNpcInteractionTimeout()
    {
        if (_dialogNpcId.HasValue &&
            DateTime.UtcNow - _npcInteractionStart > TimeSpan.FromSeconds(10))
        {
            _dialogNpcId = null;
            _pendingSellChecks.Clear();
            _pendingRepairChecks.Clear();
            ProcessNextNpcInQueue();
        }
    }

    public void ResumeNpcInteractions()
    {
        ProcessNextNpcInQueue();
    }

    private static void FireAndForget(Task task)
    {
        task.ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
    }
}
