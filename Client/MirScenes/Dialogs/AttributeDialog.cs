using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class AttributeDialog : MirImageControl
    {
        MirButton ConfirmButton, CancelButton;
        MainAttributeRow[] Attributes;
        SecondaryAttributeRow[] SecondaryAttributes;
        MirLabel AvailablePointsLabel;

        int AvailablePoints => 0;

        public AttributeDialog()
        {
            Index = 173;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;

            var title = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Text = "Attribute Points"
            };
            title.Location = new Point(Size.Width / 2 - title.Size.Width / 2, 10);

            Attributes = new MainAttributeRow[Enum.GetNames(typeof(PrimaryAttribute)).Length];
            foreach (PrimaryAttribute attribute in Enum.GetValues(typeof(PrimaryAttribute)))
            {
                int index = (int)attribute;
                Attributes[index] = new MainAttributeRow(attribute)
                {
                    Location = new Point(8, 40 + 24 * index),
                    Parent = this
                };
            }

            SecondaryAttributes = new SecondaryAttributeRow[Enum.GetNames(typeof(SecondaryAttribute)).Length];
            foreach (SecondaryAttribute attribute in Enum.GetValues(typeof(SecondaryAttribute)))
            {
                int index = (int)attribute;
                SecondaryAttributes[index] = new SecondaryAttributeRow(attribute)
                {
                    Location = new Point(8 + 170 * (index / 3), 240 + 24 * (index % 3)),
                    Parent = this
                };
            }

            ConfirmButton = new MirButton
            {
                Index = 226,
                Location = new Point(Size.Width - 166, Size.Height - 42),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 227,
                Sound = SoundList.ButtonA,
                Text = "Confirm",
                CenterText = true
            };
            ConfirmButton.Click += (o, e) => Hide();

            CancelButton = new MirButton
            {
                Index = 226,
                Location = new Point(ConfirmButton.Location.X + 75, ConfirmButton.Location.Y),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 227,
                Sound = SoundList.ButtonA,
                Text = "Cancel",
                CenterText = true
            };
            CancelButton.Click += (o, e) => Hide();

            AvailablePointsLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(8, Size.Height - 37)
            };
        }

        public void Update()
        {
            AvailablePointsLabel.Text = $"Available Points: {AvailablePoints}";
        }

        public override void Show()
        {
            Update();
            base.Show();
        }
    }

    public sealed class MainAttributeRow : MirImageControl
    {
        public MirButton IncreaseButton, DecreaseButton;
        MirLabel LevelLabel, PointsLabel, DeltaLabel, ExperienceLabel;
        int level, points, experience, delta;

        public MainAttributeRow(PrimaryAttribute attribute)
        {
            DrawImage = false;
            Size = new Size(353, 24);

            var title = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Text = attribute.ToString()
            };

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(45, 0)
            };

            ExperienceLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(LevelLabel.Location.X + 140, 0)
            };

            DeltaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(ExperienceLabel.Location.X + 45, 0),
                ForeColour = Color.Yellow
            };

            IncreaseButton = new MirButton
            {
                Index = 1,
                Location = new Point(DeltaLabel.Location.X + 30, 0),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 2,
                Sound = SoundList.ButtonA
            };
            IncreaseButton.Click += (o, e) => Increase();

            DecreaseButton = new MirButton
            {
                Index = 3,
                Location = new Point(IncreaseButton.Location.X, 11),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 4,
                Sound = SoundList.ButtonA
            };
            DecreaseButton.Click += (o, e) => Decrease();

            PointsLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(IncreaseButton.Location.X + 30, 0)
            };

            Update();
        }

        void Update()
        {
            LevelLabel.Text = $"Lv {level}";
            ExperienceLabel.Text = experience.ToString();
            DeltaLabel.Text = $"{(delta >= 0 ? "+" : "")}{delta}";
            PointsLabel.Text = $"{(points > 0 ? "+" : "")}{points}";
            PointsLabel.Visible = points > 0;
        }

        void Increase()
        {
        }

        void Decrease()
        {
        }
    }

    public sealed class SecondaryAttributeRow : MirImageControl
    {
        public MirButton IncreaseButton, DecreaseButton;
        MirLabel PointsLabel, ValueLabel;
        int points, value;

        public SecondaryAttributeRow(SecondaryAttribute attribute)
        {
            DrawImage = false;
            Size = new Size(353, 24);

            var title = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Text = attribute.ToString()
            };

            ValueLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(55, 0)
            };

            PointsLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(ValueLabel.Location.X + 65, 0)
            };

            IncreaseButton = new MirButton
            {
                Index = 1,
                Location = new Point(PointsLabel.Location.X + 20, 0),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 2,
                Sound = SoundList.ButtonA
            };
            IncreaseButton.Click += (o, e) => Increase();

            DecreaseButton = new MirButton
            {
                Index = 3,
                Location = new Point(IncreaseButton.Location.X, 11),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 4,
                Sound = SoundList.ButtonA
            };
            DecreaseButton.Click += (o, e) => Decrease();

            Update();
        }

        void Update()
        {
            ValueLabel.Text = $"{(value > 0 ? "+" : "")}{value}";
            PointsLabel.Text = $"{(points > 0 ? "+" : "")}{points}";
        }

        void Increase()
        {
        }

        void Decrease()
        {
        }
    }
}
