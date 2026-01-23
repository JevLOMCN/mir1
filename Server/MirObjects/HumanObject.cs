using Server.MirDatabase;
using Server.MirEnvir;
using Server.MirNetwork;
using Server.MirObjects.Monsters;
using System.Numerics;
using S = ServerPackets;

namespace Server.MirObjects
{
    public class HumanObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Player; }
        }

        public CharacterInfo Info;

        protected MirConnection connection;
        public virtual MirConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }
        public override string Name
        {
            get { return Info.Name; }
            set { /*Check if Name exists.*/ }
        }
        public override int CurrentMapIndex
        {
            get { return Info.CurrentMapIndex; }
            set { Info.CurrentMapIndex = value; }
        }
        public override Point CurrentLocation
        {
            get { return Info.CurrentLocation; }
            set { Info.CurrentLocation = value; }
        }
        public override MirDirection Direction
        {
            get { return Info.Direction; }
            set { Info.Direction = value; }
        }
        public override ushort Level
        {
            get { return Info.Level; }
            set { Info.Level = value; }
        }
        public override int Health
        {
            get { return HP; }
        }
        public override int MaxHealth
        {
            get { return Stats[Stat.HP]; }
        }
        public int HP
        {
            get { return Info.HP; }
            set { Info.HP = value; }
        }
        public int MP
        {
            get { return Info.MP; }
            set { Info.MP = value; }
        }
        public override AttackMode AMode
        {
            get { return Info.AMode; }
            set { Info.AMode = value; }
        }
        public override PetMode PMode
        {
            get { return Info.PMode; }
            set { Info.PMode = value; }
        }
        public long Experience
        {
            set { Info.Experience = value; }
            get { return Info.Experience; }
        }

        public long MaxExperience;
        public byte Hair
        {
            get { return Info.Hair; }
            set { Info.Hair = value; }
        }
        public MirClass Class
        {
            get { return Info.Class; }
        }
        public MirGender Gender
        {
            get { return Info.Gender; }
        }
        public override List<Buff> Buffs
        {
            get { return Info.Buffs; }
            set { Info.Buffs = value; }
        }
        public override List<Poison> PoisonList
        {
            get { return Info.Poisons; }
            set { Info.Poisons = value; }
        }   

        public Reporting Report;
        public virtual bool CanMove
        {
            get
            {
                return !Dead && Envir.Time >= ActionTime && !CurrentPoison.HasFlag(PoisonType.Paralysis) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }
        public virtual bool CanWalk
        {
            get
            {
                return !Dead && Envir.Time >= ActionTime && !CurrentPoison.HasFlag(PoisonType.Paralysis) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }
        public virtual bool CanAttack
        {
            get
            {
                return !Dead && Envir.Time >= ActionTime && Envir.Time >= AttackTime && !CurrentPoison.HasFlag(PoisonType.Paralysis) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }
        public bool CanRegen
        {
            get
            {
                return Envir.Time >= RegenTime;
            }
        }
        protected virtual bool CanCast
        {
            get
            {
                return !Dead && Envir.Time >= ActionTime && Envir.Time >= SpellTime && !CurrentPoison.HasFlag(PoisonType.Stun) &&
                    !CurrentPoison.HasFlag(PoisonType.Paralysis) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }

        protected bool CheckCellTime = true;

        public short TransformType;
        public short Looks_Armour = 0, Looks_Weapon = -1, Looks_WeaponEffect = 0;
        public byte Looks_Wings = 0;

        public int CurrentHandWeight,
                   CurrentWearWeight,
                   CurrentBagWeight;

        public bool HasElemental;
        public int ElementsLevel;

        public bool Stacking;
        public bool IsGM, GMNeverDie, GMGameMaster;
        public bool HasUpdatedBaseStats = true;

        public uint MaximumAttributePoints => Settings.BaseAttributePoints + Level * Settings.GainAttributePoints;
        public long SpentAttributePoints => AttributeValues.Values.Sum(attribute => attribute.Points);
        public long AvailableAttributePoints => MaximumAttributePoints - SpentAttributePoints;
        public Dictionary<Attribute, UserAttribute> AttributeValues => Info.AttributeValues;

        public virtual int PotionBeltMinimum => 0;
        public virtual int PotionBeltMaximum => 4;
        public virtual int AmuletBeltMinimum => 4;
        public virtual int AmuletBeltMaximum => 6;
        public virtual int BeltSize => 6;

        public LevelEffects LevelEffects = LevelEffects.None;

        public const long LoyaltyDelay = 1000, ItemExpireDelay = 60000, DuraDelay = 10000, RegenDelay = 10000, PotDelay = 200, HealDelay = 600, VampDelay = 500, MoveDelay = 400;
        public long StruckTime, ActionTime, AttackTime, RegenTime, SpellTime, StackingTime, IncreaseLoyaltyTime, ItemExpireTime, DuraTime, PotTime, HealTime, VampTime, LogTime, DecreaseLoyaltyTime, SearchTime;

        private GuildObject myGuild = null;
        public virtual GuildObject MyGuild
        {
            get { return myGuild; }
            set { myGuild = value; }
        }
        public GuildRank MyGuildRank = null;

        public SpecialItemMode SpecialMode;

        public List<ItemSets> ItemSets = new List<ItemSets>();
        public List<EquipmentSlot> MirSet = new List<EquipmentSlot>();

        public bool Slaying, Slaying2, Slaying3, Slaying4, Slaying5, Slaying6, Slaying7, Slaying8, Slaying9, Slaying10, Slaying11, Slaying12, Slaying13, Slaying14;
        public bool FlamingSword;
        public long FlamingSwordTime;

        public long LastRevivalTime;
        public float HpDrain = 0;

        public bool UnlockCurse = false;
        public bool FastRun = false;
        public bool CanGainExp = true;
        public override bool Blocking
        {
            get
            {
                return !Dead && !Observer;
            }
        }
        public HumanObject() { }
        public HumanObject(CharacterInfo info, MirConnection connection)
        {
            Load(info, connection);
        }
        protected virtual void Load(CharacterInfo info, MirConnection connection) { }
        protected virtual void NewCharacter()
        {
            Level = 1;

            for (int i = 0; i < Envir.StartItems.Count; i++)
            {
                ItemInfo info = Envir.StartItems[i];
                if (!CorrectStartItem(info)) continue;

                AddItem(Envir.CreateFreshItem(info));
            }
        }
        public long GetDelayTime(long original)
        {
            if (CurrentPoison.HasFlag(PoisonType.Slow))
            {
                return original * 2;
            }
            return original;
        }
        public override void Process()
        {
            if ((Race == ObjectType.Player && Connection == null) || Node == null || Info == null) return;

            if (FlamingSword && Envir.Time >= FlamingSwordTime)
            {
                FlamingSword = false;
                Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.FlamingSword, CanUse = false });
            }

            if (Stacking && Envir.Time > StackingTime)
            {
                Stacking = false;

                for (int i = 0; i < 8; i++)
                {
                    if (Pushed(this, (MirDirection)i, 1) == 1) break;
                }
            }

            if (Envir.Time > ItemExpireTime)
            {
                ItemExpireTime = Envir.Time + ItemExpireDelay;

                ProcessItems();
            }

            for (int i = Pets.Count() - 1; i >= 0; i--)
            {
                MonsterObject pet = Pets[i];
                if (pet.Dead) Pets.Remove(pet);
            }

            ProcessBuffs();
            ProcessRegen();
            ProcessPoison();

            UserItem item;

            if (Envir.Time > DuraTime)
            {
                DuraTime = Envir.Time + DuraDelay;

                for (int i = 0; i < Info.Equipment.Length; i++)
                {
                    item = Info.Equipment[i];
                    if (item == null || !item.DuraChanged) continue; // || item.Info.Type == ItemType.Mount
                    item.DuraChanged = false;
                    Enqueue(new S.DuraChanged { UniqueID = item.UniqueID, CurrentDura = item.CurrentDura });
                }
            }

            base.Process();

            RefreshNameColour();
        }

        public override void OnSafeZoneChanged()
        {
            base.OnSafeZoneChanged();

            bool needsUpdate = false;

            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].ObjectID == 0) continue;
                if (!Buffs[i].Properties.HasFlag(BuffProperty.PauseInSafeZone)) continue;

                needsUpdate = true;

                if (InSafeZone)
                {
                    PauseBuff(Buffs[i]);
                }
                else
                {
                    UnpauseBuff(Buffs[i]);
                }
            }

            if (needsUpdate)
            {
                RefreshStats();
            }
        }

        public override void SetOperateTime()
        {
            OperateTime = Envir.Time;
        }
        public override void Die() { }
        protected virtual void ProcessBuffs()
        {
            bool refresh = false;
            bool clearRing = false, skill = false, gm = false, mentor = false, lover = false;

            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = Buffs[i];

                switch (buff.Type)
                {
                    case BuffType.ClearRing:
                        clearRing = true;
                        if (!SpecialMode.HasFlag(SpecialItemMode.ClearRing)) buff.FlagForRemoval = true;
                        break;
                    case BuffType.Skill:
                        skill = true;
                        if (!SpecialMode.HasFlag(SpecialItemMode.Skill)) buff.FlagForRemoval = true;
                        break;
                    case BuffType.GameMaster:
                        gm = true;
                        if (!IsGM) buff.FlagForRemoval = true;
                        break;
                    case BuffType.Lover:
                        lover = true;
                        if (Info.Married == 0) buff.FlagForRemoval = true;
                        break;
                }

                if (buff.NextTime > Envir.Time) continue;

                if (!buff.Paused && buff.StackType != BuffStackType.Infinite)
                {
                    var change = Envir.Time - buff.LastTime;
                    buff.ExpireTime -= change;
                }

                buff.LastTime = Envir.Time;
                buff.NextTime = Envir.Time + 1000;

                if ((buff.ExpireTime > 0 || buff.StackType == BuffStackType.Infinite) && !buff.FlagForRemoval) continue;

                Buffs.RemoveAt(i);
                Enqueue(new S.RemoveBuff { Type = buff.Type, ObjectID = ObjectID });

                if (buff.Info.Visible)
                {
                    Broadcast(new S.RemoveBuff { Type = buff.Type, ObjectID = ObjectID });
                }

                switch (buff.Type)
                {
                    case BuffType.Hiding:
                    case BuffType.ClearRing:
                        if (!HasAnyBuffs(buff.Type, BuffType.ClearRing, BuffType.Hiding))
                        {
                            Hidden = false;
                        }                        
                        break;                   
                    case BuffType.MagicShield:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.MagicShieldDown }, CurrentLocation);
                        break;
                }

                refresh = true;
            }

            if (IsGM && !gm)
            {
                UpdateGMBuff();
            }

            if (SpecialMode.HasFlag(SpecialItemMode.ClearRing) && !clearRing)
            {
                AddBuff(BuffType.ClearRing, this, 0, new Stats());
            }

            if (SpecialMode.HasFlag(SpecialItemMode.Skill) && !skill)
            {
                AddBuff(BuffType.Skill, this, 0, new Stats { [Stat.SkillGainMultiplier] = 3 }, false);
            }

            if (Info.Married != 0 && !lover)
            {
                CharacterInfo loverC = Envir.GetCharacterInfo(Info.Married);
                PlayerObject loverP = loverC != null ? Envir.GetPlayer(loverC.Name) : null;

                if (loverP != null)
                {
                    AddBuff(BuffType.Lover, loverP, 0, new Stats { [Stat.LoverExpRatePercent] = Settings.LoverEXPBonus });
                }
            }

            if (refresh)
            {
                RefreshStats();
            }
        }
        private void ProcessRegen()
        {
            if (Dead) return;

            int healthRegen = 0, manaRegen = 0;

            if (CanRegen)
            {
                RegenTime = Envir.Time + RegenDelay;

                if (HP < Stats[Stat.HP])
                {
                    healthRegen += (int)(Stats[Stat.HP] * 0.03F) + 1;
                    healthRegen += (int)(healthRegen * ((double)Stats[Stat.HealthRecovery] / Settings.HealthRegenWeight));
                }

                if (MP < Stats[Stat.MP])
                {
                    manaRegen += (int)(Stats[Stat.MP] * 0.03F) + 1;
                    manaRegen += (int)(manaRegen * ((double)Stats[Stat.SpellRecovery] / Settings.ManaRegenWeight));
                }
            }

            if (Envir.Time > PotTime)
            {
                //PotTime = Envir.Time + Math.Max(50,Math.Min(PotDelay, 600 - (Level * 10)));
                PotTime = Envir.Time + PotDelay;
                int PerTickRegen = 5 + (Level / 10);

                if (PotHealthAmount > PerTickRegen)
                {
                    healthRegen += PerTickRegen;
                    PotHealthAmount -= (ushort)PerTickRegen;
                }
                else
                {
                    healthRegen += PotHealthAmount;
                    PotHealthAmount = 0;
                }

                if (PotManaAmount > PerTickRegen)
                {
                    manaRegen += PerTickRegen;
                    PotManaAmount -= (ushort)PerTickRegen;
                }
                else
                {
                    manaRegen += PotManaAmount;
                    PotManaAmount = 0;
                }
            }

            if (Envir.Time > HealTime)
            {
                HealTime = Envir.Time + HealDelay;

                int incHeal = (Level / 10) + (HealAmount / 10);
                if (HealAmount > (5 + incHeal))
                {
                    healthRegen += (5 + incHeal);
                    HealAmount -= (ushort)Math.Min(HealAmount, 5 + incHeal);
                }
                else
                {
                    healthRegen += HealAmount;
                    HealAmount = 0;
                }
            }

            if (Envir.Time > VampTime)
            {
                VampTime = Envir.Time + VampDelay;

                if (VampAmount > 10)
                {
                    healthRegen += 10;
                    VampAmount -= 10;
                }
                else
                {
                    healthRegen += VampAmount;
                    VampAmount = 0;
                }
            }

            if (healthRegen > 0)
            {
                ChangeHP(healthRegen);
                BroadcastDamageIndicator(DamageType.Hit, healthRegen);
            }

            if (HP == Stats[Stat.HP])
            {
                PotHealthAmount = 0;
                HealAmount = 0;
            }

            if (manaRegen > 0) ChangeMP(manaRegen);
            if (MP == Stats[Stat.MP]) PotManaAmount = 0;
        }
        private void ProcessPoison()
        {
            PoisonType type = PoisonType.None;
            ArmourRate = 1F;
            DamageRate = 1F;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                if (Dead) return;

                Poison poison = PoisonList[i];

                if (poison.Owner != null && poison.Owner.Node == null)
                {
                    PoisonList.RemoveAt(i);
                    continue;
                }

                if (Envir.Time > poison.TickTime)
                {
                    poison.Time++;
                    poison.TickTime = Envir.Time + poison.TickSpeed;

                    if (poison.Time >= poison.Duration)
                    {
                        PoisonList.RemoveAt(i);
                    }

                    if (poison.PType == PoisonType.Green)
                    {
                        LastHitter = poison.Owner;
                        LastHitTime = Envir.Time + 10000;

                        PoisonDamage(-poison.Value, poison.Owner);
                        BroadcastDamageIndicator(DamageType.Hit, -poison.Value);

                        if (Dead) break;
                        RegenTime = Envir.Time + RegenDelay;
                    }
                }

                switch (poison.PType)
                {
                    case PoisonType.Red:
                        ArmourRate -= 0.10F;
                        break;
                    case PoisonType.Stun:
                        DamageRate += 0.20F;
                        break;
                }

                type |= poison.PType;
            }

            if (type == CurrentPoison) return;

            Enqueue(new S.Poisoned { Poison = type });
            Broadcast(new S.ObjectPoisoned { ObjectID = ObjectID, Poison = type });

            CurrentPoison = type;
        }
        private void ProcessItems()
        {
            for (var i = 0; i < Info.Inventory.Length; i++)
            {
                var item = Info.Inventory[i];

                if (item?.ExpireInfo?.ExpiryDate <= Envir.Now)
                {
                    ReceiveChat($"{item.Info.FriendlyName} has just expired from your inventory.", ChatType.Hint);
                    Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                    Info.Inventory[i] = null;
                    continue;
                }
            }

            for (var i = 0; i < Info.Equipment.Length; i++)
            {
                var item = Info.Equipment[i];

                if (item?.ExpireInfo?.ExpiryDate <= Envir.Now)
                {
                    ReceiveChat($"{item.Info.FriendlyName} has just expired from your equipment.", ChatType.Hint);
                    Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                    Info.Equipment[i] = null;
                    continue;
                }
            }

            if (Info.AccountInfo == null) return;

            for (int i = 0; i < Info.AccountInfo.Storage.Length; i++)
            {
                var item = Info.AccountInfo.Storage[i];
                if (item?.ExpireInfo?.ExpiryDate <= Envir.Now)
                {
                    ReceiveChat($"{item.Info.FriendlyName} has just expired from your storage.", ChatType.Hint);
                    Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                    Info.AccountInfo.Storage[i] = null;
                    continue;
                }
            }
        }
        public override void Process(DelayedAction action)
        {
        }
        protected void UpdateGMBuff()
        {
            if (!IsGM) return;

            GMOptions options = GMOptions.None;

            if (GMGameMaster) options |= GMOptions.GameMaster;
            if (GMNeverDie) options |= GMOptions.Superman;
            if (Observer) options |= GMOptions.Observer;

            AddBuff(BuffType.GameMaster, this, 0, null, false, values: (byte)options);
        }
        public virtual void LevelUp()
        {
            foreach (Attribute attribute in Enum.GetValues<Attribute>())
            {
                var attributeValue = AttributeValues[attribute];
                if (attributeValue.Points == 0) continue;

                attributeValue.Experience += attributeValue.Points;
                while (attributeValue.Experience >= Settings.AttributePointsMaxExperience)
                {
                    attributeValue.Level++;
                    attributeValue.Experience -= Settings.AttributePointsMaxExperience;

                    //TODO set class when attribute reaches level 11
                    //TODO Increase some stats based on attributes in RefreshStats
                }
            }

            SendAttributes();
            RefreshStats();
            SetHP(Stats[Stat.HP]);
            SetMP(Stats[Stat.MP]);
            
            Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });          
        }
        public virtual Color GetNameColour(HumanObject human)
        {
            return NameColour;
        }
        public virtual void RefreshNameColour() { }
        protected void SetHP(int amount)
        {
            if (HP == amount) return;

            HP = amount <= Stats[Stat.HP] ? amount : Stats[Stat.HP];
            HP = GMNeverDie ? Stats[Stat.HP] : HP;

            if (!Dead && HP == 0) Die();

            //HealthChanged = true;
            SendHealthChanged();
        }
        protected virtual void SendHealthChanged()
        {
            Enqueue(new S.HealthChanged { HP = HP, MP = MP });
            BroadcastHealthChange();
        }
        protected void SetMP(int amount)
        {
            if (MP == amount) return;
            //was info.MP
            MP = amount <= Stats[Stat.MP] ? amount : Stats[Stat.MP];
            MP = GMNeverDie ? Stats[Stat.MP] : MP;

            // HealthChanged = true;
            SendHealthChanged();
        }
        public void ChangeHP(int amount)
        {
            if (SpecialMode.HasFlag(SpecialItemMode.Protection) && MP > 0 && amount < 0)
            {
                ChangeMP(amount);
                return;
            }

            if (HP + amount > Stats[Stat.HP])
                amount = Stats[Stat.HP] - HP;

            if (amount == 0) return;

            HP += amount;
            HP = GMNeverDie ? Stats[Stat.HP] : HP;

            if (HP < 0) HP = 0;

            if (!Dead && HP == 0) Die();

            // HealthChanged = true;
            SendHealthChanged();
        }
        public void PoisonDamage(int amount, MapObject Attacker)
        {
            ChangeHP(amount);
        }
        public void ChangeMP(int amount)
        {
            if (MP + amount > Stats[Stat.MP])
                amount = Stats[Stat.MP] - MP;

            if (amount == 0) return;

            MP += amount;
            MP = GMNeverDie ? Stats[Stat.MP] : MP;

            if (MP < 0) MP = 0;

            // HealthChanged = true;
            SendHealthChanged();
        }

        public virtual bool CheckGroupQuestItem(UserItem item, bool gainItem = true)
        {
            return false;
        }
        protected bool TryLuckWeapon()
        {
            var item = Info.Equipment[(int)EquipmentSlot.Weapon];

            if (item == null || item.AddedStats[Stat.Luck] >= 7)
                return false;

            if (item.Info.Bind.HasFlag(BindMode.DontUpgrade))
                return false;

            string message = String.Empty;
            ChatType chatType;

            if (item.AddedStats[Stat.Luck] > (Settings.MaxLuck * -1) && Envir.Random.Next(20) == 0)
            {
                Stats[Stat.Luck]--;
                item.AddedStats[Stat.Luck]--;
                Enqueue(new S.RefreshItem { Item = item });

                message = GameLanguage.WeaponCurse;
                chatType = ChatType.System;
                
            }
            else if (item.AddedStats[Stat.Luck] <= 0 || Envir.Random.Next(10 * item.GetTotal(Stat.Luck)) == 0)
            {
                Stats[Stat.Luck]++;
                item.AddedStats[Stat.Luck]++;
                Enqueue(new S.RefreshItem { Item = item });

                message = GameLanguage.WeaponLuck;
                chatType = ChatType.Hint;
            }
            else
            {
                message = GameLanguage.WeaponNoEffect;
                chatType = ChatType.Hint;
            }

            ReceiveChat(message, chatType);

            return true;
        }
        protected bool CanUseItem(UserItem item)
        {
            if (item == null) return false;

            switch (Gender)
            {
                case MirGender.Male:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                    {
                        ReceiveChat(GameLanguage.NotFemale, ChatType.System);
                        return false;
                    }
                    break;
                case MirGender.Female:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                    {
                        ReceiveChat(GameLanguage.NotMale, ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (Class)
            {
                case MirClass.Warrior:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                    {
                        ReceiveChat("Warriors cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Wizard:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                    {
                        ReceiveChat("Wizards cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
                case MirClass.Taoist:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                    {
                        ReceiveChat("Taoists cannot use this item.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (Level < item.Info.RequiredAmount)
                    {
                        ReceiveChat(GameLanguage.LowLevel, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxAC:
                    if (Stats[Stat.MaxAC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough AC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMAC:
                    if (Stats[Stat.MaxMAC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough MAC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxDC:
                    if (Stats[Stat.MaxDC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat(GameLanguage.LowDC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMC:
                    if (Stats[Stat.MaxMC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat(GameLanguage.LowMC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxSC:
                    if (Stats[Stat.MaxSC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat(GameLanguage.LowSC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxLevel:
                    if (Level > item.Info.RequiredAmount)
                    {
                        ReceiveChat("You have exceeded the maximum level.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinAC:
                    if (Stats[Stat.MinAC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough Base AC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMAC:
                    if (Stats[Stat.MinMAC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough Base MAC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinDC:
                    if (Stats[Stat.MinDC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough Base DC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMC:
                    if (Stats[Stat.MinMC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough Base MC.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinSC:
                    if (Stats[Stat.MinSC] < item.Info.RequiredAmount)
                    {
                        ReceiveChat("You do not have enough Base SC.", ChatType.System);
                        return false;
                    }
                    break;
                default:
                    if (Functions.TryGetRequiredAttribute(item.Info.RequiredType, out Attribute attribute))
                    {
                        if (!AttributeValues.TryGetValue(attribute, out UserAttribute attributeValue) || attributeValue.Level < item.Info.RequiredAmount)
                        {
                            ReceiveChat($"You do not have enough {attribute}.", ChatType.System);
                            return false;
                        }
                    }
                    break;
            }

            switch (item.Info.Type)
            {
                case ItemType.Scroll:
                    switch (item.Info.Shape)
                    {
                        case 0:
                            if (CurrentMap.Info.NoEscape)
                            {
                                ReceiveChat(GameLanguage.CanNotDungeon, ChatType.System);
                                return false;
                            }
                            break;
                        case 1:
                            if (CurrentMap.Info.NoTownTeleport)
                            {
                                ReceiveChat(GameLanguage.NoTownTeleport, ChatType.System);
                                return false;
                            }
                            break;
                        case 2:
                            if (CurrentMap.Info.NoRandom)
                            {
                                ReceiveChat(GameLanguage.CanNotRandom, ChatType.System);
                                return false;
                            }
                            break;
                        case 6:
                            if (!Dead)
                            {
                                ReceiveChat(GameLanguage.CannotResurrection, ChatType.Hint);
                                return false;
                            }
                            break;
                        case 10:
                            {
                                int skillId = item.Info.Effect;

                                if (MyGuild == null)
                                {
                                    ReceiveChat("You must be in a guild to use this skill", ChatType.Hint);
                                    return false;
                                }
                                if (MyGuildRank != MyGuild.Ranks[0])
                                {
                                    ReceiveChat("You must be the guild leader to use this skill", ChatType.Hint);
                                    return false;
                                }
                                GuildBuffInfo buffInfo = Envir.FindGuildBuffInfo(skillId);

                                if (buffInfo == null) return false;

                                if (MyGuild.BuffList.Any(e => e.Info.Id == skillId))
                                {
                                    ReceiveChat("Your guild already has this skill", ChatType.Hint);
                                    return false;
                                }
                            }
                            break;
                    }
                    break;
                case ItemType.Potion:
                    if (CurrentMap.Info.NoDrug)
                    {
                        ReceiveChat("You cannot use Potions here", ChatType.System);
                        return false;
                    }
                    break;

                case ItemType.Book:
                    if (Info.Magics.Any(t => t.Spell == (Spell)item.Info.Shape))
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }
        public virtual void UseItem(ulong id) { }
        protected void ConsumeItem(UserItem item, byte cost)
        {
            item.Count -= cost;
            Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = cost });

            if (item.Count != 0) return;

            for (int i = 0; i < Info.Equipment.Length; i++)
            {
                if (Info.Equipment[i] != item) continue;
                Info.Equipment[i] = null;

                return;
            }

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                if (Info.Inventory[i] != item) continue;
                Info.Inventory[i] = null;
                return;
            }

            //Item not found
        }
        protected bool DropItem(UserItem item, int range = 1, bool DeathDrop = false)
        {
            ItemObject ob = new ItemObject(this, item, DeathDrop);

            if (!ob.Drop(range)) return false;

            if (item.Info.Type == ItemType.Meat)
                item.CurrentDura = (ushort)Math.Max(0, item.CurrentDura - 2000);

            return true;
        }
        protected void DeathDrop(MapObject killer)
        {
            var pkbodydrop = true;

            if (CurrentMap.Info.NoDropPlayer && Race == ObjectType.Player)
                return;

            if ((killer == null) || ((pkbodydrop) || (killer.Race != ObjectType.Player)))
            {
                for (var i = 0; i < Info.Equipment.Length; i++)
                {
                    var item = Info.Equipment[i];

                    if (item == null)
                        continue;

                    if (item.Info.Bind.HasFlag(BindMode.DontDeathdrop))
                        continue;

                    // TODO: Check this.
                    if (item.WeddingRing != -1 && Info.Equipment[(int)EquipmentSlot.RingL].UniqueID == item.UniqueID)
                        continue;

                    if (item.SealedInfo != null && item.SealedInfo.ExpiryDate > Envir.Now)
                        continue;

                    if (((killer == null) || ((killer != null) && (killer.Race != ObjectType.Player))))
                    {
                        if (item.Info.Bind.HasFlag(BindMode.BreakOnDeath))
                        {
                            Info.Equipment[i] = null;
                            Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                            ReceiveChat($"Your {item.FriendlyName} shattered upon death.", ChatType.System2);
                            Report?.ItemChanged(item, item.Count, 1);
                        }
                    }

                    if (item.Count > 1)
                    {
                        var percent = Envir.RandomomRange(10, 8);
                        var count = (ushort)Math.Ceiling(item.Count / 10F * percent);

                        if (count > item.Count)
                            throw new ArgumentOutOfRangeException();

                        var temp2 = Envir.CreateFreshItem(item.Info);
                        temp2.Count = count;

                        if (!DropItem(temp2, Settings.DropRange, true))
                            continue;

                        if (count == item.Count)
                            Info.Equipment[i] = null;

                        Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = count });
                        item.Count -= count;

                        Report?.ItemChanged(item, count, 1);
                    }
                    else if (Envir.Random.Next(30) == 0)
                    {
                        if (!DropItem(item, Settings.DropRange, true))
                        {
                            continue;
                        }

                        if (item.Info.GlobalDropNotify)
                        {
                            foreach (var player in Envir.Players)
                            {
                                player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
                            }
                        }

                        Info.Equipment[i] = null;
                        Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });

                        Report?.ItemChanged(item, item.Count, 1);
                    }
                }

            }

            for (var i = 0; i < Info.Inventory.Length; i++)
            {
                var item = Info.Inventory[i];

                if (item == null)
                    continue;

                if (item.Info.Bind.HasFlag(BindMode.DontDeathdrop))
                    continue;

                if (item.WeddingRing != -1)
                    continue;

                if (item.SealedInfo != null && item.SealedInfo.ExpiryDate > Envir.Now)
                    continue;

                if (item.Count > 1)
                {
                    var percent = Envir.RandomomRange(10, 8);

                    if (percent == 0)
                        continue;

                    var count = (ushort)Math.Ceiling(item.Count / 10F * percent);

                    if (count > item.Count)
                        throw new ArgumentOutOfRangeException();

                    var temp2 = Envir.CreateFreshItem(item.Info);
                    temp2.Count = count;

                    if (!DropItem(temp2, Settings.DropRange, true))
                        continue;

                    if (count == item.Count)
                        Info.Inventory[i] = null;

                    Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = count });
                    item.Count -= count;

                    Report?.ItemChanged(item, count, 1);
                }
                else if (Envir.Random.Next(10) == 0)
                {
                    if (!DropItem(item, Settings.DropRange, true))
                        continue;

                    if (item.Info.GlobalDropNotify)
                        foreach (var player in Envir.Players)
                        {
                            player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
                        }

                    Info.Inventory[i] = null;
                    Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });

                    Report?.ItemChanged(item, item.Count, 1);
                }
            }

            RefreshStats();
        }
        protected static int FreeSpace(IList<UserItem> array)
        {
            int count = 0;

            for (int i = 0; i < array.Count; i++)
                if (array[i] == null) count++;

            return count;
        }
        protected void AddItem(UserItem item)
        {
            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    UserItem temp = Info.Inventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= (ushort)(temp.Info.StackSize - temp.Count);
                    temp.Count = temp.Info.StackSize;
                }
            }

            if (item.Info.Type == ItemType.Potion || item.Info.Type == ItemType.Scroll || (item.Info.Type == ItemType.Script && item.Info.Effect == 1))
            {
                for (int i = PotionBeltMinimum; i < PotionBeltMaximum; i++)
                {
                    if (Info.Inventory[i] != null) continue;
                    Info.Inventory[i] = item;
                    return;
                }
            }
                        else
            {
                for (int i = BeltSize; i < Info.Inventory.Length; i++)
                {
                    if (Info.Inventory[i] != null) continue;
                    Info.Inventory[i] = item;
                    return;
                }
            }

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                if (Info.Inventory[i] != null) continue;
                Info.Inventory[i] = item;
                return;
            }
        }
        protected bool CorrectStartItem(ItemInfo info)
        {
            switch (Class)
            {
                case MirClass.Warrior:
                    if (!info.RequiredClass.HasFlag(RequiredClass.Warrior)) return false;
                    break;
                case MirClass.Wizard:
                    if (!info.RequiredClass.HasFlag(RequiredClass.Wizard)) return false;
                    break;
                case MirClass.Taoist:
                    if (!info.RequiredClass.HasFlag(RequiredClass.Taoist)) return false;
                    break;
                default:
                    return false;
            }

            switch (Gender)
            {
                case MirGender.Male:
                    if (!info.RequiredGender.HasFlag(RequiredGender.Male)) return false;
                    break;
                case MirGender.Female:
                    if (!info.RequiredGender.HasFlag(RequiredGender.Female)) return false;
                    break;
                default:
                    return false;
            }

            return true;
        }
        public void CheckItemInfo(ItemInfo info, bool dontLoop = false)
        {
            Connection.CheckItemInfo(info, dontLoop);
        }
        public void CheckItem(UserItem item)
        {
            Connection.CheckItem(item);         
        }
        public void SetLevelEffects()
        {
            LevelEffects = LevelEffects.None;

            if (Info.Flags[990]) LevelEffects |= LevelEffects.Mist;
            if (Info.Flags[991]) LevelEffects |= LevelEffects.RedDragon;
            if (Info.Flags[992]) LevelEffects |= LevelEffects.BlueDragon;
            if (Info.Flags[993]) LevelEffects |= LevelEffects.Rebirth1;
            if (Info.Flags[994]) LevelEffects |= LevelEffects.Rebirth2;
            if (Info.Flags[995]) LevelEffects |= LevelEffects.Rebirth3;
            if (Info.Flags[996]) LevelEffects |= LevelEffects.NewBlue;
            if (Info.Flags[997]) LevelEffects |= LevelEffects.YellowDragon;
            if (Info.Flags[998]) LevelEffects |= LevelEffects.Phoenix;
        }
        public virtual void Revive(int hp, bool effect)
        {
            if (!Dead) return;

            Dead = false;
            SetHP(hp);

            CurrentMap.RemoveObject(this);
            Broadcast(new S.ObjectRemove { ObjectID = ObjectID });

            CurrentMap = CurrentMap;
            CurrentLocation = CurrentLocation;

            CurrentMap.AddObject(this);

            Enqueue(new S.MapChanged
            {
                MapIndex = CurrentMap.Info.Index,
                FileName = CurrentMap.Info.FileName,
                Title = CurrentMap.Info.Title,
                MiniMap = CurrentMap.Info.MiniMap,
                BigMap = CurrentMap.Info.BigMap,
                Lights = CurrentMap.Info.Light,
                Location = CurrentLocation,
                Direction = Direction,
                MapDarkLight = CurrentMap.Info.MapDarkLight,
                Music = CurrentMap.Info.Music
            });

            Enqueue(new S.Revived());
            Broadcast(new S.ObjectRevived { ObjectID = ObjectID, Effect = effect });
        }

        protected virtual void SendBaseStats()
        {
            Enqueue(new S.BaseStatsInfo { Stats = Settings.ClassBaseStats[(byte)Class] });
        }

        public void SendAttributes()
        {
            Enqueue(new S.AttributePoints { Attributes = AttributeValues.Values.ToList() });
        }

        public void AttributeDeltas(Dictionary<Attribute, int> deltas)
        {
            foreach (var delta in deltas)
            {
                long value = delta.Value;
                if (value > AvailableAttributePoints)
                    value = AvailableAttributePoints;

                AttributeValues[delta.Key].Points = (uint)Math.Max(0, AttributeValues[delta.Key].Points + delta.Value);
            }

            SendAttributes();
        }

        #region Refresh Stats
        public void RefreshStats()
        {
            if (HasUpdatedBaseStats == false)
            {
                SendBaseStats();                
                HasUpdatedBaseStats = true;
            }

            Stats.Clear();

            RefreshLevelStats();
            RefreshBagWeight();
            RefreshEquipmentStats();
            RefreshItemSetStats();
            RefreshSkills();
            RefreshBuffs();
            RefreshGuildBuffs();

            //Add any rate percent changes

            Stats[Stat.HP] += (Stats[Stat.HP] * Stats[Stat.HPRatePercent]) / 100;
            Stats[Stat.MP] += (Stats[Stat.MP] * Stats[Stat.MPRatePercent]) / 100;
            Stats[Stat.MaxAC] += (Stats[Stat.MaxAC] * Stats[Stat.MaxACRatePercent]) / 100;
            Stats[Stat.MaxMAC] += (Stats[Stat.MaxMAC] * Stats[Stat.MaxMACRatePercent]) / 100;

            Stats[Stat.MaxDC] += (Stats[Stat.MaxDC] * Stats[Stat.MaxDCRatePercent]) / 100;
            Stats[Stat.MaxMC] += (Stats[Stat.MaxMC] * Stats[Stat.MaxMCRatePercent]) / 100;
            Stats[Stat.MaxSC] += (Stats[Stat.MaxSC] * Stats[Stat.MaxSCRatePercent]) / 100;
            Stats[Stat.AttackSpeed] += (Stats[Stat.AttackSpeed] * Stats[Stat.AttackSpeedRatePercent]) / 100;

            RefreshStatCaps();

            if (HP > Stats[Stat.HP]) SetHP(Stats[Stat.HP]);
            if (MP > Stats[Stat.MP]) SetMP(Stats[Stat.MP]);

            AttackSpeed = 1400 - ((Stats[Stat.AttackSpeed] * 60) + Math.Min(370, (Level * 14)));

            if (AttackSpeed < 550) AttackSpeed = 550;
        }
        public virtual void RefreshGuildBuffs() { }
        protected void RefreshLevelStats()
        {
            MaxExperience = Level < Settings.ExperienceList.Count ? Settings.ExperienceList[Level - 1] : 0;

            foreach (var stat in Settings.ClassBaseStats[(byte)Class].Stats)
            {
                Stats[stat.Type] = stat.Calculate(Class, Level);
            }
        }
        public void RefreshBagWeight()
        {
            CurrentBagWeight = 0;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                UserItem item = Info.Inventory[i];
                if (item != null)
                {
                    CurrentBagWeight += item.Weight;
                }
            }
        }
        private void RefreshEquipmentStats()
        {
            short OldLooks_Weapon = Looks_Weapon;
            short OldLooks_Armour = Looks_Armour;
            byte OldLooks_Wings = Looks_Wings;
            byte OldLight = Light;

            Looks_Armour = 0;
            Looks_Weapon = -1;
            Looks_WeaponEffect = 0;
            Looks_Wings = 0;
            Light = 0;
            CurrentWearWeight = 0;
            CurrentHandWeight = 0;

            SpecialMode = SpecialItemMode.None;

            Stats[Stat.SkillGainMultiplier] = 1;

            var skillsToAdd = new List<string>();
            var skillsToRemove = new List<string> { Settings.HealRing, Settings.FireRing, Settings.BlinkSkill };

            ItemSets.Clear();
            MirSet.Clear();

            for (int i = 0; i < Info.Equipment.Length; i++)
            {
                UserItem temp = Info.Equipment[i];
                if (temp == null) continue;
                ItemInfo realItem = Functions.GetRealItem(temp.Info, Info.Level, Info.Class, Envir.ItemInfoList);

                if (realItem.Type == ItemType.Weapon)
                    CurrentHandWeight = (int)Math.Min(int.MaxValue, CurrentHandWeight + temp.Weight);
                else
                    CurrentWearWeight = (int)Math.Min(int.MaxValue, CurrentWearWeight + temp.Weight);

                if (temp.CurrentDura == 0 && temp.Info.Durability > 0) continue;

                if (realItem.Type == ItemType.Armour)
                {
                    Looks_Armour = realItem.Shape;
                    Looks_Wings = realItem.Effect;
                }

                if (realItem.Type == ItemType.Weapon)
                {
                    Looks_Weapon = realItem.Shape;
                    Looks_WeaponEffect = realItem.Effect;
                }

                Stats.Add(realItem.Stats);
                Stats.Add(temp.AddedStats);

                if (realItem.Light > Light) Light = realItem.Light;
                if (realItem.Unique != SpecialItemMode.None)
                {
                    SpecialMode |= realItem.Unique;

                    if (realItem.Unique.HasFlag(SpecialItemMode.Flame)) skillsToAdd.Add(Settings.FireRing);
                    if (realItem.Unique.HasFlag(SpecialItemMode.Healing)) skillsToAdd.Add(Settings.HealRing);
                    if (realItem.Unique.HasFlag(SpecialItemMode.Blink)) skillsToAdd.Add(Settings.BlinkSkill);
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

            AddTempSkills(skillsToAdd);
            RemoveTempSkills(skillsToRemove.Except(skillsToAdd));

            if (SpecialMode.HasFlag(SpecialItemMode.Muscle))
            {
                Stats[Stat.BagWeight] = Stats[Stat.BagWeight] * 2;
                Stats[Stat.WearWeight] = Stats[Stat.WearWeight] * 2;
                Stats[Stat.HandWeight] = Stats[Stat.HandWeight] * 2;
            }

            if ((OldLooks_Armour != Looks_Armour) || (OldLooks_Weapon != Looks_Weapon) || (OldLooks_Wings != Looks_Wings) || (OldLight != Light))
            {
                UpdateLooks(OldLooks_Weapon);                
            }
        }
        private void RefreshItemSetStats()
        {
            foreach (var s in ItemSets)
            {
                if (!s.SetComplete) continue;
            }
        }
        public void RefreshStatCaps()
        {
            foreach (var cap in Settings.ClassBaseStats[(byte)Class].Caps.Values)
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
        #endregion

        private void AddTempSkills(IEnumerable<string> skillsToAdd)
        {
            foreach (var skill in skillsToAdd)
            {
                Spell spelltype;
                bool hasSkill = false;

                if (!Enum.TryParse(skill, out spelltype)) return;

                for (var i = Info.Magics.Count - 1; i >= 0; i--)
                    if (Info.Magics[i].Spell == spelltype) hasSkill = true;

                if (hasSkill) continue;

                var magic = new UserMagic(spelltype) { IsTempSpell = true };
                Info.Magics.Add(magic);
                SendMagicInfo(magic);                
            }
        }
        public virtual void SendMagicInfo(UserMagic magic)
        {
            Enqueue(magic.GetInfo());
        }
        private void RemoveTempSkills(IEnumerable<string> skillsToRemove)
        {
            foreach (var skill in skillsToRemove)
            {
                if (!Enum.TryParse(skill, out Spell spelltype)) return;

                for (var i = Info.Magics.Count - 1; i >= 0; i--)
                {
                    if (!Info.Magics[i].IsTempSpell || Info.Magics[i].Spell != spelltype) continue;

                    Info.Magics.RemoveAt(i);
                    Enqueue(new S.RemoveMagic { PlaceId = i });
                }
            }
        }
        private void RefreshSkills()
        {
            int[] spiritSwordLvPlus = { 0, 3, 5, 8 };
            int[] slayingLvPlus = { 5, 6, 7, 8 };
            for (int i = 0; i < Info.Magics.Count; i++)
            {
                UserMagic magic = Info.Magics[i];
                switch (magic.Spell)
                {
                    case Spell.Fencing:
                        Stats[Stat.Accuracy] += magic.Level * 3;
                        // Stats[Stat.MaxAC] += (magic.Level + 1) * 3;
                        break;
                    case Spell.Slaying:
                    case Spell.Slaying2:
                    case Spell.Slaying3:
                    case Spell.Slaying4:
                    case Spell.Slaying5:
                    case Spell.Slaying6:
                    case Spell.Slaying7:
                    case Spell.Slaying8:
                    case Spell.Slaying9:
                    case Spell.Slaying10:
                    case Spell.Slaying11:
                    case Spell.Slaying12:
                    case Spell.Slaying13:
                    case Spell.Slaying14:
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
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buff buff = Buffs[i];

                if (buff.Paused) continue;

                Stats.Add(buff.Stats);
            }
        }
        public void BroadcastColourChange()
        {
            if (CurrentMap == null) return;

            for (int i = CurrentMap.Players.Count - 1; i >= 0; i--)
            {
                PlayerObject player = CurrentMap.Players[i];
                if (player == this) continue;

                if (Functions.InRange(CurrentLocation, player.CurrentLocation, Globals.DataRange))
                    player.Enqueue(new S.ObjectColourChanged { ObjectID = ObjectID, NameColour = player.GetNameColour(this) });
            }
        }
        public virtual void GainExp(uint amount) { }
        public int ReduceExp(uint amount, uint targetLevel)
        {
            int expPoint;

            if (Level < targetLevel + 10 || !Settings.ExpMobLevelDifference)
            {
                expPoint = (int)amount;
            }
            else
            {
                expPoint = (int)amount - (int)Math.Round(Math.Max(amount / 15, 1) * ((double)Level - (targetLevel + 10)));
            }

            if (expPoint <= 0) expPoint = 1;

            return expPoint;
        }
        public override void BroadcastInfo()
        {
            Packet p;
            if (CurrentMap == null) return;

            for (int i = CurrentMap.Players.Count - 1; i >= 0; i--)
            {
                PlayerObject player = CurrentMap.Players[i];
                if (player == this) continue;

                if (Functions.InRange(CurrentLocation, player.CurrentLocation, Globals.DataRange))
                {
                    p = GetInfoEx(player);
                    if (p != null)
                        player.Enqueue(p);
                }
            }
        }
        public virtual bool CheckMovement(Point location)
        {
            return false;
        }
        public bool Walk(MirDirection dir)
        {
            if (!CanMove || !CanWalk)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return false;
            }

            Point location = Functions.PointMove(CurrentLocation, dir, 1);

            if (!CurrentMap.ValidPoint(location))
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return false;
            }

            Cell cell = CurrentMap.GetCell(location);
            if (cell.Objects != null)
            {
                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    MapObject ob = cell.Objects[i];

                    if (ob.Race == ObjectType.Merchant && Race == ObjectType.Player)
                    {
                        NPCObject NPC = (NPCObject)ob;
                        if (!NPC.Visible || !NPC.VisibleLog[Info.Index]) continue;
                    }
                    else
                        if (!ob.Blocking || (CheckCellTime && ob.CellTime >= Envir.Time)) continue;

                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return false;
                }
            }

            if (Hidden)
            {
                RemoveBuff(BuffType.Hiding);
            }

            Direction = dir;
            if (CheckMovement(location)) return false;

            CurrentMap.GetCell(CurrentLocation).Remove(this);
            RemoveObjects(dir, 1);

            CurrentLocation = location;
            CurrentMap.GetCell(CurrentLocation).Add(this);
            AddObjects(dir, 1);

            SafeZoneInfo szi = CurrentMap.GetSafeZone(CurrentLocation);

            if (szi != null)
            {
                SetBindSafeZone(szi);
                InSafeZone = true;
            }
            else
                InSafeZone = false;

            Moved();

            CellTime = Envir.Time + 500;
            ActionTime = Envir.Time + GetDelayTime(MoveDelay);          
            
            Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
            Broadcast(new S.ObjectWalk { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            GetPlayerLocation();

            cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                if (cell.Objects[i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)cell.Objects[i];

                ob.ProcessSpell(this);
                //break;
            }

            return true;
        }

        protected virtual void Moved()
        {
        }
        public override int Pushed(MapObject pusher, MirDirection dir, int distance)
        {
            int result = 0;
            MirDirection reverse = Functions.ReverseDirection(dir);
            Cell cell;
            for (int i = 0; i < distance; i++)
            {
                Point location = Functions.PointMove(CurrentLocation, dir, 1);

                if (!CurrentMap.ValidPoint(location)) return result;

                cell = CurrentMap.GetCell(location);

                bool stop = false;
                if (cell.Objects != null)
                    for (int c = 0; c < cell.Objects.Count; c++)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;
                        stop = true;
                    }
                if (stop) break;

                CurrentMap.GetCell(CurrentLocation).Remove(this);

                Direction = reverse;
                RemoveObjects(dir, 1);
                CurrentLocation = location;
                CurrentMap.GetCell(CurrentLocation).Add(this);
                AddObjects(dir, 1);

                Moved();

                Enqueue(new S.Pushed { Direction = Direction, Location = CurrentLocation });
                Broadcast(new S.ObjectPushed { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                GetPlayerLocation();
                result++;
            }

            if (result > 0)
            {
                cell = CurrentMap.GetCell(CurrentLocation);

                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;
                    SpellObject ob = (SpellObject)cell.Objects[i];

                    ob.ProcessSpell(this);
                    //break;
                }

                SafeZoneInfo szi = CurrentMap.GetSafeZone(CurrentLocation);

                if (szi != null)
                {
                    SetBindSafeZone(szi);
                    InSafeZone = true;
                }
                else
                    InSafeZone = false;
            }

            ActionTime = Envir.Time + 500;
            return result;
        }

        public void GetPlayerLocation()
        {
            if (GroupMembers == null) return;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                PlayerObject member = GroupMembers[i];
                
                if (member.CurrentMap.Info.BigMap <= 0) continue;
                  
                member.Enqueue(new S.SendMemberLocation { MemberName = Name, MemberLocation = CurrentLocation });
                Enqueue(new S.SendMemberLocation { MemberName = member.Name, MemberLocation = member.CurrentLocation });
            }
            Enqueue(new S.SendMemberLocation { MemberName = Name, MemberLocation = CurrentLocation });
        }



        public void RangeAttack(MirDirection dir, Point location, uint targetID)
        {
            LogTime = Envir.Time + Globals.LogDelay;

            if (Info.Equipment[(int)EquipmentSlot.Weapon] == null) return;
            ItemInfo RealItem = Functions.GetRealItem(Info.Equipment[(int)EquipmentSlot.Weapon].Info, Info.Level, Info.Class, Envir.ItemInfoList);

            if ((RealItem.Shape / Globals.ClassWeaponCount) != 2) return;
            if (Functions.InRange(CurrentLocation, location, Globals.MaxAttackRange) == false) return;

            MapObject target = null;

            if (targetID == ObjectID)
                target = this;
            else if (targetID > 0)
                target = FindObject(targetID, 10);

            if (target != null && target.Dead) return;

            if (target != null && target.Race != ObjectType.Monster && target.Race != ObjectType.Player) return;

            if (target != null && !target.Dead && target.IsAttackTarget(this) && !target.IsFriendlyTarget(this))
            {
                if (this is PlayerObject player &&
                   player.PMode == PetMode.FocusMasterTarget)
                {
                    foreach (MonsterObject pet in player.Pets)
                        pet.Target = target;
                }
            }

            Direction = dir;

            Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

            UserMagic magic;
            Spell spell = Spell.None;
            bool focus = false;

            if (target != null && !CanFly(target.CurrentLocation) && (Info.MentalState != 1))
            {
                target = null;
                targetID = 0;
            }

            if (target != null)
            {
                int distance = Functions.MaxDistance(CurrentLocation, target.CurrentLocation);

                int damage = GetRangeAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC], distance);

                int chanceToHit = (100 + Settings.RangeAccuracyBonus - ((100 / Globals.MaxAttackRange) * distance)) * (focus ? 2 : 1);

                if (chanceToHit < 0) chanceToHit = 0;

                int delay = Functions.MaxDistance(CurrentLocation, target.CurrentLocation) * 50 + 500 + 50; //50 MS per Step

                if (Envir.Random.Next(100) < chanceToHit)
                {
                    if (target.CurrentLocation != location)
                        location = target.CurrentLocation;

                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, target, damage, DefenceType.ACAgility, true);
                    ActionList.Add(action);
                }
                else
                {
                    DelayedAction action = new DelayedAction(DelayedType.DamageIndicator, Envir.Time + delay, target, DamageType.Miss);
                    ActionList.Add(action);
                }
            }
            else
                targetID = 0;

            Enqueue(new S.RangeAttack { TargetID = targetID, Target = location, Spell = spell });
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = targetID, Target = location, Spell = spell });

            AttackTime = Envir.Time + AttackSpeed;
            ActionTime = Envir.Time + 550;
            RegenTime = Envir.Time + RegenDelay;
        }
        public void Attack(MirDirection dir, Spell spell)
        {
            LogTime = Envir.Time + Globals.LogDelay;

            if (!CanAttack)
            {
                switch (spell)
                {
                    case Spell.Slaying:
                        Slaying = false;
                        break;
                    case Spell.Slaying2:
                        Slaying2 = false;
                        break;
                    case Spell.Slaying3:
                        Slaying3 = false;
                        break;
                    case Spell.Slaying4:
                        Slaying4 = false;
                        break;
                    case Spell.Slaying5:
                        Slaying5 = false;
                        break;
                    case Spell.Slaying6:
                        Slaying6 = false;
                        break;
                    case Spell.Slaying7:
                        Slaying7 = false;
                        break;
                    case Spell.Slaying8:
                        Slaying8 = false;
                        break;
                    case Spell.Slaying9:
                        Slaying9 = false;
                        break;
                    case Spell.Slaying10:
                        Slaying10 = false;
                        break;
                    case Spell.Slaying11:
                        Slaying11 = false;
                        break;
                    case Spell.Slaying12:
                        Slaying12 = false;
                        break;
                    case Spell.Slaying13:
                        Slaying13 = false;
                        break;
                    case Spell.Slaying14:
                        Slaying14 = false;
                        break;
                }

                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            byte level = 0;
            UserMagic magic;

            switch (spell)
            {
                case Spell.Slaying:
                    if (!Slaying)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying);
                        level = magic.Level;
                    }

                    Slaying = false;
                    break;
                case Spell.Slaying2:
                    if (!Slaying2)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying2);
                        level = magic.Level;
                    }

                    Slaying2 = false;
                    break;
                case Spell.Slaying3:
                    if (!Slaying3)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying3);
                        level = magic.Level;
                    }

                    Slaying3 = false;
                    break;
                case Spell.Slaying4:
                    if (!Slaying4)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying4);
                        level = magic.Level;
                    }

                    Slaying4 = false;
                    break;
                case Spell.Slaying5:
                    if (!Slaying5)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying5);
                        level = magic.Level;
                    }

                    Slaying5 = false;
                    break;
                case Spell.Slaying6:
                    if (!Slaying6)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying6);
                        level = magic.Level;
                    }

                    Slaying6 = false;
                    break;
                case Spell.Slaying7:
                    if (!Slaying7)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying7);
                        level = magic.Level;
                    }

                    Slaying7 = false;
                    break;
                case Spell.Slaying8:
                    if (!Slaying8)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying8);
                        level = magic.Level;
                    }

                    Slaying8 = false;
                    break;
                case Spell.Slaying9:
                    if (!Slaying9)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying9);
                        level = magic.Level;
                    }

                    Slaying9 = false;
                    break;
                case Spell.Slaying10:
                    if (!Slaying10)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying10);
                        level = magic.Level;
                    }

                    Slaying10 = false;
                    break;
                case Spell.Slaying11:
                    if (!Slaying11)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying11);
                        level = magic.Level;
                    }

                    Slaying11 = false;
                    break;
                case Spell.Slaying12:
                    if (!Slaying12)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying12);
                        level = magic.Level;
                    }

                    Slaying12 = false;
                    break;
                case Spell.Slaying13:
                    if (!Slaying13)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying13);
                        level = magic.Level;
                    }

                    Slaying13 = false;
                    break;
                case Spell.Slaying14:
                    if (!Slaying14)
                        spell = Spell.None;
                    else
                    {
                        magic = GetMagic(Spell.Slaying14);
                        level = magic.Level;
                    }

                    Slaying14 = false;
                    break;
                case Spell.Thrusting:
                case Spell.FlamingSword:
                    magic = GetMagic(spell);
                    if ((magic == null) || (!FlamingSword && (spell == Spell.FlamingSword)))
                    {
                        spell = Spell.None;
                        break;
                    }
                    level = magic.Level;
                    break;
                case Spell.HalfMoon:
                    magic = GetMagic(spell);
                    if (magic == null || magic.Info.BaseCost + (magic.Level * magic.Info.LevelCost) > MP)
                    {
                        spell = Spell.None;
                        break;
                    }
                    level = magic.Level;
                    ChangeMP(-(magic.Info.BaseCost + magic.Level * magic.Info.LevelCost));
                    break;
                default:
                    spell = Spell.None;
                    break;
            }


            if (!Slaying)
            {
                magic = GetMagic(Spell.Slaying);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying, CanUse = Slaying });
                }
            }

            if (!Slaying2)
            {
                magic = GetMagic(Spell.Slaying2);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying2 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying2, CanUse = Slaying2 });
                }
            }

            if (!Slaying3)
            {
                magic = GetMagic(Spell.Slaying3);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying3 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying3, CanUse = Slaying3 });
                }
            }

            if (!Slaying4)
            {
                magic = GetMagic(Spell.Slaying4);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying4 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying4, CanUse = Slaying4 });
                }
            }

            if (!Slaying5)
            {
                magic = GetMagic(Spell.Slaying5);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying5 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying5, CanUse = Slaying5 });
                }
            }

            if (!Slaying6)
            {
                magic = GetMagic(Spell.Slaying6);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying6 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying6, CanUse = Slaying6 });
                }
            }

            if (!Slaying7)
            {
                magic = GetMagic(Spell.Slaying7);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying7 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying7, CanUse = Slaying7 });
                }
            }

            if (!Slaying8)
            {
                magic = GetMagic(Spell.Slaying8);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying8 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying8, CanUse = Slaying8 });
                }
            }

            if (!Slaying9)
            {
                magic = GetMagic(Spell.Slaying9);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying9 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying9, CanUse = Slaying9 });
                }
            }

            if (!Slaying10)
            {
                magic = GetMagic(Spell.Slaying10);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying10 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying10, CanUse = Slaying10 });
                }
            }

            if (!Slaying11)
            {
                magic = GetMagic(Spell.Slaying11);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying11 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying11, CanUse = Slaying11 });
                }
            }

            if (!Slaying12)
            {
                magic = GetMagic(Spell.Slaying12);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying12 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying12, CanUse = Slaying12 });
                }
            }

            if (!Slaying13)
            {
                magic = GetMagic(Spell.Slaying13);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying13 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying13, CanUse = Slaying13 });
                }
            }

            if (!Slaying14)
            {
                magic = GetMagic(Spell.Slaying14);

                if (magic != null && Envir.Random.Next(12) <= magic.Level)
                {
                    Slaying14 = true;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.Slaying14, CanUse = Slaying14 });
                }
            }

            Direction = dir;

            Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = spell, Level = level });

            AttackTime = Envir.Time + AttackSpeed;
            ActionTime = Envir.Time + 550;
            RegenTime = Envir.Time + RegenDelay;

            Point target = Functions.PointMove(CurrentLocation, dir, 1);

            //damabeBase = the original damage from your gear (+ bonus from moonlight and darkbody)
            int damageBase = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            //damageFinal = the damage you're gonna do with skills added
            int damageFinal;

            if (!CurrentMap.ValidPoint(target))
            {
                switch (spell)
                {
                    case Spell.Thrusting:
                        goto Thrusting;
                    case Spell.HalfMoon:
                        goto HalfMoon;
                }
                return;
            }

            Cell cell = CurrentMap.GetCell(target);

            if (cell.Objects == null)
            {
                switch (spell)
                {
                    case Spell.Thrusting:
                        goto Thrusting;
                    case Spell.HalfMoon:
                        goto HalfMoon;
                }
                return;
            }

            damageFinal = damageBase;//incase we're not using skills
            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObject ob = cell.Objects[i];
                if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) continue;
                if (!ob.IsAttackTarget(this)) continue;

                if (ob != null && !ob.Dead && ob.IsAttackTarget(this) && !ob.IsFriendlyTarget(this))
                {
                    if (this is PlayerObject player &&
                   player.PMode == PetMode.FocusMasterTarget)
                    {
                        foreach (MonsterObject pet in player.Pets)
                            pet.Target = ob;
                    }
                }

                //Only undead targets
                if (ob.Undead)
                {
                    damageBase = Math.Min(int.MaxValue, damageBase + Stats[Stat.Holy]);
                    damageFinal = damageBase;//incase we're not using skills
                }

                var defence = DefenceType.ACAgility;

                DelayedAction action;
                switch (spell)
                {
                    case Spell.Slaying:
                        magic = GetMagic(Spell.Slaying);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying2:
                        magic = GetMagic(Spell.Slaying2);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying3:
                        magic = GetMagic(Spell.Slaying3);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying4:
                        magic = GetMagic(Spell.Slaying4);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying5:
                        magic = GetMagic(Spell.Slaying5);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying6:
                        magic = GetMagic(Spell.Slaying6);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying7:
                        magic = GetMagic(Spell.Slaying7);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying8:
                        magic = GetMagic(Spell.Slaying8);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying9:
                        magic = GetMagic(Spell.Slaying9);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying10:
                        magic = GetMagic(Spell.Slaying10);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying11:
                        magic = GetMagic(Spell.Slaying11);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying12:
                        magic = GetMagic(Spell.Slaying12);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying13:
                        magic = GetMagic(Spell.Slaying13);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Slaying14:
                        magic = GetMagic(Spell.Slaying14);
                        damageFinal = magic.GetDamage(damageBase);
                        LevelMagic(magic);
                        break;
                    case Spell.Thrusting:
                        magic = GetMagic(Spell.Thrusting);
                        LevelMagic(magic);
                        break;
                    case Spell.HalfMoon:
                        magic = GetMagic(Spell.HalfMoon);
                        LevelMagic(magic);
                        break;
                    case Spell.FlamingSword:
                        magic = GetMagic(Spell.FlamingSword);
                        damageFinal = magic.GetDamage(damageBase);
                        FlamingSword = false;
                        defence = DefenceType.AC;
                        //action = new DelayedAction(DelayedType.Damage, Envir.Time + 400, ob, damage, DefenceType.Agility, true);
                        //ActionList.Add(action);
                        LevelMagic(magic);
                        break;
                }

                //if (ob.Attacked(this, damage, defence) <= 0) break;
                action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, ob, damageFinal, defence, true);
                ActionList.Add(action);
                break;
            }

        Thrusting:
            if (spell == Spell.Thrusting)
            {
                target = Functions.PointMove(target, dir, 1);

                if (!CurrentMap.ValidPoint(target)) return;

                cell = CurrentMap.GetCell(target);

                if (cell.Objects == null) return;

                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    MapObject ob = cell.Objects[i];
                    if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) continue;
                    if (!ob.IsAttackTarget(this)) continue;

                    magic = GetMagic(spell);
                    damageFinal = magic.GetDamage(damageBase);
                    ob.Attacked(this, damageFinal, DefenceType.Agility, false);
                    break;
                }


            }
        HalfMoon:
            if (spell == Spell.HalfMoon)
            {
                dir = Functions.PreviousDir(dir);

                magic = GetMagic(spell);
                damageFinal = magic.GetDamage(damageBase);
                for (int i = 0; i < 4; i++)
                {
                    target = Functions.PointMove(CurrentLocation, dir, 1);
                    dir = Functions.NextDir(dir);
                    if (target == Front) continue;

                    if (!CurrentMap.ValidPoint(target)) continue;

                    cell = CurrentMap.GetCell(target);

                    if (cell.Objects == null) continue;

                    for (int o = 0; o < cell.Objects.Count; o++)
                    {
                        MapObject ob = cell.Objects[o];
                        if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) continue;
                        if (!ob.IsAttackTarget(this)) continue;

                        ob.Attacked(this, damageFinal, DefenceType.Agility, false);
                        break;
                    }
                }
            }
        }
        public virtual bool TryMagic()
        {
            return !Dead && Envir.Time >= ActionTime || Envir.Time >= SpellTime;
        }
        public virtual void BeginMagic(Spell spell, MirDirection dir, uint targetID, Point location, Boolean spellTargetLock = false)
        {
            Magic(spell, dir, targetID, location, spellTargetLock);
        }

        public int MagicCost(UserMagic magic)
        {
            int cost = magic.Info.BaseCost + magic.Info.LevelCost * magic.Level;
            Spell spell = magic.Spell;
            if (spell == Spell.Teleport)
            {
                if (Stats[Stat.TeleportManaPenaltyPercent] > 0)
                {
                    cost += (cost * Stats[Stat.TeleportManaPenaltyPercent]) / 100;
                }
            }

            if (Stats[Stat.ManaPenaltyPercent] > 0)
            {
                cost += (cost * Stats[Stat.ManaPenaltyPercent]) / 100;
            }

            return cost;
        }
        public virtual MapObject DefaultMagicTarget => this;
        public void Magic(Spell spell, MirDirection dir, uint targetID, Point location, bool spellTargetLock = false)
        {
            if (!CanCast)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            UserMagic magic = GetMagic(spell);

            if (magic == null)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if ((location.X != 0) && (location.Y != 0) && magic.Info.Range != 0 && Functions.InRange(CurrentLocation, location, magic.Info.Range) == false) return;

            AttackTime = Envir.Time + MoveDelay;
            SpellTime = Envir.Time + 1800; //Spell Delay

            if (spell != Spell.ShoulderDash)
            {
                ActionTime = Envir.Time + MoveDelay;
            }

            LogTime = Envir.Time + Globals.LogDelay;

            long delay = magic.GetDelay();

            if (magic != null && Envir.Time < (magic.CastTime + delay))
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            int cost = MagicCost(magic);

            if (cost > MP)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            RegenTime = Envir.Time + RegenDelay;
            ChangeMP(-cost);

            Direction = dir;
            if (spell != Spell.ShoulderDash)
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

            MapObject target = null;

            if (targetID == ObjectID)
            {
                target = this;
            }
            else if (targetID > 0)
            {
                target = FindObject(targetID, 10);
            }

            if (target != null && target.Race != ObjectType.Monster && target.Race != ObjectType.Player)
            {
                target = null;
            }

            if (target != null && !target.Dead && target.IsAttackTarget(this) && !target.IsFriendlyTarget(this))
            {
                if (this is PlayerObject player && player.PMode == PetMode.FocusMasterTarget)
                {
                    foreach (MonsterObject pet in player.Pets)
                        pet.Target = target;
                }
            }

            bool cast = true;
            byte level = magic.Level;
            switch (spell)
            {
                case Spell.FireBall:
                case Spell.FireBall2:
                case Spell.WindBall:
                case Spell.ThunderBall:
                case Spell.ThunderBall2:
                case Spell.ThunderBall3:
                case Spell.IceBall:
                case Spell.WaterBall:
                case Spell.IceFireBall:
                case Spell.FireBall3:
                case Spell.FireBall4:
                case Spell.FireBall5:
                case Spell.FireBall6:
                case Spell.FireBall7:
                case Spell.FireBall8:
                case Spell.FireBall9:
                    if (!Fireball(target, magic)) targetID = 0;
                    break;
                case Spell.Healing:
                case Spell.Healing2:
                case Spell.Healing3:
                    if (target == null)
                    {
                        target = DefaultMagicTarget;
                        targetID = ObjectID;
                    }
                    Healing(target, magic);
                    break;
                case Spell.Repulsion:
                case Spell.EnergyRepulsor:
                    Repulsion(magic);
                    break;
                case Spell.ElectricShock:
                    ActionList.Add(new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, target as MonsterObject));
                    break;
                case Spell.Poisoning:
                    if (!Poisoning(target, magic)) cast = false;
                    break;
                case Spell.HellFire:
                    HellFire(magic);
                    break;
                case Spell.ThunderBolt:
                case Spell.Rock:
                case Spell.ThunderBolt2:
                case Spell.ThunderBolt3:
                case Spell.IceRock:
                    ThunderBolt(target, magic);
                    break;
                case Spell.SoulFireBall:
                    if (!SoulFireball(target, magic, out cast)) targetID = 0;
                    break;
                case Spell.SummonSkeleton:
                    SummonSkeleton(magic);
                    break;
                case Spell.Teleport:
                    ActionList.Add(new DelayedAction(DelayedType.Magic, Envir.Time + 200, magic, location));
                    break;
                case Spell.Hiding:
                    Hiding(magic);
                    break;
                case Spell.FireBang:
                    FireBang(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location);
                    break;
                case Spell.MassHiding:
                    MassHiding(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location, out cast);
                    break;
                case Spell.SoulShield:
                case Spell.BlessedArmour:
                    SoulShield(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location, out cast);
                    break;
                case Spell.FireWall:
                    FireWall(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location);
                    break;
                case Spell.Lightning:
                    Lightning(magic);
                    break;
                case Spell.MassHealing:
                    MassHealing(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location);
                    break;
                case Spell.ShoulderDash:
                    ShoulderDash(magic);
                    return;
                case Spell.ThunderStorm:
                    ThunderStorm(magic);
                    break;
                case Spell.MagicShield:
                    ActionList.Add(new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, magic.GetPower(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]) + 15)));
                    break;
                case Spell.Purification:
                    if (target == null)
                    {
                        target = DefaultMagicTarget;
                        targetID = ObjectID;
                    }
                    Purification(target, magic);
                    break;
                case Spell.Revelation:
                    Revelation(target, magic);
                    break;
                case Spell.TrapHexagon:
                    TrapHexagon(magic, spellTargetLock ? (target != null ? target.CurrentLocation : location) : location, out cast);
                    break;
                case Spell.UltimateEnhancer:
                    UltimateEnhancer(target, magic, out cast);
                    break;
                default:
                    cast = false;
                    break;
            }

            if (cast)
            {
                magic.CastTime = Envir.Time;
            }

            Enqueue(new S.Magic { Spell = spell, TargetID = targetID, Target = location, Cast = cast, Level = level });
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = spell, TargetID = targetID, Target = location, Cast = cast, Level = level });
        }

        #region Wizard Skills
        private bool Fireball(MapObject target, UserMagic magic)
        {
            if (target == null || !target.IsAttackTarget(this) || !CanFly(target.CurrentLocation)) return false;

            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            int delay = Functions.MaxDistance(CurrentLocation, target.CurrentLocation) * 50 + 500; //50 MS per Step

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + delay, magic, damage, target, target.CurrentLocation);

            ActionList.Add(action);

            return true;
        }
        private void Repulsion(UserMagic magic)
        {
            bool result = false;
            for (int d = 0; d <= 1; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (!cell.Valid || cell.Objects == null) continue;

                        for (int i = 0; cell.Objects != null && i < cell.Objects.Count; i++)
                        {
                            MapObject ob = cell.Objects[i];
                            if (ob.Race != ObjectType.Monster && ob.Race != ObjectType.Player) continue;

                            if (!ob.IsAttackTarget(this) || ob.Level >= Level) continue;

                            if (Envir.Random.Next(20) >= 6 + magic.Level * 3 + Level - ob.Level) continue;

                            int distance = 1 + Math.Max(0, magic.Level - 1) + Envir.Random.Next(2);
                            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);

                            if (ob.Pushed(this, dir, distance) == 0) continue;

                            if (ob.Race == ObjectType.Player)
                            {
                                SafeZoneInfo szi = CurrentMap.GetSafeZone(ob.CurrentLocation);

                                if (szi != null)
                                {
                                    ((PlayerObject)ob).BindLocation = szi.Location;
                                    ((PlayerObject)ob).BindMapIndex = CurrentMapIndex;
                                    ob.InSafeZone = true;
                                }
                                else
                                    ob.InSafeZone = false;

                                ob.Attacked(this, magic.GetDamage(0), DefenceType.None, false);
                            }
                            result = true;
                        }
                    }
                }
            }

            if (result) LevelMagic(magic);
        }
        private void ElectricShock(MonsterObject target, UserMagic magic)
        {
            if (target == null || !target.IsAttackTarget(this)) return;

            if (Envir.Random.Next(4 - magic.Level) > 0)
            {
                if (Envir.Random.Next(2) == 0) LevelMagic(magic);
                return;
            }

            LevelMagic(magic);

            if (target.Master == this)
            {
                target.ShockTime = Envir.Time + (magic.Level * 5 + 10) * 1000;
                target.Target = null;
                return;
            }

            if (Envir.Random.Next(2) > 0)
            {
                target.ShockTime = Envir.Time + (magic.Level * 5 + 10) * 1000;
                target.Target = null;
                return;
            }

            if (target.Level > Level + 2 || !target.Info.CanTame) return;

            if (Envir.Random.Next(Level + 20 + magic.Level * 5) <= target.Level + 10)
            {
                if (Envir.Random.Next(5) > 0 && target.Master == null)
                {
                    target.RageTime = Envir.Time + (Envir.Random.Next(20) + 10) * 1000;
                    target.Target = null;
                }
                return;
            }

            var petBonus = Globals.MaxPets - 3;

            if (Pets.Count(t => !t.Dead) >= magic.Level + petBonus) return;

            int rate = (int)(target.Stats[Stat.HP] / 100);
            if (rate <= 2) rate = 2;
            else rate *= 2;

            if (Envir.Random.Next(rate) != 0) return;
            //else if (Envir.Random.Next(20) == 0) target.Die();

            if (target.Master != null)
            {
                target.SetHP(target.Stats[Stat.HP] / 10);
                target.Master.Pets.Remove(target);
            }
            else if (target.Respawn != null)
            {
                target.Respawn.Count--;
                Envir.MonsterCount--;
                CurrentMap.MonsterCount--;
                target.Respawn = null;
            }

            target.Master = this;
            //target.HealthChanged = true;
            target.BroadcastHealthChange();
            Pets.Add(target);
            target.Target = null;
            target.RageTime = 0;
            target.ShockTime = 0;
            target.OperateTime = 0;
            target.MaxPetLevel = (byte)(1 + magic.Level * 2);

            if (!Settings.PetSave)
            {
                target.TameTime = Envir.Time + (Settings.Minute * 60);
            }

            target.Broadcast(new S.ObjectName { ObjectID = target.ObjectID, Name = target.Name });
        }
        private void HellFire(UserMagic magic)
        {
            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, CurrentLocation, Direction, 4);
            CurrentMap.ActionList.Add(action);

            if (magic.Level != 3) return;

            MirDirection dir = (MirDirection)(((int)Direction + 1) % 8);
            action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, CurrentLocation, dir, 4);
            CurrentMap.ActionList.Add(action);

            dir = (MirDirection)(((int)Direction - 1 + 8) % 8);
            action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, CurrentLocation, dir, 4);
            CurrentMap.ActionList.Add(action);
        }
        private void ThunderBolt(MapObject target, UserMagic magic)
        {
            if (target == null || !target.IsAttackTarget(this)) return;

            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            if (target.Undead) damage = (int)(damage * 1.5F);

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, damage, target, target.CurrentLocation);

            ActionList.Add(action);
        }
        private void FireBang(UserMagic magic, Point location)
        {
            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, location);
            CurrentMap.ActionList.Add(action);
        }
        private void FireWall(UserMagic magic, Point location)
        {
            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, location);
            CurrentMap.ActionList.Add(action);
        }
        private void Lightning(UserMagic magic)
        {
            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, CurrentLocation, Direction);
            CurrentMap.ActionList.Add(action);
        }
        private void ThunderStorm(UserMagic magic)
        {
            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, damage, CurrentLocation);
            CurrentMap.ActionList.Add(action);
        }

        #endregion

        #region Taoist Skills
        private void Healing(MapObject target, UserMagic magic)
        {
            if (target == null || !target.IsFriendlyTarget(this)) return;

            int health = magic.GetDamage(GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) * 2) + Level;

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, health, target);

            ActionList.Add(action);
        }
        private bool Poisoning(MapObject target, UserMagic magic)
        {
            if (target == null || !target.IsAttackTarget(this)) return false;

            UserItem item = GetPoison(1);
            if (item == null) return false;

            int power = magic.GetDamage(GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, power, target, item);
            ActionList.Add(action);
            ConsumeItem(item, 1);
            return true;
        }
        private bool SoulFireball(MapObject target, UserMagic magic, out bool cast)
        {
            cast = false;
            UserItem item = null;
            if (item == null) return false;
            cast = true;

            if (target == null || !target.IsAttackTarget(this) || !CanFly(target.CurrentLocation)) return false;

            int damage = magic.GetDamage(GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]));

            int delay = Functions.MaxDistance(CurrentLocation, target.CurrentLocation) * 50 + 500; //50 MS per Step

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + delay, magic, damage, target, target.CurrentLocation);

            ActionList.Add(action);
            ConsumeItem(item, 1);

            return true;
        }
        private void SummonSkeleton(UserMagic magic)
        {
            MonsterObject monster;
            for (int i = 0; i < Pets.Count; i++)
            {
                monster = Pets[i];
                if ((monster.Info.Name != Settings.SkeletonName) || monster.Dead) continue;
                if (monster.Node == null) continue;
                monster.ActionList.Add(new DelayedAction(DelayedType.Recall, Envir.Time + 500));
                return;
            }

            if (Pets.Count(x => x.Race == ObjectType.Monster) >= 2) return;

            UserItem item = null;
            if (item == null) return;

            MonsterInfo info = Envir.GetMonsterInfo(Settings.SkeletonName);
            if (info == null) return;

            LevelMagic(magic);
            ConsumeItem(item, 1);

            monster = MonsterObject.GetMonster(info);
            monster.PetLevel = magic.Level;
            monster.Master = this;
            monster.MaxPetLevel = (byte)(4 + magic.Level);
            monster.ActionTime = Envir.Time + 1000;
            monster.RefreshNameColour(false);

            //Pets.Add(monster);

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, monster, Front);
            CurrentMap.ActionList.Add(action);
        }
        private void Purification(MapObject target, UserMagic magic)
        {
            if (target == null || !target.IsFriendlyTarget(this)) return;

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, target);

            ActionList.Add(action);
        }
        private void Hiding(UserMagic magic)
        {
            UserItem item = null;
            if (item == null) return;

            ConsumeItem(item, 1);

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) + (magic.Level + 1) * 5);
            ActionList.Add(action);

        }
        private void MassHiding(UserMagic magic, Point location, out bool cast)
        {
            cast = false;
            UserItem item = null;
            if (item == null) return;
            cast = true;

            int delay = Functions.MaxDistance(CurrentLocation, location) * 50 + 500; //50 MS per Step

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + delay, this, magic, GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) / 2 + (magic.Level + 1) * 2, location);
            CurrentMap.ActionList.Add(action);
        }
        private void SoulShield(UserMagic magic, Point location, out bool cast)
        {
            cast = false;
            UserItem item = null;
            if (item == null) return;
            cast = true;

            int delay = Functions.MaxDistance(CurrentLocation, location) * 50 + 500; //50 MS per Step

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + delay, this, magic, GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) * 4 + (magic.Level + 1) * 50, location);
            CurrentMap.ActionList.Add(action);

            ConsumeItem(item, 1);
        }
        private void MassHealing(UserMagic magic, Point location)
        {
            int value = magic.GetDamage(GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]));

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, value, location);
            CurrentMap.ActionList.Add(action);
        }
        private void Revelation(MapObject target, UserMagic magic)
        {
            if (target == null) return;

            int value = GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) + magic.GetPower();

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, magic, value, target);

            ActionList.Add(action);
        }

        private void TrapHexagon(UserMagic magic, Point location, out bool cast)
        {
            cast = false;
            bool anyTargetsFound = false;
            for (int x = location.X - 1; x <= location.X + 1; x++)
            {
                if (x < 0 || x >= CurrentMap.Width) continue;
                for (int y = location.Y - 1; y < location.Y + 1; y++)
                {
                    if (y < 0 || y >= CurrentMap.Height) continue;
                    if (!CurrentMap.ValidPoint(x, y)) continue;
                    var cell = CurrentMap.GetCell(x, y);
                    if (cell == null ||
                        cell.Objects == null ||
                        cell.Objects.Count <= 0) continue;
                    foreach (var target in cell.Objects)
                    {
                        switch (target.Race)
                        {
                            case ObjectType.Monster:
                                if (!target.IsAttackTarget(this)) continue;
                                if (target.Level > Level + 2) continue;
                                anyTargetsFound = true;
                                break;
                        }
                    }
                }
            }
            if (!anyTargetsFound)
                return;

            UserItem item = null;
            //Point location = target.CurrentLocation;

            if (item == null) return;

            LevelMagic(magic);
            uint duration = (uint)((magic.Level * 5 + 10) * 1000);
            int value = (int)duration;

            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 500, this, magic, value, location);
            CurrentMap.ActionList.Add(action);

            ConsumeItem(item, 1);
            cast = true;
        }
        private void UltimateEnhancer(MapObject target, UserMagic magic, out bool cast)
        {
            cast = false;

            if (target == null || target.Node == null || !target.IsFriendlyTarget(this)) return;
            UserItem item = null;
            if (item == null) return;

            int expiretime = GetAttackPower(Stats[Stat.MinSC], Stats[Stat.MaxSC]) * 4 + (magic.Level + 1) * 50;
            int value = Stats[Stat.MaxSC] >= 5 ? Math.Min(8, Stats[Stat.MaxSC] / 5) : 1;

            switch (target.Race)
            {
                case ObjectType.Monster:
                case ObjectType.Player:
                    //Only targets
                    if (target.IsFriendlyTarget(this))
                    {
                        var stats = new Stats();

                        if (target.Race == ObjectType.Monster || ((HumanObject)target).Class == MirClass.Warrior)
                        {
                            stats[Stat.MaxDC] = value;
                        }
                        else if (((HumanObject)target).Class == MirClass.Wizard)
                        {
                            stats[Stat.MaxMC] = value;
                        }
                        else if (((HumanObject)target).Class == MirClass.Taoist)
                        {
                            stats[Stat.MaxSC] = value;
                        }

                        target.AddBuff(BuffType.UltimateEnhancer, this, Settings.Second * expiretime, stats);
                        target.OperateTime = 0;
                        LevelMagic(magic);
                        ConsumeItem(item, 1);
                        cast = true;
                    }
                    break;
            }
        }

        #endregion

        #region Warrior Skills

        private void ShoulderDash(UserMagic magic)
        {
            if (!CanWalk)
            {
                return;
            }

            Point _nextLocation;
            MapObject _target = null;

            bool _blocking = false;
            bool _canDash = false;

            int _cellsTravelled = 0;
            int dist = Envir.Random.Next(2) + magic.Level + 2;

            ActionTime = Envir.Time + MoveDelay;

            for (int i = 0; i < dist; i++)
            {
                if (_blocking)
                {
                    break;
                }

                _nextLocation = Functions.PointMove(CurrentLocation, Direction, 1);

                if (!CurrentMap.ValidPoint(_nextLocation) || CurrentMap.GetSafeZone(_nextLocation) != null)
                {
                    break;
                }

                // acquire target
                if (i == 0)
                {
                    Cell targetCell = CurrentMap.GetCell(_nextLocation);

                    if (targetCell.Objects != null)
                    {
                        int cellCnt = targetCell.Objects.Count;

                        for (int j = 0; j < cellCnt; j++)
                        {
                            MapObject ob = targetCell.Objects[j];

                            if ((ob.Race == ObjectType.Player ||
                                ob.Race == ObjectType.Monster) &&
                                ob.IsAttackTarget(this) &&
                                ob.Level < Level)
                            {
                                _target = ob;
                                break;
                            }

                            if(ob.Blocking)
                            {
                                _blocking = true;
                                break;
                            }
                        }
                    }
                    
                    if (_blocking)
                    {
                        break;
                    }
                }

                // try to dash
                Cell dashCell = CurrentMap.GetCell(_nextLocation);
                _canDash = false;

                if (_target == null)
                {
                    if (dashCell.Objects != null)
                    {
                        int cellCnt = dashCell.Objects.Count;

                        for (int k = 0; k < cellCnt; k++)
                        {
                            MapObject ob = dashCell.Objects[k];

                            if (ob.Blocking)
                            {
                                _blocking = true;
                                break;
                            }
                        }

                        if(!_blocking)
                        {
                            _canDash = true;
                        }
                    }
                    else
                    {
                        _canDash = true;
                    }
                }
                else
                {
                    // try to push
                    if (_target.Pushed(this, Direction, 1) == 0)
                    {
                        _blocking = true;
                    }
                    else
                    {
                        _canDash = true;
                    }
                }

                if (_canDash)
                {
                    CurrentMap.GetCell(CurrentLocation).Remove(this);
                    RemoveObjects(Direction, 1);

                    Enqueue(new S.UserDash { Direction = Direction, Location = _nextLocation });
                    Broadcast(new S.ObjectDash { ObjectID = ObjectID, Direction = Direction, Location = _nextLocation });

                    CurrentMap.GetCell(_nextLocation).Add(this);
                    AddObjects(Direction, 1);

                    // dash interrupt
                    Cell cell = CurrentMap.GetCell(_nextLocation);
                    for (int l = 0; l < cell.Objects.Count; l++)
                    {
                        if (cell.Objects[l].Race == ObjectType.Spell)
                        {
                            SpellObject ob = (SpellObject)cell.Objects[l];

                            if (IsAttackTarget(ob.Caster))
                            {
                                switch(ob.Spell)
                                {
                                    case Spell.FireWall:
                                        Attacked((PlayerObject)ob.Caster, ob.Value, DefenceType.MAC, false);
                                        _blocking = true;
                                        break;
                                }
                            }
                        }
                    }

                    CurrentLocation = _nextLocation;
                    _cellsTravelled++;
                }
            }

            if (_cellsTravelled == 0)
            {
                Enqueue(new S.UserDashFail { Direction = Direction, Location = CurrentLocation });
                Broadcast(new S.ObjectDashFail { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                if (InSafeZone)
                {
                    ReceiveChat("No pushing in the safezone. tut tut.", ChatType.System);
                }
                else
                {
                    ReceiveChat("Not enough pushing Power.", ChatType.System);
                }
            }
            else
            {
                _target?.Attacked(this, magic.GetDamage(0), DefenceType.None, false);
                LevelMagic(magic);

                Broadcast(new S.ObjectDash { ObjectID = ObjectID, Direction = Direction, Location = Front });
            }

            long now = Envir.Time;

            magic.CastTime = now;
            Enqueue(new S.MagicCast { Spell = magic.Spell });

            CellTime = now + 500;
        }
        #endregion
        protected void CompleteMagic(IList<object> data)
        {
            UserMagic magic = (UserMagic)data[0];
            int value;
            MapObject target;
            Point targetLocation;
            Point location;
            MonsterObject monster;

            switch (magic.Spell)
            {
                #region FireBall, GreatFireBall, ThunderBolt, SoulFireBall, FlameDisruptor

                case Spell.FireBall:
                case Spell.FireBall2:
                case Spell.WindBall:
                case Spell.Rock:
                case Spell.ThunderBall:
                case Spell.ThunderBall2:
                case Spell.ThunderBall3:
                case Spell.ThunderBolt:
                case Spell.ThunderBolt2:
                case Spell.ThunderBolt3:
                case Spell.IceBall:
                case Spell.WaterBall:
                case Spell.IceFireBall:
                case Spell.IceRock:
                case Spell.FireBall3:
                case Spell.FireBall4:
                case Spell.FireBall5:
                case Spell.FireBall6:
                case Spell.FireBall7:
                case Spell.FireBall8:
                case Spell.FireBall9:
                    value = (int)data[1];
                    target = (MapObject)data[2];
                    targetLocation = (Point)data[3];

                    if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null || !Functions.InRange(target.CurrentLocation, targetLocation, 2)) return;
                    if (target.Attacked(this, value, DefenceType.MAC, false) > 0) LevelMagic(magic);
                    break;

                #endregion


                #region FrostCrunch
                case Spell.FrostCrunch:
                    value = (int)data[1];
                    target = (MapObject)data[2];
                    targetLocation = (Point)data[3];

                    if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null || !Functions.InRange(target.CurrentLocation, targetLocation, 2)) return;
                    if (target.Attacked(this, value, DefenceType.MAC, false) > 0)
                    {
                        if (Level + (target.Race == ObjectType.Player ? 2 : 10) >= target.Level && Envir.Random.Next(target.Race == ObjectType.Player ? 100 : 20) <= magic.Level)
                        {
                            target.ApplyPoison(new Poison
                            {
                                Owner = this,
                                Duration = target.Race == ObjectType.Player ? 4 : 5 + Envir.Random.Next(5),
                                PType = PoisonType.Slow,
                                TickSpeed = 1000,
                            }, this);
                            target.OperateTime = 0;
                        }

                        if (Level + (target.Race == ObjectType.Player ? 2 : 10) >= target.Level && Envir.Random.Next(target.Race == ObjectType.Player ? 100 : 40) <= magic.Level)
                        {
                            target.ApplyPoison(new Poison
                            {
                                Owner = this,
                                Duration = target.Race == ObjectType.Player ? 2 : 5 + Envir.Random.Next(Stats[Stat.Freezing]),
                                PType = PoisonType.Frozen,
                                TickSpeed = 1000,
                            }, this);
                            target.OperateTime = 0;
                        }

                        LevelMagic(magic);
                    }
                    break;

                #endregion

                #region Healing

                case Spell.Healing:
                case Spell.Healing2:
                case Spell.Healing3:
                    value = (int)data[1];
                    target = (MapObject)data[2];

                    if (target == null || !target.IsFriendlyTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;
                    if (target.Health >= target.MaxHealth) return;
                    target.HealAmount = (ushort)Math.Min(ushort.MaxValue, target.HealAmount + value);
                    target.OperateTime = 0;
                    LevelMagic(magic);
                    break;

                #endregion

                #region ElectricShock

                case Spell.ElectricShock:
                    monster = (MonsterObject)data[1];
                    if (monster == null || !monster.IsAttackTarget(this) || monster.CurrentMap != CurrentMap || monster.Node == null) return;
                    ElectricShock(monster, magic);
                    break;

                #endregion

                #region Poisoning

                case Spell.Poisoning:
                    value = (int)data[1];
                    target = (MapObject)data[2];
                    UserItem item = (UserItem)data[3];

                    if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

                    switch (item.Info.Shape)
                    {
                        case 1:
                            target.ApplyPoison(new Poison
                            {
                                Duration = (value * 2) + ((magic.Level + 1) * 7),
                                Owner = this,
                                PType = PoisonType.Green,
                                TickSpeed = 2000,
                                Value = value / 15 + magic.Level + 1 + Envir.Random.Next(Stats[Stat.PoisonAttack])
                            }, this);
                            break;
                        case 2:
                            target.ApplyPoison(new Poison
                            {
                                Duration = (value * 2) + (magic.Level + 1) * 7,
                                Owner = this,
                                PType = PoisonType.Red,
                                TickSpeed = 2000,
                            }, this);
                            break;
                    }
                    target.OperateTime = 0;

                    LevelMagic(magic);
                    break;

                #endregion

                #region Teleport
                case Spell.Teleport:                                 
                    if (CurrentMap.Info.NoTeleport)
                    {
                        ReceiveChat(("You cannot teleport on this map"), ChatType.System);
                        return;
                    }

                    if (!MagicTeleport(magic))
                        return;                    

                    LevelMagic(magic);

                    break;
                #endregion

                #region Hiding

                case Spell.Hiding:
                    {
                        value = (int)data[1];

                        AddBuff(BuffType.Hiding, this, Settings.Second * value, new Stats());

                        LevelMagic(magic);
                    }
                    break;

                #endregion

                #region MagicShield

                case Spell.MagicShield:
                    {
                        if (HasBuff(BuffType.MagicShield, out _)) return;

                        LevelMagic(magic);
                        AddBuff(BuffType.MagicShield, this, Settings.Second * (int)data[1], new Stats { [Stat.DamageReductionPercent] = (magic.Level + 2) * 10 });
                    }
                    break;

                #endregion

                #region Purification

                case Spell.Purification:
                    target = (MapObject)data[1];

                    if (target == null || !target.IsFriendlyTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;
                    if (Envir.Random.Next(4) > magic.Level) return;

                    for (int i = 0; i < target.Buffs.Count; i++)
                    {
                        var buff = target.Buffs[i];

                        if (!buff.Properties.HasFlag(BuffProperty.Debuff)) continue;

                        target.RemoveBuff(buff.Type);
                    }

                    target.PoisonList.Clear();
                    target.OperateTime = 0;

                    LevelMagic(magic);
                    break;

                #endregion

                #region Revelation

                case Spell.Revelation:
                    value = (int)data[1];
                    target = (MapObject)data[2];
                    if (target == null || target.CurrentMap != CurrentMap || target.Node == null) return;
                    if (target.Race != ObjectType.Player && target.Race != ObjectType.Monster) return;
                    if (Envir.Random.Next(4) > magic.Level || Envir.Time < target.RevTime) return;

                    target.RevTime = Envir.Time + value * 1000;
                    target.OperateTime = 0;
                    target.BroadcastHealthChange();

                    LevelMagic(magic);
                    break;

                #endregion

                #region Hallucination

                case Spell.Hallucination:
                    value = (int)data[1];
                    target = (MapObject)data[2];

                    if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null ||
                        Functions.MaxDistance(CurrentLocation, target.CurrentLocation) > 7 || Envir.Random.Next(Level + 20 + magic.Level * 5) <= target.Level + 10) return;
                    item = null;
                    if (item == null) return;

                    ((MonsterObject)target).HallucinationTime = Envir.Time + (Envir.Random.Next(20) + 10) * 1000;
                    target.Target = null;

                    ConsumeItem(item, 1);

                    LevelMagic(magic);
                    break;

                #endregion

            }
        }
        protected void CompleteAttack(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];
            bool damageWeapon = (bool)data[3];
            UserMagic userMagic = null;
            bool finalHit = false;
            if (data.Count >= 5)
                userMagic = (UserMagic)data[4];
            if (data.Count >= 6)
                finalHit = (bool)data[5];
            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            if (target.Attacked(this, damage, defence, damageWeapon) <= 0) return;

            //Level Fencing / SpiritSword
            foreach (UserMagic magic in Info.Magics)
            {
                switch (magic.Spell)
                {
                    case Spell.Fencing:
                    case Spell.SpiritSword:
                        LevelMagic(magic);
                        break;
                }
            }
        }
        protected void CompleteDamageIndicator(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            DamageType type = (DamageType)data[1];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.BroadcastDamageIndicator(type);
        }
        protected void CompleteSpellEffect(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            SpellEffect effect = (SpellEffect)data[1];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            S.ObjectEffect p = new S.ObjectEffect { ObjectID = target.ObjectID, Effect = effect };
            CurrentMap.Broadcast(p, target.CurrentLocation);
        }
        protected void CompletePoison(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            PoisonType pt = (PoisonType)data[1];
            SpellEffect sp = (SpellEffect)data[2];
            int duration = (int)data[3];
            int tickSpeed = (int)data[4];

            if (target == null) return;

            target.ApplyPoison(new Poison { PType = pt, Duration = duration, TickSpeed = tickSpeed }, this);
            target.Broadcast(new S.ObjectEffect { ObjectID = target.ObjectID, Effect = sp });
        }
        protected UserItem GetPoison(int count, byte shape = 0)
        {
            return null;
        }
        public UserMagic GetMagic(Spell spell)
        {
            for (int i = 0; i < Info.Magics.Count; i++)
            {
                UserMagic magic = Info.Magics[i];
                if (magic.Spell != spell) continue;
                return magic;
            }

            return null;
        }
        public void LevelMagic(UserMagic magic)
        {
            byte exp = (byte)(Envir.Random.Next(3) + 1);            

            exp *= (byte)Math.Min(byte.MaxValue, Stats[Stat.SkillGainMultiplier]);

            if (Level == ushort.MaxValue) exp = byte.MaxValue;

            int oldLevel = magic.Level;

            switch (magic.Level)
            {
                case 0:
                    if (Level < magic.Info.Level1)
                        return;

                    magic.Experience += exp;
                    if (magic.Experience >= magic.Info.Need1)
                    {
                        magic.Level++;
                        magic.Experience = (ushort)(magic.Experience - magic.Info.Need1);
                        RefreshStats();
                    }
                    break;
                case 1:
                    if (Level < magic.Info.Level2)
                        return;

                    magic.Experience += exp;
                    if (magic.Experience >= magic.Info.Need2)
                    {
                        magic.Level++;
                        magic.Experience = (ushort)(magic.Experience - magic.Info.Need2);
                        RefreshStats();
                    }
                    break;
                case 2:
                    if (Level < magic.Info.Level3)
                        return;

                    magic.Experience += exp;
                    if (magic.Experience >= magic.Info.Need3)
                    {
                        magic.Level++;
                        magic.Experience = 0;
                        RefreshStats();
                    }
                    break;
                default:
                    return;
            }

            if (oldLevel != magic.Level)
            {
                long delay = magic.GetDelay();
                Enqueue(new S.MagicDelay { ObjectID = ObjectID, Spell = magic.Spell, Delay = delay });
            }

            Enqueue(new S.MagicLeveled { ObjectID = ObjectID, Spell = magic.Spell, Level = magic.Level, Experience = magic.Experience });

        }
        public virtual bool MagicTeleport(UserMagic magic)
        {
            return false;
        }
        public override bool Teleport(Map temp, Point location, bool effects = true, byte effectnumber = 0)
        {
            Map oldMap = CurrentMap;
            Point oldLocation = CurrentLocation;

            bool mapChanged = temp != oldMap;

            if (!base.Teleport(temp, location, effects)) return false;

            Enqueue(new S.MapChanged
            {
                MapIndex = CurrentMap.Info.Index,
                FileName = CurrentMap.Info.FileName,
                Title = CurrentMap.Info.Title,
                MiniMap = CurrentMap.Info.MiniMap,
                BigMap = CurrentMap.Info.BigMap,
                Lights = CurrentMap.Info.Light,
                Location = CurrentLocation,
                Direction = Direction,
                MapDarkLight = CurrentMap.Info.MapDarkLight,
                Music = CurrentMap.Info.Music
            });

            if (effects) Enqueue(new S.ObjectTeleportIn { ObjectID = ObjectID, Type = effectnumber });

            if (CheckStacked())
            {
                StackingTime = Envir.Time + 1000;
                Stacking = true;
            }

            SafeZoneInfo szi = CurrentMap.GetSafeZone(CurrentLocation);

            if (szi != null)
            {
                SetBindSafeZone(szi);
                InSafeZone = true;
            }
            else
                InSafeZone = false;            

            return true;
        }
        protected virtual void SetBindSafeZone(SafeZoneInfo szi) { }
        public virtual bool AtWar(HumanObject attacker)
        {
            return false;
        }
        protected Packet GetUpdateInfo()
        {
            return new S.PlayerUpdate
            {
                ObjectID = ObjectID,
                Weapon = Looks_Weapon,
                WeaponEffect = Looks_WeaponEffect,
                Armour = Looks_Armour,
                Light = Light,
                WingEffect = Looks_Wings
            };
        }
        protected virtual void UpdateLooks(short OldLooks_Weapon)
        {
            Broadcast(GetUpdateInfo());
        }
        public Packet GetInfoEx(HumanObject player)
        {
            var p = (S.ObjectPlayer)GetInfo();

            if (p != null)
            {
                p.NameColour = player.GetNameColour(this);
            }

            return p;
        }
        public override bool IsAttackTarget(HumanObject attacker)
        {
            return true;
        }
        public override bool IsAttackTarget(MonsterObject attacker)
        {            
            return true;
        }

        public override bool IsFriendlyTarget(HumanObject ally)
        {
            return true;
        }
        public override bool IsFriendlyTarget(MonsterObject ally)
        {            
            return true;
        }
        public override void ReceiveChat(string text, ChatType type) { }
        public override Packet GetInfo() { return null; }
        public override int Attacked(HumanObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            var armour = GetArmour(type, attacker, out bool hit);

            if (!hit)
            {
                return 0;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            if (damageWeapon)
                attacker.DamageWeapon();

            damage += attacker.Stats[Stat.AttackBonus];

            //MagicShield, ElementalBarrier
            if (Stats[Stat.DamageReductionPercent] > 0)
            {
                damage -= (damage * Stats[Stat.DamageReductionPercent]) / 100;
            }

            if (armour >= damage)
            {
                BroadcastDamageIndicator(DamageType.Miss);
                return 0;
            }

            //EnergyShield
            if (Stats[Stat.EnergyShieldPercent] > 0)
            {
                if (Envir.Random.Next(100) < Stats[Stat.EnergyShieldPercent])
                {
                    if (HP + (Stats[Stat.EnergyShieldHPGain]) >= Stats[Stat.HP])
                        SetHP(Stats[Stat.HP]);
                    else
                        ChangeHP(Stats[Stat.EnergyShieldHPGain]);
                }
            }

            if (HasBuff(BuffType.MagicShield, out Buff magicShield))
            {
                var duration = (int)Math.Min(int.MaxValue, magicShield.ExpireTime - ((damage - armour) * 60));
                AddBuff(BuffType.MagicShield, this, duration, null);
            }

            if (attacker.Stats[Stat.HPDrainRatePercent] > 0 && damageWeapon)
            {
                attacker.HpDrain += Math.Max(0, ((float)(damage - armour) / 100) * attacker.Stats[Stat.HPDrainRatePercent]);
                if (attacker.HpDrain > 2)
                {
                    int HpGain = (int)Math.Floor(attacker.HpDrain);
                    attacker.ChangeHP(HpGain);
                    attacker.HpDrain -= HpGain;
                }
            }

            LastHitter = attacker;
            LastHitTime = Envir.Time + 10000;
            RegenTime = Envir.Time + RegenDelay;
            LogTime = Envir.Time + Globals.LogDelay;

            if (Envir.Time > BrownTime && PKPoints < 200 && !AtWar(attacker))
                attacker.BrownTime = Envir.Time + Settings.Minute;

            ushort LevelOffset = (byte)(Level > attacker.Level ? 0 : Math.Min(10, attacker.Level - Level));

            ApplyNegativeEffects(attacker, type, LevelOffset);

            DamageDura();
            Enqueue(new S.Struck { AttackerID = attacker.ObjectID });
            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            BroadcastDamageIndicator(DamageType.Hit, armour - damage);

            ChangeHP(armour - damage);
            return damage - armour;
        }
        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            var armour = GetArmour(type, attacker, out bool hit);

            if (!hit)
            {
                return 0;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            //MagicShield, ElementalBarrier
            if (Stats[Stat.DamageReductionPercent] != 0)
            {
                damage -= (damage * Stats[Stat.DamageReductionPercent]) / 100;
            }

            if (armour >= damage)
            {
                BroadcastDamageIndicator(DamageType.Miss);
                return 0;
            }

            if (Stats[Stat.EnergyShieldPercent] > 0)
            {
                if (Envir.Random.Next(100) < Stats[Stat.EnergyShieldPercent])
                {
                    if (HP + (Stats[Stat.EnergyShieldHPGain]) >= Stats[Stat.HP])
                        SetHP(Stats[Stat.HP]);
                    else
                        ChangeHP(Stats[Stat.EnergyShieldHPGain]);
                }
            }

            if (HasBuff(BuffType.MagicShield, out Buff magicShield))
            {
                var duration = (int)Math.Min(int.MaxValue, magicShield.ExpireTime - ((damage - armour) * 60));
                AddBuff(BuffType.MagicShield, this, duration, null);
            }

            LastHitter = attacker.Master ?? attacker;
            LastHitTime = Envir.Time + 10000;
            RegenTime = Envir.Time + RegenDelay;
            LogTime = Envir.Time + Globals.LogDelay;

            DamageDura();

            if (StruckTime < Envir.Time)
            {
                Enqueue(new S.Struck { AttackerID = attacker.ObjectID });
                Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });
                StruckTime = Envir.Time + 500;
            }

            BroadcastDamageIndicator(DamageType.Hit, armour - damage);

            ChangeHP(armour - damage);
            return damage - armour;
        }
        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.AC:
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.MACAgility:
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.MAC:
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.Agility:
                    break;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            //MagicShield, ElementalBarrier
            if (Stats[Stat.DamageReductionPercent] != 0)
            {
                damage -= (damage * Stats[Stat.DamageReductionPercent]) / 100;
            }

            if (armour >= damage) return 0;

            if (HasBuff(BuffType.MagicShield, out Buff magicShield))
            {
                var duration = (int)Math.Min(int.MaxValue, magicShield.ExpireTime - ((damage - armour) * 60));
                AddBuff(BuffType.MagicShield, this, duration, null);
            }

            RegenTime = Envir.Time + RegenDelay;
            LogTime = Envir.Time + Globals.LogDelay;

            DamageDura();
            Enqueue(new S.Struck { AttackerID = 0 });
            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = 0, Direction = Direction, Location = CurrentLocation });

            ChangeHP(armour - damage);
            return damage - armour;
        }

        public override void ApplyPoison(Poison p, MapObject Caster = null, bool NoResist = false, bool ignoreDefence = true)
        {
            if (Caster != null && !NoResist)
            {
                if (((Caster.Race != ObjectType.Player) || Settings.PvpCanResistPoison) && (Envir.Random.Next(Settings.PoisonResistWeight) < Stats[Stat.PoisonResist]))
                {
                    return;
                }
            }

            if (!ignoreDefence && (p.PType == PoisonType.Green))
            {
                int armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);

                if (p.Value < armour)
                    p.PType = PoisonType.None;
                else
                    p.Value -= armour;
            }

            if (p.Owner != null && p.Owner is PlayerObject player && Envir.Time > BrownTime && PKPoints < 200)
            {
                bool ownerBrowns = true;
                if (player.MyGuild != null && MyGuild != null && MyGuild.IsAtWar() && MyGuild.IsEnemy(player.MyGuild))
                    ownerBrowns = false;

                if (ownerBrowns && !player.WarZone)
                        p.Owner.BrownTime = Envir.Time + Settings.Minute;
            }

            if ((p.PType == PoisonType.Green) || (p.PType == PoisonType.Red)) p.Duration = Math.Max(0, p.Duration - Stats[Stat.PoisonRecovery]);
            if (p.Duration == 0) return;
            if (p.PType == PoisonType.None) return;

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].PType != p.PType) continue;
                if ((PoisonList[i].PType == PoisonType.Green) && (PoisonList[i].Value > p.Value)) return;//cant cast weak poison to cancel out strong poison
                if ((PoisonList[i].PType != PoisonType.Green) && ((PoisonList[i].Duration - PoisonList[i].Time) > p.Duration)) return;//cant cast 1 second poison to make a 1minute poison go away!
                if ((PoisonList[i].PType == PoisonType.Frozen) || (PoisonList[i].PType == PoisonType.Slow) || (PoisonList[i].PType == PoisonType.Paralysis)) return;//prevents mobs from being perma frozen/slowed

                ReceiveChat(GameLanguage.BeenPoisoned, ChatType.System2);
                PoisonList[i] = p;
                return;
            }

            switch (p.PType)
            {
                default:
                    {
                        ReceiveChat(GameLanguage.BeenPoisoned, ChatType.System2);
                    }
                    break;
            }

            PoisonList.Add(p);
        }

        public override Buff AddBuff(BuffType type, MapObject owner, int duration, Stats stats, bool refreshStats = true, bool updateOnly = false, params int[] values)
        {
            Buff b = base.AddBuff(type, owner, duration, stats, refreshStats, updateOnly, values);

            switch (b.Type)
            {
                case BuffType.MagicShield:
                    CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.MagicShieldUp }, CurrentLocation);
                    break;
            }

            var packet = new S.AddBuff { Buff = b.ToClientBuff() };

            Enqueue(packet);

            if (b.Info.Visible)
            {
                Broadcast(packet);
            }

            if (refreshStats)
            {
                RefreshStats();
            }

            return b;
        }

        public override void PauseBuff(Buff b)
        {
            if (b.Paused) return;

            base.PauseBuff(b);

            Enqueue(new S.PauseBuff { Type = b.Type, ObjectID = ObjectID, Paused = true });
        }

        public override void UnpauseBuff(Buff b)
        {
            if (!b.Paused) return;

            base.UnpauseBuff(b);

            Enqueue(new S.PauseBuff { Type = b.Type, ObjectID = ObjectID, Paused = false });
        }
        protected int GetCurrentStatCount(UserItem gem, UserItem item)
        {
            if (gem.GetTotal(Stat.MaxDC) > 0)
                return item.AddedStats[Stat.MaxDC];

            else if (gem.GetTotal(Stat.MaxMC) > 0)
                return item.AddedStats[Stat.MaxMC];

            else if (gem.GetTotal(Stat.MaxSC) > 0)
                return item.AddedStats[Stat.MaxSC];

            else if (gem.GetTotal(Stat.MaxAC) > 0)
                return item.AddedStats[Stat.MaxAC];

            else if (gem.GetTotal(Stat.MaxMAC) > 0)
                return item.AddedStats[Stat.MaxMAC];

            else if ((gem.Info.Durability) > 0)
                return item.Info.Durability > item.MaxDura ? 0 : ((item.MaxDura - item.Info.Durability) / 1000);

            else if (gem.GetTotal(Stat.AttackSpeed) > 0)
                return item.AddedStats[Stat.AttackSpeed];

            else if (gem.GetTotal(Stat.Agility) > 0)
                return item.AddedStats[Stat.Agility];

            else if (gem.GetTotal(Stat.Accuracy) > 0)
                return item.AddedStats[Stat.Accuracy];

            else if (gem.GetTotal(Stat.PoisonAttack) > 0)
                return item.AddedStats[Stat.PoisonAttack];

            else if (gem.GetTotal(Stat.Freezing) > 0)
                return item.AddedStats[Stat.Freezing];

            else if (gem.GetTotal(Stat.MagicResist) > 0)
                return item.AddedStats[Stat.MagicResist];

            else if (gem.GetTotal(Stat.PoisonResist) > 0)
                return item.AddedStats[Stat.PoisonResist];

            else if (gem.GetTotal(Stat.Luck) > 0)
                return item.AddedStats[Stat.Luck];

            else if (gem.GetTotal(Stat.PoisonRecovery) > 0)
                return item.AddedStats[Stat.PoisonRecovery];

            else if (gem.GetTotal(Stat.HP) > 0)
                return item.AddedStats[Stat.HP];

            else if (gem.GetTotal(Stat.MP) > 0)
                return item.AddedStats[Stat.MP];

            else if (gem.GetTotal(Stat.HealthRecovery) > 0)
                return item.AddedStats[Stat.HealthRecovery];

            // Definitions are missing for these.
            /*
            else if ((gem.Info.HPrate) > 0)
                return item.h

            else if ((gem.Info.MPrate) > 0)
                return 

            else if ((gem.Info.SpellRecovery) > 0)
                return 

            else if ((gem.Info.Holy) > 0)
                return 

            else if ((gem.Info.Strong + gem.Strong) > 0)
                return 

            else if (gem.Info.HPrate > 0)
                return
            */
            return 0;
        }
        public bool CanGainItem(UserItem item)
        {
            if (FreeSpace(Info.Inventory) > 0)
            {
                return true;
            }

            if (item.Info.StackSize > 1)
            {
                ushort count = item.Count;

                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    UserItem bagItem = Info.Inventory[i];

                    if (bagItem.Info != item.Info) continue;

                    if (bagItem.Count + count <= bagItem.Info.StackSize) return true;

                    count -= (ushort)(bagItem.Info.StackSize - bagItem.Count);
                }
            }

            return false;
        }
        public bool CanGainItems(UserItem[] items)
        {
            int itemCount = items.Count(e => e != null);
            ushort stackOffset = 0;

            if (itemCount < 1) return true;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;

                if (items[i].Info.StackSize > 1)
                {
                    ushort count = items[i].Count;

                    for (int u = 0; u < Info.Inventory.Length; u++)
                    {
                        UserItem bagItem = Info.Inventory[u];

                        if (bagItem == null || bagItem.Info != items[i].Info) continue;

                        if (bagItem.Count + count > bagItem.Info.StackSize) stackOffset++;

                        break;
                    }
                }
            }

            if (FreeSpace(Info.Inventory) < itemCount + stackOffset) return false;

            return true;
        }
        public bool CanEquipItem(UserItem item, int slot)
        {
            switch ((EquipmentSlot)slot)
            {
                case EquipmentSlot.Weapon:
                    if (item.Info.Type != ItemType.Weapon)
                        return false;
                    break;
                case EquipmentSlot.Armour:
                    if (item.Info.Type != ItemType.Armour)
                        return false;
                    break;
                case EquipmentSlot.Helmet:
                    if (item.Info.Type != ItemType.Helmet)
                        return false;
                    break;
                case EquipmentSlot.Necklace:
                    if (item.Info.Type != ItemType.Necklace)
                        return false;
                    break;
                case EquipmentSlot.BraceletL:
                    if (item.Info.Type != ItemType.Bracelet)
                        return false;
                    break;
                case EquipmentSlot.BraceletR:
                    if (item.Info.Type != ItemType.Bracelet)
                        return false;
                    break;
                case EquipmentSlot.RingL:
                case EquipmentSlot.RingR:
                    if (item.Info.Type != ItemType.Ring)
                        return false;
                    break;               
                default:
                    return false;
            }


            switch (Gender)
            {
                case MirGender.Male:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                        return false;
                    break;
                case MirGender.Female:
                    if (!item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                        return false;
                    break;
            }


            switch (Class)
            {
                case MirClass.Warrior:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                        return false;
                    break;
                case MirClass.Wizard:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                        return false;
                    break;
                case MirClass.Taoist:
                    if (!item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                        return false;
                    break;
            }

            switch (item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (Level < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxAC:
                    if (Stats[Stat.MaxAC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxMAC:
                    if (Stats[Stat.MaxMAC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxDC:
                    if (Stats[Stat.MaxDC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxMC:
                    if (Stats[Stat.MaxMC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxSC:
                    if (Stats[Stat.MaxSC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MaxLevel:
                    if (Level > item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MinAC:
                    if (Stats[Stat.MinAC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MinMAC:
                    if (Stats[Stat.MinMAC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MinDC:
                    if (Stats[Stat.MinDC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MinMC:
                    if (Stats[Stat.MinMC] < item.Info.RequiredAmount)
                        return false;
                    break;
                case RequiredType.MinSC:
                    if (Stats[Stat.MinSC] < item.Info.RequiredAmount)
                        return false;
                    break;
                default:
                    if (Functions.TryGetRequiredAttribute(item.Info.RequiredType, out Attribute attribute))
                    {
                        if (!AttributeValues.TryGetValue(attribute, out UserAttribute attributeValue) || attributeValue.Level < item.Info.RequiredAmount)
                            return false;
                    }
                    break;
            }

            if (item.Info.Type == ItemType.Weapon)
            {
                if (item.Weight - (Info.Equipment[slot] != null ? Info.Equipment[slot].Weight : 0) + CurrentHandWeight > Stats[Stat.HandWeight])
                    return false;
            }
            else
                if (item.Weight - (Info.Equipment[slot] != null ? Info.Equipment[slot].Weight : 0) + CurrentWearWeight > Stats[Stat.WearWeight])
                return false;

            return true;
        }
        public void GainItem(UserItem item)
        {
            //CheckItemInfo(item.Info);
            CheckItem(item);

            UserItem clonedItem = item.Clone();

            Enqueue(new S.GainedItem { Item = clonedItem }); //Cloned because we are probably going to change the amount.

            AddItem(item);
            RefreshBagWeight();
        }

        private void DamageDura()
        {
            if (!SpecialMode.HasFlag(SpecialItemMode.NoDuraLoss))
                for (int i = 0; i < Info.Equipment.Length; i++)
                    if (i != (int)EquipmentSlot.Weapon)
                        DamageItem(Info.Equipment[i], Envir.Random.Next(1) + 1);
        }
        public void DamageWeapon()
        {
            if (!SpecialMode.HasFlag(SpecialItemMode.NoDuraLoss))
                DamageItem(Info.Equipment[(int)EquipmentSlot.Weapon], Envir.Random.Next(4) + 1);
        }
        public void DamageItem(UserItem item, int amount, bool isChanged = false)
        {
            if (item == null || item.CurrentDura == 0) return;
            if ((item.WeddingRing == Info.Married) && (Info.Equipment[(int)EquipmentSlot.RingL].UniqueID == item.UniqueID)) return;
            if (item.GetTotal(Stat.Strong) > 0) amount = Math.Max(1, amount - item.GetTotal(Stat.Strong));
            item.CurrentDura = (ushort)Math.Max(ushort.MinValue, item.CurrentDura - amount);
            item.DuraChanged = true;

            if (item.CurrentDura > 0 && isChanged != true) return;
            Enqueue(new S.DuraChanged { UniqueID = item.UniqueID, CurrentDura = item.CurrentDura });

            item.DuraChanged = false;
            RefreshStats();
        }

        public void RemoveObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
            }
        }
        public void AddObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                ob.Add(this);
                            }
                        }
                    }
                    break;
            }
        }
        public override void Remove(HumanObject player)
        {
            if (player == this) return;

            base.Remove(player);
            Enqueue(new S.ObjectRemove { ObjectID = player.ObjectID });
        }
        public override void Add(HumanObject player)
        {
            if (player == this) return;

            //base.Add(player);
            Enqueue(player.GetInfoEx(this));
            player.Enqueue(GetInfoEx(player));

            player.SendHealth(this);
            SendHealth(player);
        }
        public override void Remove(MonsterObject monster)
        {
            Enqueue(new S.ObjectRemove { ObjectID = monster.ObjectID });
        }
        public override void Add(MonsterObject monster)
        {
            Enqueue(monster.GetInfo());

            monster.SendHealth(this);
        }
        public override void SendHealth(HumanObject player)
        {
            if (!player.IsMember(this) && Envir.Time > RevTime) return;
            byte time = Math.Min(byte.MaxValue, (byte)Math.Max(5, (RevTime - Envir.Time) / 1000));
            player.Enqueue(new S.ObjectHealth { ObjectID = ObjectID, Percent = PercentHealth, Expire = time });
        }
        protected virtual void CleanUp()
        {
            Connection.Player = null;
            Info.Player = null;
            Connection.CleanObservers();
            Connection = null;
            Info = null;
        }
        public virtual void Enqueue(Packet p)
        {
            if (Connection == null) return;
            Connection.Enqueue(p);

            //MessageQueue.EnqueueDebugging(((ServerPacketIds)p.Index).ToString());
        }
        public virtual void Enqueue(Packet p, MirConnection c)
        {            
            if (c == null)
            {
                Enqueue(p);
                return;
            }

            c.Enqueue(p);
        }

        public void SpellToggle(Spell spell, SpellToggleState state)
        {
            if (Dead) return;

            UserMagic magic;
            bool use = Convert.ToBoolean(state);

            magic = GetMagic(spell);
            if (magic == null) return;

            int cost;
            switch (spell)
            {
                case Spell.Thrusting:
                    Info.Thrusting = state == SpellToggleState.None ? !Info.Thrusting : use;
                    break;
                case Spell.HalfMoon:
                    Info.HalfMoon = state == SpellToggleState.None ? !Info.HalfMoon : use;
                    break;
                case Spell.FlamingSword:
                    if (FlamingSword || Envir.Time < FlamingSwordTime) return;
                    magic = GetMagic(spell);
                    if (magic == null) return;
                    cost = magic.Info.BaseCost + magic.Level * magic.Info.LevelCost;
                    if (cost >= MP) return;

                    FlamingSword = true;
                    FlamingSwordTime = Envir.Time + 10000;
                    Enqueue(new S.SpellToggle { ObjectID = ObjectID, Spell = Spell.FlamingSword, CanUse = true });
                    ChangeMP(-cost);
                    break;
            }
        }
    }
}
