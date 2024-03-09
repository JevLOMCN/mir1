using System.Text.RegularExpressions;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirSounds;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Client.MirScenes.Dialogs
{
    public sealed class NewCharacterDialog : MirImageControl
    {
        private static readonly Regex Reg = new Regex(@"^[A-Za-z0-9]|[\u4e00-\u9fa5]{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength + "}$");
        private static readonly int[] ClassBodyShapes = new int[3]
        {
            2,
            4,
            5
        };

        private static readonly int[] ClassWeaponShapes = new int[3]
        {
            2,
            4,
            5
        };

        public MirImageControl CharacterDisplay, HeadDisplay, WeaponDisplay;

        public MirTextBox NameTextBox;
        public MirLabel Description;
        public MirButton HairLeft, HairRight, GenderLeft, GenderRight, ClassLeft, ClassRight;

        public MirClass Class;
        public MirGender Gender;
        public byte Hair;

        #region Descriptions
        public const string WarriorDescription =
            "<$Gender> Warrior\r\nWarriors are a class of great strength and vitality. They are not easily killed in battle and have the advantage of being able to use" +
            " a variety of heavy weapons and Armour. Therefore, Warriors favor attacks that are based on melee physical damage. They are weak in ranged" +
            " attacks, however the variety of equipment that are developed specifically for Warriors complement their weakness in ranged combat.";

        public const string WizardDescription =
            "<$Gender> Wizard\r\nWizards are a class of low strength and stamina, but have the ability to use powerful spells. Their offensive spells are very effective, but" +
            " because it takes time to cast these spells, they're likely to leave themselves open for enemy's attacks. Therefore, the physically weak wizards" +
            " must aim to attack their enemies from a safe distance.";

        public const string TaoistDescription =
            "<$Gender> Taoist\r\nTaoists are well disciplined in the study of Astronomy, Medicine, and others aside from Mu-Gong. Rather then directly engaging the enemies, their" +
            " specialty lies in assisting their allies with support. Taoists can summon powerful creatures and have a high resistance to magic, and is a class" +
            " with well balanced offensive and defensive abilities.";

        #endregion

        private SelectScene ParentScene => (SelectScene)Parent;

        public NewCharacterDialog()
        {
            Index = 136;
            Library = Libraries.Prguse;
            Location = new Point(Settings.ScreenWidth - 230, 200);

            NameTextBox = new MirTextBox
            {
                Location = new Point(31, 146),
                Parent = this,
                Size = new Size(100, 16),
                MaxLength = Globals.MaxCharacterNameLength
            };
            NameTextBox.TextBox.KeyPress += TextBox_KeyPress;
            NameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
            NameTextBox.SetFocus();

            CharacterDisplay = new MirImageControl
            {
                Library = Libraries.Body,
                Location = new Point(57, 115),
                Parent = this,
                NotControl = true,
                UseOffSet = true,
            };
            HeadDisplay = new MirImageControl
            {
                Library = Libraries.Head,
                Location = new Point(57, 115),
                Parent = this,
                NotControl = true,
                UseOffSet = true,
            };
            WeaponDisplay = new MirImageControl
            {
                Library = Libraries.Weapon,
                Location = new Point(57, 115),
                Parent = this,
                NotControl = true,
                UseOffSet = true,
            };

            Description = new MirLabel
            {
                Border = true,
                Location = new Point(-200, 150),
                Parent = this,
                Size = new Size(178, 170),
                Text = WarriorDescription,
            };

            HairLeft = new MirButton
            {
                HoverIndex = 32,
                Index = 33,
                Library = Libraries.Prguse,
                Location = new Point(42, 46),
                Parent = this,
                PressedIndex = 34
            };
            HairLeft.Click += (o, e) =>
            {
                Hair--;
                UpdateInterface();
            };

            HairRight = new MirButton
            {
                HoverIndex = 35,
                Index = 36,
                Library = Libraries.Prguse,
                Location = new Point(104, 46),
                Parent = this,
                PressedIndex = 37
            };
            HairRight.Click += (o, e) =>
            {
                Hair++;
                UpdateInterface();
            };

            ClassLeft = new MirButton
            {
                HoverIndex = 32,
                Index = 33,
                Library = Libraries.Prguse,
                Location = new Point(42, 76),
                Parent = this,
                PressedIndex = 34
            };
            ClassLeft.Click += (o, e) =>
            {
                Class--;
                UpdateInterface();
            };

            ClassRight = new MirButton
            {
                HoverIndex = 35,
                Index = 36,
                Library = Libraries.Prguse,
                Location = new Point(104, 76),
                Parent = this,
                PressedIndex = 37
            };
            ClassRight.Click += (o, e) =>
            {
                Class++;
                UpdateInterface();
            };

            GenderLeft = new MirButton
            {
                HoverIndex = 32,
                Index = 33,
                Library = Libraries.Prguse,
                Location = new Point(42, 106),
                Parent = this,
                PressedIndex = 34
            };
            GenderLeft.Click += (o, e) =>
            {
                Gender--;
                UpdateInterface();
            };

            GenderRight = new MirButton
            {
                HoverIndex = 35,
                Index = 36,
                Library = Libraries.Prguse,
                Location = new Point(104, 106),
                Parent = this,
                PressedIndex = 37
            };
            GenderRight.Click += (o, e) =>
            {
                Gender++;
                UpdateInterface();
            };
        }

        public override void Show()
        {
            base.Show();

            Class = MirClass.Warrior;
            Gender = MirGender.Male;
            Hair = 1;
            NameTextBox.Text = string.Empty;

            UpdateInterface();
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender == null) return;
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            if (ParentScene.CreateCharacterButton.Enabled)
                ParentScene.CreateCharacterButton.InvokeMouseClick(null);
        }
        private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                ParentScene.CreateCharacterButton.Enabled = false;
                NameTextBox.Border = false;
            }
            else if (!Reg.IsMatch(NameTextBox.Text))
            {
                ParentScene.CreateCharacterButton.Enabled = false;
                NameTextBox.Border = true;
                NameTextBox.BorderColour = Color.Red;
            }
            else
            {
                ParentScene.CreateCharacterButton.Enabled = true;
                NameTextBox.Border = true;
                NameTextBox.BorderColour = Color.Green;
            }
        }

        public event EventHandler OnCreateCharacter;
        public void CreateCharacter()
        {
            ParentScene.CreateCharacterButton.Enabled = false;

            if (OnCreateCharacter != null)
                OnCreateCharacter.Invoke(this, EventArgs.Empty);        
        }

        private void UpdateInterface()
        {
            CharacterDisplay.Index = 74 + (ClassBodyShapes[(int)Class] * 240) + (120 * (int)Gender);
            HeadDisplay.Index = 74 + (Hair * 240) + (120 * (int)Gender);
            WeaponDisplay.Index = 74 + (ClassWeaponShapes[(int)Class] * 240) + (120 * (int)Gender);

            switch (Class)
            {
                case MirClass.Warrior:
                    Description.Text = WarriorDescription;
                    break;
                case MirClass.Wizard:
                    Description.Text = WizardDescription;
                    break;
                case MirClass.Taoist:
                    Description.Text = TaoistDescription;
                    break;
            }
            Description.Text = Description.Text.Replace("<$Gender>", Gender.ToString());

            HairLeft.Visible = Hair > 1;
            HairRight.Visible = Hair < 21;
            ClassLeft.Visible = Class > MirClass.Warrior;
            ClassRight.Visible = Class < MirClass.Taoist;
            GenderLeft.Visible = Gender > MirGender.Male;
            GenderRight.Visible = Gender < MirGender.Female;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            OnCreateCharacter = null;
        }
    }
}
