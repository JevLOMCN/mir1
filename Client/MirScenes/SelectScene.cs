using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirScenes.Dialogs;
using Client.MirSounds;
using C = ClientPackets;
using S = ServerPackets;
namespace Client.MirScenes
{
    public class SelectScene : MirScene
    {
        public MirImageControl Background;
        private NewCharacterDialog _character;

        public MirImageControl CharacterDisplayBackground, CharacterDisplay, HeadDisplay, WeaponDisplay;
        public MirButton StartGameButton, NewCharacterButton, DeleteCharacterButton, ExitGame, CreateCharacterButton, CancelCreateButton, CharacterLeft, CharacterRight;
        public MirLabel NameLabel, DescriptionLabel;
        public List<SelectInfo> Characters = new List<SelectInfo>();
        private int _selected;

        public SelectScene(List<SelectInfo> characters)
        {
            SoundManager.PlayMusic(SoundList.SelectMusic, true);
            Disposing += (o, e) => SoundManager.StopMusic();

            Characters = characters;
            SortList();

            KeyPress += SelectScene_KeyPress;

            Background = new MirImageControl
            {
                Index = 0,
                Library = Libraries.Prguse,
                Parent = this,
            };

            StartGameButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 11,
                Index = 10,
                Library = Libraries.Prguse,
                Location = new Point(410, 221),
                Parent = Background,
                PressedIndex = 9
            };
            StartGameButton.Click += (o, e) => StartGame();

            NewCharacterButton = new MirButton
            {
                HoverIndex = 17,
                Index = 16,
                Library = Libraries.Prguse,
                Location = new Point(410, 249),
                Parent = Background,
                PressedIndex = 15,
            };
            NewCharacterButton.Click += (o, e) => OpenNewCharacterDialog();

            DeleteCharacterButton = new MirButton
            {
                HoverIndex = 20,
                Index = 19,
                Library = Libraries.Prguse,
                Location = new Point(410, 277),
                Parent = Background,
                PressedIndex = 18
            };
            DeleteCharacterButton.Click += (o, e) => DeleteCharacter();

            ExitGame = new MirButton
            {
                HoverIndex = 26,
                Index = 25,
                Library = Libraries.Prguse,
                Location = new Point(410, 305),
                Parent = Background,
                PressedIndex = 24
            };
            ExitGame.Click += (o, e) => Program.Form.Close();

            CreateCharacterButton = new MirButton
            {
                HoverIndex = 17,
                Index = 16,
                Library = Libraries.Prguse,
                Location = new Point(410, 249),
                Parent = Background,
                PressedIndex = 15,
                Visible = false
            };
            CreateCharacterButton.Click += (o, e) => CreateCharacter();

            CancelCreateButton = new MirButton
            {
                HoverIndex = 26,
                Index = 25,
                Library = Libraries.Prguse,
                Location = new Point(410, 277),
                Parent = Background,
                PressedIndex = 24,
                Visible = false
            };
            CancelCreateButton.Click += (o, e) => CancelCreateCharacter();

            CharacterDisplayBackground = new MirImageControl
            {
                Index = 136,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 230, 200),
                Parent = Background
            };

            CharacterDisplay = new MirImageControl
            {
                Library = Libraries.Body,
                Location = new Point(57, 115),
                Parent = CharacterDisplayBackground,
                NotControl = true,
                UseOffSet = true,
            };
            HeadDisplay = new MirImageControl
            {
                Library = Libraries.Head,
                Location = new Point(57, 115),
                Parent = CharacterDisplayBackground,
                NotControl = true,
                UseOffSet = true,
            };
            WeaponDisplay = new MirImageControl
            {
                Library = Libraries.Weapon,
                Location = new Point(57, 115),
                Parent = CharacterDisplayBackground,
                NotControl = true,
                UseOffSet = true,
            };

            NameLabel = new MirLabel
            {
                Location = new Point(31, 146),
                Parent = CharacterDisplayBackground,
                Size = new Size(100, 16),
                DrawFormat = TextFormatFlags.HorizontalCenter,
            };

            DescriptionLabel = new MirLabel
            {
                Location = new Point(31, 166),
                Parent = CharacterDisplayBackground,
                Size = new Size(100, 16),
                DrawFormat = TextFormatFlags.HorizontalCenter,
            };

            CharacterLeft = new MirButton
            {
                HoverIndex = 32,
                Index = 33,
                Library = Libraries.Prguse,
                Location = new Point(65, 229),
                Parent = CharacterDisplayBackground,
                PressedIndex = 34
            };
            CharacterLeft.Click += (o, e) =>
            {
                _selected = _selected == 0 ? Characters.Count - 1 : _selected - 1;
                UpdateInterface();
            };

