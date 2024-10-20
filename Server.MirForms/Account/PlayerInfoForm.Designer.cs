namespace Server
{
    partial class PlayerInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerInfoForm));
            PetInfoTab = new TabPage();
            PetView = new ListView();
            PetName = new ColumnHeader();
            Level = new ColumnHeader();
            HP = new ColumnHeader();
            AC = new ColumnHeader();
            AMC = new ColumnHeader();
            DC = new ColumnHeader();
            Location = new ColumnHeader();
            MagicInfoTab = new TabPage();
            MagicListViewNF = new CustomFormControl.ListViewNF();
            MagicNameHeader = new ColumnHeader();
            MagicLevelHeader = new ColumnHeader();
            MagicExperienceHeader = new ColumnHeader();
            Key = new ColumnHeader();
            ItemInfoTab = new TabPage();
            PlayerItemInfoListViewNF = new CustomFormControl.ListViewNF();
            UIDHeader = new ColumnHeader();
            LocationHeader = new ColumnHeader();
            NameHeader = new ColumnHeader();
            CountHeader = new ColumnHeader();
            DurabilityHeader = new ColumnHeader();
            PlayerItemsTreeView = new TreeView();
            QuestInfoTab = new TabPage();
            QuestInfoListViewNF = new CustomFormControl.ListViewNF();
            QuestIndexHeader = new ColumnHeader();
            QuestStatusHeader = new ColumnHeader();
            PlayerInfoTab = new TabPage();
            SearchBox = new GroupBox();
            FlagSearchBox = new NumericUpDown();
            FlagSearch = new Label();
            ResultLabel = new Label();
            groupBox1 = new GroupBox();
            ATKSPDBox = new TextBox();
            GameGold = new Label();
            AGILBox = new TextBox();
            GameGoldTextBox = new TextBox();
            ACCBox = new TextBox();
            Gold = new Label();
            SCBox = new TextBox();
            GoldTextBox = new TextBox();
            MCBox = new TextBox();
            PKPoints = new Label();
            DCBox = new TextBox();
            PKPointsTextBox = new TextBox();
            AMCBox = new TextBox();
            label12 = new Label();
            ACBox = new TextBox();
            ExpTextBox = new TextBox();
            StatsLabel = new Label();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            NameTextBox = new TextBox();
            IndexTextBox = new TextBox();
            UpdateButton = new Button();
            LevelTextBox = new TextBox();
            groupBox2 = new GroupBox();
            AccountBanButton = new Button();
            OpenAccountButton = new Button();
            SafeZoneButton = new Button();
            label9 = new Label();
            ChatBanExpiryTextBox = new TextBox();
            ChatBanButton = new Button();
            KillPetsButton = new Button();
            KillButton = new Button();
            KickButton = new Button();
            groupBox3 = new GroupBox();
            SendMessageTextBox = new TextBox();
            SendMessageButton = new Button();
            groupBox4 = new GroupBox();
            CurrentXY = new Label();
            label7 = new Label();
            CurrentIPLabel = new Label();
            CurrentMapLabel = new Label();
            label8 = new Label();
            label5 = new Label();
            OnlineTimeLabel = new Label();
            label6 = new Label();
            tabControl1 = new TabControl();
            NamelistInfoTab = new TabPage();
            Viewnamelistbutton = new Button();
            DeleteNamelistButton = new Button();
            NamelistView = new ListView();
            columnHeader1 = new ColumnHeader();
            PetInfoTab.SuspendLayout();
            MagicInfoTab.SuspendLayout();
            ItemInfoTab.SuspendLayout();
            QuestInfoTab.SuspendLayout();
            PlayerInfoTab.SuspendLayout();
            SearchBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FlagSearchBox).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            tabControl1.SuspendLayout();
            NamelistInfoTab.SuspendLayout();
            SuspendLayout();
            // 
            // PetInfoTab
            // 
            PetInfoTab.Controls.Add(PetView);
            PetInfoTab.Location = new Point(4, 24);
            PetInfoTab.Name = "PetInfoTab";
            PetInfoTab.Padding = new Padding(3);
            PetInfoTab.Size = new Size(701, 443);
            PetInfoTab.TabIndex = 4;
            PetInfoTab.Text = "Pet Info";
            PetInfoTab.UseVisualStyleBackColor = true;
            // 
            // PetView
            // 
            PetView.Columns.AddRange(new ColumnHeader[] { PetName, Level, HP, AC, AMC, DC, Location });
            PetView.Dock = DockStyle.Fill;
            PetView.GridLines = true;
            PetView.Location = new Point(3, 3);
            PetView.Name = "PetView";
            PetView.Size = new Size(695, 437);
            PetView.TabIndex = 0;
            PetView.UseCompatibleStateImageBehavior = false;
            PetView.View = View.Details;
            // 
            // PetName
            // 
            PetName.Text = "Name";
            PetName.Width = 150;
            // 
            // Level
            // 
            Level.Text = "Level";
            // 
            // HP
            // 
            HP.Text = "HP";
            // 
            // AC
            // 
            AC.Text = "AC";
            // 
            // AMC
            // 
            AMC.Text = "AMC";
            // 
            // DC
            // 
            DC.Text = "DC";
            // 
            // Location
            // 
            Location.Text = "Location";
            Location.Width = 240;
            // 
            // MagicInfoTab
            // 
            MagicInfoTab.Controls.Add(MagicListViewNF);
            MagicInfoTab.Location = new Point(4, 24);
            MagicInfoTab.Name = "MagicInfoTab";
            MagicInfoTab.Size = new Size(701, 443);
            MagicInfoTab.TabIndex = 3;
            MagicInfoTab.Text = "Magic Info";
            MagicInfoTab.UseVisualStyleBackColor = true;
            // 
            // MagicListViewNF
            // 
            MagicListViewNF.Columns.AddRange(new ColumnHeader[] { MagicNameHeader, MagicLevelHeader, MagicExperienceHeader, Key });
            MagicListViewNF.Dock = DockStyle.Fill;
            MagicListViewNF.GridLines = true;
            MagicListViewNF.Location = new Point(0, 0);
            MagicListViewNF.Name = "MagicListViewNF";
            MagicListViewNF.Size = new Size(701, 443);
            MagicListViewNF.TabIndex = 1;
            MagicListViewNF.UseCompatibleStateImageBehavior = false;
            MagicListViewNF.View = View.Details;
            // 
            // MagicNameHeader
            // 
            MagicNameHeader.Text = "Spell Name";
            MagicNameHeader.Width = 150;
            // 
            // MagicLevelHeader
            // 
            MagicLevelHeader.Text = "Level";
            MagicLevelHeader.Width = 50;
            // 
            // MagicExperienceHeader
            // 
            MagicExperienceHeader.Text = "Experience";
            MagicExperienceHeader.Width = 150;
            // 
            // Key
            // 
            Key.Text = "Key";
            Key.Width = 80;
            // 
            // ItemInfoTab
            // 
            ItemInfoTab.Controls.Add(PlayerItemInfoListViewNF);
            ItemInfoTab.Controls.Add(PlayerItemsTreeView);
            ItemInfoTab.Location = new Point(4, 24);
            ItemInfoTab.Name = "ItemInfoTab";
            ItemInfoTab.Size = new Size(701, 443);
            ItemInfoTab.TabIndex = 2;
            ItemInfoTab.Text = "Item Info";
            ItemInfoTab.UseVisualStyleBackColor = true;
            // 
            // PlayerItemInfoListViewNF
            // 
            PlayerItemInfoListViewNF.Columns.AddRange(new ColumnHeader[] { UIDHeader, LocationHeader, NameHeader, CountHeader, DurabilityHeader });
            PlayerItemInfoListViewNF.Dock = DockStyle.Fill;
            PlayerItemInfoListViewNF.GridLines = true;
            PlayerItemInfoListViewNF.Location = new Point(0, 0);
            PlayerItemInfoListViewNF.Name = "PlayerItemInfoListViewNF";
            PlayerItemInfoListViewNF.Size = new Size(701, 443);
            PlayerItemInfoListViewNF.TabIndex = 3;
            PlayerItemInfoListViewNF.UseCompatibleStateImageBehavior = false;
            PlayerItemInfoListViewNF.View = View.Details;
            // 
            // UIDHeader
            // 
            UIDHeader.Text = "UID";
            UIDHeader.Width = 100;
            // 
            // LocationHeader
            // 
            LocationHeader.Text = "Location";
            LocationHeader.Width = 150;
            // 
            // NameHeader
            // 
            NameHeader.Text = "Name";
            NameHeader.Width = 150;
            // 
            // CountHeader
            // 
            CountHeader.Text = "Count";
            CountHeader.Width = 80;
            // 
            // DurabilityHeader
            // 
            DurabilityHeader.Text = "Durability";
            DurabilityHeader.Width = 90;
            // 
            // PlayerItemsTreeView
            // 
            PlayerItemsTreeView.Dock = DockStyle.Fill;
            PlayerItemsTreeView.Location = new Point(0, 0);
            PlayerItemsTreeView.Name = "PlayerItemsTreeView";
            PlayerItemsTreeView.Size = new Size(701, 443);
            PlayerItemsTreeView.TabIndex = 1;
            // 
            // QuestInfoTab
            // 
            QuestInfoTab.Controls.Add(QuestInfoListViewNF);
            QuestInfoTab.Location = new Point(4, 24);
            QuestInfoTab.Name = "QuestInfoTab";
            QuestInfoTab.Padding = new Padding(3);
            QuestInfoTab.Size = new Size(701, 443);
            QuestInfoTab.TabIndex = 1;
            QuestInfoTab.Text = "Quest Info";
            QuestInfoTab.UseVisualStyleBackColor = true;
            // 
            // QuestInfoListViewNF
            // 
            QuestInfoListViewNF.Columns.AddRange(new ColumnHeader[] { QuestIndexHeader, QuestStatusHeader });
            QuestInfoListViewNF.Dock = DockStyle.Fill;
            QuestInfoListViewNF.GridLines = true;
            QuestInfoListViewNF.Location = new Point(3, 3);
            QuestInfoListViewNF.Name = "QuestInfoListViewNF";
            QuestInfoListViewNF.Size = new Size(695, 437);
            QuestInfoListViewNF.TabIndex = 0;
            QuestInfoListViewNF.UseCompatibleStateImageBehavior = false;
            QuestInfoListViewNF.View = View.Details;
            // 
            // QuestIndexHeader
            // 
            QuestIndexHeader.Text = "Index";
            QuestIndexHeader.Width = 100;
            // 
            // QuestStatusHeader
            // 
            QuestStatusHeader.Text = "Status";
            QuestStatusHeader.Width = 100;
            // 
            // PlayerInfoTab
            // 
            PlayerInfoTab.Controls.Add(SearchBox);
            PlayerInfoTab.Controls.Add(groupBox1);
            PlayerInfoTab.Controls.Add(groupBox2);
            PlayerInfoTab.Controls.Add(groupBox3);
            PlayerInfoTab.Controls.Add(groupBox4);
            PlayerInfoTab.Location = new Point(4, 24);
            PlayerInfoTab.Name = "PlayerInfoTab";
            PlayerInfoTab.Padding = new Padding(3);
            PlayerInfoTab.Size = new Size(701, 443);
            PlayerInfoTab.TabIndex = 0;
            PlayerInfoTab.Text = "Player Info";
            PlayerInfoTab.UseVisualStyleBackColor = true;
            // 
            // SearchBox
            // 
            SearchBox.Controls.Add(FlagSearchBox);
            SearchBox.Controls.Add(FlagSearch);
            SearchBox.Controls.Add(ResultLabel);
            SearchBox.Location = new Point(369, 178);
            SearchBox.Name = "SearchBox";
            SearchBox.Size = new Size(183, 82);
            SearchBox.TabIndex = 46;
            SearchBox.TabStop = false;
            SearchBox.Text = "Search";
            // 
            // FlagSearchBox
            // 
            FlagSearchBox.Location = new Point(33, 39);
            FlagSearchBox.Name = "FlagSearchBox";
            FlagSearchBox.Size = new Size(120, 23);
            FlagSearchBox.TabIndex = 45;
            FlagSearchBox.ValueChanged += FlagSearchBox_ValueChanged;
            // 
            // FlagSearch
            // 
            FlagSearch.AutoSize = true;
            FlagSearch.Location = new Point(54, 19);
            FlagSearch.Name = "FlagSearch";
            FlagSearch.Size = new Size(67, 15);
            FlagSearch.TabIndex = 25;
            FlagSearch.Text = "Flag Search";
            // 
            // ResultLabel
            // 
            ResultLabel.AutoSize = true;
            ResultLabel.Location = new Point(36, 63);
            ResultLabel.Name = "ResultLabel";
            ResultLabel.Size = new Size(0, 15);
            ResultLabel.TabIndex = 27;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ATKSPDBox);
            groupBox1.Controls.Add(GameGold);
            groupBox1.Controls.Add(AGILBox);
            groupBox1.Controls.Add(GameGoldTextBox);
            groupBox1.Controls.Add(ACCBox);
            groupBox1.Controls.Add(Gold);
            groupBox1.Controls.Add(SCBox);
            groupBox1.Controls.Add(GoldTextBox);
            groupBox1.Controls.Add(MCBox);
            groupBox1.Controls.Add(PKPoints);
            groupBox1.Controls.Add(DCBox);
            groupBox1.Controls.Add(PKPointsTextBox);
            groupBox1.Controls.Add(AMCBox);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(ACBox);
            groupBox1.Controls.Add(ExpTextBox);
            groupBox1.Controls.Add(StatsLabel);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(NameTextBox);
            groupBox1.Controls.Add(IndexTextBox);
            groupBox1.Controls.Add(UpdateButton);
            groupBox1.Controls.Add(LevelTextBox);
            groupBox1.Location = new Point(21, 6);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(340, 254);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "Character Info";
            // 
            // ATKSPDBox
            // 
            ATKSPDBox.Enabled = false;
            ATKSPDBox.Location = new Point(258, 229);
            ATKSPDBox.Margin = new Padding(4, 3, 4, 3);
            ATKSPDBox.Name = "ATKSPDBox";
            ATKSPDBox.ReadOnly = true;
            ATKSPDBox.Size = new Size(73, 23);
            ATKSPDBox.TabIndex = 23;
            // 
            // GameGold
            // 
            GameGold.AutoSize = true;
            GameGold.Font = new Font("Segoe UI", 8F);
            GameGold.Location = new Point(10, 202);
            GameGold.Margin = new Padding(4, 0, 4, 0);
            GameGold.Name = "GameGold";
            GameGold.Size = new Size(52, 13);
            GameGold.TabIndex = 14;
            GameGold.Text = "Credits : ";
            // 
            // AGILBox
            // 
            AGILBox.Enabled = false;
            AGILBox.Location = new Point(258, 200);
            AGILBox.Margin = new Padding(4, 3, 4, 3);
            AGILBox.Name = "AGILBox";
            AGILBox.ReadOnly = true;
            AGILBox.Size = new Size(73, 23);
            AGILBox.TabIndex = 22;
            // 
            // GameGoldTextBox
            // 
            GameGoldTextBox.Location = new Point(70, 198);
            GameGoldTextBox.Margin = new Padding(4, 3, 4, 3);
            GameGoldTextBox.Name = "GameGoldTextBox";
            GameGoldTextBox.Size = new Size(116, 23);
            GameGoldTextBox.TabIndex = 15;
            // 
            // ACCBox
            // 
            ACCBox.Enabled = false;
            ACCBox.Location = new Point(258, 168);
            ACCBox.Margin = new Padding(4, 3, 4, 3);
            ACCBox.Name = "ACCBox";
            ACCBox.ReadOnly = true;
            ACCBox.Size = new Size(73, 23);
            ACCBox.TabIndex = 21;
            // 
            // Gold
            // 
            Gold.AutoSize = true;
            Gold.Location = new Point(10, 172);
            Gold.Margin = new Padding(4, 0, 4, 0);
            Gold.Name = "Gold";
            Gold.Size = new Size(41, 15);
            Gold.TabIndex = 12;
            Gold.Text = "Gold : ";
            // 
            // SCBox
            // 
            SCBox.Enabled = false;
            SCBox.Location = new Point(258, 139);
            SCBox.Margin = new Padding(4, 3, 4, 3);
            SCBox.Name = "SCBox";
            SCBox.ReadOnly = true;
            SCBox.Size = new Size(73, 23);
            SCBox.TabIndex = 20;
            // 
            // GoldTextBox
            // 
            GoldTextBox.Location = new Point(70, 169);
            GoldTextBox.Margin = new Padding(4, 3, 4, 3);
            GoldTextBox.Name = "GoldTextBox";
            GoldTextBox.Size = new Size(116, 23);
            GoldTextBox.TabIndex = 13;
            // 
            // MCBox
            // 
            MCBox.Enabled = false;
            MCBox.Location = new Point(258, 110);
            MCBox.Margin = new Padding(4, 3, 4, 3);
            MCBox.Name = "MCBox";
            MCBox.ReadOnly = true;
            MCBox.Size = new Size(73, 23);
            MCBox.TabIndex = 19;
            // 
            // PKPoints
            // 
            PKPoints.AutoSize = true;
            PKPoints.Location = new Point(10, 143);
            PKPoints.Margin = new Padding(4, 0, 4, 0);
            PKPoints.Name = "PKPoints";
            PKPoints.Size = new Size(58, 15);
            PKPoints.TabIndex = 10;
            PKPoints.Text = "PKPoint : ";
            // 
            // DCBox
            // 
            DCBox.Enabled = false;
            DCBox.Location = new Point(258, 81);
            DCBox.Margin = new Padding(4, 3, 4, 3);
            DCBox.Name = "DCBox";
            DCBox.ReadOnly = true;
            DCBox.Size = new Size(73, 23);
            DCBox.TabIndex = 18;
            // 
            // PKPointsTextBox
            // 
            PKPointsTextBox.Location = new Point(70, 140);
            PKPointsTextBox.Margin = new Padding(4, 3, 4, 3);
            PKPointsTextBox.Name = "PKPointsTextBox";
            PKPointsTextBox.Size = new Size(116, 23);
            PKPointsTextBox.TabIndex = 11;
            // 
            // AMCBox
            // 
            AMCBox.Enabled = false;
            AMCBox.Location = new Point(258, 49);
            AMCBox.Margin = new Padding(4, 3, 4, 3);
            AMCBox.Name = "AMCBox";
            AMCBox.ReadOnly = true;
            AMCBox.Size = new Size(73, 23);
            AMCBox.TabIndex = 17;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(10, 114);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(36, 15);
            label12.TabIndex = 8;
            label12.Text = "EXP : ";
            // 
            // ACBox
            // 
            ACBox.Enabled = false;
            ACBox.Location = new Point(258, 20);
            ACBox.Margin = new Padding(4, 3, 4, 3);
            ACBox.Name = "ACBox";
            ACBox.ReadOnly = true;
            ACBox.Size = new Size(73, 23);
            ACBox.TabIndex = 16;
            // 
            // ExpTextBox
            // 
            ExpTextBox.Location = new Point(70, 111);
            ExpTextBox.Margin = new Padding(4, 3, 4, 3);
            ExpTextBox.Name = "ExpTextBox";
            ExpTextBox.ReadOnly = true;
            ExpTextBox.Size = new Size(116, 23);
            ExpTextBox.TabIndex = 9;
            // 
            // StatsLabel
            // 
            StatsLabel.AutoSize = true;
            StatsLabel.Location = new Point(194, 23);
            StatsLabel.Margin = new Padding(4, 0, 4, 0);
            StatsLabel.Name = "StatsLabel";
            StatsLabel.Size = new Size(57, 225);
            StatsLabel.TabIndex = 16;
            StatsLabel.Text = "AC :\r\n\r\nAMC :\r\n\r\nDC :\r\n\r\nMC :\r\n\r\nSC :\r\n\r\nACC :\r\n\r\nAGIL :\r\n\r\nATK SPD :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 23);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Index : ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 53);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 1;
            label1.Text = "Name : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 85);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 3;
            label3.Text = "Level : ";
            // 
            // NameTextBox
            // 
            NameTextBox.Location = new Point(70, 50);
            NameTextBox.Margin = new Padding(4, 3, 4, 3);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.Size = new Size(116, 23);
            NameTextBox.TabIndex = 4;
            // 
            // IndexTextBox
            // 
            IndexTextBox.Enabled = false;
            IndexTextBox.Location = new Point(70, 20);
            IndexTextBox.Margin = new Padding(4, 3, 4, 3);
            IndexTextBox.Name = "IndexTextBox";
            IndexTextBox.ReadOnly = true;
            IndexTextBox.Size = new Size(116, 23);
            IndexTextBox.TabIndex = 5;
            // 
            // UpdateButton
            // 
            UpdateButton.Location = new Point(83, 225);
            UpdateButton.Margin = new Padding(4, 3, 4, 3);
            UpdateButton.Name = "UpdateButton";
            UpdateButton.Size = new Size(88, 27);
            UpdateButton.TabIndex = 7;
            UpdateButton.Text = "Update";
            UpdateButton.UseVisualStyleBackColor = true;
            UpdateButton.Click += UpdateButton_Click;
            // 
            // LevelTextBox
            // 
            LevelTextBox.Location = new Point(70, 82);
            LevelTextBox.Margin = new Padding(4, 3, 4, 3);
            LevelTextBox.Name = "LevelTextBox";
            LevelTextBox.Size = new Size(116, 23);
            LevelTextBox.TabIndex = 6;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(AccountBanButton);
            groupBox2.Controls.Add(OpenAccountButton);
            groupBox2.Controls.Add(SafeZoneButton);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(ChatBanExpiryTextBox);
            groupBox2.Controls.Add(ChatBanButton);
            groupBox2.Controls.Add(KillPetsButton);
            groupBox2.Controls.Add(KillButton);
            groupBox2.Controls.Add(KickButton);
            groupBox2.Location = new Point(369, 11);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(324, 157);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "Actions";
            // 
            // AccountBanButton
            // 
            AccountBanButton.Location = new Point(8, 88);
            AccountBanButton.Margin = new Padding(4, 3, 4, 3);
            AccountBanButton.Name = "AccountBanButton";
            AccountBanButton.Size = new Size(88, 27);
            AccountBanButton.TabIndex = 24;
            AccountBanButton.Text = "Account Ban";
            AccountBanButton.UseVisualStyleBackColor = true;
            AccountBanButton.Click += AccountBanButton_Click;
            // 
            // OpenAccountButton
            // 
            OpenAccountButton.Location = new Point(196, 22);
            OpenAccountButton.Margin = new Padding(4, 3, 4, 3);
            OpenAccountButton.Name = "OpenAccountButton";
            OpenAccountButton.Size = new Size(115, 27);
            OpenAccountButton.TabIndex = 23;
            OpenAccountButton.Text = "Open Account";
            OpenAccountButton.UseVisualStyleBackColor = true;
            OpenAccountButton.Click += OpenAccountButton_Click;
            // 
            // SafeZoneButton
            // 
            SafeZoneButton.Location = new Point(102, 22);
            SafeZoneButton.Margin = new Padding(4, 3, 4, 3);
            SafeZoneButton.Name = "SafeZoneButton";
            SafeZoneButton.Size = new Size(88, 27);
            SafeZoneButton.TabIndex = 22;
            SafeZoneButton.Text = "Safezone";
            SafeZoneButton.UseVisualStyleBackColor = true;
            SafeZoneButton.Click += SafeZoneButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(108, 113);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(53, 15);
            label9.TabIndex = 21;
            label9.Text = "Expires : ";
            // 
            // ChatBanExpiryTextBox
            // 
            ChatBanExpiryTextBox.Location = new Point(174, 109);
            ChatBanExpiryTextBox.Margin = new Padding(4, 3, 4, 3);
            ChatBanExpiryTextBox.Name = "ChatBanExpiryTextBox";
            ChatBanExpiryTextBox.Size = new Size(137, 23);
            ChatBanExpiryTextBox.TabIndex = 20;
            ChatBanExpiryTextBox.TextChanged += ChatBanExpiryTextBox_TextChanged;
            // 
            // ChatBanButton
            // 
            ChatBanButton.Location = new Point(7, 121);
            ChatBanButton.Margin = new Padding(4, 3, 4, 3);
            ChatBanButton.Name = "ChatBanButton";
            ChatBanButton.Size = new Size(88, 27);
            ChatBanButton.TabIndex = 19;
            ChatBanButton.Text = "Chat Ban";
            ChatBanButton.UseVisualStyleBackColor = true;
            ChatBanButton.Click += ChatBanButton_Click;
            // 
            // KillPetsButton
            // 
            KillPetsButton.Location = new Point(102, 55);
            KillPetsButton.Margin = new Padding(4, 3, 4, 3);
            KillPetsButton.Name = "KillPetsButton";
            KillPetsButton.Size = new Size(88, 27);
            KillPetsButton.TabIndex = 18;
            KillPetsButton.Text = "Kill Pets";
            KillPetsButton.UseVisualStyleBackColor = true;
            KillPetsButton.Click += KillPetsButton_Click;
            // 
            // KillButton
            // 
            KillButton.Location = new Point(7, 55);
            KillButton.Margin = new Padding(4, 3, 4, 3);
            KillButton.Name = "KillButton";
            KillButton.Size = new Size(88, 27);
            KillButton.TabIndex = 17;
            KillButton.Text = "Kill Player";
            KillButton.UseVisualStyleBackColor = true;
            KillButton.Click += KillButton_Click;
            // 
            // KickButton
            // 
            KickButton.Location = new Point(7, 22);
            KickButton.Margin = new Padding(4, 3, 4, 3);
            KickButton.Name = "KickButton";
            KickButton.Size = new Size(88, 27);
            KickButton.TabIndex = 8;
            KickButton.Text = "Kick Player";
            KickButton.UseVisualStyleBackColor = true;
            KickButton.Click += KickButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(SendMessageTextBox);
            groupBox3.Controls.Add(SendMessageButton);
            groupBox3.Location = new Point(21, 386);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(436, 57);
            groupBox3.TabIndex = 13;
            groupBox3.TabStop = false;
            groupBox3.Text = "Send Message";
            // 
            // SendMessageTextBox
            // 
            SendMessageTextBox.Location = new Point(7, 20);
            SendMessageTextBox.Margin = new Padding(4, 3, 4, 3);
            SendMessageTextBox.Name = "SendMessageTextBox";
            SendMessageTextBox.Size = new Size(347, 23);
            SendMessageTextBox.TabIndex = 9;
            // 
            // SendMessageButton
            // 
            SendMessageButton.Location = new Point(362, 20);
            SendMessageButton.Margin = new Padding(4, 3, 4, 3);
            SendMessageButton.Name = "SendMessageButton";
            SendMessageButton.Size = new Size(68, 27);
            SendMessageButton.TabIndex = 10;
            SendMessageButton.Text = "Send";
            SendMessageButton.UseVisualStyleBackColor = true;
            SendMessageButton.Click += SendMessageButton_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(CurrentXY);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(CurrentIPLabel);
            groupBox4.Controls.Add(CurrentMapLabel);
            groupBox4.Controls.Add(label8);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(OnlineTimeLabel);
            groupBox4.Controls.Add(label6);
            groupBox4.Location = new Point(21, 272);
            groupBox4.Margin = new Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(335, 108);
            groupBox4.TabIndex = 25;
            groupBox4.TabStop = false;
            groupBox4.Text = "Details";
            // 
            // CurrentXY
            // 
            CurrentXY.AutoSize = true;
            CurrentXY.Location = new Point(133, 39);
            CurrentXY.Margin = new Padding(4, 0, 4, 0);
            CurrentXY.Name = "CurrentXY";
            CurrentXY.Size = new Size(30, 15);
            CurrentXY.TabIndex = 25;
            CurrentXY.Text = "$x/y";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(10, 39);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(105, 15);
            label7.TabIndex = 26;
            label7.Text = "Current Location : ";
            // 
            // CurrentIPLabel
            // 
            CurrentIPLabel.AutoSize = true;
            CurrentIPLabel.Location = new Point(132, 79);
            CurrentIPLabel.Margin = new Padding(4, 0, 4, 0);
            CurrentIPLabel.Name = "CurrentIPLabel";
            CurrentIPLabel.Size = new Size(23, 15);
            CurrentIPLabel.TabIndex = 24;
            CurrentIPLabel.Text = "$IP";
            CurrentIPLabel.Click += CurrentIPLabel_Click;
            // 
            // CurrentMapLabel
            // 
            CurrentMapLabel.AutoSize = true;
            CurrentMapLabel.Location = new Point(132, 19);
            CurrentMapLabel.Margin = new Padding(4, 0, 4, 0);
            CurrentMapLabel.Name = "CurrentMapLabel";
            CurrentMapLabel.Size = new Size(37, 15);
            CurrentMapLabel.TabIndex = 15;
            CurrentMapLabel.Text = "$map";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(10, 79);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(69, 15);
            label8.TabIndex = 23;
            label8.Text = "Current IP : ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 19);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(83, 15);
            label5.TabIndex = 16;
            label5.Text = "Current Map : ";
            // 
            // OnlineTimeLabel
            // 
            OnlineTimeLabel.AutoSize = true;
            OnlineTimeLabel.Location = new Point(132, 59);
            OnlineTimeLabel.Margin = new Padding(4, 0, 4, 0);
            OnlineTimeLabel.Name = "OnlineTimeLabel";
            OnlineTimeLabel.Size = new Size(70, 15);
            OnlineTimeLabel.TabIndex = 20;
            OnlineTimeLabel.Text = "$onlinetime";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 59);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(80, 15);
            label6.TabIndex = 19;
            label6.Text = "Online Time : ";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(PlayerInfoTab);
            tabControl1.Controls.Add(QuestInfoTab);
            tabControl1.Controls.Add(ItemInfoTab);
            tabControl1.Controls.Add(MagicInfoTab);
            tabControl1.Controls.Add(PetInfoTab);
            tabControl1.Controls.Add(NamelistInfoTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(709, 471);
            tabControl1.TabIndex = 45;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // NamelistInfoTab
            // 
            NamelistInfoTab.Controls.Add(Viewnamelistbutton);
            NamelistInfoTab.Controls.Add(DeleteNamelistButton);
            NamelistInfoTab.Controls.Add(NamelistView);
            NamelistInfoTab.Location = new Point(4, 24);
            NamelistInfoTab.Name = "NamelistInfoTab";
            NamelistInfoTab.Size = new Size(701, 443);
            NamelistInfoTab.TabIndex = 5;
            NamelistInfoTab.Text = "Namelists";
            NamelistInfoTab.UseVisualStyleBackColor = true;
            // 
            // Viewnamelistbutton
            // 
            Viewnamelistbutton.Location = new Point(207, 404);
            Viewnamelistbutton.Name = "Viewnamelistbutton";
            Viewnamelistbutton.Size = new Size(75, 23);
            Viewnamelistbutton.TabIndex = 2;
            Viewnamelistbutton.Text = "View";
            Viewnamelistbutton.UseVisualStyleBackColor = true;
            Viewnamelistbutton.Click += Viewnamelistbutton_Click;
            // 
            // DeleteNamelistButton
            // 
            DeleteNamelistButton.Location = new Point(93, 404);
            DeleteNamelistButton.Name = "DeleteNamelistButton";
            DeleteNamelistButton.Size = new Size(75, 23);
            DeleteNamelistButton.TabIndex = 1;
            DeleteNamelistButton.Text = "Delete";
            DeleteNamelistButton.UseVisualStyleBackColor = true;
            DeleteNamelistButton.Click += DeleteNamelistButton_Click;
            // 
            // NamelistView
            // 
            NamelistView.BorderStyle = BorderStyle.None;
            NamelistView.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            NamelistView.FullRowSelect = true;
            NamelistView.GridLines = true;
            NamelistView.HeaderStyle = ColumnHeaderStyle.None;
            NamelistView.Location = new Point(3, 3);
            NamelistView.MultiSelect = false;
            NamelistView.Name = "NamelistView";
            NamelistView.Size = new Size(380, 395);
            NamelistView.TabIndex = 0;
            NamelistView.UseCompatibleStateImageBehavior = false;
            NamelistView.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Width = 380;
            // 
            // PlayerInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 471);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "PlayerInfoForm";
            Text = "Player Info Form";
            PetInfoTab.ResumeLayout(false);
            MagicInfoTab.ResumeLayout(false);
            ItemInfoTab.ResumeLayout(false);
            QuestInfoTab.ResumeLayout(false);
            PlayerInfoTab.ResumeLayout(false);
            SearchBox.ResumeLayout(false);
            SearchBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)FlagSearchBox).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            tabControl1.ResumeLayout(false);
            NamelistInfoTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label SCLabel;
        private Label MCLabel;
        private Label DCLabel;
        private Label AMCLabel;
        private GroupBox Pets;
        private TabPage PetInfoTab;
        private ListView PetView;
        private ColumnHeader PetName;
        private ColumnHeader Level;
        private ColumnHeader HP;
        private ColumnHeader AC;
        private ColumnHeader AMC;
        private ColumnHeader DC;
        private ColumnHeader Location;
        private TabPage MagicInfoTab;
        private CustomFormControl.ListViewNF MagicListViewNF;
        private ColumnHeader MagicNameHeader;
        private ColumnHeader MagicLevelHeader;
        private ColumnHeader MagicExperienceHeader;
        private ColumnHeader Key;
        private TabPage ItemInfoTab;
        private CustomFormControl.ListViewNF PlayerItemInfoListViewNF;
        private ColumnHeader UIDHeader;
        private ColumnHeader LocationHeader;
        private ColumnHeader NameHeader;
        private ColumnHeader CountHeader;
        private ColumnHeader DurabilityHeader;
        private TreeView PlayerItemsTreeView;
        private TabPage QuestInfoTab;
        private CustomFormControl.ListViewNF QuestInfoListViewNF;
        private ColumnHeader QuestIndexHeader;
        private ColumnHeader QuestStatusHeader;
        private TabPage PlayerInfoTab;
        private GroupBox SearchBox;
        private NumericUpDown FlagSearchBox;
        private Label FlagSearch;
        private Label ResultLabel;
        private GroupBox groupBox1;
        private TextBox ATKSPDBox;
        private Label GameGold;
        private TextBox AGILBox;
        private TextBox GameGoldTextBox;
        private TextBox ACCBox;
        private Label Gold;
        private TextBox SCBox;
        private TextBox GoldTextBox;
        private TextBox MCBox;
        private Label PKPoints;
        private TextBox DCBox;
        private TextBox PKPointsTextBox;
        private TextBox AMCBox;
        private Label label12;
        private TextBox ACBox;
        private TextBox ExpTextBox;
        private Label StatsLabel;
        private Label label2;
        private Label label1;
        private Label label3;
        private TextBox NameTextBox;
        private TextBox IndexTextBox;
        private Button UpdateButton;
        private TextBox LevelTextBox;
        private GroupBox groupBox2;
        private Button AccountBanButton;
        private Button OpenAccountButton;
        private Button SafeZoneButton;
        private Label label9;
        private TextBox ChatBanExpiryTextBox;
        private Button ChatBanButton;
        private Button KillPetsButton;
        private Button KillButton;
        private Button KickButton;
        private GroupBox groupBox3;
        private TextBox SendMessageTextBox;
        private Button SendMessageButton;
        private GroupBox groupBox4;
        private Label CurrentXY;
        private Label label7;
        private Label CurrentIPLabel;
        private Label CurrentMapLabel;
        private Label label8;
        private Label label5;
        private Label OnlineTimeLabel;
        private Label label6;
        private TabControl tabControl1;
        private TabPage NamelistInfoTab;
        private ListView NamelistView;
        private ColumnHeader columnHeader1;
        private Button Viewnamelistbutton;
        private Button DeleteNamelistButton;
    }
}