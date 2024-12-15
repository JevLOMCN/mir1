﻿using Client.MirScenes;
using Client.MirScenes.Dialogs;
using S = ServerPackets;

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

        public bool Slaying, Thrusting, HalfMoon, FlamingSword;
        public ClientMagic NextMagic;
        public Point NextMagicLocation;
        public MapObject NextMagicObject;
        public MirDirection NextMagicDirection;
        public QueuedAction QueuedAction;

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
                switch (magic.Spell)
                {
                    case Spell.Fencing:
                        Stats[Stat.Accuracy] += magic.Level * 3;
                        //Stats[Stat.MaxAC] += (magic.Level + 1) * 3;
                        break;
                    case Spell.Slaying:
                    // case Spell.FatalSword:
                        Stats[Stat.Accuracy] += magic.Level;
                        Stats[Stat.MaxDC] += slayingLvPlus[magic.Level];
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
                if ((ActionFeed.Count == 0) || (ActionFeed.Count == 1 && NextAction.Action == MirAction.Stance))
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
            if ((CurrentAction == MirAction.Standing || CurrentAction == MirAction.Stance || CurrentAction == MirAction.DashFail) && (QueuedAction != null || NextAction != null))
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

