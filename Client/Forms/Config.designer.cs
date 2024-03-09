namespace Launcher
{
    partial class Config
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            AccountLogin_txt = new TextBox();
            AccountPass_txt = new TextBox();
            label2 = new Label();
            label3 = new Label();
            Res3_pb = new PictureBox();
            Res2_pb = new PictureBox();
            ID_l = new Label();
            Password_l = new Label();
            label4 = new Label();
            AutoStart_pb = new PictureBox();
            AutoStart_label = new Label();
            Fullscreen_label = new Label();
            Fullscreen_pb = new PictureBox();
            OnTop_label = new Label();
            OnTop_pb = new PictureBox();
            FPScap_label = new Label();
            FPScap_pb = new PictureBox();
            CleanFiles_pb = new PictureBox();
            pictureBox6 = new PictureBox();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label5 = new Label();
            Res4_pb = new PictureBox();
            label1 = new Label();
            Res5_pb = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)Res3_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Res2_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AutoStart_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Fullscreen_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OnTop_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FPScap_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CleanFiles_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Res4_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Res5_pb).BeginInit();
            SuspendLayout();
            // 
            // AccountLogin_txt
            // 
            AccountLogin_txt.BackColor = Color.FromArgb(64, 64, 64);
            AccountLogin_txt.BorderStyle = BorderStyle.None;
            AccountLogin_txt.Cursor = Cursors.IBeam;
            AccountLogin_txt.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point);
            AccountLogin_txt.ForeColor = Color.White;
            AccountLogin_txt.Location = new Point(29, 205);
            AccountLogin_txt.Margin = new Padding(4, 3, 4, 3);
            AccountLogin_txt.Name = "AccountLogin_txt";
            AccountLogin_txt.Size = new Size(124, 15);
            AccountLogin_txt.TabIndex = 0;
            AccountLogin_txt.Tag = "Testing";
            AccountLogin_txt.Click += AccountLogin_txt_Click;
            AccountLogin_txt.TextChanged += AccountLogin_txt_TextChanged;
            AccountLogin_txt.Leave += AccountLogin_txt_TextChanged;
            // 
            // AccountPass_txt
            // 
            AccountPass_txt.BackColor = Color.FromArgb(64, 64, 64);
            AccountPass_txt.BorderStyle = BorderStyle.None;
            AccountPass_txt.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point);
            AccountPass_txt.ForeColor = Color.White;
            AccountPass_txt.Location = new Point(29, 232);
            AccountPass_txt.Margin = new Padding(4, 3, 4, 3);
            AccountPass_txt.Name = "AccountPass_txt";
            AccountPass_txt.PasswordChar = '*';
            AccountPass_txt.Size = new Size(124, 15);
            AccountPass_txt.TabIndex = 1;
            AccountPass_txt.Click += AccountPass_txt_Click;
            AccountPass_txt.TextChanged += AccountPass_txt_TextChanged;
            AccountPass_txt.Leave += AccountPass_txt_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(178, 59);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(48, 13);
            label2.TabIndex = 5;
            label2.Text = "800x600";
            label2.Click += Res2_pb_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.Gray;
            label3.Location = new Point(178, 100);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(54, 13);
            label3.TabIndex = 7;
            label3.Text = "1366x768";
            label3.Click += Res3_pb_Click;
            // 
            // Res3_pb
            // 
            Res3_pb.Location = new Point(157, 100);
            Res3_pb.Margin = new Padding(4, 3, 4, 3);
            Res3_pb.Name = "Res3_pb";
            Res3_pb.Size = new Size(12, 12);
            Res3_pb.TabIndex = 6;
            Res3_pb.TabStop = false;
            Res3_pb.Click += Res3_pb_Click;
            // 
            // Res2_pb
            // 
            Res2_pb.Location = new Point(157, 59);
            Res2_pb.Margin = new Padding(4, 3, 4, 3);
            Res2_pb.Name = "Res2_pb";
            Res2_pb.Size = new Size(12, 12);
            Res2_pb.TabIndex = 4;
            Res2_pb.TabStop = false;
            Res2_pb.Click += Res2_pb_Click;
            // 
            // ID_l
            // 
            ID_l.BackColor = Color.Transparent;
            ID_l.Cursor = Cursors.IBeam;
            ID_l.Enabled = false;
            ID_l.Font = new Font("Calibri", 9.75F, FontStyle.Italic, GraphicsUnit.Point);
            ID_l.ForeColor = Color.Transparent;
            ID_l.Location = new Point(29, 204);
            ID_l.Margin = new Padding(4, 0, 4, 0);
            ID_l.Name = "ID_l";
            ID_l.Size = new Size(124, 17);
            ID_l.TabIndex = 8;
            ID_l.Text = "Username";
            ID_l.Click += AccountLogin_txt_Click;
            // 
            // Password_l
            // 
            Password_l.BackColor = Color.Transparent;
            Password_l.Cursor = Cursors.IBeam;
            Password_l.Enabled = false;
            Password_l.Font = new Font("Calibri Light", 9.75F, FontStyle.Italic, GraphicsUnit.Point);
            Password_l.ForeColor = Color.Transparent;
            Password_l.Location = new Point(29, 232);
            Password_l.Margin = new Padding(4, 0, 4, 0);
            Password_l.Name = "Password_l";
            Password_l.Size = new Size(124, 17);
            Password_l.TabIndex = 9;
            Password_l.Text = "Password";
            Password_l.Click += AccountPass_txt_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(261, 31);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(0, 15);
            label4.TabIndex = 10;
            // 
            // AutoStart_pb
            // 
            AutoStart_pb.BackgroundImageLayout = ImageLayout.Center;
            AutoStart_pb.Location = new Point(23, 160);
            AutoStart_pb.Margin = new Padding(4, 3, 4, 3);
            AutoStart_pb.Name = "AutoStart_pb";
            AutoStart_pb.Size = new Size(12, 12);
            AutoStart_pb.TabIndex = 11;
            AutoStart_pb.TabStop = false;
            AutoStart_pb.Click += AutoStart_pb_Click;
            // 
            // AutoStart_label
            // 
            AutoStart_label.AutoSize = true;
            AutoStart_label.BackColor = Color.Transparent;
            AutoStart_label.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            AutoStart_label.ForeColor = Color.Gray;
            AutoStart_label.Location = new Point(40, 159);
            AutoStart_label.Margin = new Padding(4, 0, 4, 0);
            AutoStart_label.Name = "AutoStart_label";
            AutoStart_label.Size = new Size(54, 13);
            AutoStart_label.TabIndex = 12;
            AutoStart_label.Text = "Auto start";
            AutoStart_label.Click += AutoStart_pb_Click;
            // 
            // Fullscreen_label
            // 
            Fullscreen_label.AutoSize = true;
            Fullscreen_label.BackColor = Color.Transparent;
            Fullscreen_label.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            Fullscreen_label.ForeColor = Color.Gray;
            Fullscreen_label.Location = new Point(40, 59);
            Fullscreen_label.Margin = new Padding(4, 0, 4, 0);
            Fullscreen_label.Name = "Fullscreen_label";
            Fullscreen_label.Size = new Size(56, 13);
            Fullscreen_label.TabIndex = 14;
            Fullscreen_label.Text = "Fullscreen";
            Fullscreen_label.Click += Fullscreen_pb_Click;
            // 
            // Fullscreen_pb
            // 
            Fullscreen_pb.BackgroundImageLayout = ImageLayout.Center;
            Fullscreen_pb.Location = new Point(23, 59);
            Fullscreen_pb.Margin = new Padding(4, 3, 4, 3);
            Fullscreen_pb.Name = "Fullscreen_pb";
            Fullscreen_pb.Size = new Size(12, 12);
            Fullscreen_pb.TabIndex = 13;
            Fullscreen_pb.TabStop = false;
            Fullscreen_pb.Click += Fullscreen_pb_Click;
            // 
            // OnTop_label
            // 
            OnTop_label.AutoSize = true;
            OnTop_label.BackColor = Color.Transparent;
            OnTop_label.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            OnTop_label.ForeColor = Color.Gray;
            OnTop_label.Location = new Point(40, 113);
            OnTop_label.Margin = new Padding(4, 0, 4, 0);
            OnTop_label.Name = "OnTop_label";
            OnTop_label.Size = new Size(72, 13);
            OnTop_label.TabIndex = 16;
            OnTop_label.Text = "Always on top";
            OnTop_label.Click += OnTop_pb_Click;
            // 
            // OnTop_pb
            // 
            OnTop_pb.BackgroundImageLayout = ImageLayout.Center;
            OnTop_pb.Image = Client.Resources.Images.Config_Check_Off1;
            OnTop_pb.Location = new Point(23, 113);
            OnTop_pb.Margin = new Padding(4, 3, 4, 3);
            OnTop_pb.Name = "OnTop_pb";
            OnTop_pb.Size = new Size(12, 12);
            OnTop_pb.TabIndex = 15;
            OnTop_pb.TabStop = false;
            OnTop_pb.Click += OnTop_pb_Click;
            // 
            // FPScap_label
            // 
            FPScap_label.AutoSize = true;
            FPScap_label.BackColor = Color.Transparent;
            FPScap_label.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            FPScap_label.ForeColor = Color.Gray;
            FPScap_label.Location = new Point(40, 84);
            FPScap_label.Margin = new Padding(4, 0, 4, 0);
            FPScap_label.Name = "FPScap_label";
            FPScap_label.Size = new Size(42, 13);
            FPScap_label.TabIndex = 18;
            FPScap_label.Text = "FPS cap";
            FPScap_label.Click += FPScap_pb_Click;
            // 
            // FPScap_pb
            // 
            FPScap_pb.BackgroundImageLayout = ImageLayout.Center;
            FPScap_pb.Image = Client.Resources.Images.Config_Check_Off1;
            FPScap_pb.Location = new Point(23, 85);
            FPScap_pb.Margin = new Padding(4, 3, 4, 3);
            FPScap_pb.Name = "FPScap_pb";
            FPScap_pb.Size = new Size(12, 12);
            FPScap_pb.TabIndex = 17;
            FPScap_pb.TabStop = false;
            FPScap_pb.Click += FPScap_pb_Click;
            // 
            // CleanFiles_pb
            // 
            CleanFiles_pb.BackgroundImageLayout = ImageLayout.Center;
            CleanFiles_pb.Image = Client.Resources.Images.CheckF_Base2;
            CleanFiles_pb.Location = new Point(23, 275);
            CleanFiles_pb.Margin = new Padding(4, 3, 4, 3);
            CleanFiles_pb.Name = "CleanFiles_pb";
            CleanFiles_pb.Size = new Size(67, 23);
            CleanFiles_pb.TabIndex = 19;
            CleanFiles_pb.TabStop = false;
            CleanFiles_pb.Click += CleanFiles_pb_Click;
            CleanFiles_pb.MouseDown += CleanFiles_pb_MouseDown;
            CleanFiles_pb.MouseEnter += CleanFiles_pb_MouseEnter;
            CleanFiles_pb.MouseLeave += CleanFiles_pb_MouseLeave;
            CleanFiles_pb.MouseUp += CleanFiles_pb_MouseUp;
            // 
            // pictureBox6
            // 
            pictureBox6.BackColor = Color.Transparent;
            pictureBox6.Image = Client.Resources.Images.textboxes;
            pictureBox6.Location = new Point(23, 198);
            pictureBox6.Margin = new Padding(4, 3, 4, 3);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(137, 57);
            pictureBox6.TabIndex = 20;
            pictureBox6.TabStop = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ForeColor = Color.White;
            label9.Location = new Point(23, 42);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(55, 14);
            label9.TabIndex = 21;
            label9.Text = "Graphics";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ForeColor = Color.White;
            label10.Location = new Point(157, 42);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(66, 14);
            label10.TabIndex = 22;
            label10.Text = "Resolution";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.Transparent;
            label11.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label11.ForeColor = Color.White;
            label11.Location = new Point(20, 145);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(39, 14);
            label11.TabIndex = 23;
            label11.Text = "Game";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.BackColor = Color.Transparent;
            label12.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label12.ForeColor = Color.Gray;
            label12.Location = new Point(20, 182);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(80, 13);
            label12.TabIndex = 24;
            label12.Text = "Account details";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.Gray;
            label5.Location = new Point(178, 80);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(54, 13);
            label5.TabIndex = 26;
            label5.Text = "1280x800";
            // 
            // Res4_pb
            // 
            Res4_pb.Image = Client.Resources.Images.Radio_Unactive;
            Res4_pb.Location = new Point(157, 80);
            Res4_pb.Margin = new Padding(4, 3, 4, 3);
            Res4_pb.Name = "Res4_pb";
            Res4_pb.Size = new Size(12, 12);
            Res4_pb.TabIndex = 25;
            Res4_pb.TabStop = false;
            Res4_pb.Click += Res4_pb_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(178, 121);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(60, 13);
            label1.TabIndex = 27;
            label1.Text = "1920x1080";
            // 
            // Res5_pb
            // 
            Res5_pb.Image = Client.Resources.Images.Radio_Unactive;
            Res5_pb.Location = new Point(157, 121);
            Res5_pb.Margin = new Padding(4, 3, 4, 3);
            Res5_pb.Name = "Res5_pb";
            Res5_pb.Size = new Size(12, 12);
            Res5_pb.TabIndex = 28;
            Res5_pb.TabStop = false;
            Res5_pb.Click += Res5_pb_Click;
            // 
            // Config
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImage = Client.Resources.Images.Config_Base1;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(257, 315);
            Controls.Add(Res5_pb);
            Controls.Add(label1);
            Controls.Add(label5);
            Controls.Add(Res4_pb);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(CleanFiles_pb);
            Controls.Add(FPScap_label);
            Controls.Add(FPScap_pb);
            Controls.Add(OnTop_label);
            Controls.Add(OnTop_pb);
            Controls.Add(Fullscreen_label);
            Controls.Add(Fullscreen_pb);
            Controls.Add(AutoStart_label);
            Controls.Add(AutoStart_pb);
            Controls.Add(label4);
            Controls.Add(Password_l);
            Controls.Add(ID_l);
            Controls.Add(label3);
            Controls.Add(Res3_pb);
            Controls.Add(label2);
            Controls.Add(Res2_pb);
            Controls.Add(AccountPass_txt);
            Controls.Add(AccountLogin_txt);
            Controls.Add(pictureBox6);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Config";
            Opacity = 0.94D;
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Config";
            TransparencyKey = Color.Black;
            Load += Config_Load;
            VisibleChanged += Config_VisibleChanged;
            Click += Config_Click;
            ((System.ComponentModel.ISupportInitialize)Res3_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)Res2_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)AutoStart_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)Fullscreen_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)OnTop_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)FPScap_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)CleanFiles_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)Res4_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)Res5_pb).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox AccountLogin_txt;
        private TextBox AccountPass_txt;
        private Label label2;
        private PictureBox Res2_pb;
        private Label label3;
        private PictureBox Res3_pb;
        private Label ID_l;
        private Label Password_l;
        private Label label4;
        private PictureBox AutoStart_pb;
        private Label AutoStart_label;
        private Label Fullscreen_label;
        private PictureBox Fullscreen_pb;
        private Label OnTop_label;
        private PictureBox OnTop_pb;
        private Label FPScap_label;
        private PictureBox FPScap_pb;
        private PictureBox CleanFiles_pb;
        private PictureBox pictureBox6;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label5;
        private PictureBox Res4_pb;
        private Label label1;
        private PictureBox Res5_pb;
    }
}