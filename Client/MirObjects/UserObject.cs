using Client.MirScenes;
using Client.MirScenes.Dialogs;
using S = ServerPackets;
using Shared.Extensions;

namespace Client.MirObjects
{
    public class UserObject : PlayerObject
    {
        public uint Id;

        public int HP, MP;

        public int AttackSpeed;

        public Stats Stats;

        public int CurrentHandWeight,
                      CurrentWearWeight,
                      CurrentBagWeight;

        public long Experience, MaxExperience;

        public bool TradeLocked;
        public uint TradeGoldAmount;
        public bool AllowTrade;

        public bool RentalGoldLocked;
        public bool RentalItemLocked;
        public uint RentalGoldAmount;

        public SpecialItemMode ItemMode;

        public BaseStats CoreStats = new BaseStats(0);

        public virtual BuffDialog GetBuffDialog => GameScene.Scene.BuffsDialog;

        public UserItem[] Inventory = new UserItem[46], Equipment = new UserItem[14], Trade = new UserItem[10], QuestInventory = new UserItem[40];
        public int BeltIdx = 6, HeroBeltIdx = 2;
        public bool HasExpandedStorage = false;
        public DateTime ExpandedStorageExpiryTime;

        public Dictionary<Attribute, UserAttribute> AttributeValues = new Dictionary<Attribute, UserAttribute>();

        public List<ClientMagic> Magics = new List<ClientMagic>();
        public List<ItemSets> ItemSets = new List<ItemSets>();


        public List<ClientQuestProgress> CurrentQuests = new List<ClientQuestProgress>();
        public List<int> CompletedQuests = new List<int>();
        public List<ClientMail> Mail = new List<ClientMail>();

        public bool Slaying, Slaying2, Slaying3, Slaying4, Slaying5, Slaying6, Slaying7, Slaying8, Slaying9, Slaying10, Slaying11, Slaying12, Slaying13, Slaying14; 
        public bool HalfMoon, HalfMoon2, HalfMoon3, HalfMoon4, HalfMoon5, HalfMoon6, HalfMoon7, HalfMoon8, HalfMoon9, HalfMoon10, HalfMoon11, HalfMoon12, HalfMoon13, HalfMoon14;
        public bool FlamingSword, FlamingSword2, FlamingSword3, FlamingSword4, FlamingSword5, FlamingSword6, FlamingSword7, FlamingSword8, FlamingSword9, FlamingSword10, FlamingSword11, FlamingSword12, FlamingSword13, FlamingSword14;
        public ClientMagic NextMagic;
        public Point NextMagicLocation;
        public MapObject NextMagicObject;
        public MirDirection NextMagicDirection;
        public QueuedAction QueuedAction;

        public Spell GetActiveSlayingSpell()
        {
            Spell active = Spell.None;

            foreach (Spell slayingSpell in Shared.Extensions.SpellExtensions.SlayingSpells)
            {
                if (GetSlayingFlag(slayingSpell))
                {
                    active = slayingSpell;
                }
            }

            return active;
        }

        public Spell GetActiveHalfMoonSpell()
        {
            Spell active = Spell.None;

            foreach (Spell halfMoonSpell in SpellExtensions.HalfMoonSpells)
            {
                if (GetHalfMoonFlag(halfMoonSpell))
                {
                    active = halfMoonSpell;
                }
            }

            return active;
        }

        public Spell GetActiveFlamingSwordSpell()
        {
            Spell active = Spell.None;

            foreach (Spell flamingSwordSpell in SpellExtensions.FlamingSwordSpells)
            {
                if (GetFlamingSwordFlag(flamingSwordSpell))
                {
                    active = flamingSwordSpell;
                }
            }

            return active;
        }

        public bool GetSlayingFlag(Spell spell)
        {
            switch (spell)
            {
                case Spell.Slaying:
                    return Slaying;
                case Spell.Slaying2:
                    return Slaying2;
                case Spell.Slaying3:
                    return Slaying3;
                case Spell.Slaying4:
                    return Slaying4;
                case Spell.Slaying5:
                    return Slaying5;
                case Spell.Slaying6:
                    return Slaying6;
                case Spell.Slaying7:
                    return Slaying7;
                case Spell.Slaying8:
                    return Slaying8;
                case Spell.Slaying9:
                    return Slaying9;
                case Spell.Slaying10:
                    return Slaying10;
                case Spell.Slaying11:
                    return Slaying11;
                case Spell.Slaying12:
                    return Slaying12;
                case Spell.Slaying13:
                    return Slaying13;
                case Spell.Slaying14:
                    return Slaying14;
                default:
                    return false;
            }
        }

