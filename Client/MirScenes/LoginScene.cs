using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using S = ServerPackets;
using C = ClientPackets;

namespace Client.MirScenes
{
    public sealed class LoginScene : MirScene
    {
        private MirImageControl _background;
        public MirLabel Version;

        private LoginDialog _login;
        private NewAccountDialog _account;
        private ChangePasswordDialog _password;

        private MirMessageBox _connectBox;

        public LoginScene()
        {
            SoundManager.PlayMusic(SoundList.IntroMusic, true);
            Disposing += (o, e) => SoundManager.StopMusic();

            _background = new MirImageControl
            {
                Index = 0,
                Library = Libraries.Prguse,
                Parent = this,
            };

            _login = new LoginDialog { Parent = _background, Visible = false };
            _login.AccountButton.Click += (o, e) =>
                {
                    _login.Hide();
                    _account = new NewAccountDialog { Parent = _background };
                    _account.Disposing += (o1, e1) => _login.Show();
                };

            _login.PassButton.Click += (o, e) =>
                {
                    OpenPasswordChangeDialog(string.Empty, string.Empty);
                };

            Version = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.FromArgb(200, 50, 50, 50),
                Border = true,
                BorderColour = Color.Black,
                Location = new Point(5, Settings.ScreenHeight - 20),
                Parent = _background,
                Text = string.Format("Build: {0}.{1}.{2}", Globals.ProductCodename, Settings.UseTestConfig ? "Debug" : "Release", Application.ProductVersion),
            };

            _connectBox = new MirMessageBox("Attempting to connect to the server.", MirMessageBoxButtons.Cancel);
            _connectBox.CancelButton.Click += (o, e) => Program.Form.Close();
            Shown += (sender, args) =>
                {
                    Network.Connect();
                    _connectBox.Show();
                };
        }

