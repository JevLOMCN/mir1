namespace Server.Account
{
    partial class CharacterInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterInfoForm));
            CharactersList = new ListView();
            IndexHeader = new ColumnHeader();
            NameHeader = new ColumnHeader();
            AccountNameHeader = new ColumnHeader();
            CharacterCountLabel = new Label();
            FindPlayerLabel = new Label();
            label1 = new Label();
            RefreshButton = new Button();
            MatchFilterCheckBox = new CheckBox();
            FilterPlayerTextBox = new TextBox();
            FilterItemTextBox = new TextBox();
            SuspendLayout();
            // 
            // CharactersList
            // 
            CharactersList.Columns.AddRange(new ColumnHeader[] { IndexHeader, NameHeader, AccountNameHeader });
            CharactersList.Dock = DockStyle.Bottom;
            CharactersList.FullRowSelect = true;
            CharactersList.GridLines = true;
            CharactersList.Location = new Point(0, 97);
            CharactersList.Name = "CharactersList";
            CharactersList.Size = new Size(382, 464);
            CharactersList.TabIndex = 0;
            CharactersList.UseCompatibleStateImageBehavior = false;
            CharactersList.View = View.Details;
            // 
            // IndexHeader
            // 
            IndexHeader.Text = "Index";
            IndexHeader.Width = 80;
            // 
            // NameHeader
            // 
            NameHeader.Text = "Player";
            NameHeader.Width = 145;
            // 
            // AccountNameHeader
            // 
            AccountNameHeader.Text = "Account";
            AccountNameHeader.Width = 145;
            // 
            // CharacterCountLabel
            // 
            CharacterCountLabel.AutoSize = true;
            CharacterCountLabel.Location = new Point(12, 9);
            CharacterCountLabel.Name = "CharacterCountLabel";
            CharacterCountLabel.Size = new Size(103, 15);
            CharacterCountLabel.TabIndex = 1;
            CharacterCountLabel.Text = "Characters count :";
            // 
            // FindPlayerLabel
            // 
            FindPlayerLabel.AutoSize = true;
            FindPlayerLabel.Location = new Point(180, 9);
            FindPlayerLabel.Name = "FindPlayerLabel";
            FindPlayerLabel.Size = new Size(74, 15);
            FindPlayerLabel.TabIndex = 2;
            FindPlayerLabel.Text = "Find Player : ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(122, 35);
            label1.Name = "label1";
            label1.Size = new Size(133, 15);
            label1.TabIndex = 3;
            label1.Text = "Find Item (Name/UID) : ";
            // 
            // RefreshButton
            // 
            RefreshButton.Location = new Point(180, 61);
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Size = new Size(75, 23);
            RefreshButton.TabIndex = 4;
            RefreshButton.Text = "Refresh";
            RefreshButton.UseVisualStyleBackColor = true;
            RefreshButton.Click += RefreshButton_Click;
            // 
            // MatchFilterCheckBox
            // 
            MatchFilterCheckBox.AutoSize = true;
            MatchFilterCheckBox.Location = new Point(273, 63);
            MatchFilterCheckBox.Name = "MatchFilterCheckBox";
            MatchFilterCheckBox.Size = new Size(89, 19);
            MatchFilterCheckBox.TabIndex = 5;
            MatchFilterCheckBox.Text = "Match Filter";
            MatchFilterCheckBox.UseVisualStyleBackColor = true;
            // 
            // FilterPlayerTextBox
            // 
            FilterPlayerTextBox.Location = new Point(257, 6);
            FilterPlayerTextBox.Name = "FilterPlayerTextBox";
            FilterPlayerTextBox.Size = new Size(122, 23);
            FilterPlayerTextBox.TabIndex = 6;
            // 
            // FilterItemTextBox
            // 
            FilterItemTextBox.Location = new Point(257, 32);
            FilterItemTextBox.Name = "FilterItemTextBox";
            FilterItemTextBox.Size = new Size(122, 23);
            FilterItemTextBox.TabIndex = 7;
            // 
            // CharacterInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(382, 561);
            Controls.Add(FilterItemTextBox);
            Controls.Add(FilterPlayerTextBox);
            Controls.Add(MatchFilterCheckBox);
            Controls.Add(RefreshButton);
            Controls.Add(label1);
            Controls.Add(FindPlayerLabel);
            Controls.Add(CharacterCountLabel);
            Controls.Add(CharactersList);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CharacterInfoForm";
            Text = "Characters";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView CharactersList;
        private ColumnHeader IndexHeader;
        private ColumnHeader NameHeader;
        private ColumnHeader AccountNameHeader;
        private Label CharacterCountLabel;
        private Label FindPlayerLabel;
        private Label label1;
        private Button RefreshButton;
        private CheckBox MatchFilterCheckBox;
        private TextBox FilterPlayerTextBox;
        private TextBox FilterItemTextBox;
    }
}