        public bool GetFlamingSwordFlag(Spell spell)
        {
            switch (spell)
            {
                case Spell.FlamingSword:
                    return FlamingSword;
                case Spell.FlamingSword2:
                    return FlamingSword2;
                case Spell.FlamingSword3:
                    return FlamingSword3;
                case Spell.FlamingSword4:
                    return FlamingSword4;
                case Spell.FlamingSword5:
                    return FlamingSword5;
                case Spell.FlamingSword6:
                    return FlamingSword6;
                case Spell.FlamingSword7:
                    return FlamingSword7;
                case Spell.FlamingSword8:
                    return FlamingSword8;
                case Spell.FlamingSword9:
                    return FlamingSword9;
                case Spell.FlamingSword10:
                    return FlamingSword10;
                case Spell.FlamingSword11:
                    return FlamingSword11;
                case Spell.FlamingSword12:
                    return FlamingSword12;
                case Spell.FlamingSword13:
                    return FlamingSword13;
                case Spell.FlamingSword14:
                    return FlamingSword14;
                default:
                    return false;
            }
        }

        public bool GetHalfMoonFlag(Spell spell)
        {
            switch (spell)
            {
                case Spell.HalfMoon:
                    return HalfMoon;
                case Spell.HalfMoon2:
                    return HalfMoon2;
                case Spell.HalfMoon3:
                    return HalfMoon3;
                case Spell.HalfMoon4:
                    return HalfMoon4;
                case Spell.HalfMoon5:
                    return HalfMoon5;
                case Spell.HalfMoon6:
                    return HalfMoon6;
                case Spell.HalfMoon7:
                    return HalfMoon7;
                case Spell.HalfMoon8:
                    return HalfMoon8;
                case Spell.HalfMoon9:
                    return HalfMoon9;
                case Spell.HalfMoon10:
                    return HalfMoon10;
                case Spell.HalfMoon11:
                    return HalfMoon11;
                case Spell.HalfMoon12:
                    return HalfMoon12;
                case Spell.HalfMoon13:
                    return HalfMoon13;
                case Spell.HalfMoon14:
                    return HalfMoon14;
                default:
                    return false;
            }
        }

        public void SetSlayingFlag(Spell spell, bool value)
        {
            switch (spell)
            {
                case Spell.Slaying:
                    Slaying = value;
                    break;
                case Spell.Slaying2:
                    Slaying2 = value;
                    break;
                case Spell.Slaying3:
                    Slaying3 = value;
                    break;
                case Spell.Slaying4:
                    Slaying4 = value;
                    break;
                case Spell.Slaying5:
                    Slaying5 = value;
                    break;
                case Spell.Slaying6:
                    Slaying6 = value;
                    break;
                case Spell.Slaying7:
                    Slaying7 = value;
                    break;
                case Spell.Slaying8:
                    Slaying8 = value;
                    break;
                case Spell.Slaying9:
                    Slaying9 = value;
                    break;
                case Spell.Slaying10:
                    Slaying10 = value;
                    break;
                case Spell.Slaying11:
                    Slaying11 = value;
                    break;
                case Spell.Slaying12:
                    Slaying12 = value;
                    break;
                case Spell.Slaying13:
                    Slaying13 = value;
                    break;
                case Spell.Slaying14:
                    Slaying14 = value;
                    break;
            }
        }

        public void SetFlamingSwordFlag(Spell spell, bool value)
        {
            if (value)
            {
                foreach (Spell flamingSwordSpell in SpellExtensions.FlamingSwordSpells)
                {
                    if (flamingSwordSpell == spell) continue;
                    SetFlamingSwordFlag(flamingSwordSpell, false);
                }
            }

            switch (spell)
            {
                case Spell.FlamingSword:
                    FlamingSword = value;
                    break;
                case Spell.FlamingSword2:
                    FlamingSword2 = value;
                    break;
                case Spell.FlamingSword3:
                    FlamingSword3 = value;
                    break;
                case Spell.FlamingSword4:
                    FlamingSword4 = value;
                    break;
                case Spell.FlamingSword5:
                    FlamingSword5 = value;
                    break;
                case Spell.FlamingSword6:
                    FlamingSword6 = value;
                    break;
                case Spell.FlamingSword7:
                    FlamingSword7 = value;
                    break;
                case Spell.FlamingSword8:
                    FlamingSword8 = value;
                    break;
                case Spell.FlamingSword9:
                    FlamingSword9 = value;
                    break;
                case Spell.FlamingSword10:
                    FlamingSword10 = value;
                    break;
                case Spell.FlamingSword11:
                    FlamingSword11 = value;
                    break;
                case Spell.FlamingSword12:
                    FlamingSword12 = value;
                    break;
                case Spell.FlamingSword13:
                    FlamingSword13 = value;
                    break;
                case Spell.FlamingSword14:
                    FlamingSword14 = value;
                    break;
            }
        }

