using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

public sealed class MapMovementEntry
{
    public string SourceMap { get; set; } = string.Empty;
    public int SourceX { get; set; }
    public int SourceY { get; set; }
    public string DestinationMap { get; set; } = string.Empty;
    public int DestinationX { get; set; }
    public int DestinationY { get; set; }
}

public sealed class MapMovementMemoryBank : MemoryBankBase<MapMovementEntry>
{
    private readonly HashSet<string> _keys = new();
    public MapMovementMemoryBank(string path) : base(path, "Global\\MapMovementMemoryBankMutex")
    {
        foreach (var e in _entries)
            _keys.Add(Key(e.SourceMap, e.SourceX, e.SourceY, e.DestinationMap, e.DestinationX, e.DestinationY));
    }

    protected override void OnLoaded()
    {
        _keys.Clear();
        foreach (var e in _entries)
            _keys.Add(Key(e.SourceMap, e.SourceX, e.SourceY, e.DestinationMap, e.DestinationX, e.DestinationY));
    }

    private static string Key(string src, int sx, int sy, string dest, int dx, int dy) =>
        $"{src}:{sx}:{sy}:{dest}:{dx}:{dy}";

    public void AddMovement(string sourceMapFile, Point sourceLocation, string destinationMapFile, Point destinationLocation)
    {
        bool added = false;
        lock (_lock)
        {
            ReloadIfUpdated();
            var src = Path.GetFileNameWithoutExtension(sourceMapFile);
            var dest = Path.GetFileNameWithoutExtension(destinationMapFile);
            var key = Key(src, sourceLocation.X, sourceLocation.Y, dest, destinationLocation.X, destinationLocation.Y);
            if (!_keys.Contains(key))
            {
                _entries.Add(new MapMovementEntry
                {
                    SourceMap = src,
                    SourceX = sourceLocation.X,
                    SourceY = sourceLocation.Y,
                    DestinationMap = dest,
                    DestinationX = destinationLocation.X,
                    DestinationY = destinationLocation.Y
                });
                _keys.Add(key);
                added = true;
            }
        }

        if (added)
            Save();
    }

    public bool RemoveMovements(string sourceMapFile, Point sourceLocation)
    {
        bool removed = false;
        lock (_lock)
        {
            ReloadIfUpdated();
            var src = Path.GetFileNameWithoutExtension(sourceMapFile);
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                var e = _entries[i];
                if (e.SourceMap == src && e.SourceX == sourceLocation.X && e.SourceY == sourceLocation.Y)
                {
                    _entries.RemoveAt(i);
                    _keys.Remove(Key(e.SourceMap, e.SourceX, e.SourceY, e.DestinationMap, e.DestinationX, e.DestinationY));
                    removed = true;
                }
            }
        }
        if (removed)
            Save();
        return removed;
    }

    public IReadOnlyList<string> GetKnownMaps()
    {
        lock (_lock)
        {
            ReloadIfUpdated();
            var set = new HashSet<string>();
            foreach (var e in _entries)
            {
                set.Add(e.SourceMap);
                set.Add(e.DestinationMap);
            }
            return set.ToList();
        }
    }
}
