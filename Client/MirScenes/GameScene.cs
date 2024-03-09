using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using SlimDX;
using SlimDX.Direct3D9;
using Font = System.Drawing.Font;
using S = ServerPackets;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;
using Client.MirScenes.Dialogs;
using Client.Utils;
using System;
using System.Drawing.Imaging;

namespace Client.MirScenes
{
    public sealed class GameScene : MirScene
    {
        public static GameScene Scene;
        public static bool Observing;
        public static bool AllowObserve;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static long MoveTime, AttackTime, LogTime;
        public static bool CanMove;

        public MapControl MapControl;
        public MainCharacterDialog MainCharacterDialog;
        public MainDialog MainDialog;
        public ChatDialog ChatDialog;
        public InventoryDialog InventoryDialog;
        public CharacterDialog CharacterDialog;
        public StorageDialog StorageDialog;
        public BeltDialog BeltDialog;
        public OptionDialog OptionDialog;
        public MenuDialog MenuDialog;
        public NPCDialog NPCDialog;
        public NPCGoodsDialog NPCGoodsDialog;
        public NPCGoodsDialog NPCSubGoodsDialog;
        public NPCDropDialog NPCDropDialog;
        public HelpDialog HelpDialog;
        public RefineDialog RefineDialog;

        public GroupDialog GroupDialog;
        public GuildDialog GuildDialog;

        public BigMapDialog BigMapDialog;
        public TrustMerchantDialog TrustMerchantDialog;
        public TradeDialog TradeDialog;
        public GuestTradeDialog GuestTradeDialog;

        public List<SkillBarDialog> SkillBarDialogs = new List<SkillBarDialog>();
        public ChatOptionDialog ChatOptionDialog;
        public ChatNoticeDialog ChatNoticeDialog;

        public QuestListDialog QuestListDialog;
        public QuestDetailDialog QuestDetailDialog;
        public QuestDiaryDialog QuestLogDialog;
        public QuestTrackingDialog QuestTrackingDialog;

        public RankingDialog RankingDialog;

        public MailListDialog MailListDialog;
        public MailComposeLetterDialog MailComposeLetterDialog;
        public MailComposeParcelDialog MailComposeParcelDialog;
        public MailReadLetterDialog MailReadLetterDialog;
        public MailReadParcelDialog MailReadParcelDialog;

        public FriendDialog FriendDialog;
        public MemoDialog MemoDialog;
        public RelationshipDialog RelationshipDialog;
        public GameShopDialog GameShopDialog;

        public ReportDialog ReportDialog;

        public BuffDialog BuffsDialog;

        public KeyboardLayoutDialog KeyboardLayoutDialog;
        public NoticeDialog NoticeDialog;

        public TimerDialog TimerControl;
        public CompassDialog CompassControl;
        public RollDialog RollControl;


        public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public static List<UserId> UserIdList = new List<UserId>();
        public static List<UserItem> ChatItemList = new List<UserItem>();
        public static List<ClientQuestInfo> QuestInfoList = new List<ClientQuestInfo>();
        public static List<GameShopItem> GameShopInfoList = new List<GameShopItem>();
        public static Dictionary<long, RankCharacterInfo> RankingList = new Dictionary<long, RankCharacterInfo>();

        public static UserItem[] Storage = new UserItem[80];
        public static UserItem[] GuildStorage = new UserItem[112];
        public static UserItem[] Refine = new UserItem[16];
        public static UserItem HoverItem, SelectedItem;
        public static MirItemCell SelectedCell;

        public static bool PickedUpGold;
        public MirControl ItemLabel, MailLabel, MemoLabel, GuildBuffLabel;
        public static long UseItemTime, PickUpTime, DropViewTime, TargetDeadTime;
        public static uint Gold, Credit;
        public bool ShowReviveMessage;


        public bool NewMail;
        public int NewMailCounter = 0;


        public AttackMode AMode;
        public PetMode PMode;
        public LightSetting Lights;

        public static long NPCTime;
        public static uint NPCID;
        public static float NPCRate;
        public static uint DefaultNPCID;
        public static bool HideAddedStoreStats;

        public long ToggleTime;        
        public static long SpellTime;

        public MirLabel[] OutputLines = new MirLabel[10];
        public List<OutPutMessage> OutputMessages = new List<OutPutMessage>();

        public long OutputDelay;

        public GameScene()
        {
            Scene = this;
            BackColour = Color.Transparent;
            MoveTime = CMain.Time;

            KeyDown += GameScene_KeyDown;

            MainCharacterDialog = new MainCharacterDialog { Parent = this };
            MainDialog = new MainDialog { Parent = this };
            ChatDialog = new ChatDialog { Parent = MainCharacterDialog };
            InventoryDialog = new InventoryDialog { Parent = this };            
            BeltDialog = new BeltDialog { Parent = this };
            StorageDialog = new StorageDialog { Parent = this, Visible = false };
            OptionDialog = new OptionDialog { Parent = this, Visible = false };
            MenuDialog = new MenuDialog { Parent = this, Visible = false };
            NPCDialog = new NPCDialog { Parent = this, Visible = false };
            NPCGoodsDialog = new NPCGoodsDialog(PanelType.Buy) { Parent = this, Visible = false };
            NPCSubGoodsDialog = new NPCGoodsDialog(PanelType.BuySub) { Parent = this, Visible = false };
            NPCDropDialog = new NPCDropDialog { Parent = this, Visible = false };

            HelpDialog = new HelpDialog { Parent = this, Visible = false };
            KeyboardLayoutDialog = new KeyboardLayoutDialog { Parent = this, Visible = false };
            NoticeDialog = new NoticeDialog { Parent = this, Visible = false };

            GroupDialog = new GroupDialog { Parent = this, Visible = false };
            GuildDialog = new GuildDialog { Parent = this, Visible = false };

            BigMapDialog = new BigMapDialog { Parent = this, Visible = false };
            TrustMerchantDialog = new TrustMerchantDialog { Parent = this, Visible = false };
            TradeDialog = new TradeDialog { Parent = this, Visible = false };
            GuestTradeDialog = new GuestTradeDialog { Parent = this, Visible = false };

            SkillBarDialog Bar1 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 0 };
            SkillBarDialogs.Add(Bar1);
            SkillBarDialog Bar2 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 1 };
            SkillBarDialogs.Add(Bar2);
            ChatOptionDialog = new ChatOptionDialog { Parent = this, Visible = false };
            ChatNoticeDialog = new ChatNoticeDialog { Parent = this, Visible = false };

            QuestListDialog = new QuestListDialog { Parent = this, Visible = false };
            QuestDetailDialog = new QuestDetailDialog { Parent = this, Visible = false };
            QuestTrackingDialog = new QuestTrackingDialog { Parent = this, Visible = false };
            QuestLogDialog = new QuestDiaryDialog { Parent = this, Visible = false };

            RankingDialog = new RankingDialog { Parent = this, Visible = false };

            MailListDialog = new MailListDialog { Parent = this, Visible = false };
            MailComposeLetterDialog = new MailComposeLetterDialog { Parent = this, Visible = false };
            MailComposeParcelDialog = new MailComposeParcelDialog { Parent = this, Visible = false };
            MailReadLetterDialog = new MailReadLetterDialog { Parent = this, Visible = false };
            MailReadParcelDialog = new MailReadParcelDialog { Parent = this, Visible = false };

            RefineDialog = new RefineDialog { Parent = this, Visible = false };
            RelationshipDialog = new RelationshipDialog { Parent = this, Visible = false };
            FriendDialog = new FriendDialog { Parent = this, Visible = false };
            MemoDialog = new MemoDialog { Parent = this, Visible = false };
            GameShopDialog = new GameShopDialog { Parent = this, Visible = false };
            ReportDialog = new ReportDialog { Parent = this, Visible = false };

            BuffsDialog = new BuffDialog { 
                Parent = this, 
                Visible = true,
                GetExpandedParameter = () => { return Settings.ExpandedBuffWindow; },
                SetExpandedParameter = (value) => { Settings.ExpandedBuffWindow = value; }
            };            

            KeyboardLayoutDialog = new KeyboardLayoutDialog { Parent = this, Visible = false };

