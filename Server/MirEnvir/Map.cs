using Server.MirDatabase;
using Server.MirObjects;
using S = ServerPackets;

namespace Server.MirEnvir
{
    public class Map
    {
        private static Envir Envir
        {
            get { return Envir.Main; }
        }

        protected static MessageQueue MessageQueue
        {
            get { return MessageQueue.Instance; }
        }

        public MapInfo Info;

        public int Thread = 0;

        public int Width, Height;
        public Cell[,] Cells;
        public List<Point> WalkableCells;
        public long LightningTime, FireTime;
        public int MonsterCount;

        public List<NPCObject> NPCs = new List<NPCObject>();
        public List<SpellObject> Spells = new List<SpellObject>();
        public List<PlayerObject> Players = new List<PlayerObject>();
        public List<MapRespawn> Respawns = new List<MapRespawn>();
        public List<DelayedAction> ActionList = new List<DelayedAction>();

        public Map(MapInfo info)
        {
            Info = info;
            Thread = Envir.Random.Next(Settings.ThreadLimit);
        }

        private void LoadMapCells(byte[] fileBytes)
        {
            int offSet = 0;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];

            offSet = 56;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)                            
                {
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        Cells[x, y] = Cell.LowWall;
                    offSet += 2;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        Cells[x, y] = Cell.HighWall;
                    offSet += 2;
                    if (Cells[x, y] == null) Cells[x, y] = new Cell { Attribute = CellAttribute.Walk };
                }
        }

        public bool Load()
        {
            try
            {
                string fileName = Path.Combine(Settings.MapPath, Info.FileName + ".map");
                if (File.Exists(fileName))
                {
                    byte[] fileBytes = File.ReadAllBytes(fileName);
                    LoadMapCells(fileBytes);

                    GetWalkableCells();
                    
                    for (int i = 0; i < Info.Respawns.Count; i++)
                    {
                        MapRespawn info = new MapRespawn(Info.Respawns[i]);
                        if (info.Monster == null) continue;
                        info.Map = this;
                        info.WalkableCells = WalkableCells.Where(x =>
                        x.X <= info.Info.Location.X + info.Info.Spread &&
                        x.X >= info.Info.Location.X - info.Info.Spread &&
                        x.Y <= info.Info.Location.Y + info.Info.Spread &&
                        x.Y >= info.Info.Location.Y - info.Info.Spread).ToList();

                        Respawns.Add(info);

                        if ((info.Info.SaveRespawnTime) && (info.Info.RespawnTicks != 0))
                            Envir.SavedSpawns.Add(info);
                    }

                    for (int i = 0; i < Info.NPCs.Count; i++)
                    {
                        NPCInfo info = Info.NPCs[i];
                        if (!ValidPoint(info.Location)) continue;

                        AddObject(new NPCObject(info) { CurrentMap = this });
                    }

                    for (int i = 0; i < Info.SafeZones.Count; i++)
                        CreateSafeZone(Info.SafeZones[i]);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Enqueue(ex);
            }

            MessageQueue.Enqueue("Failed to Load Map: " + Info.Title);
            MessageQueue.Enqueue("Filename: " + Info.FileName);
            return false;
        }

        public void GetWalkableCells()
        {
            if (WalkableCells == null)
            {
                WalkableCells = new List<Point>();

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        if (Cells[x, y].Attribute == CellAttribute.Walk)
                            WalkableCells.Add(new Point(x, y));
            }
        }

        private void CreateSafeZone(SafeZoneInfo info)
        {
            if (Settings.SafeZoneBorder)
            {
                for (int y = info.Location.Y - info.Size; y <= info.Location.Y + info.Size; y++)
                {
                    if (y < 0) continue;
                    if (y >= Height) break;
                    for (int x = info.Location.X - info.Size; x <= info.Location.X + info.Size; x += Math.Abs(y - info.Location.Y) == info.Size ? 1 : info.Size * 2)
                    {
                        if (x < 0) continue;
                        if (x >= Width) break;
                        if (!Cells[x, y].Valid) continue;

                        SpellObject spell = new SpellObject
                        {
                            ExpireTime = long.MaxValue,
                            Spell = Spell.TrapHexagon,
                            TickSpeed = int.MaxValue,
                            CurrentLocation = new Point(x, y),
                            CurrentMap = this,
                            Decoration = true
                        };

                        Cells[x, y].Add(spell);

                        spell.Spawned();
                    }
                }
            }

            if (Settings.SafeZoneHealing)
            {
                for (int y = info.Location.Y - info.Size; y <= info.Location.Y + info.Size; y++)
                {
                    if (y < 0) continue;
                    if (y >= Height) break;
                    for (int x = info.Location.X - info.Size; x <= info.Location.X + info.Size; x++)
                    {
                        if (x < 0) continue;
                        if (x >= Width) break;
                        if (!Cells[x, y].Valid) continue;

                        SpellObject spell = new SpellObject
                            {
                                ExpireTime = long.MaxValue,
                                Value = 25,
                                TickSpeed = 2000,
                                Spell = Spell.Healing,
                                CurrentLocation = new Point(x, y),
                                CurrentMap = this
                            };

                        Cells[x, y].Add(spell);

                        spell.Spawned();
                    }
                }
            }


        }

        public Cell GetCell(Point location)
        {
            return Cells[location.X, location.Y];
        }

        public Cell GetCell(int x, int y)
        {
            return Cells[x, y];
        }

        public bool ValidPoint(Point location)
        {
            return location.X >= 0 && location.X < Width && location.Y >= 0 && location.Y < Height && GetCell(location).Valid;
        }
        public bool ValidPoint(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height && GetCell(x, y).Valid;
        }

        public void Process()
        {
            ProcessRespawns();

            if ((Info.Lightning) && Envir.Time > LightningTime)
            {
                LightningTime = Envir.Time + Envir.Random.Next(3000, 15000);
                for (int i = Players.Count - 1; i >= 0; i--)
                {
                    PlayerObject player = Players[i];
                    Point location;
                    if (Envir.Random.Next(4) == 0)
                    {
                        location = player.CurrentLocation;          
                    }
                    else
                        location = new Point(player.CurrentLocation.X - 10 + Envir.Random.Next(20), player.CurrentLocation.Y - 10 + Envir.Random.Next(20));

                    if (!ValidPoint(location)) continue;
                }
            }

            if ((Info.Fire) && Envir.Time > FireTime)
            {
                FireTime = Envir.Time + Envir.Random.Next(3000, 15000);
                for (int i = Players.Count - 1; i >= 0; i--)
                {
                    PlayerObject player = Players[i];
                    Point location;
                    if (Envir.Random.Next(4) == 0)
                    {
                        location = player.CurrentLocation;

                    }
                    else
                        location = new Point(player.CurrentLocation.X - 10 + Envir.Random.Next(20), player.CurrentLocation.Y - 10 + Envir.Random.Next(20));

                    if (!ValidPoint(location)) continue;
                }
            }

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (Envir.Time < ActionList[i].Time) continue;
                Process(ActionList[i]);
                ActionList.RemoveAt(i);
            }
        }

        private void ProcessRespawns()
        {
            bool Success = true;
            for (int i = 0; i < Respawns.Count; i++)
            {
                MapRespawn respawn = Respawns[i];
                if ((respawn.Info.RespawnTicks != 0) && (Envir.RespawnTick.CurrentTickcounter < respawn.NextSpawnTick)) continue;
                if ((respawn.Info.RespawnTicks == 0) && (Envir.Time < respawn.RespawnTime)) continue;

                if (respawn.Count < (respawn.Info.Count * Envir.SpawnMultiplier))
                {
                    int count = (respawn.Info.Count * Envir.SpawnMultiplier) - respawn.Count;

                    for (int c = 0; c < count; c++)
                        Success = respawn.Spawn();
                }
                if (Success)
                {
                    respawn.ErrorCount = 0;
                    long delay = Math.Max(1, respawn.Info.Delay - respawn.Info.RandomDelay + Envir.Random.Next(respawn.Info.RandomDelay * 2));
                    respawn.RespawnTime = Envir.Time + (delay * Settings.Minute);
                    if (respawn.Info.RespawnTicks != 0)
                    {
                        respawn.NextSpawnTick = Envir.RespawnTick.CurrentTickcounter + (ulong)respawn.Info.RespawnTicks;
                        if (respawn.NextSpawnTick > long.MaxValue)//since nextspawntick is ulong this simple thing allows an easy way of preventing the counter from overflowing
                            respawn.NextSpawnTick -= long.MaxValue;
                    }
                }
                else
                {
                    respawn.RespawnTime = Envir.Time + 1 * Settings.Minute; // each time it fails to spawn, give it a 1 minute cooldown
                    if (respawn.ErrorCount < 5)
                        respawn.ErrorCount++;
                    else
                    {
                        if (respawn.ErrorCount == 5)
                        {
                            respawn.ErrorCount++;

                            Logger.GetLogger(LogType.Spawn).Info($"Failed to spawn: " +
                                $"mapindex: {respawn.Map.Info.Index}, " +
                                $"mob info: index: {respawn.Info.MonsterIndex}, " +
                                $"spawncoords ({respawn.Info.Location.X}:{respawn.Info.Location.Y}), " +
                                $"range {respawn.Info.Spread}");
                        }

                    }
                }
            }
        }

        public void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Magic:
                    CompleteMagic(action.Params);
                    break;
                case DelayedType.Spawn:
                    MapObject obj = (MapObject)action.Params[0];

                    switch(obj.Race)
                    {
                        case ObjectType.Monster:
                            {
                                MonsterObject mob = (MonsterObject)action.Params[0];
                                mob.Spawn(this, (Point)action.Params[1]);
                                if (action.Params.Length > 2) ((MonsterObject)action.Params[2]).SlaveList.Add(mob);
                            }
                            break;
                        case ObjectType.Spell:
                            {
                                SpellObject spell = (SpellObject)action.Params[0];
                                AddObject(spell);
                                spell.Spawned();
                            }
                            break;
                    }
                    break;
            }
        }

         /**
         * return the coordinates of effect coordinates within an n x n square (n should be odd number. i.e. 3x3, 5x5, 7x7)
         * then use GetCell() in Map.cs to retrive real objects
         * default 3x3
         */
        public static List<Point> GetPointsInEffectiveSquare(Point location, int mapWidth, int mapHeight, int squareEdgeLength = 3)
        {
            var pointsWithinTheMap = new List<Point>();
            int fallBackSpread = 1;
            int spread;
            if (squareEdgeLength > 1)
            {
                spread = (int)((squareEdgeLength - 1) / 2);
            } else
            {
                spread = fallBackSpread; // 3x3
            }

            for (int y = location.Y - spread; y <= location.Y + spread; y++)
            {
                if (y < 0) continue;
                if (y >= mapHeight) break;

                for (int x = location.X - spread; x <= location.X + spread; x++)
                {
                    if (x < 0) continue;
                    if (x >= mapWidth) break;
                    pointsWithinTheMap.Add(new Point(x, y));
                }
            }
            return pointsWithinTheMap;
        }

        private void CompleteMagic(IList<object> data)
        {
            bool train = false;
            HumanObject player = (HumanObject)data[0];
            UserMagic magic = (UserMagic)data[1];

            if (player == null || player.Info == null || player.Node == null) return;

            int value, value2;
            Point location;
            Cell cell;
            MirDirection dir;
            MonsterObject monster;
            Point front;
            switch (magic.Spell)
            {

                #region HellFire

                case Spell.HellFire:
                    value = (int)data[2];
                    dir = (MirDirection)data[4];
                    location = Functions.PointMove((Point)data[3], dir, 1);
                    int count = (int)data[5] - 1;

                    if (!ValidPoint(location)) return;

                    if (count > 0)
                    {
                        DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 100, player, magic, value, location, dir, count);
                        ActionList.Add(action);
                    }

                    cell = GetCell(location);

                    if (cell.Objects == null) return;

                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObject target = cell.Objects[i];
                        switch (target.Race)
                        {
                            case ObjectType.Monster:
                            case ObjectType.Player:
                                //Only targets
                                if (target.IsAttackTarget(player))
                                {
                                    if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                        train = true;
                                    break;
                                }
                                break;
                        }
                    }
                    break;

                #endregion

                #region SummonSkeleton, SummonShinsu, SummonHolyDeva, ArcherSummons

                case Spell.SummonSkeleton:
                    monster = (MonsterObject)data[2];
                    front = (Point)data[3];

                    if (monster.Master.Dead) return;

                    if (ValidPoint(front))
                        monster.Spawn(this, front);
                    else
                        monster.Spawn(player.CurrentMap, player.CurrentLocation);

                    monster.Master.Pets.Add(monster);
                    break;
                #endregion

                #region FireBang, IceStorm

                case Spell.FireBang:
                    value = (int)data[2];
                    location = (Point)data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsAttackTarget(player))
                                        {
                                            if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                                train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                #endregion

                #region MassHiding

                case Spell.MassHiding:
                    value = (int)data[2];
                    location = (Point)data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];

                                if (target.Node == null) continue;

                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            target.AddBuff(BuffType.Hiding, player, (Settings.Second * value), new Stats());
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                #endregion

                #region SoulShield, BlessedArmour

                case Spell.SoulShield:
                case Spell.BlessedArmour:
                    value = (int)data[2];
                    location = (Point)data[3];
                    BuffType type = magic.Spell == Spell.SoulShield ? BuffType.SoulShield : BuffType.BlessedArmour;

                    for (int y = location.Y - 3; y <= location.Y + 3; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 3; x <= location.X + 3; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];

                                if (target.Node == null) continue;

                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            var stats = new Stats
                                            {
                                                [type == BuffType.SoulShield ? Stat.MaxMAC : Stat.MaxAC] = target.Level / 7 + 4
                                            };

                                            target.AddBuff(type, player, Settings.Second * value, stats);
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                #endregion

                #region FireWall

                case Spell.FireWall:
                    value = (int)data[2];
                    location = (Point)data[3];

                    player.LevelMagic(magic);

                    if (ValidPoint(location))
                    {
                        cell = GetCell(location);

                        bool cast = true;
                        if (cell.Objects != null)
                            for (int o = 0; o < cell.Objects.Count; o++)
                            {
                                MapObject target = cell.Objects[o];
                                if (target.Race != ObjectType.Spell || ((SpellObject)target).Spell != Spell.FireWall) continue;

                                cast = false;
                                break;
                            }

                        if (cast)
                        {
                            SpellObject ob = new SpellObject
                                {
                                    Spell = Spell.FireWall,
                                    Value = value,
                                    ExpireTime = Envir.Time + (10 + value / 2) * 1000,
                                    TickSpeed = 2000,
                                    Caster = player,
                                    CurrentLocation = location,
                                    CurrentMap = this,
                                };
                            AddObject(ob);
                            ob.Spawned();
                        }
                    }

                    dir = MirDirection.Up;
                    for (int i = 0; i < 4; i++)
                    {
                        location = Functions.PointMove((Point)data[3], dir, 1);
                        dir += 2;

                        if (!ValidPoint(location)) continue;

                        cell = GetCell(location);
                        bool cast = true;

                        if (cell.Objects != null)
                            for (int o = 0; o < cell.Objects.Count; o++)
                            {
                                MapObject target = cell.Objects[o];
                                if (target.Race != ObjectType.Spell || ((SpellObject)target).Spell != Spell.FireWall) continue;

                                cast = false;
                                break;
                            }

                        if (!cast) continue;

                        SpellObject ob = new SpellObject
                        {
                            Spell = Spell.FireWall,
                            Value = value,
                            ExpireTime = Envir.Time + (10 + value / 2) * 1000,
                            TickSpeed = 2000,
                            Caster = player,
                            CurrentLocation = location,
                            CurrentMap = this,
                        };
                        AddObject(ob);
                        ob.Spawned();
                    }

                    break;

                #endregion

                #region Lightning

                case Spell.Lightning:
                    value = (int)data[2];
                    location = (Point)data[3];
                    dir = (MirDirection)data[4];

                    for (int i = 0; i < 6; i++)
                    {
                        location = Functions.PointMove(location, dir, 1);

                        if (!ValidPoint(location)) continue;

                        cell = GetCell(location);

                        if (cell.Objects == null) continue;

                        for (int o = 0; o < cell.Objects.Count; o++)
                        {
                            MapObject target = cell.Objects[o];
                            if (target.Race != ObjectType.Player && target.Race != ObjectType.Monster) continue;

                            if (!target.IsAttackTarget(player)) continue;
                            if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                train = true;
                            break;
                        }
                    }

                    break;

                #endregion

                #region MassHealing

                case Spell.MassHealing:
                    value = (int)data[2];
                    location = (Point)data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            if (target.Health >= target.MaxHealth) continue;
                                            target.HealAmount = (ushort)Math.Min(ushort.MaxValue, target.HealAmount + value);
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                #endregion

                #region ThunderStorm

                case Spell.ThunderStorm:
                    value = (int)data[2];
                    location = (Point)data[3];
                    for (int y = location.Y - 2; y <= location.Y + 2; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 2; x <= location.X + 2; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (!target.IsAttackTarget(player)) break;
                                        target.Attacked(player, magic.Spell == Spell.ThunderStorm && !target.Undead ? value / 10 : value, DefenceType.MAC, false);
                                        train = true;
                                        break;
                                }
                            }

                        }
                    }

                    break;

                #endregion

                #region TrapHexagon

                case Spell.TrapHexagon:
                    value = (int)data[2];
                    location = (Point)data[3];

                    MonsterObject centerTarget = null;

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];

                                if (y == location.Y && x == location.X && target.Race == ObjectType.Monster)
                                {
                                    centerTarget = (MonsterObject)target;
                                }
                                
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                        if (target == null || !target.IsAttackTarget(player) || target.Node == null || target.Level > player.Level + 2) continue;

                                        MonsterObject mobTarget = (MonsterObject)target;

                                        if (centerTarget == null) centerTarget = mobTarget;

                                        mobTarget.ShockTime = Envir.Time + value;
                                        mobTarget.Target = null;
                                        break;
                                }
                            }

                        }
                    }

                    if (centerTarget == null) return;

                    for (byte i = 0; i < 8; i += 2)
                    {
                        Point startpoint = Functions.PointMove(location, (MirDirection)i, 2);
                        for (byte j = 0; j <= 4; j += 4)
                        {
                            MirDirection spawndirection = i == 0 || i == 4 ? MirDirection.Right : MirDirection.Up;
                            Point spawnpoint = Functions.PointMove(startpoint, spawndirection + j, 1);
                            if (spawnpoint.X <= 0 || spawnpoint.X > centerTarget.CurrentMap.Width) continue;
                            if (spawnpoint.Y <= 0 || spawnpoint.Y > centerTarget.CurrentMap.Height) continue;
                            SpellObject ob = new SpellObject
                            {
                                Spell = Spell.TrapHexagon,
                                ExpireTime = Envir.Time + value,
                                TickSpeed = 100,
                                Caster = player,
                                CurrentLocation = spawnpoint,
                                CastLocation = location,
                                CurrentMap = centerTarget.CurrentMap,
                                Target = centerTarget,
                            };

                            centerTarget.CurrentMap.AddObject(ob);
                            ob.Spawned();
                        }
                    }

                    train = true;

                    break;

                #endregion
        }

            if (train)
                player.LevelMagic(magic);

        }

        public void AddObject(MapObject ob)
        {
            if (ob.Race == ObjectType.Player)
            {
                Players.Add((PlayerObject)ob);
            }

            if (ob.Race == ObjectType.Merchant) NPCs.Add((NPCObject)ob);
            if (ob.Race == ObjectType.Spell) Spells.Add((SpellObject)ob);

            GetCell(ob.CurrentLocation).Add(ob);
        }

        public void RemoveObject(MapObject ob)
        {
            if (ob.Race == ObjectType.Player) Players.Remove((PlayerObject)ob);
            if (ob.Race == ObjectType.Merchant) NPCs.Remove((NPCObject)ob);
            if (ob.Race == ObjectType.Spell) Spells.Remove((SpellObject)ob);

            GetCell(ob.CurrentLocation).Remove(ob);
        }


        public SafeZoneInfo GetSafeZone(Point location)
        {
            for (int i = 0; i < Info.SafeZones.Count; i++)
            {
                SafeZoneInfo szi = Info.SafeZones[i];
                if (Functions.InRange(szi.Location, location, szi.Size))
                    return szi;
            }
            return null;
        }

        public List<SpellObject> GetSpellObjects(Spell spell ,MapObject caster)
        {
            List<SpellObject> spellObjects = new List<SpellObject>();

            for (int i = 0; i < Spells.Count; i++)
            {
                if (spell != Spells[i].Spell) continue;
                if (caster != null && Spells[i].Caster != caster) continue;

                spellObjects.Add(Spells[i]);
            }

            return spellObjects;
        }

        public void Broadcast(Packet p, Point location)
        {
            if (p == null) return;

            for (int i = Players.Count - 1; i >= 0; i--)
            {
                PlayerObject player = Players[i];

                if (Functions.InRange(location, player.CurrentLocation, Globals.DataRange))
                    player.Enqueue(p);                   
            }
        }

        public void BroadcastNPC(Packet p, Point location)
        {
            if (p == null) return;

            for (int i = Players.Count - 1; i >= 0; i--)
            {
                PlayerObject player = Players[i];

                if (Functions.InRange(location, player.CurrentLocation, Globals.DataRange))
                    player.Enqueue(p);

            }
        }


        public void Broadcast(Packet p, Point location, PlayerObject Player)
        {
            if (p == null) return;

            if (Functions.InRange(location, Player.CurrentLocation, Globals.DataRange))
            {
                Player.Enqueue(p);
            }    
        }
    }
    public class Cell
    {
        public static Cell LowWall { get { return new Cell { Attribute = CellAttribute.LowWall }; } }
        public static Cell HighWall { get { return new Cell { Attribute = CellAttribute.HighWall }; } }

        public bool Valid
        {
            get { return Attribute == CellAttribute.Walk; }
        }

        public List<MapObject> Objects;
        public CellAttribute Attribute;
        public sbyte FishingAttribute = -1;

        public void Add(MapObject mapObject)
        {
            if (Objects == null) Objects = new List<MapObject>();

            Objects.Add(mapObject);
        }
        public void Remove(MapObject mapObject)
        {
            Objects.Remove(mapObject);
            if (Objects.Count == 0) Objects = null;
        }
    }
    public class MapRespawn
    {
        protected static Envir Envir
        {
            get { return Envir.Main; }
        }

        public RespawnInfo Info;
        public MonsterInfo Monster;
        public Map Map;
        public int Count;
        public long RespawnTime;
        public ulong NextSpawnTick;
        public byte ErrorCount = 0;

        public List<RouteInfo> Route;
        public List<Point> WalkableCells;

        public MapRespawn(RespawnInfo info)
        {
            Info = info;
            Monster = Envir.GetMonsterInfo(info.MonsterIndex);

            LoadRoutes();
        }
        public bool Spawn()
        {
            MonsterObject ob = MonsterObject.GetMonster(Monster);
            if (ob == null) return true;
            return ob.Spawn(this);
        }

        public void LoadRoutes()
        {
            Route = new List<RouteInfo>();

            if (string.IsNullOrEmpty(Info.RoutePath)) return;

            string fileName = Path.Combine(Settings.RoutePath, Info.RoutePath + ".txt");

            if (!File.Exists(fileName)) return;

            List<string> lines = File.ReadAllLines(fileName).ToList();

            foreach (string line in lines)
            {
                RouteInfo info = RouteInfo.FromText(line);

                if (info == null) continue;

                Route.Add(info);
            }
        }
    }
}
