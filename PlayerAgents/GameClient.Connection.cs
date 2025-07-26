using System;
using System.Net.Sockets;
using System.IO;
using C = ClientPackets;
using S = ServerPackets;

public sealed partial class GameClient
{
    public async Task ConnectAsync()
    {
        _client = new TcpClient { NoDelay = true };
        await _client.ConnectAsync(_config.ServerIP, _config.ServerPort);
        _stream = _client.GetStream();
        UpdateAction("connected");
        _canRun = false;
        FireAndForget(Task.Run(ReceiveLoop));
        FireAndForget(Task.Run(KeepAliveLoop));
        FireAndForget(Task.Run(MaintenanceLoop));
    }

    public async Task LoginAsync()
    {
        if (_stream == null) return;
        UpdateAction("logging in");
        var ver = new C.ClientVersion { VersionHash = Array.Empty<byte>() };
        await RandomStartupDelayAsync();
        await SendAsync(ver);

        var login = new C.Login { AccountID = _config.AccountID, Password = _config.Password };
        await RandomStartupDelayAsync();
        await SendAsync(login);
    }

    private async Task CreateAccountAsync()
    {
        if (_stream == null) return;
        UpdateAction("creating account");
        var acc = new C.NewAccount
        {
            AccountID = _config.AccountID,
            Password = _config.Password
        };
        await RandomStartupDelayAsync();
        await SendAsync(acc);
    }

    private async Task CreateCharacterAsync()
    {
        if (_stream == null) return;
        UpdateAction("creating character");
        var chr = new C.NewCharacter
        {
            Name = _config.CharacterName,
            Gender = (MirGender)_random.Next(Enum.GetValues<MirGender>().Length),
            Class = (MirClass)_random.Next(Enum.GetValues<MirClass>().Length)
        };
        await RandomStartupDelayAsync();
        await SendAsync(chr);
    }
}
