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
    public partial class CellInfoControl_2 : UserControl
    {
        public CellInfoControl_2()
        {
            InitializeComponent();
        }

        public void SetText(int x, int y, int backLimit, int frontLimit,
            byte fFrame, byte ftick, bool fblend, byte mFrame, byte mTick, bool mBlend, byte doorOffSet, byte doorIndex, bool entityDoor, byte light, bool fishing)
        {
            LabX.Text = x.ToString();
            labY.Text = y.ToString();

            if (backLimit != 0)
            {
                LabBackLimit.Text = "True";
            }
            else
            {
                LabBackLimit.Text = "False";
            }
            if (frontLimit != 0)
            {
                labFrontLimit.Text = "True";
            }
            else
            {
                labFrontLimit.Text = "False";
            }


            if (fFrame > 0)
            {
                labFFrame.Text = fFrame.ToString();
                labFTick.Text = ftick.ToString();
                labFBlend.Text = fblend.ToString();
            }
            else
            {
                labFFrame.Text = String.Empty;
                labFTick.Text = String.Empty;
                labFBlend.Text = String.Empty;
            }
            if ((mFrame > 0) && (mFrame < 255))
            {
                labMFrame.Text = (mFrame & 0x0F).ToString();
                labMTick.Text = mTick.ToString();
                labMBlend.Text = Convert.ToBoolean(mFrame & 0x0F).ToString();
            }
            else
            {
                labMFrame.Text = String.Empty;
                labMTick.Text = String.Empty;
                labMBlend.Text = String.Empty;
            }

            labDoorOffSet.Text = doorOffSet.ToString();
            labDoorIndex.Text = doorIndex.ToString();
            labEntityDoor.Text = entityDoor.ToString();

            labLight.Text = light.ToString();
            labfishing.Text = fishing.ToString();
        }

    }
}
