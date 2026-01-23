using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class SkillDialog : MirImageControl
    {
        public SkillDialog()
        {
            Index = 157;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Size = new Size(380, 308);
            Location = Center;
        }

        public void Toggle()
        {
            if (Visible) Hide();
            else Show();
        }

        public void Refresh()
        {
        }
    }
}
