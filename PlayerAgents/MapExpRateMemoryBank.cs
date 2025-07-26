using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public sealed class MapExpRateEntry
{
    public string MapFile { get; set; } = string.Empty;
    public MirClass Class { get; set; }
    public ushort Level { get; set; }
    public double ExpPerHour { get; set; }
}

public sealed class MapExpRateMemoryBank : MemoryBankBase<MapExpRateEntry>
{
    private readonly Dictionary<(string, MirClass, ushort), MapExpRateEntry> _lookup = new();
    public MapExpRateMemoryBank(string path) : base(path, "Global\\MapExpRateMemoryBankMutex")
    {
        foreach (var e in _entries)
            _lookup[(e.MapFile, e.Class, e.Level)] = e;
    }

    protected override void OnLoaded()
    {
        _lookup.Clear();
        foreach (var e in _entries)
            _lookup[(e.MapFile, e.Class, e.Level)] = e;
    }

    public void AddRate(string mapFile, MirClass playerClass, ushort level, double expPerHour)
    {
        bool added = false;
        lock (_lock)
        {
            ReloadIfUpdated();
            var normalized = Path.GetFileNameWithoutExtension(mapFile);
            var key = (normalized, playerClass, level);
            if (_lookup.TryGetValue(key, out var existing))
            {
                if (expPerHour > existing.ExpPerHour)
                {
                    existing.ExpPerHour = expPerHour;
                    added = true;
                }
            }
            else
            {
                var entry = new MapExpRateEntry { MapFile = normalized, Class = playerClass, Level = level, ExpPerHour = expPerHour };
                _entries.Add(entry);
                _lookup[key] = entry;
                added = true;
            }
        }

        if (added)
            Save();
    }

    public string? GetBestMapFile(MirClass playerClass, ushort level)
    {
        lock (_lock)
        {
            ReloadIfUpdated();
            return _entries
                .Where(e => e.Class == playerClass && e.Level == level && e.ExpPerHour > 0)
                .OrderByDescending(e => e.ExpPerHour)
                .Select(e => e.MapFile)
                .FirstOrDefault();
        }
    }
}
