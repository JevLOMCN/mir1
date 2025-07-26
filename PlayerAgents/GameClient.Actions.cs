using System;
using System.Drawing;
using C = ClientPackets;

public sealed partial class GameClient
{
    private void AddPendingMovementCell(Point p)
    {
        if (!IsKnownMovementCell(p))
            _pendingMovementAction.Add(p);
    }

    public async Task WalkAsync(MirDirection direction)
    {
        if (_stream == null) return;
        if (_movementSaveCts != null) return;
        CancelMovementDeleteCheck();
        var target = Functions.PointMove(_currentLocation, direction, 1);
        await TryOpenDoorAsync(target);
        Log($"I am walking to {target.X}, {target.Y}");
        _pendingMoveTarget = target;
        _pendingMovementAction.Clear();
        AddPendingMovementCell(target);
        var walk = new C.Walk { Direction = direction };
        await SendAsync(walk);
        _lastMoveTime = DateTime.UtcNow;
        _canRun = true;
    }

    private bool IsCellBlocked(Point p)
    {
        if (_mapData == null || !_mapData.IsWalkable(p.X, p.Y))
            return true;

        return _blockingCells.ContainsKey(p);
    }

    private async Task TryOpenDoorAsync(Point p)
    {
        if (_mapData == null) return;
        byte door = _mapData.GetDoorIndex(p.X, p.Y);
        if (door > 0)
            await SendAsync(new C.Opendoor { DoorIndex = door });
    }

    public bool CanWalk(MirDirection direction)
    {
        var target = Functions.PointMove(_currentLocation, direction, 1);
        return !IsCellBlocked(target);
    }

    public bool CanRun(MirDirection direction)
    {
        if (!_canRun) return false;
        var now = DateTime.UtcNow;
        if (now - _lastMoveTime > TimeSpan.FromMilliseconds(900)) return false;

        var first = Functions.PointMove(_currentLocation, direction, 1);
        var second = Functions.PointMove(_currentLocation, direction, 2);

        return !(IsCellBlocked(first) || IsCellBlocked(second));
    }

    public async Task AttackAsync(MirDirection direction)
    {
        if (_stream == null) return;
        var attack = new C.Attack { Direction = direction, Spell = Spell.None };
        await SendAsync(attack);
    }

    public async Task TurnAsync(MirDirection direction)
    {
        if (_stream == null) return;
        if (_movementSaveCts != null) return;
        _pendingMoveTarget = _currentLocation;
        _pendingMovementAction.Clear();
        AddPendingMovementCell(_currentLocation);
        await SendAsync(new C.Turn { Direction = direction });
        MaybeStartMovementDeleteCheck();
    }

    public async Task TownReviveAsync()
    {
        if (_stream == null) return;
        await SendAsync(new C.TownRevive());
    }

    public async Task EquipItemAsync(UserItem item, EquipmentSlot slot)
    {
        if (_stream == null) return;
        var equip = new C.EquipItem
        {
            Grid = MirGridType.Inventory,
            UniqueID = item.UniqueID,
            To = (int)slot
        };
        await SendAsync(equip);
    }

    private int FindFreeInventorySlot() =>
        _inventory == null ? -1 : Array.FindIndex(_inventory, item => item == null);

    public async Task UnequipItemAsync(EquipmentSlot slot)
    {
        if (_stream == null || _equipment == null) return;
        var item = _equipment[(int)slot];
        if (item == null) return;
        int index = FindFreeInventorySlot();
        if (index < 0) return;

        var remove = new C.RemoveItem
        {
            Grid = MirGridType.Inventory,
            UniqueID = item.UniqueID,
            To = index
        };

        await SendAsync(remove);
    }

