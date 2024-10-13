using System.Text.RegularExpressions;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Font = System.Drawing.Font;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class MainDialog : MirImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public MirImageControl TopBar, LeftCap, RightCap, Decoration1, Decoration2, Decoration3, Decoration4, HealthOrb, ManaOrb;
        public MirButton InventoryButton, CharacterButton, SkillButton, LipsButton, MiniMapButton, HelpButton, QuitButton;
        public MirLabel LocationLabel, NameLabel, MapLabel;

        public MainDialog()
        {
            Index = Settings.Resolution == 800 ? 69 : 14;
            Library = Settings.Resolution == 800 ? Libraries.Prguse : Libraries.Prguse2;

            LeftCap = new MirImageControl
            {
                Library = Library,
                Parent = this
            };
            RightCap = new MirImageControl
            {
                Library = Library,                
                Parent = this
            };
            TopBar = new MirImageControl
            {
                Library = Library,
                Parent = this
            };

            switch (Settings.Resolution)
            {
                default:
                    LeftCap.Index = 74;
                    RightCap.Index = 75;
                    TopBar.Index = 73;
                    break;
                case 1024:
                    LeftCap.Index = 17;
                    RightCap.Index = 18;
                    TopBar.Index = 16;
                    break;
                case 1280:
                    LeftCap.Index = 20;
                    RightCap.Index = 21;
                    TopBar.Index = 19;
                    break;
                case 1366:
                    LeftCap.Index = 23;
                    RightCap.Index = 24;
                    TopBar.Index = 22;
                    break;
                case 1920:
                    LeftCap.Index = 26;
                    RightCap.Index = 27;
                    TopBar.Index = 25;
                    break;
            }

            Location = new Point(Settings.ScreenWidth / 2 - Library.GetSize(Index).Width / 2, Settings.ScreenHeight - Size.Height);
            PixelDetect = true;

            TopBar.Location = new Point(-Location.X, -Location.Y);
            LeftCap.Location = new Point(-Location.X, TopBar.Size.Height - Location.Y);
            RightCap.Location = new Point(-Location.X + Settings.ScreenWidth - RightCap.Size.Width, TopBar.Size.Height - Location.Y);

            Decoration1 = new MirImageControl
            {
                Index = 156,
                Library = Libraries.Prguse,
                Parent = RightCap
            };
            Decoration1.Location = new Point(RightCap.Size.Width - Decoration1.Size.Width, RightCap.Size.Height - 298);

            Decoration2 = new MirImageControl
            {
                Index = 153,
                Library = Libraries.Prguse,
                Parent = TopBar
            };

            Decoration3 = new MirImageControl
            {
                Index = 154,
                Library = Libraries.Prguse,
                Parent = TopBar,
                Location = new Point(Decoration2.Size.Width, 0)
            };

            Decoration4 = new MirImageControl
            {
                Index = 155,
                Library = Libraries.Prguse,
                Parent = TopBar,
                Location = new Point(Decoration2.Size.Width + Decoration3.Size.Width + 20, 0)
            };

            LocationLabel = new MirLabel
            {
                AutoSize = true,
                Parent = Decoration4,
                ForeColour = Color.Yellow,
                Location = new Point(12, 6),
            };

            NameLabel = new MirLabel
            {
                AutoSize = true,
                Parent = TopBar,
                Location = new Point(TopBar.Size.Width - 95, 8),
            };

            MapLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(Size.Width - 110, - 118),
            };

            CharacterButton = new MirButton
            {
                Index = 0,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 258, 3),
                Parent = this,
                PressedIndex = 1,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Character, CMain.InputKeys.GetKey(KeybindOptions.Equipment))
            };
            CharacterButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDialog.Visible)
                    GameScene.Scene.CharacterDialog.Hide();
                else
                    GameScene.Scene.CharacterDialog.Show();
            };

            InventoryButton = new MirButton
            {
                Index = 2,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 225, 3),
                Parent = this,
                PressedIndex = 3,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Inventory, CMain.InputKeys.GetKey(KeybindOptions.Inventory))
            };
            InventoryButton.Click += (o, e) =>
            {
                if (GameScene.Scene.InventoryDialog.Visible)
                    GameScene.Scene.InventoryDialog.Hide();
                else
                    GameScene.Scene.InventoryDialog.Show();
            };

            SkillButton = new MirButton
            {
                Index = 4,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 192, 3),
                Parent = this,
                PressedIndex = 5,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Skills, CMain.InputKeys.GetKey(KeybindOptions.Skills))
            };
            SkillButton.Click += (o, e) =>
            {
            };

            LipsButton = new MirButton
            {
                Index = 6,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 159, 3),
                Parent = this,
                PressedIndex = 7,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Quests, CMain.InputKeys.GetKey(KeybindOptions.Quests))
            };
            LipsButton.Click += (o, e) =>
            {
            };

            MiniMapButton = new MirButton
            {
                Index = 8,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 126, 3),
                Parent = this,
                PressedIndex = 9,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Options, CMain.InputKeys.GetKey(KeybindOptions.Options))
            };
            MiniMapButton.Click += (o, e) =>
            {
                GameScene.Scene.BigMapDialog.Toggle();
            };

            HelpButton = new MirButton
            {
                Index = 10,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 93, 3),
                Parent = this,
                PressedIndex = 11,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Help, CMain.InputKeys.GetKey(KeybindOptions.Help))
            };
            HelpButton.Click += (o, e) =>
            {
            };

            QuitButton = new MirButton
            {
                Index = 12,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 51, 4),
                Parent = this,
                PressedIndex = 13,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Exit, CMain.InputKeys.GetKey(KeybindOptions.Exit))
            };
            QuitButton.Click += (o, e) =>
            {
                GameScene.Scene.QuitGame();
            };

            HealthOrb = new MirImageControl
            {
                Library = Libraries.Prguse,
                Index = 76,
                DrawImage = false,
                Parent = TopBar,
                Location = new Point(Settings.ScreenWidth - 362, 9),
                NotControl = true,
            };
            HealthOrb.BeforeDraw += HealthOrb_BeforeDraw;

            ManaOrb = new MirImageControl
            {
                Library = Libraries.Prguse,
                Index = 77,
                DrawImage = false,
                Parent = TopBar,
                Location = new Point(Settings.ScreenWidth - 224, 9),
                NotControl = true,
            };
            ManaOrb.BeforeDraw += ManaOrb_BeforeDraw;
        }

        public void Process()
        {
            /*if (Settings.HPView)
            {
                HealthLabel.Text = string.Format("HP {0}/{1}", User.HP, User.Stats[Stat.HP]);
                ManaLabel.Text = string.Format("MP {0}/{1} ", User.MP, User.Stats[Stat.MP]);
                TopLabel.Text = string.Empty;
                BottomLabel.Text = string.Empty;
            }
            else
            {
                if (HPOnly)
                {
                    TopLabel.Text = string.Format("{0}\n" + "--", User.HP);
                    BottomLabel.Text = string.Format("{0}", User.Stats[Stat.HP]);
                }
                else
                {
                    TopLabel.Text = string.Format(" {0}    {1} \n" + "---------------", User.HP, User.MP);
                    BottomLabel.Text = string.Format(" {0}    {1} ", User.Stats[Stat.HP], User.Stats[Stat.MP]);
                }
                HealthLabel.Text = string.Empty;
                ManaLabel.Text = string.Empty;
            }*/

            LocationLabel.Text = Functions.PointToString(MapObject.User.CurrentLocation);
            NameLabel.Text = MapObject.User?.Name ?? string.Empty;
        }

        private void HealthOrb_BeforeDraw(object sender, EventArgs e)
        {
            double percent = User.HP / (double)User.Stats[Stat.HP];
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((HealthOrb.Size.Width - 3) * percent), HealthOrb.Size.Height)
            };

            HealthOrb.Library.Draw(HealthOrb.Index, section, HealthOrb.DisplayLocation, Color.White, false);
        }

        private void ManaOrb_BeforeDraw(object sender, EventArgs e)
        {
            double percent = User.MP / (double)User.Stats[Stat.MP];
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((ManaOrb.Size.Width - 3) * percent), ManaOrb.Size.Height)
            };

            ManaOrb.Library.Draw(ManaOrb.Index, section, ManaOrb.DisplayLocation, Color.White, false);
        }
    }

    public sealed class MainCharacterDialog : MirImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }
        public MirLabel LevelLabel, ExperienceLabel, GoldLabel, WeightLabel, AccuracyLabel, DestructiveLabel, ACLabel, MACLabel, HPLabel, MPLabel, AgilityLabel;

        public MainCharacterDialog()
        {
            Index = 99;
            Library = Libraries.Prguse;
            Location = new Point(GameScene.Scene.MainDialog.Location.X + 16, Settings.ScreenHeight - Size.Height - 32);

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(433, 8)
            };

            ExperienceLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(538, 8)
            };

            GoldLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(645, 8)
            };

            AccuracyLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(423, 30),
                Size = new Size(40, 14),
            };

            DestructiveLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(532, 30),
                Size = new Size(40, 14),
            };

            ACLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(622, 30),
                Size = new Size(40, 14),
            };

            MACLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(720, 30),
                Size = new Size(40, 14),
            };

            HPLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(418, 55),
                Size = new Size(96, 14),
            };

            MPLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(557, 54),
                Size = new Size(96, 14),
            };

            AgilityLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(720, 54),
                Size = new Size(40, 14),
            };

            WeightLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(720, 76),
                Size = new Size(40, 17),
            };
        }

        public void Process()
        {
            LevelLabel.Text = User.Level.ToString();
            ExperienceLabel.Text = string.Format("{0:#0.##%}", User.Experience / (double)User.MaxExperience);
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
            AccuracyLabel.Text = MapObject.User.Stats[Stat.Accuracy].ToString();
            DestructiveLabel.Text = string.Format("{0}-{1}", MapObject.User.Class == MirClass.Warrior ? MapObject.User.Stats[Stat.MinDC] : MapObject.User.Stats[Stat.MinMC],
                MapObject.User.Class == MirClass.Warrior ? MapObject.User.Stats[Stat.MaxDC] : MapObject.User.Stats[Stat.MaxMC]);
            ACLabel.Text = string.Format("{0}-{1}", MapObject.User.Stats[Stat.MinAC],MapObject.User.Stats[Stat.MaxAC]);
            MACLabel.Text = string.Format("{0}-{1}", MapObject.User.Stats[Stat.MinMAC], MapObject.User.Stats[Stat.MaxMAC]);
            HPLabel.Text = $"{MapObject.User.HP}/{MapObject.User.Stats[Stat.HP]} ({MapObject.User.PercentHealth}%)";
            MPLabel.Text = $"{MapObject.User.MP}/{MapObject.User.Stats[Stat.MP]} ({MapObject.User.PercentMana}%)";
            AgilityLabel.Text = MapObject.User.Stats[Stat.Agility].ToString();
            WeightLabel.Text = (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.CurrentBagWeight).ToString();
        }
    }
    public sealed class ChatDialog : MirImageControl
    {
        public List<ChatHistory> FullHistory = new List<ChatHistory>();
        public List<ChatHistory> History = new List<ChatHistory>();
        public List<MirLabel> ChatLines = new List<MirLabel>();

        public List<ChatItem> LinkedItems = new List<ChatItem>();
        public List<MirLabel> LinkedItemButtons = new List<MirLabel>();

        public MirButton UpButton, DownButton, PositionBar;
        public MirTextBox ChatTextBox;
        public Font ChatFont = new Font(Settings.FontName, 8F);
        public string LastPM = string.Empty;

        public int StartIndex, LineCount = 7, WindowSize;
        public string ChatPrefix = "";

        public bool Transparent;

        public ChatDialog()
        {
            Index = 99;
            DrawImage = false;
            Library = Libraries.Prguse;
            Location = new Point(5, 4);

            KeyPress += ChatPanel_KeyPress;
            KeyDown += ChatPanel_KeyDown;
            MouseWheel += ChatPanel_MouseWheel;

            ChatTextBox = new MirTextBox
            {
                BackColour = Color.DarkGray,
                ForeColour = Color.Black,
                Parent = this,
                Size = new Size(503, 13),
                Location = new Point(1, 106),
                MaxLength = Globals.MaxChatLength,
                Visible = false,
                Font = ChatFont,
            };
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;
            ChatTextBox.TextBox.KeyDown += ChatTextBox_KeyDown;
            ChatTextBox.TextBox.KeyUp += ChatTextBox_KeyUp;

            UpButton = new MirButton
            {
                Index = 1,
                Library = Libraries.Prguse,
                Location = new Point(364, -1),
                Parent = this,
                PressedIndex = 2,
                Sound = SoundList.ButtonA,
            };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };

            DownButton = new MirButton
            {
                Index = 3,
                Library = Libraries.Prguse,
                Location = new Point(364, 83),
                Parent = this,
                PressedIndex = 4,
                Sound = SoundList.ButtonA,
            };
            DownButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex++;
                Update();
            };

            PositionBar = new MirButton
            {
                Index = 6,
                Library = Libraries.Prguse,
                Location = new Point(364, 11),
                Parent = this,
                Sound = SoundList.None,
            };
            PositionBar.OnMoving += PositionBar_OnMoving;
        }

        public void SetChatText(string newText)
        {
            string newMsg = ChatTextBox.Text += newText;

            if (newMsg.Length > Globals.MaxChatLength) return;

            ChatTextBox.Text = newMsg;
            ChatTextBox.SetFocus();
            ChatTextBox.Visible = true;
            ChatTextBox.TextBox.SelectionLength = 0;
            ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(ChatTextBox.Text))
                    {
                        string msg = ChatTextBox.Text;
                        bool send = true;

                        if (msg.ToUpper() == "@LEVELEFFECT")
                        {
                            Settings.LevelEffect = !Settings.LevelEffect;
                        }

                        if (msg.ToUpper() == "@TARGETDEAD")
                        {
                            Settings.TargetDead = !Settings.TargetDead;
                        }

                        if (msg.ToUpper().StartsWith("@GROUP "))
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 1)
                                GameScene.Scene.GroupDialog.ConfirmAddMember(parts[1]);
                            send = false;
                        }

                        if (send)
                            Network.Enqueue(new C.Chat
                            {
                                Message = msg,
                                LinkedItems = new List<ChatItem>(LinkedItems)
                            });

                        if (ChatTextBox.Text[0] == '/')
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 0)
                                LastPM = parts[0];
                        }
                    }
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    LinkedItems.Clear();
                    break;
                case (char)Keys.Escape:
                    e.Handled = true;
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    LinkedItems.Clear();
                    break;
            }
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int x = PositionBar.Location.X;
            int y = PositionBar.Location.Y;
            if (y >= 11 + 71 - PositionBar.Size.Height) y = 11 + 71 - PositionBar.Size.Height;
            if (y < 11) y = 11;

            int h = 100 - PositionBar.Size.Height;
            h = (int)((y - 11) / (h / (float)(History.Count - 1)));

            if (h != StartIndex)
            {
                StartIndex = h;
                Update();
            }

            PositionBar.Location = new Point(x, y);
        }

        public void ReceiveChat(string text, ChatType type)
        {
            Color foreColour, backColour;

            switch (type)
            {
                case ChatType.Hint:
                    backColour = Color.Transparent;
                    foreColour = Color.DarkGreen;
                    break;
                case ChatType.Announcement:
                    backColour = Color.Blue;
                    foreColour = Color.White;
                    GameScene.Scene.ChatNoticeDialog.ShowNotice(RegexFunctions.CleanChatString(text));
                    break;
                case ChatType.LineMessage:
                    backColour = Color.Blue;
                    foreColour = Color.White;
                    break;
                case ChatType.Shout:
                    backColour = Color.Yellow;
                    foreColour = Color.Black;
                    break;
                case ChatType.Shout2:
                    backColour = Color.Green;
                    foreColour = Color.White;
                    break;
                case ChatType.Shout3:
                    backColour = Color.Purple;
                    foreColour = Color.White;
                    break;
                case ChatType.System:
                    backColour = Color.Red;
                    foreColour = Color.White;
                    break;
                case ChatType.System2:
                    backColour = Color.DarkRed;
                    foreColour = Color.White;
                    break;
                case ChatType.Group:
                    backColour = Color.White;
                    foreColour = Color.Brown;
                    break;
                case ChatType.WhisperOut:
                    foreColour = Color.CornflowerBlue;
                    backColour = Color.White;
                    break;
                case ChatType.WhisperIn:
                    foreColour = Color.DarkBlue;
                    backColour = Color.White;
                    break;
                case ChatType.Guild:
                    backColour = Color.White;
                    foreColour = Color.Green;
                    break;
                case ChatType.LevelUp:
                    backColour = Color.FromArgb(255, 225, 185, 250);
                    foreColour = Color.Blue;
                    break;
                case ChatType.Relationship:
                    backColour = Color.Transparent;
                    foreColour = Color.HotPink;
                    break;
                default:
                    backColour = Color.Transparent;
                    foreColour = Color.Black;
                    break;
            }

            List<string> chat = new List<string>();

            int chatWidth = 370;
            int index = 0;

            for (int i = 1; i < text.Length; i++)
            {
                if (i - index < 0) continue;

                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    int offset = i - index;
                    int newIndex = i - 1;

                    var itemLinkMatches = RegexFunctions.ChatItemLinks.Matches(text.Substring(index)).Cast<Match>();

                    if (itemLinkMatches.Any())
                    {
                        var match = itemLinkMatches.SingleOrDefault(x => (x.Index < (i - index)) && (x.Index + x.Length > offset - 1));

                        if (match != null)
                        {
                            offset = match.Index;
                            newIndex = match.Index;
                        }
                    }

                    chat.Add(text.Substring(index, offset - 1));
                    index = newIndex;
                }
            }

            chat.Add(text.Substring(index, text.Length - index));
            
            if (StartIndex == History.Count - LineCount)
                StartIndex += chat.Count;

            for (int i = 0; i < chat.Count; i++)
                FullHistory.Add(new ChatHistory { Text = chat[i], BackColour = backColour, ForeColour = foreColour, Type = type });

            Update();
        }

        public void Update()
        {
            History = new List<ChatHistory>();

            for (int i = 0; i < FullHistory.Count; i++)
            {
                switch (FullHistory[i].Type)
                {
                    case ChatType.Normal:
                    case ChatType.LineMessage:
                        if (Settings.FilterNormalChat) continue;
                        break;
                    case ChatType.WhisperIn:
                    case ChatType.WhisperOut:
                        if (Settings.FilterWhisperChat) continue;
                        break;
                    case ChatType.Shout:
                    case ChatType.Shout2:
                    case ChatType.Shout3:
                        if (Settings.FilterShoutChat) continue;
                        break;
                    case ChatType.System:
                    case ChatType.System2:
                        if (Settings.FilterSystemChat) continue;
                        break;
                    case ChatType.Group:
                        if (Settings.FilterGroupChat) continue;
                        break;
                    case ChatType.Guild:
                        if (Settings.FilterGuildChat) continue;
                        break;
                }

                History.Add(FullHistory[i]);
            }

            for (int i = 0; i < ChatLines.Count; i++)
                ChatLines[i].Dispose();

            for (int i = 0; i < LinkedItemButtons.Count; i++)
                LinkedItemButtons[i].Dispose();

            ChatLines.Clear();
            LinkedItemButtons.Clear();

            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;

            if (History.Count > 1)
            {
                int h = 71 - PositionBar.Size.Height;
                h = (int)((h / (float)(History.Count - 1)) * StartIndex);
                PositionBar.Location = new Point(PositionBar.Location.X, 11 + h);
            }

            int y = 1;

            for (int i = StartIndex; i < History.Count; i++)
            {
                MirLabel temp = new MirLabel
                {
                    AutoSize = true,
                    BackColour = History[i].BackColour,
                    ForeColour = History[i].ForeColour,
                    Location = new Point(1, y),
                    OutLine = false,
                    Parent = this,
                    Text = History[i].Text,
                    Font = ChatFont,
                };
                temp.MouseWheel += ChatPanel_MouseWheel;
                ChatLines.Add(temp);

                temp.Click += (o, e) =>
                {
                    if (!(o is MirLabel l)) return;

                    string[] parts = l.Text.Split(':', ' ');
                    if (parts.Length == 0) return;

                    string name = Regex.Replace(parts[0], "[^A-Za-z0-9]", "");

                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = string.Format("/{0} ", name);
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                };

                string currentLine = History[i].Text;

                int oldLength = currentLine.Length;

                Capture capture = null;

                foreach (Match match in RegexFunctions.ChatItemLinks.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList())
                {
                    try
                    {
                        int offSet = oldLength - currentLine.Length;

                        capture = match.Groups[1].Captures[0];
                        string[] values = capture.Value.Split('/');
                        currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, values[0]);
                        string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                        Size size = TextRenderer.MeasureText(CMain.Graphics, text, temp.Font, temp.Size, TextFormatFlags.TextBoxControl);

                        ChatLink(values[0], ulong.Parse(values[1]), temp.Location.Add(new Point(size.Width - 10, 0)));
                    }
                    catch(Exception ex)
                    {
						//Temporary debug to catch unknown error
                        CMain.SaveError(ex.ToString());
                        CMain.SaveError(currentLine);
                        CMain.SaveError(capture.Value);
                        throw;
                    }
                }

                temp.Text = currentLine;

                y += 13;
                if (i - StartIndex == LineCount - 1) break;
            }

        }

        private void ChatLink(string name, ulong uniqueID, Point p)
        {
            UserItem item = GameScene.ChatItemList.FirstOrDefault(x => x.UniqueID == uniqueID);

            if (item != null)
            {
                MirLabel temp = new MirLabel
                {
                    AutoSize = true,
                    Visible = true,
                    Parent = this,
                    Location = p,
                    Text = name,
                    ForeColour = Color.Blue,
                    Sound = SoundList.ButtonC,
                    Font = ChatFont,
                    OutLine = false,
                };

                temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
                temp.MouseLeave += (o, e) =>
                {
                    GameScene.Scene.DisposeItemLabel();
                    temp.ForeColour = Color.Blue;
                };
                temp.MouseDown += (o, e) => temp.ForeColour = Color.Blue;
                temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

                temp.Click += (o, e) =>
                {
                    GameScene.Scene.CreateItemLabel(item);
                };

                LinkedItemButtons.Add(temp);
            }
        }


        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (StartIndex == 0) return;
                    StartIndex--;
                    break;
                case Keys.Home:
                    if (StartIndex == 0) return;
                    StartIndex = 0;
                    break;
                case Keys.Down:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex++;
                    break;
                case Keys.End:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex = History.Count - 1;
                    break;
                case Keys.PageUp:
                    if (StartIndex == 0) return;
                    StartIndex -= LineCount;
                    break;
                case Keys.PageDown:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex += LineCount;
                    break;
                default:
                    return;
            }
            Update();
            e.Handled = true;
        }
        private void ChatPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '@':
                case '!':
                case ' ':
                case (char)Keys.Enter:
                    ChatTextBox.SetFocus();
                    if (e.KeyChar == '!') ChatTextBox.Text = "!";
                    if (e.KeyChar == '@') ChatTextBox.Text = "@";
                    if (ChatPrefix != "") ChatTextBox.Text = ChatPrefix;

                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '/':
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = LastPM + " ";
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        private void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == History.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }
        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }

        public class ChatHistory
        {
            public string Text;
            public Color ForeColour, BackColour;
            public ChatType Type;
        }
    }

    public sealed class SkillBarDialog : MirImageControl
    {
        private readonly MirButton _switchBindsButton;

        public bool AltBind;
        public bool HasSkill = false;
        public byte BarIndex;

        //public bool TopBind = !Settings.SkillMode;
        public MirImageControl[] Cells = new MirImageControl[8];
        public MirLabel[] KeyNameLabels = new MirLabel[8];
        public MirLabel BindNumberLabel = new MirLabel();

        public MirImageControl[] CoolDowns = new MirImageControl[8];

        public SkillBarDialog()
        {
            Index = 2190;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point(0, BarIndex * 20);
            Visible = true;

            BeforeDraw += MagicKeyDialog_BeforeDraw;

            _switchBindsButton = new MirButton
            {
                Index = 2247,
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(16, 28),
                Location = new Point(0, 0)
            };
            _switchBindsButton.Click += (o, e) =>
            {
                //Settings.SkillSet = !Settings.SkillSet;

                Update();
            };

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new MirImageControl
                {
                    Index = -1,
                    Library = Libraries.MagIcon,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                };
                int j = i + 1;
                Cells[i].Click += (o, e) =>
                    {
                        GameScene.Scene.UseSpell(j + (8 * BarIndex));
                    };

                CoolDowns[i] = new MirImageControl
                {
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                    NotControl = true,
                    UseOffSet = true,
                    Opacity = 0.6F
                };
            }

            BindNumberLabel = new MirLabel
            {
                Text = "1",
                Font = new Font(Settings.FontName, 8F),
                ForeColour = Color.White,
                Parent = this,
                Location = new Point(0, 1),
                Size = new Size(10, 25),
                NotControl = true
            };

            for (var i = 0; i < KeyNameLabels.Length; i++)
            {
                KeyNameLabels[i] = new MirLabel
                {
                    Text = "F" + (i + 1),
                    Font = new Font(Settings.FontName, 8F),
                    ForeColour = Color.White,
                    Parent = this,
                    Location = new Point(i * 25 + 13, 0),
                    Size = new Size(25, 25),
                    NotControl = true
                };
            }
            OnMoving += SkillBar_OnMoving;
        }

        private void SkillBar_OnMoving(object sender, MouseEventArgs e)
        {
            if (BarIndex * 2 >= Settings.SkillbarLocation.Length) return;
            Settings.SkillbarLocation[BarIndex, 0] = this.Location.X;
            Settings.SkillbarLocation[BarIndex, 1] = this.Location.Y;
        }

        private string GetKey(int barindex, int i)
        {
            //KeybindOptions Type = KeybindOptions.Bar1Skill1;
            if ((barindex == 0) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill1);
            if ((barindex == 0) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill2);
            if ((barindex == 0) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill3);
            if ((barindex == 0) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill4);
            if ((barindex == 0) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill5);
            if ((barindex == 0) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill6);
            if ((barindex == 0) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill7);
            if ((barindex == 0) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill8);
            if ((barindex == 1) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill1);
            if ((barindex == 1) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill2);
            if ((barindex == 1) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill3);
            if ((barindex == 1) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill4);
            if ((barindex == 1) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill5);
            if ((barindex == 1) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill6);
            if ((barindex == 1) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill7);
            if ((barindex == 1) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill8);
            return "";
        }
                    

        void MagicKeyDialog_BeforeDraw(object sender, EventArgs e)
        {
            Libraries.Prguse.Draw(2193, new Point(DisplayLocation.X + 12, DisplayLocation.Y), Color.White, true, 0.5F);
        }

        public void Update()
        {
            HasSkill = false;
            foreach (var m in GameScene.User.Magics)
            {
                if ((m.Key < (BarIndex * 8)+1) || (m.Key > ((BarIndex + 1) * 8)+1)) continue;
                HasSkill = true;
            }

            if (!Visible) return;
            Index = 2190;
            _switchBindsButton.Index = 2247;
            BindNumberLabel.Text = (BarIndex + 1).ToString();
            BindNumberLabel.Location = new Point(0, 1);

            for (var i = 1; i <= 8; i++)
            {
                Cells[i - 1].Index = -1;

                int offset = BarIndex * 8;
                string key = GetKey(BarIndex, i);
                KeyNameLabels[i - 1].Text = key;

                foreach (var m in GameScene.User.Magics)
                {
                    if (m.Key != i + offset) continue;
                    HasSkill = true;
                    ClientMagic magic = MapObject.User.GetMagic(m.Spell);
                    if (magic == null) continue;

                    //string key = m.Key > 8 ? string.Format("CTRL F{0}", i) : string.Format("F{0}", m.Key);

                    Cells[i - 1].Index = magic.Icon*2;
                    Cells[i - 1].Hint = string.Format("{0}\nMP: {1}\nCooldown: {2}\nKey: {3}", magic.Name,
                        (magic.BaseCost + (magic.LevelCost * magic.Level)), Functions.PrintTimeSpanFromMilliSeconds(magic.Delay), key);

                    KeyNameLabels[i - 1].Text = "";
                }
            }
        }


        public void Process()
        {
            ProcessSkillDelay();
        }

        private void ProcessSkillDelay()
        {
            if (!Visible) return;

            int offset = BarIndex * 8;

            for (int i = 0; i < Cells.Length; i++)
            {
                foreach (var magic in GameScene.User.Magics)
                {
                    if (magic.Key != i + offset + 1) continue;

                    int totalFrames = 22;
                    long timeLeft = magic.CastTime + magic.Delay - CMain.Time;

                    if (timeLeft < 100)
                    {
                        if (timeLeft > 0) { 
                            CoolDowns[i].Visible = false;
                           // CoolDowns[i].Dispose();
                        }
                        else
                            continue;
                    }

                    int delayPerFrame = (int)(magic.Delay / totalFrames);
                    int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

                    if ((CMain.Time <= magic.CastTime + magic.Delay))
                    {
                        CoolDowns[i].Visible = true;
                        CoolDowns[i].Index = 1260 + startFrame;
                    }
                }
            }
        }

        public override void Show()
        {
            if (Visible) return;
            if (!HasSkill) return;
            Settings.SkillBar = true;
            Visible = true;
            Update();
        }

        public override void Hide()
        {
            if (!Visible) return;
            Settings.SkillBar = false;
            Visible = false;
        }
    }
    
    public sealed class OptionDialog : MirImageControl
    {
        public MirButton SkillModeOn, SkillModeOff;
        public MirButton SkillBarOn, SkillBarOff;
        public MirButton EffectOn, EffectOff;
        public MirButton DropViewOn, DropViewOff;
        public MirButton NameViewOn, NameViewOff;
        public MirButton HPViewOn, HPViewOff;
        public MirButton NewMoveOn, NewMoveOff;
        public MirButton ObserveOn, ObserveOff;
        public MirImageControl SoundBar, MusicSoundBar;
        public MirImageControl VolumeBar, MusicVolumeBar;

        public MirButton CloseButton;


        public OptionDialog()
        {
            Index = 411;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            BeforeDraw += OptionPanel_BeforeDraw;

            CloseButton = new MirButton
            {
                Index = 360,
                HoverIndex = 361,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 26, 5),
                Parent = this,
                Sound = SoundList.ButtonA,
                PressedIndex = 362,
            };
            CloseButton.Click += (o, e) => Hide();

            //tilde option
            SkillModeOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 451,
            };
            SkillModeOn.Click += (o, e) =>
            {
                GameScene.Scene.ChangeSkillMode(false);
            };

            //ctrl option
            SkillModeOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 454
            };
            SkillModeOff.Click += (o, e) =>
            {
                GameScene.Scene.ChangeSkillMode(true);
            };

            SkillBarOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            SkillBarOn.Click += (o, e) => Settings.SkillBar = true;

            SkillBarOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            SkillBarOff.Click += (o, e) => Settings.SkillBar = false;

            EffectOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            EffectOn.Click += (o, e) => Settings.Effect = true;

            EffectOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            EffectOff.Click += (o, e) => Settings.Effect = false;

            DropViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            DropViewOn.Click += (o, e) => Settings.DropView = true;

            DropViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            DropViewOff.Click += (o, e) => Settings.DropView = false;

            NameViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            NameViewOn.Click += (o, e) => Settings.NameView = true;

            NameViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            NameViewOff.Click += (o, e) => Settings.NameView = false;

            HPViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 463,
            };
            HPViewOn.Click += (o, e) =>
            {
                Settings.HPView = true;
                GameScene.Scene.ChatDialog.ReceiveChat("[HP/MP Mode 1]", ChatType.Hint);
            };

            HPViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 466
            };
            HPViewOff.Click += (o, e) =>
            {
                Settings.HPView = false;
                GameScene.Scene.ChatDialog.ReceiveChat("[HP/MP Mode 2]", ChatType.Hint);
            };

            SoundBar = new MirImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 225),
                Parent = this,
                DrawImage = false,
            };
            SoundBar.MouseDown += SoundBar_MouseMove;
            SoundBar.MouseMove += SoundBar_MouseMove;
            SoundBar.BeforeDraw += SoundBar_BeforeDraw;

            VolumeBar = new MirImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 218),
                Parent = this,
                NotControl = true,
            };

            MusicSoundBar = new MirImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 251),
                Parent = this,
                DrawImage = false
            };
            MusicSoundBar.MouseDown += MusicSoundBar_MouseMove;
            MusicSoundBar.MouseMove += MusicSoundBar_MouseMove;
            MusicSoundBar.BeforeDraw += MusicSoundBar_BeforeDraw;

            MusicVolumeBar = new MirImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 244),
                Parent = this,
                NotControl = true,
            };

            NewMoveOn = new MirButton
            {
                Library = Libraries.Prguse,
                Location = new Point(159, 296),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 853,
            };
            NewMoveOn.Click += (o, e) =>
            {
                Settings.NewMove = true;
                GameScene.Scene.ChatDialog.ReceiveChat("[New Movement Style]", ChatType.Hint);
            };

            NewMoveOff = new MirButton
            {
                Library = Libraries.Prguse,
                Location = new Point(201, 296),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 850
            };
            NewMoveOff.Click += (o, e) =>
            {
                Settings.NewMove = false;
                GameScene.Scene.ChatDialog.ReceiveChat("[Old Movement Style]", ChatType.Hint);
            };

            ObserveOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            ObserveOn.Click += (o, e) => ToggleObserve(true);

            ObserveOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            ObserveOff.Click += (o, e) => ToggleObserve(false);
        }

        private void ToggleObserve(bool allow)
        {
            if (GameScene.AllowObserve == allow) return;

            Network.Enqueue(new C.Chat
            {
                Message = "@ALLOWOBSERVE",
            });
        }

        public void ToggleSkillButtons(bool Ctrl)
        {
            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if ((KeyCheck.function < KeybindOptions.Bar1Skill1) || (KeyCheck.function > KeybindOptions.Bar2Skill8)) continue;
                //need to test this 
                if ((KeyCheck.RequireCtrl != 1) && (KeyCheck.RequireTilde != 1)) continue;
                KeyCheck.RequireCtrl = (byte)(Ctrl ? 1 : 0);
                KeyCheck.RequireTilde = (byte)(Ctrl ? 0 : 1);
            }
        }

        private void SoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || SoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(SoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)SoundBar.Size.Width * 100);
            Settings.Volume = volume;

            double percent = Settings.Volume / 100D;

            SoundBar.Hint = $"{Settings.Volume}%";

            if (percent > 1) percent = 1;

            VolumeBar.Location = percent > 0 ? new Point(159 + (int)((SoundBar.Size.Width - 2) * percent), 218) : new Point(159, 218);
        }

        private void SoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (SoundBar.Library == null) return;

            double percent = Settings.Volume / 100D;

            SoundBar.Hint = $"{Settings.Volume}%";

            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((SoundBar.Size.Width - 2) * percent), SoundBar.Size.Height)
                };

                SoundBar.Library.Draw(SoundBar.Index, section, SoundBar.DisplayLocation, Color.White, false);
                VolumeBar.Location = new Point(159 + section.Size.Width, 218);
            }
            else
                VolumeBar.Location = new Point(159, 218);
        }

        private void MusicSoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (MusicSoundBar.Library == null) return;

            double percent = Settings.MusicVolume / 100D;

            MusicSoundBar.Hint = $"{Settings.MusicVolume}%";

            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((MusicSoundBar.Size.Width - 2) * percent), MusicSoundBar.Size.Height)
                };

                MusicSoundBar.Library.Draw(MusicSoundBar.Index, section, MusicSoundBar.DisplayLocation, Color.White, false);
                MusicVolumeBar.Location = new Point(159 + section.Size.Width, 244);
            }
            else
                MusicVolumeBar.Location = new Point(159, 244);
        }

        private void MusicSoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || MusicSoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(MusicSoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)MusicSoundBar.Size.Width * 100);
            Settings.MusicVolume = volume;

            double percent = Settings.MusicVolume / 100D;

            MusicSoundBar.Hint = $"{Settings.MusicVolume}%";

            if (percent > 1) percent = 1;

            MusicVolumeBar.Location = percent > 0 ? new Point(159 + (int)((MusicSoundBar.Size.Width - 2) * percent), 244) : new Point(159, 244);
        }

        private void OptionPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (Settings.SkillMode)
            {
                SkillModeOn.Index = 452;
                SkillModeOff.Index = 453;
            }
            else
            {
                SkillModeOn.Index = 450;
                SkillModeOff.Index = 455;
            }

            if (Settings.SkillBar)
            {
                SkillBarOn.Index = 458;
                SkillBarOff.Index = 459;
            }
            else
            {
                SkillBarOn.Index = 456;
                SkillBarOff.Index = 461;
            }

            if (Settings.Effect)
            {
                EffectOn.Index = 458;
                EffectOff.Index = 459;
            }
            else
            {
                EffectOn.Index = 456;
                EffectOff.Index = 461;
            }

            if (Settings.DropView)
            {
                DropViewOn.Index = 458;
                DropViewOff.Index = 459;
            }
            else
            {
                DropViewOn.Index = 456;
                DropViewOff.Index = 461;
            }

            if (Settings.NameView)
            {
                NameViewOn.Index = 458;
                NameViewOff.Index = 459;
            }
            else
            {
                NameViewOn.Index = 456;
                NameViewOff.Index = 461;
            }

            if (Settings.HPView)
            {
                HPViewOn.Index = 464;
                HPViewOff.Index = 465;
            }
            else
            {
                HPViewOn.Index = 462;
                HPViewOff.Index = 467;
            }

            if (Settings.NewMove)
            {
                NewMoveOn.Index = 853;
                NewMoveOff.Index = 848;
            }
            else
            {
                NewMoveOn.Index = 851;
                NewMoveOff.Index = 850;
            }

            if (GameScene.AllowObserve)
            {
                ObserveOn.Index = 458;
                ObserveOff.Index = 459;
            }
            else
            {        
                ObserveOn.Index = 456;
                ObserveOff.Index = 461;
            }
        }

    }
    public sealed class MenuDialog : MirImageControl
    {
        public MirButton ExitButton,
                         HelpButton,
                         KeyboardLayoutButton,
                         RankingButton,
                         CraftingButton,
                         FriendButton,
                         MentorButton,
                         RelationshipButton,
                         GroupButton,
                         GuildButton;

        public MenuDialog()
        {
            Index = 567;
            Parent = GameScene.Scene;
            Library = Libraries.Prguse;
            Location = new Point(Settings.ScreenWidth - Size.Width, GameScene.Scene.MainDialog.Location.Y - this.Size.Height + 15);
            Sort = true;
            Visible = false;
            Movable = true;

            ExitButton = new MirButton
            {
                HoverIndex = 634,
                Index = 633,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 12),
                PressedIndex = 635,
                Hint = string.Format(GameLanguage.Exit, CMain.InputKeys.GetKey(KeybindOptions.Exit))
            };
            ExitButton.Click += (o, e) => GameScene.Scene.QuitGame();

            HelpButton = new MirButton
            {
                Index = 1970,
                HoverIndex = 1971,
                PressedIndex = 1972,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 50),
                Hint = string.Format(GameLanguage.Help, CMain.InputKeys.GetKey(KeybindOptions.Help))
            };
            HelpButton.Click += (o, e) =>
            {
                if (GameScene.Scene.HelpDialog.Visible)
                    GameScene.Scene.HelpDialog.Hide();
                else GameScene.Scene.HelpDialog.Show();
            };

            KeyboardLayoutButton = new MirButton
            {
                Index = 1973,
                HoverIndex = 1974,
                PressedIndex = 1975,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 69),
                Visible = true,
                Hint = "Keyboard (" + CMain.InputKeys.GetKey(KeybindOptions.Keybind) + ")"
            };
            KeyboardLayoutButton.Click += (o, e) =>
            {
                if (GameScene.Scene.KeyboardLayoutDialog.Visible)
                    GameScene.Scene.KeyboardLayoutDialog.Hide();
                else GameScene.Scene.KeyboardLayoutDialog.Show();
            };

            RankingButton = new MirButton
            {
                Index = 2000,
                HoverIndex = 2001,
                PressedIndex = 2002,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 88),
                Hint = string.Format(GameLanguage.Ranking, CMain.InputKeys.GetKey(KeybindOptions.Ranking))
                //Visible = false
            };
            RankingButton.Click += (o, e) =>
            {
                if (GameScene.Scene.RankingDialog.Visible)
                    GameScene.Scene.RankingDialog.Hide();
                else GameScene.Scene.RankingDialog.Show();
            };

            CraftingButton = new MirButton
            {
                Index = 2000,
                HoverIndex = 2001,
                PressedIndex = 2002,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 107),
                Visible = false
            };
            CraftingButton.Click += (o, e) =>
            {

            };

            FriendButton = new MirButton
            {
                Index = 1982,
                HoverIndex = 1983,
                PressedIndex = 1984,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 183),
                Visible = true,
                Hint = string.Format(GameLanguage.Friends, CMain.InputKeys.GetKey(KeybindOptions.Friends))
            };
            FriendButton.Click += (o, e) =>
            {
                if (GameScene.Scene.FriendDialog.Visible)
                    GameScene.Scene.FriendDialog.Hide();
                else GameScene.Scene.FriendDialog.Show();
            };

            RelationshipButton = new MirButton  /* lover button */
            {
                Index = 1988,
                HoverIndex = 1989,
                PressedIndex = 1990,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 221),
                Visible = true,
                Hint = string.Format(GameLanguage.Relationship, CMain.InputKeys.GetKey(KeybindOptions.Relationship))
            };
            RelationshipButton.Click += (o, e) =>
            {
                if (GameScene.Scene.RelationshipDialog.Visible)
                    GameScene.Scene.RelationshipDialog.Hide();
                else GameScene.Scene.RelationshipDialog.Show();
            };

            GroupButton = new MirButton
            {
                Index = 1991,
                HoverIndex = 1992,
                PressedIndex = 1993,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 240),
                Hint = string.Format(GameLanguage.Groups, CMain.InputKeys.GetKey(KeybindOptions.Group))
            };
            GroupButton.Click += (o, e) =>
            {
                if (GameScene.Scene.GroupDialog.Visible)
                    GameScene.Scene.GroupDialog.Hide();
                else GameScene.Scene.GroupDialog.Show();
            };

            GuildButton = new MirButton
            {
                Index = 1994,
                HoverIndex = 1995,
                PressedIndex = 1996,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 259),
                Hint = string.Format(GameLanguage.Guild, CMain.InputKeys.GetKey(KeybindOptions.Guilds))
            };
            GuildButton.Click += (o, e) =>
            {
                if (GameScene.Scene.GuildDialog.Visible)
                    GameScene.Scene.GuildDialog.Hide();
                else GameScene.Scene.GuildDialog.Show();
            };

        }


    }
    public sealed class MagicButton : MirControl
    {
        public MirImageControl LevelImage, ExpImage;
        public MirButton SkillButton;
        public MirLabel LevelLabel, NameLabel, ExpLabel, KeyLabel;
        public ClientMagic Magic;
        public MirImageControl CoolDown;

        string[] Prefixes = new string[] { "", "CTRL", "Shift" };

        public MagicButton()
        {
            Size = new Size(231, 33);

            SkillButton = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.MagIcon,
                Parent = this,
                Location = new Point(36, 0),
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) =>
            {
                new AssignKeyPanel(Magic, 1, new string[]
                    {
                            "F1",
                            "F2",
                            "F3",
                            "F4",
                            "F5",
                            "F6",
                            "F7",
                            "F8",
                            "Ctrl" + Environment.NewLine + "F1",
                            "Ctrl" + Environment.NewLine + "F2",
                            "Ctrl" + Environment.NewLine + "F3",
                            "Ctrl" + Environment.NewLine + "F4",
                            "Ctrl" + Environment.NewLine + "F5",
                            "Ctrl" + Environment.NewLine + "F6",
                            "Ctrl" + Environment.NewLine + "F7",
                            "Ctrl" + Environment.NewLine + "F8"
                    })
                { Actor = GameScene.User };

            };

            LevelImage = new MirImageControl
            {
                Index = 516,
                Library = Libraries.Prguse,
                Location = new Point(73, 7),
                Parent = this,
                NotControl = true,
            };

            ExpImage = new MirImageControl
            {
                Index = 517,
                Library = Libraries.Prguse,
                Location = new Point(73, 19),
                Parent = this,
                NotControl = true,
            };

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(88, 2),
                NotControl = true,
            };

            NameLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 2),
                NotControl = true,
            };

            ExpLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 15),
                NotControl = true,
            };

            KeyLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(2, 2),
                NotControl = true,
            };

            CoolDown = new MirImageControl
            {
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(36, 0),
                Opacity = 0.6F,
                NotControl = true,
                UseOffSet = true,
            };
        }

        public void Update(ClientMagic magic)
        {
            Magic = magic;

            NameLabel.Text = Magic.Name;

            LevelLabel.Text = Magic.Level.ToString();
            switch (Magic.Level)
            {
                case 0:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need1);
                    break;
                case 1:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need2);
                    break;
                case 2:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need3);
                    break;
                case 3:
                    ExpLabel.Text = "-";
                    break;
            }

            KeyLabel.Text = Magic.Key == 0 ? string.Empty : string.Format("{0}{1}F{2}",
                Prefixes[(Magic.Key - 1) / 8],
                Magic.Key > 8 ? Environment.NewLine : string.Empty,
                (Magic.Key - 1) % 8 + 1);

            switch (magic.Spell)
            {  //Warrior
                case Spell.Fencing:
                    SkillButton.Hint = string.Format("Fencing \n\nHitting accuracy will be increased in accordance\nwith practice level.\nPassive Skill\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Slaying:
                    SkillButton.Hint = string.Format("Slaying \n\nHitting accuracy and destructive power will\nbe increased in accordance with practive level.\nPassive Skill\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Thrusting:
                    SkillButton.Hint = string.Format("Dark Damage\nThrusting \n\nIncreases the reach of your hits destructive power\nwill increase in accordance with practive level.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.HalfMoon:
                    SkillButton.Hint = string.Format("Wind Damage\nHalfMoon \n\nCause damage to mobs in a semi circle with\nthe shock waves from your fast moving weapon.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.FlamingSword:
                    SkillButton.Hint = string.Format("Fire Damage\nFlamingSword \n\nCause additional damage by summoning the spirit\nof fire into weapon\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.ShoulderDash:
                    SkillButton.Hint = string.Format("ShoulderDash \n\nA warrior can push away mobs by charging\nthem with his shoulder, inflicting damage\nif they hit any obstacle.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                //Wizard
                case Spell.FireBall:
                    SkillButton.Hint = string.Format("Fireball \n\nInstant Casting\nMana Cost {2}\n\nElements of fire are gathered to form\na fireball. Throw at monsters for damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ThunderBolt:
                    SkillButton.Hint = string.Format("Thundebolt \n\nInstant Casting\nMana Cost {2}\n\nStrikes the foe with a lightning bolt \ninflicting high damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.GreatFireBall:
                    SkillButton.Hint = string.Format("GreatFireBall \n\nInstant Casting\nMana Cost {2}\n\nStronger then fire ball, Great Fire Ball\nwill fire up the mobs.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Repulsion:
                    SkillButton.Hint = string.Format("Repulsion \n\nInstant Casting\nMana Cost {2}\n\nPush away mobs useing the power of fire.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.HellFire:
                    SkillButton.Hint = string.Format("Hellfire \n\nInstant Casting\nMana Cost {2}\n\nShoots out a streak of fire attack\nthe monster in front.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Lightning:
                    SkillButton.Hint = string.Format("Lightning \n\nInstant Casting\nMana Cost {2}\n\nShoots out a steak of lightning to attack\nthe monster in front.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ElectricShock:
                    SkillButton.Hint = string.Format("ElectrickShock \n\nInstant Casting\nMana Cost {2}\n\nStrong shock wave hits the mob and the\nmob will not be able to move or the mob\nwill get confused and fight for you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Teleport:
                    SkillButton.Hint = string.Format("Teleport \n\nInstant Casting\nMana Cost {2}\n\nTeleport to a random spot.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FireWall:
                    SkillButton.Hint = string.Format("FireWall \n\nInstant Casting\nMana Cost {2}\n\nThis skill will build a fire wall at a designated\nspot to attack the monster passing the area.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FireBang:
                    SkillButton.Hint = string.Format("FireBang \n\nInstant Casting\nMana Cost {2}\n\nFirebang will burst out fire at a designated spot to\nburn all the monster within the area.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ThunderStorm:
                    SkillButton.Hint = string.Format("Thunderstorm \n\nInstant Casting\nMana Cost {2}\n\nThis skill will make a thunder storm with in a designated area \nto attack the monster with in.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MagicShield:
                    SkillButton.Hint = string.Format("MagicShield \n\nInstant Casting\nMana Cost {2}\n\nThis skill will use Mp to create protective\nlayer around you\nAttack will be absorbed by the protective layer\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FrostCrunch:
                    SkillButton.Hint = string.Format("FrostCrunch \n\nInstant Casting\nMana Cost {2}\n\nFreeze the elements in the air around the \nmonster to slow them down\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                //Taoist
                case Spell.SpiritSword:
                    SkillButton.Hint = string.Format("SpiritSword \n\nIncreases the chance of hitting the target in\n melee combat.\nPassive Skill\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Healing:
                    SkillButton.Hint = string.Format("Healing \n\nInstant Casting\nMana Cost {2}\n\nHeals a single target \nrecovering HP over time.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Poisoning:
                    SkillButton.Hint = string.Format("Poisoning \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Poison Powder\n\nThrow poison at mobs to weaken them.\nUse green poison to weaken Hp.\nUse red poison to weaken defense.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SoulFireBall:
                    SkillButton.Hint = string.Format("SoulFireBall \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nPut power into a scroll and throw it at \na mob. The scroll will burst into fire.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SoulShield:
                    SkillButton.Hint = string.Format("SoulShield \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nBless the partymembers to strengthen there magic\ndefence.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.BlessedArmour:
                    SkillButton.Hint = string.Format("BlessedArmour \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nBless the partymemebers to strenghten there defence.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.TrapHexagon:
                    SkillButton.Hint = string.Format("TrapHexagon \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nTrap the monster with this magical power\n to stop them from moving. Any damages\nfrom outside source will allow the monsters\nto move again.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SummonSkeleton:
                    SkillButton.Hint = string.Format("SummonSkeleton \n\n\nInstant Casting\nMana Cost {2}\n\nSummons a Powerful AOE Skeleton, Which will Fight Side By Side With You\n\nRequired Items: Amulet.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Hiding:
                    SkillButton.Hint = string.Format("Hiding \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nMobs will not be able to spot you for a short\nmoment.Mobs will notice you if you start\nto move around.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MassHiding:
                    SkillButton.Hint = string.Format("MassHiding \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nMobs will not be able to spot you or your \nparty members for a short moment. \nMobs will notice you and your party if \nyou start to move around.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Revelation:
                    SkillButton.Hint = string.Format("Revelation \n\nInstant Casting\nMana Cost {2}\n\nYou will be able to read Hp of others\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MassHealing:
                    SkillButton.Hint = string.Format("MassHealing \n\nInstant Casting\nMana Cost {2}\n\nHeal all injured players in the specified\narea by surrounding them with mana.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.UltimateEnhancer:
                    SkillButton.Hint = string.Format("UltimateEnhancer \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nAbsorb the energy from the surroundings to increase the stats.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.EnergyRepulsor:
                    SkillButton.Hint = string.Format("EnergyRepulsor \n\nInstant Casting\nMana Cost {2}\n\nConcentrate your energy for one big blast to push away the monsters around you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Purification:
                    SkillButton.Hint = string.Format("Purification \n\nInstant Casting\nMana Cost {2}\n\nHelp others to recover from poisoning and\nparalysis useing this skill.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Hallucination:
                    SkillButton.Hint = string.Format("Hallucination \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nThe monster will only see hallucination \nand attack anyone on the way\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;

                default:

                    break;
            }
            

            SkillButton.Index = Magic.Icon * 2;
            SkillButton.PressedIndex = Magic.Icon * 2 + 1;

            SetDelay();
        }

        public void SetDelay()
        {
            if (Magic == null) return;

            int totalFrames = 34;

            long timeLeft = Magic.CastTime + Magic.Delay - CMain.Time;

            if (timeLeft < 100)
            {
                CoolDown.Visible = false;
                return;
            }

            int delayPerFrame = (int)(Magic.Delay / totalFrames);
            int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

            if ((CMain.Time <= Magic.CastTime + Magic.Delay))
            {
                CoolDown.Visible = true;
                CoolDown.Index = 1290 + startFrame;
            }
        }
    }
    public sealed class AssignKeyPanel : MirImageControl
    {
        public MirButton SaveButton, NoneButton;
        public UserObject Actor;
        public MirLabel TitleLabel;
        public MirImageControl MagicImage;
        public MirButton[] FKeys;

        public ClientMagic Magic;
        public byte Key;
        public byte KeyOffset;

        public AssignKeyPanel(ClientMagic magic, byte keyOffset, string[] keyStrings)
        {
            Magic = magic;
            Key = magic.Key;
            KeyOffset = keyOffset;

            Modal = true;
            Index = 710;
            Library = Libraries.Prguse;
            Location = Center;
            Parent = GameScene.Scene;
            Visible = true;

            MagicImage = new MirImageControl
            {
                Location = new Point(16, 16),
                Index = magic.Icon * 2,
                Library = Libraries.MagIcon,
                Parent = this,
            };

            TitleLabel = new MirLabel
            {
                Location = new Point(49, 17),
                Parent = this,
                Size = new Size(230, 32),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                Text = string.Format(GameLanguage.SelectKey, magic.Name)
            };

            NoneButton = new MirButton
            {
                Index = 287, //154
                HoverIndex = 288,
                PressedIndex = 289,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(284, 64),
            };
            NoneButton.Click += (o, e) => Key = 0;

            SaveButton = new MirButton
            {
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(284, 101),
                Index = 156,
                HoverIndex = 157,
                PressedIndex = 158,
            };
            SaveButton.Click += (o, e) =>
            {
                for (int i = 0; i < Actor.Magics.Count; i++)
                {
                    if (Actor.Magics[i].Key == Key)
                        Actor.Magics[i].Key = 0;
                }

                Network.Enqueue(new C.MagicKey { Spell = Magic.Spell, Key = Key, OldKey = Magic.Key });
                Magic.Key = Key;
                foreach (SkillBarDialog Bar in GameScene.Scene.SkillBarDialogs)
                    Bar.Update();

                Dispose();
            };

            FKeys = new MirButton[keyStrings.Length];

            for (byte i = 0; i < FKeys.Length; i++)
            {
                FKeys[i] = new MirButton
                {
                    Index = 0,
                    PressedIndex = 1,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(17 + 32 * (i % 8) + 5 * (i % 8 / 4), 58 + 37 * (i / 8)),
                    Sound = SoundList.ButtonA,
                    Text = keyStrings[i]
                };
                int num = i + keyOffset;
                FKeys[i].Click += (o, e) =>
                {
                    Key = (byte)num;
                };
            }

            BeforeDraw += AssignKeyPanel_BeforeDraw;
        }

        private void AssignKeyPanel_BeforeDraw(object sender, EventArgs e)
        {
            for (int i = 0; i < FKeys.Length; i++)
            {
                FKeys[i].Index = 1656;
                FKeys[i].HoverIndex = 1657;
                FKeys[i].PressedIndex = 1658;
                FKeys[i].Visible = true;
            }

            int key = Key - KeyOffset;
            if (key < 0 || key > FKeys.Length) return;

            FKeys[key].Index = 1658;
            FKeys[key].HoverIndex = 1658;
            FKeys[key].PressedIndex = 1658;
        }
    }
}
