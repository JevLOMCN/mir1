using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class SkillDialog : MirImageControl
    {
        private const int GridColumns = 5;
        private const int GridPadding = 10;
        private const int GridSpacing = 30;
        private readonly List<MirButton> _magicButtons = new List<MirButton>();
        private static readonly string[] KeyStrings =
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
        };

        public SkillDialog()
        {
            Index = 170;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Size = new Size(380, 308);
            Location = Center.Subtract(200, 0);
        }

        public void Toggle()
        {
            if (Visible) Hide();
            else Show();
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }

        public void Refresh()
        {
            for (int i = 0; i < _magicButtons.Count; i++)
                _magicButtons[i].Dispose();

            _magicButtons.Clear();

            if (GameScene.User?.Magics == null) return;

            int index = 0;
            foreach (var magic in GameScene.User.Magics)
            {
                var button = new MirButton
                {
                    Library = Libraries.MagIcon,
                    Index = magic.Icon,
                    HoverIndex = magic.Icon,
                    PressedIndex = magic.Icon,
                    Parent = this,
                    Hint = magic.Name,
                    Sound = SoundList.ButtonA
                };

                int column = index % GridColumns;
                int row = index / GridColumns;
                int x = GridPadding + column * (button.Size.Width + GridSpacing);
                int y = GridPadding + row * (button.Size.Height + GridSpacing);
                button.Location = new Point(x, y);

                button.Click += (o, e) =>
                {
                    new AssignKeyPanel(magic, 1, KeyStrings) { Actor = GameScene.User };
                };

                _magicButtons.Add(button);
                index++;
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
            Index = 170;
            Library = Libraries.Prguse;
            Location = Center;
            Parent = GameScene.Scene;
            Visible = true;

            MagicImage = new MirImageControl
            {
                Location = new Point(16, 16),
                Index = magic.Icon,
                Library = Libraries.MagIcon,
                Parent = this,
            };

            TitleLabel = new MirLabel
            {
                Location = new Point(80, 30),
                Parent = this,
                Size = new Size(230, 32),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                Text = string.Format(GameLanguage.SelectKey, magic.Name)
            };

            NoneButton = new MirButton
            {
                Index = 102,
                HoverIndex = 102,
                PressedIndex = 102,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(10, 64),
                CenterText = true,
                Text = "None"
            };
            NoneButton.Click += (o, e) => Key = 0;

            SaveButton = new MirButton
            {
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(155, Size.Height - 35),
                Index = 226,
                HoverIndex = 226,
                PressedIndex = 227,
                CenterText = true,
                Text = "Save"
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
                    Index = 102,
                    PressedIndex = 102,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(10 + 60 * (i % 6), 114 + 50 * (i / 6)),
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
                FKeys[i].Index = 102;
                FKeys[i].HoverIndex = 102;
                FKeys[i].PressedIndex = 102;
                FKeys[i].Visible = true;
                FKeys[i].Border = false;
            }

            int key = Key - KeyOffset;
            if (key < 0 || key > FKeys.Length) return;

            FKeys[key].Index = 102;
            FKeys[key].HoverIndex = 102;
            FKeys[key].PressedIndex = 102;
            FKeys[key].Border = true;
            FKeys[key].BorderColour = Color.Red;

        }
    }
}
