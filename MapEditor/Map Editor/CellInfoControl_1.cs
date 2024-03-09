using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_Editor
{
    public partial class CellInfoControl_1 : UserControl
    {
        public CellInfoControl_1()
        {
            InitializeComponent();
        }
        public void SetText(int x, int y)
        {
            LabX.Text = x.ToString();
            labY.Text = y.ToString();
        }

        internal void SetText(int cellX, int cellY, int v1, int v2, byte v3, byte frontAnimationTick, bool v4, byte middleAnimationFrame, byte middleAnimationTick, bool v5, byte doorOffset, byte v6, bool v7, byte light, bool fishingCell)
        {
            throw new NotImplementedException();
        }
    }
}
