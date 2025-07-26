using System.Threading;
using System.Threading.Tasks;
using C = ClientPackets;
using S = ServerPackets;

public sealed partial class GameClient
{
    public async Task CallNPCAsync(uint objectId, string key)
    {
        if (_stream == null) return;
        await SendAsync(new C.CallNPC { ObjectID = objectId, Key = $"[{key}]" });
    }

    public Task<S.NPCResponse> WaitForNpcResponseAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<S.NPCResponse>();
        _npcResponseTcs = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.TrySetCanceled());
        return tcs.Task;
    }

    public async Task<S.NPCResponse> WaitForLatestNpcResponseAsync(CancellationToken cancellationToken = default)
    {
        var response = await WaitForNpcResponseAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            var nextTask = WaitForNpcResponseAsync(cancellationToken);
            var delayTask = Task.Delay(NpcResponseDebounceMs, cancellationToken);
            var finished = await Task.WhenAny(nextTask, delayTask).ConfigureAwait(false);
            if (finished == nextTask)
            {
                response = await nextTask.ConfigureAwait(false);
                continue;
            }
            break;
        }
        return response;
    }

    private S.NPCResponse? _queuedNpcResponse;
    private CancellationTokenSource? _npcResponseCts;

    private void DeliverNpcResponse(S.NPCResponse response)
    {
        if (response.Page.Count == 0)
            return;

        _queuedNpcResponse = response;
        _npcResponseCts?.Cancel();
        var cts = new CancellationTokenSource();
        _npcResponseCts = cts;
        FireAndForget(Task.Run(async () =>
        {
            try
            {
                await Task.Delay(NpcResponseDebounceMs, cts.Token);
                if (!cts.IsCancellationRequested && _queuedNpcResponse != null)
                {
                    _npcResponseTcs?.TrySetResult(_queuedNpcResponse);
                    _npcResponseTcs = null;
                    _queuedNpcResponse = null;
                }
            }
            catch (TaskCanceledException)
            {
            }
        }));
    }

    public Task WaitForNpcGoodsAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        _npcGoodsTcs = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.TrySetCanceled());
        return tcs.Task;
    }

    public Task WaitForNpcSellAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        _npcSellTcs = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.TrySetCanceled());
        return tcs.Task;
    }

    public Task WaitForNpcRepairAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        _npcRepairTcs = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.TrySetCanceled());
        return tcs.Task;
    }

    public Task<S.SellItem> WaitForSellItemAsync(ulong uniqueId, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<S.SellItem>();
        _sellItemTcs[uniqueId] = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                _sellItemTcs.Remove(uniqueId);
            });
        return tcs.Task;
    }

    public Task<bool> WaitForRepairItemAsync(ulong uniqueId, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        _repairItemTcs[uniqueId] = tcs;
        if (cancellationToken != default)
            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                _repairItemTcs.Remove(uniqueId);
            });
        return tcs.Task;
    }
}
