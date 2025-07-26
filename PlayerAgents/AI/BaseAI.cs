using ClientPackets;
using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlayerAgents.Map;

public class BaseAI
{
    protected readonly GameClient Client;
    protected static readonly Random Random = new();
    private TrackedObject? _currentTarget;
    private Point? _searchDestination;
    private Point? _lostTargetLocation;
    private DateTime _nextTargetSwitchTime = DateTime.MinValue;
    private List<Point>? _currentRoamPath;
    private List<Point>? _lostTargetPath;
    private DateTime _nextPathFindTime = DateTime.MinValue;


    protected virtual TimeSpan TargetSwitchInterval => TimeSpan.FromSeconds(3);
    // Using HashSet for faster Contains checks
    protected static readonly HashSet<EquipmentSlot> OffensiveSlots = new()
    {
        EquipmentSlot.Weapon,
        EquipmentSlot.Necklace,
        EquipmentSlot.RingL,
        EquipmentSlot.RingR
    };

    // Monsters with these AI values are ignored when selecting a target
    protected static readonly HashSet<byte> IgnoredAIs = new() { 6, 58, 57, 56, 64 };

    protected static bool IsOffensiveSlot(EquipmentSlot slot) => OffensiveSlots.Contains(slot);

    public BaseAI(GameClient client)
    {
        Client = client;
        Client.ItemScoreFunc = GetItemScore;
        Client.DesiredItemsProvider = () => DesiredItems;
        Client.MovementEntryRemoved += OnMovementEntryRemoved;
    }

    private void OnMovementEntryRemoved()
    {
        _travelPath = null;
        _currentRoamPath = null;
        _nextPathFindTime = DateTime.MinValue;
    }

    protected virtual int WalkDelay => 600;
    protected virtual int AttackDelay => 1400;
    protected virtual TimeSpan RoamPathFindInterval => TimeSpan.FromSeconds(2);
    protected virtual TimeSpan FailedPathFindDelay => TimeSpan.FromSeconds(5);
    protected virtual TimeSpan EquipCheckInterval => TimeSpan.FromSeconds(5);
    protected virtual IReadOnlyList<DesiredItem> DesiredItems => Array.Empty<DesiredItem>();
    private DateTime _nextEquipCheck = DateTime.UtcNow;
    private DateTime _nextAttackTime = DateTime.UtcNow;
    private DateTime _nextPotionTime = DateTime.MinValue;
    private DateTime _nextTownTeleportTime = DateTime.MinValue;
    private DateTime _nextBestMapCheck = DateTime.MinValue;
    private string? _currentBestMap;
    private DateTime _travelPauseUntil = DateTime.MinValue;
    private List<MapMovementEntry>? _travelPath;
    private int _travelIndex;
    private DateTime _stationarySince = DateTime.MinValue;
    private Point _lastStationaryLocation = Point.Empty;
    private DateTime _travelStuckSince = DateTime.MinValue;
    private DateTime _lastMoveOrAttackTime = DateTime.MinValue;
    
    private readonly Dictionary<(Point Location, string Name), DateTime> _itemRetryTimes = new();
    private readonly Dictionary<uint, DateTime> _monsterIgnoreTimes = new();
    private static readonly TimeSpan ItemRetryDelay = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan DroppedItemRetryDelay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MonsterIgnoreDelay = TimeSpan.FromSeconds(10);
    private bool _sentRevive;
    private bool _sellingItems;
    private bool _repairingItems;

    protected virtual int GetItemScore(UserItem item, EquipmentSlot slot)
    {
        int score = 0;
        if (item.Info != null)
            score += item.Info.Stats.Count;
        if (item.AddedStats != null)
            score += item.AddedStats.Count;
        return score;
    }

    private Point GetRandomPoint(PlayerAgents.Map.MapData map, Random random, Point origin, int radius)
    {
        var obstacles = BuildObstacles();
        var nav = Client.NavData;
        const int attempts = 20;
        if (nav != null)
        {
            int r = radius;
            if (radius > 0 && random.Next(5) == 0)
                r = 0; // occasionally roam anywhere using full nav data

            for (int i = 0; i < attempts; i++)
            {
                if (!nav.TryGetRandomCell(random, origin, r, out var navPoint))
                    break;
                if (!obstacles.Contains(navPoint))
                    return navPoint;
            }
        }

        var cells = map.WalkableCells;
        if (cells.Count > 0)
        {
            if (radius > 0)
            {
                var subset = cells.Where(c => Functions.MaxDistance(c, origin) <= radius && !obstacles.Contains(c)).ToList();
                if (subset.Count > 0)
                    return subset[random.Next(subset.Count)];
            }
            var free = cells.Where(c => !obstacles.Contains(c)).ToList();
            if (free.Count > 0)
                return free[random.Next(free.Count)];
        }

        // Fallback for maps without walk data
        int width = Math.Max(map.Width, 1);
        int height = Math.Max(map.Height, 1);
        for (int i = 0; i < attempts; i++)
        {
            int x = Math.Clamp(origin.X + random.Next(-10, 11), 0, width - 1);
            int y = Math.Clamp(origin.Y + random.Next(-10, 11), 0, height - 1);
            if (map.IsWalkable(x, y))
            {
                var p = new Point(x, y);
                if (!obstacles.Contains(p))
                    return p;
            }
        }
        int fx = Math.Clamp(origin.X + random.Next(-10, 11), 0, width - 1);
        int fy = Math.Clamp(origin.Y + random.Next(-10, 11), 0, height - 1);
        return new Point(fx, fy);
    }

