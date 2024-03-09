using System.Globalization;
using System.Text.RegularExpressions;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Font = System.Drawing.Font;
using C = ClientPackets;
using System.Diagnostics;

namespace Client.MirScenes.Dialogs
{
    public sealed class NPCDialog : MirImageControl
    {
        public static Regex R = new Regex(@"<((.*?)\/(\@.*?))>");
        public static Regex C = new Regex(@"{((.*?)\/(.*?))}");
        public static Regex L = new Regex(@"\(((.*?)\/(.*?))\)");
        public static Regex B = new Regex(@"<<((.*?)\/(\@.*?))>>");

        public MirButton CloseButton, UpButton, DownButton, PositionBar, QuestButton, HelpButton;
        public MirLabel[] TextLabel;
        public List<MirLabel> TextButtons;
        public List<BigButton> BigButtons;
        public BigButtonDialog BigButtonDialog;
        private MirGeneratedBox GeneratedBox;

        Font font = new Font(Settings.FontName, 9F);

        public List<string> CurrentLines = new List<string>();
        private int _index = 0;
        public int MaximumLines = 8;

        public NPCDialog()
        {
            TextLabel = new MirLabel[30];
            TextButtons = new List<MirLabel>();
            BigButtons = new List<BigButton>();            
            AutoSize = false;
            Size = new Size(360, 160);
            PixelDetect = false;
            Location = new Point(40, 40);

            MouseWheel += NPCDialog_MouseWheel;

            Sort = true;

            GeneratedBox = new MirGeneratedBox(142, Size)
            {
                Parent = this,
                Visible = true
            };

            UpButton = new MirButton
            {
                Index = 197,
                HoverIndex = 198,
                PressedIndex = 199,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(417, 34),
                Sound = SoundList.ButtonA,
                Visible = false
            };
            UpButton.Click += (o, e) =>
            {
                if (_index <= 0) return;

                _index--;

                NewText(CurrentLines, false);
                UpdatePositionBar();
            };

            DownButton = new MirButton
            {
                Index = 207,
                HoverIndex = 208,
                Library = Libraries.Prguse2,
                PressedIndex = 209,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(417, 175),
                Sound = SoundList.ButtonA,
                Visible = false
            };
            DownButton.Click += (o, e) =>
            {
                if (_index + MaximumLines >= CurrentLines.Count) return;

                _index++;

                NewText(CurrentLines, false);
                UpdatePositionBar();
            };

            PositionBar = new MirButton
            {
                Index = 205,
                HoverIndex = 206,
                PressedIndex = 206,
                Library = Libraries.Prguse2,
                Location = new Point(417, 47),
                Parent = this,
                Movable = true,
                Sound = SoundList.None,
                Visible = false
            };
            PositionBar.OnMoving += PositionBar_OnMoving;            

            CloseButton = new MirButton
            {
                Index = 54,
                Location = new Point(Size.Width / 2 - 35, Size.Height - 30),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 55,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            HelpButton = new MirButton
            {
                Index = 257,
                HoverIndex = 258,
                PressedIndex = 259,
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(390, 3),
                Sound = SoundList.ButtonA,
            };
            HelpButton.Click += (o, e) => GameScene.Scene.HelpDialog.DisplayPage("Purchasing");

            BigButtonDialog = new BigButtonDialog()
            {
                Parent = this,               
            };

            QuestButton = new MirAnimatedButton()
            {
                Animated = true,
                AnimationCount = 10,
                Loop = true,
                AnimationDelay = 130,
                Index = 530,
                HoverIndex = 284,
                PressedIndex = 286,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(96, 25),
                Sound = SoundList.ButtonA,
                Visible = false
            };

            QuestButton.Click += (o, e) => GameScene.Scene.QuestListDialog.Toggle();
        }

        void NPCDialog_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (_index == 0 && count >= 0) return;
            if (_index == CurrentLines.Count - 1 && count <= 0) return;
            if (CurrentLines.Count <= MaximumLines) return;

            _index -= count;

            if (_index < 0) _index = 0;
            if (_index + MaximumLines > CurrentLines.Count - 1) _index = CurrentLines.Count - MaximumLines;

            NewText(CurrentLines, false);

            UpdatePositionBar();
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int x = 417;
            int y = PositionBar.Location.Y;

            if (y >= 155) y = 155;
            if (y <= 47) y = 47;

            int location = y - 47;
            int interval = 108 / (CurrentLines.Count - MaximumLines);

            double yPoint = location / interval;

            _index = Convert.ToInt16(Math.Floor(yPoint));

            NewText(CurrentLines, false);

            PositionBar.Location = new Point(x, y);
        }

