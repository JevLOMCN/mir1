using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Shared;

public sealed partial class GameClient
{
    public bool TryFindNearestNpc(ItemType type, out uint id, out Point location, out NpcEntry? entry, bool includeUnknowns = true)
    {
        id = 0;
        location = default;
        entry = null;
        if (string.IsNullOrEmpty(_currentMapFile))
            return false;

        int bestDist = int.MaxValue;
        string map = Path.GetFileNameWithoutExtension(_currentMapFile);

        foreach (var e in _npcMemory.GetAll())
        {
            if (e.MapFile != map) continue;
            bool knows = e.SellItemTypes != null && e.SellItemTypes.Contains(type);
            bool unknown = e.CanSell &&
                (e.SellItemTypes == null || !e.SellItemTypes.Contains(type)) &&
                (e.CannotSellItemTypes == null || !e.CannotSellItemTypes.Contains(type));
            if (!knows && (!includeUnknowns || !unknown)) continue;

            int dist = Functions.MaxDistance(_currentLocation, new Point(e.X, e.Y));
            if (dist < bestDist)
            {
                bestDist = dist;
                entry = e;
                location = new Point(e.X, e.Y);
            }
        }

        if (entry != null)
        {
            foreach (var kv in _npcEntries)
            {
                if (kv.Value == entry)
                {
                    id = kv.Key;
                    break;
                }
            }
        }

        return entry != null;
    }

    public bool TryFindNearestRepairNpc(ItemType type, out uint id, out Point location, out NpcEntry? entry, bool includeUnknowns = true, bool special = false)
    {
        id = 0;
        location = default;
        entry = null;
        if (string.IsNullOrEmpty(_currentMapFile))
            return false;

        int bestDist = int.MaxValue;
        string map = Path.GetFileNameWithoutExtension(_currentMapFile);

        foreach (var e in _npcMemory.GetAll())
        {
            if (e.MapFile != map) continue;
            bool knows = special ? (e.SpecialRepairItemTypes != null && e.SpecialRepairItemTypes.Contains(type))
                                 : (e.RepairItemTypes != null && e.RepairItemTypes.Contains(type));
            bool unknown = (special ? e.CanSpecialRepair : e.CanRepair) &&
                ((special ? e.SpecialRepairItemTypes : e.RepairItemTypes) == null || !(special ? e.SpecialRepairItemTypes : e.RepairItemTypes)!.Contains(type)) &&
                ((special ? e.CannotSpecialRepairItemTypes : e.CannotRepairItemTypes) == null || !(special ? e.CannotSpecialRepairItemTypes : e.CannotRepairItemTypes)!.Contains(type));
            if (!knows && (!includeUnknowns || !unknown)) continue;

            int dist = Functions.MaxDistance(_currentLocation, new Point(e.X, e.Y));
            if (dist < bestDist)
            {
                bestDist = dist;
                entry = e;
                location = new Point(e.X, e.Y);
            }
        }

        if (entry != null)
        {
            foreach (var kv in _npcEntries)
            {
                if (kv.Value == entry)
                {
                    id = kv.Key;
                    break;
                }
            }
        }

        return entry != null;
    }

    public bool TryFindNearestNpc(IEnumerable<ItemType> types, out uint id, out Point location, out NpcEntry? entry, out List<ItemType> matchedTypes, bool includeUnknowns = true)
    {
        id = 0;
        location = default;
        entry = null;
        matchedTypes = new List<ItemType>();
        if (string.IsNullOrEmpty(_currentMapFile))
            return false;

        int bestDist = int.MaxValue;
        string map = Path.GetFileNameWithoutExtension(_currentMapFile);

        foreach (var e in _npcMemory.GetAll())
        {
            if (e.MapFile != map) continue;
            var sells = new List<ItemType>();
            foreach (var t in types)
            {
                bool knows = e.SellItemTypes != null && e.SellItemTypes.Contains(t);
                bool unknown = e.CanSell &&
                    (e.SellItemTypes == null || !e.SellItemTypes.Contains(t)) &&
                    (e.CannotSellItemTypes == null || !e.CannotSellItemTypes.Contains(t));
                if (knows || (includeUnknowns && unknown))
                    sells.Add(t);
            }
            if (sells.Count == 0) continue;

            int dist = Functions.MaxDistance(_currentLocation, new Point(e.X, e.Y));
            if (dist < bestDist)
            {
                bestDist = dist;
                entry = e;
                location = new Point(e.X, e.Y);
                matchedTypes = sells;
            }
        }

        if (entry != null)
        {
            foreach (var kv in _npcEntries)
            {
                var e = kv.Value;
                if (ReferenceEquals(e, entry) ||
                    (e.Name == entry.Name &&
                     e.MapFile == entry.MapFile &&
                     e.X == entry.X &&
                     e.Y == entry.Y))
                {
                    id = kv.Key;
                    break;
                }
            }
        }

        return entry != null;
    }