    private UserItem? GetBestItemForSlot(EquipmentSlot slot, IEnumerable<UserItem?> inventory, UserItem? current)
    {
        int bestScore = current != null ? GetItemScore(current, slot) : -1;
        UserItem? bestItem = current;
        foreach (var item in inventory)
        {
            if (item == null) continue;
            if (!Client.CanEquipItem(item, slot)) continue;
            int score = GetItemScore(item, slot);
            if (bestItem == null || score > bestScore)
            {
                bestItem = item;
                bestScore = score;
            }
        }
        return bestItem;
    }

    private async Task CheckEquipmentAsync()
    {
        var inventory = Client.Inventory;
        var equipment = Client.Equipment;
        if (inventory == null || equipment == null) return;

        // create a mutable copy so we can mark equipped items as used
        var available = inventory.ToList();

        for (int slot = 0; slot < equipment.Count; slot++)
        {
            var equipSlot = (EquipmentSlot)slot;
            UserItem? current = equipment[slot];
            UserItem? bestItem = GetBestItemForSlot(equipSlot, available, current);

            if (bestItem != null && bestItem != current)
            {
                await Client.EquipItemAsync(bestItem, equipSlot);
                int idx = available.IndexOf(bestItem);
                if (idx >= 0) available[idx] = null; // prevent using same item twice
                if (bestItem.Info != null)
                    Client.Log($"I have equipped {bestItem.Info.FriendlyName}");
            }
        }
    }

    private async Task TryUsePotionsAsync()
    {
        if (DateTime.UtcNow < _nextPotionTime) return;

        int maxHP = Client.GetMaxHP();
        int maxMP = Client.GetMaxMP();

        if (Client.HP < maxHP)
        {
            var pot = Client.FindPotion(true);
            double hpPercent = (double)Client.HP / maxHP;
            if (pot != null)
            {
                int heal = Client.GetPotionRestoreAmount(pot, true);
                if (heal > 0 && (maxHP - Client.HP >= heal || hpPercent <= 0.10))
                {
                    await Client.UseItemAsync(pot);
                    string name = pot.Info?.FriendlyName ?? "HP potion";
                    Client.Log($"Used {name}");
                    _nextPotionTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
                    return;
                }
            }
            else if (DateTime.UtcNow >= _nextTownTeleportTime && hpPercent <= 0.10)
            {
                var teleport = Client.FindTownTeleport();
                if (teleport != null)
                {
                    await Client.UseItemAsync(teleport);
                    string name = teleport.Info?.FriendlyName ?? "town teleport";
                    Client.Log($"Used {name}");
                    _nextPotionTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
                    _nextTownTeleportTime = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                    return;
                }
            }
        }

        if (Client.MP < maxMP)
        {
            var pot = Client.FindPotion(false);
            if (pot != null)
            {
                int heal = Client.GetPotionRestoreAmount(pot, false);
                double mpPercent = (double)Client.MP / maxMP;
                if (heal > 0 && (maxMP - Client.MP >= heal || mpPercent <= 0.10))
                {
                    await Client.UseItemAsync(pot);
                    string name = pot.Info?.FriendlyName ?? "MP potion";
                    Client.Log($"Used {name}");
                    _nextPotionTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
                }
            }
        }
    }