        private void UpdatePositionBar()
        {
            if (CurrentLines.Count <= MaximumLines) return;

            int interval = 108 / (CurrentLines.Count - MaximumLines);

            int x = 417;
            int y = 48 + (_index * interval);

            if (y >= 155) y = 155;
            if (y <= 47) y = 47;

            PositionBar.Location = new Point(x, y);
        }

        private void ButtonClicked(string action)
        {
            if (action == "@Exit")
            {
                Hide();
                return;
            }

            if (CMain.Time <= GameScene.NPCTime) return;

            GameScene.NPCTime = CMain.Time + 5000;
            Network.Enqueue(new C.CallNPC { ObjectID = GameScene.NPCID, Key = $"[{action}]" });
        }


        public void NewText(List<string> lines, bool resetIndex = true)
        {
            Size = TrueSize;

            if (resetIndex)
            {
                _index = 0;
                CurrentLines = lines;
                UpdatePositionBar();
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    string currentLine = lines[i];

                    List<Match> bigButtonSortedList = B.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList();

                    for (int j = 0; j < bigButtonSortedList.Count; j++)
                    {
                        Match match = bigButtonSortedList[j];
                        Capture capture = match.Groups[1].Captures[0];
                        string txt = match.Groups[2].Captures[0].Value;
                        string action = match.Groups[3].Captures[0].Value;
                        string colourString = "RoyalBlue";

                        string[] actionSplit = action.Split('/');

                        action = actionSplit[0];
                        if (actionSplit.Length > 1)
                            colourString = actionSplit[1];

                        Color color = Color.FromName(colourString);

                        BigButton button = new BigButton
                        {
                            Index = 841,
                            HoverIndex = 842,
                            PressedIndex = 843,
                            Library = Libraries.Prguse,
                            Sound = SoundList.ButtonA,
                            Text = txt,
                            FontColour = Color.White,
                            ForeColour = color
                        };

                        button.Click += (o, e) =>
                        {
                            ButtonClicked(action);
                        };
                        BigButtons.Insert(0, button);
                    }

                    var bigButtonString = B.ToString();

                    var oldCurrentLine = currentLine;

                    currentLine = Regex.Replace(currentLine, bigButtonString, "");

                    if (string.IsNullOrWhiteSpace(currentLine) && oldCurrentLine != currentLine)
                        lines.RemoveAt(i);
                }

                if (BigButtons.Count > 0)
                {
                    int minimumButtons = 0;
                    if (string.IsNullOrWhiteSpace(string.Concat(lines)))
                    {
                        BigButtonDialog.Location = new Point(1, 27);
                        minimumButtons = 4;
                    }
                    else
                        BigButtonDialog.Location = new Point(1, Size.Height - 33);

                    BigButtonDialog.Show(BigButtons, minimumButtons);
                    Size = new Size(Size.Width, BigButtonDialog.Location.Y + BigButtonDialog.Size.Height);
                }
            }                
            
            if (lines.Count > MaximumLines)
            {
                Index = 385;
                UpButton.Visible = true;
                DownButton.Visible = true;
                PositionBar.Visible = true;
            }
            else
            {
                Index = 384;
                UpButton.Visible = false;
                DownButton.Visible = false;
                PositionBar.Visible = false;                
            }         

            QuestButton.Location = new Point(172, Size.Height - 30);

            for (int i = 0; i < TextButtons.Count; i++)
                TextButtons[i].Dispose();

            for (int i = 0; i < TextLabel.Length; i++)
            {
                if (TextLabel[i] != null) TextLabel[i].Text = "";
            }

            TextButtons.Clear();

            int lastLine = lines.Count > MaximumLines ? ((MaximumLines + _index) > lines.Count ? lines.Count : (MaximumLines + _index)) : lines.Count;

