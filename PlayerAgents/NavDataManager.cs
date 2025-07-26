using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public sealed class NavDataManager
{
    private readonly ConcurrentDictionary<string, NavData> _cache = new();

    public NavData GetNavData(string mapPath, IEnumerable<Point> initialCells)
    {
        var navDir = Path.Combine(AppContext.BaseDirectory, "nav_data");
        var navPath = Path.Combine(navDir, Path.GetFileName(mapPath));
        return _cache.GetOrAdd(navPath, p => new NavData(p, initialCells));
    }
}
