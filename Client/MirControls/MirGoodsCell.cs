using Client.MirGraphics;
using Client.MirScenes;
using SlimDX;

namespace Client.MirControls
{
    public sealed class MirGoodsCell : MirControl
    {
        public UserItem Item;

        public MirLabel NameLabel, PriceLabel, CountLabel;

        public bool MultipleAvailable = false;
        public MirImageControl NewIcon;

        public MirGoodsCell()
        {
            Size = new Size(130, 32);

            NameLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(49, 0),
            };

            CountLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                DrawControlTexture = true,
                Location = new Point(23, 17),
                ForeColour = Color.Yellow,
            };

            PriceLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(49, 14),
            };

            NewIcon = new MirImageControl
            {
                Index = 550,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(190, 5),
                NotControl = true,
                Visible = false
            };

            BeforeDraw += (o, e) => Update();
            AfterDraw += (o, e) => DrawItem();
        }

        private void Update()
        {
            NewIcon.Visible = false;

            if (Item == null || Item.Info == null) return;
            NameLabel.Text = Item.Info.FriendlyName;
            CountLabel.Text = (Item.Count <= 1) ? "" : Item.Count.ToString();

            NewIcon.Visible = !Item.IsShopItem || MultipleAvailable;

            PriceLabel.Text = string.Format("Price: {0} gold", (uint)(Item.Price() * GameScene.NPCRate));
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            GameScene.Scene.CreateItemLabel(Item, hideAdded: GameScene.HideAddedStoreStats);
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            GameScene.Scene.DisposeItemLabel();
            GameScene.HoverItem = null;
        }

        private void DrawItem()
        {
            if (Item == null || Item.Info == null) return;

            Size size = Libraries.Items.GetTrueSize(Item.Image);
            Point offSet = new Point((40 - size.Width)/2, (32 - size.Height)/2);
            Libraries.Items.Draw(Item.Image, offSet.X + DisplayLocation.X, offSet.Y + DisplayLocation.Y);

            CountLabel.Draw();
        }
    }
}
