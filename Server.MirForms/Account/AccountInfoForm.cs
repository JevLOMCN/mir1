﻿using Server.MirDatabase;
using Server.MirEnvir;
using Server.MirObjects;
using System.Globalization;

namespace Server
{
    public partial class AccountInfoForm : Form
    {
        private List<AccountInfo> _selectedAccountInfos;

        public Envir AccountEnvir => SMain.Envir;

        public AccountInfoForm()
        {
            InitializeComponent();

            Setup();
        }

        public AccountInfoForm(string accountId, bool match = false)
        {
            InitializeComponent();

            FilterTextBox.Text = accountId;
            MatchFilterCheckBox.Checked = match;

            Setup();
        }

        private void Setup()
        {
            RefreshInterface();
            AutoResize();

            AccountIDTextBox.MaxLength = Globals.MaxAccountIDLength;
        }

        private void AutoResize()
        {
            indexHeader.Width = -2;
            accountIDHeader.Width = -2;
            userNameHeader.Width = -2;
            bannedHeader.Width = -2;
            banReasonHeader.Width = -2;
            expiryDateHeader.Width = -2;
            Gold.Width = -2;
            Credits.Width = -2;
        }

        #region Update Account List View
        private void Update(ListViewItem ListItem, AccountInfo account)
        {
            ListItem.SubItems[0].Text = account.Index.ToString();
            ListItem.SubItems[1].Text = account.AccountID;
            ListItem.SubItems[2].Text = string.Empty;
            ListItem.SubItems[3].Text = account.AdminAccount.ToString();
            ListItem.SubItems[4].Text = account.Banned.ToString();
            ListItem.SubItems[5].Text = account.BanReason;
            ListItem.SubItems[6].Text = account.ExpiryDate.ToString();
            ListItem.SubItems[7].Text = account.Gold.ToString();
            ListItem.SubItems[8].Text = account.Credit.ToString();
        }

        #endregion

        #region Account List View
        private ListViewItem CreateListView(AccountInfo account)
        {
            ListViewItem ListItem = new ListViewItem(account.Index.ToString()) { Tag = account };

            ListItem.SubItems.Add(account.AccountID);
            ListItem.SubItems.Add(string.Empty);
            ListItem.SubItems.Add(account.AdminAccount.ToString());
            ListItem.SubItems.Add(account.Banned.ToString());
            ListItem.SubItems.Add(account.BanReason);
            ListItem.SubItems.Add(account.ExpiryDate.ToString());
            ListItem.SubItems.Add(account.Gold.ToString());
            ListItem.SubItems.Add(account.Credit.ToString());

            return ListItem;
        }

        #endregion

