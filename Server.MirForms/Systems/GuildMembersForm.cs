using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Systems
{
    public partial class GuildMembersForm : Form
    {
        public string GuildName;
        public SMain main;
        public GuildMembersForm()
        {
            InitializeComponent();
        }



        private void DeleteMember_Click(object sender, EventArgs e)
        {
            if (GuildMembersListView == null) return;
            if (GuildMembersListView.SelectedItems == null) return;

            Server.MirObjects.GuildObject Guild = SMain.Envir.GetGuild(GuildName);
            if (Guild == null) return;

            foreach (var m in GuildMembersListView.SelectedItems)
            {
                var lm = (ListViewItem)m;

                Guild.DeleteMember(lm.SubItems[0].Text);
                GuildMembersListView.Items.Remove(lm);
                main.ProcessGuildViewTab(true);
                break;
            }
        }
    }
}