            for (int i = _index; i < lastLine; i++)
            {
                TextLabel[i] = new MirLabel
                {
                    Font = font,
                    DrawFormat = TextFormatFlags.WordBreak,
                    Visible = true,
                    Parent = this,
                    Size = new Size(420, 20),
                    Location = new Point(8, 8 + (i - _index) * 18),
                    NotControl = true
                };

                if (i >= lines.Count)
                {
                    TextLabel[i].Text = string.Empty;
                    continue;
                }

                string currentLine = lines[i];

                List<Match> matchList = R.Matches(currentLine).Cast<Match>().ToList();
                matchList.AddRange(C.Matches(currentLine).Cast<Match>());
                matchList.AddRange(L.Matches(currentLine).Cast<Match>());

                int oldLength = currentLine.Length;

                foreach (Match match in matchList.OrderBy(o => o.Index).ToList())
                {
                    int offSet = oldLength - currentLine.Length;

                    Capture capture = match.Groups[1].Captures[0];
                    string txt = match.Groups[2].Captures[0].Value;
                    string action = match.Groups[3].Captures[0].Value;

                    currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, txt);
                    string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                    Size size = TextRenderer.MeasureText(CMain.Graphics, text, TextLabel[i].Font, TextLabel[i].Size, TextFormatFlags.TextBoxControl);

                    if (R.Match(match.Value).Success)
                        NewButton(txt, action, TextLabel[i].Location.Add(new Point(size.Width - 10, 0)));

                    if (C.Match(match.Value).Success)
                        NewColour(txt, action, TextLabel[i].Location.Add(new Point(size.Width - 10, 0)));

                    if (L.Match(match.Value).Success)
                        NewButton(txt, null, TextLabel[i].Location.Add(new Point(size.Width - 10, 0)), action);
                }