        public void RefreshInterface()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(RefreshInterface));
                return;
            }

            List<AccountInfo> accounts = SMain.Envir.AccountList;

            #region Credit/Gold Label
            long totalGold = accounts
            .Where(account => !account.AdminAccount && !account.Banned)
            .Sum(account => account.Gold);

            ServerGoldTextBox.Text = totalGold.ToString("N0", CultureInfo.GetCultureInfo("en-GB"));


            long totalCredit = accounts
            .Where(account => !account.AdminAccount && !account.Banned)
            .Sum(account => account.Credit);

            ServerCreditTextBox.Text = totalCredit.ToString("N0", CultureInfo.GetCultureInfo("en-GB"));
            #endregion

            if (FilterTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccounts(FilterTextBox.Text, MatchFilterCheckBox.Checked);

            else if (FilterPlayerTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccountsByPlayer(FilterPlayerTextBox.Text, MatchFilterCheckBox.Checked);
            else if (FilterIPTextBox.Text.Length > 0)
                accounts = SMain.Envir.MatchAccountsByIP(FilterIPTextBox.Text, MatchFilterCheckBox.Checked);

            if (AccountInfoListView.Items.Count != accounts.Count)
            {
                AccountInfoListView.SelectedIndexChanged -= AccountInfoListView_SelectedIndexChanged;
                AccountInfoListView.Items.Clear();
                for (int i = AccountInfoListView.Items.Count; i < accounts.Count; i++)
                {
                    AccountInfo account = accounts[i];

                    ListViewItem tempItem = CreateListView(account);

                    AccountInfoListView.Items.Add(tempItem);
                }
                AccountInfoListView.SelectedIndexChanged += AccountInfoListView_SelectedIndexChanged;
            }

            _selectedAccountInfos = new List<AccountInfo>();
            CharactersListView.Items.Clear();


            for (int i = 0; i < AccountInfoListView.SelectedItems.Count; i++)
                _selectedAccountInfos.Add(AccountInfoListView.SelectedItems[i].Tag as AccountInfo);



            if (_selectedAccountInfos.Count == 0)
            {
                AccountInfoPanel.Enabled = false;

                AccountIDTextBox.Text = string.Empty;
                return;
            }


            AccountInfo info = _selectedAccountInfos[0];

            AccountInfoPanel.Enabled = true;

            AccountIDTextBox.Enabled = _selectedAccountInfos.Count == 1;
            AccountIDTextBox.Text = info.AccountID;

            CreationIPTextBox.Text = info.CreationIP;
            CreationDateTextBox.Text = info.CreationDate.ToString();

            LastIPTextBox.Text = info.LastIP;
            LastDateTextBox.Text = info.LastDate.ToString();

            BanReasonTextBox.Text = info.BanReason;
            BannedCheckBox.CheckState = info.Banned ? CheckState.Checked : CheckState.Unchecked;
            ExpiryDateTextBox.Text = info.ExpiryDate.ToString();
            AdminCheckBox.CheckState = info.AdminAccount ? CheckState.Checked : CheckState.Unchecked;
            PasswordChangeCheckBox.CheckState = info.RequirePasswordChange ? CheckState.Checked : CheckState.Unchecked;



            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                info = _selectedAccountInfos[i];

                if (AccountIDTextBox.Text != info.AccountID) AccountIDTextBox.Text = string.Empty;

                if (CreationIPTextBox.Text != info.CreationIP) CreationIPTextBox.Text = string.Empty;
                if (CreationDateTextBox.Text != info.CreationDate.ToString()) CreationDateTextBox.Text = string.Empty;


                if (LastIPTextBox.Text != info.LastIP) LastIPTextBox.Text = string.Empty;
                if (LastDateTextBox.Text != info.LastDate.ToString()) LastDateTextBox.Text = string.Empty;


                if (BanReasonTextBox.Text != info.BanReason) BanReasonTextBox.Text = string.Empty;
                if (BannedCheckBox.Checked != info.Banned) BannedCheckBox.CheckState = CheckState.Indeterminate;
                if (ExpiryDateTextBox.Text != info.ExpiryDate.ToString()) ExpiryDateTextBox.Text = string.Empty;
                if (AdminCheckBox.Checked != info.AdminAccount) AdminCheckBox.CheckState = CheckState.Indeterminate;
                if (PasswordChangeCheckBox.Checked != info.RequirePasswordChange) PasswordChangeCheckBox.CheckState = CheckState.Indeterminate;

                foreach (var character in info.Characters)
                {
                    var listItem = new ListViewItem(character.Name) { Tag = character };
                    listItem.SubItems.Add(character.Class.ToString());
                    listItem.SubItems.Add(character.Level.ToString());
                    listItem.SubItems.Add(character.PKPoints.ToString());

                    #region Guild Info

                    GuildObject guild = null;
                    if (character.GuildIndex != -1)
                    {
                        guild = AccountEnvir.GetGuild(character.GuildIndex);
                        if (guild != null)
                        {
                            listItem.SubItems.Add(guild.Name.ToString());
                        }
                    }
                    else
                    {
                        listItem.SubItems.Add("No Guild");
                    }

                    string status = $"";

                    if (character.LastLoginDate > character.LastLogoutDate)
                    {
                        status = $"Online: {(SMain.Envir.Now - character.LastLoginDate).TotalMinutes.ToString("##")} minutes";
                        listItem.ForeColor = Color.Green;
                    }
                    else
                    {
                        status = $"Offline: {character.LastLogoutDate}";
                    }

                    if (character.Deleted)
                    {
                        status = $"Deleted: {character.DeleteDate}";
                        listItem.ForeColor = Color.Red;
                    }

                    listItem.SubItems.Add(status.ToString());

                    CharactersListView.Items.Add(listItem);
                }
            }
        }

        #endregion

        private void AccountInfoListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInterface();
            Update(AccountInfoListView.SelectedItems[0], _selectedAccountInfos[0]);
        }

        #region Create Account Button
        private void CreateButton_Click(object sender, EventArgs e)
        {
            lock (Envir.AccountLock)
            {
                SMain.Envir.CreateAccountInfo();
                RefreshInterface();
            }
        }

        #endregion

        #region Account ID Change
        private void AccountIDTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;
            if (_selectedAccountInfos.Count != 1) return;

            lock (Envir.AccountLock)
            {
                if (SMain.Envir.AccountExists(ActiveControl.Text))
                {
                    ActiveControl.BackColor = Color.Red;
                    return;
                }
                AccountInfoListView.BeginUpdate();

                ActiveControl.BackColor = SystemColors.Window;
                _selectedAccountInfos[0].AccountID = ActiveControl.Text;
                Update(AccountInfoListView.SelectedItems[0], _selectedAccountInfos[0]);

                AutoResize();
                AccountInfoListView.EndUpdate();
            }
        }

        #endregion

        #region Bans
        private void DayBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to ban the selected Accounts?", "Ban Selected.", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            DateTime expiry = SMain.Envir.Now.AddDays(1);

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = expiry;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void WeekBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to ban the selected Accounts?", "Ban Selected.", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            DateTime expiry = SMain.Envir.Now.AddDays(7);

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = expiry;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void PermBanButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to ban the selected Accounts?", "Ban Selected.", MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;


            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = true;
                _selectedAccountInfos[i].ExpiryDate = DateTime.MaxValue;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            RefreshInterface();
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void BannedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].Banned = false;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void BanReasonTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].BanReason = ActiveControl.Text;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        private void ExpiryDateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            DateTime temp;

            if (!DateTime.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].ExpiryDate = temp;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }

            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        #endregion

        #region Refresh Accounts
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshInterface();
        }

        #endregion

        private void AccountInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SMain.Envir.Running) return;

            SMain.Envir.SaveAccounts();
        }

        #region Admin Checkbox
        private void AdminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].AdminAccount = AdminCheckBox.CheckState == CheckState.Checked ? true : false;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        #endregion

        #region Wipe Char Button
        private void WipeCharButton_Click(object sender, EventArgs e)
        {
            if (SMain.Envir.Running)
            {
                MessageBox.Show("Cannot wipe characters whilst the server is running", "Notice",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (MessageBox.Show("Are you sure you want to wipe all characters from the database?", "Notice",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                for (int i = 0; i < SMain.Envir.AccountList.Count; i++)
                {
                    AccountInfo account = SMain.Envir.AccountList[i];

                    account.Characters.Clear();
                }

                SMain.Envir.Auctions.Clear();
                SMain.Envir.GuildList.Clear();

                MessageBox.Show("All characters and associated data has been cleared", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }

        #endregion

        #region Password Change
        private void button1_Click(object sender, EventArgs e)
        {
            ChangePasswordDialog PasswordDialog = new ChangePasswordDialog();

            if (PasswordDialog.ShowDialog(this) == DialogResult.OK && PasswordDialog.PasswordTextBox.Text.Length > 0)
            {
                AccountInfoListView.BeginUpdate();
                for (int i = 0; i < _selectedAccountInfos.Count; i++)
                {
                    _selectedAccountInfos[i].Password = PasswordDialog.PasswordTextBox.Text;
                    _selectedAccountInfos[i].RequirePasswordChange = true;
                    PasswordChangeCheckBox.CheckState = CheckState.Checked;
                    Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
                    MessageBox.Show("Password Changed");
                }

                AutoResize();
                AccountInfoListView.EndUpdate();
            }
        }

        private void PasswordChangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            AccountInfoListView.BeginUpdate();
            for (int i = 0; i < _selectedAccountInfos.Count; i++)
            {
                _selectedAccountInfos[i].RequirePasswordChange = PasswordChangeCheckBox.CheckState == CheckState.Checked;
                Update(AccountInfoListView.SelectedItems[i], _selectedAccountInfos[i]);
            }
            AutoResize();
            AccountInfoListView.EndUpdate();
        }

        #endregion

        #region IPSearch
        private void CreationIPSearch_Click(object sender, EventArgs e)
        {
            string ipAddress = CreationIPTextBox.Text;

            string url = $"https://whatismyipaddress.com/ip/{ipAddress}";

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LastIPSearch_Click(object sender, EventArgs e)
        {
            string ipAddress = LastIPTextBox.Text;

            string url = $"https://whatismyipaddress.com/ip/{ipAddress}";

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Delete account btn
        private void Delaccbtn_Click(object sender, EventArgs e)
        {
            if (AccountInfoListView.SelectedItems.Count > 0)
            {
                // Ask for confirmation
                DialogResult result = MessageBox.Show("Are you sure you want to delete this account?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Get the selected account from the ListViewItem's Tag property
                    AccountInfo accInfo = (AccountInfo)AccountInfoListView.SelectedItems[0].Tag;

                    // Remove the selected account from AccountList
                    if (SMain.Envir.AccountList.Contains(accInfo))
                    {
                        SMain.Envir.AccountList.Remove(accInfo);
                    }

                    // Remove the selected item from AccountInfoListView
                    AccountInfoListView.SelectedItems[0].Remove();
                }
            }
            else
            {
                MessageBox.Show("Please select an account to remove.");
            }
        }
        #endregion
    }
}