            TimerControl = new TimerDialog { Parent = this, Visible = false };
            CompassControl = new CompassDialog { Parent = this, Visible = false };
            RollControl = new RollDialog { Parent = this, Visible = false };

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i] = new MirLabel
                {
                    AutoSize = true,
                    BackColour = Color.Transparent,
                    Font = new Font(Settings.FontName, 10F),
                    ForeColour = Color.LimeGreen,
                    Location = new Point(20, 25 + i * 13),
                    OutLine = true,
                };
        }

        private void UpdateMouseCursor()
        {
            if (!Settings.UseMouseCursors) return;

            if (GameScene.HoverItem != null)
            {
                if (GameScene.SelectedCell != null && GameScene.SelectedCell.Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Gem && CMain.Ctrl)
                {
                    CMain.SetMouseCursor(MouseCursor.Upgrade);
                }
                else
                {
                    CMain.SetMouseCursor(MouseCursor.Default);
                }
            }
            else if (MapObject.MouseObject != null)
            {
                switch (MapObject.MouseObject.Race)
                {
                    case ObjectType.Monster:
                        CMain.SetMouseCursor(MouseCursor.Attack);
                        break;
                    case ObjectType.Merchant:
                        CMain.SetMouseCursor(MouseCursor.NPCTalk);
                        break;
                    case ObjectType.Player:
                        if (CMain.Shift)
                        {
                            CMain.SetMouseCursor(MouseCursor.AttackRed);
                        }
                        else
                        {
                            CMain.SetMouseCursor(MouseCursor.Default);
                        }
                        break;
                    default:
                        CMain.SetMouseCursor(MouseCursor.Default);
                        break;
                }
            }
            else
            {
                CMain.SetMouseCursor(MouseCursor.Default);
            }

        }

        public void OutputMessage(string message, OutputMessageType type = OutputMessageType.Normal)
        {
            OutputMessages.Add(new OutPutMessage { Message = message, ExpireTime = CMain.Time + 5000, Type = type });
            if (OutputMessages.Count > 10)
                OutputMessages.RemoveAt(0);
        }

        private void ProcessOuput()
        {
            for (int i = 0; i < OutputMessages.Count; i++)
            {
                if (CMain.Time >= OutputMessages[i].ExpireTime)
                    OutputMessages.RemoveAt(i);
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                if (OutputMessages.Count > i)
                {
                    Color color;
                    switch (OutputMessages[i].Type)
                    {
                        case OutputMessageType.Quest:
                            color = Color.Gold;
                            break;
                        case OutputMessageType.Guild:
                            color = Color.DeepPink;
                            break;
                        default:
                            color = Color.LimeGreen;
                            break;
                    }

                    OutputLines[i].Text = OutputMessages[i].Message;
                    OutputLines[i].ForeColour = color;
                    OutputLines[i].Visible = true;
                }
                else
                {
                    OutputLines[i].Text = string.Empty;
                    OutputLines[i].Visible = false;
                }
            }
        }
        private void GameScene_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameScene.Scene.KeyboardLayoutDialog.WaitingForBind != null)
            {
                GameScene.Scene.KeyboardLayoutDialog.CheckNewInput(e);
                return;
            }

            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if (KeyCheck.Key != e.KeyCode)
                    continue;
                if ((KeyCheck.RequireAlt != 2) && (KeyCheck.RequireAlt != (CMain.Alt ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireShift != 2) && (KeyCheck.RequireShift != (CMain.Shift ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireCtrl != 2) && (KeyCheck.RequireCtrl != (CMain.Ctrl ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireTilde != 2) && (KeyCheck.RequireTilde != (CMain.Tilde ? 1 : 0)))
                    continue;
                //now run the real code
                switch (KeyCheck.function)
                {
                    case KeybindOptions.Bar1Skill1: UseSpell(1); break;
                    case KeybindOptions.Bar1Skill2: UseSpell(2); break;
                    case KeybindOptions.Bar1Skill3: UseSpell(3); break;
                    case KeybindOptions.Bar1Skill4: UseSpell(4); break;
                    case KeybindOptions.Bar1Skill5: UseSpell(5); break;
                    case KeybindOptions.Bar1Skill6: UseSpell(6); break;
                    case KeybindOptions.Bar1Skill7: UseSpell(7); break;
                    case KeybindOptions.Bar1Skill8: UseSpell(8); break;
                    case KeybindOptions.Bar2Skill1: UseSpell(9); break;
                    case KeybindOptions.Bar2Skill2: UseSpell(10); break;
                    case KeybindOptions.Bar2Skill3: UseSpell(11); break;
                    case KeybindOptions.Bar2Skill4: UseSpell(12); break;
                    case KeybindOptions.Bar2Skill5: UseSpell(13); break;
                    case KeybindOptions.Bar2Skill6: UseSpell(14); break;
                    case KeybindOptions.Bar2Skill7: UseSpell(15); break;
                    case KeybindOptions.Bar2Skill8: UseSpell(16); break;
                    case KeybindOptions.Inventory:
                    case KeybindOptions.Inventory2:
                        if (!InventoryDialog.Visible) InventoryDialog.Show();
                        else InventoryDialog.Hide();
                        break;
                    case KeybindOptions.Equipment:
                    case KeybindOptions.Equipment2:
                        if (!CharacterDialog.Visible)
                            CharacterDialog.Show();
                        else 
                            CharacterDialog.Hide();
                        break;
                    case KeybindOptions.Skills:
                    case KeybindOptions.Skills2:
                        break;
                    case KeybindOptions.GameShop:
                        //if (!GameShopDialog.Visible) GameShopDialog.Show();
                        //else GameShopDialog.Hide();
                        break;                    
                    case KeybindOptions.Skillbar:
                        if (!Settings.SkillBar)
                            foreach (SkillBarDialog Bar in SkillBarDialogs)
                                Bar.Show();
                        else
                            foreach (SkillBarDialog Bar in SkillBarDialogs)
                                Bar.Hide();
                        break;
                    case KeybindOptions.Relationship:
                        if (!RelationshipDialog.Visible) RelationshipDialog.Show();
                        else RelationshipDialog.Hide();
                        break;
                    case KeybindOptions.Friends:
                        //if (!FriendDialog.Visible) FriendDialog.Show();
                        //else FriendDialog.Hide();
                        break;
                    case KeybindOptions.Guilds:
                        //if (!GuildDialog.Visible) GuildDialog.Show();
                        //else GuildDialog.Hide();
                        break;

                    case KeybindOptions.Ranking:
                        //if (!RankingDialog.Visible) RankingDialog.Show();
                        //else RankingDialog.Hide();
                        break;
                    case KeybindOptions.Quests:
                        if (!QuestLogDialog.Visible) QuestLogDialog.Show();
                        else QuestLogDialog.Hide();
                        break;
                    case KeybindOptions.Exit:
                        QuitGame();
                        return;
                    case KeybindOptions.Logout:
                        LogOut();
                        break;
                    case KeybindOptions.Closeall:
                        InventoryDialog.Hide();
                        CharacterDialog.Hide();
                        OptionDialog.Hide();
                        MenuDialog.Hide();
                        if (NPCDialog.Visible) NPCDialog.Hide();
                        HelpDialog.Hide();
                        KeyboardLayoutDialog.Hide();
                        RankingDialog.Hide();
                        FriendDialog.Hide();
                        RelationshipDialog.Hide();
                        GameShopDialog.Hide();
                        GroupDialog.Hide();
                        GuildDialog.Hide();
                        StorageDialog.Hide();
                        TrustMerchantDialog.Hide();
                        QuestListDialog.Hide();
                        QuestDetailDialog.Hide();
                        QuestLogDialog.Hide();
                        RefineDialog.Hide();
                        BigMapDialog.Hide();
                        MailComposeLetterDialog.Hide();
                        MailComposeParcelDialog.Hide();
                        MailListDialog.Hide();
                        MailReadLetterDialog.Hide();
                        MailReadParcelDialog.Hide();
                        NoticeDialog.Hide();

                        GameScene.Scene.DisposeItemLabel();
                        break;
                    case KeybindOptions.Options:
                    case KeybindOptions.Options2:
                        if (!OptionDialog.Visible) OptionDialog.Show();
                        else OptionDialog.Hide();
                        break;
                    case KeybindOptions.Group:
                        //if (!GroupDialog.Visible) GroupDialog.Show();
                        //else GroupDialog.Hide();
                        break;
                    case KeybindOptions.Belt:
                        if (!BeltDialog.Visible) BeltDialog.Show();
                        else BeltDialog.Hide();
                        break;
                    case KeybindOptions.Pickup:
                        if (CMain.Time > PickUpTime)
                        {
                            PickUpTime = CMain.Time + 200;
                            Network.Enqueue(new C.PickUp());
                        }
                        break;
                    case KeybindOptions.Belt1:
                    case KeybindOptions.Belt1Alt:
                        BeltDialog.Grid[0].UseItem();
                        break;
                    case KeybindOptions.Belt2:
                    case KeybindOptions.Belt2Alt:
                        BeltDialog.Grid[1].UseItem();
                        break;
                    case KeybindOptions.Belt3:
                    case KeybindOptions.Belt3Alt:
                        BeltDialog.Grid[2].UseItem();
                        break;
                    case KeybindOptions.Belt4:
                    case KeybindOptions.Belt4Alt:
                        BeltDialog.Grid[3].UseItem();
                        break;
                    case KeybindOptions.Belt5:
                    case KeybindOptions.Belt5Alt:
                        BeltDialog.Grid[4].UseItem();
                        break;
                    case KeybindOptions.Belt6:
                    case KeybindOptions.Belt6Alt:
                        BeltDialog.Grid[5].UseItem();
                        break;
                    case KeybindOptions.Bigmap:
                        BigMapDialog.Toggle();
                        break;
                    case KeybindOptions.Trade:
                        Network.Enqueue(new C.TradeRequest());
                        break;
                    case KeybindOptions.ChangePetmode:
                        ChangePetMode();
                        break;
                    case KeybindOptions.PetmodeBoth:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.Both });
                        return;
                    case KeybindOptions.PetmodeMoveonly:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.MoveOnly });
                        return;
                    case KeybindOptions.PetmodeAttackonly:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.AttackOnly });
                        return;
                    case KeybindOptions.PetmodeNone:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.None });
                        return;
                    case KeybindOptions.PetmodeFocusMasterTarget:
                        Network.Enqueue(new C.ChangePMode { Mode = PetMode.FocusMasterTarget });
                        return;
                    case KeybindOptions.ChangeAttackmode:
                        ChangeAttackMode();
                        break;
                    case KeybindOptions.AttackmodePeace:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Peace });
                        return;
                    case KeybindOptions.AttackmodeGroup:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Group });
                        return;
                    case KeybindOptions.AttackmodeGuild:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Guild });
                        return;
                    case KeybindOptions.AttackmodeEnemyguild:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.EnemyGuild });
                        return;
                    case KeybindOptions.AttackmodeRedbrown:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.RedBrown });
                        return;
                    case KeybindOptions.AttackmodeAll:
                        Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.All });
                        return;

                    case KeybindOptions.Help:
                        if (!HelpDialog.Visible) HelpDialog.Show();
                        else HelpDialog.Hide();
                        break;
                    case KeybindOptions.Keybind:
                        //if (!KeyboardLayoutDialog.Visible) KeyboardLayoutDialog.Show();
                        //else KeyboardLayoutDialog.Hide();
                        break;
                    case KeybindOptions.Cameramode:

                        if (!MainDialog.Visible)
                        {
                            MainDialog.Show();
                            ChatDialog.Show();
                            BeltDialog.Show();
                            //BuffsDialog.Show();
                        }
                        else
                        {
                            MainDialog.Hide();
                            ChatDialog.Hide();
                            BeltDialog.Hide();
                            //BuffsDialog.Hide();
                        }
                        break;
                    case KeybindOptions.DropView:
                        if (CMain.Time > DropViewTime)
                            DropViewTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.TargetDead:
                        if (CMain.Time > TargetDeadTime)
                            TargetDeadTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.AddGroupMember:
                        if (MapObject.MouseObject == null) break;
                        if (MapObject.MouseObject.Race != ObjectType.Player) break;

                        GameScene.Scene.GroupDialog.AddMember(MapObject.MouseObject.Name);
                        break;
                }
            }
        }

        public void ChangeSkillMode(bool? ctrl)
        {
            if (Settings.SkillMode || ctrl == true)
            {
                Settings.SkillMode = false;
                GameScene.Scene.ChatDialog.ReceiveChat("[SkillMode Ctrl]", ChatType.Hint);
                GameScene.Scene.OptionDialog.ToggleSkillButtons(true);
            }
            else if (!Settings.SkillMode || ctrl == false)
            {
                Settings.SkillMode = true;
                GameScene.Scene.ChatDialog.ReceiveChat("[SkillMode ~]", ChatType.Hint);
                GameScene.Scene.OptionDialog.ToggleSkillButtons(false);
            }
        }

        public void ChangePetMode()
        {
            switch (PMode)
            {
                case PetMode.Both:
                    Network.Enqueue(new C.ChangePMode { Mode = PetMode.MoveOnly });
                    return;
                case PetMode.MoveOnly:
                    Network.Enqueue(new C.ChangePMode { Mode = PetMode.AttackOnly });
                    return;
                case PetMode.AttackOnly:
                    Network.Enqueue(new C.ChangePMode { Mode = PetMode.None });
                    return;
                case PetMode.None:
                    Network.Enqueue(new C.ChangePMode { Mode = PetMode.FocusMasterTarget });
                    return;
                case PetMode.FocusMasterTarget:
                    Network.Enqueue(new C.ChangePMode { Mode = PetMode.Both });
                    return;
            }
        }

        public void ChangeAttackMode()
        {
            switch (AMode)
            {
                case AttackMode.Peace:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Group });
                    return;
                case AttackMode.Group:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Guild });
                    return;
                case AttackMode.Guild:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.EnemyGuild });
                    return;
                case AttackMode.EnemyGuild:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.RedBrown });
                    return;
                case AttackMode.RedBrown:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.All });
                    return;
                case AttackMode.All:
                    Network.Enqueue(new C.ChangeAMode { Mode = AttackMode.Peace });
                    return;
            }
        }

        public void UseSpell(int key)
        {
            UserObject actor = User;

            if (actor.Dead) return;

            ClientMagic magic = null;

            for (int i = 0; i < actor.Magics.Count; i++)
            {
                if (actor.Magics[i].Key != key) continue;
                magic = actor.Magics[i];
                break;
            }

            if (magic == null) return;

            int cost;
            string prefix = string.Empty;
            switch (magic.Spell)
            {
                case Spell.Fencing:
                case Spell.SpiritSword:
                case Spell.Slaying:
                    return;
                case Spell.Thrusting:
                    if (CMain.Time < ToggleTime) return;
                    actor.Thrusting = !actor.Thrusting;
                    ChatDialog.ReceiveChat(prefix + (actor.Thrusting ? "Use Thrusting." : "Do not use Thrusting."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.Thrusting);                    
                    break;
                case Spell.HalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    actor.HalfMoon = !actor.HalfMoon;
                    ChatDialog.ReceiveChat(prefix + (actor.HalfMoon ? "Use Half Moon." : "Do not use Half Moon."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.HalfMoon);
                    break;
                case Spell.FlamingSword:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > actor.MP)
                    {
                        Scene.OutputMessage(GameLanguage.LowMana);
                        return;
                    }
                    SendSpellToggle(actor, magic.Spell, true);
                    break;
                default:
                    actor.NextMagic = magic;
                    actor.NextMagicLocation = MapControl.MapLocation;
                    actor.NextMagicObject = MapObject.MouseObject;
                    actor.NextMagicDirection = MapControl.MouseDirection();
                    break;
            }
        }
        private void SendSpellToggle(UserObject Actor, Spell Spell, bool CanUse)
        {
            if (Actor == User)
                Network.Enqueue(new C.SpellToggle { Spell = Spell, CanUse = CanUse });
            else
                Network.Enqueue(new C.SpellToggle { Spell = Spell });
        }
        public void QuitGame()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                MirMessageBox messageBox = new MirMessageBox(GameLanguage.ExitTip, MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) => Program.Form.Close();
                messageBox.Show();
            }
            else
            {
                ChatDialog.ReceiveChat(string.Format(GameLanguage.CannotLeaveGame, (LogTime - CMain.Time) / 1000), ChatType.System);
            }
        }

        public void LogOut()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                MirMessageBox messageBox = new MirMessageBox(GameLanguage.LogOutTip, MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) =>
                {
                    Network.Enqueue(new C.LogOut());
                    Enabled = false;
                };
                messageBox.Show();
            }
            else
            {
                ChatDialog.ReceiveChat(string.Format(GameLanguage.CannotLeaveGame, (LogTime - CMain.Time) / 1000), ChatType.System);
            }
        }

        protected internal override void DrawControl()
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.DrawControl();
            base.DrawControl();


            if (PickedUpGold || (SelectedCell != null && SelectedCell.Item != null))
            {
                MLibrary library = PickedUpGold ? Libraries.Prguse : Libraries.Items;
                int image = PickedUpGold ? 38 : SelectedCell.Item.Image;
                Size imgSize = Libraries.Items.GetTrueSize(image);
                Point p = CMain.MPoint.Add(-imgSize.Width / 2, -imgSize.Height / 2);

                if (p.X + imgSize.Width >= Settings.ScreenWidth)
                    p.X = Settings.ScreenWidth - imgSize.Width;

                if (p.Y + imgSize.Height >= Settings.ScreenHeight)
                    p.Y = Settings.ScreenHeight - imgSize.Height;

                library.Draw(image, p.X, p.Y);
            }

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i].Draw();
        }
        public override void Process()
        {
            if (MapControl == null || User == null)
                return;

            if (CMain.Time >= MoveTime)
            {
                MoveTime += 100; //Move Speed
                CanMove = true;
                MapControl.AnimationCount++;
                MapControl.TextureValid = false;
            }
            else
                CanMove = false;

            if (CMain.Time >= CMain.NextPing)
            {
                CMain.NextPing = CMain.Time + 60000;
                Network.Enqueue(new C.KeepAlive() { Time = CMain.Time });
            }

            TimerControl.Process();
            CompassControl.Process();
            RankingDialog.Process();

            MirItemCell cell = MouseControl as MirItemCell;

            if (cell != null && HoverItem != cell.Item && HoverItem != cell.ShadowItem)
            {
                DisposeItemLabel();
                HoverItem = null;
                CreateItemLabel(cell.Item);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                ItemLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + ItemLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - ItemLabel.Size.Width;

                if (y + ItemLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - ItemLabel.Size.Height;
                ItemLabel.Location = new Point(x, y);
            }

            if (MailLabel != null && !MailLabel.IsDisposed)
            {
                MailLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MailLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MailLabel.Size.Width;

                if (y + MailLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MailLabel.Size.Height;
                MailLabel.Location = new Point(x, y);
            }

            if (MemoLabel != null && !MemoLabel.IsDisposed)
            {
                MemoLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MemoLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MemoLabel.Size.Width;

                if (y + MemoLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MemoLabel.Size.Height;
                MemoLabel.Location = new Point(x, y);
            }

            if (GuildBuffLabel != null && !GuildBuffLabel.IsDisposed)
            {
                GuildBuffLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + GuildBuffLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - GuildBuffLabel.Size.Width;

                if (y + GuildBuffLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - GuildBuffLabel.Size.Height;
                GuildBuffLabel.Location = new Point(x, y);
            }

            if (!User.Dead) ShowReviveMessage = false;

            if (ShowReviveMessage && CMain.Time > User.DeadTime && User.CurrentAction == MirAction.Dead)
            {
                ShowReviveMessage = false;
                MirMessageBox messageBox = new MirMessageBox(GameLanguage.DiedTip, MirMessageBoxButtons.YesNo, false);

                messageBox.YesButton.Click += (o, e) =>
                {
                    if (User.Dead) Network.Enqueue(new C.TownRevive());
                };

                messageBox.AfterDraw += (o, e) =>
                {
                    if (!User.Dead) messageBox.Dispose();
                };

                messageBox.Show();
            }

            BuffsDialog.Process();

            MapControl.Process();
            MainDialog.Process();
            MainCharacterDialog.Process();
            InventoryDialog.Process();
            GameShopDialog.Process();

            foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                Bar.Process();

            DialogProcess();

            ProcessOuput();

            UpdateMouseCursor();

            SoundManager.ProcessDelayedSounds();
        }

        public void DialogProcess()
        {
            if(Settings.SkillBar)
            {
                foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                    Bar.Show();
            }
            else
            {
                foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                    Bar.Hide();
            }

            for (int i = 0; i < Scene.SkillBarDialogs.Count; i++)
            {
                if (i * 2 > Settings.SkillbarLocation.Length) break;
                if ((Settings.SkillbarLocation[i, 0] > Settings.Resolution - 100) || (Settings.SkillbarLocation[i, 1] > 700)) continue;//in theory you'd want the y coord to be validated based on resolution, but since client only allows for wider screens and not higher :(
                Scene.SkillBarDialogs[i].Location = new Point(Settings.SkillbarLocation[i, 0], Settings.SkillbarLocation[i, 1]);
            }
        }

        public override void ProcessPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.KeepAlive:
                    KeepAlive((S.KeepAlive)p);
                    break;
                case (short)ServerPacketIds.MapInformation: //MapInfo
                    MapInformation((S.MapInformation)p);
                    break;
                case (short)ServerPacketIds.UserInformation:
                    UserInformation((S.UserInformation)p);
                    break;
                case (short)ServerPacketIds.UserLocation:
                    UserLocation((S.UserLocation)p);
                    break;
                case (short)ServerPacketIds.ObjectPlayer:
                    ObjectPlayer((S.ObjectPlayer)p);
                    break;
                case (short)ServerPacketIds.ObjectRemove:
                    ObjectRemove((S.ObjectRemove)p);
                    break;
                case (short)ServerPacketIds.ObjectTurn:
                    ObjectTurn((S.ObjectTurn)p);
                    break;
                case (short)ServerPacketIds.ObjectWalk:
                    ObjectWalk((S.ObjectWalk)p);
                    break;
                case (short)ServerPacketIds.Chat:
                    ReceiveChat((S.Chat)p);
                    break;
                case (short)ServerPacketIds.ObjectChat:
                    ObjectChat((S.ObjectChat)p);
                    break;
                case (short)ServerPacketIds.MoveItem:
                    MoveItem((S.MoveItem)p);
                    break;
                case (short)ServerPacketIds.EquipItem:
                    EquipItem((S.EquipItem)p);
                    break;
                case (short)ServerPacketIds.MergeItem:
                    MergeItem((S.MergeItem)p);
                    break;
                case (short)ServerPacketIds.RemoveItem:
                    RemoveItem((S.RemoveItem)p);
                    break;
                case (short)ServerPacketIds.TakeBackItem:
                    TakeBackItem((S.TakeBackItem)p);
                    break;
                case (short)ServerPacketIds.StoreItem:
                    StoreItem((S.StoreItem)p);
                    break;
                case (short)ServerPacketIds.DepositRefineItem:
                    DepositRefineItem((S.DepositRefineItem)p);
                    break;
                case (short)ServerPacketIds.RetrieveRefineItem:
                    RetrieveRefineItem((S.RetrieveRefineItem)p);
                    break;
                case (short)ServerPacketIds.RefineCancel:
                    RefineCancel((S.RefineCancel)p);
                    break;
                case (short)ServerPacketIds.RefineItem:
                    RefineItem((S.RefineItem)p);
                    break;
                case (short)ServerPacketIds.DepositTradeItem:
                    DepositTradeItem((S.DepositTradeItem)p);
                    break;
                case (short)ServerPacketIds.RetrieveTradeItem:
                    RetrieveTradeItem((S.RetrieveTradeItem)p);
                    break;
                case (short)ServerPacketIds.SplitItem:
                    SplitItem((S.SplitItem)p);
                    break;
                case (short)ServerPacketIds.SplitItem1:
                    SplitItem1((S.SplitItem1)p);
                    break;
                case (short)ServerPacketIds.UseItem:
                    UseItem((S.UseItem)p);
                    break;
                case (short)ServerPacketIds.DropItem:
                    DropItem((S.DropItem)p);
                    break;
                case (short)ServerPacketIds.PlayerUpdate:
                    PlayerUpdate((S.PlayerUpdate)p);
                    break;
                case (short)ServerPacketIds.LogOutSuccess:
                    LogOutSuccess((S.LogOutSuccess)p);
                    break;
                case (short)ServerPacketIds.LogOutFailed:
                    LogOutFailed((S.LogOutFailed)p);
                    break;
                case (short)ServerPacketIds.ReturnToLogin:
                    ReturnToLogin((S.ReturnToLogin)p);
                    break;
                case (short)ServerPacketIds.TimeOfDay:
                    TimeOfDay((S.TimeOfDay)p);
                    break;
                case (short)ServerPacketIds.ChangeAMode:
                    ChangeAMode((S.ChangeAMode)p);
                    break;
                case (short)ServerPacketIds.ChangePMode:
                    ChangePMode((S.ChangePMode)p);
                    break;
                case (short)ServerPacketIds.ObjectItem:
                    ObjectItem((S.ObjectItem)p);
                    break;
                case (short)ServerPacketIds.ObjectGold:
                    ObjectGold((S.ObjectGold)p);
                    break;
                case (short)ServerPacketIds.GainedItem:
                    GainedItem((S.GainedItem)p);
                    break;
                case (short)ServerPacketIds.GainedGold:
                    GainedGold((S.GainedGold)p);
                    break;
                case (short)ServerPacketIds.LoseGold:
                    LoseGold((S.LoseGold)p);
                    break;
                case (short)ServerPacketIds.GainedCredit:
                    GainedCredit((S.GainedCredit)p);
                    break;
                case (short)ServerPacketIds.LoseCredit:
                    LoseCredit((S.LoseCredit)p);
                    break;
                case (short)ServerPacketIds.ObjectMonster:
                    ObjectMonster((S.ObjectMonster)p);
                    break;
                case (short)ServerPacketIds.ObjectAttack:
                    ObjectAttack((S.ObjectAttack)p);
                    break;
                case (short)ServerPacketIds.Struck:
                    Struck((S.Struck)p);
                    break;
                case (short)ServerPacketIds.DamageIndicator:
                    DamageIndicator((S.DamageIndicator)p);
                    break;
                case (short)ServerPacketIds.ObjectStruck:
                    ObjectStruck((S.ObjectStruck)p);
                    break;
                case (short)ServerPacketIds.DuraChanged:
                    DuraChanged((S.DuraChanged)p);
                    break;
                case (short)ServerPacketIds.HealthChanged:
                    HealthChanged((S.HealthChanged)p);
                    break;
                case (short)ServerPacketIds.DeleteItem:
                    DeleteItem((S.DeleteItem)p);
                    break;
                case (short)ServerPacketIds.Death:
                    Death((S.Death)p);
                    break;
                case (short)ServerPacketIds.ObjectDied:
                    ObjectDied((S.ObjectDied)p);
                    break;
                case (short)ServerPacketIds.ColourChanged:
                    ColourChanged((S.ColourChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectColourChanged:
                    ObjectColourChanged((S.ObjectColourChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectGuildNameChanged:
                    ObjectGuildNameChanged((S.ObjectGuildNameChanged)p);
                    break;
                case (short)ServerPacketIds.GainExperience:
                    GainExperience((S.GainExperience)p);
                    break;
                case (short)ServerPacketIds.LevelChanged:
                    LevelChanged((S.LevelChanged)p);
                    break; 
                case (short)ServerPacketIds.ObjectLeveled:
                    ObjectLeveled((S.ObjectLeveled)p);
                    break;
                case (short)ServerPacketIds.ObjectHarvest:
                    ObjectHarvest((S.ObjectHarvest)p);
                    break;
                case (short)ServerPacketIds.ObjectHarvested:
                    ObjectHarvested((S.ObjectHarvested)p);
                    break;
                case (short)ServerPacketIds.ObjectNpc:
                    ObjectNPC((S.ObjectNPC)p);
                    break;
                case (short)ServerPacketIds.NPCResponse:
                    NPCResponse((S.NPCResponse)p);
                    break;
                case (short)ServerPacketIds.ObjectHide:
                    ObjectHide((S.ObjectHide)p);
                    break;
                case (short)ServerPacketIds.ObjectShow:
                    ObjectShow((S.ObjectShow)p);
                    break;
                case (short)ServerPacketIds.Poisoned:
                    Poisoned((S.Poisoned)p);
                    break;
                case (short)ServerPacketIds.ObjectPoisoned:
                    ObjectPoisoned((S.ObjectPoisoned)p);
                    break;
                case (short)ServerPacketIds.MapChanged:
                    MapChanged((S.MapChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectTeleportOut:
                    ObjectTeleportOut((S.ObjectTeleportOut)p);
                    break;
                case (short)ServerPacketIds.ObjectTeleportIn:
                    ObjectTeleportIn((S.ObjectTeleportIn)p);
                    break;
                case (short)ServerPacketIds.TeleportIn:
                    TeleportIn();
                    break;
                case (short)ServerPacketIds.NPCGoods:
                    NPCGoods((S.NPCGoods)p);
                    break;
                case (short)ServerPacketIds.NPCSell:
                    NPCSell();
                    break;
                case (short)ServerPacketIds.NPCRepair:
                    NPCRepair((S.NPCRepair)p);
                    break;
                case (short)ServerPacketIds.NPCSRepair:
                    NPCSRepair((S.NPCSRepair)p);
                    break;
                case (short)ServerPacketIds.NPCRefine:
                    NPCRefine((S.NPCRefine)p);
                    break;
                case (short)ServerPacketIds.NPCCheckRefine:
                    NPCCheckRefine((S.NPCCheckRefine)p);
                    break;
                case (short)ServerPacketIds.NPCCollectRefine:
                    NPCCollectRefine((S.NPCCollectRefine)p);
                    break;
                case (short)ServerPacketIds.NPCReplaceWedRing:
                    NPCReplaceWedRing((S.NPCReplaceWedRing)p);
                    break;
                case (short)ServerPacketIds.NPCStorage:
                    NPCStorage();
                    break;
                case (short)ServerPacketIds.NPCRequestInput:
                    NPCRequestInput((S.NPCRequestInput)p);
                    break;
                case (short)ServerPacketIds.SellItem:
                    SellItem((S.SellItem)p);
                    break;
                case (short)ServerPacketIds.RepairItem:
                    RepairItem((S.RepairItem)p);
                    break;
                case (short)ServerPacketIds.ItemRepaired:
                    ItemRepaired((S.ItemRepaired)p);
                    break;
                case (short)ServerPacketIds.ItemSealChanged:
                    ItemSealChanged((S.ItemSealChanged)p);
                    break;
                case (short)ServerPacketIds.NewMagic:
                    NewMagic((S.NewMagic)p);
                    break;
                case (short)ServerPacketIds.MagicLeveled:
                    MagicLeveled((S.MagicLeveled)p);
                    break;
                case (short)ServerPacketIds.Magic:
                    Magic((S.Magic)p);
                    break;
                case (short)ServerPacketIds.MagicDelay:
                    MagicDelay((S.MagicDelay)p);
                    break;
                case (short)ServerPacketIds.MagicCast:
                    MagicCast((S.MagicCast)p);
                    break;
                case (short)ServerPacketIds.ObjectMagic:
                    ObjectMagic((S.ObjectMagic)p);
                    break;
                case (short)ServerPacketIds.ObjectProjectile:
                    ObjectProjectile((S.ObjectProjectile)p);
                    break;
                case (short)ServerPacketIds.ObjectEffect:
                    ObjectEffect((S.ObjectEffect)p);
                    break;
                case (short)ServerPacketIds.RangeAttack:
                    RangeAttack((S.RangeAttack)p);
                    break;
                case (short)ServerPacketIds.Pushed:
                    Pushed((S.Pushed)p);
                    break;
                case (short)ServerPacketIds.ObjectPushed:
                    ObjectPushed((S.ObjectPushed)p);
                    break;
                case (short)ServerPacketIds.ObjectName:
                    ObjectName((S.ObjectName)p);
                    break;
                case (short)ServerPacketIds.UserStorage:
                    UserStorage((S.UserStorage)p);
                    break;
                case (short)ServerPacketIds.SwitchGroup:
                    SwitchGroup((S.SwitchGroup)p);
                    break;
                case (short)ServerPacketIds.DeleteGroup:
                    DeleteGroup();
                    break;
                case (short)ServerPacketIds.DeleteMember:
                    DeleteMember((S.DeleteMember)p);
                    break;
                case (short)ServerPacketIds.GroupInvite:
                    GroupInvite((S.GroupInvite)p);
                    break;
                case (short)ServerPacketIds.AddMember:
                    AddMember((S.AddMember)p);
                    break;
                case (short)ServerPacketIds.GroupMembersMap:
                    GroupMembersMap((S.GroupMembersMap)p);
                    break;
                case (short)ServerPacketIds.SendMemberLocation:
                    SendMemberLocation((S.SendMemberLocation)p);
                    break;
                case (short)ServerPacketIds.Revived:
                    Revived();
                    break;
                case (short)ServerPacketIds.ObjectRevived:
                    ObjectRevived((S.ObjectRevived)p);
                    break;
                case (short)ServerPacketIds.SpellToggle:
                    SpellToggle((S.SpellToggle)p);
                    break;
                case (short)ServerPacketIds.ObjectHealth:
                    ObjectHealth((S.ObjectHealth)p);
                    break;
                case (short)ServerPacketIds.ObjectMana:
                    ObjectMana((S.ObjectMana)p);
                    break;
                case (short)ServerPacketIds.MapEffect:
                    MapEffect((S.MapEffect)p);
                    break;
                case (short)ServerPacketIds.AllowObserve:
                    AllowObserve = ((S.AllowObserve)p).Allow;
                    break;
                case (short)ServerPacketIds.ObjectRangeAttack:
                    ObjectRangeAttack((S.ObjectRangeAttack)p);
                    break;
                case (short)ServerPacketIds.AddBuff:
                    AddBuff((S.AddBuff)p);
                    break;
                case (short)ServerPacketIds.RemoveBuff:
                    RemoveBuff((S.RemoveBuff)p);
                    break;
                case (short)ServerPacketIds.PauseBuff:
                    PauseBuff((S.PauseBuff)p);
                    break;
                case (short)ServerPacketIds.ObjectHidden:
                    ObjectHidden((S.ObjectHidden)p);
                    break;
                case (short)ServerPacketIds.RefreshItem:
                    RefreshItem((S.RefreshItem)p);
                    break;
                case (short)ServerPacketIds.ObjectSpell:
                    ObjectSpell((S.ObjectSpell)p);
                    break;
                case (short)ServerPacketIds.UserDash:
                    UserDash((S.UserDash)p);
                    break;
                case (short)ServerPacketIds.ObjectDash:
                    ObjectDash((S.ObjectDash)p);
                    break;
                case (short)ServerPacketIds.UserDashFail:
                    UserDashFail((S.UserDashFail)p);
                    break;
                case (short)ServerPacketIds.ObjectDashFail:
                    ObjectDashFail((S.ObjectDashFail)p);
                    break;
                case (short)ServerPacketIds.NPCConsign:
                    NPCConsign();
                    break;
                case (short)ServerPacketIds.NPCMarket:
                    NPCMarket((S.NPCMarket)p);
                    break;
                case (short)ServerPacketIds.NPCMarketPage:
                    NPCMarketPage((S.NPCMarketPage)p);
                    break;
                case (short)ServerPacketIds.ConsignItem:
                    ConsignItem((S.ConsignItem)p);
                    break;
                case (short)ServerPacketIds.MarketFail:
                    MarketFail((S.MarketFail)p);
                    break;
                case (short)ServerPacketIds.MarketSuccess:
                    MarketSuccess((S.MarketSuccess)p);
                    break;
                case (short)ServerPacketIds.RemoveMagic:
                    RemoveMagic((S.RemoveMagic)p);
                    break;
                case (short)ServerPacketIds.BaseStatsInfo:
                    BaseStatsInfo((S.BaseStatsInfo)p);
                    break;
                case (short)ServerPacketIds.UserName:
                    UserName((S.UserName)p);
                    break;
                case (short)ServerPacketIds.ChatItemStats:
                    ChatItemStats((S.ChatItemStats)p);
                    break;
                case (short)ServerPacketIds.GuildInvite:
                    GuildInvite((S.GuildInvite)p);
                    break;
                case (short)ServerPacketIds.GuildMemberChange:
                    GuildMemberChange((S.GuildMemberChange)p);
                    break;
                case (short)ServerPacketIds.GuildNoticeChange:
                    GuildNoticeChange((S.GuildNoticeChange)p);
                    break;
                case (short)ServerPacketIds.GuildStatus:
                    GuildStatus((S.GuildStatus)p);
                    break;
                case (short)ServerPacketIds.GuildExpGain:
                    GuildExpGain((S.GuildExpGain)p);
                    break;
                case (short)ServerPacketIds.GuildNameRequest:
                    GuildNameRequest((S.GuildNameRequest)p);
                    break;
                case (short)ServerPacketIds.GuildStorageGoldChange:
                    GuildStorageGoldChange((S.GuildStorageGoldChange)p);
                    break;
                case (short)ServerPacketIds.GuildStorageItemChange:
                    GuildStorageItemChange((S.GuildStorageItemChange)p);
                    break;
                case (short)ServerPacketIds.GuildStorageList:
                    GuildStorageList((S.GuildStorageList)p);
                    break;
                case (short)ServerPacketIds.GuildRequestWar:
                    GuildRequestWar((S.GuildRequestWar)p);
                    break;
                case (short)ServerPacketIds.DefaultNPC:
                    DefaultNPC((S.DefaultNPC)p);
                    break;
                case (short)ServerPacketIds.NPCUpdate:
                    NPCUpdate((S.NPCUpdate)p);
                    break;
                case (short)ServerPacketIds.NPCImageUpdate:
                    NPCImageUpdate((S.NPCImageUpdate)p);
                    break;
                case (short)ServerPacketIds.MarriageRequest:
                    MarriageRequest((S.MarriageRequest)p);
                    break;
                case (short)ServerPacketIds.DivorceRequest:
                    DivorceRequest((S.DivorceRequest)p);
                    break;
                case (short)ServerPacketIds.TradeRequest:
                    TradeRequest((S.TradeRequest)p);
                    break;
                case (short)ServerPacketIds.TradeAccept:
                    TradeAccept((S.TradeAccept)p);
                    break;
                case (short)ServerPacketIds.TradeGold:
                    TradeGold((S.TradeGold)p);
                    break;
                case (short)ServerPacketIds.TradeItem:
                    TradeItem((S.TradeItem)p);
                    break;
                case (short)ServerPacketIds.TradeConfirm:
                    TradeConfirm();
                    break;
                case (short)ServerPacketIds.TradeCancel:
                    TradeCancel((S.TradeCancel)p);
                    break;
                case (short)ServerPacketIds.ChangeQuest:
                    ChangeQuest((S.ChangeQuest)p);
                    break;
                case (short)ServerPacketIds.CompleteQuest:
                    CompleteQuest((S.CompleteQuest)p);
                    break;
                case (short)ServerPacketIds.ShareQuest:
                    ShareQuest((S.ShareQuest)p);
                    break;
                case (short)ServerPacketIds.GainedQuestItem:
                    GainedQuestItem((S.GainedQuestItem)p);
                    break;
                case (short)ServerPacketIds.DeleteQuestItem:
                    DeleteQuestItem((S.DeleteQuestItem)p);
                    break;
                case (short)ServerPacketIds.CombineItem:
                    CombineItem((S.CombineItem)p);
                    break;
                case (short)ServerPacketIds.ItemUpgraded:
                    ItemUpgraded((S.ItemUpgraded)p);
                    break;
                case (short)ServerPacketIds.ObjectLevelEffects:
                    ObjectLevelEffects((S.ObjectLevelEffects)p);
                    break;
                case (short)ServerPacketIds.SendOutputMessage:
                    SendOutputMessage((S.SendOutputMessage)p);
                    break;
                case (short)ServerPacketIds.ReceiveMail:
                    ReceiveMail((S.ReceiveMail)p);
                    break;
                case (short)ServerPacketIds.MailLockedItem:
                    MailLockedItem((S.MailLockedItem)p);
                    break;
                case (short)ServerPacketIds.MailSent:
                    MailSent((S.MailSent)p);
                    break;
                case (short)ServerPacketIds.MailSendRequest:
                    MailSendRequest((S.MailSendRequest)p);
                    break;
                case (short)ServerPacketIds.ParcelCollected:
                    ParcelCollected((S.ParcelCollected)p);
                    break;
                case (short)ServerPacketIds.MailCost:
                    MailCost((S.MailCost)p);
                    break;
                case (short)ServerPacketIds.ResizeInventory:
                    ResizeInventory((S.ResizeInventory)p);
                    break;
                case (short)ServerPacketIds.ResizeStorage:
                    ResizeStorage((S.ResizeStorage)p);
                    break;
                case (short)ServerPacketIds.FriendUpdate:
                    FriendUpdate((S.FriendUpdate)p);
                    break;
                case (short)ServerPacketIds.LoverUpdate:
                    LoverUpdate((S.LoverUpdate)p);
                    break;
                case (short)ServerPacketIds.GuildBuffList:
                    GuildBuffList((S.GuildBuffList)p);
                    break;
                case (short)ServerPacketIds.GameShopInfo:
                    GameShopUpdate((S.GameShopInfo)p);
                    break;
                case (short)ServerPacketIds.GameShopStock:
                    GameShopStock((S.GameShopStock)p);
                    break;
                case (short)ServerPacketIds.Rankings:
                    Rankings((S.Rankings)p);
                    break;
                case (short)ServerPacketIds.OpenBrowser:                  
                    OpenBrowser((S.OpenBrowser)p);
                    break;
                case (short)ServerPacketIds.PlaySound:
                    PlaySound((S.PlaySound)p);
                    break;
                case (short)ServerPacketIds.SetTimer:
                    SetTimer((S.SetTimer)p);
                    break;
                case (short)ServerPacketIds.ExpireTimer:
                    ExpireTimer((S.ExpireTimer)p);
                    break;
                case (short)ServerPacketIds.UpdateNotice:
                    ShowNotice((S.UpdateNotice)p);
                    break;
                case (short)ServerPacketIds.Roll:
                    Roll((S.Roll)p);
                    break;
                case (short)ServerPacketIds.SetCompass:
                    SetCompass((S.SetCompass)p);
                    break;
                default:
                    base.ProcessPacket(p);
                    break;
            }
        }

        private void KeepAlive(S.KeepAlive p)
        {
            if (p.Time == 0) return;
            CMain.PingTime = (CMain.Time - p.Time);
        }
        private void MapInformation(S.MapInformation p)
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.Dispose();
            MapControl = new MapControl { Index = p.MapIndex, FileName = Path.Combine(Settings.MapPath, p.FileName + ".map"), Title = p.Title, MiniMap = p.MiniMap, BigMap = p.BigMap, Lights = p.Lights, Lightning = p.Lightning, Fire = p.Fire, MapDarkLight = p.MapDarkLight, Music = p.Music };
            
            MapControl.LoadMap();
            Scene.MainDialog.MapLabel.Text = p.Title;
            InsertControl(0, MapControl);
        }

        private void UserInformation(S.UserInformation p)
        {
            User = new UserObject(p.ObjectID);
            User.Load(p);
            Gold = p.Gold;
            Credit = p.Credit;

            CharacterDialog = new CharacterDialog(MirGridType.Equipment, User) { Parent = this, Visible = false };
            InventoryDialog.RefreshInventory();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
                Bar.Update();
            AllowObserve = p.AllowObserve;
            Observing = p.Observer;
        }

        private void UserLocation(S.UserLocation p)
        {
            MapControl.NextAction = 0;
            if (User.CurrentLocation == p.Location && User.Direction == p.Direction) return;

            if (Settings.DebugMode)
            {
                ReceiveChat(new S.Chat { Message = "Displacement", Type = ChatType.System });
            }

            MapControl.RemoveObject(User);
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;

            if (User.Dead) return;

            User.ClearMagic();
            User.QueuedAction = null;

            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == MirAction.Pushed) continue;
                User.ActionFeed.RemoveAt(i);
            }

            User.SetAction();
        }
        private void ReceiveChat(S.Chat p)
        {
            ChatDialog.ReceiveChat(p.Message, p.Type);
        }
        private void ObjectPlayer(S.ObjectPlayer p)
        {
            PlayerObject player = new PlayerObject(p.ObjectID);
            player.Load(p);
        }

        private void ObjectRemove(S.ObjectRemove p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Remove();
            }
        }
        private void ObjectTurn(S.ObjectTurn p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Standing, Direction = p.Direction, Location = p.Location });
                return;
            }
        }
        private void ObjectWalk(S.ObjectWalk p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Walking, Direction = p.Direction, Location = p.Location });
                return;
            }
        }
        private void ObjectChat(S.ObjectChat p)
        {
            ChatDialog.ReceiveChat(p.Text, p.Type);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Chat(RegexFunctions.CleanChatString(p.Text));
                return;
            }

        }
        private void MoveItem(S.MoveItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.Grid[p.From];
                    break;
                case MirGridType.Trade:
                    fromCell = TradeDialog.Grid[p.From];
                    break;
                case MirGridType.Refine:
                    fromCell = RefineDialog.Grid[p.From];
                    break;
                default:
                    return;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                case MirGridType.Trade:
                    toCell = TradeDialog.Grid[p.To];
                    break;
                case MirGridType.Refine:
                    toCell = RefineDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.Grid == MirGridType.Trade)
                TradeDialog.ChangeLockState(false);

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;

            User.RefreshStats();
        }
        private void EquipItem(S.EquipItem p)
        {
            MirItemCell fromCell, toCell;

            toCell = CharacterDialog.Grid[p.To];

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;
            User.RefreshStats();
        }

        private void CombineItem(S.CombineItem p)
        {
            MirItemCell fromCell = null;
            MirItemCell toCell = null;
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = InventoryDialog.GetCell(p.IDFrom) ?? BeltDialog.GetCell(p.IDFrom);
                    toCell = InventoryDialog.GetCell(p.IDTo) ?? BeltDialog.GetCell(p.IDTo);
                    break;
            }            

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.Destroy) toCell.Item = null;

            if (!p.Success) return;

            fromCell.Item = null;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    User.RefreshStats();
                    break;
            }
        }

        private void MergeItem(S.MergeItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.GridFrom)
            {
                case MirGridType.Inventory:
                    fromCell = InventoryDialog.GetCell(p.IDFrom) ?? BeltDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Equipment:
                    fromCell = CharacterDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Trade:
                    fromCell = TradeDialog.GetCell(p.IDFrom);
                    break;
                default:
                    return;
            }

            switch (p.GridTo)
            {
                case MirGridType.Inventory:
                    toCell = InventoryDialog.GetCell(p.IDTo) ?? BeltDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Equipment:
                    toCell = CharacterDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Trade:
                    toCell = TradeDialog.GetCell(p.IDTo);
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.GridFrom == MirGridType.Trade || p.GridTo == MirGridType.Trade)
                TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            if (fromCell.Item.Count <= toCell.Item.Info.StackSize - toCell.Item.Count)
            {
                toCell.Item.Count += fromCell.Item.Count;
                fromCell.Item = null;
            }
            else
            {
                fromCell.Item.Count -= (ushort)(toCell.Item.Info.StackSize - toCell.Item.Count);
                toCell.Item.Count = toCell.Item.Info.StackSize;
            }

            User.RefreshStats();
        }
        private void RemoveItem(S.RemoveItem p)
        {
            MirItemCell toCell;

            int index = -1;
            MirItemCell fromCell = null;
            for (int i = 0; i < MapObject.User.Equipment.Length; i++)
            {
                if (MapObject.User.Equipment[i] == null || MapObject.User.Equipment[i].UniqueID != p.UniqueID) continue;
                index = i;
                fromCell = CharacterDialog.Grid[index];
                break;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();

        }

        private void TakeBackItem(S.TakeBackItem p)
        {
            MirItemCell fromCell = StorageDialog.Grid[p.From];

            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void StoreItem(S.StoreItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = StorageDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void DepositRefineItem(S.DepositRefineItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = RefineDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }

        private void RetrieveRefineItem(S.RetrieveRefineItem p)
        {
            MirItemCell fromCell = RefineDialog.Grid[p.From];
            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }

        private void RefineCancel(S.RefineCancel p)
        {
            RefineDialog.RefineReset();  
        }

        private void RefineItem(S.RefineItem p)
        {
            RefineDialog.RefineReset();
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    User.Inventory[i] = null;
                    break;
                }
            }
            NPCDialog.Hide();
        }
        private void DepositTradeItem(S.DepositTradeItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = TradeDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;
            TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void RetrieveTradeItem(S.RetrieveTradeItem p)
        {
            MirItemCell fromCell = TradeDialog.Grid[p.From];
            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;
            TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void SplitItem(S.SplitItem p)
        {
            Bind(p.Item);

            UserItem[] array;
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    array = MapObject.User.Inventory;
                    break;
                case MirGridType.Storage:
                    array = Storage;
                    break;
                default:
                    return;
            }

            if (p.Grid == MirGridType.Inventory && (p.Item.Info.Type == ItemType.Potion || p.Item.Info.Type == ItemType.Scroll || (p.Item.Info.Type == ItemType.Script && p.Item.Info.Effect == 1)))
            {
                if (p.Item.Info.Type == ItemType.Potion || p.Item.Info.Type == ItemType.Scroll || (p.Item.Info.Type == ItemType.Script && p.Item.Info.Effect == 1))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (array[i] != null) continue;
                        array[i] = p.Item;
                        User.RefreshStats();
                        return;
                    }
                }                
            }

            for (int i = GameScene.User.BeltIdx; i < array.Length; i++)
            {
                if (array[i] != null) continue;
                array[i] = p.Item;
                User.RefreshStats();
                return;
            }

            for (int i = 0; i < GameScene.User.BeltIdx; i++)
            {
                if (array[i] != null) continue;
                array[i] = p.Item;
                User.RefreshStats();
                return;
            }
        }

        private void SplitItem1(S.SplitItem1 p)
        {
            MirItemCell cell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    cell = StorageDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            cell.Item.Count -= p.Count;
            User.RefreshStats();
        }
        private void UseItem(S.UseItem p)
        {
            MirItemCell cell = null;
            bool hero = false;
            
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
            }            

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            if (cell.Item.Count > 1) cell.Item.Count--;
            else cell.Item = null;
            User.RefreshStats();
        }
        private void DropItem(S.DropItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);            

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;

            User.RefreshStats();            
        }

        private void CompleteQuest(S.CompleteQuest p)
        {
            User.CompletedQuests = p.CompletedQuests;
        }

        private void ShareQuest(S.ShareQuest p)
        {
            ClientQuestInfo quest = GameScene.QuestInfoList.FirstOrDefault(e => e.Index == p.QuestIndex);
            
            if (quest == null) return;

            MirMessageBox messageBox = new MirMessageBox(string.Format("{0} would like to share a quest with you. Do you accept?", p.SharerName), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.AcceptQuest { NPCIndex = 0, QuestIndex = quest.Index });

            messageBox.Show();
        }

        private void ChangeQuest(S.ChangeQuest p)
        {
            switch(p.QuestState)
            {
                case QuestState.Add:
                    User.CurrentQuests.Add(p.Quest);

                    foreach (ClientQuestProgress quest in User.CurrentQuests)
                        BindQuest(quest);
                    if (Settings.TrackedQuests.Contains(p.Quest.Id))
                    {
                        GameScene.Scene.QuestTrackingDialog.AddQuest(p.Quest, true);
                    }

                    if (p.TrackQuest)
                    {
                        GameScene.Scene.QuestTrackingDialog.AddQuest(p.Quest);
                    }

                    break;
                case QuestState.Update:
                    for (int i = 0; i < User.CurrentQuests.Count; i++)
                    {
                        if (User.CurrentQuests[i].Id != p.Quest.Id) continue;

                        User.CurrentQuests[i] = p.Quest;
                    }

                    foreach (ClientQuestProgress quest in User.CurrentQuests)
                        BindQuest(quest);

                    break;
                case QuestState.Remove:

                    for (int i = User.CurrentQuests.Count - 1; i >= 0; i--)
                    {
                        if (User.CurrentQuests[i].Id != p.Quest.Id) continue;

                        User.CurrentQuests.RemoveAt(i);
                    }

                    GameScene.Scene.QuestTrackingDialog.RemoveQuest(p.Quest);

                    break;
            }

            GameScene.Scene.QuestTrackingDialog.DisplayQuests();

            if (Scene.QuestListDialog.Visible)
            {
                Scene.QuestListDialog.DisplayInfo();
            }

            if (Scene.QuestLogDialog.Visible)
            {
                Scene.QuestLogDialog.DisplayQuests();
            }
        }

        private void PlayerUpdate(S.PlayerUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].ObjectID != p.ObjectID) continue;

                PlayerObject player = MapControl.Objects[i] as PlayerObject;
                if (player != null) player.Update(p);
                return;
            }
        }

        private void LogOutSuccess(S.LogOutSuccess p)
        {
            User = null;
            CMain.SetResolution(800, 600);

            ActiveScene = new SelectScene(p.Characters);
            Dispose();
        }
        private void LogOutFailed(S.LogOutFailed p)
        {
            Enabled = true;
        }

        private void ReturnToLogin(S.ReturnToLogin p)
        {
            User = null;
            CMain.SetResolution(800, 600);

            ActiveScene = new LoginScene();
            Dispose();
            MirMessageBox.Show("The person you was observing has logged off.");
        }

        private void TimeOfDay(S.TimeOfDay p)
        {
            Lights = p.Lights;
        }
        private void ChangeAMode(S.ChangeAMode p)
        {
            AMode = p.Mode;

            switch (p.Mode)
            {
                case AttackMode.Peace:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_Peace, ChatType.Hint);
                    break;
                case AttackMode.Group:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_Group, ChatType.Hint);
                    break;
                case AttackMode.Guild:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_Guild, ChatType.Hint);
                    break;
                case AttackMode.EnemyGuild:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_EnemyGuild, ChatType.Hint);
                    break;
                case AttackMode.RedBrown:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_RedBrown, ChatType.Hint);
                    break;
                case AttackMode.All:
                    ChatDialog.ReceiveChat(GameLanguage.AttackMode_All, ChatType.Hint);
                    break;
            }
        }
        private void ChangePMode(S.ChangePMode p)
        {
            PMode = p.Mode;
            switch (p.Mode)
            {
                case PetMode.Both:
                    ChatDialog.ReceiveChat(GameLanguage.PetMode_Both, ChatType.Hint);
                    break;
                case PetMode.MoveOnly:
                    ChatDialog.ReceiveChat(GameLanguage.PetMode_MoveOnly, ChatType.Hint);
                    break;
                case PetMode.AttackOnly:
                    ChatDialog.ReceiveChat(GameLanguage.PetMode_AttackOnly, ChatType.Hint);
                    break;
                case PetMode.None:
                    ChatDialog.ReceiveChat(GameLanguage.PetMode_None, ChatType.Hint);
                    break;
                case PetMode.FocusMasterTarget:
                    ChatDialog.ReceiveChat(GameLanguage.PetMode_FocusMasterTarget, ChatType.Hint);
                    break;
            }
        }

        private void ObjectItem(S.ObjectItem p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
            /*
            string[] Warnings = new string[] {"HeroNecklace","AdamantineNecklace","8TrigramWheel","HangMaWheel","BaekTaGlove","SpiritReformer","BokMaWheel","BoundlessRing","ThunderRing","TaeGukRing","OmaSpiritRing","NobleRing"};
            if (Warnings.Contains(p.Name))
            {
                ChatDialog.ReceiveChat(string.Format("{0} at {1}", p.Name, p.Location), ChatType.Hint);
            }
            */
        }
        private void ObjectGold(S.ObjectGold p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
        }
        private void GainedItem(S.GainedItem p)
        {
            Bind(p.Item);
            AddItem(p.Item);
            User.RefreshStats();

            OutputMessage(string.Format(GameLanguage.YouGained, p.Item.FriendlyName));
        }
        private void GainedQuestItem(S.GainedQuestItem p)
        {
            Bind(p.Item);
            AddQuestItem(p.Item);
        }

        private void GainedGold(S.GainedGold p)
        {
            if (p.Gold == 0) return;

            Gold += p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
            OutputMessage(string.Format(GameLanguage.YouGained2, p.Gold, GameLanguage.Gold));
        }
        private void LoseGold(S.LoseGold p)
        {
            Gold -= p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
        }
        private void GainedCredit(S.GainedCredit p)
        {
            if (p.Credit == 0) return;

            Credit += p.Credit;
            SoundManager.PlaySound(SoundList.Gold);
            OutputMessage(string.Format(GameLanguage.YouGained2, p.Credit, GameLanguage.Credit));
        }
        private void LoseCredit(S.LoseCredit p)
        {
            Credit -= p.Credit;
            SoundManager.PlaySound(SoundList.Gold);
        }
        private void ObjectMonster(S.ObjectMonster p)
        {
            var found = false;
            var mob = (MonsterObject)MapControl.Objects.Find(ob => ob.ObjectID == p.ObjectID);
            if (mob != null)
                found = true;
            if (!found)
                mob = new MonsterObject(p.ObjectID);
            mob.Load(p, found);
        }
        private void ObjectAttack(S.ObjectAttack p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

            QueuedAction action = null;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                action = new QueuedAction { Action = MirAction.Attack1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.Spell);
                action.Params.Add(p.Level);
                ob.ActionFeed.Add(action);
                return;
            }
        }
        private void Struck(S.Struck p)
        {
            uint attackerID = p.AttackerID;
            User.StruckWeapon = -1;
            for (int j = 0; j < MapControl.Objects.Count; j++)
            {
                MapObject ob = MapControl.Objects[j];
                if (ob.ObjectID != attackerID) continue;
                if (ob.Race != ObjectType.Player) break;
                PlayerObject player = (PlayerObject)ob;
                User.StruckWeapon = player.Weapon;
                break;
            }
            User.PlayStruckSound();
        }
        private void ObjectStruck(S.ObjectStruck p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                uint attackerID = p.AttackerID;
                ob.StruckWeapon = -1;
                for (int j = 0; j < MapControl.Objects.Count; j++)
                {
                    MapObject ob2 = MapControl.Objects[j];
                    if (ob2.ObjectID != attackerID) continue;
                    if (ob2.Race != ObjectType.Player) break;
                    PlayerObject player = (PlayerObject)ob2;
                    ob.StruckWeapon = player.Weapon;
                    break;
                }
                ob.PlayStruckSound();

                return;
            }
        }

        private void DamageIndicator(S.DamageIndicator p)
        {
            if (Settings.DisplayDamage)
            {
                for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject obj = MapControl.Objects[i];
                    if (obj.ObjectID != p.ObjectID) continue;

                    if (obj.Damages.Count >= 10) return;

                    switch (p.Type)
                    {
                        case DamageType.Hit: //add damage level colours
                            obj.Damages.Add(new Damage(p.Damage.ToString("#,##0"), 1000, obj.Race == ObjectType.Player ? Color.Red : Color.White, 50));
                            break;
                        case DamageType.Miss:
                            obj.Damages.Add(new Damage("Miss", 1200, obj.Race == ObjectType.Player ? Color.LightCoral : Color.LightGray, 50));
                            break;
                        case DamageType.Critical:
                            obj.Damages.Add(new Damage("Crit", 1000, obj.Race == ObjectType.Player ? Color.DarkRed : Color.DarkRed, 50) { Offset = 15 });
                            break;
                    }
                }
            }
        }

        private void DuraChanged(S.DuraChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }


            if (item == null)
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }

            if (item == null) return;

            item.CurrentDura = p.CurrentDura;

            if (item.CurrentDura == 0)
            {
                User.RefreshStats();
                switch (item.Info.Type)
                {
                    default:
                        ChatDialog.ReceiveChat(string.Format("{0}'s dura has dropped to 0.", item.Info.FriendlyName), ChatType.System);
                        break;
                }
                
            }

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }
        private void HealthChanged(S.HealthChanged p)
        {
            User.HP = p.HP;
            User.MP = p.MP;

            User.PercentHealth = (byte)(User.HP / (float)User.Stats[Stat.HP] * 100);
        }

        private void DeleteQuestItem(S.DeleteQuestItem p)
        {
            for (int i = 0; i < User.QuestInventory.Length; i++)
            {
                UserItem item = User.QuestInventory[i];

                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.QuestInventory[i] = null;
                else
                    item.Count -= p.Count;
                break;
            } 
        }

        private void DeleteItem(S.DeleteItem p)
        {
            UserObject actor = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (actor != null) break;
                UserItem item = User.Inventory[i];

                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.Inventory[i] = null;
                else
                    item.Count -= p.Count;
                actor = User;
            }

            if (actor == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (actor != null) break;
                    UserItem item = User.Equipment[i];

                    if (item == null || item.UniqueID != p.UniqueID) continue;

                    if (item.Count == p.Count)
                        User.Equipment[i] = null;
                    else
                        item.Count -= p.Count;
                    actor = User;
                }
            }

            if (actor == null)
            {
                for (int i = 0; i < Storage.Length; i++)
                {
                    var item = Storage[i];
                    if (item == null || item.UniqueID != p.UniqueID) continue;

                    if (item.Count == p.Count)
                        Storage[i] = null;
                    else
                        item.Count -= p.Count;
                    break;
                }
            }
            actor?.RefreshStats();
        }
        private void Death(S.Death p)
        {
            User.Dead = true;

            User.ActionFeed.Add(new QueuedAction { Action = MirAction.Die, Direction = p.Direction, Location = p.Location });
            ShowReviveMessage = true;

            LogTime = 0;
        }
        private void ObjectDied(S.ObjectDied p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                switch(p.Type)
                {
                    default:
                        ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Die, Direction = p.Direction, Location = p.Location });
                        ob.Dead = true;
                        break;
                    case 1:
                        MapControl.Effects.Add(new Effect(Libraries.Magic2, 690, 10, 1000, ob.CurrentLocation));
                        ob.Remove();
                        break;
                }

                if (ob.ObjectID == MapObject.TargetObjectID)
                    MapObject.TargetObjectID = 0;
                return;
            }
        }
        private void ColourChanged(S.ColourChanged p)
        {
            User.NameColour = p.NameColour;
        }
        private void ObjectColourChanged(S.ObjectColourChanged p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.NameColour = p.NameColour;
                return;
            }
        }

        private void ObjectGuildNameChanged(S.ObjectGuildNameChanged p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                PlayerObject obPlayer = (PlayerObject)ob;
                obPlayer.GuildName = p.GuildName;
                return;
            }
        }
        private void GainExperience(S.GainExperience p)
        {
            OutputMessage(string.Format(GameLanguage.ExperienceGained, p.Amount));
            MapObject.User.Experience += p.Amount;
        }

        private void LevelChanged(S.LevelChanged p)
        {
            User.Level = p.Level;
            User.Experience = p.Experience;
            User.MaxExperience = p.MaxExperience;
            User.RefreshStats();
            OutputMessage(GameLanguage.LevelUp);
            User.Effects.Add(new Effect(Libraries.Magic2, 1200, 20, 2000, User));
            SoundManager.PlaySound(SoundList.LevelUp);
            ChatDialog.ReceiveChat(GameLanguage.LevelUp, ChatType.LevelUp); 
        }
        private void ObjectLeveled(S.ObjectLeveled p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Effects.Add(new Effect(Libraries.Magic2, 1180, 16, 2500, ob));
                SoundManager.PlaySound(SoundList.LevelUp);
                return;
            }
        }
        private void ObjectHarvest(S.ObjectHarvest p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Harvest, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void ObjectHarvested(S.ObjectHarvested p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                return;
            }
        }
        private void ObjectNPC(S.ObjectNPC p)
        {
            NPCObject ob = new NPCObject(p.ObjectID);
            ob.Load(p);
        }
        private void NPCResponse(S.NPCResponse p)
        {
            NPCTime = 0;
            NPCDialog.BigButtons.Clear();
            NPCDialog.BigButtonDialog.Hide();
            NPCDialog.NewText(p.Page);

            if (p.Page.Count > 0 || NPCDialog.BigButtons.Count > 0)
                NPCDialog.Show();
            else
                NPCDialog.Hide();

            NPCGoodsDialog.Hide();
            NPCSubGoodsDialog.Hide();
            NPCDropDialog.Hide();
            StorageDialog.Hide();
            RefineDialog.Hide();
            StorageDialog.Hide();
            TrustMerchantDialog.Hide();
            QuestListDialog.Hide();
        }

        private void NPCUpdate(S.NPCUpdate p)
        {
            GameScene.NPCID = p.NPCID; //Updates the client with the correct NPC ID if it's manually called from the client
        }

        private void NPCImageUpdate(S.NPCImageUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID || ob.Race != ObjectType.Merchant) continue;

                NPCObject npc = (NPCObject)ob;
                npc.Image = p.Image;
                npc.Colour = p.Colour;

                npc.LoadLibrary();
                return;
            }
        }
        private void DefaultNPC(S.DefaultNPC p)
        {
            GameScene.DefaultNPCID = p.ObjectID; //Updates the client with the correct Default NPC ID
        }


        private void ObjectHide(S.ObjectHide p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Hide, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void ObjectShow(S.ObjectShow p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Show, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void Poisoned(S.Poisoned p)
        {
            var previousPoisons = User.Poison;

            User.Poison = p.Poison;
            if (p.Poison.HasFlag(PoisonType.Stun) || p.Poison.HasFlag(PoisonType.Frozen) || p.Poison.HasFlag(PoisonType.Paralysis))
            {
                User.ClearMagic();
            }
        }

        private void ObjectPoisoned(S.ObjectPoisoned p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Poison = p.Poison;
                return;
            }
        }
        private void MapChanged(S.MapChanged p)
        {
            var isCurrentMap = (MapControl.Index == p.MapIndex);

            if (isCurrentMap)
                MapControl.ResetMap();
            else
            {
                MapControl.Index = p.MapIndex;
                MapControl.FileName = Path.Combine(Settings.MapPath, p.FileName + ".map");
                MapControl.Title = p.Title;
                MapControl.MiniMap = p.MiniMap;
                MapControl.BigMap = p.BigMap;
                MapControl.Lights = p.Lights;
                MapControl.MapDarkLight = p.MapDarkLight;
                MapControl.Music = p.Music;
                MapControl.LoadMap();
            }

            MapControl.NextAction = 0;
            Scene.MapControl.AutoPath = false;
            Scene.MainDialog.MapLabel.Text = p.Title;
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            User.Direction = p.Direction;

            User.QueuedAction = null;
            User.ActionFeed.Clear();
            User.ClearMagic();
            User.SetAction();

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;
        }
        private void ObjectTeleportOut(S.ObjectTeleportOut p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                Effect effect = null;

                bool playDefaultSound = true;

                switch (p.Type)
                {
                    case 1: //Yimoogi
                        {
                            effect = new Effect(Libraries.Magic2, 1300, 10, 500, ob.CurrentLocation);
                            break;
                        }
                    default:
                        {
                            effect = new Effect(Libraries.Magic, 250, 10, 500, ob.CurrentLocation);
                            break;
                        }
                }

                MapControl.Effects.Add(effect);

                return;
            }
        }
        private void ObjectTeleportIn(S.ObjectTeleportIn p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                bool playDefaultSound = true;

                switch (p.Type)
                {
                    default:
                        {
                            ob.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, ob));
                            break;
                        }
                }

                return;
            }
        }

        private void TeleportIn()
        {
            User.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, User));
        }
        private void NPCGoods(S.NPCGoods p)
        {
            for (int i = 0; i < p.List.Count; i++)
            {
                p.List[i].Info = GetInfo(p.List[i].ItemIndex);
            }

            NPCRate = p.Rate;
            HideAddedStoreStats = p.HideAddedStats;

            if (!NPCDialog.Visible) return;

            switch (p.Type)
            {
                case PanelType.Buy:
                    NPCGoodsDialog.NewGoods(p.List);
                    NPCGoodsDialog.Show();
                    break;
                case PanelType.BuySub:
                    NPCSubGoodsDialog.NewGoods(p.List);
                    NPCSubGoodsDialog.Show();
                    break;
            }
        }

        private void NPCSell()
        {
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Sell;
            NPCDropDialog.Show();
        }
        private void NPCRepair(S.NPCRepair p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Repair;
            NPCDropDialog.Show();
        }
        private void NPCStorage()
        {
            if (NPCDialog.Visible)
                StorageDialog.Show();
        }
        private void NPCRequestInput(S.NPCRequestInput p)
        {
            MirInputBox inputBox = new MirInputBox("Please enter the required information.");

            inputBox.OKButton.Click += (o1, e1) =>
            {
                Network.Enqueue(new C.NPCConfirmInput { Value = inputBox.InputTextBox.Text, NPCID = p.NPCID, PageName = p.PageName });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void NPCSRepair(S.NPCSRepair p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.SpecialRepair;
            NPCDropDialog.Show();
        }

        private void NPCRefine(S.NPCRefine p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Refine;
            if (p.Refining)
            {
                NPCDropDialog.Hide();
                NPCDialog.Hide();
            }
            else
                NPCDropDialog.Show();
        }

        private void NPCCheckRefine(S.NPCCheckRefine p)
        {
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.CheckRefine;
            NPCDropDialog.Show();
        }

        private void NPCCollectRefine(S.NPCCollectRefine p)
        {
            if (!NPCDialog.Visible) return;
            NPCDialog.Hide();
        }

        private void NPCReplaceWedRing(S.NPCReplaceWedRing p)
        {
            if (!NPCDialog.Visible) return;
            NPCRate = p.Rate;
            NPCDropDialog.PType = PanelType.ReplaceWedRing;
            NPCDropDialog.Show();
        }


        private void SellItem(S.SellItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;

            User.RefreshStats();
        }
        private void RepairItem(S.RepairItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;
        }
        private void ItemRepaired(S.ItemRepaired p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }
            }

            if (item == null) return;

            item.MaxDura = p.MaxDura;
            item.CurrentDura = p.CurrentDura;

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void ItemSealChanged(S.ItemSealChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }
            }

            if (item == null) return;

            item.SealedInfo = new SealedInfo { ExpiryDate = p.ExpiryDate };

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void ItemUpgraded(S.ItemUpgraded p)
        {
            UserItem item = null;
            MirGridType grid = MirGridType.Inventory;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.Item.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null) return;

            item.AddedStats.Clear();
            item.AddedStats.Add(p.Item.AddedStats);

            item.MaxDura = p.Item.MaxDura;
            item.RefineAdded = p.Item.RefineAdded;
            
            switch (grid)
            {
                case MirGridType.Inventory:
                    InventoryDialog.DisplayItemGridEffect(item.UniqueID, 0);
                    break;
            }
           

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void NewMagic(S.NewMagic p)
        {
            ClientMagic magic = p.Magic;

            UserObject actor = User;

            actor.Magics.Add(magic);
            actor.RefreshStats();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
            {
                Bar.Update();
            }
        }

        private void RemoveMagic(S.RemoveMagic p)
        {
            User.Magics.RemoveAt(p.PlaceId);
            User.RefreshStats();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
            {
                Bar.Update();
            }
        }

        private void MagicLeveled(S.MagicLeveled p)
        {
            for (int i = 0; i < User.Magics.Count; i++)
            {
                ClientMagic magic = User.Magics[i];
                if (magic.Spell != p.Spell) continue;

                if (magic.Level != p.Level)
                {
                    magic.Level = p.Level;
                    User.RefreshStats();
                }

                magic.Experience = p.Experience;
                break;
            }
        }
        private void Magic(S.Magic p)
        {
            User.Spell = p.Spell;
            User.Cast = p.Cast;
            User.TargetID = p.TargetID;
            User.TargetPoint = p.Target;
            User.SpellLevel = p.Level;
            User.SecondaryTargetIDs = p.SecondaryTargetIDs;

            if (!p.Cast) return;

            ClientMagic magic = User.GetMagic(p.Spell);
            magic.CastTime = CMain.Time;
        }

        private void MagicDelay(S.MagicDelay p)
        {
            ClientMagic magic;
            magic = User.GetMagic(p.Spell);
            magic.Delay = p.Delay;
        }

        private void MagicCast(S.MagicCast p)
        {
            ClientMagic magic = User.GetMagic(p.Spell);
            magic.CastTime = CMain.Time;
        }

        private void ObjectMagic(S.ObjectMagic p)
        {
            if (p.SelfBroadcast == false && p.ObjectID == User.ObjectID && !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                QueuedAction action = new QueuedAction { Action = MirAction.Spell, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.Spell);
                action.Params.Add(p.TargetID);
                action.Params.Add(p.Target);
                action.Params.Add(p.Cast);
                action.Params.Add(p.Level);
                action.Params.Add(p.SecondaryTargetIDs);

                ob.ActionFeed.Add(action);
                return;
            }
        }

        private void ObjectProjectile(S.ObjectProjectile p)
        {
            MapObject source = MapControl.GetObject(p.Source);

            if (source == null) return;
        }

        private void ObjectEffect(S.ObjectEffect p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                PlayerObject player;

                switch (p.Effect)
                {
                    case SpellEffect.Teleport:
                        ob.Effects.Add(new Effect(Libraries.Magic, 1600, 10, 600, ob));
                        break;
                    case SpellEffect.Healing:
                        SoundManager.PlaySound(20000 + (ushort)Spell.Healing * 10 + 1);
                        ob.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, ob));
                        break;
                    case SpellEffect.MagicShieldUp:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }
                        player.MagicShield = true;
                        player.Effects.Add(player.ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, ob) { Repeat = true });
                        break;
                    case SpellEffect.MagicShieldDown:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }
                        player.ShieldEffect = null;
                        player.MagicShield = false;
                        break;
                }

                return;
            }
        }

        private void RangeAttack(S.RangeAttack p)
        {
            User.TargetID = p.TargetID;
            User.TargetPoint = p.Target;
            User.Spell = p.Spell;
        }

        private void Pushed(S.Pushed p)
        {
            User.ActionFeed.Add(new QueuedAction { Action = MirAction.Pushed, Direction = p.Direction, Location = p.Location });
        }

        private void ObjectPushed(S.ObjectPushed p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Pushed, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void ObjectName(S.ObjectName p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Name = p.Name;
                return;
            }
        }
        private void UserStorage(S.UserStorage p)
        {
            if(Storage.Length != p.Storage.Length)
            {
                Array.Resize(ref Storage, p.Storage.Length);
            }

            Storage = p.Storage;

            for (int i = 0; i < Storage.Length; i++)
            {
                if (Storage[i] == null) continue;
                Bind(Storage[i]);
            }
        }
        private void SwitchGroup(S.SwitchGroup p)
        {
            GroupDialog.AllowGroup = p.AllowGroup;

            if (!p.AllowGroup && GroupDialog.GroupList.Count > 0)
                DeleteGroup();
        }

        private void DeleteGroup()
        {
            GroupDialog.GroupList.Clear();
            GroupDialog.GroupMembersMap.Clear();
            BigMapViewPort.PlayerLocations.Clear();
            ChatDialog.ReceiveChat("You have left the group.", ChatType.Group);
        }

        private void DeleteMember(S.DeleteMember p)
        {
            GroupDialog.GroupList.Remove(p.Name);
            GroupDialog.GroupMembersMap.Remove(p.Name);
            BigMapViewPort.PlayerLocations.Remove(p.Name);
            ChatDialog.ReceiveChat(string.Format("-{0} has left the group.", p.Name), ChatType.Group);
        }

        private void GroupInvite(S.GroupInvite p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("Do you want to group with {0}?", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.GroupInvite { AcceptInvite = true });
            }; 
            messageBox.NoButton.Click += (o, e) => Network.Enqueue(new C.GroupInvite { AcceptInvite = false });
            messageBox.Show();
        }
        private void AddMember(S.AddMember p)
        {
            GroupDialog.GroupList.Add(p.Name);
            ChatDialog.ReceiveChat(string.Format("-{0} has joined the group.", p.Name), ChatType.Group);
        }
        private void GroupMembersMap(S.GroupMembersMap p)
        {
            if (!GroupDialog.GroupMembersMap.ContainsKey(p.PlayerName))
                GroupDialog.GroupMembersMap.Add(p.PlayerName, p.PlayerMap);
            else
            {
                GroupDialog.GroupMembersMap.Remove(p.PlayerName);
                GroupDialog.GroupMembersMap.Add(p.PlayerName, p.PlayerMap);
            }
        }
        private void SendMemberLocation(S.SendMemberLocation p)
        {
            if (!BigMapViewPort.PlayerLocations.ContainsKey(p.MemberName))
                BigMapViewPort.PlayerLocations.Add(p.MemberName, p.MemberLocation);
            else
            {
                BigMapViewPort.PlayerLocations.Remove(p.MemberName);
                BigMapViewPort.PlayerLocations.Add(p.MemberName, p.MemberLocation);
            }
        }
        private void Revived()
        {
            User.SetAction();
            User.Dead = false;
            User.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, User));
        }
        private void ObjectRevived(S.ObjectRevived p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (p.Effect)
                {
                    ob.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, ob));
                }
                ob.Dead = false;
                ob.ActionFeed.Clear();
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Revive, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void SpellToggle(S.SpellToggle p)
        {
            UserObject actor = User;
            string prefix = string.Empty;

            switch (p.Spell)
            {
                //Warrior
                case Spell.Slaying:
                    actor.Slaying = p.CanUse;
                    break;
                case Spell.Thrusting:
                    actor.Thrusting = p.CanUse;
                    ChatDialog.ReceiveChat(prefix + (actor.Thrusting ? "Use Thrusting." : "Do not use Thrusting."), ChatType.Hint);
                    break;
                case Spell.HalfMoon:
                    actor.HalfMoon = p.CanUse;
                    ChatDialog.ReceiveChat(prefix + (actor.HalfMoon ? "Use HalfMoon." : "Do not use HalfMoon."), ChatType.Hint);
                    break;
                case Spell.FlamingSword:
                    actor.FlamingSword = p.CanUse;
                    if (actor.FlamingSword)
                        ChatDialog.ReceiveChat(prefix + GameLanguage.WeaponSpiritFire, ChatType.Hint);
                    else
                        ChatDialog.ReceiveChat(prefix + GameLanguage.SpiritsFireDisappeared, ChatType.System);
                    break;
            }
        }

        private void ObjectHealth(S.ObjectHealth p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.PercentHealth = p.Percent;
                ob.HealthTime = CMain.Time + p.Expire * 1000;
                return;
            }
        }

        private void ObjectMana(S.ObjectMana p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.PercentMana = p.Percent;
                return;
            }
        }

        private void MapEffect(S.MapEffect p)
        {
            switch (p.Effect)
            {
            }
        }

        private void ObjectRangeAttack(S.ObjectRangeAttack p)
        {
            if (p.ObjectID == User.ObjectID &&
                !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                QueuedAction action = new QueuedAction { Action = MirAction.AttackRange1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.TargetID);
                action.Params.Add(p.Target);
                action.Params.Add(p.Spell);
                action.Params.Add(new List<uint>());
                action.Params.Add(p.Level);

                ob.ActionFeed.Add(action);
                return;
            }
        }

        private void AddBuff(S.AddBuff p)
        {
            ClientBuff buff = p.Buff;

            if (!buff.Paused)
            {
                buff.ExpireTime += CMain.Time;
            }

            if (buff.ObjectID == User.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != buff.Type) continue;

                    BuffsDialog.Buffs[i] = buff;
                    User.RefreshStats();
                    return;
                }

                BuffsDialog.Buffs.Add(buff);
                BuffsDialog.CreateBuff(buff);

                User.RefreshStats();     
            }

            if (!buff.Visible || buff.ObjectID <= 0) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != buff.ObjectID) continue;
                if ((ob is PlayerObject) || (ob is MonsterObject))
                {
                    if (!ob.Buffs.Contains(buff.Type))
                    {
                        ob.Buffs.Add(buff.Type);
                    }

                    ob.AddBuffEffect(buff.Type);
                    return;
                }
            }
        }

        private void RemoveBuff(S.RemoveBuff p)
        {
            if (User.ObjectID == p.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != p.Type) continue;

                    BuffsDialog.RemoveBuff(i);
                    BuffsDialog.Buffs.RemoveAt(i);
                }
                User.RefreshStats();
            }

            if (p.ObjectID <= 0) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];

                if (ob.ObjectID != p.ObjectID) continue;

                ob.Buffs.Remove(p.Type);
                ob.RemoveBuffEffect(p.Type);
                return;
            }
        }

        private void PauseBuff(S.PauseBuff p)
        {
            if (User.ObjectID == p.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != p.Type) continue;

                    User.RefreshStats();

                    if (BuffsDialog.Buffs[i].Paused == p.Paused) return;

                    BuffsDialog.Buffs[i].Paused = p.Paused;

                    if (p.Paused)
                    {
                        BuffsDialog.Buffs[i].ExpireTime -= CMain.Time;
                    }
                    else
                    {
                        BuffsDialog.Buffs[i].ExpireTime += CMain.Time;
                    }
                }
            }
        }

        private void ObjectHidden(S.ObjectHidden p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Hidden = p.Hidden;
                return;
            }
        }

        private void ObjectLevelEffects(S.ObjectLevelEffects p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID || ob.Race != ObjectType.Player) continue;

                PlayerObject temp = (PlayerObject)ob;

                temp.LevelEffects = p.LevelEffects;

                temp.SetEffects();
                return;
            }
        }

        private void RefreshItem(S.RefreshItem p)
        {
            Bind(p.Item);

            if (SelectedCell != null && SelectedCell.Item.UniqueID == p.Item.UniqueID)
                SelectedCell = null;

            if (HoverItem != null && HoverItem.UniqueID == p.Item.UniqueID)
            {
                DisposeItemLabel();
                CreateItemLabel(p.Item);
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.Item.UniqueID)
                {
                    User.Inventory[i] = p.Item;
                    User.RefreshStats();
                    return;
                }
            }

            for (int i = 0; i < User.Equipment.Length; i++)
            {
                if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.Item.UniqueID)
                {
                    User.Equipment[i] = p.Item;
                    User.RefreshStats();
                    return;
                }
            }
        }

        private void ObjectSpell(S.ObjectSpell p)
        {
            SpellObject ob = new SpellObject(p.ObjectID);
            ob.Load(p);
        }

        private void UserDash(S.UserDash p)
        {
            if (User.Direction == p.Direction && User.CurrentLocation == p.Location)
            {
                MapControl.NextAction = 0;
                return;
            }
            MirAction action = User.CurrentAction == MirAction.DashL ? MirAction.DashR : MirAction.DashL;
            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == MirAction.DashR)
                {
                    action = MirAction.DashL;
                    break;
                }
                if (User.ActionFeed[i].Action == MirAction.DashL)
                {
                    action = MirAction.DashR;
                    break;
                }
            }

            User.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });
        }

        private void UserDashFail(S.UserDashFail p)
        {
            MapControl.NextAction = 0;
            User.ActionFeed.Add(new QueuedAction { Action = MirAction.DashFail, Direction = p.Direction, Location = p.Location });
        }

        private void ObjectDash(S.ObjectDash p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                MirAction action = MirAction.DashL;

                if (ob.ActionFeed.Count > 0 && ob.ActionFeed[ob.ActionFeed.Count - 1].Action == action)
                    action = MirAction.DashR;

                ob.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void ObjectDashFail(S.ObjectDashFail p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.DashFail, Direction = p.Direction, Location = p.Location });

                return;
            }
        }
        private void SendOutputMessage(S.SendOutputMessage p)
        {
            OutputMessage(p.Message, p.Type);
        }

        private void NPCConsign()
        {
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Consign;
            NPCDropDialog.Show();
        }
        private void NPCMarket(S.NPCMarket p)
        {
            for (int i = 0; i < p.Listings.Count; i++)
                Bind(p.Listings[i].Item);

            TrustMerchantDialog.Show();
            TrustMerchantDialog.UserMode = p.UserMode;
            TrustMerchantDialog.Listings = p.Listings;
            TrustMerchantDialog.Page = 0;
            TrustMerchantDialog.PageCount = p.Pages;
            TrustMerchantDialog.UpdateInterface();
        }
        private void NPCMarketPage(S.NPCMarketPage p)
        {
            if (!TrustMerchantDialog.Visible) return;

            for (int i = 0; i < p.Listings.Count; i++)
                Bind(p.Listings[i].Item);

            TrustMerchantDialog.Listings.AddRange(p.Listings);
            TrustMerchantDialog.Page = (TrustMerchantDialog.Listings.Count - 1) / 10;
            TrustMerchantDialog.UpdateInterface();
        }
        private void ConsignItem(S.ConsignItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            cell.Item = null;

            User.RefreshStats();
        }
        private void MarketFail(S.MarketFail p)
        {
            TrustMerchantDialog.MarketTime = 0;
            switch (p.Reason)
            {
                case 0:
                    MirMessageBox.Show("You cannot use the TrustMerchant when dead.");
                    break;
                case 1:
                    MirMessageBox.Show("You cannot buy from the TrustMerchant without using.");
                    break;
                case 2:
                    MirMessageBox.Show("This item has already been sold.");
                    break;
                case 3:
                    MirMessageBox.Show("This item has Expired and cannot be brought.");
                    break;
                case 4:
                    MirMessageBox.Show(GameLanguage.LowGold);
                    break;
                case 5:
                    MirMessageBox.Show("You do not have enough weight or space spare to buy this item.");
                    break;
                case 6:
                    MirMessageBox.Show("You cannot buy your own items.");
                    break;
                case 7:
                    MirMessageBox.Show("You are too far away from the Trust Merchant.");
                    break;
                case 8:
                    MirMessageBox.Show("You cannot hold enough gold to get your sale.");
                    break;
                case 9:
                    MirMessageBox.Show("This item has not met the minimum bid yet.");
                    break;
                case 10:
                    MirMessageBox.Show("Auction has already ended for this item.");
                    break;
            }

        }
        private void MarketSuccess(S.MarketSuccess p)
        {
            TrustMerchantDialog.MarketTime = 0;
            MirMessageBox.Show(p.Message);
        }

        private void BaseStatsInfo(S.BaseStatsInfo p)
        {
            User.CoreStats = p.Stats;
            User.RefreshStats();
        }

        private void UserName(S.UserName p)
        {
            for (int i = 0; i < UserIdList.Count; i++)
                if (UserIdList[i].Id == p.Id)
                {
                    UserIdList[i].UserName = p.Name;
                    break;
                }
            DisposeItemLabel();
            HoverItem = null;
        }

        private void ChatItemStats(S.ChatItemStats p)
        {
            //for (int i = 0; i < ChatItemList.Count; i++)
            //    if (ChatItemList[i].ID == p.ChatItemId)
            //    {
            //        ChatItemList[i].ItemStats = p.Stats;
            //        ChatItemList[i].RecievedTick = CMain.Time;
            //    }
        }

        private void GuildInvite(S.GuildInvite p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("Do you want to join the {0} guild?", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.GuildInvite { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => Network.Enqueue(new C.GuildInvite { AcceptInvite = false });

            messageBox.Show();
        }

        private void GuildNameRequest(S.GuildNameRequest p)
        {
            MirInputBox inputBox = new MirInputBox("Please enter a guild name, length must be 3~20 characters.");
            inputBox.InputTextBox.TextBox.KeyPress += (o, e) =>
            {
                string Allowed = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (!Allowed.Contains(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            };
            inputBox.OKButton.Click += (o, e) =>
            {
                if (inputBox.InputTextBox.Text.Contains('\\'))
                {
                    ChatDialog.ReceiveChat("You cannot use the \\ sign in a guildname!", ChatType.System);
                    inputBox.InputTextBox.Text = "";
                }
                Network.Enqueue(new C.GuildNameReturn { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void GuildRequestWar(S.GuildRequestWar p)
        {
            MirInputBox inputBox = new MirInputBox("Please enter the guild you would like to go to war with.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.GuildWarReturn { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void GuildNoticeChange(S.GuildNoticeChange p)
        {
            if (p.update == -1)
                GuildDialog.NoticeChanged = true;
            else
                GuildDialog.NoticeChange(p.notice);
        }
        private void GuildMemberChange(S.GuildMemberChange p)
        {
            switch (p.Status)
            {
                case 0: // logged of
                    GuildDialog.MemberStatusChange(p.Name, false);
                    break;
                case 1: // logged on
                    ChatDialog.ReceiveChat(String.Format("{0} logged on.", p.Name), ChatType.Guild);
                    GuildDialog.MemberStatusChange(p.Name, true);
                    break;
                case 2://new member
                    ChatDialog.ReceiveChat(String.Format("{0} joined guild.", p.Name), ChatType.Guild);
                    GuildDialog.MemberCount++;
                    GuildDialog.MembersChanged = true;
                    break;
                case 3://kicked member
                    ChatDialog.ReceiveChat(String.Format("{0} got removed from the guild.", p.Name), ChatType.Guild);
                    GuildDialog.MembersChanged = true;
                    break;
                case 4://member left
                    ChatDialog.ReceiveChat(String.Format("{0} left the guild.", p.Name), ChatType.Guild);
                    GuildDialog.MembersChanged = true;
                    break;
                case 5://rank change (name or different rank)
                    GuildDialog.MembersChanged = true;
                    break;
                case 6: //new rank
                    if (p.Ranks.Count > 0)
                        GuildDialog.NewRankRecieved(p.Ranks[0]);
                    break;
                case 7: //rank option changed
                    if (p.Ranks.Count > 0)
                        GuildDialog.RankChangeRecieved(p.Ranks[0]);
                    break;
                case 8: //my rank changed
                    if (p.Ranks.Count > 0)
                        GuildDialog.MyRankChanged(p.Ranks[0]);
                    break;
                case 255:
                    GuildDialog.NewMembersList(p.Ranks);
                    break;
            }
        }

        private void GuildStatus(S.GuildStatus p)
        {
            if ((User.GuildName == "") && (p.GuildName != ""))
            {
                GuildDialog.NoticeChanged = true;
                GuildDialog.MembersChanged = true;
            }
            if (p.GuildName == "")
            {
                GuildDialog.Hide();
            }

            if ((User.GuildName == p.GuildName) && (GuildDialog.Level < p.Level))
            {
                //guild leveled
            }
            bool GuildChange = User.GuildName != p.GuildName;
            User.GuildName = p.GuildName;
            User.GuildRankName = p.GuildRankName;
            GuildDialog.Level = p.Level;
            GuildDialog.Experience = p.Experience;
            GuildDialog.MaxExperience = p.MaxExperience;
            GuildDialog.Gold = p.Gold;
            GuildDialog.SparePoints = p.SparePoints;
            GuildDialog.MemberCount = p.MemberCount;
            GuildDialog.MaxMembers = p.MaxMembers;
            GuildDialog.Voting = p.Voting;
            GuildDialog.ItemCount = p.ItemCount;
            GuildDialog.BuffCount = p.BuffCount;
            GuildDialog.StatusChanged(p.MyOptions);
            GuildDialog.MyRankId = p.MyRankId;
            GuildDialog.UpdateMembers();
            //reset guildbuffs
            if (GuildChange)
            {
                GuildDialog.EnabledBuffs.Clear();
                GuildDialog.UpdateActiveStats();
                RemoveBuff(new S.RemoveBuff { ObjectID = User.ObjectID, Type = BuffType.Guild });
                User.RefreshStats();
            }
        }

        private void GuildExpGain(S.GuildExpGain p)
        {
            //OutputMessage(string.Format("Guild Experience Gained {0}.", p.Amount));
            GuildDialog.Experience += p.Amount;
        }

        private void GuildStorageGoldChange(S.GuildStorageGoldChange p)
        {
            switch (p.Type)
            {
                case 0:
                    ChatDialog.ReceiveChat(String.Format("{0} donated {1} gold to guild funds.", p.Name, p.Amount), ChatType.Guild);
                    GuildDialog.Gold += p.Amount;
                    break;
                case 1:
                    ChatDialog.ReceiveChat(String.Format("{0} retrieved {1} gold from guild funds.", p.Name, p.Amount), ChatType.Guild);
                    if (GuildDialog.Gold > p.Amount)
                        GuildDialog.Gold -= p.Amount;
                    else
                        GuildDialog.Gold = 0;
                    break;
                case 2:
                    if (GuildDialog.Gold > p.Amount)
                        GuildDialog.Gold -= p.Amount;
                    else
                        GuildDialog.Gold = 0;
                    break;
                case 3:
                    GuildDialog.Gold += p.Amount;
                    break;
            }
        }

        private void GuildStorageItemChange(S.GuildStorageItemChange p)
        {
            MirItemCell fromCell = null;
            MirItemCell toCell = null;
            switch (p.Type)
            {
                case 0://store
                    toCell = GuildDialog.StorageGrid[p.To];

                    if (toCell == null) return;

                    toCell.Locked = false;
                    toCell.Item = p.Item.Item;
                    Bind(toCell.Item);
                    if (p.User != User.Id) return;
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];
                    fromCell.Locked = false;
                    if (fromCell != null)
                        fromCell.Item = null;
                    User.RefreshStats();
                    break;
                case 1://retrieve
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (fromCell == null) return;
                    fromCell.Locked = false;

                    if (p.User != User.Id)
                    {
                        fromCell.Item = null;
                        return;
                    }
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];
                    if (toCell == null) return;
                    toCell.Locked = false;
                    toCell.Item = fromCell.Item;
                    fromCell.Item = null;
                    break;

                case 2:
                    toCell = GuildDialog.StorageGrid[p.To];
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    fromCell.Item = toCell.Item;
                    toCell.Item = p.Item.Item;
                    
                    Bind(toCell.Item);
                    if (fromCell.Item != null) 
                        Bind(fromCell.Item);
                    break;
                case 3://failstore
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : InventoryDialog.Grid[p.From - User.BeltIdx];
                    toCell = GuildDialog.StorageGrid[p.To];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    break;
                case 4://failretrieve
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : InventoryDialog.Grid[p.To - User.BeltIdx];
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    break;
                case 5://failmove
                    fromCell = GuildDialog.StorageGrid[p.To];
                    toCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    GuildDialog.StorageGrid[p.From].Locked = false;
                    GuildDialog.StorageGrid[p.To].Locked = false;
                    break;
            }
        }
        private void GuildStorageList(S.GuildStorageList p)
        {
            for (int i = 0; i < p.Items.Length; i++)
            {
                if (i >= GuildDialog.StorageGrid.Length) break;
                if (p.Items[i] == null)
                {
                    GuildDialog.StorageGrid[i].Item = null;
                    continue;
                }
                GuildDialog.StorageGrid[i].Item = p.Items[i].Item;
                Bind(GuildDialog.StorageGrid[i].Item);
            }
        }

        private void MarriageRequest(S.MarriageRequest p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("{0} has asked for your hand in marriage.", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.MarriageReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.Enqueue(new C.MarriageReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }

        private void DivorceRequest(S.DivorceRequest p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("{0} has requested a divorce", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.DivorceReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.Enqueue(new C.DivorceReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }

        private bool UpdateGuildBuff(GuildBuff buff, bool Remove = false)
        {
            for (int i = 0; i < GuildDialog.EnabledBuffs.Count; i++)
            {
                if (GuildDialog.EnabledBuffs[i].Id == buff.Id)
                {
                    if (Remove)
                    {
                        GuildDialog.EnabledBuffs.RemoveAt(i);
                    }
                    else
                        GuildDialog.EnabledBuffs[i] = buff;
                    return true;
                }
            }
            return false;
        }

        private void GuildBuffList(S.GuildBuffList p)
        {
            //getting the list of all guildbuffs on server?
            if (p.GuildBuffs.Count > 0)
                GuildDialog.GuildBuffInfos.Clear();
            for (int i = 0; i < p.GuildBuffs.Count; i++)
            {
                GuildDialog.GuildBuffInfos.Add(p.GuildBuffs[i]);
            }
            //getting the list of all active/removedbuffs?
            for (int i = 0; i < p.ActiveBuffs.Count; i++)
            {
                //if (p.ActiveBuffs[i].ActiveTimeRemaining > 0)
                //    p.ActiveBuffs[i].ActiveTimeRemaining = Convert.ToInt32(CMain.Time / 1000) + (p.ActiveBuffs[i].ActiveTimeRemaining * 60);
                if (UpdateGuildBuff(p.ActiveBuffs[i], p.Remove == 1)) continue;
                if (!(p.Remove == 1))
                {
                    GuildDialog.EnabledBuffs.Add(p.ActiveBuffs[i]);
                    //CreateGuildBuff(p.ActiveBuffs[i]);
                }
            }

            for (int i = 0; i < GuildDialog.EnabledBuffs.Count; i++)
            {
                if (GuildDialog.EnabledBuffs[i].Info == null)
                {
                    GuildDialog.EnabledBuffs[i].Info = GuildDialog.FindGuildBuffInfo(GuildDialog.EnabledBuffs[i].Id);
                }
            }

            ClientBuff buff = BuffsDialog.Buffs.FirstOrDefault(e => e.Type == BuffType.Guild);

            if (GuildDialog.EnabledBuffs.Any(e => e.Active))
            {
                if (buff == null)
                {
                    buff = new ClientBuff { Type = BuffType.Guild, ObjectID = User.ObjectID, Caster = "Guild", Infinite = true, Values = new int[0] };

                    BuffsDialog.Buffs.Add(buff);
                    BuffsDialog.CreateBuff(buff);
                }

                GuildDialog.UpdateActiveStats();
            }
            else
            {
                RemoveBuff(new S.RemoveBuff { ObjectID = User.ObjectID, Type = BuffType.Guild });
            }

            User.RefreshStats();
        }

        private void TradeRequest(S.TradeRequest p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("Player {0} has requested to trade with you.", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.TradeReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.Enqueue(new C.TradeReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }
        private void TradeAccept(S.TradeAccept p)
        {
            GuestTradeDialog.GuestName = p.Name;
            TradeDialog.TradeAccept();
        }
        private void TradeGold(S.TradeGold p)
        {
            GuestTradeDialog.GuestGold = p.Amount;
            TradeDialog.ChangeLockState(false);
            TradeDialog.RefreshInterface();
        }
        private void TradeItem(S.TradeItem p)
        {
            GuestTradeDialog.GuestItems = p.TradeItems;
            TradeDialog.ChangeLockState(false);
            TradeDialog.RefreshInterface();
        }
        private void TradeConfirm()
        {
            TradeDialog.TradeReset();
        }
        private void TradeCancel(S.TradeCancel p)
        {
            if (p.Unlock)
            {
                TradeDialog.ChangeLockState(false);
            }
            else
            {
                TradeDialog.TradeReset();

                MirMessageBox messageBox = new MirMessageBox("Deal cancelled.\r\nTo deal correctly you must face the other party.", MirMessageBoxButtons.OK);
                messageBox.Show();
            }
        }

        private void ReceiveMail(S.ReceiveMail p)
        {
            NewMail = false;
            NewMailCounter = 0;
            User.Mail.Clear();

            User.Mail = p.Mail.OrderByDescending(e => !e.Locked).ThenByDescending(e => e.DateSent).ToList();

            foreach(ClientMail mail in User.Mail)
            {
                foreach(UserItem itm in mail.Items)
                {
                    Bind(itm);
                }
            }

            //display new mail received
            if (User.Mail.Any(e => e.Opened == false))
            {
                NewMail = true;
            }

            GameScene.Scene.MailListDialog.UpdateInterface();
        }

        private void MailLockedItem(S.MailLockedItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID);
            if (cell != null)
                cell.Locked = p.Locked;
        }

        private void MailSendRequest(S.MailSendRequest p)
        {
            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you would like to mail.");

            inputBox.OKButton.Click += (o1, e1) =>
            {
                GameScene.Scene.MailComposeParcelDialog.ComposeMail(inputBox.InputTextBox.Text);
                GameScene.Scene.InventoryDialog.Show();

                //open letter dialog, pass in name
                inputBox.Dispose();
            };

            inputBox.Show();
        }

        private void MailSent(S.MailSent p)
        {
            for (int i = 0; i < InventoryDialog.Grid.Length; i++)
            {
                if (InventoryDialog.Grid[i].Locked == true)
                {
                    InventoryDialog.Grid[i].Locked = false;
                }
            }

            for (int i = 0; i < BeltDialog.Grid.Length; i++)
            {
                if (BeltDialog.Grid[i].Locked == true)
                {
                    BeltDialog.Grid[i].Locked = false;
                }
            }

            GameScene.Scene.MailComposeParcelDialog.Hide();
        }

        private void ParcelCollected(S.ParcelCollected p)
        {
            switch(p.Result)
            {
                case -1:
                    MirMessageBox messageBox = new MirMessageBox(string.Format("No parcels to collect."), MirMessageBoxButtons.OK);
                    messageBox.Show();
                    break;
                case 0:
                    messageBox = new MirMessageBox(string.Format("All parcels have been collected."), MirMessageBoxButtons.OK);
                    messageBox.Show();
                    break;
                case 1:
                    GameScene.Scene.MailReadParcelDialog.Hide();
                    break;
            }
        }

        private void ResizeInventory(S.ResizeInventory p)
        {
            Array.Resize(ref User.Inventory, p.Size);
        }

        private void ResizeStorage(S.ResizeStorage p)
        {
            Array.Resize(ref Storage, p.Size);
            User.HasExpandedStorage = p.HasExpandedStorage;
            User.ExpandedStorageExpiryTime = p.ExpiryTime;

            //StorageDialog.RefreshStorage2();
        }

        private void MailCost(S.MailCost p)
        {
            if(GameScene.Scene.MailComposeParcelDialog.Visible)
            {
                if (p.Cost > 0)
                    SoundManager.PlaySound(SoundList.Gold);

                GameScene.Scene.MailComposeParcelDialog.ParcelCostLabel.Text = p.Cost.ToString();
            }
        }

        public void AddQuestItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.QuestInventory.Length; i++)
                {
                    UserItem temp = User.QuestInventory[i];
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

            for (int i = 0; i < User.QuestInventory.Length; i++)
            {
                if (User.QuestInventory[i] != null) continue;
                User.QuestInventory[i] = item;
                return;
            }
        }

        private void FriendUpdate(S.FriendUpdate p)
        {
            GameScene.Scene.FriendDialog.Friends = p.Friends;

            if (GameScene.Scene.FriendDialog.Visible)
            {
                GameScene.Scene.FriendDialog.Update(false);
            }
        }

        private void LoverUpdate(S.LoverUpdate p)
        {
            GameScene.Scene.RelationshipDialog.LoverName = p.Name;
            GameScene.Scene.RelationshipDialog.Date = p.Date;
            GameScene.Scene.RelationshipDialog.MapName = p.MapName;
            GameScene.Scene.RelationshipDialog.MarriedDays = p.MarriedDays;
            GameScene.Scene.RelationshipDialog.UpdateInterface();
        }

        private void GameShopUpdate(S.GameShopInfo p)
        {
            p.Item.Stock = p.StockLevel;
            GameShopInfoList.Add(p.Item);
            if (p.Item.Date > CMain.Now.AddDays(-7)) GameShopDialog.New.Visible = true;
        }

        private void GameShopStock(S.GameShopStock p)
        {
            for (int i = 0; i < GameShopInfoList.Count; i++)
            {
                if (GameShopInfoList[i].GIndex == p.GIndex)
                    {
                    if (p.StockLevel == 0) GameShopInfoList.Remove(GameShopInfoList[i]);
                    else GameShopInfoList[i].Stock = p.StockLevel;

                    if (GameShopDialog.Visible) GameShopDialog.UpdateShop();
                    }
            }
        }
        public void AddItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.Inventory.Length; i++)
                {
                    UserItem temp = User.Inventory[i];
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
                for (int i = 0; i < User.BeltIdx - 2; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }
            else
            {
                for (int i = User.BeltIdx; i < User.Inventory.Length; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        public static void Bind(UserItem item)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                if (ItemInfoList[i].Index != item.ItemIndex) continue;

                item.Info = ItemInfoList[i];

                return;
            }
        }

        public ItemInfo GetItemInfo(int index)
        {
            for (var i = 0; i < ItemInfoList.Count; i++)
            {
                var info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }
            return null;
        }

        public static void BindQuest(ClientQuestProgress quest)
        {
            for (int i = 0; i < QuestInfoList.Count; i++)
            {
                if (QuestInfoList[i].Index != quest.Id) continue;

                quest.QuestInfo = QuestInfoList[i];

                return;
            }
        }

        public Color GradeNameColor(ItemGrade grade)
        {
            switch (grade)
            {
                case ItemGrade.Common:
                    return Color.Yellow;
                case ItemGrade.YangWarrior:
                    return Color.DeepSkyBlue;
                case ItemGrade.NegativeWarrior:
                    return Color.DarkOrange;
                case ItemGrade.FireWizard:
                    return Color.Plum;
                case ItemGrade.CelestialWizard:
                    return Color.Red;
                case ItemGrade.MonkTaoist:
                    return Color.Red;
                case ItemGrade.InstructorTaoist:
                    return Color.Red;
                default:
                    return Color.Yellow;
            }
        }

        public void DisposeItemLabel()
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Dispose();
            ItemLabel = null;
        }
        public void DisposeMailLabel()
        {
            if (MailLabel != null && !MailLabel.IsDisposed)
                MailLabel.Dispose();
            MailLabel = null;
        }
        public void DisposeMemoLabel()
        {
            if (MemoLabel != null && !MemoLabel.IsDisposed)
                MemoLabel.Dispose();
            MemoLabel = null;
        }
        public void DisposeGuildBuffLabel()
        {
            if (GuildBuffLabel != null && !GuildBuffLabel.IsDisposed)
                GuildBuffLabel.Dispose();
            GuildBuffLabel = null;
        }

        public MirControl NameInfoLabel(UserItem item, bool inspect = false, bool hideDura = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            string GradeString = "";
            switch (HoverItem.Info.Grade)
            {
                case ItemGrade.None:                   
                    break;
                case ItemGrade.Common:
                    GradeString = GameLanguage.ItemGradeCommon;
                    break;
                case ItemGrade.YangWarrior:
                    GradeString = GameLanguage.ItemGradeYangWarrior;
                    break;
                case ItemGrade.NegativeWarrior:
                    GradeString = GameLanguage.ItemGradeNegativeWarrior;
                    break;
                case ItemGrade.FireWizard:
                    GradeString = GameLanguage.ItemGradeFireWizard;
                    break;
                case ItemGrade.CelestialWizard:
                    GradeString = GameLanguage.ItemGradeCelestialWizard;
                    break;
                case ItemGrade.MonkTaoist:
                    GradeString = GameLanguage.ItemGradeMonkTaoist;
                    break;
                case ItemGrade.InstructorTaoist:
                    GradeString = GameLanguage.ItemGradeInstructorTaoist;
                    break;
            }
            MirLabel nameLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = GradeNameColor(HoverItem.Info.Grade),
                Location = new Point(4, 4),
                OutLine = true,
                Parent = ItemLabel,
                Text = HoverItem.Info.Grade != ItemGrade.None ? string.Format("{0}{1}{2}", HoverItem.Info.FriendlyName, "\n", GradeString) : HoverItem.Info.FriendlyName,
            };

            if (HoverItem.RefineAdded > 0)
                nameLabel.Text = "(*)" + nameLabel.Text;

            ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, nameLabel.DisplayRectangle.Right + 4),
                Math.Max(ItemLabel.Size.Height, nameLabel.DisplayRectangle.Bottom));

            string text = "";

            if (HoverItem.Info.Durability > 0 && !hideDura)
            {
                switch (HoverItem.Info.Type)
                {
                    case ItemType.Ore:
                        text += string.Format(" Purity {0}", Math.Floor(HoverItem.CurrentDura / 1000M));
                        break;
                    case ItemType.Meat:
                        text += string.Format(" Quality {0}", Math.Floor(HoverItem.CurrentDura / 1000M));
                        break;
                    case ItemType.Gem:
                    case ItemType.Potion:
                        break;
                    default:
                        text += string.Format(" {0} {1}/{2}", GameLanguage.Durability, Math.Floor(HoverItem.CurrentDura / 1000M),
                                                   Math.Floor(HoverItem.MaxDura / 1000M));
                        break;
                }
            }

            string baseText = "";
            switch (HoverItem.Info.Type)
            {
                case ItemType.Nothing:
                    break;
                case ItemType.Weapon:
                    baseText = GameLanguage.ItemTypeWeapon;
                    break;
                case ItemType.Armour:
                    baseText = GameLanguage.ItemTypeArmour;
                    break;
                case ItemType.Helmet:
                    baseText = GameLanguage.ItemTypeHelmet;
                    break;
                case ItemType.Necklace:
                    baseText = GameLanguage.ItemTypeNecklace;
                    break;
                case ItemType.Bracelet:
                    baseText = GameLanguage.ItemTypeBracelet;
                    break;
                case ItemType.Ring:
                    baseText = GameLanguage.ItemTypeRing;
                    break;
                case ItemType.Potion:
                    baseText = GameLanguage.ItemTypePotion;
                    break;
                case ItemType.Ore:
                    baseText = GameLanguage.ItemTypeOre;
                    break;
                case ItemType.Meat:
                    baseText = GameLanguage.ItemTypeMeat;
                    break;
                case ItemType.Material:
                    baseText = GameLanguage.ItemTypeMaterial;
                    break;
                case ItemType.Scroll:
                    baseText = GameLanguage.ItemTypeScroll;
                    break;
                case ItemType.Gem:
                    baseText = GameLanguage.ItemTypeGem;
                    break;
                case ItemType.Book:
                    baseText = GameLanguage.ItemTypeBook;
                    break;
                case ItemType.Script:
                    baseText = GameLanguage.ItemTypeScript;
                    break;
                case ItemType.Quest:
                    baseText = GameLanguage.ItemTypeQuest;
                    break;
            }

            if (HoverItem.WeddingRing != -1)
            {
                baseText = GameLanguage.WeddingRing;
            }

            baseText = string.Format(GameLanguage.ItemTextFormat, baseText, string.IsNullOrEmpty(baseText) ? "" : "\n", GameLanguage.Weight, HoverItem.Weight + text);

            MirLabel etcLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Location = new Point(4, nameLabel.DisplayRectangle.Bottom),
                OutLine = true,
                Parent = ItemLabel,
                Text = baseText
            };

            ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, etcLabel.DisplayRectangle.Right + 4),
                Math.Max(ItemLabel.Size.Height, etcLabel.DisplayRectangle.Bottom + 4));

            #region OUTLINE
            MirControl outLine = new MirControl
            {
                BackColour = Color.FromArgb(255, 50, 50, 50),
                Border = true,
                BorderColour = Color.Gray,
                NotControl = true,
                Parent = ItemLabel,
                Opacity = 0.4F,
                Location = new Point(0, 0)
            };
            outLine.Size = ItemLabel.Size;
            #endregion

            return outLine;
        }
        public MirControl AttackInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;
            string text = "";

            #region Dura gem
            minValue = realItem.Durability;

            if (minValue > 0 && realItem.Type == ItemType.Gem)
            {
                switch (realItem.Shape)
                {
                    default:
                        text = string.Format("Adds +{0} Durability", minValue / 1000);
                        break;
                    case 8:
                        text = string.Format("Seals for {0}", Functions.PrintTimeSpanFromSeconds(minValue * 60));
                        break;
                }

                count++;
                MirLabel DuraLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DuraLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DuraLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DC
            minValue = realItem.Stats[Stat.MinDC];
            maxValue = realItem.Stats[Stat.MaxDC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxDC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.DC : GameLanguage.DC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} DC", minValue + maxValue + addValue);
                MirLabel DCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MC

            minValue = realItem.Stats[Stat.MinMC];
            maxValue = realItem.Stats[Stat.MaxMC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxMC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.MC : GameLanguage.MC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} MC", minValue + maxValue + addValue);
                MirLabel MCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SC

            minValue = realItem.Stats[Stat.MinSC];
            maxValue = realItem.Stats[Stat.MaxSC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxSC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.SC : GameLanguage.SC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} SC", minValue + maxValue + addValue);
                MirLabel SCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("SC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region LUCK / SUCCESS

            minValue = realItem.Stats[Stat.Luck];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Luck] : 0;

            if (minValue != 0 || addValue != 0)
            {
                count++;

                if (realItem.Type == ItemType.Potion && realItem.Shape == 4)
                {
                    text = string.Format("Exp + {0}% ", minValue + addValue);
                }
                else if (realItem.Type == ItemType.Potion && realItem.Shape == 5)
                {
                    text = string.Format("Drop + {0}% ", minValue + addValue);
                }
                else
                {
                    text = string.Format(minValue + addValue > 0 ? GameLanguage.Luck: "Curse + {0}", Math.Abs(minValue + addValue));
                }

                MirLabel LUCKLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, LUCKLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, LUCKLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region ACC

            minValue = realItem.Stats[Stat.Accuracy];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Accuracy] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.Accuracy : GameLanguage.Accuracy2, minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Accuracy", minValue + maxValue + addValue);
                MirLabel ACCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Accuracy + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ACCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ACCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region HOLY

            minValue = realItem.Stats[Stat.Holy];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel HOLYLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Holy + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? GameLanguage.Holy : GameLanguage.Holy2, minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HOLYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HOLYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region ASPEED

            minValue = realItem.Stats[Stat.AttackSpeed];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.AttackSpeed] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                {
                    string negative = "+";
                    if (addValue < 0) negative = "";
                    text = string.Format(addValue != 0 ? "A.Speed: " + plus + "{0} ({2}{1})" : "A.Speed: " + plus + "{0}", minValue + addValue, addValue, negative);
                    //text = string.Format(addValue > 0 ? "A.Speed: + {0} (+{1})" : "A.Speed: + {0}", minValue + addValue, addValue);
                }
                else
                    text = string.Format("Adds +{0} A.Speed", minValue + maxValue + addValue);
                MirLabel ASPEEDLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("A.Speed + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ASPEEDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ASPEEDLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region FREEZING

            minValue = realItem.Stats[Stat.Freezing];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Freezing] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Freezing: + {0} (+{1})" : "Freezing: + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Freezing", minValue + maxValue + addValue);
                MirLabel FREEZINGLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Freezing + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, FREEZINGLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, FREEZINGLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON

            minValue = realItem.Stats[Stat.PoisonAttack];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonAttack] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Poison: + {0} (+{1})" : "Poison: + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Poison", minValue + maxValue + addValue);
                MirLabel POISONLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Poison + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISONLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISONLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CRITICALRATE / FLEXIBILITY

            minValue = realItem.Stats[Stat.CriticalRate];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.CriticalRate] : 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;                    
                MirLabel CRITICALRATELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Critical Chance + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Critical Chance: + {0} (+{1})" : "Critical Chance: + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CRITICALRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CRITICALRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CRITICALDAMAGE

            minValue = realItem.Stats[Stat.CriticalDamage];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.CriticalDamage] : 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                MirLabel CRITICALDAMAGELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Critical Damage + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Critical Damage: + {0} (+{1})" : "Critical Damage: + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CRITICALDAMAGELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CRITICALDAMAGELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Reflect

            minValue = realItem.Stats[Stat.Reflect];
            maxValue = 0;
            addValue = 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                MirLabel ReflectLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Reflect chance: {0}", minValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ReflectLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ReflectLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Hpdrain

            minValue = realItem.Stats[Stat.HPDrainRatePercent];
            maxValue = 0;
            addValue = 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                MirLabel HPdrainLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("HP Drain Rate: {0}%", minValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HPdrainLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HPdrainLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Exp Rate

            minValue = realItem.Stats[Stat.ExpRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.ExpRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Exp Rate: " + plus + "{0}% ({2}{1}%)" : "Exp Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                MirLabel expRateLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, expRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, expRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Drop Rate

            minValue = realItem.Stats[Stat.ItemDropRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.ItemDropRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Drop Rate: " + plus + "{0}% ({2}{1}%)" : "Drop Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                MirLabel dropRateLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, dropRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, dropRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Gold Rate

            minValue = realItem.Stats[Stat.GoldDropRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.GoldDropRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Gold Rate: " + plus + "{0}% ({2}{1}%)" : "Gold Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                MirLabel goldRateLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, goldRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, goldRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl DefenceInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;

            string text = "";
            #region AC

            minValue = realItem.Stats[Stat.MinAC];
            maxValue = realItem.Stats[Stat.MaxAC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxAC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.AC : GameLanguage.AC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} AC", minValue + maxValue + addValue);
                MirLabel ACLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("AC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ACLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ACLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAC

            minValue = realItem.Stats[Stat.MinMAC];
            maxValue = realItem.Stats[Stat.MaxMAC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxMAC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.MAC : GameLanguage.MAC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} MAC", minValue + maxValue + addValue);
                MirLabel MACLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("MAC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MACLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MACLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXHP

            minValue = realItem.Stats[Stat.HP];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.HP] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXHPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format(realItem.Type == ItemType.Potion ? "HP + {0} Recovery" : "MAXHP + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Max HP + {0} (+{1})" : "Max HP + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXHPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXHPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXMP

            minValue = realItem.Stats[Stat.MP];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MP] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXMPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format(realItem.Type == ItemType.Potion ? "MP + {0} Recovery" : "MAXMP + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Max MP + {0} (+{1})" : "Max MP + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXHPRATE

            minValue = realItem.Stats[Stat.HPRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXHPRATELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max HP + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXHPRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXHPRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXMPRATE

            minValue = realItem.Stats[Stat.MPRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXMPRATELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MP + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMPRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMPRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXACRATE

            minValue = realItem.Stats[Stat.MaxACRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXACRATE = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max AC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXACRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXACRATE.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXMACRATE

            minValue = realItem.Stats[Stat.MaxMACRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXMACRATELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MAC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMACRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMACRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region HEALTH_RECOVERY

            minValue = realItem.Stats[Stat.HealthRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.HealthRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel HEALTH_RECOVERYLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(addValue > 0 ? "Health Recovery + {0} (+{1})" : "Health Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HEALTH_RECOVERYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HEALTH_RECOVERYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MANA_RECOVERY

            minValue = realItem.Stats[Stat.SpellRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.SpellRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MANA_RECOVERYLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("ManaRecovery + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Mana Recovery + {0} (+{1})" : "Mana Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MANA_RECOVERYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MANA_RECOVERYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON_RECOVERY

            minValue = realItem.Stats[Stat.PoisonRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel POISON_RECOVERYabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Poison Recovery + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Poison Recovery + {0} (+{1})" : "Poison Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISON_RECOVERYabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISON_RECOVERYabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region AGILITY

            minValue = realItem.Stats[Stat.Agility];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Agility] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.Agility : GameLanguage.Agility2, minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Agility", minValue + maxValue + addValue);

                MirLabel AGILITYLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, AGILITYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, AGILITYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region STRONG

            minValue = realItem.Stats[Stat.Strong];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Strong] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel STRONGLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Strong + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Strong + {0} (+{1})" : "Strong + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, STRONGLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, STRONGLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON_RESIST

            minValue = realItem.Stats[Stat.PoisonResist];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonResist] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Poison Resist + {0} (+{1})" : "Poison Resist + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Poison Resist", minValue + maxValue + addValue);
                MirLabel POISON_RESISTLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISON_RESISTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISON_RESISTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAGIC_RESIST

            minValue = realItem.Stats[Stat.MagicResist];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MagicResist] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Magic Resist + {0} (+{1})" : "Magic Resist + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Magic Resist", minValue + maxValue + addValue);
                MirLabel MAGIC_RESISTLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Magic Resist + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAGIC_RESISTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAGIC_RESISTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAX_DC_RATE

            minValue = realItem.Stats[Stat.MaxDCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXDCRATE = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max DC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXDCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXDCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region MAX_MC_RATE

            minValue = realItem.Stats[Stat.MaxMCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXMCRATE = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region MAX_SC_RATE

            minValue = realItem.Stats[Stat.MaxSCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel MAXSCRATE = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max SC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXSCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXSCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region DAMAGE_REDUCTION

            minValue = realItem.Stats[Stat.DamageReductionPercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel DAMAGEREDUC = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("All Damage Reduction + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DAMAGEREDUC.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DAMAGEREDUC.DisplayRectangle.Bottom));
            }
            #endregion
            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl WeightInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);
            
            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;
            
            #region HANDWEIGHT

            minValue = realItem.Stats[Stat.HandWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel HANDWEIGHTLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Hand Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Hand Weight + {0} (+{1})" : "Hand Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HANDWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HANDWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region WEARWEIGHT

            minValue = realItem.Stats[Stat.WearWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel WEARWEIGHTLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Wear Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Wear Weight + {0} (+{1})" : "Wear Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, WEARWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, WEARWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BAGWEIGHT

            minValue = realItem.Stats[Stat.BagWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                MirLabel BAGWEIGHTLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Bag Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Bag Weight + {0} (+{1})" : "Bag Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BAGWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BAGWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region TIME & RANGE
            minValue = 0;
            maxValue = 0;
            addValue = 0;

            if (HoverItem.Info.Type == ItemType.Potion && HoverItem.Info.Durability > 0)
            {
                count++;
                MirLabel TNRLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Time : {0}", Functions.PrintTimeSpanFromSeconds(HoverItem.Info.Durability * 60))
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, TNRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, TNRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl NeedInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region LEVEL
            if (realItem.RequiredAmount > 0)
            {
                count++;
                string text;
                Color colour = Color.White;
                switch (realItem.RequiredType)
                {
                    case RequiredType.Level:
                        text = string.Format(GameLanguage.RequiredLevel, realItem.RequiredAmount);
                        if (MapObject.User.Level < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxAC:
                        text = string.Format("Required AC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxMAC:
                        text = string.Format("Required MAC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxMAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxDC:
                        text = string.Format(GameLanguage.RequiredDC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxDC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxMC:
                        text = string.Format(GameLanguage.RequiredMC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxMC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxSC:
                        text = string.Format(GameLanguage.RequiredSC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxSC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxLevel:
                        text = string.Format("Maximum Level : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Level > realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinAC:
                        text = string.Format("Required Base AC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinMAC:
                        text = string.Format("Required Base MAC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinMAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinDC:
                        text = string.Format("Required Base DC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinDC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinMC:
                        text = string.Format("Required Base MC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinMC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinSC:
                        text = string.Format("Required Base SC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinSC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    default:
                        text = "Unknown Type Required";
                        break;
                }

                MirLabel LEVELLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, LEVELLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, LEVELLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CLASS
            if (realItem.RequiredClass != RequiredClass.None)
            {
                count++;
                Color colour = Color.White;

                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                }

                MirLabel CLASSLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(GameLanguage.ClassRequired, realItem.RequiredClass)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CLASSLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CLASSLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BUYING - SELLING PRICE
            if (item.Price() > 0)
            {
                count++;
                string text;
                var colour = Color.White;

                text = $"Selling Price : {((long)(item.Price() / 2)).ToString("###,###,##0")} Gold";

                var costLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, costLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, costLabel.DisplayRectangle.Bottom));
            }


            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl BindInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region DONT_DEATH_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontDeathdrop))
            {
                count++;
                MirLabel DONT_DEATH_DROPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't drop on death")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DEATH_DROPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DEATH_DROPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontDrop))
            {
                count++;
                MirLabel DONT_DROPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't drop")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DROPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DROPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_UPGRADE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontUpgrade))
            {
                count++;
                MirLabel DONT_UPGRADELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't upgrade")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_UPGRADELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_UPGRADELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_SELL

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontSell))
            {
                count++;
                MirLabel DONT_SELLLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't sell")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_SELLLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_SELLLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_TRADE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontTrade))
            {
                count++;
                MirLabel DONT_TRADELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't trade")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_TRADELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_TRADELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_STORE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontStore))
            {
                count++;
                MirLabel DONT_STORELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't store")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_STORELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_STORELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_REPAIR

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontRepair))
            {
                count++;
                MirLabel DONT_REPAIRLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't repair")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_SPECIALREPAIR

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.NoSRepair))
            {
                count++;
                MirLabel DONT_REPAIRLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't special repair")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BREAK_ON_DEATH

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.BreakOnDeath))
            {
                count++;
                MirLabel DONT_REPAIRLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Breaks on death")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_DESTROY_ON_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DestroyOnDrop))
            {
                count++;
                MirLabel DONT_DODLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Destroyed when dropped")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DODLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DODLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region NoWeddingRing

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.NoWeddingRing))
            {
                count++;
                MirLabel No_WedLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Cannot be a Wedding Ring")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, No_WedLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, No_WedLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BIND_ON_EQUIP

            if ((HoverItem.Info.Bind.HasFlag(BindMode.BindOnEquip)) & HoverItem.SoulBoundId == -1)
            {
                count++;
                MirLabel BOELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("SoulBinds on equip")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BOELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BOELabel.DisplayRectangle.Bottom));
            }
            else if (HoverItem.SoulBoundId != -1)
            {
                count++;
                MirLabel BOELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = "Soulbound to: " + GetUserName((uint)HoverItem.SoulBoundId)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BOELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BOELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CURSED

            if ((!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) && HoverItem.Cursed)
            {
                count++;
                MirLabel CURSEDLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Cursed")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CURSEDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CURSEDLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Gems

            if (HoverItem.Info.Type == ItemType.Gem)
            {
                #region UseOn text
                count++;
                string Text = "";
                if (HoverItem.Info.Unique == SpecialItemMode.None)
                {
                    Text = "Cannot be used on any item.";
                }
                else
                {
                    Text = "Can be used on: ";
                }
                MirLabel GemUseOn = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = Text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemUseOn.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, GemUseOn.DisplayRectangle.Bottom));
                #endregion
                #region Weapon text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Paralize))
                {
                    MirLabel GemWeapon = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Weapon"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemWeapon.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemWeapon.DisplayRectangle.Bottom));
                }
                #endregion
                #region Armour text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Teleport))
                {
                    MirLabel GemArmour = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Armour"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemArmour.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemArmour.DisplayRectangle.Bottom));
                }
                #endregion
                #region Helmet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.ClearRing))
                {
                    MirLabel Gemhelmet = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Helmet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemhelmet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemhelmet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Necklace text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Protection))
                {
                    MirLabel Gemnecklace = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Necklace"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemnecklace.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemnecklace.DisplayRectangle.Bottom));
                }
                #endregion
                #region Bracelet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Revival))
                {
                    MirLabel GemBracelet = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Bracelet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemBracelet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemBracelet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Ring text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Muscle))
                {
                    MirLabel GemRing = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Ring"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemRing.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemRing.DisplayRectangle.Bottom));
                }
                #endregion
                #region Amulet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Flame))
                {
                    MirLabel Gemamulet = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Amulet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemamulet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemamulet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Belt text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Healing))
                {
                    MirLabel Gembelt = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Belt"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gembelt.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gembelt.DisplayRectangle.Bottom));
                }
                #endregion
                #region Boots text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Probe))
                {
                    MirLabel Gemboots = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Boots"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemboots.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemboots.DisplayRectangle.Bottom));
                }
                #endregion
                #region Stone text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Skill))
                {
                    MirLabel Gemstone = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Stone"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemstone.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemstone.DisplayRectangle.Bottom));
                }
                #endregion
                #region Torch text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.NoDuraLoss))
                {
                    MirLabel Gemtorch = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Candle"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemtorch.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemtorch.DisplayRectangle.Bottom));
                }
                #endregion
            }

            #endregion

            #region CANTAWAKEN

            //if ((HoverItem.Info.CanAwakening != true) && (HoverItem.Info.Type != ItemType.Gem))
            //{
            //    count++;
            //    MirLabel CANTAWAKENINGLabel = new MirLabel
            //    {
            //        AutoSize = true,
            //        ForeColour = Color.Yellow,
            //        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
            //        OutLine = true,
            //        Parent = ItemLabel,
            //        Text = string.Format("Can't awaken")
            //    };

            //    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CANTAWAKENINGLabel.DisplayRectangle.Right + 4),
            //        Math.Max(ItemLabel.Size.Height, CANTAWAKENINGLabel.DisplayRectangle.Bottom));
            //}

            #endregion

            #region EXPIRE

            if (HoverItem.ExpireInfo != null)
            {
                double remainingSeconds = (HoverItem.ExpireInfo.ExpiryDate - CMain.Now).TotalSeconds;

                count++;
                MirLabel EXPIRELabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = remainingSeconds > 0 ? string.Format("Expires in {0}", Functions.PrintTimeSpanFromSeconds(remainingSeconds)) : "Expired"
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, EXPIRELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, EXPIRELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SEALED

            if (HoverItem.SealedInfo != null)
            {
                double remainingSeconds = (HoverItem.SealedInfo.ExpiryDate - CMain.Now).TotalSeconds;

                if (remainingSeconds > 0)
                {
                    count++;
                    MirLabel SEALEDLabel = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.Red,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = remainingSeconds > 0 ? string.Format("Sealed for {0}", Functions.PrintTimeSpanFromSeconds(remainingSeconds)) : ""
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SEALEDLabel.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, SEALEDLabel.DisplayRectangle.Bottom));
                }
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl OverlapInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;


            #region GEM

            if (realItem.Type == ItemType.Gem)
            {
                string text = "";

                switch (realItem.Shape)
                {
                    case 1:
                        text = "Hold CTRL and left click to repair weapons.";
                        break;
                    case 2:
                        text = "Hold CTRL and left click to repair armour\nand accessory items.";
                        break;
                    case 3:
                    case 4:
                        text = "Hold CTRL and left click to combine with an item.";
                        break;
                    case 8:
                        text = "Hold CTRL and left click to seal an item.";
                        break;
                }
                count++;
                MirLabel GEMLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GEMLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, GEMLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SPLITUP

            if (realItem.StackSize > 1 && realItem.Type != ItemType.Gem)
            {
                count++;
                MirLabel SPLITUPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(GameLanguage.MaxCombine, realItem.StackSize, "\n")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SPLITUPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SPLITUPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public MirControl StoryInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region TOOLTIP

            if (realItem.Type == ItemType.Scroll && realItem.Shape == 7)//Credit Scroll
            {
                HoverItem.Info.ToolTip = string.Format("Adds {0} Credits to your Account.", HoverItem.Info.Price);
            }

            if (!string.IsNullOrEmpty(HoverItem.Info.ToolTip))
            {
                count++;

                MirLabel IDLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.DarkKhaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = GameLanguage.ItemDescription
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, IDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, IDLabel.DisplayRectangle.Bottom));

                MirLabel TOOLTIPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Khaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = HoverItem.Info.ToolTip
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, TOOLTIPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, TOOLTIPLabel.DisplayRectangle.Bottom));
            }

            #endregion
     
            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                MirControl outLine = new MirControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }

        public void CreateItemLabel(UserItem item, bool inspect = false, bool hideDura = false, bool hideAdded = false)
        {
            CMain.DebugText = CMain.Random.Next(1, 100).ToString();

            if (item == null || HoverItem != item)
            {
                DisposeItemLabel();

                if (item == null)
                {
                    HoverItem = null;
                    return;
                }
            }

            if (item == HoverItem && ItemLabel != null && !ItemLabel.IsDisposed) return;
            ushort level = MapObject.User.Level;
            MirClass job = MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

             ItemLabel = new MirControl
            {
                BackColour = Color.FromArgb(255, 0, 0, 0),
                Border = true,
                BorderColour = ((HoverItem.CurrentDura == 0 && HoverItem.MaxDura != 0) ? Color.Red : Color.FromArgb(255, 148, 146, 148)),
                DrawControlTexture = true,
                NotControl = true,
                Parent = this,
                Opacity = 0.8F
            };

            //Name Info Label
            MirControl[] outlines = new MirControl[8];
            outlines[0] = NameInfoLabel(item, inspect, hideDura);
            //Attribute Info1 Label - Attack Info
            outlines[1] = AttackInfoLabel(item, inspect, hideAdded);
            //Attribute Info2 Label - Defence Info
            outlines[2] = DefenceInfoLabel(item, inspect, hideAdded);
            //Attribute Info3 Label - Weight Info
            outlines[3] = WeightInfoLabel(item, inspect);
            //need Info Label
            outlines[4] = NeedInfoLabel(item, inspect);
            //Bind Info Label
            outlines[5] = BindInfoLabel(item, inspect, hideAdded);
            //Overlap Info Label
            outlines[6] = OverlapInfoLabel(item, inspect);
            //Story Label
            outlines[7] = StoryInfoLabel(item, inspect);

            foreach (var outline in outlines)
            {
                if (outline != null)
                {
                    outline.Size = new Size(ItemLabel.Size.Width, outline.Size.Height);
                }
            }

            //ItemLabel.Visible = true;
        }
        public void CreateMailLabel(ClientMail mail)
        {
            if (mail == null)
            {
                DisposeMailLabel();
                return;
            }

            if (MailLabel != null && !MailLabel.IsDisposed) return;

            MailLabel = new MirControl
            {
                BackColour = Color.FromArgb(255, 50, 50, 50),
                Border = true,
                BorderColour = Color.Gray,
                DrawControlTexture = true,
                NotControl = true,
                Parent = this,
                Opacity = 0.7F
            };

            MirLabel nameLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                Location = new Point(4, 4),
                OutLine = true,
                Parent = MailLabel,
                Text = mail.SenderName
            };

            MailLabel.Size = new Size(Math.Max(MailLabel.Size.Width, nameLabel.DisplayRectangle.Right + 4),
                Math.Max(MailLabel.Size.Height, nameLabel.DisplayRectangle.Bottom));

            MirLabel dateLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Location = new Point(4, MailLabel.DisplayRectangle.Bottom),
                OutLine = true,
                Parent = MailLabel,
                Text = string.Format(GameLanguage.DateSent, mail.DateSent.ToString("dd/MM/yy H:mm:ss"))
            };

            MailLabel.Size = new Size(Math.Max(MailLabel.Size.Width, dateLabel.DisplayRectangle.Right + 4),
                Math.Max(MailLabel.Size.Height, dateLabel.DisplayRectangle.Bottom));

            if (mail.Gold > 0)
            {
                MirLabel goldLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, MailLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = MailLabel,
                    Text = "Gold: " + mail.Gold
                };

                MailLabel.Size = new Size(Math.Max(MailLabel.Size.Width, goldLabel.DisplayRectangle.Right + 4),
                Math.Max(MailLabel.Size.Height, goldLabel.DisplayRectangle.Bottom));
            }

            MirLabel openedLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Red,
                Location = new Point(4, MailLabel.DisplayRectangle.Bottom),
                OutLine = true,
                Parent = MailLabel,
                Text = mail.Opened ? "[Old]" : "[New]"
            };

            MailLabel.Size = new Size(Math.Max(MailLabel.Size.Width, openedLabel.DisplayRectangle.Right + 4),
            Math.Max(MailLabel.Size.Height, openedLabel.DisplayRectangle.Bottom));
        }
        public void CreateMemoLabel(ClientFriend friend)
        {
            if (friend == null)
            {
                DisposeMemoLabel();
                return;
            }

            if (MemoLabel != null && !MemoLabel.IsDisposed) return;

            MemoLabel = new MirControl
            {
                BackColour = Color.FromArgb(255, 50, 50, 50),
                Border = true,
                BorderColour = Color.Gray,
                DrawControlTexture = true,
                NotControl = true,
                Parent = this,
                Opacity = 0.7F
            };

            MirLabel memoLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Location = new Point(4, 4),
                OutLine = true,
                Parent = MemoLabel,
                Text = Functions.StringOverLines(friend.Memo, 5, 20)
            };

            MemoLabel.Size = new Size(Math.Max(MemoLabel.Size.Width, memoLabel.DisplayRectangle.Right + 4),
                Math.Max(MemoLabel.Size.Height, memoLabel.DisplayRectangle.Bottom + 4));
        }

        public static ItemInfo GetInfo(int index)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }

            return null;
        }

        public string GetUserName(uint id)
        {
            for (int i = 0; i < UserIdList.Count; i++)
            {
                UserId who = UserIdList[i];
                if (id == who.Id)
                    return who.UserName;
            }
            Network.Enqueue(new C.RequestUserName { UserID = id });
            UserIdList.Add(new UserId() { Id = id, UserName = "Unknown" });
            return "";
        }

        public class UserId
        {
            public long Id = 0;
            public string UserName = "";
        }

        public class OutPutMessage
        {
            public string Message;
            public long ExpireTime;
            public OutputMessageType Type;
        }

        public void Rankings(S.Rankings p)
        {
            foreach (RankCharacterInfo info in p.ListingDetails)
            {
                if (RankingList.ContainsKey(info.PlayerId))
                    RankingList[info.PlayerId] = info;
                else
                    RankingList.Add(info.PlayerId, info);
            }
            List<RankCharacterInfo> listings = new List<RankCharacterInfo>();
            foreach (long id in p.Listings)
                listings.Add(RankingList[id]);

            RankingDialog.RecieveRanks(listings, p.RankType, p.MyRank, p.Count);
        }

        private void OpenBrowser(S.OpenBrowser p) {
            BrowserHelper.OpenDefaultBrowser(p.Url);
        }

        public void PlaySound(S.PlaySound p)
        {
            SoundManager.PlaySound(p.Sound, false);
        }
        private void SetTimer(S.SetTimer p)
        {
            GameScene.Scene.TimerControl.AddTimer(p);
        }

        private void ExpireTimer(S.ExpireTimer p)
        {
            GameScene.Scene.TimerControl.ExpireTimer(p.Key);
        }

        private void SetCompass(S.SetCompass p)
        {
            GameScene.Scene.CompassControl.SetPoint(p.Location);
        }

        private void Roll(S.Roll p)
        {
            GameScene.Scene.RollControl.Setup(p.Type, p.Page, p.Result, p.AutoRoll);
        }

        public void ShowNotice(S.UpdateNotice p)
        {
            NoticeDialog.Update(p.Notice);
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Scene = null;
                User = null;

                MoveTime = 0;
                AttackTime = 0;
                CanMove = false;

                MapControl = null;
                MainDialog = null;
                ChatDialog = null;
                InventoryDialog = null;
                CharacterDialog = null;
                StorageDialog = null;
                BeltDialog = null;
                OptionDialog = null;
                MenuDialog = null;
                NPCDialog = null;
                QuestDetailDialog = null;
                QuestListDialog = null;
                QuestLogDialog = null;
                QuestTrackingDialog = null;
                GameShopDialog = null;

                RelationshipDialog = null;

                HoverItem = null;
                SelectedCell = null;
                PickedUpGold = false;

                UseItemTime = 0;
                PickUpTime = 0;

                DisposeItemLabel();

                AMode = 0;
                PMode = 0;
                Lights = 0;

                NPCTime = 0;
                NPCID = 0;
                DefaultNPCID = 0;

                for (int i = 0; i < OutputLines.Length; i++)
                    if (OutputLines[i] != null && OutputLines[i].IsDisposed)
                        OutputLines[i].Dispose();

                OutputMessages.Clear();
                OutputMessages = null;
            }

            base.Dispose(disposing);
        }

        #endregion

    }

    public sealed class MapControl : MirControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static List<MapObject> Objects = new List<MapObject>();

        public const int CellWidth = 48;
        public const int CellHeight = 32;

        public static int OffSetX;
        public static int OffSetY;

        public static int ViewRangeX;
        public static int ViewRangeY;

        private bool _autoPath;
        public bool AutoPath
        {
            get
            {
                return _autoPath;
            }
            set
            {
                if (_autoPath == value) return;
                _autoPath = value;

                if (!_autoPath)
                    CurrentPath = null;
            }
        }

        public PathFinder PathFinder;
        public List<Node> CurrentPath = null;

        public static Point MapLocation
        {
            get { return GameScene.User == null ? Point.Empty : new Point(MouseLocation.X / CellWidth - OffSetX, MouseLocation.Y / CellHeight - OffSetY).Add(GameScene.User.CurrentLocation); }
        }

        public static Point ToMouseLocation(Point p)
        {
            return new Point((p.X - MapObject.User.Movement.X + OffSetX) * CellWidth, (p.Y - MapObject.User.Movement.Y + OffSetY) * CellHeight).Add(MapObject.User.OffSetMove);
        }

        public static MouseButtons MapButtons;
        public static Point MouseLocation;
        public static long InputDelay;

        public static Dictionary<int, Texture> MiniMaps = new Dictionary<int, Texture>();

        private static long nextAction;
        public static long NextAction
        {
            get { return nextAction; }
            set
            {
                if (GameScene.Observing) return;
                nextAction = value;
            }
        }

        public CellInfo[,] M2CellInfo;
        public int Width, Height;

        public int Index;
        public string FileName = String.Empty;
        public string Title = String.Empty;
        public ushort MiniMap, BigMap, Music, SetMusic;
        public LightSetting Lights;
        public bool Lightning, Fire;
        public byte MapDarkLight;
        public long LightningTime, FireTime;

        public bool FloorValid, LightsValid;

        public long OutputDelay;
        public Texture MiniMapTexture;

        public int AnimationCount;
        
        public static List<Effect> Effects = new List<Effect>();

        public MapControl()
        {
            MapButtons = MouseButtons.None;

            OffSetX = Settings.ScreenWidth / 2 / CellWidth;
            OffSetY = Settings.ScreenHeight / 2 / CellHeight - 1;

            ViewRangeX = OffSetX + 6;
            ViewRangeY = OffSetY + 6;

            Size = new Size(Settings.ScreenWidth, Settings.ScreenHeight);
            DrawControlTexture = true;
            BackColour = Color.Black;

            MouseDown += OnMouseDown;
            MouseMove += (o, e) => MouseLocation = e.Location;
            Click += OnMouseClick;
        }

        public void ResetMap()
        {
            GameScene.Scene.NPCDialog.Hide();

            MapObject.MouseObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.MagicObjectID = 0;

            if (M2CellInfo != null)
            {
                for (var i = Objects.Count - 1; i >= 0; i--)
                {
                    var obj = Objects[i];
                    if (obj == null) continue;

                    obj.Remove();
                }
            }

            Objects.Clear();
            Effects.Clear();

            if (User != null)
                Objects.Add(User);
        }

        public void LoadMap()
        {
            ResetMap();

            MapObject.MouseObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.MagicObjectID = 0;
            MiniMaps.TryGetValue(Index, out MiniMapTexture);

            MapReader Map = new MapReader(FileName);
            M2CellInfo = Map.MapCells;
            Width = Map.Width;
            Height = Map.Height;

            if (MiniMapTexture == null)
                CreateMiniMap();

            PathFinder = new PathFinder(this);

            try
            {
                if (SetMusic != Music)
                {
                    SoundManager.Music?.Dispose();
                    SoundManager.PlayMusic(Music, true);
                }
            }
            catch (Exception)
            {
                // Do nothing. index was not valid.
            }

            SetMusic = Music;
        }

        public void CreateMiniMap()
        {
            var texture = new Texture(DXManager.Device, Width, Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            DataRectangle stream = texture.LockRectangle(0, LockFlags.Discard);
            using (Bitmap image = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppArgb, stream.Data.DataPointer))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);
                }
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                    {
                        if (!WalkableCell(x, y))
                            image.SetPixel(x, y, Color.Black);
                    }
            }
            texture.UnlockRectangle(0);
            MiniMaps.Add(Index, texture);
            MiniMapTexture = texture;
        }
        public bool WalkableCell(int x, int y)
        {
            if ((M2CellInfo[x, y].BackImage & 0x20000000) != 0 || (M2CellInfo[x, y].FrontImage & 0x8000) != 0)
                return false;
            return true;
        }


        public void Process()
        {
            User.Process();
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = Objects[i];
                if (ob == User) continue;
                //  if (ob.ActionFeed.Count > 0 || ob.Effects.Count > 0 || GameScene.CanMove || CMain.Time >= ob.NextMotion)
                ob.Process();
            }

            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Process();

            if (MapObject.TargetObject != null && MapObject.TargetObject is MonsterObject && MapObject.TargetObject.AI == 64)
                MapObject.TargetObjectID = 0;
            if (MapObject.MagicObject != null && MapObject.MagicObject is MonsterObject && MapObject.MagicObject.AI == 64)
                MapObject.MagicObjectID = 0;

            CheckInput();


            MapObject bestmouseobject = null;
            for (int y = MapLocation.Y + 2; y >= MapLocation.Y - 2; y--)
            {
                if (y >= Height) continue;
                if (y < 0) break;
                for (int x = MapLocation.X + 2; x >= MapLocation.X - 2; x--)
                {
                    if (x >= Width) continue;
                    if (x < 0) break;
                    CellInfo cell = M2CellInfo[x, y];
                    if (cell.CellObjects == null) continue;

                    for (int i = cell.CellObjects.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = cell.CellObjects[i];
                        if (ob == MapObject.User || !ob.MouseOver(CMain.MPoint)) continue;

                        if (MapObject.MouseObject != ob)
                        {
                            if (ob.Dead)
                            {
                                if (!Settings.TargetDead && GameScene.TargetDeadTime <= CMain.Time) continue;

                                bestmouseobject = ob;
                                //continue;
                            }
                            MapObject.MouseObjectID = ob.ObjectID;
                            Redraw();
                        }
                        if (bestmouseobject != null && MapObject.MouseObject == null)
                        {
                            MapObject.MouseObjectID = bestmouseobject.ObjectID;
                            Redraw();
                        }
                        return;
                    }
                }
            }


            if (MapObject.MouseObject != null)
            {
                MapObject.MouseObjectID = 0;
                Redraw();
            }
        }

        public static MapObject GetObject(uint targetID)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.ObjectID != targetID) continue;
                return ob;
            }
            return null;
        }

        public override void Draw()
        {
            //Do nothing.
        }

        protected override void CreateTexture()
        {
            if (User == null) return;

            if (!FloorValid)
                DrawFloor();


            if (Size != TextureSize)
                DisposeTexture();

            if (ControlTexture == null || ControlTexture.Disposed)
            {
                DXManager.ControlList.Add(this);
                ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                TextureSize = Size;
            }

            Surface oldSurface = DXManager.CurrentSurface;
            Surface surface = ControlTexture.GetSurfaceLevel(0);
            DXManager.SetSurface(surface);
            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0, 0);

            if (FloorValid)
            {
                DXManager.Draw(DXManager.FloorTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White);
            }

            DrawObjects();

            //Render Death, 

            LightSetting setting = Lights == LightSetting.Normal ? GameScene.Scene.Lights : Lights;

            if (setting != LightSetting.Day)
            {
                DrawLights(setting);
            }

            if (Settings.DropView || GameScene.DropViewTime > CMain.Time)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    ItemObject ob = Objects[i] as ItemObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation))
                        ob.DrawName();
                }
            }

            if (MapObject.MouseObject != null && !(MapObject.MouseObject is ItemObject))
                MapObject.MouseObject.DrawName();

            int offSet = 0; 
            
            if (Settings.DisplayBodyName)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    MonsterObject ob = Objects[i] as MonsterObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation)) continue;
                    ob.DrawName();
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                ItemObject ob = Objects[i] as ItemObject;
                if (ob == null) continue;

                if (!ob.MouseOver(MouseLocation)) continue;
                ob.DrawName(offSet);
                offSet -= ob.NameLabel.Size.Height + (ob.NameLabel.Border ? 1 : 0);
            }

            if (MapObject.User.MouseOver(MouseLocation))
                MapObject.User.DrawName();

            DXManager.SetSurface(oldSurface);
            surface.Dispose();
            TextureValid = true;

        }
        protected internal override void DrawControl()
        {
            if (!DrawControlTexture)
                return;

            if (!TextureValid)
                CreateTexture();

            if (ControlTexture == null || ControlTexture.Disposed)
                return;

            float oldOpacity = DXManager.Opacity;

            if (MapObject.User.Dead) DXManager.SetGrayscale(true);

            DXManager.DrawOpaque(ControlTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White, Opacity);

            if (MapObject.User.Dead) DXManager.SetGrayscale(false);

            CleanTime = CMain.Time + Settings.CleanDelay;
        }

        private void DrawFloor()
        {
            if (DXManager.FloorTexture == null || DXManager.FloorTexture.Disposed)
            {
                DXManager.FloorTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                DXManager.FloorSurface = DXManager.FloorTexture.GetSurfaceLevel(0);
            }

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.FloorSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0); //Color.Black

            int index;
            int drawY, drawX;

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 5; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet

                    index = M2CellInfo[x, y].MiddleImage - 1;

                    if ((index < 0) || (M2CellInfo[x, y].MiddleIndex == -1)) continue;
                    Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].Draw(index, drawX, drawY);
                }
            }            

            DXManager.SetSurface(oldSurface);

            FloorValid = true;
        }

        private void DrawObjects()
        {
            if (Settings.Effect)
            {
                for (int i = Effects.Count - 1; i >= 0; i--)
                {
                    if (!Effects[i].DrawBehind) continue;
                    Effects[i].Draw();
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawDeadObjects();
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                int drawY = (y - User.Movement.Y + OffSetY + 1) * CellHeight + User.OffSetMove.Y;

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X;
                    int index;
                    byte animation;
                    bool blend;
                    Size s;

                    #region Draw front layer
                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;

                    if (index < 0) continue;

                    int fileIndex = M2CellInfo[x, y].FrontIndex;
                    if (fileIndex == -1) continue;
                    animation = M2CellInfo[x, y].FrontAnimationFrame;

                    if ((animation & 0x80) > 0)
                    {
                        blend = true;
                        animation &= 0x7F;
                    }
                    else
                        blend = false;

                    
                    if (animation > 0)
                    {
                        byte animationTick = M2CellInfo[x, y].FrontAnimationTick;
                        index += (AnimationCount % (animation + (animation * animationTick))) / (1 + animationTick);
                    }

                    s = Libraries.MapLibs[fileIndex].GetSize(index);

                    if (blend)
                    {
                        if ((fileIndex > 99) & (fileIndex < 199))
                            Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - (3 * CellHeight)), Color.White, true);
                        else
                            Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - s.Height), Color.White, (index >= 2723 && index <= 2732));
                    }
                    else
                        Libraries.MapLibs[fileIndex].Draw(index, drawX, drawY - s.Height);
                    #endregion
                }

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawObjects();
                }
            }

            DXManager.Sprite.Flush();
            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(0.4F);

            //MapObject.User.DrawMount();

            MapObject.User.DrawBody();

            if ((MapObject.User.Direction == MirDirection.Up) ||
                (MapObject.User.Direction == MirDirection.UpLeft) ||
                (MapObject.User.Direction == MirDirection.UpRight) ||
                (MapObject.User.Direction == MirDirection.Right) ||
                (MapObject.User.Direction == MirDirection.Left))
            {
                MapObject.User.DrawHead();
                MapObject.User.DrawWings();
            }
            else
            {
                MapObject.User.DrawWings();
                MapObject.User.DrawHead();
            }

            DXManager.SetOpacity(oldOpacity);

            if (Settings.HighlightTarget)
            {
                if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && MapObject.MouseObject != MapObject.TargetObject && MapObject.MouseObject.Blend)
                    MapObject.MouseObject.DrawBlend();

                if (MapObject.TargetObject != null)
                {
                    Libraries.Prguse.Draw(210, MapObject.TargetObject.DrawLocation.X - 16, MapObject.TargetObject.DrawLocation.Y - 16);
                    MapObject.TargetObject.DrawBlend();
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].DrawEffects(Settings.Effect);

                if (Settings.NameView && !(Objects[i] is ItemObject) && !Objects[i].Dead)
                    Objects[i].DrawName();

                Objects[i].DrawChat();
                Objects[i].DrawHealth();
                Objects[i].DrawPoison();

                Objects[i].DrawDamages();
            }

            if (Settings.Effect)
            {
                for (int i = Effects.Count - 1; i >= 0; i--)
                {
                    if (Effects[i].DrawBehind) continue;
                    Effects[i].Draw();
                }
            }
        }

        private Color GetBlindLight(Color light)
        {
            if (MapObject.User.BlindTime <= CMain.Time && MapObject.User.BlindCount < 25)
            {
                MapObject.User.BlindTime = CMain.Time + 100;
                MapObject.User.BlindCount++;
            }

            int count = MapObject.User.BlindCount;
            light = Color.FromArgb(255, Math.Max(20, light.R - (count * 10)), Math.Max(20, light.G - (count * 10)), Math.Max(20, light.B - (count * 10)));

            return light;
        }

        private void DrawLights(LightSetting setting)
        {
            if (DXManager.Lights == null || DXManager.Lights.Count == 0) return;

            if (DXManager.LightTexture == null || DXManager.LightTexture.Disposed)
            {
                DXManager.LightTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                DXManager.LightSurface = DXManager.LightTexture.GetSurfaceLevel(0);
            }

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.LightSurface);

            #region Night Lights
            Color darkness;

            switch (setting)
            {
                case LightSetting.Night:
                    {
                        switch (MapDarkLight)
                        {
                            case 1:
                                darkness = Color.FromArgb(255, 20, 20, 20);
                                break;
                            case 2:
                                darkness = Color.LightSlateGray;
                                break;
                            case 3:
                                darkness = Color.SkyBlue;
                                break;
                            case 4:
                                darkness = Color.Goldenrod;
                                break;
                            default:
                                darkness = Color.Black;
                                break;
                        }
                    }
                    break;
                case LightSetting.Evening:
                case LightSetting.Dawn:
                    darkness = Color.FromArgb(255, 50, 50, 50);
                    break;
                default:
                case LightSetting.Day:
                    darkness = Color.FromArgb(255, 255, 255, 255);
                    break;
            }

            DXManager.Device.Clear(ClearFlags.Target, darkness, 0, 0);

            #endregion

            int light;
            Point p;
            DXManager.SetBlend(true);
            DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

            #region Object Lights (Player/Mob/NPC)
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                {
                    light = ob.Light;

                    int lightRange = light % 15;
                    if (lightRange >= DXManager.Lights.Count)
                        lightRange = DXManager.Lights.Count - 1;

                    p = ob.DrawLocation;

                    Color lightColour = ob.LightColour;

                    if (ob.Race == ObjectType.Player)
                    {
                        switch (light / 15)
                        {
                            case 0://no light source
                                lightColour = Color.FromArgb(255, 60, 60, 60);
                                break;
                            case 1:
                                lightColour = Color.FromArgb(255, 120, 120, 120);
                                break;
                            case 2://Candle
                                lightColour = Color.FromArgb(255, 180, 180, 180);
                                break;
                            case 3://Torch
                                lightColour = Color.FromArgb(255, 240, 240, 240);
                                break;
                            default://Peddler Torch
                                lightColour = Color.FromArgb(255, 255, 255, 255);
                                break;
                        }
                    }
                    else if (ob.Race == ObjectType.Merchant)
                    {
                        lightColour = Color.FromArgb(255, 120, 120, 120);
                    }

                    if (DXManager.Lights[lightRange] != null && !DXManager.Lights[lightRange].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[lightRange].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[lightRange].Y / 2) - (CellHeight / 2) -5);
                        DXManager.Draw(DXManager.Lights[lightRange], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }
                }

                #region Object Effect Lights
                if (!Settings.Effect) continue;
                for (int e = 0; e < ob.Effects.Count; e++)
                {
                    Effect effect = ob.Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start || (!(effect is Missile) && effect.Light < ob.Light)) continue;

                    light = effect.Light;
                    
                    p = effect.DrawLocation;

                    var lightColour = effect.LightColour;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }

                }
                #endregion
            }
            #endregion

            #region Map Effect Lights
            if (Settings.Effect)
            {
                for (int e = 0; e < Effects.Count; e++)
                {
                    Effect effect = Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start) continue;

                    light = effect.Light;
                    if (light == 0) continue;

                    p = effect.DrawLocation;

                    var lightColour = Color.White;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }
                }
            }
            #endregion

            #region Map Lights
            for (int y = MapObject.User.Movement.Y - ViewRangeY - 24; y <= MapObject.User.Movement.Y + ViewRangeY + 24; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = MapObject.User.Movement.X - ViewRangeX - 24; x < MapObject.User.Movement.X + ViewRangeX + 24; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int imageIndex = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (M2CellInfo[x, y].Light <= 0 || M2CellInfo[x, y].Light >= 10) continue;
                    if (M2CellInfo[x, y].Light == 0) continue;

                    Color lightIntensity;

                    light = (M2CellInfo[x, y].Light % 10) * 3;

                    switch (M2CellInfo[x, y].Light / 10)
                    {
                        case 1:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                        case 2:
                            lightIntensity = Color.FromArgb(255, 120, 180, 255);
                            break;
                        case 3:
                            lightIntensity = Color.FromArgb(255, 255, 180, 120);
                            break;
                        case 4:
                            lightIntensity = Color.FromArgb(255, 22, 160, 5);
                            break;
                        default:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                    }

                    int fileIndex = M2CellInfo[x, y].FrontIndex;

                    p = new Point(
                        (x + OffSetX - MapObject.User.Movement.X) * CellWidth + MapObject.User.OffSetMove.X,
                        (y + OffSetY - MapObject.User.Movement.Y) * CellHeight + MapObject.User.OffSetMove.Y + 32);


                    if (M2CellInfo[x, y].FrontAnimationFrame > 0)
                        p.Offset(Libraries.MapLibs[fileIndex].GetOffSet(imageIndex));

                    if (light >= DXManager.Lights.Count)
                        light = DXManager.Lights.Count - 1;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2) + 10, -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightIntensity);
                    }
                }
            }
            #endregion

            DXManager.SetBlend(false);
            DXManager.SetSurface(oldSurface);

            DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
            DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);

            DXManager.Draw(DXManager.LightTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White);

            DXManager.Sprite.End();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
        }

        private static void OnMouseClick(object sender, EventArgs e)
        {
            if (!(e is MouseEventArgs me)) return;

            switch (me.Button)
            {
                case MouseButtons.Left:
                    {
                        GameScene.Scene.MapControl.AutoPath = false;
                        if (MapObject.MouseObject == null) return;
                        NPCObject npc = MapObject.MouseObject as NPCObject;
                        if (npc != null)
                        {
                            if (npc.ObjectID == GameScene.NPCID && 
                                (CMain.Time <= GameScene.NPCTime || GameScene.Scene.NPCDialog.Visible))
                            {
                                return;
                            }

                            //GameScene.Scene.NPCDialog.Hide();

                            GameScene.NPCTime = CMain.Time + 5000;
                            GameScene.NPCID = npc.ObjectID;
                            Network.Enqueue(new C.CallNPC { ObjectID = npc.ObjectID, Key = "[@Main]" });
                        }
                    }
                    break;
                case MouseButtons.Right:
                    {
                        if (MapObject.MouseObject == null)
                        {
                            if (Settings.NewMove && MapLocation != MapObject.User.CurrentLocation && GameScene.Scene.MapControl.EmptyCell(MapLocation))
                            {
                                var path = GameScene.Scene.MapControl.PathFinder.FindPath(MapObject.User.CurrentLocation, MapLocation, 20);

                                if (path != null && path.Count > 0)
                                {
                                    GameScene.Scene.MapControl.CurrentPath = path;
                                    GameScene.Scene.MapControl.AutoPath = true;
                                    var offset = MouseLocation.Subtract(ToMouseLocation(MapLocation));
                                }
                            }
                            return;
                        }
                    }
                    break;
            }
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            MapButtons |= e.Button;

            if (e.Button != MouseButtons.Left) return;

            if (GameScene.SelectedCell != null)
            {
                MirItemCell cell = GameScene.SelectedCell;
                if (cell.Item.Info.Bind.HasFlag(BindMode.DontDrop))
                {
                    MirMessageBox messageBox = new MirMessageBox(string.Format("You cannot drop {0}", cell.Item.FriendlyName), MirMessageBoxButtons.OK);
                    messageBox.Show();
                    GameScene.SelectedCell = null;
                    return;
                }
                if (cell.Item.Count == 1)
                {
                    MirMessageBox messageBox = new MirMessageBox(string.Format(GameLanguage.DropTip, cell.Item.FriendlyName), MirMessageBoxButtons.YesNo);

                    messageBox.YesButton.Click += (o, a) =>
                    {
                        Network.Enqueue(new C.DropItem 
                        {   UniqueID = cell.Item.UniqueID, 
                            Count = 1
                        });
                        
                        cell.Locked = true;
                    };
                    messageBox.Show();
                }
                else
                {
                    MirAmountBox amountBox = new MirAmountBox(GameLanguage.DropAmount, cell.Item.Info.Image, Libraries.Items, cell.Item.Count);

                    amountBox.OKButton.Click += (o, a) =>
                    {
                        if (amountBox.Amount <= 0) return;
                        Network.Enqueue(new C.DropItem
                        {
                            UniqueID = cell.Item.UniqueID,
                            Count = (ushort)amountBox.Amount
                        });

                        cell.Locked = true;
                    };

                    amountBox.Show();
                }
                GameScene.SelectedCell = null;

                return;
            }

            if (GameScene.PickedUpGold)
            {
                MirAmountBox amountBox = new MirAmountBox(GameLanguage.DropAmount, 38, Libraries.Prguse, GameScene.Gold);

                amountBox.OKButton.Click += (o, a) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        Network.Enqueue(new C.DropGold { Amount = amountBox.Amount });
                    }
                };

                amountBox.Show();
                GameScene.PickedUpGold = false;
            }

            if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && !(MapObject.MouseObject is ItemObject) &&
                !(MapObject.MouseObject is NPCObject) && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 64)
                 && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70))
            {
                MapObject.TargetObjectID = MapObject.MouseObject.ObjectID;
                if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI != 6)
                    MapObject.MagicObjectID = MapObject.TargetObject.ObjectID;
            }
            else
                MapObject.TargetObjectID = 0;
        }

        private void CheckInput()
        {            
            if (CMain.Time < InputDelay || User.Poison.HasFlag(PoisonType.Paralysis) || User.Poison.HasFlag(PoisonType.Frozen)) return;
            
            if (User.NextMagic != null)
            {
                UseMagic(User.NextMagic, User);
                return;
            }

            if (MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (((MapObject.TargetObject.Name.EndsWith(")") || MapObject.TargetObject is PlayerObject) && CMain.Shift) ||
                    (!MapObject.TargetObject.Name.EndsWith(")") && MapObject.TargetObject is MonsterObject))
                {
                    GameScene.LogTime = CMain.Time + Globals.LogDelay;

                    if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1))
                    {
                        if (CMain.Time > GameScene.AttackTime)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                            return;
                        }
                    }
                }
            }
            
            MirDirection direction;
            if (MouseControl == this)
            {
                direction = MouseDirection();

                switch (MapButtons)
                {
                    case MouseButtons.Left:
                        if (MapObject.MouseObject is NPCObject || (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User)) break;
                        if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70) break;

                        if (CMain.Shift)
                        {
                            if (CMain.Time > GameScene.AttackTime)
                            {
                                MapObject target = null;
                                if (MapObject.MouseObject is MonsterObject || MapObject.MouseObject is PlayerObject) target = MapObject.MouseObject;

                                User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        if (MapLocation == User.CurrentLocation)
                        {
                            if (CMain.Time > GameScene.PickUpTime)
                            {
                                GameScene.PickUpTime = CMain.Time + 200;
                                Network.Enqueue(new C.PickUp());
                            }
                            return;
                        }

                        if (CanWalk(direction, out direction))
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }

                        break;
                    case MouseButtons.Right:
                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User && CMain.Ctrl) break;
                        if (Settings.NewMove) break;

                        if (Functions.InRange(MapLocation, User.CurrentLocation, 2))
                        {
                            if (direction != User.Direction)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        if (CanWalk(direction, out direction))
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        break;
                }
            }

            if (AutoPath)
            {
                if (CurrentPath == null || CurrentPath.Count == 0)
                {
                    AutoPath = false;
                    return;
                }

                var path = GameScene.Scene.MapControl.PathFinder.FindPath(MapObject.User.CurrentLocation, CurrentPath.Last().Location);

                if (path != null && path.Count > 0)
                    GameScene.Scene.MapControl.CurrentPath = path;
                else
                {
                    AutoPath = false;
                    return;
                }

                Node currentNode = CurrentPath.SingleOrDefault(x => User.CurrentLocation == x.Location);
                if (currentNode != null)
                {
                    while (true)
                    {
                        Node first = CurrentPath.First();
                        CurrentPath.Remove(first);

                        if (first == currentNode)
                            break;
                    }
                }

                if (CurrentPath.Count > 0)
                {
                    MirDirection dir = Functions.DirectionFromPoint(User.CurrentLocation, CurrentPath.First().Location);

                    if (CanWalk(dir))
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = dir, Location = Functions.PointMove(User.CurrentLocation, dir, 1) };

                        return;
                    }
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if (((!MapObject.TargetObject.Name.EndsWith(")") && !(MapObject.TargetObject is PlayerObject)) || !CMain.Shift) &&
                (MapObject.TargetObject.Name.EndsWith(")") || !(MapObject.TargetObject is MonsterObject))) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1)) return;
            direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanWalk(direction, out direction)) return;

            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
        }

        public void UseMagic(ClientMagic magic, UserObject actor)
        {
            if (CMain.Time < GameScene.SpellTime || actor.Poison.HasFlag(PoisonType.Stun))
            {
                actor.ClearMagic();
                return;
            }

            if ((CMain.Time <= magic.CastTime + magic.Delay))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage(string.Format("You cannot cast {0} for another {1} seconds.", magic.Spell.ToString(), ((magic.CastTime + magic.Delay) - CMain.Time - 1) / 1000 + 1));
                }

                actor.ClearMagic();
                return;
            }

            int cost = magic.Level * magic.LevelCost + magic.BaseCost;

            if (magic.Spell == Spell.Teleport)
            {
                if (actor.Stats[Stat.TeleportManaPenaltyPercent] > 0)
                {
                    cost += (cost * actor.Stats[Stat.TeleportManaPenaltyPercent]) / 100;
                }
            }

            if (actor.Stats[Stat.ManaPenaltyPercent] > 0)
            {
                cost += (cost * actor.Stats[Stat.ManaPenaltyPercent]) / 100;
            }

            if (cost > actor.MP)
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage(GameLanguage.LowMana);
                }
                actor.ClearMagic();
                return;
            }

            //bool isTargetSpell = true;

            MapObject target = null;

            //Targeting
            switch (magic.Spell)
            {
                case Spell.FireBall:
                case Spell.GreatFireBall:
                case Spell.ElectricShock:
                case Spell.Poisoning:
                case Spell.ThunderBolt:
                case Spell.SoulFireBall:
                case Spell.FrostCrunch:
                case Spell.Revelation:
                case Spell.Hallucination:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObjectID = target.ObjectID;
                    break;
                case Spell.Purification:
                case Spell.Healing:
                case Spell.UltimateEnhancer:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }

                    if (target == null) target = User;
                    break;
                case Spell.FireBang:
                case Spell.MassHiding:
                case Spell.FireWall:
                case Spell.TrapHexagon:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }
                    break;
                default:
                        break;
            }

            MirDirection dir = (target == null || target == User) ? actor.NextMagicDirection : Functions.DirectionFromPoint(actor.CurrentLocation, target.CurrentLocation);

            Point location = target != null ? target.CurrentLocation : actor.NextMagicLocation;

            uint targetID = target != null ? target.ObjectID : 0;

            if ((magic.Range != 0) && (!Functions.InRange(actor.CurrentLocation, location, magic.Range)))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage("Target is too far.");
                }
                actor.ClearMagic();
                return;
            }

            GameScene.LogTime = CMain.Time + Globals.LogDelay;

            if (actor == User)
            {
                User.QueuedAction = new QueuedAction { Action = MirAction.Spell, Direction = dir, Location = User.CurrentLocation, Params = new List<object>() };
                User.QueuedAction.Params.Add(magic.Spell);
                User.QueuedAction.Params.Add(targetID);
                User.QueuedAction.Params.Add(location);
                User.QueuedAction.Params.Add(magic.Level);
            }
            else
            {
                Network.Enqueue(new C.Magic { ObjectID = actor.ObjectID, Spell = magic.Spell, Direction = dir, TargetID = targetID, Location = location, SpellTargetLock = CMain.SpellTargetLock });
            }
        }

        public static MirDirection MouseDirection(float ratio = 45F) //22.5 = 16
        {
            Point p = new Point(MouseLocation.X / CellWidth, MouseLocation.Y / CellHeight);
            if (Functions.InRange(new Point(OffSetX, OffSetY), p, 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), p);

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += ratio / 2;
            if (angle > 360) angle -= 360;

            return (MirDirection)(angle / ratio);
        }

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new PointF(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static double Distance(PointF p1, PointF p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public bool EmptyCell(Point p)
        {
            if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Height) return false;
            if ((M2CellInfo[p.X, p.Y].BackImage & 0x20000000) != 0 || (M2CellInfo[p.X, p.Y].FrontImage & 0x8000) != 0) // + (M2CellInfo[P.X, P.Y].FrontImage & 0x7FFF) != 0)
                return false;

            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return false;
            }

            return true;
        }

        private bool CanWalk(MirDirection dir)
        {
            return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1));
        }

        private bool CanWalk(MirDirection dir, out MirDirection outDir)
        {
            outDir = dir;          
            
            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
                return true;

            dir = Functions.NextDir(outDir);
            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
            {
                outDir = dir;
                return true;
            }

            dir = Functions.PreviousDir(outDir);
            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
            {
                outDir = dir;
                return true;
            }

            return false;
        }

        public bool CanFly(Point target)
        {
            Point location = User.CurrentLocation;
            while (location != target)
            {
                MirDirection dir = Functions.DirectionFromPoint(location, target);

                location = Functions.PointMove(location, dir, 1);

                if (location.X < 0 || location.Y < 0 || location.X >= GameScene.Scene.MapControl.Width || location.Y >= GameScene.Scene.MapControl.Height) return false;

                if (!GameScene.Scene.MapControl.ValidPoint(location)) return false;
            }

            return true;
        }


        public bool ValidPoint(Point p)
        {
            return (M2CellInfo[p.X, p.Y].BackImage & 0x20000000) == 0;
        }
        public bool HasTarget(Point p)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return true;
            }
            return false;
        }
        public bool CanHalfMoon(Point p, MirDirection d)
        {
            d = Functions.PreviousDir(d);
            for (int i = 0; i < 4; i++)
            {
                if (HasTarget(Functions.PointMove(p, d, 1))) return true;
                d = Functions.NextDir(d);
            }
            return false;
        }
        public bool CanCrossHalfMoon(Point p)
        {
            MirDirection dir = MirDirection.Up;
            for (int i = 0; i < 8; i++)
            {
                if (HasTarget(Functions.PointMove(p, dir, 1))) return true;
                dir = Functions.NextDir(dir);
            }
            return false;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Objects.Clear();

                MapButtons = 0;
                MouseLocation = Point.Empty;
                InputDelay = 0;
                NextAction = 0;

                M2CellInfo = null;
                Width = 0;
                Height = 0;

                Index = 0;
                FileName = String.Empty;
                Title = String.Empty;
                MiniMap = 0;
                BigMap = 0;
                Lights = 0;
                FloorValid = false;
                LightsValid = false;
                MapDarkLight = 0;
                Music = 0;

                AnimationCount = 0;
                Effects.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion



        public void RemoveObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].RemoveObject(ob);
        }
        public void AddObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].AddObject(ob);
        }
        public MapObject FindObject(uint ObjectID, int x, int y)
        {
            return M2CellInfo[x, y].FindObject(ObjectID);
        }
        public void SortObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].Sort();
        }
    }
}

