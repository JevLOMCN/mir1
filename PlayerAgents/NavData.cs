using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

public sealed class NavData
{
    private readonly string _path;
    private readonly Mutex _fileMutex;
    private readonly object _lock = new();
    private readonly HashSet<Point> _cells;
    private DateTime _lastWriteTime;
    private int _lastSavedCount;
    private bool _dormant;

    public NavData(string path, IEnumerable<Point> initialCells)
    {
        _path = path;
        _fileMutex = new Mutex(false, "Global\\NavData_" + Path.GetFileNameWithoutExtension(path));
        _cells = new HashSet<Point>(initialCells);
        Load(initialCells);
        _dormant = _cells.Count == 0;
    }

    private void AcquireFileMutex()
    {
        try
        {
            if (!_fileMutex.WaitOne(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Failed to acquire file mutex");
        }
        catch (AbandonedMutexException)
        {
            // mutex was abandoned by another process, we now hold it
        }
    }

    private void Load(IEnumerable<Point> defaultCells)
    {
        AcquireFileMutex();
        try
        {
            if (File.Exists(_path))
            {
                var lines = File.ReadAllLines(_path);
                var list = new List<Point>(lines.Length);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out var x) &&
                        int.TryParse(parts[1], out var y))
                    {
                        list.Add(new Point(x, y));
                    }
                }
                lock (_lock)
                {
                    _cells.Clear();
                    foreach (var pt in list)
                        _cells.Add(pt);
                }
                _lastWriteTime = File.GetLastWriteTimeUtc(_path);
                _lastSavedCount = _cells.Count;
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                SaveInternal();
            }
        }
        finally
        {
            _fileMutex.ReleaseMutex();
        }
    }

    private void SaveInternal()
    {
        var lines = _cells.Select(p => $"{p.X},{p.Y}").ToArray();
        File.WriteAllLines(_path + ".tmp", lines);
        if (File.Exists(_path))
            File.Replace(_path + ".tmp", _path, null);
        else
            File.Move(_path + ".tmp", _path);
        _lastWriteTime = File.GetLastWriteTimeUtc(_path);
        _lastSavedCount = _cells.Count;
        if (_lastSavedCount == 0)
            _dormant = true;
    }

    public void Remove(Point p)
    {
        if (_dormant)
            return;
        lock (_lock)
        {
            _cells.Remove(p);
            if (_cells.Count == 0)
                _dormant = true;
        }
    }

    public void Save()
    {
        if (_dormant && _lastSavedCount == 0)
            return;
        string[] lines;
        lock (_lock)
        {
            lines = _cells.Select(c => $"{c.X},{c.Y}").ToArray();
        }
        AcquireFileMutex();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllLines(_path + ".tmp", lines);
            if (File.Exists(_path))
                File.Replace(_path + ".tmp", _path, null);
            else
                File.Move(_path + ".tmp", _path);
            _lastWriteTime = File.GetLastWriteTimeUtc(_path);
            lock (_lock)
            {
                _lastSavedCount = _cells.Count;
                if (_lastSavedCount == 0)
                    _dormant = true;
            }
        }
        finally
        {
            _fileMutex.ReleaseMutex();
        }
    }

    public void SaveIfNeeded(TimeSpan interval)
    {
        bool changed;
        lock (_lock)
        {
            if (_dormant && _lastSavedCount == 0)
                return;
            changed = _cells.Count != _lastSavedCount;
        }
        if (changed && DateTime.UtcNow - _lastWriteTime >= interval)
            Save();
    }

    public bool TryGetRandomCell(Random random, Point origin, int radius, out Point cell)
    {
        cell = default;
        if (_dormant)
            return false;

        lock (_lock)
        {
            if (_cells.Count == 0)
            {
                _dormant = true;
                return false;
            }

            IEnumerable<Point> source = _cells;
            if (radius > 0)
            {
                var subset = _cells.Where(c => Functions.MaxDistance(c, origin) <= radius).ToList();
                if (subset.Count == 0)
                {
                    cell = default;
                    return false;
                }
                source = subset;
            }

            var list = source as IList<Point> ?? source.ToList();
            cell = list[random.Next(list.Count)];
            return true;
        }
    }
}