    private TrackedObject? FindClosestTarget(Point current, out int bestDist)
    {
        TrackedObject? closestMonster = null;
        int monsterDist = int.MaxValue;
        TrackedObject? closestItem = null;
        int itemDist = int.MaxValue;

        foreach (var obj in Client.TrackedObjects.Values)
        {
            if (obj.Type == ObjectType.Monster)
            {
                if (_monsterIgnoreTimes.TryGetValue(obj.Id, out var ignore) && DateTime.UtcNow < ignore) continue;
                if (obj.Dead) continue;
                if (IgnoredAIs.Contains(obj.AI)) continue;
                // previously ignored monsters that were recently engaged with another player
                // now we attempt to attack them unless we cannot reach them
                int dist = Functions.MaxDistance(current, obj.Location);
                if (dist < monsterDist)
                {
                    monsterDist = dist;
                    closestMonster = obj;
                }
            }
            else if (obj.Type == ObjectType.Item)
            {
                if (_itemRetryTimes.TryGetValue((obj.Location, obj.Name), out var retry) && DateTime.UtcNow < retry)
                    continue;
                int dist = Functions.MaxDistance(current, obj.Location);
                if (dist < itemDist)
                {
                    itemDist = dist;
                    closestItem = obj;
                }
            }
        }

        // Prioritize adjacent monsters
        if (closestMonster != null && monsterDist <= 1)
        {
            bestDist = monsterDist;
            return closestMonster;
        }

        // choose nearest between remaining options
        if (closestMonster != null && (closestItem == null || monsterDist <= itemDist))
        {
            bestDist = monsterDist;
            return closestMonster;
        }

        if (closestItem != null)
        {
            bool isGold = string.Equals(closestItem.Name, "Gold", StringComparison.OrdinalIgnoreCase);
            if (isGold || (Client.HasFreeBagSpace() && Client.GetCurrentBagWeight() < Client.GetMaxBagWeight()))
            {
                bestDist = itemDist;
                return closestItem;
            }
        }

        bestDist = int.MaxValue;
        return null;
    }

    private HashSet<Point> BuildObstacles(uint ignoreId = 0, int radius = 1)
    {
        var obstacles = new HashSet<Point>(Client.BlockingCells);
        if (ignoreId != 0 && Client.TrackedObjects.TryGetValue(ignoreId, out var obj))
            obstacles.Remove(obj.Location);

        if (radius > 0 && !string.IsNullOrEmpty(Client.CurrentMapFile))
        {
            var current = Path.GetFileNameWithoutExtension(Client.CurrentMapFile);
            foreach (var entry in Client.MovementMemory.GetAll())
            {
                if (entry.SourceMap == current)
                    obstacles.Add(new Point(entry.SourceX, entry.SourceY));
            }
        }
        return obstacles;
    }

    private async Task<List<Point>> FindPathAsync(PlayerAgents.Map.MapData map, Point start, Point dest, uint ignoreId = 0, int radius = 1)
    {
        try
        {
            var obstacles = BuildObstacles(ignoreId, radius);
            return await PlayerAgents.Map.PathFinder.FindPathAsync(map, start, dest, obstacles, radius);
        }
        catch
        {
            return new List<Point>();
        }
    }

    private async Task<bool> MoveAlongPathAsync(List<Point> path, Point destination)
    {
        if (path.Count <= 1) return true;
        if (Client.MovementSavePending) return false;

        var current = Client.CurrentLocation;

        if (path.Count > 2)
        {
            var next = path[1];
            var dir = Functions.DirectionFromPoint(current, next);
            if (Functions.PointMove(current, dir, 2) == path[2] && Client.CanWalk(dir))
            {
                await Client.WalkAsync(dir);
                _lastMoveOrAttackTime = DateTime.UtcNow;
                path.RemoveRange(0, 2);
                return true;
            }
        }

        if (path.Count > 1)
        {
            var dir = Functions.DirectionFromPoint(current, path[1]);
            if (Client.CanWalk(dir))
            {
                await Client.WalkAsync(dir);
                _lastMoveOrAttackTime = DateTime.UtcNow;
                path.RemoveAt(0);
                return true;
            }
        }
        else
        {
            var dir = Functions.DirectionFromPoint(current, destination);
            if (Client.CanWalk(dir))
            {
                await Client.WalkAsync(dir);
                _lastMoveOrAttackTime = DateTime.UtcNow;
                return true;
            }
        }

        return false;
    }

    private Task<bool> TravelToMapAsync(string destMapFile)
    {
        if (DateTime.UtcNow < _travelPauseUntil)
            return Task.FromResult(false);

        var startMap = Path.GetFileNameWithoutExtension(Client.CurrentMapFile);
        var destMap = Path.GetFileNameWithoutExtension(destMapFile);
        if (startMap == destMap)
        {
            _travelPath = null;
            _searchDestination = null;
            return Task.FromResult(true);
        }

        var entries = Client.MovementMemory.GetAll()
            .Where(e => e.SourceMap != e.DestinationMap)
            .ToList();
        var queue = new Queue<(string Map, List<MapMovementEntry> Path)>();
        var visited = new HashSet<string> { startMap };
        queue.Enqueue((startMap, new List<MapMovementEntry>()));
        List<MapMovementEntry>? path = null;

        while (queue.Count > 0)
        {
            var (map, soFar) = queue.Dequeue();
            if (map == destMap)
            {
                path = soFar;
                break;
            }

            foreach (var e in entries)
            {
                if (e.SourceMap != map) continue;
                if (visited.Contains(e.DestinationMap)) continue;
                visited.Add(e.DestinationMap);
                var newList = new List<MapMovementEntry>(soFar) { e };
                queue.Enqueue((e.DestinationMap, newList));
            }
        }

        if (path == null)
        {
            _travelPath = null;
            _searchDestination = null;
            _travelPauseUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            _nextBestMapCheck = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            return Task.FromResult(false);
        }

        _travelPath = path;
        _travelIndex = 0;
        UpdateTravelDestination();
        return Task.FromResult(true);
    }

