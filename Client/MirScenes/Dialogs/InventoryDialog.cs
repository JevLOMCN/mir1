using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class InventoryDialog : MirImageControl
    {
        public MirItemCell[] Grid;
        public MirItemCell[] QuestGrid;
        public MirImageControl GoldImage;
        public MirLabel GoldLabel;

        public MirButton CloseButton, QuestButton;

        public InventoryDialog()
        {
            Index = 169;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Visible = false;
            Location = new Point(40, 40);

            QuestButton = new MirButton
            {
                Index = 739,
                Library = Libraries.Prguse,
                Location = new Point(146, 7),
                Parent = this,
                Size = new Size(72, 23),
                Sound = SoundList.ButtonA,
                Visible = false
            };
            QuestButton.Click += Button_Click;

            CloseButton = new MirButton
            {
                Index = 54,
                Location = new Point(Size.Width - 86, Size.Height - 40),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 55,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            GoldImage = new MirImageControl
            {
                Library = Libraries.Prguse,
                Index = 38,
                Parent = this,
                Location = new Point(7, Size.Height - 40)
            };
            GoldImage.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null)
                    GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
            };

            GoldLabel = new MirLabel
            {
                Parent = this,
                AutoSize = true,
                NotControl = true,
                Location = new Point(45, Size.Height - 41)
            };

            Grid = new MirItemCell[6 * 10];

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int idx = 6 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = 6 + idx,
                        GridType = MirGridType.Inventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 60 + 11 + x, y % 5 * 46 + 16 + y % 5),
                    };

                    if (idx >= 30)
                        Grid[idx].Visible = false;
                }
            }

            QuestGrid = new MirItemCell[8 * 5];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    QuestGrid[8 * y + x] = new MirItemCell
                    {
                        ItemSlot = 8 * y + x,
                        GridType = MirGridType.QuestInventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 9 + x, y * 32 + 37 + y),
                        Visible = false
                    };
                }
            }
        }

        void Button_Click(object sender, EventArgs e)
        {
            Reset();
            QuestButton.Index = 198;

            foreach (var grid in QuestGrid)
            {
                grid.Visible = true;
            }
        }

        void Reset()
        {
            foreach (MirItemCell grid in QuestGrid)
            {
                grid.Visible = false;
            }

            foreach (MirItemCell grid in Grid)
            {
                grid.Visible = false;
            }
        }



        public void RefreshInventory()
        {
            Reset();
            QuestButton.Index = 739;

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 36)
                    grid.Visible = true;
                else
                    grid.Visible = false;
            }
        }

        public void Process()
        {
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
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

        public MirItemCell GetQuestCell(ulong id)
        {
            return QuestGrid.FirstOrDefault(t => t.Item != null && t.Item.UniqueID == id);
        }

        public void DisplayItemGridEffect(ulong id, int type = 0)
        {
            MirItemCell cell = GetCell(id);

            if (cell.Item == null) return;

            MirAnimatedControl animEffect = null;

            switch (type)
            {
                case 0:
                    animEffect = new MirAnimatedControl
                    {
                        Animated = true,
                        AnimationCount = 9,
                        AnimationDelay = 150,
                        Index = 410,
                        Library = Libraries.Prguse,
                        Location = cell.Location,
                        Parent = this,
                        Loop = false,
                        NotControl = true,
                        UseOffSet = true,
                        Blending = true,
                        BlendingRate = 1F
                    };
                    animEffect.AfterAnimation += (o, e) => animEffect.Dispose();
                    SoundManager.PlaySound(20000 + (ushort)Spell.MagicShield * 10);
                    break;
            }
        }
    }
    public sealed class BeltDialog : MirImageControl
    {
        public MirItemCell[] Grid;

        public BeltDialog()
        {
            Index = 73;
            AutoSize = false;
            Size = new Size(234, 35);
            Library = Libraries.Prguse;
            DrawImage = false;
            Visible = true;
            Location = new Point(20, GameScene.Scene.MainCharacterDialog.Location.Y - 40);

            BeforeDraw += BeltPanel_BeforeDraw;

            Grid = new MirItemCell[6];

            for (int x = 0; x < 6; x++)
            {
                Grid[x] = new MirItemCell
                {
                    ItemSlot = x,
                    Size = new Size(32, 32),
                    GridType = MirGridType.Inventory,
                    Library = Libraries.Items,
                    Parent = this,
                    Location = new Point(x * 39 + 3, 3)
            };
            }

        }

        private void BeltPanel_BeforeDraw(object sender, EventArgs e)
        {
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
}