    public bool TryFindNearestRepairNpc(IEnumerable<ItemType> types, out uint id, out Point location, out NpcEntry? entry, out List<ItemType> matchedTypes, bool includeUnknowns = true, bool special = false)
    {
        id = 0;
        location = default;
        entry = null;
        matchedTypes = new List<ItemType>();
        if (string.IsNullOrEmpty(_currentMapFile))
            return false;

        int bestDist = int.MaxValue;
        string map = Path.GetFileNameWithoutExtension(_currentMapFile);

        foreach (var e in _npcMemory.GetAll())
        {
            if (e.MapFile != map) continue;
            var repairs = new List<ItemType>();
            foreach (var t in types)
            {
                bool knows = special ? (e.SpecialRepairItemTypes != null && e.SpecialRepairItemTypes.Contains(t))
                                     : (e.RepairItemTypes != null && e.RepairItemTypes.Contains(t));
                bool unknown = (special ? e.CanSpecialRepair : e.CanRepair) &&
                    ((special ? e.SpecialRepairItemTypes : e.RepairItemTypes) == null || !(special ? e.SpecialRepairItemTypes : e.RepairItemTypes)!.Contains(t)) &&
                    ((special ? e.CannotSpecialRepairItemTypes : e.CannotRepairItemTypes) == null || !(special ? e.CannotSpecialRepairItemTypes : e.CannotRepairItemTypes)!.Contains(t));
                if (knows || (includeUnknowns && unknown))
                    repairs.Add(t);
            }
            if (repairs.Count == 0) continue;

            int dist = Functions.MaxDistance(_currentLocation, new Point(e.X, e.Y));
            if (dist < bestDist)
            {
                bestDist = dist;
                entry = e;
                location = new Point(e.X, e.Y);
                matchedTypes = repairs;
            }
        }

        if (entry != null)
        {
            foreach (var kv in _npcEntries)
            {
                if (kv.Value == entry)
                {
                    id = kv.Key;
                    break;
                }
            }
        }

        return entry != null;
    }

    public async Task SellItemsToNpcAsync(uint npcId, IReadOnlyList<(UserItem item, ushort count)> items)
    {
        var entry = await ResolveNpcEntryAsync(npcId);
        if (entry == null) return;
        var interaction = new NPCInteraction(this, npcId);
        var page = await interaction.BeginAsync();
        string[] sellKeys = { "@BUYSELLNEW", "@BUYSELL", "@SELL" };
        var sellKey = page.Buttons.Select(b => b.Key).FirstOrDefault(k => sellKeys.Contains(k.ToUpper())) ?? "@SELL";
        if (sellKey.Equals("@BUYBACK", StringComparison.OrdinalIgnoreCase))
            return;
        using (var cts = new System.Threading.CancellationTokenSource(2000))
        {
            var waitTask = WaitForLatestNpcResponseAsync(cts.Token);
            await interaction.SelectFromMainAsync(sellKey);
            try
            {
                await waitTask;
            }
            catch (OperationCanceledException)
            {
            }
        }
        foreach (var (item, count) in items)
        {
            if (item.Info == null) continue;
            _pendingSellChecks[item.UniqueID] = (entry, item.Info.Type);
            using var cts = new System.Threading.CancellationTokenSource(2000);
            var waitTask = WaitForSellItemAsync(item.UniqueID, cts.Token);
            await SellItemAsync(item.UniqueID, count);
            try
            {
                await waitTask;
            }
            catch (OperationCanceledException)
            {
            }
            await Task.Delay(200);
        }

        // clear out any leftover npc responses that may arrive after selling
        try
        {
            using var cts = new System.Threading.CancellationTokenSource(200);
            await WaitForLatestNpcResponseAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task RepairItemsAtNpcAsync(uint npcId)
    {
        var entry = await ResolveNpcEntryAsync(npcId);
        if (entry == null) return;
        var interaction = new NPCInteraction(this, npcId);
        var page = await interaction.BeginAsync();
        string[] repairKeys = { "@SREPAIR", "@REPAIR" };
        var repairKey = page.Buttons.Select(b => b.Key).FirstOrDefault(k => repairKeys.Contains(k.ToUpper())) ?? "@REPAIR";
        bool special = repairKey.Equals("@SREPAIR", StringComparison.OrdinalIgnoreCase);
        if (repairKey.Equals("@BUYBACK", StringComparison.OrdinalIgnoreCase))
            return;
        using (var cts = new CancellationTokenSource(2000))
        {
            var waitTask = WaitForLatestNpcResponseAsync(cts.Token);
            await interaction.SelectFromMainAsync(repairKey);
            try
            {
                await waitTask;
            }
            catch (OperationCanceledException)
            {
            }
        }
        await RepairNeededItemsAsync(entry, special);
        try
        {
            using var cts = new CancellationTokenSource(200);
            await WaitForLatestNpcResponseAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private UserItem? AddItem(UserItem item)
    {
        if (_inventory == null) return null;

        if (item.Info != null && item.Info.StackSize > 1)
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                var temp = _inventory[i];
                if (temp == null || temp.Info != item.Info || temp.Count >= temp.Info.StackSize) continue;

                if (item.Count + temp.Count <= temp.Info.StackSize)
                {
                    temp.Count += item.Count;
                    return temp;
                }

                item.Count -= (ushort)(temp.Info.StackSize - temp.Count);
                temp.Count = temp.Info.StackSize;
            }
        }

        for (int i = 0; i < _inventory.Length; i++)
        {
            if (_inventory[i] == null)
            {
                _inventory[i] = item;
                return item;
            }
        }

        return null;
    }

    private async Task<NpcEntry?> ResolveNpcEntryAsync(uint npcId, int timeoutMs = 2000)
    {
        int waited = 0;
        while (waited < timeoutMs)
        {
            if (_npcEntries.TryGetValue(npcId, out var e))
                return e;
            await Task.Delay(50);
            waited += 50;
        }
        return null;
    }

    public async Task<uint> ResolveNpcIdAsync(NpcEntry entry, int timeoutMs = 2000)
    {
        int waited = 0;
        while (waited < timeoutMs)
        {
            foreach (var kv in _npcEntries)
            {
                var e = kv.Value;
                if (ReferenceEquals(e, entry) ||
                    (e.Name == entry.Name &&
                     e.MapFile == entry.MapFile &&
                     e.X == entry.X &&
                     e.Y == entry.Y))
                {
                    return kv.Key;
                }
            }

            await Task.Delay(50);
            waited += 50;
        }

        return 0u;
    }
}
