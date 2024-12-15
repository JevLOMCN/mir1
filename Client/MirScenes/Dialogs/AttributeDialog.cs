using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
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

        public static uint BasePoints;
        public static uint LevelGain;
        uint MaximumPoints => BasePoints + (GameScene.User?.Level ?? 0) * LevelGain;

        public long AvailablePoints => MaximumPoints - SpentPoints;
        long SpentPoints => (GameScene.User?.AttributeValues.Values.Sum(attribute => attribute.Points) ?? 0) + DeltaPoints;
        long DeltaPoints => Attributes.Sum(a => a.Delta) + SecondaryAttributes.Sum(b => b.Delta);

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

            Attributes = new MainAttributeRow[(int)Attribute.Yang + 1];
            for (int i = 0; i <= (int)Attribute.Yang; i++)
            {
                Attribute attribute = (Attribute)i;
                Attributes[i] = new MainAttributeRow(attribute)
                {
                    Location = new Point(8, 40 + 24 * i),
                    Parent = this
                };
            }

            SecondaryAttributes = new SecondaryAttributeRow[(int)Attribute.Defence + 1 - (int)Attribute.Health];
            for (int i = (int)Attribute.Health; i <= (int)Attribute.Defence; i++)
            {
                Attribute attribute = (Attribute)i;
                int index = i - (int)Attribute.Health;
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
            ConfirmButton.Click += (o, e) => Confirm();

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

        public void Update(bool reset)
        {
            foreach (var a in Attributes)
                a.Update(reset);
            foreach (var b in SecondaryAttributes)
                b.Update(reset);

            AvailablePointsLabel.Text = $"Available Points: {AvailablePoints}";
            Enabled = true;
        }

        public override void Show()
        {
            Update(true);
            base.Show();
        }

        void Confirm()
        {
            if (DeltaPoints == 0) return;

            Enabled = false;
            Dictionary<Attribute, int> allDeltas = new Dictionary<Attribute, int>();
            foreach (var a in Attributes)
                allDeltas[a.Type] = a.Delta;
            foreach (var b in SecondaryAttributes)
                allDeltas[b.Type] = b.Delta;

            Network.Enqueue(new C.AttributeDeltas { Deltas = allDeltas});
        }
    }

    public class AttributeRow : MirImageControl
    {
        public Attribute Type;
        public MirButton IncreaseButton, DecreaseButton;
        protected MirLabel LevelLabel, PointsLabel, DeltaLabel, ExperienceLabel;
        protected uint level, points;
        protected int delta;
        protected ulong experience;

        public int Delta => delta;
        protected long PointsWithDelta => points + delta;

        public AttributeRow(Attribute attribute) {}

        public virtual void Update(bool reset)
        {
            if (reset)
                delta = 0;

            if (GameScene.User != null)
            {
                points = GameScene.User.AttributeValues[Type].Points;
                level = GameScene.User.AttributeValues[Type].Level;
                experience = GameScene.User.AttributeValues[Type].Experience;
            }    

            if (LevelLabel != null)
                LevelLabel.Text = $"Lv {level}";
            ExperienceLabel.Text = experience.ToString();
            if (DeltaLabel != null)
                DeltaLabel.Text = $"{(delta >= 0 ? "+" : "")}{delta}";
            PointsLabel.Text = $"{(PointsWithDelta > 0 ? "+" : "")}{PointsWithDelta}";
            PointsLabel.Visible = PointsWithDelta > 0;
        }

        protected void Increase()
        {
            AttributeDialog parent = (AttributeDialog)Parent;
            if (parent.AvailablePoints > 0)
                delta++;
            parent.Update(false);
        }

        protected void Decrease()
        {
            AttributeDialog parent = (AttributeDialog)Parent;
            if (points + delta > 0)
                delta--;
            parent.Update(false);
        }
    }

    public sealed class MainAttributeRow : AttributeRow
    {
        public MainAttributeRow(Attribute attribute) : base(attribute)
        {
            Type = attribute;
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

            Update(true);
        }
    }

    public sealed class SecondaryAttributeRow : AttributeRow
    {
        public SecondaryAttributeRow(Attribute attribute) : base(attribute)
        {
            Type = attribute;
            DrawImage = false;
            Size = new Size(353, 24);

            var title = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Text = attribute.ToString()
            };

            ExperienceLabel = new MirLabel
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
                Location = new Point(ExperienceLabel.Location.X + 65, 0)
            };

            IncreaseButton = new MirButton
            {
                Index = 1,
                Location = new Point(PointsLabel.Location.X + 25, 0),
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

            Update(true);
        }

        public override void Update(bool reset)
        {
            base.Update(reset);

            ExperienceLabel.Text = $"{(experience > 0 ? "+" : "")}{experience}";
        }
    }
}
