using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Shared;

public sealed class NpcEntry
{
    public string Name { get; set; } = string.Empty;
    public string MapFile { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public bool CanBuy { get; set; }
    public bool CanSell { get; set; }
    public bool CanRepair { get; set; }
    public bool CanSpecialRepair { get; set; }
    public List<int>? BuyItemIndexes { get; set; }
    public List<ItemType>? SellItemTypes { get; set; }
    public List<ItemType>? CannotSellItemTypes { get; set; }
    public List<ItemType>? RepairItemTypes { get; set; }
    public List<ItemType>? CannotRepairItemTypes { get; set; }
    public List<ItemType>? SpecialRepairItemTypes { get; set; }
    public List<ItemType>? CannotSpecialRepairItemTypes { get; set; }
    public bool CheckedMerchantKeys { get; set; }
}

public sealed class NpcMemoryBank : MemoryBankBase<NpcEntry>
{
    private readonly Dictionary<(string, string, int, int), NpcEntry> _lookup = new();
    public NpcMemoryBank(string path) : base(path, "Global\\NpcMemoryBankMutex")
    {
        foreach (var e in _entries)
            _lookup[(e.Name, e.MapFile, e.X, e.Y)] = e;
    }

    protected override void OnLoaded()
    {
        _lookup.Clear();
        foreach (var e in _entries)
            _lookup[(e.Name, e.MapFile, e.X, e.Y)] = e;
    }

    public NpcEntry AddNpc(string name, string mapFile, Point location)
    {
        bool added = false;
        NpcEntry? entry = null;
        lock (_lock)
        {
            ReloadIfUpdated();
            var normalized = Path.GetFileNameWithoutExtension(mapFile);
            var key = (name, normalized, location.X, location.Y);
            if (!_lookup.TryGetValue(key, out entry))
            {
                entry = new NpcEntry { Name = name, MapFile = normalized, X = location.X, Y = location.Y };
                _entries.Add(entry);
                _lookup[key] = entry;
                added = true;
            }
        }

        if (added)
            Save();

        return entry!;
    }
}
