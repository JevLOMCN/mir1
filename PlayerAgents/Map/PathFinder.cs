using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PlayerAgents.Map;

public static class PathFinder
{
    private const int StepLimit = 40000;
    public readonly record struct MapPoint(string MapFile, Point Location);

    public static async Task<List<Point>> FindPathAsync(MapData map, Point start, Point end, ISet<Point>? obstacles = null, int radius = 1)
    {
        return await Task.Run(() => FindPath(map, start, end, obstacles, radius));
    }

    private static List<Point> FindPath(MapData map, Point start, Point end, ISet<Point>? obstacles, int radius)
    {
        int width = map.Width;
        int height = map.Height;

        if (!map.IsWalkable(start.X, start.Y) || !map.IsWalkable(end.X, end.Y))
            return new List<Point>();

        if (Functions.MaxDistance(start, end) <= radius)
            return new List<Point> { start };

        if (width == 0 || height == 0)
            return new List<Point>();
        var open = new PriorityQueue<Point, int>();
        var cameFrom = new Dictionary<Point, Point>();
        var gScore = new Dictionary<Point, int>();
        gScore[start] = 0;
        open.Enqueue(start, Heuristic(start, end));
        var directions = new[]
        {
            new Point(0,-1), new Point(1,0), new Point(0,1), new Point(-1,0),
            new Point(1,-1), new Point(1,1), new Point(-1,1), new Point(-1,-1)
        };

        int steps = 0;
        int maxSteps = Math.Min(width * height, StepLimit);
        var closed = new HashSet<Point>();
        while (open.Count > 0)
        {
            var current = open.Dequeue();
            if (!closed.Add(current))
                continue;
            if (Functions.MaxDistance(current, end) <= radius)
                return ReconstructPath(cameFrom, current);

            if (++steps > maxSteps)
                break;

            foreach (var dir in directions)
            {
                var neighbor = new Point(current.X + dir.X, current.Y + dir.Y);
                if (!map.IsWalkable(neighbor.X, neighbor.Y)) continue;
                if (obstacles != null && obstacles.Contains(neighbor) && neighbor != end && neighbor != start) continue;
                int tentative = gScore[current] + ((dir.X == 0 || dir.Y == 0) ? 10 : 14);
                if (gScore.TryGetValue(neighbor, out var g) && tentative >= g) continue;
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative;
                int f = tentative + Heuristic(neighbor, end);
                open.Enqueue(neighbor, f);
            }
        }
        return new List<Point>();
    }

    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        var totalPath = new List<Point> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    private static int Heuristic(Point a, Point b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return 10 * (dx + dy);
    }

    public static async Task<List<MapPoint>> FindPathAsync(MapMovementMemoryBank movements,
        string startMapFile, Point start, string endMapFile, Point end, ISet<Point>? obstacles = null, int radius = 1)
    {
        // normalize map names for memory lookup
        var startMap = Path.GetFileNameWithoutExtension(startMapFile);
        var destMap = Path.GetFileNameWithoutExtension(endMapFile);

        if (startMap == destMap)
        {
            var data = await MapManager.GetMapAsync(startMapFile);
            var path = await FindPathAsync(data, start, end, obstacles, radius);
            var result = new List<MapPoint>(path.Count);
            foreach (var p in path)
                result.Add(new MapPoint(startMapFile, p));
            return result;
        }

        var entries = movements.GetAll();

        // BFS to find sequence of map transitions
        var queue = new Queue<(string Map, List<MapMovementEntry> Path)>();
        var visited = new HashSet<string> { startMap };
        queue.Enqueue((startMap, new List<MapMovementEntry>()));
        List<MapMovementEntry>? edgePath = null;

        while (queue.Count > 0)
        {
            var (map, pathSoFar) = queue.Dequeue();
            if (map == destMap)
            {
                edgePath = pathSoFar;
                break;
            }

            foreach (var e in entries)
            {
                if (e.SourceMap != map) continue;
                if (visited.Contains(e.DestinationMap)) continue;
                var newList = new List<MapMovementEntry>(pathSoFar) { e };
                visited.Add(e.DestinationMap);
                queue.Enqueue((e.DestinationMap, newList));
            }
        }

        if (edgePath == null)
            return new List<MapPoint>();

        var resultPath = new List<MapPoint>();
        string currentMapPath = startMapFile;
        var currentMapData = await MapManager.GetMapAsync(currentMapPath);
        var currentPoint = start;

        foreach (var edge in edgePath)
        {
            var exitPoint = new Point(edge.SourceX, edge.SourceY);
            var partial = await FindPathAsync(currentMapData, currentPoint, exitPoint, obstacles, 0);
            if (partial.Count == 0)
                return new List<MapPoint>();
            for (int i = 0; i < partial.Count - 1; i++)
                resultPath.Add(new MapPoint(currentMapPath, partial[i]));

            currentMapPath = Path.Combine(MapManager.MapDirectory, edge.DestinationMap + ".map");
            currentMapData = await MapManager.GetMapAsync(currentMapPath);
            currentPoint = new Point(edge.DestinationX, edge.DestinationY);
            resultPath.Add(new MapPoint(currentMapPath, currentPoint));
            obstacles = null; // only respect obstacles on first map
        }

        var finalPartial = await FindPathAsync(currentMapData, currentPoint, end, obstacles, radius);
        if (finalPartial.Count == 0)
            return new List<MapPoint>();
        foreach (var p in finalPartial)
            resultPath.Add(new MapPoint(currentMapPath, p));

        return resultPath;
    }
}