    public bool CanEquipItem(UserItem item, EquipmentSlot slot)
    {
        if (_playerClass == null) return false;
        if (item.Info == null) return false;

        if (!IsItemForSlot(item.Info, slot)) return false;

        if (item.Info.RequiredGender != RequiredGender.None)
        {
            RequiredGender genderFlag = _gender == MirGender.Male ? RequiredGender.Male : RequiredGender.Female;
            if (!item.Info.RequiredGender.HasFlag(genderFlag))
                return false;
        }

        RequiredClass playerClassFlag = _playerClass switch
        {
            MirClass.Warrior => RequiredClass.Warrior,
            MirClass.Wizard => RequiredClass.Wizard,
            MirClass.Taoist => RequiredClass.Taoist,
            _ => RequiredClass.None
        };

        if (!item.Info.RequiredClass.HasFlag(playerClassFlag))
            return false;

        switch (item.Info.RequiredType)
        {
            case RequiredType.Level:
                if (_level < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MaxDC:
                if (GetStatTotal(Stat.MaxDC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MaxMC:
                if (GetStatTotal(Stat.MaxMC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MaxSC:
                if (GetStatTotal(Stat.MaxSC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MinDC:
                if (GetStatTotal(Stat.MinDC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MinMC:
                if (GetStatTotal(Stat.MinMC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MinSC:
                if (GetStatTotal(Stat.MinSC) < item.Info.RequiredAmount) return false;
                break;
            case RequiredType.MaxLevel:
                if (_level > item.Info.RequiredAmount) return false;
                break;
        }

        if (_equipment != null)
        {
            var current = _equipment.Length > (int)slot ? _equipment[(int)slot] : null;
            if (item.Info.Type == ItemType.Weapon)
            {
                int weight = GetCurrentHandWeight();
                if (current?.Info != null)
                    weight -= current.Weight;
                if (weight + item.Weight > GetMaxHandWeight())
                    return false;
            }
            else
            {
                int weight = GetCurrentWearWeight();
                if (current?.Info != null)
                    weight -= current.Weight;
                if (weight + item.Weight > GetMaxWearWeight())
                    return false;
            }
        }

        return true;
    }

    private static bool IsItemForSlot(ItemInfo info, EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.Weapon => info.Type == ItemType.Weapon,
            EquipmentSlot.Armour => info.Type == ItemType.Armour,
            EquipmentSlot.Helmet => info.Type == ItemType.Helmet,
            EquipmentSlot.Necklace => info.Type == ItemType.Necklace,
            EquipmentSlot.BraceletL => info.Type == ItemType.Bracelet,
            EquipmentSlot.BraceletR => info.Type == ItemType.Bracelet,
            EquipmentSlot.RingL => info.Type == ItemType.Ring,
            EquipmentSlot.RingR => info.Type == ItemType.Ring,
            _ => false
        };
    }

    public async Task PickUpAsync()
    {
        if (_stream == null) return;
        await SendAsync(new C.PickUp());
    }

    public async Task HarvestAsync(MirDirection direction)
    {
        if (_stream == null) return;
        await SendAsync(new C.Harvest { Direction = direction });
    }

    public async Task UseItemAsync(UserItem item)
    {
        if (_stream == null) return;
        var use = new C.UseItem
        {
            UniqueID = item.UniqueID,
            Grid = MirGridType.Inventory
        };
        await SendAsync(use);
    }

    public async Task DropItemAsync(UserItem item)
    {
        if (_stream == null) return;
        var drop = new C.DropItem
        {
            UniqueID = item.UniqueID,
            Count = item.Count
        };
        await SendAsync(drop);
    }

    public async Task SellItemAsync(UserItem item)
    {
        await SellItemAsync(item.UniqueID, item.Count);
    }

    public async Task SellItemAsync(ulong uniqueId, ushort count)
    {
        if (_stream == null) return;
        var sell = new C.SellItem
        {
            UniqueID = uniqueId,
            Count = count
        };
        await SendAsync(sell);
    }

    public async Task BuyItemAsync(ulong uniqueId, ushort count, PanelType type)
    {
        if (_stream == null) return;
        var buy = new C.BuyItem
        {
            ItemIndex = uniqueId,
            Count = count,
            Type = type
        };
        await SendAsync(buy);
    }
}
