using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

public sealed class AgentStatus
{
    public ushort Level { get; init; }
    public string MapFile { get; init; } = string.Empty;
    public string MapName { get; init; } = string.Empty;
    public int X { get; init; }
    public int Y { get; init; }
    public string Action { get; init; } = string.Empty;
}

public interface IAgentLogger
{
    void RegisterAgent(string agent);
    void RemoveAgent(string agent);
    void UpdateStatus(string agent, AgentStatus status);
}

public sealed class ConsoleAgentLogger : IAgentLogger
{
    public void RegisterAgent(string agent)
    {
        // no-op
    }

    public void RemoveAgent(string agent)
    {
        // no-op
    }

    public void UpdateStatus(string agent, AgentStatus status)
    {
        var map = Path.GetFileNameWithoutExtension(status.MapFile);
        Console.Error.WriteLine($"{agent} --- Level {status.Level} --- {map} ({status.MapName}) ({status.X},{status.Y}) --- {status.Action}");
    }
}

public sealed class NullAgentLogger : IAgentLogger
{
    public void RegisterAgent(string agent)
    {
        // no-op
    }

    public void RemoveAgent(string agent)
    {
        // no-op
    }

    public void UpdateStatus(string agent, AgentStatus status)
    {
        // no-op
    }
}

public sealed class SummaryAgentLogger : IAgentLogger, IDisposable
{
    private readonly Dictionary<string, AgentStatus> _status = new();
    private readonly List<string> _order = new();
    private readonly HashSet<string> _registered = new();
    private readonly object _lockObj = new();
    private readonly Timer _timer;
    private readonly CpuMonitor _cpu = new();
    private int _lastLineCount;

    public SummaryAgentLogger()
    {
        _timer = new Timer(_ =>
        {
            lock (_lockObj)
            {
                Render();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        // CPU monitor initialized above
    }

    public void Dispose() => _timer.Dispose();

    public void RegisterAgent(string agent)
    {
        lock (_lockObj)
        {
            if (_registered.Add(agent))
            {
                _status[agent] = new AgentStatus();
                _order.Add(agent);
                Render();
            }
        }
    }

    public void RemoveAgent(string agent)
    {
        lock (_lockObj)
        {
            if (_registered.Remove(agent) && _status.Remove(agent))
            {
                _order.Remove(agent);
                Render();
            }
        }
    }

    public void UpdateStatus(string agent, AgentStatus status)
    {
        lock (_lockObj)
        {
            if (!_registered.Contains(agent))
                return;

            _status[agent] = status;
        }
    }

    private void Render()
    {
        Console.CursorVisible = false;
        int colWidth = Math.Max(20, Console.WindowWidth / 4);
        var lines = new List<string>();
        string currentLine = string.Empty;
        for (int i = 0; i < _order.Count; i++)
        {
            var agent = _order[i];
            _status.TryGetValue(agent, out var status);
            var map = Path.GetFileNameWithoutExtension(status.MapFile);
            string cell = $"{agent} --- Level {status.Level} --- {map} ({status.MapName}) ({status.X},{status.Y}) --- {status.Action}";
            if (cell.Length > colWidth)
                cell = cell.Substring(0, colWidth);
            cell = cell.PadRight(colWidth);
            currentLine += cell;
            if (i % 4 == 3 || i == _order.Count - 1)
            {
                if (currentLine.Length > Console.WindowWidth)
                    currentLine = currentLine.Substring(0, Console.WindowWidth);
                lines.Add(currentLine.PadRight(Console.WindowWidth));
                currentLine = string.Empty;
            }
        }
        try
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Error.Write(lines[i]);
            }

            for (int i = lines.Count; i < _lastLineCount; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Error.Write(new string(' ', Console.WindowWidth));
            }
        }
        catch { }


        var cpuUsage = _cpu.GetCpuUsage();
        Console.Title = $"Agents: {_order.Count} CPU: {cpuUsage:0.0}%";

        _lastLineCount = lines.Count;
    }

    // CPU usage handled by CpuMonitor
}