        public override void Process()
        {
            if (!Network.Connected && _connectBox.Label != null)
                _connectBox.Label.Text = string.Format(GameLanguage.AttemptingConnect,"\n\n", Network.ConnectAttempt);
        }
        public override void ProcessPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.Connected:
                    Network.Connected = true;
                    SendVersion();
                    break;
                case (short)ServerPacketIds.ClientVersion:
                    ClientVersion((S.ClientVersion) p);
                    break;
                case (short)ServerPacketIds.NewAccount:
                    NewAccount((S.NewAccount) p);
                    break;
                case (short)ServerPacketIds.ChangePassword:
                    ChangePassword((S.ChangePassword) p);
                    break;
                case (short)ServerPacketIds.ChangePasswordBanned:
                    ChangePassword((S.ChangePasswordBanned) p);
                    break;
                case (short)ServerPacketIds.Login:
                    Login((S.Login) p);
                    break;
                case (short)ServerPacketIds.LoginBanned:
                    Login((S.LoginBanned) p);
                    break;
                case (short)ServerPacketIds.LoginSuccess:
                    Login((S.LoginSuccess)p);
                    break;
                default:
                    base.ProcessPacket(p);
                    break;
            }
        }

        private  void SendVersion()
        {
            _connectBox.Label.Text = "Sending Client Version.";

            C.ClientVersion p = new C.ClientVersion();
            try
            {
                byte[] sum;
                using (MD5 md5 = MD5.Create())
                using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                    sum = md5.ComputeHash(stream);

                p.VersionHash = sum;
                    Network.Enqueue(p);
            }
            catch (Exception ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }
        }
        private void ClientVersion(S.ClientVersion p)
        {
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Wrong version, please update your game.\nGame will now Close", true);

                    Network.Disconnect();
                    break;
                case 1:
                    _connectBox.Dispose();
                    _login.Show();
                    break;
            }
        }

        private void OpenPasswordChangeDialog(string autoFillID, string autoFillPassword)
        {
            _login.Hide();
            _password = new ChangePasswordDialog { Parent = _background };
            _password.AccountIDTextBox.Text = autoFillID;
            _password.CurrentPasswordTextBox.Text = autoFillPassword;
            _password.Disposing += (o1, e1) => _login.Show();
        }
        private void NewAccount(S.NewAccount p)
        {
            _account.OKButton.Enabled = true;
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Account creation is currently disabled.");
                    _account.Dispose();
                    break;
                case 1:
                    MirMessageBox.Show("Your AccountID is not acceptable.");
                    _account.AccountIDTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("Your Password is not acceptable.");
                    _account.Password1TextBox.SetFocus();
                    break;
                case 7:
                    MirMessageBox.Show("An Account with this ID already exists.");
                    _account.AccountIDTextBox.Text = string.Empty;
                    _account.AccountIDTextBox.SetFocus();
                    break;
                case 8:
                    MirMessageBox.Show("Your account was created successfully.");
                    _account.Dispose();
                    break;
            }
        }
        private void ChangePassword(S.ChangePassword p)
        {
            _password.OKButton.Enabled = true;

            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Password Changing is currently disabled.");
                    _password.Dispose();
                    break;
                case 1:
                    MirMessageBox.Show("Your AccountID is not acceptable.");
                    _password.AccountIDTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("The current Password is not acceptable.");
                    _password.CurrentPasswordTextBox.SetFocus();
                    break;
                case 3:
                    MirMessageBox.Show("Your new Password is not acceptable.");
                    _password.NewPassword1TextBox.SetFocus();
                    break;
                case 4:
                    MirMessageBox.Show(GameLanguage.NoAccountID);
                    _password.AccountIDTextBox.SetFocus();
                    break;
                case 5:
                    MirMessageBox.Show(GameLanguage.IncorrectPasswordAccountID);
                    _password.CurrentPasswordTextBox.SetFocus();
                    _password.CurrentPasswordTextBox.Text = string.Empty;
                    break;
                case 6:
                    MirMessageBox.Show("Your password was changed successfully.");
                    _password.Dispose();
                    break;
            }
        }
        private void ChangePassword(S.ChangePasswordBanned p)
        {
            _password.Dispose();

            TimeSpan d = p.ExpiryDate - CMain.Now;
            MirMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds ));
        }
        private void Login(S.Login p)
        {
            _login.OKButton.Enabled = true;
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Logging in is currently disabled.");
                    _login.Clear();
                    break;
                case 1:
                    MirMessageBox.Show("Your AccountID is not acceptable.");
                    _login.AccountIDTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("Your Password is not acceptable.");
                    _login.PasswordTextBox.SetFocus();
                    break;
                case 3:
                    MirMessageBox.Show(GameLanguage.NoAccountID);
                    _login.PasswordTextBox.SetFocus();
                    break;
                case 4:
                    MirMessageBox.Show(GameLanguage.IncorrectPasswordAccountID);
                    _login.PasswordTextBox.Text = string.Empty;
                    _login.PasswordTextBox.SetFocus();
                    break;
                case 5:
                    MirMessageBox.Show("The account's password must be changed before logging in.");                    
                    OpenPasswordChangeDialog(_login.AccountIDTextBox.Text, _login.PasswordTextBox.Text);
                    _login.PasswordTextBox.Text = string.Empty;
                    break;
            }
        }
        private void Login(S.LoginBanned p)
        {
            _login.OKButton.Enabled = true;

            TimeSpan d = p.ExpiryDate - CMain.Now;
            MirMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds));
        }

        private void Login(S.LoginSuccess p)
        {
            Enabled = false;
            _login.Dispose();

            Dispose();
            ActiveScene = new SelectScene(p.Characters);
        }

        public sealed class LoginDialog : MirImageControl
        {
            public MirLabel AccountIDLabel, PassLabel;
            public MirButton AccountButton, CloseButton, OKButton, PassButton;
            public MirTextBox AccountIDTextBox, PasswordTextBox;
            private bool _accountIDValid, _passwordValid;

            public LoginDialog()
            {
                Index = 185;
                Library = Libraries.Prguse;
                DrawImage = true;
                Location = new Point(Settings.ScreenWidth / 2 - Size.Width / 2, Settings.ScreenHeight / 2 - Size.Height / 2);
                PixelDetect = false;
                AutoSize = false;
                Size = new Size(548, 600);

                AccountIDLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(168, 241),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Name:"
                };

                PassLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(173, 263),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Pass:"
                };

                PasswordTextBox = new MirTextBox
                {
                    Location = new Point(225, 265),
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 15),
                    MaxLength = Globals.MaxPasswordLength
                };

                PasswordTextBox.TextBox.TextChanged += PasswordTextBox_TextChanged;
                PasswordTextBox.TextBox.KeyPress += TextBox_KeyPress;

                AccountIDTextBox = new MirTextBox
                {
                    Location = new Point(225, 242),
                    Parent = this,
                    Size = new Size(136, 15),
                    MaxLength = Globals.MaxAccountIDLength
                };

                AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;
                AccountIDTextBox.TextBox.KeyPress += TextBox_KeyPress;

                OKButton = new MirButton
                {
                    Enabled = false,
                    HoverIndex = 11,
                    Index = 10,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 372),
                    Parent = this,
                    PressedIndex = 9
                };
                OKButton.Click += (o, e) => Login();

                AccountButton = new MirButton
                {
                    HoverIndex = 17,
                    Index = 16,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 400),
                    Parent = this,
                    PressedIndex = 15,
                };

                PassButton = new MirButton
                {
                    HoverIndex = 23,
                    Index = 22,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 428),
                    Parent = this,
                    PressedIndex = 21,
                };

                CloseButton = new MirButton
                {
                    HoverIndex = 26,
                    Index = 25,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 456),
                    Parent = this,
                    PressedIndex = 24,
                };
                CloseButton.Click += (o, e) => Program.Form.Close();

                AccountIDTextBox.Text = Settings.AccountID;
                PasswordTextBox.Text = Settings.Password;
            }

            private void AccountIDTextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg =
                    new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");

                if (string.IsNullOrEmpty(AccountIDTextBox.Text) || !reg.IsMatch(AccountIDTextBox.TextBox.Text))
                {
                    _accountIDValid = false;
                    AccountIDTextBox.Border = !string.IsNullOrEmpty(AccountIDTextBox.Text);
                    AccountIDTextBox.BorderColour = Color.Red;
                }
                else
                {
                    _accountIDValid = true;
                    AccountIDTextBox.Border = true;
                    AccountIDTextBox.BorderColour = Color.Green;
                }
            }
            private void PasswordTextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg =
                    new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

                if (string.IsNullOrEmpty(PasswordTextBox.TextBox.Text) || !reg.IsMatch(PasswordTextBox.TextBox.Text))
                {
                    _passwordValid = false;
                    PasswordTextBox.Border = !string.IsNullOrEmpty(PasswordTextBox.TextBox.Text);
                    PasswordTextBox.BorderColour = Color.Red;
                }
                else
                {
                    _passwordValid = true;
                    PasswordTextBox.Border = true;
                    PasswordTextBox.BorderColour = Color.Green;
                }

                RefreshLoginButton();
            }
            public void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (sender == null || e.KeyChar != (char) Keys.Enter) return;

                e.Handled = true;

                if (!_accountIDValid)
                {
                    AccountIDTextBox.SetFocus();
                    return;
                }
                if (!_passwordValid)
                {
                    PasswordTextBox.SetFocus();
                    return;
                }

                if (OKButton.Enabled)
                    OKButton.InvokeMouseClick(null);
            }
            private void RefreshLoginButton()
            {
                OKButton.Enabled = _accountIDValid && _passwordValid;
            }

            private void Login()
            {
                OKButton.Enabled = false;
                Network.Enqueue(new C.Login { AccountID = AccountIDTextBox.Text, Password = PasswordTextBox.Text });
            }

            public override void Show()
            {
                if (Visible) return;
                Visible = true;
                AccountIDTextBox.SetFocus();

                if (Settings.Password != string.Empty && Settings.AccountID != string.Empty)
                {
                    Login();
                }
            }
            public void Clear()
            {
                AccountIDTextBox.Text = string.Empty;
                PasswordTextBox.Text = string.Empty;
            }

            #region Disposable

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    AccountIDLabel = null;
                    PassLabel = null;
                    AccountButton = null;
                    CloseButton = null;
                    OKButton = null;
                    PassButton = null;
                    AccountIDTextBox = null;
                    PasswordTextBox = null;

                }

                base.Dispose(disposing);
            }

            #endregion
        }

        public sealed class NewAccountDialog : MirImageControl
        {
            public MirButton OKButton, CancelButton, GenderLeftButton, GenderRightButton;
            public MirLabel AccountIDLabel, PassLabel, ConfirmLabel, GenderLabel, MyGenderLabel;
            public MirGender Gender;

            public MirTextBox AccountIDTextBox,
                              Password1TextBox,
                              Password2TextBox;

            private bool _accountIDValid,
                         _password1Valid,
                         _password2Valid;


            public NewAccountDialog()
            {
                Index = 185;
                Library = Libraries.Prguse;
                DrawImage = true;
                Location = new Point(Settings.ScreenWidth / 2 - Size.Width / 2, Settings.ScreenHeight / 2 - Size.Height / 2);
                PixelDetect = false;
                AutoSize = false;
                Size = new Size(548, 600);

                CancelButton = new MirButton
                {
                    HoverIndex = 26,
                    Index = 25,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 456),
                    Parent = this,
                    PressedIndex = 24,
                };
                CancelButton.Click += (o, e) => Dispose();

                OKButton = new MirButton
                {
                    Enabled = false,
                    HoverIndex = 11,
                    Index = 10,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 372),
                    Parent = this,
                    PressedIndex = 9
                };
                OKButton.Click += (o, e) => CreateAccount();

                AccountIDLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(168, 241),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Name:"
                };

                PassLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(173, 263),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Pass:"
                };

                ConfirmLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(153, 285),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Confirm:"
                };

                Password1TextBox = new MirTextBox
                {
                    Location = new Point(225, 265),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 18),
                    TextBox = { MaxLength = Globals.MaxPasswordLength },
                };
                Password1TextBox.TextBox.TextChanged += Password1TextBox_TextChanged;

                Password2TextBox = new MirTextBox
                {
                    Location = new Point(225, 288),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 18),
                    TextBox = { MaxLength = Globals.MaxPasswordLength },
                };
                Password2TextBox.TextBox.TextChanged += Password2TextBox_TextChanged;

                AccountIDTextBox = new MirTextBox
                {
                    Location = new Point(225, 242),
                    MaxLength = Globals.MaxAccountIDLength,
                    Parent = this,
                    Size = new Size(136, 18),
                };

                AccountIDTextBox.TextBox.MaxLength = Globals.MaxAccountIDLength;
                AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;
            }

            private void AccountIDTextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");

                if (string.IsNullOrEmpty(AccountIDTextBox.Text) || !reg.IsMatch(AccountIDTextBox.Text))
                {
                    _accountIDValid = false;
                    AccountIDTextBox.BorderColour = Color.Red;
                }
                else
                {
                    _accountIDValid = true;
                    AccountIDTextBox.BorderColour = Color.Green;
                }
                RefreshConfirmButton();
            }
            private void Password1TextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

                if (string.IsNullOrEmpty(Password1TextBox.Text) || !reg.IsMatch(Password1TextBox.Text))
                {
                    _password1Valid = false;
                    Password1TextBox.BorderColour = Color.Red;
                }
                else
                {
                    _password1Valid = true;
                    Password1TextBox.BorderColour = Color.Green;
                }
                Password2TextBox_TextChanged(sender, e);
            }
            private void Password2TextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

                if (string.IsNullOrEmpty(Password2TextBox.Text) || !reg.IsMatch(Password2TextBox.Text) ||
                    Password1TextBox.Text != Password2TextBox.Text)
                {
                    _password2Valid = false;
                    Password2TextBox.BorderColour = Color.Red;
                }
                else
                {
                    _password2Valid = true;
                    Password2TextBox.BorderColour = Color.Green;
                }
                RefreshConfirmButton();
            }

            private void RefreshConfirmButton()
            {
                OKButton.Enabled = _accountIDValid && _password1Valid && _password2Valid;
            }
            private void CreateAccount()
            {
                OKButton.Enabled = false;

                Network.Enqueue(new C.NewAccount
                    {
                        AccountID = AccountIDTextBox.Text,
                        Password = Password1TextBox.Text
                    });
            }
            
            public override void Show()
            {
                if (Visible) return;
                Visible = true;
                AccountIDTextBox.SetFocus();
            }

            #region Disposable
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    OKButton = null;
                    CancelButton = null;

                    AccountIDTextBox = null;
                    Password1TextBox = null;
                    Password2TextBox = null;
                }

                base.Dispose(disposing);
            }
            #endregion
        }

        public sealed class ChangePasswordDialog : MirImageControl
        {
            public readonly MirButton OKButton,
                                      CancelButton;

            public readonly MirTextBox AccountIDTextBox,
                                       CurrentPasswordTextBox,
                                       NewPassword1TextBox,
                                       NewPassword2TextBox;

            public MirLabel AccountIDLabel, CurrentPassLabel, NewPassLabel, ConfirmLabel;

            private bool _accountIDValid,
                         _currentPasswordValid,
                         _newPassword1Valid,
                         _newPassword2Valid;
            
            public ChangePasswordDialog()
            {
                Index = 185;
                Library = Libraries.Prguse;
                DrawImage = true;
                Location = new Point(Settings.ScreenWidth / 2 - Size.Width / 2, Settings.ScreenHeight / 2 - Size.Height / 2);
                PixelDetect = false;
                AutoSize = false;
                Size = new Size(548, 600);

                CancelButton = new MirButton
                {
                    HoverIndex = 26,
                    Index = 25,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 456),
                    Parent = this,
                    PressedIndex = 24,
                };
                CancelButton.Click += (o, e) => Dispose();

                OKButton = new MirButton
                {
                    Enabled = false,
                    HoverIndex = 11,
                    Index = 10,
                    Library = Libraries.Prguse,
                    Location = new Point(362, 372),
                    Parent = this,
                    PressedIndex = 9
                };
                OKButton.Click += (o, e) => ChangePassword();

                AccountIDLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(168, 240),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Name:"
                };

                CurrentPassLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(112, 263),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Current Pass:"
                };

                NewPassLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(136, 285),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "New Pass:"
                };

                ConfirmLabel = new MirLabel
                {
                    Border = true,
                    Location = new Point(151, 308),
                    Parent = this,
                    AutoSize = true,
                    Font = new Font(Settings.FontName, 12F, FontStyle.Bold),
                    Text = "Confirm:"
                };

                AccountIDTextBox = new MirTextBox
                {
                    Location = new Point(225, 242),
                    MaxLength = Globals.MaxAccountIDLength,
                    Parent = this,
                    Size = new Size(136, 18),
                };
                AccountIDTextBox.SetFocus();
                AccountIDTextBox.TextBox.MaxLength = Globals.MaxAccountIDLength;
                AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;

                CurrentPasswordTextBox = new MirTextBox
                {
                    Location = new Point(225, 265),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 18),
                    TextBox = { MaxLength = Globals.MaxPasswordLength },
                };
                CurrentPasswordTextBox.TextBox.TextChanged += CurrentPasswordTextBox_TextChanged;

                NewPassword1TextBox = new MirTextBox
                {
                    Location = new Point(225, 288),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 18),
                    TextBox = { MaxLength = Globals.MaxPasswordLength },
                };
                NewPassword1TextBox.TextBox.TextChanged += NewPassword1TextBox_TextChanged;

                NewPassword2TextBox = new MirTextBox
                {
                    Location = new Point(225, 311),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 18),
                    TextBox = { MaxLength = Globals.MaxPasswordLength },
                };
                NewPassword2TextBox.TextBox.TextChanged += NewPassword2TextBox_TextChanged;

            }

            void RefreshConfirmButton()
            {
                OKButton.Enabled = _accountIDValid && _currentPasswordValid && _newPassword1Valid && _newPassword2Valid;
            }

            private void AccountIDTextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");

                if (string.IsNullOrEmpty(AccountIDTextBox.Text) || !reg.IsMatch(AccountIDTextBox.Text))
                {
                    _accountIDValid = false;
                    AccountIDTextBox.BorderColour = Color.Red;
                }
                else
                {
                    _accountIDValid = true;
                    AccountIDTextBox.BorderColour = Color.Green;
                }
                RefreshConfirmButton();
            }
            private void CurrentPasswordTextBox_TextChanged(object sender, EventArgs e)
            {
              Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

                if (string.IsNullOrEmpty(CurrentPasswordTextBox.Text) || !reg.IsMatch(CurrentPasswordTextBox.Text))
                {
                    _currentPasswordValid = false;
                    CurrentPasswordTextBox.BorderColour = Color.Red;
                }
                else
                {
                    _currentPasswordValid = true;
                    CurrentPasswordTextBox.BorderColour = Color.Green;
                }
                RefreshConfirmButton();
            }
            private void NewPassword1TextBox_TextChanged(object sender, EventArgs e)
            {
                Regex reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

                if (string.IsNullOrEmpty(NewPassword1TextBox.Text) || !reg.IsMatch(NewPassword1TextBox.Text))
                {
                    _newPassword1Valid = false;
                    NewPassword1TextBox.BorderColour = Color.Red;
                }
                else
                {
                    _newPassword1Valid = true;
                    NewPassword1TextBox.BorderColour = Color.Green;
                }
                NewPassword2TextBox_TextChanged(sender, e);
            }
            private void NewPassword2TextBox_TextChanged(object sender, EventArgs e)
            {
                if (NewPassword1TextBox.Text == NewPassword2TextBox.Text)
                {
                    _newPassword2Valid = _newPassword1Valid;
                    NewPassword2TextBox.BorderColour = NewPassword1TextBox.BorderColour;
                }
                else
                {
                    _newPassword2Valid = false;
                    NewPassword2TextBox.BorderColour = Color.Red;
                }
                RefreshConfirmButton();
            }

            private void ChangePassword()
            {
                OKButton.Enabled = false;

                Network.Enqueue(new C.ChangePassword
                    {
                        AccountID = AccountIDTextBox.Text,
                        CurrentPassword = CurrentPasswordTextBox.Text,
                        NewPassword = NewPassword1TextBox.Text
                    });
            }

            public override void Show()
            {
                if (Visible) return;
                Visible = true;
                AccountIDTextBox.SetFocus();
            }
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _background = null;
                Version = null;

                _login = null;
                _account = null;
                _password = null;

                _connectBox = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