        public void SetHalfMoonFlag(Spell spell, bool value)
        {
            switch (spell)
            {
                case Spell.HalfMoon:
                    HalfMoon = value;
                    break;
                case Spell.HalfMoon2:
                    HalfMoon2 = value;
                    break;
                case Spell.HalfMoon3:
                    HalfMoon3 = value;
                    break;
                case Spell.HalfMoon4:
                    HalfMoon4 = value;
                    break;
                case Spell.HalfMoon5:
                    HalfMoon5 = value;
                    break;
                case Spell.HalfMoon6:
                    HalfMoon6 = value;
                    break;
                case Spell.HalfMoon7:
                    HalfMoon7 = value;
                    break;
                case Spell.HalfMoon8:
                    HalfMoon8 = value;
                    break;
                case Spell.HalfMoon9:
                    HalfMoon9 = value;
                    break;
                case Spell.HalfMoon10:
                    HalfMoon10 = value;
                    break;
                case Spell.HalfMoon11:
                    HalfMoon11 = value;
                    break;
                case Spell.HalfMoon12:
                    HalfMoon12 = value;
                    break;
                case Spell.HalfMoon13:
                    HalfMoon13 = value;
                    break;
                case Spell.HalfMoon14:
                    HalfMoon14 = value;
                    break;
            }
        }

        public UserObject() { }
        public UserObject(uint objectID) : base(objectID)
        {
            Stats = new Stats();
        }

        public virtual void Load(S.UserInformation info)
        {
            Id = info.RealId;
            Name = info.Name;
            Settings.LoadTrackedQuests(info.Name);
            NameColour = info.NameColour;
            GuildName = info.GuildName;
            GuildRankName = info.GuildRank;
            Class = info.Class;
            Gender = info.Gender;
            Level = info.Level;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);

            Direction = info.Direction;
            Hair = info.Hair;

            HP = info.HP;
            MP = info.MP;

            Experience = info.Experience;
            MaxExperience = info.MaxExperience;

            LevelEffects = info.LevelEffects;

            Inventory = info.Inventory;
            Equipment = info.Equipment;
            QuestInventory = info.QuestInventory;

            HasExpandedStorage = info.HasExpandedStorage;
            ExpandedStorageExpiryTime = info.ExpandedStorageExpiryTime;

            Magics = info.Magics;
            for (int i = 0; i < Magics.Count; i++ )
            {
                Magics[i].CastTime += CMain.Time;
            }

            BindAllItems();

            RefreshStats();

            SetAction();
        }

        public override void SetLibraries()
        {
            base.SetLibraries();
        }

        public override void SetEffects()
        {
            base.SetEffects();
        }

        public void RefreshStats()
        {
            Stats.Clear();

            RefreshLevelStats();
            RefreshBagWeight();
            RefreshEquipmentStats();
            RefreshItemSetStats();
            RefreshSkills();
            RefreshBuffs();
            RefreshGuildBuffs();

            SetLibraries();
            SetEffects();

            Stats[Stat.HP] += (Stats[Stat.HP] * Stats[Stat.HPRatePercent]) / 100;
            Stats[Stat.MP] += (Stats[Stat.MP] * Stats[Stat.MPRatePercent]) / 100;
            Stats[Stat.MaxAC] += (Stats[Stat.MaxAC] * Stats[Stat.MaxACRatePercent]) / 100;
            Stats[Stat.MaxMAC] += (Stats[Stat.MaxMAC] * Stats[Stat.MaxMACRatePercent]) / 100;

            Stats[Stat.MaxDC] += (Stats[Stat.MaxDC] * Stats[Stat.MaxDCRatePercent]) / 100;
            Stats[Stat.MaxMC] += (Stats[Stat.MaxMC] * Stats[Stat.MaxMCRatePercent]) / 100;
            Stats[Stat.MaxSC] += (Stats[Stat.MaxSC] * Stats[Stat.MaxSCRatePercent]) / 100;
            Stats[Stat.AttackSpeed] += (Stats[Stat.AttackSpeed] * Stats[Stat.AttackSpeedRatePercent]) / 100;

            RefreshStatCaps();

            if (this == User && Light < 3) Light = 3;
            AttackSpeed = 1400 - ((Stats[Stat.AttackSpeed] * 60) + Math.Min(370, (Level * 14)));
            if (AttackSpeed < 550) AttackSpeed = 550;

            PercentHealth = (byte)(HP / (float)Stats[Stat.HP] * 100);
            PercentMana = (byte)(MP / (float)Stats[Stat.MP] * 100);

            GameScene.Scene.Redraw();
        }

