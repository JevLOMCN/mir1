using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirScenes;
using Client.MirSounds;
using Client.MirControls;
using S = ServerPackets;
using C = ClientPackets;
using Client.MirScenes.Dialogs;
using System.Reflection;

namespace Client.MirObjects
{
    public class PlayerObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Player; }
        }

        public override bool Blocking
        {
            get { return !Dead; }
        }

        public MirGender Gender;
        public MirClass Class;
        public byte Hair;
        public ushort Level;

        public MLibrary WeaponLibrary1, HairLibrary, WingLibrary;
        public int Armour, Weapon, ArmourOffSet, HairOffSet, WeaponOffSet, WingOffset;

        public int DieSound, FlinchSound, AttackSound;


        public FrameSet Frames;
        public Frame Frame, WingFrame;
        public int FrameIndex, FrameInterval, EffectFrameIndex, EffectFrameInterval, SlowFrameIndex;
        public byte SkipFrameUpdate = 0;

        public Spell Spell;
        public byte SpellLevel;
        public bool Cast;
        public uint TargetID;
        public List<uint> SecondaryTargetIDs;
        public Point TargetPoint;

        public bool MagicShield;
        public Effect ShieldEffect;

        public byte WingEffect;
        private short StanceDelay = 2500;

        public SpellEffect CurrentEffect;

        public long StanceTime;

        public string GuildName;
        public string GuildRankName;

        public LevelEffects LevelEffects;

        public PlayerObject() { }
        public PlayerObject(uint objectID) : base(objectID)
        {
            Frames = FrameSet.Player;
        }

        public void Load(S.ObjectPlayer info)
        {
            Name = info.Name;
            NameColour = info.NameColour;
            GuildName = info.GuildName;
            GuildRankName = info.GuildRankName;
            Class = info.Class;
            Gender = info.Gender;
            Level = info.Level;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);

            Direction = info.Direction;
            Hair = info.Hair;

            Weapon = info.Weapon;
			Armour = info.Armour;
            Light = info.Light;

            Poison = info.Poison;

            Dead = info.Dead;
            Hidden = info.Hidden;

            WingEffect = info.WingEffect;
            CurrentEffect = info.Effect;

            SetLibraries();

            if (Dead) ActionFeed.Add(new QueuedAction { Action = MirAction.Dead, Direction = Direction, Location = CurrentLocation });
            if (info.Extra) Effects.Add(new Effect(Libraries.Magic2, 670, 10, 800, this));

            Buffs = info.Buffs;

            LevelEffects = info.LevelEffects;

            ProcessBuffs();

            SetAction();

            SetEffects();
        }
        public void Update(S.PlayerUpdate info)
        {
            Weapon = info.Weapon;
			Armour = info.Armour;
            Light = info.Light;
            WingEffect = info.WingEffect;

            SetLibraries();
            SetEffects();
        }

        public override bool ShouldDrawHealth()
        {
            if (GroupDialog.GroupList.Contains(Name) || this == User)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ProcessBuffs()
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                AddBuffEffect(Buffs[i]);
            }
        }


        public virtual void SetLibraries()
        {
            #region Armours
            BodyLibrary = Libraries.Body;
            HairLibrary = Libraries.Head;
            #endregion

            #region Weapons

            if (Weapon >= 0)
            {
                WeaponLibrary1 = Libraries.Weapon;
            }
            else
            {
                WeaponLibrary1 = null;
            }

            #endregion

            #region WingEffects
            if (WingEffect > 0 && WingEffect < 100)
            {
                WingLibrary = null;
            }
            #endregion

            #region Offsets
            ArmourOffSet = 240 * Armour + 120 * (int)Gender;
            HairOffSet = 240 * Hair + 120 * (int)Gender;
            WeaponOffSet = 240 * Weapon + 120 * (int)Gender;
            WingOffset = 240 * WingEffect + 120 * (int)Gender;
            #endregion
        }

        public virtual void SetEffects()
        {
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                if (Effects[i] is SpecialEffect) Effects[i].Remove();
            }

            if (CurrentEffect == SpellEffect.MagicShieldUp)
            {
                if (ShieldEffect != null)
                {
                    ShieldEffect.Clear();
                    ShieldEffect.Remove();
                }

                MagicShield = true;
                Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, this) { Repeat = true });
            }

            if (WingEffect >= 100)
            {
                switch(WingEffect)
                {
                    case 100: //Oma King Robe effect
                        Effects.Add(new SpecialEffect(Libraries.Effect, 352, 33, 3600, this, true, false, 0) { Repeat = true });
                        break;
                }
            }

			long delay = 5000;

            if (LevelEffects == LevelEffects.None) return;

            //Effects dependant on flags
            if (LevelEffects.HasFlag(LevelEffects.BlueDragon))
            {
                Effects.Add(new SpecialEffect(Libraries.Effect, 1210, 20, 3200, this, true, true, 1) { Repeat = true });
                SpecialEffect effect = new SpecialEffect(Libraries.Effect, 1240, 32, 4200, this, true, false, 1) { Repeat = true };
                effect.SetStart(CMain.Time);
                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.RedDragon))
            {
                Effects.Add(new SpecialEffect(Libraries.Effect, 990, 20, 3200, this, true, true, 1) { Repeat = true });
                SpecialEffect effect = new SpecialEffect(Libraries.Effect, 1020, 32, 4200, this, true, false, 1) { Repeat = true, Delay = delay };
                effect.SetStart(CMain.Time + delay);
                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.Mist))
            {
                Effects.Add(new SpecialEffect(Libraries.Effect, 296, 32, 3600, this, true, false, 1) { Repeat = true });
            }

            if (LevelEffects.HasFlag(LevelEffects.Rebirth1))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 6800, 20, 3600, this, true, false, 1) { Repeat = true });


            }

            if (LevelEffects.HasFlag(LevelEffects.Rebirth2))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 6870, 19, 3600, this, true, true, 1) { Repeat = true });
                SpecialEffect effect = new SpecialEffect(Libraries.Magic2, 6840, 22, 3600, this, true, false, 1) { Repeat = true, Delay = delay };
                effect.SetStart(CMain.Time + delay);
                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.Rebirth3))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 6906, 19, 3600, this, true, false, 1) { Repeat = true, Delay = delay });
                SpecialEffect effect = new SpecialEffect(Libraries.Magic2, 6930, 29, 3600, this, true, true, 1) { Repeat = true };

                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.NewBlue))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 7040, 31, 3600, this, true, false, 1) { Repeat = true, Delay = delay });
                SpecialEffect effect = new SpecialEffect(Libraries.Magic2,7080, 24, 3600, this, true, true, 1) { Repeat = true };

                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.YellowDragon))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 7120, 31, 3600, this, true, false, 1) { Repeat = true, Delay = delay });
                SpecialEffect effect = new SpecialEffect(Libraries.Magic2, 7160, 24, 3600, this, true, true, 1) { Repeat = true };

                Effects.Add(effect);
            }

            if (LevelEffects.HasFlag(LevelEffects.Phoenix))
            {
                Effects.Add(new SpecialEffect(Libraries.Magic2, 6970, 26, 3600, this, true, false, 1) { Repeat = true, Delay = delay });
                SpecialEffect effect = new SpecialEffect(Libraries.Magic2, 7000, 21, 3600, this, true, true, 1) { Repeat = true };

                Effects.Add(effect);
            }
        }

        public override void Process()
        {
            bool update = CMain.Time >= NextMotion || GameScene.CanMove;

            SkipFrames = this != User && ActionFeed.Count > 1;

            ProcessFrames();

            if (Frame == null)
            {
                DrawFrame = 0;
                DrawWingFrame = 0;
            }
            else
            {
                DrawFrame = Frame.Start + (Frame.OffSet * (byte)Direction) + FrameIndex;
                DrawWingFrame = Frame.EffectStart + (Frame.EffectOffSet * (byte)Direction) + EffectFrameIndex;
            }

            #region Moving OffSet

            switch (CurrentAction)
            {
                case MirAction.Walking:
                case MirAction.Pushed:
                case MirAction.DashL:
                case MirAction.DashR:
                    if (Frame == null)
                    {
                        OffSetMove = Point.Empty;
                        Movement = CurrentLocation;
                        break;
                    }

                    var i = 1;

                    Movement = Functions.PointMove(CurrentLocation, Direction, CurrentAction == MirAction.Pushed ? 0 : -i);

                    int count = Frame.Count;
                    int index = FrameIndex;

                    if (CurrentAction == MirAction.DashR || CurrentAction == MirAction.DashL)
                    {
                        count = 3;
                        index %= 3;
                    }

                    switch (Direction)
                    {
                        case MirDirection.Up:
                            OffSetMove = new Point(0, (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.UpRight:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Right:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), 0);
                            break;
                        case MirDirection.DownRight:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Down:
                            OffSetMove = new Point(0, (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.DownLeft:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Left:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), 0);
                            break;
                        case MirDirection.UpLeft:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                    }

                    OffSetMove = new Point(OffSetMove.X % 2 + OffSetMove.X, OffSetMove.Y % 2 + OffSetMove.Y);
                    break;
                default:
                    OffSetMove = Point.Empty;
                    Movement = CurrentLocation;
                    break;
            }

            #endregion


            DrawY = Movement.Y > CurrentLocation.Y ? Movement.Y : CurrentLocation.Y;

            DrawLocation = new Point((Movement.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (Movement.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset(GlobalDisplayLocationOffset);

            if (this != User)
            {
                DrawLocation.Offset(User.OffSetMove);
                DrawLocation.Offset(-OffSetMove.X, -OffSetMove.Y);
            }

            if (BodyLibrary != null && update)
            {
                FinalDrawLocation = DrawLocation.Add(BodyLibrary.GetOffSet(DrawFrame));
                DisplayRectangle = new Rectangle(DrawLocation, BodyLibrary.GetTrueSize(DrawFrame));
            }

            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Process();

            Color colour = DrawColour;
            DrawColour = Color.White;
            if (Poison != PoisonType.None)
            {
                
                if (Poison.HasFlag(PoisonType.Green))
                    DrawColour = Color.Green;
                if (Poison.HasFlag(PoisonType.Red))
                    DrawColour = Color.Red;
                if (Poison.HasFlag(PoisonType.Slow))
                    DrawColour = Color.Purple;
                if (Poison.HasFlag(PoisonType.Stun))
                    DrawColour = Color.Yellow;
                if (Poison.HasFlag(PoisonType.Frozen))
                    DrawColour = Color.Blue;
                if (Poison.HasFlag(PoisonType.Paralysis))
                    DrawColour = Color.Gray;
            }


            if (colour != DrawColour) GameScene.Scene.MapControl.TextureValid = false;
        }
        public virtual void SetAction()
        {
            if (NextAction != null && !GameScene.CanMove)
            {
                switch (NextAction.Action)
                {
                    case MirAction.Walking:
                    case MirAction.Pushed:
                    case MirAction.DashL:
                    case MirAction.DashR:
                        return;
                }
            }

            if (User == this && CMain.Time < MapControl.NextAction)// && CanSetAction)
            {
                //NextMagic = null;
                return;
            }


            if (ActionFeed.Count == 0)
            {
                CurrentAction = MirAction.Standing;

                if (CurrentAction == MirAction.Standing)
                {
                    CurrentAction = CMain.Time > StanceTime ? MirAction.Standing : MirAction.Stance;
                }

                Frames.TryGetValue(CurrentAction, out Frame);
                FrameIndex = 0;
                EffectFrameIndex = 0;

                if (MapLocation != CurrentLocation)
                {
                    GameScene.Scene.MapControl.RemoveObject(this);
                    MapLocation = CurrentLocation;
                    GameScene.Scene.MapControl.AddObject(this);
                }

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
                EffectFrameInterval = Frame.EffectInterval;

                SetLibraries();
            }
            else
            {
                QueuedAction action = ActionFeed[0];
                ActionFeed.RemoveAt(0);


                CurrentAction = action.Action;

                CurrentLocation = action.Location;
                MirDirection olddirection = Direction;
                Direction = action.Direction;

                Point temp;
                switch (CurrentAction)
                {
                    case MirAction.Walking:
                    case MirAction.Pushed:
                    case MirAction.DashL:
                    case MirAction.DashR:
                        var steps = 1;

                        temp = Functions.PointMove(CurrentLocation, Direction, CurrentAction == MirAction.Pushed ? 0 : -steps);

                        break;
                    default:
                        temp = CurrentLocation;
                        break;
                }

                temp = new Point(action.Location.X, temp.Y > CurrentLocation.Y ? temp.Y : CurrentLocation.Y);

                if (MapLocation != temp)
                {
                    GameScene.Scene.MapControl.RemoveObject(this);
                    MapLocation = temp;
                    GameScene.Scene.MapControl.AddObject(this);
                }


                bool ArcherLayTrap = false;

                switch (CurrentAction)
                {
                    case MirAction.Pushed:
                        if (this == User)
                            MapControl.InputDelay = CMain.Time + 500;
                        Frames.TryGetValue(MirAction.Walking, out Frame);
                        break;
                    case MirAction.DashL:
                    case MirAction.DashR:
                        Frames.TryGetValue(MirAction.Walking, out Frame);
                        break;
                    case MirAction.DashFail:
                        Frames.TryGetValue(MirAction.Standing, out Frame);
                        break;
                    case MirAction.Attack1:
                        Frames.TryGetValue(CurrentAction, out Frame);
                        break;
                    case MirAction.Spell:
                        Spell = (Spell)action.Params[0];
                        switch (Spell)
                        {
                            case Spell.ShoulderDash:
                                Frames.TryGetValue(MirAction.Walking, out Frame);
                                CurrentAction = MirAction.DashL;
                                Direction = olddirection;
                                CurrentLocation = Functions.PointMove(CurrentLocation, Direction, 1);
                                if (this == User)
                                {
                                    MapControl.NextAction = CMain.Time + 2500;
                                    GameScene.SpellTime = CMain.Time + 2500; //Spell Delay

                                    Network.Enqueue(new C.Magic { ObjectID = GameScene.User.ObjectID, Spell = Spell, Direction = Direction, });
                                }
                                break;
                            default:
                                Frames.TryGetValue(CurrentAction, out Frame);
                                break;
                        }
                        
                        break;
                    default:
                        Frames.TryGetValue(CurrentAction, out Frame);
                        break;

                }

                SetLibraries();

                FrameIndex = 0;
                EffectFrameIndex = 0;
                Spell = Spell.None;
                SpellLevel = 0;
                //NextMagic = null;

                ClientMagic magic;

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
                EffectFrameInterval = Frame.EffectInterval;

                if (this == User)
                {
                    switch (CurrentAction)
                    {
                        case MirAction.DashFail:
                            //CanSetAction = false;
                            break;
                        case MirAction.Standing:
                            Network.Enqueue(new C.Turn { Direction = Direction });
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Walking:
                            Network.Enqueue(new C.Walk { Direction = Direction });
                            GameScene.Scene.MapControl.FloorValid = false;
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Pushed:
                            GameScene.Scene.MapControl.FloorValid = false;
                            MapControl.InputDelay = CMain.Time + 500;
                            GameScene.CanMove = false;
                            break;
                        case MirAction.DashL:
                        case MirAction.DashR:
                            GameScene.Scene.MapControl.FloorValid = false;
                            //CanSetAction = false;
                            break;
                        case MirAction.Attack1:
                                if (GameScene.User.Slaying && TargetObject != null)
                                    Spell = Spell.Slaying;

                                if (GameScene.User.Thrusting && GameScene.Scene.MapControl.HasTarget(Functions.PointMove(CurrentLocation, Direction, 2)))
                                    Spell = Spell.Thrusting;

                                if (GameScene.User.HalfMoon)
                                {
                                    if (TargetObject != null || GameScene.Scene.MapControl.CanHalfMoon(CurrentLocation, Direction))
                                    {
                                        magic = User.GetMagic(Spell.HalfMoon);
                                        if (magic != null && magic.BaseCost + magic.LevelCost * magic.Level <= User.MP)
                                            Spell = Spell.HalfMoon;
                                    }
                                }

                                if (GameScene.User.FlamingSword)
                                {
                                    if (TargetObject != null)
                                    {
                                        magic = User.GetMagic(Spell.FlamingSword);
                                        if (magic != null)
                                        {
                                            Spell = Spell.FlamingSword;
                                            magic.CastTime = CMain.Time;
                                        }
                                    }
                                }
                            

                            Network.Enqueue(new C.Attack { Direction = Direction, Spell = Spell });

                            if (Spell == Spell.Slaying)
                                GameScene.User.Slaying = false;
                            if (Spell == Spell.FlamingSword)
                                GameScene.User.FlamingSword = false;

                            magic = User.GetMagic(Spell);

                            if (magic != null) SpellLevel = magic.Level;

                            GameScene.AttackTime = CMain.Time + User.AttackSpeed;
                            MapControl.NextAction = CMain.Time + 2500;
                            break;

                        case MirAction.AttackRange1:
                            {
                                GameScene.AttackTime = CMain.Time + User.AttackSpeed + 200;

                                uint targetID = (uint)action.Params[0];
                                Point location = (Point)action.Params[1];

                                Network.Enqueue(new C.RangeAttack { Direction = Direction, Location = CurrentLocation, TargetID = targetID, TargetLocation = location });
                            }
                            break;
                        case MirAction.Spell:
                            {
                                Spell = (Spell)action.Params[0];
                                uint targetID = (uint)action.Params[1];
                                Point location = (Point)action.Params[2];

                                Network.Enqueue(new C.Magic { ObjectID = GameScene.User.ObjectID, Spell = Spell, Direction = Direction, TargetID = targetID, Location = location });

                                GameScene.SpellTime = CMain.Time + 1800;
                                MapControl.NextAction = CMain.Time + 2500;
                            }
                            break;                         
                        case MirAction.Harvest:
                            if (ArcherLayTrap)
                            {
                                ArcherLayTrap = false;
                                SoundManager.PlaySound(20000 + 124 * 10);
                            }
                            else
                            {
                                Network.Enqueue(new C.Harvest { Direction = Direction });
                                MapControl.NextAction = CMain.Time + 2500;
                            }
                            break;

                    }
                }


                switch (CurrentAction)
                {
                    case MirAction.Pushed:
                        FrameIndex = Frame.Count - 1;
                        EffectFrameIndex = Frame.EffectCount - 1;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.DashL:
                        FrameIndex = 0;
                        EffectFrameIndex = 0;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.DashR:
                        FrameIndex = 3;
                        EffectFrameIndex = 3;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.Walking:
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.Attack1:
                        if (this != User)
                        {
                            Spell = (Spell)action.Params[0];
                            SpellLevel = (byte)action.Params[1];
                        }

                        switch (Spell)
                        {
                            case Spell.Slaying:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 + (Gender == MirGender.Male ? 0 : 1));
                                break;
                            case Spell.Thrusting:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;
                            case Spell.HalfMoon:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;
                            case Spell.FlamingSword:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                break;

                            
                        }
                        break;
                    case MirAction.AttackRange1: //ArcherTest - Assign Target for other users
                        if (this != User)
                        {
                            TargetID = (uint)action.Params[0];
                            TargetPoint = (Point)action.Params[1];
                            Spell = (Spell)action.Params[2];
                        }
                        break;
                    case MirAction.Spell:
                        if (this != User)
                        {
                            Spell = (Spell)action.Params[0];
                            TargetID = (uint)action.Params[1];
                            TargetPoint = (Point)action.Params[2];
                            Cast = (bool)action.Params[3];
                            SpellLevel = (byte)action.Params[4];
                            SecondaryTargetIDs = (List<uint>)action.Params[5];
                        }

                        switch (Spell)
                        {
                            #region FireBall

                            case Spell.FireBall:
                                Effects.Add(new Effect(Libraries.Magic, 0, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Healing

                            case Spell.Healing:
                                Effects.Add(new Effect(Libraries.Magic, 200, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Repulsion

                            case Spell.Repulsion:
                                Effects.Add(new Effect(Libraries.Magic, 900, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region ElectricShock

                            case Spell.ElectricShock:
                                Effects.Add(new Effect(Libraries.Magic, 1560, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Poisoning

                            case Spell.Poisoning:
                                Effects.Add(new Effect(Libraries.Magic, 600, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region GreatFireBall

                            case Spell.GreatFireBall:
                                Effects.Add(new Effect(Libraries.Magic, 400, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region HellFire

                            case Spell.HellFire:
                                Effects.Add(new Effect(Libraries.Magic, 920, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region ThunderBolt

                            case Spell.ThunderBolt:
                                Effects.Add(new Effect(Libraries.Magic2, 20, 3, 300, this));
                                break;

                            #endregion

                            #region SoulFireBall

                            case Spell.SoulFireBall:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region SummonSkeleton

                            case Spell.SummonSkeleton:
                                Effects.Add(new Effect(Libraries.Magic, 1500, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion
                           
                            #region Teleport

                            case Spell.Teleport:
                                Effects.Add(new Effect(Libraries.Magic, 1590, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Hiding

                            case Spell.Hiding:
                                Effects.Add(new Effect(Libraries.Magic, 1520, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FireBang

                            case Spell.FireBang:
                                Effects.Add(new Effect(Libraries.Magic, 1650, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FireWall

                            case Spell.FireWall:
                                Effects.Add(new Effect(Libraries.Magic, 1620, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region TrapHexagon

                            case Spell.TrapHexagon:
                                Effects.Add(new Effect(Libraries.Magic, 1380, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region EnergyRepulsor

                            case Spell.EnergyRepulsor:
                                Effects.Add(new Effect(Libraries.Magic2, 190, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region UltimateEnchancer

                            case Spell.UltimateEnhancer:
                                Effects.Add(new Effect(Libraries.Magic2, 160, 15, 1000, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FrostCrunch

                            case Spell.FrostCrunch:
                                Effects.Add(new Effect(Libraries.Magic2, 400, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Purification

                            case Spell.Purification:
                                Effects.Add(new Effect(Libraries.Magic2, 600, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region ThunderStorm

                            case Spell.ThunderStorm:
                                MapControl.Effects.Add(new Effect(Libraries.Magic, 1680, 10, Frame.Count * FrameInterval, CurrentLocation));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region MassHealing

                            case Spell.MassHealing:
                                Effects.Add(new Effect(Libraries.Magic, 1790, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region MagicShield

                            case Spell.MagicShield:
                                Effects.Add(new Effect(Libraries.Magic, 3880, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Revelation

                            case Spell.Revelation:
                                Effects.Add(new Effect(Libraries.Magic, 3960, 20, 1200, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion
                        }


                        break;
                    case MirAction.Dead:
                        GameScene.Scene.Redraw();
                        GameScene.Scene.MapControl.SortObject(this);
                        if (MouseObject == this) MouseObjectID = 0;
                        if (TargetObject == this) TargetObjectID = 0;
                        if (MagicObject == this) MagicObjectID = 0;
                        DeadTime = CMain.Time;
                        break;

                }

            }

            GameScene.Scene.MapControl.TextureValid = false;

            NextMotion = CMain.Time + FrameInterval;
            NextMotion2 = CMain.Time + EffectFrameInterval;

            if (MagicShield)
            {
                switch (CurrentAction)
                {
                    default:
                        if (ShieldEffect == null)
                            Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, this) { Repeat = true });
                        break;
                }
            }

        }

        public virtual void ProcessFrames()
        {
            if (Frame == null) return;

            switch (CurrentAction)
            {
                case MirAction.Walking:
                    if (!GameScene.CanMove) return;                    

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;
                    //if (CMain.Time < NextMotion) return;
                    if (SkipFrames) UpdateFrame();

                    if (UpdateFrame(false) >= Frame.Count)
                    {
                        FrameIndex = Frame.Count - 1;
                        SetAction();
                    }                    

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        if (this == User) GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.DashL:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;
                    if (UpdateFrame() >= 3)
                    {
                        FrameIndex = 2;
                        SetAction();
                    }

                    if (UpdateFrame2() >= 3) EffectFrameIndex = 2;
                    break;
                case MirAction.DashR:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;

                    if (UpdateFrame() >= 6)
                    {
                        FrameIndex = 5;
                        SetAction();
                    }

                    if (UpdateFrame2() >= 6) EffectFrameIndex = 5;
                    break;
                case MirAction.Pushed:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;

                    FrameIndex -= 2;
                    EffectFrameIndex -= 2;

                    if (FrameIndex < 0)
                    {
                        FrameIndex = 0;
                        SetAction();
                    }

                    if (FrameIndex < 0) EffectFrameIndex = 0;
                    break;

                case MirAction.Standing:
                case MirAction.DashFail:
                case MirAction.Harvest:
                case MirAction.Stance:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            NextMotion += FrameInterval;
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Attack1:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            //if (ActionFeed.Count == 0)
                            //    ActionFeed.Add(new QueuedAction { Action = MirAction.Stance, Direction = Direction, Location = CurrentLocation });

                            StanceTime = CMain.Time + StanceDelay;
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            if (FrameIndex == 1) PlayAttackSound();
                            NextMotion += FrameInterval;
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;

                case MirAction.AttackRange1:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            if (FrameIndex == 1) PlayAttackSound();
                            Missile missile;
                            switch (FrameIndex)
                            {
                                case 5:
                                    missile = CreateProjectile(1030, Libraries.Magic2, true, 5, 30, 5);
                                    StanceTime = CMain.Time + StanceDelay;
                                    SoundManager.PlaySound(20000 + 121 * 10);
                                    if (missile.Target != null)
                                    {
                                        missile.Complete += (o, e) =>
                                        {
                                            SoundManager.PlaySound(20000 + 121 * 10 + 2);
                                        };
                                    }
                                    break;
                            }

                            NextMotion += FrameInterval;
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Spell:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            if (Cast)
                            {

                                MapObject ob = MapControl.GetObject(TargetID);

                                Missile missile;
                                Effect effect;
                                switch (Spell)
                                {
                                    #region FireBall

                                    case Spell.FireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(10, Libraries.Magic, true, 6, 30, 4);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 170, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region GreatFireBall

                                    case Spell.GreatFireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(410, Libraries.Magic, true, 6, 30, 4);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 570, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.GreatFireBall * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region Healing

                                    case Spell.Healing:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, ob));
                                        break;

                                    #endregion

                                    #region ElectricShock

                                    case Spell.ElectricShock:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1570, 10, 1000, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 1570, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region Poisoning

                                    case Spell.Poisoning:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob != null)
                                            ob.Effects.Add(new Effect(Libraries.Magic, 770, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region HellFire

                                    case Spell.HellFire:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);

                                        
                                        Point dest = CurrentLocation;
                                        for (int i = 0; i < 4; i++)
                                        {
                                            dest = Functions.PointMove(dest, Direction, 1);
                                            if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                            effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                            effect.SetStart(CMain.Time + i * 50);
                                            MapControl.Effects.Add(effect);
                                        }

                                        if (SpellLevel == 3)
                                        {
                                            MirDirection dir = (MirDirection)(((int)Direction + 1) % 8);
                                            dest = CurrentLocation;
                                            for (int r = 0; r < 4; r++)
                                            {
                                                dest = Functions.PointMove(dest, dir, 1);
                                                if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                                effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                                effect.SetStart(CMain.Time + r * 50);
                                                MapControl.Effects.Add(effect);
                                            }

                                            dir = (MirDirection)(((int)Direction - 1 + 8) % 8);
                                            dest = CurrentLocation;
                                            for (int r = 0; r < 4; r++)
                                            {
                                                dest = Functions.PointMove(dest, dir, 1);
                                                if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                                effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                                effect.SetStart(CMain.Time + r * 50);
                                                MapControl.Effects.Add(effect);
                                            }
                                        }
                                        break;

                                    #endregion

                                    #region ThunderBolt

                                    case Spell.ThunderBolt:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);

                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 10, 5, 400, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 10, 5, 400, ob));
                                        break;

                                    #endregion

                                    #region SoulFireBall

                                    case Spell.SoulFireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 1360, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.SoulFireBall * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region FireBang

                                    case Spell.FireBang:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        MapControl.Effects.Add(new Effect(Libraries.Magic, 1660, 10, 1000, TargetPoint));
                                        break;

                                    #endregion

                                    #region MassHiding

                                    case Spell.MassHiding:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1540, 10, 800, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.MassHiding * 10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region SoulShield

                                    case Spell.SoulShield:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1320, 15, 1200, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.SoulShield * 10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region BlessedArmour

                                    case Spell.BlessedArmour:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1340, 15, 1200, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.BlessedArmour * 10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region FireWall

                                    case Spell.FireWall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        break;

                                    #endregion

                                    #region MassHealing

                                    case Spell.MassHealing:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        MapControl.Effects.Add(new Effect(Libraries.Magic, 1800, 10, 1000, TargetPoint));
                                        break;

                                    #endregion

                                    #region Revelation

                                    case Spell.Revelation:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 3990, 10, 1000, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 3990, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region FrostCrunch

                                    case Spell.FrostCrunch:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(410, Libraries.Magic2, true, 4, 30, 6);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic2, 570, 8, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.FrostCrunch * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region Purification

                                    case Spell.Purification:
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 620, 10, 800, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 620, 10, 800, ob));
                                        break;

                                    #endregion

                                    #region Hallucination

                                    case Spell.Hallucination:
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 48, 7);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic2, 1110, 10, 1000, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region Lightning

                                    case Spell.Lightning:
                                        Effects.Add(new Effect(Libraries.Magic, 970 + (int)Direction * 20, 6, Frame.Count * FrameInterval, this));
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        break;

                                    #endregion

                                    #region UltimateEnhancer

                                    case Spell.UltimateEnhancer:
                                        if (ob != null && ob != User)
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 160, 15, 1000, ob));
                                        break;

                                    #endregion

                                    #region TrapHexagon

                                    case Spell.TrapHexagon:
                                        if (ob != null)
                                        SoundManager.PlaySound(20000 + (ushort)Spell.TrapHexagon * 10 + 1);
                                        break;

                                    #endregion
                                }


                                Cast = false;
                            }
                            //if (ActionFeed.Count == 0)
                            //    ActionFeed.Add(new QueuedAction { Action = MirAction.Stance, Direction = Direction, Location = CurrentLocation });

                            StanceTime = CMain.Time + StanceDelay;
                            FrameIndex = Frame.Count - 1;
                            SetAction();

                        }
                        else
                        {
                            NextMotion += FrameInterval;

                        }
                    }
                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Die:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            ActionFeed.Clear();
                            ActionFeed.Add(new QueuedAction { Action = MirAction.Dead, Direction = Direction, Location = CurrentLocation });
                            SetAction();
                        }
                        else
                        {
                            if (FrameIndex == 1)
                                PlayDieSound();

                            NextMotion += FrameInterval;
                        }
                    }
                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Dead:
                    break;
                case MirAction.Revive:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            ActionFeed.Clear();
                            ActionFeed.Add(new QueuedAction { Action = MirAction.Standing, Direction = Direction, Location = CurrentLocation });
                            SetAction();
                        }
                        else
                        {
                            NextMotion += FrameInterval;
                        }
                    }
                    break;

            }

            if (this == User) return;

            if ((CurrentAction == MirAction.Standing || CurrentAction == MirAction.Stance || CurrentAction == MirAction.DashFail) && NextAction != null)
                SetAction();
            //if Revive and dead set action

        }
        public int UpdateFrame(bool skip = true)
        {
            if (Frame == null) return 0;
            if (Poison.HasFlag(PoisonType.Slow) && !skip)
            {
                SkipFrameUpdate++;
                if (SkipFrameUpdate == 2)
                    SkipFrameUpdate = 0;
                else
                    return FrameIndex;
            }
            if (Frame.Reverse) return Math.Abs(--FrameIndex);

            return ++FrameIndex;
        }

        public int UpdateFrame2()
        {
            if (Frame == null) return 0;

            if (Frame.Reverse) return Math.Abs(--EffectFrameIndex);

            return ++EffectFrameIndex;
        }


        public override Missile CreateProjectile(int baseIndex, MLibrary library, bool blend, int count, int interval, int skip, int lightDistance = 6, bool direction16 = true, Color? lightColour = null, uint targetID = 0)
        {
            if (targetID == 0)
            {
                targetID = TargetID;
            }

            MapObject ob = MapControl.GetObject(targetID);

            var targetPoint = TargetPoint;

            if (ob != null) targetPoint = ob.CurrentLocation;

            int duration = Functions.MaxDistance(CurrentLocation, targetPoint) * 50;

            Missile missile = new Missile(library, baseIndex, duration / interval, duration, this, targetPoint)
            {
                Target = ob,
                Interval = interval,
                FrameCount = count,
                Blend = blend,
                Skip = skip,
                Light = lightDistance,
                LightColour = lightColour == null ? Color.White : (Color)lightColour
            };

            Effects.Add(missile);

            return missile;
        }

        public override void PlayStruckSound()
        {
            switch (StruckWeapon)
            {
                default:
                    SoundManager.PlaySound(SoundList.StruckBodySword);
                    break;
                case -1:
                    SoundManager.PlaySound(SoundList.StruckBodyFist);
                    break;
            }
        }

        public void PlayAttackSound()
        {
            switch (Weapon)
            {
                case 1:
                case 2:
                case 4:
                    SoundManager.PlaySound(SoundList.SwingWood);
                    break;
                case 3:
                case 5:
                case 6:
                    SoundManager.PlaySound(SoundList.SwingSword);
                    break;
                default:
                    SoundManager.PlaySound(SoundList.SwingFist);
                    break;
            }
        }

        public void PlayDieSound()
        {
        }


        public override void Draw()
        {
            DrawBehindEffects(Settings.Effect);

            float oldOpacity = DXManager.Opacity;
            if (Hidden && !DXManager.Blending) DXManager.SetOpacity(0.5F);


            if (Direction == MirDirection.Left || Direction == MirDirection.Up || Direction == MirDirection.UpLeft || Direction == MirDirection.DownLeft)
                DrawWeapon();

            DrawBody();

            if (Direction == MirDirection.Up || Direction == MirDirection.UpLeft || Direction == MirDirection.UpRight || Direction == MirDirection.Right || Direction == MirDirection.Left)
            {
                DrawHead();
                if (this != User)
                {
                    DrawWings();
                }
            }
            else
            {
                if (this != User)
                {
                    DrawWings();
                }
                DrawHead();
            }


            if (Direction == MirDirection.UpRight || Direction == MirDirection.Right || Direction == MirDirection.DownRight || Direction == MirDirection.Down)
                DrawWeapon();

            DXManager.SetOpacity(oldOpacity);
        }

        public override void DrawBehindEffects(bool effectsEnabled)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (!Effects[i].DrawBehind) continue;
                if (!Settings.LevelEffect && (Effects[i] is SpecialEffect) && ((SpecialEffect)Effects[i]).EffectType == 1) continue;
                if ((!effectsEnabled) && (!IsVitalEffect(Effects[i]))) continue;
                Effects[i].Draw();
            }
        }

        public override void DrawEffects(bool effectsEnabled)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].DrawBehind) continue;
                if (!Settings.LevelEffect && (Effects[i] is SpecialEffect) && ((SpecialEffect)Effects[i]).EffectType == 1) continue;
                if ((!effectsEnabled) && (!IsVitalEffect(Effects[i]))) continue;
                Effects[i].Draw();
            }

            if (!effectsEnabled) return;

            switch (CurrentAction)
            {
                case MirAction.Attack1:
                    switch (Spell)
                    {
                        case Spell.Slaying:
                            Libraries.Magic.DrawBlend(1820 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.Thrusting:
                            Libraries.Magic.DrawBlend(2190 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.HalfMoon:
                            Libraries.Magic.DrawBlend(2560 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.FlamingSword:
                            Libraries.Magic.DrawBlend(3480 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                    }
                    break;
            }


        }

        public void DrawCurrentEffects()
        {
            if (CurrentEffect == SpellEffect.MagicShieldUp && !MagicShield)
            {
                MagicShield = true;
                Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, this) { Repeat = true });
                CurrentEffect = SpellEffect.None;
            }
        }

        public override void DrawBlend()
        {
            DXManager.SetBlend(true, 0.3F);
            Draw();
            DXManager.SetBlend(false);
        }
        public void DrawBody()
        {
            bool oldGrayScale = DXManager.GrayScale;
            Color drawColour = ApplyDrawColour();                     

            if (BodyLibrary != null)
                BodyLibrary.Draw(DrawFrame + ArmourOffSet, DrawLocation, drawColour, true);

            DXManager.SetGrayscale(oldGrayScale);

            //BodyLibrary.DrawTinted(DrawFrame + ArmourOffSet, DrawLocation, DrawColour, Color.DarkSeaGreen);
        }
        public void DrawHead()
        {
            if (HairLibrary != null)
                HairLibrary.Draw(DrawFrame + HairOffSet, DrawLocation, DrawColour, true);
        }
		public void DrawWeapon()
		{
			if (Weapon < 0) return;

			if (WeaponLibrary1 != null)
			{
				WeaponLibrary1.Draw(DrawFrame + WeaponOffSet, DrawLocation, DrawColour, true); //original
			}
		}
        public void DrawWings()
        {
            if (WingEffect <= 0 || WingEffect >= 100) return;

            if (WingLibrary != null)
                WingLibrary.DrawBlend(DrawWingFrame + WingOffset, DrawLocation, DrawColour, true);
        }

        private bool IsVitalEffect(Effect effect)
        {
            if ((effect.Library == Libraries.Magic) && (effect.BaseIndex == 3890))
                return true;
            if ((effect.Library == Libraries.Magic2) && (effect.BaseIndex == 1890))
                return true;
            return false;
        }

        public override bool MouseOver(Point p)
        {
            return MapControl.MapLocation == CurrentLocation || BodyLibrary != null && BodyLibrary.VisiblePixel(DrawFrame + ArmourOffSet, p.Subtract(FinalDrawLocation), false);
        }

        private void CreateNameLabel()
        {
            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != Name || LabelList[i].ForeColour != NameColour) continue;
                NameLabel = LabelList[i];
                break;
            }

            if (NameLabel != null && !NameLabel.IsDisposed) return;

            NameLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = NameColour,
                OutLine = true,
                OutLineColour = Color.Black,
                Text = Name,
            };
            NameLabel.Disposing += (o, e) => LabelList.Remove(NameLabel);
            LabelList.Add(NameLabel);
        }

        private void CreateGuildLabel()
        {
            if (string.IsNullOrEmpty(GuildName))
                return;

            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != GuildName || LabelList[i].ForeColour != NameColour) continue;
                GuildLabel = LabelList[i];
                break;
            }

            if (GuildLabel != null && !GuildLabel.IsDisposed) return;

            GuildLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = NameColour,
                OutLine = true,
                OutLineColour = Color.Black,
                Text = GuildName,
            };
            GuildLabel.Disposing += (o, e) => LabelList.Remove(GuildLabel);
            LabelList.Add(GuildLabel);
        }

        public override void CreateLabel()
        {
            NameLabel = null;
            GuildLabel = null;

            CreateNameLabel();
            CreateGuildLabel();
        }

        public override void DrawName()
        {
            CreateLabel();

            if (GuildLabel != null && !string.IsNullOrEmpty(GuildName))
            {
                GuildLabel.Text = GuildName;
                GuildLabel.Location = new Point(DisplayRectangle.X + (50 - GuildLabel.Size.Width) / 2, DisplayRectangle.Y - (19 - GuildLabel.Size.Height / 2) + (Dead ? 35 : 8)); //was 48 -
                GuildLabel.Draw();
            }

            if (NameLabel != null)
            {
                NameLabel.Text = Name;
                NameLabel.Location = new Point(DisplayRectangle.X + (50 - NameLabel.Size.Width) / 2, DisplayRectangle.Y - (51 - NameLabel.Size.Height / 2) + (Dead ? 55 : 8)); //was 48 -
                NameLabel.Draw();
            }
        }

    }


    public class QueuedAction
    {
        public MirAction Action;
        public Point Location;
        public MirDirection Direction;
        public List<object> Params;
    }

}