                TextLabel[i].Text = currentLine;
                TextLabel[i].MouseWheel += NPCDialog_MouseWheel;
            }
        }

        private void NewButton(string text, string key, Point p, string link = "")
        {
            MirLabel temp = new MirLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = this,
                Location = p,
                Text = text,
                ForeColour = Color.Yellow,
                Sound = SoundList.ButtonC,
                Font = font
            };

            temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
            temp.MouseLeave += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseDown += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

            if (!string.IsNullOrEmpty(link))
            {
                temp.Click += (o, e) =>
                {
                    if (link.StartsWith("http://", true, CultureInfo.InvariantCulture))
                    {
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = link,
                            UseShellExecute = true
                        });
                    }
                };
            }
            else
            {
                temp.Click += (o, e) =>
                {
                    ButtonClicked(key);
                };
            }

            temp.MouseWheel += NPCDialog_MouseWheel;

            TextButtons.Add(temp);
        }

        private void NewColour(string text, string colour, Point p)
        {
            Color textColour = Color.FromName(colour);

            MirLabel temp = new MirLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = this,
                Location = p,
                Text = text,
                ForeColour = textColour,
                Font = font
            };
            temp.MouseWheel += NPCDialog_MouseWheel;

            TextButtons.Add(temp);
        }

        public void CheckQuestButtonDisplay()
        {

            QuestButton.Visible = false;

            NPCObject npc = (NPCObject)MapControl.GetObject(GameScene.NPCID);
            if (npc != null)
            {
                if (npc.GetAvailableQuests().Any())
                    QuestButton.Visible = true;
            }
        }

        public override void Hide()
        {
            Visible = false;
            GameScene.Scene.NPCGoodsDialog.Hide();
            GameScene.Scene.NPCSubGoodsDialog.Hide();
            GameScene.Scene.NPCDropDialog.Hide();
            GameScene.Scene.RefineDialog.Hide();
            GameScene.Scene.StorageDialog.Hide();
            GameScene.Scene.TrustMerchantDialog.Hide();
            GameScene.Scene.QuestListDialog.Hide();
            GameScene.Scene.InventoryDialog.Location = new Point(40, 40);
            GameScene.Scene.RollControl.Hide();
            BigButtonDialog.Hide();
        }

        public override void Show()
        {
            GameScene.Scene.InventoryDialog.Location = new Point(Location.X + Size.Width + 5, 40);
            Visible = true;

            CheckQuestButtonDisplay();
        }
    }
    public sealed class NPCGoodsDialog : MirImageControl
    {
        public PanelType PType;

        public int StartIndex;
        public UserItem SelectedItem;

        public List<UserItem> Goods = new List<UserItem>();
        public List<UserItem> DisplayGoods = new List<UserItem>();
        public MirGoodsCell[] Cells;
        public MirButton BuyButton, CloseButton;
        public MirImageControl BuyLabel;

        public MirButton UpButton, DownButton;
        private MirGeneratedBox GeneratedBox;

        public NPCGoodsDialog(PanelType type)
        {
            PType = type;
            Location = new Point(GameScene.Scene.NPCDialog.Location.X, GameScene.Scene.NPCDialog.Location.Y + GameScene.Scene.NPCDialog.Size.Height);
            Cells = new MirGoodsCell[4];
            Sort = true;
            DrawImage = false;
            AutoSize = false;
            Size = new Size(160, 160);
            GeneratedBox = new MirGeneratedBox(142, Size)
            {
                Parent = this,
                Visible = true
            };

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new MirGoodsCell
                {
                    Parent = this,
                    Location = new Point(13, 16 + i * 33),
                    Sound = SoundList.ButtonC
                };
                Cells[i].Click += (o, e) =>
                {
                    SelectedItem = ((MirGoodsCell)o).Item;
                    Update();
                };
                Cells[i].MouseWheel += NPCGoodsPanel_MouseWheel;
                Cells[i].DoubleClick += (o, e) =>
                {
                    BuyItem();
                };
            }

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(217, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            BuyButton = new MirButton
            {
                HoverIndex = 313,
                Index = 312,
                Location = new Point(77, 304),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 314,
                Sound = SoundList.ButtonA,
            };
            BuyButton.Click += (o, e) => BuyItem();

            BuyLabel = new MirImageControl
            {
                Index = 27,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(20, 9),
            };

            UpButton = new MirButton
            {
                Index = 1,
                Library = Libraries.Prguse,
                Location = new Point(Size.Width - 15, 5),
                Parent = this,
                PressedIndex = 2,
                Sound = SoundList.ButtonA
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
                Location = new Point(Size.Width - 15, Size.Height - 15),
                Parent = this,
                PressedIndex = 4,
                Sound = SoundList.ButtonA
            };
            DownButton.Click += (o, e) =>
            {
                if (DisplayGoods.Count <= 4) return;

                if (StartIndex == DisplayGoods.Count - 4) return;
                StartIndex++;
                Update();
            };
        }

        private bool CheckSubGoods()
        {
            if (SelectedItem == null) return false;

            if (PType == PanelType.Buy)
            {
                var list = Goods.Where(x => x.Info.Index == SelectedItem.Info.Index).ToList();

                if (list.Count > 1 || GameScene.Scene.NPCSubGoodsDialog.Visible)
                {
                    GameScene.Scene.NPCSubGoodsDialog.NewGoods(list);
                    GameScene.Scene.NPCSubGoodsDialog.Show();
                    return true;
                }
            }

            return false;
        }

        private void BuyItem()
        {
            if (SelectedItem == null) return;

            if (CheckSubGoods())
            {
                return;
            }

            if (SelectedItem.Info.StackSize > 1)
            {
                ushort tempCount = SelectedItem.Count;
                ushort maxQuantity = SelectedItem.Info.StackSize;

                SelectedItem.Count = maxQuantity;

                if (SelectedItem.Price() > GameScene.Gold)
                {
                    maxQuantity = Math.Min(ushort.MaxValue, (ushort)(GameScene.Gold / (SelectedItem.Price() / SelectedItem.Count)));
                    if (maxQuantity == 0)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                        return;
                    }
                }

                MapObject.User.GetMaxGain(SelectedItem);

                if (SelectedItem.Count == 0)
                {
                    SelectedItem.Count = tempCount;
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.NoBagSpace, ChatType.System);
                    return;
                }

                if (SelectedItem.Count < maxQuantity)
                {
                    maxQuantity = SelectedItem.Count;
                }

                if (SelectedItem.Count > tempCount)
                {
                    SelectedItem.Count = tempCount;
                }

                MirAmountBox amountBox = new("Purchase Amount:", SelectedItem.Image, Libraries.Items, maxQuantity, 0, SelectedItem.Count);

                amountBox.OKButton.Click += (o, e) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        Network.Enqueue(new C.BuyItem { ItemIndex = SelectedItem.UniqueID, Count = (ushort)amountBox.Amount, Type = PanelType.Buy });
                    }
                };

                amountBox.Show();
            }
            else
            {
                if (SelectedItem.Info.Price > GameScene.Gold)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    return;
                }

                for (int i = 0; i < MapObject.User.Inventory.Length; i++)
                {
                    if (MapObject.User.Inventory[i] == null) break;
                    if (i == MapObject.User.Inventory.Length - 1)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.NoBagSpace, ChatType.System);
                        return;
                    }
                }

                Network.Enqueue(new C.BuyItem { ItemIndex = SelectedItem.UniqueID, Count = 1, Type = PanelType.Buy });
            }
        }

        private void NPCGoodsPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == DisplayGoods.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void Update()
        {
            if (StartIndex > DisplayGoods.Count - 4) StartIndex = DisplayGoods.Count - 8;
            if (StartIndex <= 0) StartIndex = 0;



            for (int i = 0; i < 4; i++)
            {
                if (i + StartIndex >= DisplayGoods.Count)
                {
                    Cells[i].Visible = false;
                    continue;
                }
                Cells[i].Visible = true;

                var matchingGoods = Goods.Where(x => x.Info.Index == Cells[i].Item.Info.Index);

                Cells[i].Item = DisplayGoods[i + StartIndex];
                Cells[i].MultipleAvailable = matchingGoods.Count() > 1 && matchingGoods.Any(x => x.IsShopItem == false);
                Cells[i].Border = SelectedItem != null && Cells[i].Item == SelectedItem;
            }
        }

        public void NewGoods(IEnumerable<UserItem> list)
        {
            Goods.Clear();
            DisplayGoods.Clear();

            if (PType == PanelType.BuySub)
            {
                StartIndex = 0;
                SelectedItem = null;

                list = list.OrderBy(x => x.Price());
            }

            foreach (UserItem item in list)
            {
                //Normal shops just want to show one of each item type
                if (PType == PanelType.Buy)
                {
                    Goods.Add(item);
                    if (DisplayGoods.Any(x => x.Info.Index == item.Info.Index)) continue;
                }

                DisplayGoods.Add(item);
            }

            if (GameScene.Scene.NPCSubGoodsDialog.Visible)
            {
                CheckSubGoods();
            }

            Update();
        }

        public override void Hide()
        {
            Visible = false;
        }

        public override void Show()
        {
            Visible = true;

            GameScene.Scene.InventoryDialog.Show();
        }
    }
    public sealed class NPCDropDialog : MirImageControl
    {

        public readonly MirButton ConfirmButton, HoldButton;
        public readonly MirItemCell ItemCell;
        public MirItemCell OldCell;
        public readonly MirLabel InfoLabel;
        public PanelType PType;

        public static UserItem TargetItem;
        public bool Hold;


        public NPCDropDialog()
        {
            Index = 392;
            Library = Libraries.Prguse;
            Location = new Point(264, 224);
            Sort = true;

            Click += NPCDropPanel_Click;

            HoldButton = new MirButton
            {
                HoverIndex = 294,
                Index = 293,
                Location = new Point(114, 36),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 295,
                Sound = SoundList.ButtonA,
            };
            HoldButton.Click += (o, e) => Hold = !Hold;

            ConfirmButton = new MirButton
            {
                HoverIndex = 291,
                Index = 290,
                Location = new Point(114, 62),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 292,
                Sound = SoundList.ButtonA,
            };
            ConfirmButton.Click += (o, e) => Confirm();

            InfoLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(30, 10),
                Parent = this,
                NotControl = true,
            };

            ItemCell = new MirItemCell
            {
                BorderColour = Color.Lime,
                GridType = MirGridType.DropPanel,
                Library = Libraries.Items,
                Parent = this,
                Location = new Point(38, 72),
            };
            ItemCell.Click += (o, e) => ItemCell_Click();

            BeforeDraw += NPCDropPanel_BeforeDraw;
            AfterDraw += NPCDropPanel_AfterDraw;
        }

        private void NPCDropPanel_AfterDraw(object sender, EventArgs e)
        {
            if (Hold)
                Libraries.Prguse.Draw(295, 114 + DisplayLocation.X, 36 + DisplayLocation.Y);
        }

        private void NPCDropPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;

            if (me == null) return;
            int x = me.X - DisplayLocation.X;
            int y = me.Y - DisplayLocation.Y;

            if (new Rectangle(20, 55, 75, 75).Contains(x, y))
                ItemCell_Click();
        }

        private void Confirm()
        {
            if (TargetItem == null) return;

            switch (PType)
            {
                case PanelType.Sell:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontSell))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Cannot sell this item.", ChatType.System);
                        return;
                    }
                    if (GameScene.Gold + TargetItem.Price() / 2 <= uint.MaxValue)
                    {
                        Network.Enqueue(new C.SellItem { UniqueID = TargetItem.UniqueID, Count = TargetItem.Count });
                        TargetItem = null;
                        return;
                    }
                    GameScene.Scene.ChatDialog.ReceiveChat("Cannot carry anymore gold.", ChatType.System);
                    break;
                case PanelType.Repair:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontRepair))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Cannot repair this item.", ChatType.System);
                        return;
                    }
                    if (GameScene.Gold >= TargetItem.RepairPrice() * GameScene.NPCRate)
                    {
                        Network.Enqueue(new C.RepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    break;
                case PanelType.SpecialRepair:
                    if ((TargetItem.Info.Bind.HasFlag(BindMode.DontRepair)) || (TargetItem.Info.Bind.HasFlag(BindMode.NoSRepair)))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Cannot repair this item.", ChatType.System);
                        return;
                    }
                    if (GameScene.Gold >= (TargetItem.RepairPrice() * 3) * GameScene.NPCRate)
                    {
                        Network.Enqueue(new C.SRepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    break;
                case PanelType.Consign:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontStore) || TargetItem.Info.Bind.HasFlag(BindMode.DontSell))
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Cannot consign this item.", ChatType.System);
                        return;
                    }
                    MirAmountBox box = new MirAmountBox("Consignment Price:", TargetItem.Image, Libraries.Items, Globals.MaxConsignment, Globals.MinConsignment)
                    {
                        InputTextBox = { Text = string.Empty },
                        Amount = 0
                    };

                    box.Show();
                    box.OKButton.Click += (o, e) =>
                    {
                        Network.Enqueue(new C.ConsignItem { UniqueID = TargetItem.UniqueID, Price = box.Amount, Type = MarketPanelType.Consign });
                        TargetItem = null;
                    };
                    return;
                case PanelType.Refine:

                    for (int i = 0; i < GameScene.Scene.RefineDialog.Grid.Length; i++)
                    {
                        if (GameScene.Scene.RefineDialog.Grid[i].Item != null)
                        {
                            if (GameScene.Gold >= ((TargetItem.Info.RequiredAmount * 10) * GameScene.NPCRate))
                            {
                                Network.Enqueue(new C.RefineItem { UniqueID = TargetItem.UniqueID });
                                TargetItem = null;
                                return;
                            }
                            GameScene.Scene.ChatDialog.ReceiveChat(String.Format("You don't have enough gold to refine your {0}.", TargetItem.FriendlyName), ChatType.System);
                            return;
                        }

                    }
                    GameScene.Scene.ChatDialog.ReceiveChat(String.Format("You haven't deposited any items to refine your {0} with.", TargetItem.FriendlyName), ChatType.System);
                    break;
                case PanelType.CheckRefine:

                    if (TargetItem.RefineAdded == 0)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(String.Format("Your {0} hasn't been refined so it doesn't need checking.", TargetItem.FriendlyName), ChatType.System);
                        return;
                    }
                    Network.Enqueue(new C.CheckRefine { UniqueID = TargetItem.UniqueID });
                    break;

                case PanelType.ReplaceWedRing:

                    if (TargetItem.Info.Type != ItemType.Ring)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(String.Format("{0} isn't a ring.", TargetItem.FriendlyName), ChatType.System);
                        return;
                    }

                    Network.Enqueue(new C.ReplaceWedRing { UniqueID = TargetItem.UniqueID });
                    break;
            }


            TargetItem = null;
            OldCell.Locked = false;
            OldCell = null;
        }

        private void ItemCell_Click()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }

            if (GameScene.SelectedCell == null || GameScene.SelectedCell.GridType != MirGridType.Inventory ||
                (PType != PanelType.Sell && PType != PanelType.Consign && GameScene.SelectedCell.Item != null && GameScene.SelectedCell.Item.Info.Durability == 0))
                return;

            TargetItem = GameScene.SelectedCell.Item;
            OldCell = GameScene.SelectedCell;
            OldCell.Locked = true;
            GameScene.SelectedCell = null;
            if (Hold) Confirm();
        }

        private void NPCDropPanel_BeforeDraw(object sender, EventArgs e)
        {
            string text;

            HoldButton.Visible = true;

            Index = 351;
            Library = Libraries.Prguse2;
            Location = new Point(264, GameScene.Scene.NPCDialog.Size.Height);

            ConfirmButton.HoverIndex = 291;
            ConfirmButton.Index = 290;
            ConfirmButton.PressedIndex = 292;
            ConfirmButton.Location = new Point(114, 62);

            InfoLabel.Location = new Point(30, 10);

            ItemCell.Location = new Point(38, 72);

            switch (PType)
            {
                case PanelType.Sell:
                    text = "Sale: ";
                    break;
                case PanelType.Repair:
                    text = "Repair: ";
                    break;
                case PanelType.SpecialRepair:
                    text = "S. Repair: ";
                    break;
                case PanelType.Consign:
                    InfoLabel.Text = "Consignment: ";
                    return;
                case PanelType.Refine:
                    text = "Refine: ";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    GameScene.Scene.RefineDialog.Show();
                    break;
                case PanelType.CheckRefine:
                    text = "Check Refine";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    break;
                case PanelType.ReplaceWedRing:
                    text = "Replace: ";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    break;

                default: return;

            }

            if (TargetItem != null)
            {

                switch (PType)
                {
                    case PanelType.Sell:
                        text += (TargetItem.Price() / 2).ToString();
                        break;
                    case PanelType.Repair:
                        text += (TargetItem.RepairPrice() * GameScene.NPCRate).ToString();
                        break;
                    case PanelType.SpecialRepair:
                        text += ((TargetItem.RepairPrice() * 3) * GameScene.NPCRate).ToString();
                        break;
                    case PanelType.Refine:
                        text += ((TargetItem.Info.RequiredAmount * 10) * GameScene.NPCRate).ToString();
                        break;
                    case PanelType.ReplaceWedRing:
                        text += ((TargetItem.Info.RequiredAmount * 10) * GameScene.NPCRate).ToString();
                        break;
                    default: return;
                }

                text += " Gold";
            }

            InfoLabel.Text = text;
        }

        public override void Hide()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }
            Visible = false;
        }
        public override void Show()
        {
            Hold = false;
            GameScene.Scene.InventoryDialog.Show();
            Visible = true;
        }
    }
    public sealed class RefineDialog : MirImageControl
    {
        public MirItemCell[] Grid;
        public MirButton RefineButton;

        public RefineDialog()
        {
            Index = 1002;
            Library = Libraries.Prguse;
            Location = new Point(0, 225);
            Sort = true;

            MirImageControl TitleLabel = new MirImageControl
            {
                Index = 18,
                Library = Libraries.Prguse,
                Location = new Point(28, 8),
                Parent = this
            };


            Grid = new MirItemCell[4 * 4];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int idx = 4 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,
                        GridType = MirGridType.Refine,
                        Library = Libraries.Items,
                        Parent = this,
                        Size = new Size(34, 32),
                        Location = new Point(x * 34 + 12 + x, y * 32 + 37 + y),
                    };
                }
            }
        }

        public override void Hide()
        {
            if (!Visible) return;

            Visible = false;
            RefineCancel();
        }

        public void RefineCancel()
        {
            Network.Enqueue(new C.RefineCancel());
        }

        public void RefineReset()
        {
            for (int i = 0; i < Grid.Length; i++)
                Grid[i].Item = null;
        }



        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }

    }
    public sealed class StorageDialog : MirImageControl
    {
        public MirItemCell[] Grid;
        public MirButton CloseButton;

        public StorageDialog()
        {
            Index = 169;
            Library = Libraries.Prguse;
            Location = new Point(20, 40);
            Sort = true;
        
            CloseButton = new MirButton
            {
                Index = 54,
                Location = new Point(Size.Width / 2 - 35, Size.Height - 30),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 55,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            Grid = new MirItemCell[6 * 10];

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int idx = 6 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,
                        GridType = MirGridType.Storage,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 60 + 11 + x, y % 5 * 46 + 16 + y % 5),
                    };

                    if (idx >= 30)
                        Grid[idx].Visible = false;
                }
            }
        }

        public override void Show()
        {
            GameScene.Scene.InventoryDialog.Show();
            RefreshStorage1();

            Visible = true;
        }

        public void RefreshStorage1()
        {
            if (GameScene.User == null) return;

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 30)
                    grid.Visible = true;
                else
                    grid.Visible = false;
            }
        }

        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
    public sealed class BigButtonDialog : MirImageControl
    {
        const int MaximumRows = 8;
        private List<BigButton> CurrentButtons;
        private int ScrollOffset = 0;
        public BigButtonDialog()
        {
            Visible = false;
        }

        public void Show(List<BigButton> buttons, int minimumButtons)
        {
            if (Visible) return;
            CurrentButtons = buttons;

            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Dispose();
            Controls.Clear();
            Size = Size.Empty;
            ScrollOffset = 0;

            CurrentButtons.ToList().ForEach(b => b.MouseWheel += (o, e) => BigButtonDialog_MouseWheel(o, e));
            int count = Math.Max(minimumButtons, buttons.Count);
            for (int i = 0; i < Math.Min(count, MaximumRows); i++)
            {
                MirImageControl background = new MirImageControl()
                {
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(buttons.Count == 1 ? -1 : 0, Size.Height),
                    Index = count == 1 ? 836 : (i == 0 ? 838 : (i == count - 1 ? 840 : 839)),
                    NotControl = false,
                    Visible = true,
                };
                background.MouseWheel += (o, e) => BigButtonDialog_MouseWheel(o, e);
                Size = new Size(background.Size.Width, Size.Height + background.Size.Height);
            }

            RefreshButtons();

            MirImageControl footer = new MirImageControl()
            {
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(-1, Size.Height),
                Index = 837,
                NotControl = false,
                Visible = true,
            };
            Size = new Size(Size.Width, Size.Height + footer.Size.Height);

            if (buttons.Count > MaximumRows)
            {
                MirButton upButton = new MirButton
                {
                    Index = 197,
                    HoverIndex = 198,
                    PressedIndex = 199,
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Size = new Size(16, 14),
                    Sound = SoundList.ButtonA,
                    Location = new Point(Size.Width - 26, 17)
                };
                upButton.Click += (o, e) =>
                {
                    ScrollUp();
                };

                MirButton downButton = new MirButton
                {
                    Index = 207,
                    HoverIndex = 208,
                    Library = Libraries.Prguse2,
                    PressedIndex = 209,
                    Parent = this,
                    Size = new Size(16, 14),
                    Sound = SoundList.ButtonA,
                    Location = new Point(Size.Width - 26, Size.Height - 57)
                };
                downButton.Click += (o, e) =>
                {
                    ScrollDown();
                };
            }

            Visible = true;
        }

        public override void Hide()
        {
            Size = Size.Empty;
            Visible = false;
        }

        private void RefreshButtons()
        {
            CurrentButtons.ToList().ForEach(b => b.Visible = false);

            for (int i = 0; i < Math.Min(CurrentButtons.Count, MaximumRows); i++)
            {
                CurrentButtons[i + ScrollOffset].Parent = this;
                CurrentButtons[i + ScrollOffset].Visible = true;
                CurrentButtons[i + ScrollOffset].Location = new Point(97, 7 + i * 40);
            }            
        }

        private void BigButtonDialog_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (count > 0)
                ScrollUp();
            else if (count < 0)
                ScrollDown();
        }

        private void ScrollUp()
        {
            if (ScrollOffset <= 0) return;

            ScrollOffset--;
            RefreshButtons();
        }

        private void ScrollDown()
        {
            if (ScrollOffset + MaximumRows >= CurrentButtons.Count) return;

            ScrollOffset++;
            RefreshButtons();
        }
    }
    public sealed class BigButton : MirButton
    {
        #region Label
        private MirLabel _shadowLabel;
        #endregion

        #region CenterText
        public override bool CenterText
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                if (_center)
                {
                    _label.Size = Size;
                    _label.DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    _shadowLabel.Size = Size;
                    _shadowLabel.DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                }
                else
                {
                    _label.AutoSize = true;
                    _shadowLabel.AutoSize = true;
                }
            }
        }
        #endregion

        #region Font Colour
        public override Color FontColour
        {
            get
            {
                if (_label != null && !_label.IsDisposed)
                    return _label.ForeColour;
                return Color.Empty;
            }
            set
            {
                if (_label != null && !_label.IsDisposed)
                    _label.ForeColour = value;
            }
        }
        #endregion

        #region Size
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                _shadowLabel.Size = Size;
        }
        #endregion

        #region Text
        public override string Text
        {
            set
            {
                if (_label != null && !_label.IsDisposed)
                {
                    _label.Text = value;
                    _label.Visible = !string.IsNullOrEmpty(value);
                }

                if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                {
                    _shadowLabel.Text = value;
                    _shadowLabel.Visible = !string.IsNullOrEmpty(value);
                }
            }
        }
        #endregion
        public BigButton()
        {
            HoverIndex = -1;
            PressedIndex = -1;
            DisabledIndex = -1;
            Sound = SoundList.ButtonB;

            _shadowLabel = new MirLabel
            {
                NotControl = true,
                Parent = this,
                Location = new Point(2, 7),
                AutoSize = false,
                Size = new Size(237, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                ForeColour = Color.Black,
                Font = ScaleFont(new Font(Settings.FontName, 12F, FontStyle.Bold))
            };

            _label = new MirLabel
            {
                NotControl = true,
                Parent = this,
                Location = new Point(0, 5),
                AutoSize = false,
                Size = new Size(237, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter,                
                Font = ScaleFont(new Font(Settings.FontName, 12F, FontStyle.Bold))
            };
        }

        protected internal override void DrawControl()
        {
            base.DrawControl();

            if (DrawImage && Library != null)
            {
                bool oldGray = DXManager.GrayScale;

                if (GrayScale)
                {
                    DXManager.SetGrayscale(true);
                }

                if (Blending)
                    Library.DrawBlend(Index + 3, DisplayLocation, Color.White, false, BlendingRate);
                else
                    Library.Draw(Index + 3, DisplayLocation, Color.White, false, Opacity);

                if (GrayScale) DXManager.SetGrayscale(oldGray);
            }
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                _shadowLabel.Dispose();
            _shadowLabel = null;
        }
        #endregion
    }
}