    private void UpdateTravelDestination()
    {
        if (_travelPath == null) return;
        if (_travelIndex >= _travelPath.Count)
        {
            _travelPath = null;
            _searchDestination = null;
            Client.UpdateAction("roaming...");
            return;
        }

        string current = Path.GetFileNameWithoutExtension(Client.CurrentMapFile);
        var step = _travelPath[_travelIndex];

        if (current == step.DestinationMap)
        {
            _travelIndex++;
            if (_travelIndex >= _travelPath.Count)
            {
                _travelPath = null;
                _searchDestination = null;
                Client.UpdateAction("roaming...");
                return;
            }
            step = _travelPath[_travelIndex];
        }
        else if (current != step.SourceMap)
        {
            _travelPath = null;
            _searchDestination = null;
            Client.UpdateAction("roaming...");
            return;
        }

        var dest = new Point(step.SourceX, step.SourceY);
        if (_searchDestination == null || _searchDestination.Value != dest)
        {
            _searchDestination = dest;
            _currentRoamPath = null;
            _nextPathFindTime = DateTime.MinValue;
        }
    }

    private async Task ProcessBestMapAsync()
    {
        if (Client.IgnoreNpcInteractions || DateTime.UtcNow < _travelPauseUntil)
            return;

        if (DateTime.UtcNow >= _nextBestMapCheck)
        {
            _nextBestMapCheck = DateTime.UtcNow + TimeSpan.FromHours(1);
            if (Random.Next(5) == 0)
            {
                var explore = Client.GetRandomExplorationMap();
                if (!string.IsNullOrEmpty(explore))
                    _currentBestMap = explore;
            }
            else
            {
                _currentBestMap = Client.GetBestMapForLevel();
            }

            // force path recalculation if destination changes or interval lapses
            _travelPath = null;
        }

        if (_currentBestMap == null)
            return;

        var target = Path.Combine(MapManager.MapDirectory, _currentBestMap + ".map");
        if (!string.Equals(Client.CurrentMapFile, target, StringComparison.OrdinalIgnoreCase))
        {
            if (_travelPath == null)
            {
                Client.Log($"Travelling to best map {_currentBestMap}");
                if (!await TravelToMapAsync(target))
                {
                    _nextBestMapCheck = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                    return;
                }
                _currentRoamPath = null;
                _lostTargetLocation = null;
                _lostTargetPath = null;
                _currentTarget = null;
            }
        }
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

    private Dictionary<UserItem, ushort> GetItemKeepCounts(IEnumerable<UserItem> inventory)
    {
        var keep = new Dictionary<UserItem, ushort>();
        int maxWeight = Client.GetMaxBagWeight();

        // Keep any potential equipment upgrades
        var equipment = Client.Equipment;
        if (equipment != null)
        {
            // Create mutable list so each item can only fill a single slot
            var available = inventory.ToList();

            for (int slot = 0; slot < equipment.Count; slot++)
            {
                var equipSlot = (EquipmentSlot)slot;
                UserItem? current = equipment[slot];
                UserItem? bestItem = GetBestItemForSlot(equipSlot, available, current);

                if (bestItem != null && bestItem != current)
                {
                    keep[bestItem] = bestItem.Count;
                    int idx = available.IndexOf(bestItem);
                    if (idx >= 0) available[idx] = null; // don't reuse same item
                }
            }
        }

        // Keep desired items up to the configured quota
        foreach (var desired in DesiredItems)
        {
            var matching = inventory.Where(i => MatchesDesiredItem(i, desired)).ToList();

            if (desired.Count.HasValue)
            {
                int remaining = desired.Count.Value;
                foreach (var item in matching.OrderByDescending(i => i.Weight))
                {
                    if (remaining <= 0) break;
                    ushort already = keep.TryGetValue(item, out var val) ? val : (ushort)0;
                    int available = item.Count - already;
                    if (available <= 0) continue;
                    ushort amount = (ushort)Math.Min(available, remaining);
                    keep[item] = (ushort)(already + amount);
                    remaining -= amount;
                }
            }

            if (desired.WeightFraction > 0)
            {
                int requiredWeight = (int)Math.Ceiling(maxWeight * desired.WeightFraction);
                int current = matching.Sum(i =>
                {
                    ushort kept = keep.TryGetValue(i, out var val) ? val : (ushort)0;
                    return i.Info!.Weight * kept;
                });
                foreach (var item in matching.OrderByDescending(i => i.Weight))
                {
                    if (current >= requiredWeight) break;
                    ushort already = keep.TryGetValue(item, out var val) ? val : (ushort)0;
                    if (item.Info == null) continue;
                    int available = item.Count - already;
                    if (available <= 0) continue;
                    int weightPer = item.Info.Weight;
                    int needed = (requiredWeight - current + weightPer - 1) / weightPer;
                    int add = Math.Min(available, needed);
                    keep[item] = (ushort)(already + add);
                    current += add * weightPer;
                }
            }
        }

        return keep;
    }

    private async Task HandleInventoryAsync()
    {
        if (_sellingItems) return;
        var inventory = Client.Inventory;
        if (inventory == null) return;

        bool full = !Client.HasFreeBagSpace();
        bool heavy = Client.GetCurrentBagWeight() >= Client.GetMaxBagWeight() * 0.9;
        if (!full && !heavy) return;

        var items = inventory.Where(i => i != null && i.Info != null).ToList();
        var keepCounts = GetItemKeepCounts(items);
        var sellGroups = items
            .Select(i => {
                ushort keep = keepCounts.TryGetValue(i, out var k) ? k : (ushort)0;
                ushort sell = (ushort)Math.Max(i.Count - keep, 0);
                return (item: i, sell);
            })
            .Where(t => t.sell > 0)
            .GroupBy(t => t.item!.Info!.Type)
            .ToDictionary(g => g.Key, g => g.ToList());

        _sellingItems = true;
        Client.UpdateAction("selling items");
        Client.IgnoreNpcInteractions = true;
        while (sellGroups.Count > 0)
        {
            var types = sellGroups.Keys.ToList();
            if (!Client.TryFindNearestNpc(types, out var npcId, out var loc, out var entry, out var matchedTypes, includeUnknowns: false))
                break;

            int count = matchedTypes.Sum(t => sellGroups[t].Sum(x => x.sell));
            Client.Log($"Heading to {entry?.Name ?? "unknown npc"} at {loc.X},{loc.Y} to sell {count} items");

            var map = Client.CurrentMap;
            if (map == null) break;
            bool foundPath = true;
            while (Functions.MaxDistance(Client.CurrentLocation, loc) > 6)
            {
                var path = await FindPathAsync(map, Client.CurrentLocation, loc, npcId, 6);
                if (path.Count == 0)
                {
                    Client.Log($"Could not path to {entry?.Name ?? npcId.ToString()}");
                    foundPath = false;
                    break;
                }
                await MoveAlongPathAsync(path, loc);
                await Task.Delay(WalkDelay);
                map = Client.CurrentMap;
                if (map == null) break;
            }

            if (Functions.MaxDistance(Client.CurrentLocation, loc) <= 6)
            {
                if (npcId == 0)
                    Client.TryFindNearestNpc(types, out npcId, out _, out entry, out matchedTypes, includeUnknowns: false);

                if (npcId == 0 || entry != null)
                    npcId = await Client.ResolveNpcIdAsync(entry);

                if (npcId != 0)
                {
                    var sellItems = matchedTypes.SelectMany(t => sellGroups[t]).Where(x => x.item != null).ToList();
                    await Client.SellItemsToNpcAsync(npcId, sellItems);
                    Client.Log($"Finished selling to {entry?.Name ?? npcId.ToString()}");
                    foreach (var t in matchedTypes)
                        sellGroups.Remove(t);
                }
                else
                {
                    Client.Log($"Could not find NPC to sell items");
                    break;
                }
            }

            if (!foundPath) break; // resume normal behaviour if we cannot reach npc
        }
        Client.IgnoreNpcInteractions = false;
        Client.ResumeNpcInteractions();
        _sellingItems = false;
        Client.UpdateAction("roaming...");
    }

    private async Task HandleEquipmentRepairsAsync()
    {
        if (_repairingItems) return;
        var equipment = Client.Equipment;
        if (equipment == null) return;

        var toRepair = equipment.Where(i => i != null && i.Info != null && i.CurrentDura < i.MaxDura).ToList();
        if (toRepair.Count == 0) return;

        bool urgent = toRepair.Any(i => i.MaxDura > 0 && i.CurrentDura <= i.MaxDura * 0.05);
        if (!urgent) return;

        _repairingItems = true;
        Client.UpdateAction("repairing items...");
        Client.IgnoreNpcInteractions = true;

        var types = toRepair.Select(i => i!.Info!.Type).Distinct().ToList();
        while (types.Count > 0)
        {
            if (!Client.TryFindNearestRepairNpc(types, out var npcId, out var loc, out var entry, out var matched, includeUnknowns: false))
                break;

            if (entry != null)
            {
                var itemNames = toRepair.Where(i => i.Info != null && matched.Contains(i.Info.Type))
                    .Select(i => i.Info!.FriendlyName)
                    .ToList();
                if (itemNames.Count > 0)
                    Client.Log($"I am heading to {entry.Name} at {loc.X}, {loc.Y} to repair {string.Join(", ", itemNames)}");
            }

            var map = Client.CurrentMap;
            if (map == null) break;
            bool foundPath = true;
            while (Functions.MaxDistance(Client.CurrentLocation, loc) > 6)
            {
                var path = await FindPathAsync(map, Client.CurrentLocation, loc, npcId, 6);
                if (path.Count == 0)
                {
                    Client.Log($"Could not path to {entry?.Name ?? npcId.ToString()}");
                    foundPath = false;
                    break;
                }
                await MoveAlongPathAsync(path, loc);
                await Task.Delay(WalkDelay);
                map = Client.CurrentMap;
                if (map == null) break;
            }

            if (Functions.MaxDistance(Client.CurrentLocation, loc) <= 6)
            {
                if (npcId == 0)
                    Client.TryFindNearestRepairNpc(types, out npcId, out _, out entry, out matched, includeUnknowns: false);

                if (npcId == 0 || entry != null)
                    npcId = await Client.ResolveNpcIdAsync(entry);

                if (npcId != 0)
                {
                    await Client.RepairItemsAtNpcAsync(npcId);
                    Client.Log($"Finished repairing at {entry?.Name ?? npcId.ToString()}");
                    foreach (var t in matched)
                        types.Remove(t);
                }
                else
                {
                    Client.Log("Could not find NPC to repair items");
                    break;
                }
            }

            if (!foundPath) break;
        }

        Client.IgnoreNpcInteractions = false;
        Client.ResumeNpcInteractions();
        _repairingItems = false;
        Client.UpdateAction("roaming...");
    }

    public virtual async Task RunAsync()
    {
        Point current;
        _nextBestMapCheck = DateTime.UtcNow;
        _lastStationaryLocation = Client.CurrentLocation;
        _stationarySince = DateTime.UtcNow;
        _lastMoveOrAttackTime = DateTime.UtcNow;
        while (true)
        {
            if (await HandleReviveAsync())
                continue;

            if (await HandleHarvestingAsync())
                continue;

            if (Client.MovementSavePending)
            {
                await Task.Delay(WalkDelay);
                continue;
            }

            Client.ProcessMapExpRateInterval();
            await ProcessBestMapAsync();
            UpdateTravelDestination();
            bool traveling = _travelPath != null && DateTime.UtcNow >= _travelPauseUntil;
            if (traveling)
            {
                _currentTarget = null;
                _lostTargetLocation = null;
                _lostTargetPath = null;
            }
            if (Client.IsProcessingNpc)
            {
                await Task.Delay(WalkDelay);
                continue;
            }

            if (DateTime.UtcNow >= _nextEquipCheck)
            {
                await CheckEquipmentAsync();
                _nextEquipCheck = DateTime.UtcNow + EquipCheckInterval;
            }

            await HandleEquipmentRepairsAsync();

            await TryUsePotionsAsync();

            await HandleInventoryAsync();

            if (Client.GetCurrentBagWeight() > Client.GetMaxBagWeight() && Client.LastPickedItem != null)
            {
                Client.Log("Overweight detected, dropping last picked item");
                var drop = Client.LastPickedItem;
                await Client.DropItemAsync(drop);
                if (drop?.Info != null)
                {
                    // item may spawn on any adjacent cell so ignore all nearby copies
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            var loc = new Point(Client.CurrentLocation.X + dx, Client.CurrentLocation.Y + dy);
                            _itemRetryTimes[(loc, drop.Info.FriendlyName)] = DateTime.UtcNow + DroppedItemRetryDelay;
                        }
                    }
                }
            }

            foreach (var kv in _itemRetryTimes.ToList())
                if (DateTime.UtcNow >= kv.Value)
                    _itemRetryTimes.Remove(kv.Key);

            foreach (var kv in _monsterIgnoreTimes.ToList())
                if (DateTime.UtcNow >= kv.Value)
                    _monsterIgnoreTimes.Remove(kv.Key);

            var map = Client.CurrentMap;
            if (map == null || !Client.IsMapLoaded)
            {
                await Task.Delay(WalkDelay);
                continue;
            }

            current = Client.CurrentLocation;
            if (_currentTarget != null && !Client.TrackedObjects.ContainsKey(_currentTarget.Id))
            {
                _lostTargetLocation = _currentTarget.Location;
                _lostTargetPath = null;
                _currentRoamPath = null;
                _currentTarget = null;
                _nextPathFindTime = DateTime.MinValue;
            }
            if (_currentTarget != null && _currentTarget.Type == ObjectType.Monster)
            {
                if (_currentTarget.Dead)
                    _nextTargetSwitchTime = DateTime.MinValue;
            }
            int distance = 0;
            TrackedObject? closest = traveling ? null : FindClosestTarget(current, out distance);

            if (!traveling && _currentTarget != null && _currentTarget.Type == ObjectType.Monster &&
                !_currentTarget.Dead &&
                Client.TrackedObjects.ContainsKey(_currentTarget.Id) &&
                closest != null && closest.Type == ObjectType.Monster &&
                closest.Id != _currentTarget.Id &&
                DateTime.UtcNow < _nextTargetSwitchTime)
            {
                closest = _currentTarget;
                distance = Functions.MaxDistance(current, _currentTarget.Location);
            }

            if (closest != null)
            {
                _currentRoamPath = null;
                _lostTargetLocation = null;
                _lostTargetPath = null;
                _nextPathFindTime = DateTime.MinValue;
                if (_currentTarget?.Id != closest.Id)
                {
                    Client.Log($"I have targeted {closest.Name} at {closest.Location.X}, {closest.Location.Y}");
                    _currentTarget = closest;
                    if (closest.Type == ObjectType.Monster)
                        _nextTargetSwitchTime = DateTime.UtcNow + TargetSwitchInterval;
                }

                if (closest.Type == ObjectType.Item)
                {
                    if (distance > 0)
                    {
                        var path = await FindPathAsync(map, current, closest.Location, closest.Id, 0);
                        bool moved = path.Count > 0 && await MoveAlongPathAsync(path, closest.Location);
                        if (!moved)
                        {
                            _itemRetryTimes[(closest.Location, closest.Name)] = DateTime.UtcNow + ItemRetryDelay;
                            _currentTarget = null;
                        }
                    }
                    else
                    {
                        if (Client.HasFreeBagSpace() && Client.GetCurrentBagWeight() < Client.GetMaxBagWeight())
                        {
                            await Client.PickUpAsync();
                        }
                        _itemRetryTimes[(closest.Location, closest.Name)] = DateTime.UtcNow + ItemRetryDelay;
                        _currentTarget = null;
                    }
                }
                else
                {
                    if (distance > 1)
                    {
                        var path = await FindPathAsync(map, current, closest.Location, closest.Id);
                        bool moved = path.Count > 0 && await MoveAlongPathAsync(path, closest.Location);
                        if (!moved)
                        {
                            // ignore unreachable targets
                            _monsterIgnoreTimes[closest.Id] = DateTime.UtcNow + MonsterIgnoreDelay;
                            _currentTarget = null;
                            _nextTargetSwitchTime = DateTime.MinValue;
                        }
                    }
                    else if (DateTime.UtcNow >= _nextAttackTime)
                    {
                        var dir = Functions.DirectionFromPoint(current, closest.Location);
                        await Client.AttackAsync(dir);
                        _lastMoveOrAttackTime = DateTime.UtcNow;
                        _nextAttackTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(AttackDelay);
                    }
                }
            }
            else
            {
                _currentTarget = null;
                if (!traveling && _lostTargetLocation.HasValue)
                {
                    if (Functions.MaxDistance(current, _lostTargetLocation.Value) <= 0)
                    {
                        _lostTargetLocation = null;
                        _lostTargetPath = null;
                    }
                    else
                    {
                        if (_lostTargetPath == null || _lostTargetPath.Count <= 1)
                        {
                            if (DateTime.UtcNow >= _nextPathFindTime)
                            {
                                _lostTargetPath = await FindPathAsync(map, current, _lostTargetLocation.Value);
                                _nextPathFindTime = DateTime.UtcNow + RoamPathFindInterval;
                                if (_lostTargetPath.Count == 0)
                                {
                                    _lostTargetLocation = null;
                                    _lostTargetPath = null;
                                }
                            }
                        }

                        if (_lostTargetPath != null && _lostTargetPath.Count > 0)
                        {
                            bool moved = await MoveAlongPathAsync(_lostTargetPath, _lostTargetLocation.Value);
                            if (!moved)
                            {
                                _lostTargetPath = null;
                                _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                            }
                            else if (_lostTargetPath.Count <= 1)
                            {
                                _lostTargetPath = null;
                                _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                            }
                        }
                    }
                }

                if (!traveling && !_lostTargetLocation.HasValue)
                {
                    if (_searchDestination == null ||
                        Functions.MaxDistance(current, _searchDestination.Value) <= 1 ||
                        !map.IsWalkable(_searchDestination.Value.X, _searchDestination.Value.Y))
                    {
                        _searchDestination = GetRandomPoint(map, Random, current, 50);
                        _currentRoamPath = null;
                        _nextPathFindTime = DateTime.MinValue;
                        Client.Log($"No targets nearby, searching at {_searchDestination.Value.X}, {_searchDestination.Value.Y}");
                    }

                    if (_currentRoamPath == null || _currentRoamPath.Count <= 1)
                    {
                        if (DateTime.UtcNow >= _nextPathFindTime)
                        {
                            _currentRoamPath = await FindPathAsync(map, current, _searchDestination.Value, 0, 0);
                            _nextPathFindTime = DateTime.UtcNow + RoamPathFindInterval;
                            if (_currentRoamPath.Count == 0)
                            {
                                _currentRoamPath = null;
                                _searchDestination = null;
                                _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                                await Task.Delay(WalkDelay);
                                continue;
                            }
                        }
                    }

                    if (_currentRoamPath != null && _currentRoamPath.Count > 0)
                    {
                        bool moved = await MoveAlongPathAsync(_currentRoamPath, _searchDestination.Value);
                        if (!moved)
                        {
                            _currentRoamPath = null;
                            _searchDestination = null;
                            _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                        }
                        else if (_currentRoamPath.Count <= 1)
                        {
                            _currentRoamPath = null;
                            _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                        }
                    }
                }
                else if (traveling && _searchDestination.HasValue)
                {
                    if (_currentRoamPath == null || _currentRoamPath.Count < 1)
                    {
                        if (DateTime.UtcNow >= _nextPathFindTime)
                        {
                            _currentRoamPath = await FindPathAsync(map, current, _searchDestination.Value, 0, 0);
                            _nextPathFindTime = DateTime.UtcNow + RoamPathFindInterval;
                            if (_currentRoamPath.Count == 0)
                            {
                                _travelPath = null;
                                _searchDestination = null;
                                _currentRoamPath = null;
                                _travelPauseUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                                _nextBestMapCheck = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                                traveling = false;
                            }
                        }
                    }

                    if (traveling && _currentRoamPath != null && _currentRoamPath.Count > 1)
                    {
                        bool moved = await MoveAlongPathAsync(_currentRoamPath, _searchDestination.Value);
                        if (!moved)
                        {
                            _currentRoamPath = null;
                            _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                        }
                        else if (_currentRoamPath.Count <= 1)
                        {
                            _currentRoamPath = null;
                            _nextPathFindTime = DateTime.UtcNow + FailedPathFindDelay;
                        }
                    }
                }
                if (traveling && _searchDestination.HasValue)
                {
                    if (Client.CurrentLocation == _searchDestination.Value)
                    {
                        if (_travelStuckSince == DateTime.MinValue)
                            _travelStuckSince = DateTime.UtcNow;
                        else if (DateTime.UtcNow - _travelStuckSince > TimeSpan.FromSeconds(5))
                        {
                            var dir = (MirDirection)Random.Next(8);
                            await Client.TurnAsync(dir);
                            _travelStuckSince = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        _travelStuckSince = DateTime.MinValue;
                    }
                }
            }

            if (_sellingItems)
            {
                Client.UpdateAction("selling items");
            }
            else if (_repairingItems)
            {
                Client.UpdateAction("repairing items...");
            }
            else if (traveling)
            {
                Client.UpdateAction("travelling...");
            }
            else if (_currentTarget != null && _currentTarget.Type == ObjectType.Monster)
            {
                Client.UpdateAction($"attacking {_currentTarget.Name}");
            }
            else
            {
                Client.UpdateAction("roaming...");
            }

            if (Client.CurrentLocation != _lastStationaryLocation)
            {
                _lastStationaryLocation = Client.CurrentLocation;
                _stationarySince = DateTime.UtcNow;
            }
            else if (_stationarySince != DateTime.MinValue &&
                     DateTime.UtcNow - _stationarySince > TimeSpan.FromSeconds(5))
            {
                var dir = (MirDirection)Random.Next(8);
                await Client.TurnAsync(dir);
                _stationarySince = DateTime.UtcNow;
            }

            if (DateTime.UtcNow - _lastMoveOrAttackTime > TimeSpan.FromSeconds(60) &&
                DateTime.UtcNow >= _nextTownTeleportTime)
            {
                var teleport = Client.FindTownTeleport();
                if (teleport != null)
                {
                    await Client.UseItemAsync(teleport);
                    string name = teleport.Info?.FriendlyName ?? "town teleport";
                    Client.Log($"Used {name} due to inactivity");
                    _nextTownTeleportTime = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                    _lastMoveOrAttackTime = DateTime.UtcNow;
                }
            }

            await Task.Delay(WalkDelay);
        }
    }

    private async Task<bool> HandleReviveAsync()
    {
        if (!Client.Dead) return false;
        _currentTarget = null;
        Client.UpdateAction("reviving");
        if (!_sentRevive)
        {
            await Client.TownReviveAsync();
            _sentRevive = true;
        }
        await Task.Delay(WalkDelay);
        if (!Client.Dead) _sentRevive = false;
        return true;
    }

    private async Task<bool> HandleHarvestingAsync()
    {
        if (!Client.IsHarvesting) return false;
        Client.UpdateAction("harvesting");
        var current = Client.CurrentLocation;
        var target = FindClosestTarget(current, out int dist);
        if (target != null && target.Type == ObjectType.Monster && dist <= 1)
        {
            if (DateTime.UtcNow >= _nextAttackTime)
            {
                var dir = Functions.DirectionFromPoint(current, target.Location);
                await Client.AttackAsync(dir);
                _lastMoveOrAttackTime = DateTime.UtcNow;
                _nextAttackTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(AttackDelay);
            }
        }
        await Task.Delay(WalkDelay);
        return true;
    }
}
