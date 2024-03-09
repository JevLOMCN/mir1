namespace Server.Systems
{
    partial class GuildMembersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuildMembersForm));
            GuildMembersListView = new ListView();
            Members = new ColumnHeader();
            Rank = new ColumnHeader();
            DeleteMember = new Button();
            SuspendLayout();
            // 
            // GuildMembersListView
            // 
            GuildMembersListView.Columns.AddRange(new ColumnHeader[] { Members, Rank });
            GuildMembersListView.Dock = DockStyle.Top;
            GuildMembersListView.FullRowSelect = true;
            GuildMembersListView.GridLines = true;
            GuildMembersListView.Location = new Point(0, 0);
            GuildMembersListView.Name = "GuildMembersListView";
            GuildMembersListView.Size = new Size(375, 423);
            GuildMembersListView.TabIndex = 0;
            GuildMembersListView.UseCompatibleStateImageBehavior = false;
            GuildMembersListView.View = View.Details;
            // 
            // Members
            // 
            Members.Text = "Member Name";
            Members.Width = 180;
            // 
            // Rank
            // 
            Rank.Text = "Rank";
            Rank.Width = 190;
            // 
            // DeleteMember
            // 
            DeleteMember.Location = new Point(145, 429);
            DeleteMember.Name = "DeleteMember";
            DeleteMember.Size = new Size(75, 32);
            DeleteMember.TabIndex = 1;
            DeleteMember.Text = "Delete";
            DeleteMember.UseVisualStyleBackColor = true;
            DeleteMember.Click += DeleteMember_Click;
            // 
            // GuildMembersForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 467);
            Controls.Add(DeleteMember);
            Controls.Add(GuildMembersListView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "GuildMembersForm";
            Text = "GuildMembersForm";
            ResumeLayout(false);
        }

        #endregion
        private ColumnHeader Members;
        private ColumnHeader Rank;
        private Button DeleteMember;
        public ListView GuildMembersListView;
    }
}