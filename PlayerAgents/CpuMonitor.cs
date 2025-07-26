using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;

public sealed class CpuMonitor
{
    private readonly PerformanceCounter? _cpuCounter;
    private long _prevIdle;
    private long _prevTotal;

    public CpuMonitor()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue();
            }
            catch { }
        }
        else
        {
            ReadCpuStat(out _prevIdle, out _prevTotal);
        }
    }

    public double GetCpuUsage()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { return _cpuCounter?.NextValue() ?? 0.0; }
            catch { return 0.0; }
        }

        if (!ReadCpuStat(out var idle, out var total))
            return 0.0;

        var idleDiff = idle - _prevIdle;
        var totalDiff = total - _prevTotal;
        _prevIdle = idle;
        _prevTotal = total;
        if (totalDiff == 0) return 0.0;
        return (1.0 - (double)idleDiff / totalDiff) * 100.0;
    }

    private static bool ReadCpuStat(out long idle, out long total)
    {
        idle = total = 0;
        try
        {
            var line = File.ReadLines("/proc/stat").First();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5 || parts[0] != "cpu")
                return false;

            long user = long.Parse(parts[1]);
            long nice = long.Parse(parts[2]);
            long system = long.Parse(parts[3]);
            long idleT = long.Parse(parts[4]);
            long iowait = parts.Length > 5 ? long.Parse(parts[5]) : 0;
            long irq = parts.Length > 6 ? long.Parse(parts[6]) : 0;
            long softirq = parts.Length > 7 ? long.Parse(parts[7]) : 0;
            long steal = parts.Length > 8 ? long.Parse(parts[8]) : 0;
            long guest = parts.Length > 9 ? long.Parse(parts[9]) : 0;
            long guestNice = parts.Length > 10 ? long.Parse(parts[10]) : 0;

            idle = idleT + iowait;
            total = user + nice + system + idleT + iowait + irq + softirq + steal + guest + guestNice;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
