using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PlayerAgents.Map;

public sealed class MapData
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool[,] Walkable { get; private set; } = new bool[0,0];
    public byte[,] Doors { get; private set; } = new byte[0,0];
    public List<Point> WalkableCells { get; private set; } = new();

    private readonly string _path;
    private readonly ReaderWriterLockSlim _lock = new();
    private FileSystemWatcher? _watcher;

    public MapData(string path)
    {
        _path = path;
    }

    public async Task InitializeAsync()
    {
        await LoadAsync();
        var dir = Path.GetDirectoryName(_path);
        var file = Path.GetFileName(_path);
        if (dir != null)
        {
            _watcher = new FileSystemWatcher(dir, file);
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _watcher.Changed += async (_, _) => await LoadAsync();
            _watcher.Created += async (_, _) => await LoadAsync();
            _watcher.EnableRaisingEvents = true;
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            byte[] bytes;
            using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 4096, true))
            {
                bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
            }

            var type = MapParser.FindType(bytes);
            var cells = type switch
            {
                0 => MapParser.LoadV0(bytes),
                1 => MapParser.LoadV1(bytes),
                2 => MapParser.LoadV2(bytes),
                3 => MapParser.LoadV3(bytes),
                4 => MapParser.LoadV4(bytes),
                5 => MapParser.LoadV5(bytes),
                6 => MapParser.LoadV6(bytes),
                7 => MapParser.LoadV7(bytes),
                100 => MapParser.LoadV100(bytes),
                _ => MapParser.LoadV0(bytes)
            };

            _lock.EnterWriteLock();
            try
            {
                Width = cells.Walk.GetLength(0);
                Height = cells.Walk.GetLength(1);
                Walkable = cells.Walk;
                Doors = cells.Doors;
                var list = new List<Point>(Width * Height / 2);
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (cells.Walk[x, y])
                            list.Add(new Point(x, y));
                    }
                }
                WalkableCells = list;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading map {_path}: {ex}");
        }
    }

    public bool IsWalkable(int x, int y)
    {
        _lock.EnterReadLock();
        try
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return false;
            return Walkable[x, y];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public byte GetDoorIndex(int x, int y)
    {
        _lock.EnterReadLock();
        try
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return 0;
            return Doors[x, y];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}
