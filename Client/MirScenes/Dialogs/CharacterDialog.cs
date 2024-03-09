using Client.MirControls;
using Client.MirGraphics;
using Client.MirObjects;
using Client.MirSounds;

namespace Client.MirScenes.Dialogs
{
    public sealed class CharacterDialog : MirImageControl
    {
        public MirButton CloseButton;

        public MirItemCell[] Grid;
        private MirGridType GridType;
        private UserObject Actor;

        public CharacterDialog(MirGridType gridType, UserObject actor)
        {
            Actor = actor;
            GridType = gridType;

            Index = 137;
            Library = Libraries.Prguse;
            Location = new Point(Settings.ScreenWidth - Size.Width - 28, GameScene.Scene.MainCharacterDialog.Location.Y - Size.Height);
            Movable = true;
            Sort = true;

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(241, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            Grid = new MirItemCell[Enum.GetNames(typeof(EquipmentSlot)).Length];

            Grid[(int)EquipmentSlot.Weapon] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = gridType,
                Parent = this,
                Location = new Point(293, 43),
                Size = new Size(60, 45)
            };


            Grid[(int)EquipmentSlot.Armour] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = gridType,
                Parent = this,
                Location = new Point(157, 69),
                Size = new Size(60, 45)
            };


            Grid[(int)EquipmentSlot.Helmet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = gridType,
                Parent = this,
                Location = new Point(157, 13),
                Size = new Size(60, 45)
            };

            Grid[(int)EquipmentSlot.Necklace] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = gridType,
                Parent = this,
                Location = new Point(19, 43),
                Size = new Size(60, 45)
            };

            Grid[(int)EquipmentSlot.BraceletL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = gridType,
                Parent = this,
                Location = new Point(87, 13),
                Size = new Size(60, 45)
            };

            Grid[(int)EquipmentSlot.BraceletR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = gridType,
                Parent = this,
                Location = new Point(225, 13),
                Size = new Size(60, 45)
            };

            Grid[(int)EquipmentSlot.RingL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = gridType,
                Parent = this,
                Location = new Point(87, 69),
                Size = new Size(60, 45)
            };

            Grid[(int)EquipmentSlot.RingR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = gridType,
                Parent = this,
                Location = new Point(225, 69),
                Size = new Size(60, 45)
            };
        }

        public override void Show()
        {
            if (Visible) return;
            Visible = true;
        }

        public override void Hide()
        {
            base.Hide();
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