        private void RefreshLevelStats()
        {
            Light = 0;

            foreach (var stat in CoreStats.Stats)
            {
                Stats[stat.Type] = stat.Calculate(Class, Level);
            }
        }

        private void RefreshBagWeight()
        {
            CurrentBagWeight = 0;

            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];
                if (item != null)
                {
                    CurrentBagWeight += item.Weight;
                }
            }
        }

        private void RefreshEquipmentStats()
        {
            Weapon = -1;
            Armour = 0;
            WingEffect = 0;

            CurrentWearWeight = 0;
            CurrentHandWeight = 0;

            ItemMode = SpecialItemMode.None;

            ItemSets.Clear();

            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem temp = Equipment[i];
                if (temp == null) continue;

                ItemInfo realItem = Functions.GetRealItem(temp.Info, Level, Class, GameScene.ItemInfoList);

                if (realItem.Type == ItemType.Weapon)
                    CurrentHandWeight = (int)Math.Min(int.MaxValue, CurrentHandWeight + temp.Weight);
                else
                    CurrentWearWeight = (int)Math.Min(int.MaxValue, CurrentWearWeight + temp.Weight);

                if (temp.CurrentDura == 0 && realItem.Durability > 0) continue;

                if (realItem.Type == ItemType.Armour)
                {
                    Armour = realItem.Shape;
                    WingEffect = realItem.Effect;
                }
                if (realItem.Type == ItemType.Weapon)
                {
                    Weapon = realItem.Shape;
                }

                Stats.Add(realItem.Stats);
                Stats.Add(temp.AddedStats);

                if (realItem.Light > Light) Light = realItem.Light;
                if (realItem.Unique != SpecialItemMode.None)
                {
                    ItemMode |= realItem.Unique;
                }

                if (realItem.Set == ItemSet.None) continue;

                ItemSets itemSet = ItemSets.Where(set => set.Set == realItem.Set && !set.Type.Contains(realItem.Type) && !set.SetComplete).FirstOrDefault();

                if (itemSet != null)
                {
                    itemSet.Type.Add(realItem.Type);
                    itemSet.Count++;
                }
                else
                {
                    ItemSets.Add(new ItemSets { Count = 1, Set = realItem.Set, Type = new List<ItemType> { realItem.Type } });
                }
            }

            if (ItemMode.HasFlag(SpecialItemMode.Muscle))
            {
                Stats[Stat.BagWeight] = Stats[Stat.BagWeight] * 2;
                Stats[Stat.WearWeight] = Stats[Stat.WearWeight] * 2;
                Stats[Stat.HandWeight] = Stats[Stat.HandWeight] * 2;
            }
        }

        private void RefreshItemSetStats()
        {
            foreach (var s in ItemSets)
            {
                if (!s.SetComplete) continue;
            }
        }

        private void RefreshSkills()
        {
            int[] spiritSwordLvPlus = { 0, 3, 5, 8 };
            int[] slayingLvPlus = {5, 6, 7, 8};
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                if (magic.Spell.IsSlaying())
                {
                    Stats[Stat.Accuracy] += magic.Level;
                    Stats[Stat.MaxDC] += slayingLvPlus[magic.Level];
                    continue;
                }

                switch (magic.Spell)
                {
                    case Spell.Fencing:
                        Stats[Stat.Accuracy] += magic.Level * 3;
                        //Stats[Stat.MaxAC] += (magic.Level + 1) * 3;
                        break;
                    case Spell.SpiritSword:
                        Stats[Stat.Accuracy] += spiritSwordLvPlus[magic.Level];
                        // Stats[Stat.Accuracy] += magic.Level;
                        // Stats[Stat.MaxDC] += (int)(Stats[Stat.MaxSC] * (magic.Level + 1) * 0.1F);
                        break;
                }
            }
        }

        private void RefreshBuffs()
        {
            BuffDialog dialog = GetBuffDialog;

            for (int i = 0; i < dialog.Buffs.Count; i++)
            {
                ClientBuff buff = dialog.Buffs[i];

                Stats.Add(buff.Stats);
            }
        }

        public void RefreshGuildBuffs()
        {
            if (User != this) return;
            if (GameScene.Scene.GuildDialog == null) return;
            for (int i = 0; i < GameScene.Scene.GuildDialog.EnabledBuffs.Count; i++)
            {
                GuildBuff buff = GameScene.Scene.GuildDialog.EnabledBuffs[i];
                if (buff == null) continue;
                if (!buff.Active) continue;

                if (buff.Info == null)
                {
                    buff.Info = GameScene.Scene.GuildDialog.FindGuildBuffInfo(buff.Id);
                }

                if (buff.Info == null) continue;

                Stats.Add(buff.Info.Stats);
            }
        }

        public void RefreshStatCaps()
        {
            foreach (var cap in CoreStats.Caps.Values)
            {
                Stats[cap.Key] = Math.Min(cap.Value, Stats[cap.Key]);
            }

            Stats[Stat.HP] = Math.Max(0, Stats[Stat.HP]);
            Stats[Stat.MP] = Math.Max(0, Stats[Stat.MP]);

            Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC]);
            Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC]);
            Stats[Stat.MinMAC] = Math.Max(0, Stats[Stat.MinMAC]);
            Stats[Stat.MaxMAC] = Math.Max(0, Stats[Stat.MaxMAC]);
            Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC]);
            Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC]);
            Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC]);
            Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC]);

            Stats[Stat.MinDC] = Math.Min(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Min(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Min(Stats[Stat.MinSC], Stats[Stat.MaxSC]);
        }

        public void BindAllItems()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null) continue;
                GameScene.Bind(Inventory[i]);
            }

            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] == null) continue;
                GameScene.Bind(Equipment[i]);
            }

            for (int i = 0; i < QuestInventory.Length; i++)
            {
                if (QuestInventory[i] == null) continue;
                GameScene.Bind(QuestInventory[i]);
            }
        }


        public ClientMagic GetMagic(Spell spell)
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                if (magic.Spell != spell) continue;
                return magic;
            }

            return null;
        }


        public void GetMaxGain(UserItem item)
        {
            int freeSpace = FreeSpace(Inventory);

            if (freeSpace > 0)
            {
                return;
            }

            ushort canGain = 0;

            foreach (UserItem inventoryItem in Inventory)
            {
                if (inventoryItem.Info != item.Info)
                {
                    continue;
                }

                int availableStack = inventoryItem.Info.StackSize - inventoryItem.Count;

                if (availableStack == 0)
                {
                    continue;
                }

                canGain += (ushort)availableStack;

                if (canGain >= item.Count)
                {
                    return;
                }
            }

            if (canGain == 0)
            {
                item.Count = 0;
                return;
            }

            item.Count = canGain;
        }
        private int FreeSpace(UserItem[] array)
        {
            int freeSlots = 0;

            foreach (UserItem slot in array)
            {
                if (slot == null)
                {
                    freeSlots++;
                }
            }

            return freeSlots;
        }

        public override void SetAction()
        {
            if (QueuedAction != null && !GameScene.Observing)
            {
                if (ActionFeed.Count == 0)
                {
                    ActionFeed.Clear();
                    ActionFeed.Add(QueuedAction);
                    QueuedAction = null;
                }
            }

            base.SetAction();
        }
        public override void ProcessFrames()
        {
            bool clear = CMain.Time >= NextMotion;

            base.ProcessFrames();

            if (clear) QueuedAction = null;
            if ((CurrentAction == MirAction.Standing || CurrentAction == MirAction.DashFail) && (QueuedAction != null || NextAction != null))
                SetAction();
        }

        public void ClearMagic()
        {
            NextMagic = null;
            NextMagicDirection = 0;
            NextMagicLocation = Point.Empty;
            NextMagicObject = null;
        } 
    }
}
