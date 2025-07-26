using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using S = ServerPackets;

public sealed record NpcButton(string Text, string Key);

public sealed record NpcDialogPage(IReadOnlyList<string> Lines, IReadOnlyList<NpcButton> Buttons);

public sealed class NPCInteraction
{
    private readonly GameClient _client;
    private readonly uint _npcId;

    public NPCInteraction(GameClient client, uint npcId)
    {
        _client = client;
        _npcId = npcId;
    }

    public Task<NpcDialogPage> BeginAsync(CancellationToken cancellationToken = default)
    {
        return RequestPageAsync("@Main", cancellationToken);
    }

    public Task<NpcDialogPage> SelectAsync(string buttonKey, CancellationToken cancellationToken = default)
    {
        return RequestPageAsync(buttonKey, cancellationToken);
    }

    public async Task<NpcDialogPage> SelectFromMainAsync(string buttonKey, CancellationToken cancellationToken = default)
    {
        await RequestPageAsync("@Main", cancellationToken);
        await Task.Delay(50, cancellationToken);
        return await RequestPageAsync(buttonKey, cancellationToken);
    }

    private async Task<NpcDialogPage> RequestPageAsync(string key, CancellationToken cancellationToken)
    {
        await _client.CallNPCAsync(_npcId, key);
        var response = await _client.WaitForLatestNpcResponseAsync(cancellationToken);
        var buttons = ParseButtons(response.Page);
        return new NpcDialogPage(response.Page, buttons);
    }

    private static IReadOnlyList<NpcButton> ParseButtons(IEnumerable<string> lines)
    {
        var list = new List<NpcButton>();
        foreach (var line in lines)
        {
            foreach (Match m in Regex.Matches(line, @"<([^<>]+)/(@[^>]+)>", RegexOptions.IgnoreCase))
            {
                list.Add(new NpcButton(m.Groups[1].Value, m.Groups[2].Value));
            }
        }
        return list;
    }
}
