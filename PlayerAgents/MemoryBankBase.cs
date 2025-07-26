using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

public abstract class MemoryBankBase<TEntry>
{
    private readonly string _path;
    private DateTime _lastWriteTime;
    protected readonly List<TEntry> _entries = new();
    protected readonly object _lock = new();
    private readonly Mutex _fileMutex;

    protected MemoryBankBase(string path, string mutexName)
    {
        _path = path;
        _fileMutex = new Mutex(false, mutexName);
        Load();
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
            // Previous owner exited without releasing the mutex. We now hold it.
        }
    }

    protected virtual void OnLoaded()
    {
    }

    private void Load()
    {
        List<TEntry>? items = null;

        AcquireFileMutex();
        try
        {
            if (File.Exists(_path))
            {
                using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                items = JsonSerializer.Deserialize<List<TEntry>>(fs);
                _lastWriteTime = File.GetLastWriteTimeUtc(_path);
            }
        }
        finally
        {
            _fileMutex.ReleaseMutex();
        }

        lock (_lock)
        {
            _entries.Clear();
            if (items != null)
                _entries.AddRange(items);
            OnLoaded();
        }
    }

    protected void Save()
    {
        string json;
        lock (_lock)
        {
            json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
        }

        AcquireFileMutex();
        try
        {
            string tmp = _path + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(_path))
                File.Replace(tmp, _path, null);
            else
                File.Move(tmp, _path);
            _lastWriteTime = File.GetLastWriteTimeUtc(_path);
        }
        finally
        {
            _fileMutex.ReleaseMutex();
        }
    }

    protected void ReloadIfUpdated()
    {
        if (!File.Exists(_path)) return;
        var time = File.GetLastWriteTimeUtc(_path);
        if (time > _lastWriteTime)
        {
            Load();
        }
    }

    public void CheckForUpdates() => ReloadIfUpdated();

    public void SaveChanges()
    {
        lock (_lock)
        {
            ReloadIfUpdated();
            Save();
        }
    }

    public IReadOnlyList<TEntry> GetAll()
    {
        lock (_lock)
        {
            ReloadIfUpdated();
            return _entries.ToList();
        }
    }
}
