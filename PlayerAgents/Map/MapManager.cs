using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace PlayerAgents.Map;

public static class MapManager
{
    public const string MapDirectory = "Maps";
    private static readonly ConcurrentDictionary<string, Task<MapData>> _cache = new();

    public static Task<MapData> GetMapAsync(string path) =>
        _cache.GetOrAdd(path, async p =>
        {
            var data = new MapData(p);
            await data.InitializeAsync();
            return data;
        });
}
