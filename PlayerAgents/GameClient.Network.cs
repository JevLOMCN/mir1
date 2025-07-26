using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using C = ClientPackets;
using S = ServerPackets;
using PlayerAgents.Map;

public sealed partial class GameClient
{
    private async Task SendAsync(Packet p)
    {
        if (_stream == null) return;
        var data = p.GetPacketBytes().ToArray();
        await _stream.WriteAsync(data, 0, data.Length);
    }

    private async Task ReceiveLoop()
    {
        if (_stream == null) return;
        try
        {
            while (true)
            {
                int count = await _stream.ReadAsync(_buffer, 0, _buffer.Length);
                if (count == 0) break;
                _receiveStream.Position = _receiveStream.Length;
                _receiveStream.Write(_buffer, 0, count);

                Packet? p;
                byte[] data = _receiveStream.ToArray();
                while ((p = Packet.ReceivePacket(data, out data)) != null)
                {
                    HandlePacket(p);
                }
                _receiveStream.SetLength(0);
                _receiveStream.Write(data, 0, data.Length);
            }
        }
        catch (Exception ex)
        {
            Log($"Receive error: {ex.Message}");
        }
    }

    private void HandlePacket(Packet p)
    {
        switch (p)
        {
            case S.Login l:
                if (l.Result == 3)
                {
                    Log("Account not found, creating...");
                    _ = CreateAccountAsync();
                }
                else if (l.Result != 4)
                {
                    Log($"Login failed: {l.Result}");
                }
                else
                {
                    Log("Wrong password");
                }
                break;
            case S.NewAccount na:
                if (na.Result == 8)
                {
                    Log("Account created");
                    _ = LoginAsync();
                }
                else
                {
                    Log($"Account creation failed: {na.Result}");
                }
                break;
            case S.LoginSuccess ls:
                var match = ls.Characters.FirstOrDefault(c => c.Name.Equals(_config.CharacterName, StringComparison.OrdinalIgnoreCase));
                if (match == null)
                {
                    Log($"Character '{_config.CharacterName}' not found, creating...");
                    _ = CreateCharacterAsync();
                }
                else
                {
                    Log($"Selected character '{match.Name}' (Index {match.Index})");
                    var start = new C.StartGame { CharacterIndex = match.Index };
                    FireAndForget(Task.Run(async () => { await RandomStartupDelayAsync(); await SendAsync(start); }));
                }
                break;
            case S.NewCharacterSuccess ncs:
                Log("Character created");
                var startNew = new C.StartGame { CharacterIndex = ncs.CharInfo.Index };
                FireAndForget(Task.Run(async () => { await RandomStartupDelayAsync(); await SendAsync(startNew); }));
                break;
            case S.NewCharacter nc:
                Log($"Character creation failed: {nc.Result}");
                break;
            case S.StartGame sg:
                var reason = sg.Result switch
                {
                    0 => "Disabled",
                    1 => "Not logged in",
                    2 => "Character not found",
                    3 => "Start Game Error",
                    4 => "Success",
                    _ => "Unknown"
                };
                Log($"StartGame Result: {sg.Result} ({reason})");
                break;
            case S.StartGameBanned ban:
                Log($"StartGame Banned: {ban.Reason} until {ban.ExpiryDate}");
                break;
            case S.StartGameDelay delay:
                Log($"StartGame delayed for {delay.Milliseconds} ms");
                break;
            case S.ObjectTeleportOut oto:
                if (oto.ObjectID == _objectId)
                {
                    // keep pending move target so MapChanged can record the
                    // transition source correctly
                }
                break;
            case S.ObjectTeleportIn oti:
                if (oti.ObjectID == _objectId)
                {
                    _movementSaveCts?.Cancel();
                    _movementSaveCts = null;
                }
                break;
            case S.TeleportIn:
                // do not clear pending move target here so any upcoming MapChanged
                // event will use the intended move location
                break;
            case S.MapInformation mi:
                _currentMapFile = Path.Combine(MapManager.MapDirectory, mi.FileName + ".map");
                _currentMapName = mi.Title;
                _ = LoadMapAsync();
                StartMapExpTracking(_currentMapFile);
                ReportStatus();
                break;
            case S.MapChanged mc:
                if (!string.IsNullOrEmpty(_currentMapFile) && !_dead && _pendingMovementAction.Count > 0)
                {
                    var srcFile = _currentMapFile;
                    var destFile = mc.FileName;
                    var destLoc = mc.Location;
                    var pending = _pendingMovementAction.ToList();
                    _movementSaveCts?.Cancel();
                    var cts = new CancellationTokenSource();
                    _movementSaveCts = cts;
                    FireAndForget(Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(2), cts.Token);
                            if (!cts.IsCancellationRequested)
                                foreach (var srcLoc in pending)
                                    _movementMemory.AddMovement(srcFile, srcLoc, destFile, destLoc);
                        }
                        catch (TaskCanceledException) { }
                        finally
                        {
                            _pendingMoveTarget = null;
                            _pendingMovementAction.Clear();
                            if (ReferenceEquals(_movementSaveCts, cts))
                                _movementSaveCts = null;
                        }
                    }));
                }
                CancelMovementDeleteCheck();
                PauseMapExpTracking();
                _currentMapFile = Path.Combine(MapManager.MapDirectory, mc.FileName + ".map");
                _currentMapName = mc.Title;
                _currentLocation = mc.Location;
                _navData?.Remove(_currentLocation);
                _trackedObjects.Clear();
                _ = LoadMapAsync();
                StartMapExpTracking(_currentMapFile);
                if (_awaitingHarvest)
                    _harvestComplete = true;
                ReportStatus();
                break;
            case S.UserInformation info:
                CancelMovementDeleteCheck();
                _objectId = info.ObjectID;
                _playerClass = info.Class;
                _baseStats = new BaseStats(info.Class);
                _playerName = info.Name;
                _currentLocation = info.Location;
                _navData?.Remove(_currentLocation);
                _gender = info.Gender;
                _level = info.Level;
                _experience = info.Experience;
                _hp = info.HP;
                _mp = info.MP;
                _inventory = info.Inventory;
                _equipment = info.Equipment;
                _gold = info.Gold;
                BindAll(_inventory);
                BindAll(_equipment);
                MarkStatsDirty();
                Log($"Logged in as {_playerName}");
                Log($"I am currently at location {_currentLocation.X}, {_currentLocation.Y}");
                _classTcs.TrySetResult(info.Class);
                ReportStatus();
                break;
            case S.UserLocation loc:
                if (loc.Location == _currentLocation)
                {
                    // movement request denied, revert to walking
                    _canRun = false;
                }
                if (_pendingMoveTarget.HasValue && loc.Location != _pendingMoveTarget.Value)
                {
                    _movementSaveCts?.Cancel();
                    _movementSaveCts = null;
                    _pendingMoveTarget = null;
                    _pendingMovementAction.Clear();
                }
                if (loc.Location != _currentLocation)
                    CancelMovementDeleteCheck();
                _currentLocation = loc.Location;
                _navData?.Remove(_currentLocation);
                ReportStatus();
                break;
            case S.TimeOfDay tod:
                _timeOfDay = tod.Lights;
                break;
            case S.ObjectPlayer op:
                AddTrackedObject(new TrackedObject(op.ObjectID, ObjectType.Player, op.Name, op.Location, op.Direction));
                break;
            case S.ObjectMonster om:
                AddTrackedObject(new TrackedObject(om.ObjectID, ObjectType.Monster, om.Name, om.Location, om.Direction, om.AI, om.Dead));
                break;
            case S.ObjectNPC on:
                AddTrackedObject(new TrackedObject(on.ObjectID, ObjectType.Merchant, on.Name, on.Location, on.Direction));
                _navData?.Remove(on.Location);
                if (!string.IsNullOrEmpty(_currentMapFile))
                {
                    var mapId = Path.GetFileNameWithoutExtension(_currentMapFile);
                    var entry = _npcMemory.AddNpc(on.Name, mapId, on.Location);
                    _npcEntries[on.ObjectID] = entry;

                    foreach (var kv in _recentNpcInteractions.ToList())
                        if (DateTime.UtcNow - kv.Value >= TimeSpan.FromSeconds(10))
                            _recentNpcInteractions.Remove(kv.Key);

                    var key = (entry.Name, entry.MapFile, entry.X, entry.Y);
                    if (!_recentNpcInteractions.TryGetValue(key, out var last) || DateTime.UtcNow - last >= TimeSpan.FromSeconds(10))
                    {
                        if (!IgnoreNpcInteractions && NeedsNpcInteraction(entry))
                        {
                            if (!_npcQueue.Contains(on.ObjectID))
                                _npcQueue.Enqueue(on.ObjectID);
                            if (!_dialogNpcId.HasValue && _movementSaveCts == null)
                                ProcessNextNpcInQueue();
                        }
                    }
                }
                break;
            case S.ObjectItem oi:
                AddTrackedObject(new TrackedObject(oi.ObjectID, ObjectType.Item, oi.Name, oi.Location, MirDirection.Up));
                break;
            case S.ObjectGold og:
                AddTrackedObject(new TrackedObject(og.ObjectID, ObjectType.Item, "Gold", og.Location, MirDirection.Up));
                break;
            case S.ObjectTurn ot:
                UpdateTrackedObject(ot.ObjectID, ot.Location, ot.Direction);
                break;
            case S.ObjectWalk ow:
                UpdateTrackedObject(ow.ObjectID, ow.Location, ow.Direction);
                if (ow.ObjectID == _objectId)
                {
                    CancelMovementDeleteCheck();
                    _currentLocation = ow.Location;
                    _lastMoveTime = DateTime.UtcNow;
                    _canRun = true;
                    _pendingMoveTarget = null;
                    _pendingMovementAction.Clear();
                    _navData?.Remove(_currentLocation);
                }
                break;
            case S.Pushed push:
                CancelMovementDeleteCheck();
                _currentLocation = push.Location;
                _pendingMoveTarget = null;
                _pendingMovementAction.Clear();
                break;
            case S.ObjectPushed opu:
                UpdateTrackedObject(opu.ObjectID, opu.Location, opu.Direction);
                if (opu.ObjectID == _objectId)
                {
                    CancelMovementDeleteCheck();
                    _currentLocation = opu.Location;
                    _pendingMoveTarget = null;
                    _pendingMovementAction.Clear();
                }
                break;
            case S.Struck st:
                _lastStruckAttacker = st.AttackerID;
                break;
            case S.ObjectStruck os:
                if (os.AttackerID == _objectId)
                {
                    _lastAttackTarget = os.ObjectID;
                    if (_trackedObjects.TryGetValue(os.ObjectID, out var targ) && targ.Type == ObjectType.Monster)
                    {
                        targ.EngagedWith = _objectId;
                        targ.LastEngagedTime = DateTime.UtcNow;
                    }
                }
                else if (_trackedObjects.TryGetValue(os.AttackerID, out var atk) &&
                         _trackedObjects.TryGetValue(os.ObjectID, out var target))
                {
                    if (atk.Type == ObjectType.Player && atk.Id != _objectId && target.Type == ObjectType.Monster)
                    {
                        target.EngagedWith = atk.Id;
                        target.LastEngagedTime = DateTime.UtcNow;
                    }
                    else if (atk.Type == ObjectType.Monster && target.Type == ObjectType.Player && target.Id != _objectId)
                    {
                        atk.EngagedWith = target.Id;
                        atk.LastEngagedTime = DateTime.UtcNow;
                    }
                }
                break;
            case S.DamageIndicator di:
                if (di.ObjectID == _objectId && di.Type != DamageType.Miss && _lastStruckAttacker.HasValue)
                {
                    string name = _trackedObjects.TryGetValue(_lastStruckAttacker.Value, out var atk) ? atk.Name : "Unknown";
                    Log($"{name} has attacked me for {-di.Damage} damage");
                    _lastStruckAttacker = null;
                }
                else if (_lastAttackTarget.HasValue && di.ObjectID == _lastAttackTarget.Value)
                {
                    if (di.Type == DamageType.Miss)
                    {
                        if (_trackedObjects.TryGetValue(di.ObjectID, out var targ))
                            Log($"I attacked {targ.Name} and missed");
                        else
                            Log("I attacked an unknown target and missed");
                    }
                    else
                    {
                        string name = _trackedObjects.TryGetValue(di.ObjectID, out var targ) ? targ.Name : "Unknown";
                        Log($"I have damaged {name} for {-di.Damage} damage");
                    }
                    _lastAttackTarget = null;
                }
                break;
            case S.Death death:
                Log("I have died.");
                _dead = true;
                CancelMovementDeleteCheck();
                _currentLocation = death.Location;
                _navData?.Remove(_currentLocation);
                break;
            case S.ObjectDied od:
                if (_trackedObjects.TryGetValue(od.ObjectID, out var objD))
                {
                    bool wasBlocking = IsBlocking(objD);
                    objD.Dead = true;
                    if (wasBlocking)
                        _blockingCells.TryRemove(objD.Location, out _);
                    if (objD.Type == ObjectType.Monster && AutoHarvestAIs.Contains(objD.AI) && objD.EngagedWith == _objectId)
                    {
                        FireAndForget(Task.Run(async () => await HarvestLoopAsync(objD)));
                    }
                }
                break;
            case S.ObjectHarvested oh:
                UpdateTrackedObject(oh.ObjectID, oh.Location, oh.Direction);
                if (_harvestTargetId.HasValue && oh.ObjectID == _harvestTargetId.Value)
                    _harvestComplete = true;
                break;
            case S.ObjectRemove ore:
                RemoveTrackedObject(ore.ObjectID);
                if (_harvestTargetId.HasValue && ore.ObjectID == _harvestTargetId.Value)
                    _harvestComplete = true;
                if (_lastAttackTarget.HasValue && ore.ObjectID == _lastAttackTarget.Value)
                    _lastAttackTarget = null;
                break;
            case S.Revived:
                _movementSaveCts?.Cancel();
                _movementSaveCts = null;
                if (_dead)
                {
                    Log("I have been revived.");
                }
                _dead = false;
                _hp = GetMaxHP();
                _mp = GetMaxMP();
                break;
            case S.ObjectRevived orv:
                if (orv.ObjectID == _objectId)
                {
                    if (_dead)
                    {
                        Log("I have been revived.");
                    }
                    _dead = false;
                    _hp = GetMaxHP();
                    _mp = GetMaxMP();
                }
                else if (_trackedObjects.TryGetValue(orv.ObjectID, out var objRev))
                {
                    objRev.Dead = false;
                    if (IsBlocking(objRev))
                        _blockingCells.AddOrUpdate(objRev.Location, 1, (_, v) => v + 1);
                }
                break;
            case S.NewItemInfo nii:
                ItemInfoDict[nii.Info.Index] = nii.Info;
                break;
            case S.GainedItem gi:
                Bind(gi.Item);
                UserItem? invItem = null;
                if (_inventory != null)
                {
                    invItem = AddItem(gi.Item);
                }
                _lastPickedItem = invItem ?? gi.Item;
                if (gi.Item.Info != null)
                    Log($"Gained item: {gi.Item.Info.FriendlyName}");
                // Let the AI handle overweight checks so it can track dropped items
                break;
            case S.GainedGold gg:
                _gold += gg.Gold;
                Log($"Gained {gg.Gold} gold");
                break;
            case S.GainExperience ge:
                _experience += ge.Amount;
                _mapExpGained += ge.Amount;
                Log($"I gained {ge.Amount} experience");
                break;
            case S.LevelChanged lc:
                _level = lc.Level;
                _experience = lc.Experience;
                MarkStatsDirty();
                ReportStatus();
                // reset exp rate tracking when our level changes
                if (!string.IsNullOrEmpty(_currentMapFile))
                    StartMapExpTracking(_currentMapFile);
                break;
            case S.Chat chat:
                HandleTradeFailChat(chat.Message);
                if (chat.Type == ChatType.WhisperIn)
                    HandleDebugCommand(chat.Message);
                break;
            case S.ObjectChat oc:
                HandleTradeFailChat(oc.Text);
                if (oc.Type == ChatType.WhisperIn)
                    HandleDebugCommand(oc.Text);
                break;
            case S.LoseGold lg:
                if (lg.Gold > _gold) _gold = 0;
                else _gold -= lg.Gold;
                Log($"Lost {lg.Gold} gold");
                break;
            case S.HealthChanged hc:
                _hp = hc.HP;
                _mp = hc.MP;
                _dead = hc.HP <= 0;
                ReportStatus();
                break;
            case S.UseItem ui:
                if (ui.Success && _inventory != null)
                {
                    int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == ui.UniqueID);
                    if (idx >= 0)
                    {
                        var it = _inventory[idx];
                        if (it != null)
                        {
                            if (it.Count > 1)
                                it.Count--;
                            else
                                _inventory[idx] = null;
                        }
                    }
                }
                break;
            case S.MoveItem mi:
                if (mi.Grid == MirGridType.Inventory && _inventory != null && mi.Success && mi.From >= 0 && mi.To >= 0 && mi.From < _inventory.Length && mi.To < _inventory.Length)
                {
                    var tmp = _inventory[mi.To];
                    _inventory[mi.To] = _inventory[mi.From];
                    _inventory[mi.From] = tmp;
                }
                break;
            case S.EquipItem ei:
                if (ei.Grid == MirGridType.Inventory && ei.Success && _inventory != null && _equipment != null)
                {
                    int invIndex = Array.FindIndex(_inventory, x => x != null && x.UniqueID == ei.UniqueID);
                if (invIndex >= 0 && ei.To >= 0 && ei.To < _equipment.Length)
                {
                    var temp = _equipment[ei.To];
                    _equipment[ei.To] = _inventory[invIndex];
                    _inventory[invIndex] = temp;
                    MarkStatsDirty();
                }
            }
            break;
            case S.RemoveItem ri:
                if (ri.Grid == MirGridType.Inventory && ri.Success && _inventory != null && _equipment != null)
                {
                    int eqIndex = Array.FindIndex(_equipment, x => x != null && x.UniqueID == ri.UniqueID);
                    if (eqIndex >= 0 && ri.To >= 0 && ri.To < _inventory.Length)
                    {
                        _inventory[ri.To] = _equipment[eqIndex];
                        _equipment[eqIndex] = null;
                        MarkStatsDirty();
                    }
                }
                break;
            case S.DropItem di:
                if (di.Success && _inventory != null)
                {
                    int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == di.UniqueID);
                    if (idx >= 0)
                    {
                        var it = _inventory[idx];
                        if (it != null)
                        {
                            if (di.Count >= it.Count)
                                _inventory[idx] = null;
                            else
                                it.Count -= di.Count;
                        }
                    }
                    if (_lastPickedItem != null && _lastPickedItem.UniqueID == di.UniqueID)
                        _lastPickedItem = null;
                }
                break;
            case S.DeleteItem di:
                if (_inventory != null)
                {
                    int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == di.UniqueID);
                    if (idx >= 0)
                    {
                        var it = _inventory[idx];
                        if (it != null)
                        {
                            if (it.Count > di.Count)
                                it.Count -= di.Count;
                            else
                                _inventory[idx] = null;
                        }
                    }
                }
                if (_equipment != null)
                {
                    int idx = Array.FindIndex(_equipment, x => x != null && x.UniqueID == di.UniqueID);
                    if (idx >= 0)
                    {
                        var it = _equipment[idx];
                        if (it != null)
                        {
                            if (it.Count > di.Count)
                                it.Count -= di.Count;
                            else
                                _equipment[idx] = null;
                            MarkStatsDirty();
                        }
                    }
                }
                break;
            case S.RefreshItem rfi:
                var newItem = rfi.Item;
                Bind(newItem);
                if (_inventory != null)
                {
                    int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == newItem.UniqueID);
                    if (idx >= 0) _inventory[idx] = newItem;
                }
                if (_equipment != null)
                {
                    int idx = Array.FindIndex(_equipment, x => x != null && x.UniqueID == newItem.UniqueID);
                    if (idx >= 0) _equipment[idx] = newItem;
                    MarkStatsDirty();
                }
                break;
            case S.NPCResponse nr:
                DeliverNpcResponse(nr);
                break;
            case S.NPCGoods goods:
                ProcessNpcGoods(goods.List, goods.Type);
                break;
            case S.NPCSell:
                if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var npcSellEntry) && npcSellEntry.CanSell)
                {
                    if (HasUnknownSellTypes(npcSellEntry))
                    {
                        FireAndForget(Task.Run(async () => { await HandleNpcSellAsync(npcSellEntry); _npcSellTcs?.TrySetResult(true); _npcSellTcs = null; }));
                    }
                    else
                    {
                        ProcessNpcActionQueue();
                        _npcSellTcs?.TrySetResult(true);
                        _npcSellTcs = null;
                    }
                }
                else
                {
                    ProcessNpcActionQueue();
                    _npcSellTcs?.TrySetResult(true);
                    _npcSellTcs = null;
                }
                break;
            case S.NPCRepair:
            case S.NPCSRepair:
                if (_dialogNpcId.HasValue && _npcEntries.TryGetValue(_dialogNpcId.Value, out var npcRepairEntry))
                {
                    bool special = p is S.NPCSRepair;
                    bool changed = false;
                    if (!npcRepairEntry.CanRepair)
                    {
                        npcRepairEntry.CanRepair = true;
                        changed = true;
                    }
                    if (special && !npcRepairEntry.CanSpecialRepair)
                    {
                        npcRepairEntry.CanSpecialRepair = true;
                        changed = true;
                    }
                    if (changed)
                        _npcMemory.SaveChanges();
                    if (HasUnknownRepairTypes(npcRepairEntry, special))
                    {
                        FireAndForget(Task.Run(async () => { await HandleNpcRepairAsync(npcRepairEntry, special); _npcRepairTcs?.TrySetResult(true); _npcRepairTcs = null; }));
                    }
                    else
                    {
                        ProcessNpcActionQueue();
                        _npcRepairTcs?.TrySetResult(true);
                        _npcRepairTcs = null;
                    }
                }
                break;
            case S.SellItem sell:
                if (_sellItemTcs.TryGetValue(sell.UniqueID, out var sellTcs))
                {
                    sellTcs.TrySetResult(sell);
                    _sellItemTcs.Remove(sell.UniqueID);
                }
                if (_pendingSellChecks.TryGetValue(sell.UniqueID, out var infoSell))
                {
                    if (sell.Success)
                    {
                        string itemName = string.Empty;
                        var inventoryItem = _inventory != null ? Array.Find(_inventory, x => x != null && x.UniqueID == sell.UniqueID) : null;
                        var eqItem = inventoryItem == null && _equipment != null ? Array.Find(_equipment, x => x != null && x.UniqueID == sell.UniqueID) : null;
                        var it = inventoryItem ?? eqItem;
                        if (it != null && it.Info != null) itemName = it.Info.FriendlyName;
                        infoSell.entry.SellItemTypes ??= new List<ItemType>();
                        if (!infoSell.entry.SellItemTypes.Contains(infoSell.type))
                        {
                            infoSell.entry.SellItemTypes.Add(infoSell.type);
                            _npcMemory.SaveChanges();
                        }
                        if (_inventory != null)
                        {
                            int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == sell.UniqueID);
                            if (idx >= 0)
                            {
                                var item = _inventory[idx];
                                if (item != null)
                                {
                                    if (item.Count <= sell.Count)
                                        _inventory[idx] = null;
                                    else
                                        item.Count -= sell.Count;
                                }
                            }
                        }
                        if (_equipment != null)
                        {
                            int idx = Array.FindIndex(_equipment, x => x != null && x.UniqueID == sell.UniqueID);
                            if (idx >= 0)
                            {
                                var item = _equipment[idx];
                                if (item != null)
                                {
                                    if (item.Count <= sell.Count)
                                        _equipment[idx] = null;
                                    else
                                        item.Count -= sell.Count;
                                }
                            }
                        }
                        if (_lastPickedItem != null && _lastPickedItem.UniqueID == sell.UniqueID)
                            _lastPickedItem = null;
                        Log($"Sold {itemName} x{sell.Count} to {infoSell.entry.Name}");
                    }
                    else
                    {
                        infoSell.entry.CannotSellItemTypes ??= new List<ItemType>();
                        if (!infoSell.entry.CannotSellItemTypes.Contains(infoSell.type))
                        {
                            infoSell.entry.CannotSellItemTypes.Add(infoSell.type);
                            _npcMemory.SaveChanges();
                        }
                    }
                    _pendingSellChecks.Remove(sell.UniqueID);
                }
                break;
            case S.RepairItem repair:
                // Server acknowledges the repair request but success will be
                // indicated by ItemRepaired or a chat message.
                break;
            case S.ItemRepaired ir:
                if (_repairItemTcs.TryGetValue(ir.UniqueID, out var repTcs))
                {
                    repTcs.TrySetResult(true);
                    _repairItemTcs.Remove(ir.UniqueID);
                }
                if (_pendingRepairChecks.ContainsKey(ir.UniqueID))
                {
                    _pendingRepairChecks.Remove(ir.UniqueID);
                }
                bool eqChanged = false;
                if (_inventory != null)
                {
                    int idx = Array.FindIndex(_inventory, x => x != null && x.UniqueID == ir.UniqueID);
                    if (idx >= 0)
                    {
                        var item = _inventory[idx];
                        if (item != null)
                        {
                            item.MaxDura = ir.MaxDura;
                            item.CurrentDura = ir.CurrentDura;
                        }
                    }
                }
                if (_equipment != null)
                {
                    int idx = Array.FindIndex(_equipment, x => x != null && x.UniqueID == ir.UniqueID);
                    if (idx >= 0)
                    {
                        var item = _equipment[idx];
                        if (item != null)
                        {
                            item.MaxDura = ir.MaxDura;
                            item.CurrentDura = ir.CurrentDura;
                            eqChanged = true;
                        }
                    }
                }
                if (eqChanged)
                    MarkStatsDirty();
                break;
            case S.KeepAlive keep:
                _pingTime = Environment.TickCount64 - keep.Time;
                break;
            default:
                break;
        }
    }

    private async Task LoadMapAsync()
    {
        if (string.IsNullOrEmpty(_currentMapFile)) return;
        _mapData = await MapManager.GetMapAsync(_currentMapFile);
        if (_mapData != null)
        {
            _navData = _navDataManager.GetNavData(_currentMapFile, _mapData.WalkableCells);
        }
    }

    private async Task KeepAliveLoop()
    {
        while (_stream != null)
        {
            await Task.Delay(5000);
            try
            {
                await SendAsync(new C.KeepAlive { Time = Environment.TickCount64 });
            }
            catch (Exception ex)
            {
                Log($"KeepAlive error: {ex.Message}");
            }
        }
    }

    private async Task MaintenanceLoop()
    {
        while (_stream != null)
        {
            await Task.Delay(5000);
            try
            {
                _npcMemory.CheckForUpdates();
                _movementMemory.CheckForUpdates();
                _expRateMemory.CheckForUpdates();
                _navData?.SaveIfNeeded(TimeSpan.FromSeconds(60));
                CheckNpcInteractionTimeout();
            }
            catch (Exception ex)
            {
                Log($"Maintenance error: {ex.Message}");
            }
        }
    }


    private void HandleTradeFailChat(string text)
    {
        if (_pendingSellChecks.Count > 0 && text.Contains("cannot sell", StringComparison.OrdinalIgnoreCase))
        {
            var kv = _pendingSellChecks.First();
            var entry = kv.Value.entry;
            var type = kv.Value.type;
            entry.CannotSellItemTypes ??= new List<ItemType>();
            if (!entry.CannotSellItemTypes.Contains(type))
            {
                entry.CannotSellItemTypes.Add(type);
                _npcMemory.SaveChanges();
            }
            _pendingSellChecks.Remove(kv.Key);
        }

        if (_pendingRepairChecks.Count > 0 && text.Contains("cannot repair", StringComparison.OrdinalIgnoreCase))
        {
            var kv = _pendingRepairChecks.First();
            if (_repairItemTcs.TryGetValue(kv.Key, out var repTcs))
            {
                repTcs.TrySetResult(false);
                _repairItemTcs.Remove(kv.Key);
            }
            _pendingRepairChecks.Remove(kv.Key);
        }
    }
}
