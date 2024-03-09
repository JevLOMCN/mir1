using Client.MirGraphics;

namespace Client.MirControls
{
    public enum MirMessageBoxButtons { OK, OKCancel, YesNo, YesNoCancel, Cancel }

    public sealed class MirMessageBox : MirImageControl
    {
        public MirLabel Label;
        public MirButton OKButton, CancelButton, NoButton, YesButton;
        public MirMessageBoxButtons Buttons;
        private MirGeneratedBox GeneratedBox;
        public bool AllowKeyPress = true;

        public MirMessageBox(string message, MirMessageBoxButtons b = MirMessageBoxButtons.OK, bool allowKeys = true)
        {
            DrawImage = false;
            ForeColour = Color.White;
            Buttons = b;
            Modal = true;
            Movable = false;
            AllowKeyPress = allowKeys;

            AutoSize = false;
            Size = new Size(440, 160);

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            GeneratedBox = new MirGeneratedBox(79, Size)
            {
                Parent = this,
                Visible = true
            };

            Label = new MirLabel
            {
                AutoSize = false,
                Location = new Point(10, 10),
                Size = new Size(390, 110),
                Parent = this,
                Text = message
            };

            
            switch (Buttons)
            {
                case MirMessageBoxButtons.OK:
                    OKButton = new MirButton
                    {
                        Index = 115,
                        Library = Libraries.Prguse,
                        Location = new Point(182, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 116,
                    };
                    OKButton.Click += (o, e) => Dispose();
                    break;
                case MirMessageBoxButtons.OKCancel:
                    OKButton = new MirButton
                    {
                        Index = 115,
                        Library = Libraries.Prguse,
                        Location = new Point(124, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 116,
                    };
                    OKButton.Click += (o, e) => Dispose();
                    CancelButton = new MirButton
                    {
                        Index = 117,
                        Library = Libraries.Prguse,
                        Location = new Point(240, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 118,
                    };
                    CancelButton.Click += (o, e) => Dispose();
                    break;
                case MirMessageBoxButtons.YesNo:
                    YesButton = new MirButton
                    {
                        Index = 50,
                        Library = Libraries.Prguse,
                        Location = new Point(124, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 51,
                    };
                    YesButton.Click += (o, e) => Dispose();
                    NoButton = new MirButton
                    {
                        Index = 52,
                        Library = Libraries.Prguse,
                        Location = new Point(240, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 53,
                    };
                    NoButton.Click += (o, e) => Dispose();
                    break;
                case MirMessageBoxButtons.YesNoCancel:
                    YesButton = new MirButton
                    {
                        Index = 50,
                        Library = Libraries.Prguse,
                        Location = new Point(124, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 51,
                    };
                    YesButton.Click += (o, e) => Dispose();
                    NoButton = new MirButton
                    {
                        Index = 52,
                        Library = Libraries.Prguse,
                        Location = new Point(240, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 53,
                    };
                    NoButton.Click += (o, e) => Dispose();
                    CancelButton = new MirButton
                    {
                        Index = 117,
                        Library = Libraries.Prguse,
                        Location = new Point(364, Size.Height - 35),
                        Parent = this,
                        PressedIndex = 118,
                    };
                    CancelButton.Click += (o, e) => Dispose();
                    break;
                case MirMessageBoxButtons.Cancel:
                    CancelButton = new MirButton
                    {
                        Index = 117,
                        Library = Libraries.Prguse,
                        Location = new Point(182, Size.Height - 35),
                        Parent = this,
                    };
                    CancelButton.Click += (o, e) => Dispose();
                    break;
            }
        }

        public override void Show()
        {
            if (Parent != null) return;

            Parent = MirScene.ActiveScene;

            Highlight();

            for (int i = 0; i < Program.Form.Controls.Count; i++)
            {
                TextBox T = Program.Form.Controls[i] as TextBox;
                if (T != null && T.Tag != null && T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }
        }


        public override void OnKeyDown(KeyEventArgs e)
        {
            if (AllowKeyPress)
            {
                base.OnKeyDown(e);
                e.Handled = true;
            }
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            if (AllowKeyPress)
            {
                base.OnKeyUp(e);
                e.Handled = true;
            }
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (AllowKeyPress)
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    switch (Buttons)
                    {
                        case MirMessageBoxButtons.OK:
                            if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.OKCancel:
                        case MirMessageBoxButtons.YesNoCancel:
                            if (CancelButton != null && !CancelButton.IsDisposed) CancelButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.YesNo:
                            if (NoButton != null && !NoButton.IsDisposed) NoButton.InvokeMouseClick(null);
                            break;
                    }
                }

                else if (e.KeyChar == (char)Keys.Enter)
                {
                    switch (Buttons)
                    {
                        case MirMessageBoxButtons.OK:
                        case MirMessageBoxButtons.OKCancel:
                            if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.YesNoCancel:
                        case MirMessageBoxButtons.YesNo:
                            if (YesButton != null && !YesButton.IsDisposed) YesButton.InvokeMouseClick(null);
                            break;

                    }
                }
                e.Handled = true;
            }
        }


        public static void Show(string message, bool close = false)
        {
            MirMessageBox box = new MirMessageBox(message, MirMessageBoxButtons.OK);

            if (close) box.OKButton.Click += (o, e) => Program.Form.Close();

            box.Show();
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);

            if (!disposing) return;

            Label = null;
            OKButton = null;
            CancelButton = null;
            NoButton = null;
            YesButton = null;
            Buttons = 0;

            for (int i = 0; i < Program.Form.Controls.Count; i++)
            {
                TextBox T = (TextBox) Program.Form.Controls[i];
                if (T != null && T.Tag != null)
                    ((MirTextBox) T.Tag).DialogChanged();
            }
        }

        #endregion
    }
}