            CharacterRight = new MirButton
            {
                HoverIndex = 35,
                Index = 36,
                Library = Libraries.Prguse,
                Location = new Point(85, 229),
                Parent = CharacterDisplayBackground,
                PressedIndex = 37
            };
            CharacterRight.Click += (o, e) =>
            {
                _selected = (_selected + 1) % Characters.Count;
                UpdateInterface();
            };

            UpdateInterface();
        }

        private void SelectScene_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (StartGameButton.Enabled)
                StartGame();
            e.Handled = true;
        }


        public void SortList()
        {
            if (Characters != null)
                Characters.Sort((c1, c2) => c2.LastAccess.CompareTo(c1.LastAccess));
        }

        private void OpenNewCharacterDialog()
        {
            if (_character == null || _character.IsDisposed)
            {
                _character = new NewCharacterDialog { Parent = this };

                _character.OnCreateCharacter += (o, e) =>
                {
                    Network.Enqueue(new C.NewCharacter
                    {
                        Name = _character.NameTextBox.Text,
                        Class = _character.Class,
                        Gender = _character.Gender,
                        Hair = _character.Hair
                    });
                };
            }

            _character.Show();
            StartGameButton.Visible = false;
            NewCharacterButton.Visible = false;
            DeleteCharacterButton.Visible = false;
            ExitGame.Visible = false;

            CreateCharacterButton.Visible = true;
            CancelCreateButton.Visible = true;
        }

        private void CreateCharacter()
        {
            if (_character == null || _character.IsDisposed) return;

            _character.CreateCharacter();
        }

        private void CancelCreateCharacter()
        {
            if (_character == null || _character.IsDisposed) return;

            _character.Dispose();
            CreateCharacterButton.Visible = false;
            CancelCreateButton.Visible = false;
            StartGameButton.Visible = true;
            NewCharacterButton.Visible = true;
            DeleteCharacterButton.Visible = true;
            ExitGame.Visible = true;
        }


        public void StartGame()
        {
            if (!Libraries.Loaded)
            {
                MirAnimatedControl loadProgress = new MirAnimatedControl
                {
                    Library = Libraries.Prguse,
                    Index = 940,
                    Visible = true,
                    Parent = this,
                    Location = new Point(470, 680),
                    Animated = true,
                    AnimationCount = 9,
                    AnimationDelay = 100,
                    Loop = true,
                };
                loadProgress.AfterDraw += (o, e) =>
                {
                    if (!Libraries.Loaded) return;
                    loadProgress.Dispose();
                    StartGame();
                };
                return;
            }
            StartGameButton.Enabled = false;

            Network.Enqueue(new C.StartGame
            {
                CharacterIndex = Characters[_selected].Index
            });
        }

        public override void Process()
        {


        }
        public override void ProcessPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.NewCharacter:
                    NewCharacter((S.NewCharacter)p);
                    break;
                case (short)ServerPacketIds.NewCharacterSuccess:
                    NewCharacter((S.NewCharacterSuccess)p);
                    break;
                case (short)ServerPacketIds.DeleteCharacter:
                    DeleteCharacter((S.DeleteCharacter)p);
                    break;
                case (short)ServerPacketIds.DeleteCharacterSuccess:
                    DeleteCharacter((S.DeleteCharacterSuccess)p);
                    break;
                case (short)ServerPacketIds.StartGame:
                    StartGame((S.StartGame)p);
                    break;
                case (short)ServerPacketIds.StartGameBanned:
                    StartGame((S.StartGameBanned)p);
                    break;
                case (short)ServerPacketIds.StartGameDelay:
                    StartGame((S.StartGameDelay)p);
                    break;
                default:
                    base.ProcessPacket(p);
                    break;
            }
        }

        private void NewCharacter(S.NewCharacter p)
        {
            CreateCharacterButton.Enabled = true;

            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Creating new characters is currently disabled.");
                    _character.Dispose();
                    break;
                case 1:
                    MirMessageBox.Show("Your Character Name is not acceptable.");
                    _character.NameTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("The gender you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 3:
                    MirMessageBox.Show("The class you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 4:
                    MirMessageBox.Show("You cannot make anymore then " + Globals.MaxCharacterCount + " Characters.");
                    _character.Dispose();
                    break;
                case 5:
                    MirMessageBox.Show("A Character with this name already exists.");
                    _character.NameTextBox.SetFocus();
                    break;
            }
        }
        private void NewCharacter(S.NewCharacterSuccess p)
        {
            CancelCreateCharacter();

            MirMessageBox.Show("Your character was created successfully.");

            Characters.Insert(0, p.CharInfo);
            _selected = 0;
            UpdateInterface();
        }

        private void DeleteCharacter()
        {
            if (_selected < 0 || _selected >= Characters.Count) return;

            MirMessageBox message = new MirMessageBox(string.Format("Are you sure you want to Delete the character {0}?", Characters[_selected].Name), MirMessageBoxButtons.YesNo);
            int index = Characters[_selected].Index;

            message.YesButton.Click += (o1, e1) =>
            {
                MirInputBox inputBox = new MirInputBox("Please enter the characters name.");
                inputBox.OKButton.Click += (o, e) =>
                {
                    string name = Characters[_selected].Name.ToString();

                    if (inputBox.InputTextBox.Text == name)
                    {
                        DeleteCharacterButton.Enabled = false;
                        Network.Enqueue(new C.DeleteCharacter { CharacterIndex = index });
                    }
                    else
                    {
                        MirMessageBox failedMessage = new MirMessageBox(string.Format("Incorrect Entry."), MirMessageBoxButtons.OK);
                        failedMessage.Show();
                    }
                    inputBox.Dispose();
                };
                inputBox.Show();
            };
            message.Show();
        }

        private void DeleteCharacter(S.DeleteCharacter p)
        {
            DeleteCharacterButton.Enabled = true;
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Deleting characters is currently disabled.");
                    break;
                case 1:
                    MirMessageBox.Show("The character you selected does not exist.\n Contact a GM for assistance.");
                    break;
            }
        }
        private void DeleteCharacter(S.DeleteCharacterSuccess p)
        {
            DeleteCharacterButton.Enabled = true;
            MirMessageBox.Show("Your character was deleted successfully.");

            for (int i = 0; i < Characters.Count; i++)
                if (Characters[i].Index == p.CharacterIndex)
                {
                    Characters.RemoveAt(i);
                    break;
                }

            UpdateInterface();
        }

        private void StartGame(S.StartGameDelay p)
        {
            StartGameButton.Enabled = true;

            long time = CMain.Time + p.Milliseconds;

            MirMessageBox message = new MirMessageBox(string.Format("You cannot log onto this character for another {0} seconds.", Math.Ceiling(p.Milliseconds / 1000M)));

            message.BeforeDraw += (o, e) => message.Label.Text = string.Format("You cannot log onto this character for another {0} seconds.", Math.Ceiling((time - CMain.Time) / 1000M));


            message.AfterDraw += (o, e) =>
            {
                if (CMain.Time <= time) return;
                message.Dispose();
                StartGame();
            };

            message.Show();
        }
        public void StartGame(S.StartGameBanned p)
        {
            StartGameButton.Enabled = true;

            TimeSpan d = p.ExpiryDate - CMain.Now;
            MirMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds));
        }
        public void StartGame(S.StartGame p)
        {
            StartGameButton.Enabled = true;

            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Starting the game is currently disabled.");
                    break;
                case 1:
                    MirMessageBox.Show("You are not logged in.");
                    break;
                case 2:
                    MirMessageBox.Show("Your character could not be found.");
                    break;
                case 3:
                    MirMessageBox.Show("No active map and/or start point found.");
                    break;
                case 4:
                    if (p.Resolution < Settings.Resolution || Settings.Resolution == 0) Settings.Resolution = p.Resolution;

                    switch (Settings.Resolution)
                    {
                        default:
                        case 1024:
                            Settings.Resolution = 1024;
                            CMain.SetResolution(1024, 768);
                            break;
                        case 1280:
                            CMain.SetResolution(1280, 800);
                            break;
                        case 1366:
                            CMain.SetResolution(1366, 768);
                            break;
                        case 1920:
                            CMain.SetResolution(1920, 1080);
                            break;
                    }

                    ActiveScene = new GameScene();
                    Dispose();
                    break;
            }
        }
        private void UpdateInterface()
        {
            if (_selected >= 0 && _selected < Characters.Count)
            {
                SelectInfo character = Characters[_selected];
                StartGameButton.Enabled = true;
                NameLabel.Text = character.Name;
                NameLabel.Visible = true;
                DescriptionLabel.Text = $"Level {character.Level} {character.Class}";
                DescriptionLabel.Visible = true;

                CharacterDisplay.Index = 74 + (character.Body * 240) + (120 * (int)character.Gender);
                HeadDisplay.Index = 74 + (character.Hair * 240) + (120 * (int)character.Gender);
                WeaponDisplay.Index = 74 + (character.Weapon * 240) + (120 * (int)character.Gender);
            }
            else
            {
                StartGameButton.Enabled = false;
                NameLabel.Visible = false;
                DescriptionLabel.Visible = false;
            }

            CharacterLeft.Visible = CharacterRight.Visible = Characters.Count > 1;
        }


        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Background = null;
                _character = null;

                CharacterDisplayBackground = null;
                CharacterDisplay = null;
                HeadDisplay = null;
                WeaponDisplay = null;
                StartGameButton = null;
                NewCharacterButton = null;
                DeleteCharacterButton = null;
                CreateCharacterButton = null;
                CancelCreateButton = null;
                CharacterLeft = null;
                CharacterRight = null;
                ExitGame = null;
                Characters = null;
                NameLabel = null;
                DescriptionLabel = null;
                _selected = 0;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
