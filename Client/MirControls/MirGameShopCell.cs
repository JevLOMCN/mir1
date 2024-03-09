using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirScenes;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirControls
{
    public sealed class GameShopCell : MirImageControl
    {
        public MirLabel nameLabel, typeLabel, goldLabel, gpLabel, stockLabel, StockLabel, countLabel;
        public GameShopItem Item;
        public UserItem ShowItem;
        Rectangle ItemDisplayArea;
        public MirButton BuyItem, PreviewItem;
        public MirImageControl ViewerBackground;
        public byte Quantity = 1;
        public MirButton quantityUp, quantityDown;
        public MirLabel quantity;
        public GameShopViewer Viewer;

        public GameShopCell()
        {
            Size = new Size(125, 146);
            Index = 750;
            Library = Libraries.Prguse;
            MouseLeave += (o, e) =>
            {
                GameScene.Scene.DisposeItemLabel();
                GameScene.HoverItem = null;
                ShowItem = null;
            };

            nameLabel = new MirLabel
            {
                Size = new Size(125, 15),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(0, 13),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F),
            };

            goldLabel = new MirLabel
            {
                Size = new Size(95, 20),
                DrawFormat = TextFormatFlags.RightToLeft | TextFormatFlags.Right,
                Location = new Point(2, 102),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F)
            };

            gpLabel = new MirLabel
            {
                Size = new Size(95, 20),
                DrawFormat = TextFormatFlags.RightToLeft | TextFormatFlags.Right,
                Location = new Point(2, 81),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F)
            };

            StockLabel = new MirLabel
            {
                Size = new Size(40, 20),
                Location = new Point(53, 37),
                Parent = this,
                NotControl = true,
                ForeColour = Color.Gray,
                Font = new Font(Settings.FontName, 7F),
                Text = "STOCK:"
            };

            stockLabel = new MirLabel
            {
                Size = new Size(20, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(93, 37),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
            };

            countLabel = new MirLabel
            {
                Size = new Size(30, 20),
                DrawFormat = TextFormatFlags.Right,
                Location = new Point(16, 60),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
            };


            BuyItem = new MirButton
            {
                Index = 778,
                HoverIndex = 779,
                PressedIndex = 780,
                Location = new Point(42, 122),
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            BuyItem.Click += (o, e) =>
            {
                BuyProduct();
            };

            PreviewItem = new MirButton
            {
                Index = 781,
                HoverIndex = 782,
                PressedIndex = 783,
                Location = new Point(8, 122),
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
                Visible = false,
            };
            PreviewItem.Click += (o, e) =>
                {
                    GameScene.Scene.GameShopDialog.Viewer.Dispose();
                    GameScene.Scene.GameShopDialog.Viewer = new GameShopViewer
                    {
                        Parent = GameScene.Scene.GameShopDialog,
                        Visible = true,
                        Location = this.Location.X < 350 ? new Point(416, 115) : new Point(151, 115),
                    };
                    GameScene.Scene.GameShopDialog.Viewer.ViewerItem = Item;
                    GameScene.Scene.GameShopDialog.Viewer.UpdateViewer();
                };


            quantityUp = new MirButton
            {
                Index = 243,
                HoverIndex = 244,
                PressedIndex = 245,
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(97, 56),
                Sound = SoundList.ButtonA,
            };
            quantityUp.Click += (o, e) =>
            {
                if (CMain.Shift) Quantity += 10;
                else Quantity++;

                if (((decimal)(Quantity * Item.Count) / Item.Info.StackSize) > 5) Quantity = ((5 * Item.Info.StackSize) / Item.Count) > 99 ? Quantity = 99 : Quantity = (byte)((5 * Item.Info.StackSize) / Item.Count);
                if (Quantity >= 99) Quantity = 99;
                if (Item.Stock != 0 && Quantity > Item.Stock) Quantity = (byte)Item.Stock;
            };

            quantityDown = new MirButton
            {
                Index = 240,
                HoverIndex = 241,
                PressedIndex = 242,
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(55, 56),
                Sound = SoundList.ButtonA,
            };
            quantityDown.Click += (o, e) =>
            {

                if (CMain.Shift) Quantity -= 10;
                else Quantity--;

                if (Quantity <= 1 || Quantity > 99) Quantity = 1;
            };

            quantity = new MirLabel
            {
                Size = new Size(20, 13),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(74, 56),
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F),
            };



        }

        public void BuyProduct()
        {
            uint CreditCost;
            uint GoldCost;
            MirMessageBox messageBox;
            int pType = -1;
            if (GameScene.Scene.GameShopDialog.PaymentTypeCredit.Checked && Item.CanBuyCredit)
            {
                pType = 0;
            }
            else if (GameScene.Scene.GameShopDialog.PaymentTypeGold.Checked && Item.CanBuyGold)
            {
                pType = 1;
            }
            if (pType == -1)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("You MUST select a payment type!", ChatType.System);
                return;
            }
            switch (pType)
            {
                case 0: //  Credit
                    if (Item.CreditPrice * Quantity <= GameScene.Credit)
                    {
                        CreditCost = Item.CreditPrice * Quantity;
                        messageBox = new MirMessageBox(string.Format("Are you sure would you like to buy {1} x \n{0}({3}) for {2} Credits?", Item.Info.FriendlyName, Quantity, CreditCost, Item.Count), MirMessageBoxButtons.YesNo);
                        messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.GameshopBuy { GIndex = Item.GIndex, Quantity = Quantity, PType = pType });
                        messageBox.NoButton.Click += (o, e) => { };
                        messageBox.Show();
                    }
                    else
                        GameScene.Scene.ChatDialog.ReceiveChat("You can't afford the selected item.", ChatType.System);
                    break;
                case 1: //  Gold
                    if (Item.GoldPrice * Quantity <= GameScene.Gold)
                    {
                        GoldCost = Item.GoldPrice * Quantity;
                        messageBox = new MirMessageBox(string.Format("Are you sure would you like to buy{1} x \n{0}({3}) for {2} Gold?", Item.Info.FriendlyName, Quantity, GoldCost, Item.Count), MirMessageBoxButtons.YesNo);
                        messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.GameshopBuy { GIndex = Item.GIndex, Quantity = Quantity, PType = pType });
                        messageBox.NoButton.Click += (o, e) => { };
                        messageBox.Show();
                    }
                    else
                        GameScene.Scene.ChatDialog.ReceiveChat("You can't afford the selected item.", ChatType.System);
                    break;
                default:

                    return;
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (GameScene.HoverItem != null && (Item.Info.Index != GameScene.HoverItem.Info.Index))
            {
                GameScene.Scene.DisposeItemLabel();
                GameScene.HoverItem = null;
                ShowItem = null;
            }

            if (ShowItem == null && ItemDisplayArea.Contains(CMain.MPoint))
            {
                ShowItem = new UserItem(Item.Info) { MaxDura = Item.Info.Durability, CurrentDura = Item.Info.Durability, Count = Item.Count };
                GameScene.Scene.CreateItemLabel(ShowItem);
            }
            else if (ShowItem != null && !ItemDisplayArea.Contains(CMain.MPoint))
            {
                GameScene.Scene.DisposeItemLabel();
                GameScene.HoverItem = null;
                ShowItem = null;
            }
        }

        public void UpdateText()
        {
            nameLabel.Text = Item.Info.FriendlyName;
            nameLabel.Text = nameLabel.Text.Length > 17 ? nameLabel.Text.Substring(0, 17) : nameLabel.Text;
            nameLabel.ForeColour = GameScene.Scene.GradeNameColor(Item.Info.Grade);
            if (Item.CanBuyGold)
                goldLabel.Text = (Item.GoldPrice * Quantity).ToString("###,###,##0");
            if (Item.CanBuyCredit)
                gpLabel.Text = (Item.CreditPrice * Quantity).ToString("###,###,##0");
            if (Item.Stock >= 99) stockLabel.Text = "99+";
            if (Item.Stock == 0) stockLabel.Text = "∞";
            else stockLabel.Text = Item.Stock.ToString();
            countLabel.Text = Item.Count.ToString();

            if (Item.Info.Type == ItemType.Weapon || Item.Info.Type == ItemType.Armour)
            {
                PreviewItem.Visible = true;
                BuyItem.Location = new Point(75, 122);
            }
        }

        protected internal override void DrawControl()
        {
            
            base.DrawControl();

            if (Item == null) return;

            UpdateText();

            Size size = Libraries.Items.GetTrueSize(Item.Info.Image);
            Point offSet = new Point((32 - size.Width) / 2, (32 - size.Height) / 2);

            Libraries.Items.Draw(Item.Info.Image, offSet.X + DisplayLocation.X + 12, offSet.Y + DisplayLocation.Y + 40);
            ItemDisplayArea = new Rectangle(new Point(offSet.X + DisplayLocation.X + 12, offSet.Y + DisplayLocation.Y + 40), size);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Item = null;
            GameScene.HoverItem = null;
            ShowItem = null;
        }

    }

    public sealed class GameShopViewer : MirImageControl
    {

        public MirAnimatedControl PreviewImage, WeaponImage, WeaponImage2, MountImage;

        public int StartIndex = 0;
        public int Direction = 6;
        public GameShopItem ViewerItem;

        public MirButton RightDirection, LeftDirection, CloseButton;


        public GameShopViewer()
        {
            Index = 785;// 314;
            Library = Libraries.Prguse;// Libraries.Prguse2;
            Location = new Point(405, 108);
            BeforeDraw += GameShopViewer_BeforeDraw;
            //Click += (o, e) =>
            //{
            //Visible = false;
            //};

            CloseButton = new MirButton
            {
                HoverIndex = 362,
                Index = 361,
                Location = new Point(230, 8),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 363,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) =>
            {
                Visible = false;
            };

            WeaponImage = new MirAnimatedControl
            {
                Animated = false,
                Location = new Point(105, 160),
                AnimationCount = 6,
                AnimationDelay = 150,
                Index = 0,
                Library = Libraries.Prguse,
                Loop = true,
                Parent = this,
                UseOffSet = true,
                NotControl = true,
            };
            WeaponImage2 = new MirAnimatedControl
            {
                Animated = false,
                Location = new Point(105, 160),
                AnimationCount = 6,
                AnimationDelay = 150,
                Index = 0,
                Library = Libraries.Prguse,
                Loop = true,
                Parent = this,
                UseOffSet = true,
                NotControl = true,
            };
            MountImage = new MirAnimatedControl
            {
                Animated = false,
                Location = new Point(105, 160),
                AnimationCount = 8,
                AnimationDelay = 150,
                Index = 0,
                Library = Libraries.Prguse,
                Loop = true,
                Parent = this,
                UseOffSet = true,
                NotControl = true,
            };

            PreviewImage = new MirAnimatedControl
            {
                Animated = false,
                Location = new Point(105, 160),
                AnimationCount = 6,
                AnimationDelay = 150,
                Index = 0,
                Library = Libraries.Prguse,
                Loop = true,
                Parent = this,
                UseOffSet = true,
                NotControl = true,
            };

            RightDirection = new MirButton
            {
                Index = 243,
                HoverIndex = 244,
                PressedIndex = 245,
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(160, 282),
                Sound = SoundList.ButtonA,
            };
            RightDirection.Click += (o, e) =>
            {
                Direction++;
                if (Direction > 8) Direction = 1;

                UpdateViewer();
            };

            LeftDirection = new MirButton
            {
                Index = 240,
                HoverIndex = 241,
                PressedIndex = 242,
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(81, 282),
                Sound = SoundList.ButtonA,
            };
            LeftDirection.Click += (o, e) =>
            {
                Direction--;
                if (Direction == 0) Direction = 8;

                UpdateViewer();
            };

        }

        public void UpdateViewer()
        {
            this.Visible = true;
            if (ViewerItem.Info.Type == ItemType.Weapon) DrawWeapon();
            if (ViewerItem.Info.Type == ItemType.Armour) DrawArmour();
        }

        private void GameShopViewer_BeforeDraw(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void DrawMount()
        {
        }

        private void DrawWeapon()
        {
        }

        private void DrawArmour()
        {           
            
        }

        private void DrawTransform()
        {

        }

        private void DrawMask()
        {


        }

    }
}
