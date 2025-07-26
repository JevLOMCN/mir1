using System;
using System.Threading.Tasks;
using C = ClientPackets;

public sealed partial class GameClient
{
    public async Task DisconnectAsync()
    {
        try
        {
            if (_stream != null)
                await SendAsync(new C.Disconnect());
        }
        catch { }

        try { _stream?.Close(); } catch { }
        _stream = null;
        try { _client?.Close(); } catch { }
        _client = null;
    }
}
