using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class SkillDialog : MirImageControl
    {
        private MirButton NextButton, PreviousButton;
        private MirLabel PageLabel;
        private SkillPage CurrentPage;

        private int CurrentPageNumber = 0;
        private readonly List<SkillPage> Pages = new List<SkillPage>();

        public SkillDialog()
        {
            Index = 157;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Size = new Size(380, 308);
            Location = Center;

            LoadSkillPages();

            PreviousButton = new MirButton
            {
                Index = 240,
                HoverIndex = 241,
                PressedIndex = 242,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 16),
                Location = new Point(21, 23),
                Sound = SoundList.ButtonA
            };
            PreviousButton.Click += (o, e) => ChangePage(-1);

            NextButton = new MirButton
            {
                Index = 243,
                HoverIndex = 244,
                PressedIndex = 245,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 16),
                Location = new Point(310, 23),
                Sound = SoundList.ButtonA
            };
            NextButton.Click += (o, e) => ChangePage(+1);

            PageLabel = new MirLabel
            {
                Font = new Font(Settings.FontName, 9F),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Location = new Point(230, 28),
                Size = new Size(80, 20),
                ForeColour = Color.White
            };

            DisplayPage(0);
        }

        private void LoadSkillPages()
        {
            Point contentLocation = new Point(12, 35);
        }

        private void ChangePage(int delta)
        {
            if (Pages.Count == 0) return;
            CurrentPageNumber = (CurrentPageNumber + delta + Pages.Count) % Pages.Count;
            DisplayPage(CurrentPageNumber);
        }

        public void DisplayPage(int id)
        {
            if (Pages.Count == 0) return;

            id = Math.Max(0, Math.Min(id, Pages.Count - 1));

            if (CurrentPage != null)
            {
                CurrentPage.Visible = false;
                if (CurrentPage.Page != null) CurrentPage.Page.Visible = false;
            }

            CurrentPage = Pages[id];
            CurrentPage.Visible = true;
            if (CurrentPage.Page != null) CurrentPage.Page.Visible = true;

            CurrentPageNumber = id;
            CurrentPage.PageTitleLabel.Text = $"{id + 1}. {CurrentPage.Title}";
            PageLabel.Text = $"{id + 1} / {Pages.Count}";

            Show();
        }

        public void Toggle()
        {
            if (Visible) Hide();
            else Show();
        }

        public sealed class SkillPage : MirControl
        {
            public string Title;
            public int ImageID;
            public MirControl Page;
            public MirLabel PageTitleLabel;

            public SkillPage(string title, int imageID, MirControl page)
            {
                Title = title;
                ImageID = imageID;
                Page = page;
                NotControl = true;
                Size = new Size(508, 436);

                BeforeDraw += SkillPage_BeforeDraw;

                PageTitleLabel = new MirLabel
                {
                    Text = title,
                    Font = new Font(Settings.FontName, 10F, FontStyle.Bold),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Parent = this,
                    Size = new Size(242, 30),
                    Location = new Point((Size.Width - 242) / 2, 4)
                };
            }

            private void SkillPage_BeforeDraw(object sender, EventArgs e)
            {
                if (ImageID < 0) return;
                Libraries.Help.Draw(
                    ImageID,
                    new Point(DisplayLocation.X, DisplayLocation.Y + 40),
                    Color.White,
                    false
                );
            }
        }
    }
}
