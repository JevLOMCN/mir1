using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;

namespace Client.MirControls
{
    public sealed class MirChatLabel : MirControl
    {
        private const int TopLeftCornerIndex = 62;
        private const int TopIndex = 58;
        private const int TopRightCornerIndex = 63;
        private const int LeftIndex = 59;
        private const int RightIndex = 60;
        private const int BottomLeftCornerIndex = 64;
        private const int BottomLeftIndex = 61;
        private const int BottomCentreIndex = 67;
        private const int BottomRightIndex = 61;
        private const int BottomRightCornerIndex = 65;

        public MirLabel Label { get; private set; }
        public MirImageControl TopLeftCorner { get; private set; }
        public MirImageControl Top { get; private set; }
        public MirImageControl TopRightCorner { get; private set; }
        public MirImageControl Left { get; private set; }
        public MirImageControl Right { get; private set; }
        public MirImageControl BottomLeftCorner { get; private set; }
        public MirImageControl BottomLeft { get; private set; }
        public MirImageControl BottomCentre { get; private set; }
        public MirImageControl BottomRight { get; private set; }
        public MirImageControl BottomRightCorner { get; private set; }

        public string Text
        {
            get { return Label.Text; }
            set { Label.Text = value; }
        }

        public bool AutoSize
        {
            get { return Label.AutoSize; }
            set { Label.AutoSize = value; }
        }

        public bool OutLine
        {
            get { return Label.OutLine; }
            set { Label.OutLine = value; }
        }

        public Color OutLineColour
        {
            get { return Label.OutLineColour; }
            set { Label.OutLineColour = value; }
        }

        public TextFormatFlags DrawFormat
        {
            get { return Label.DrawFormat; }
            set { Label.DrawFormat = value; }
        }

        public MirChatLabel()
        {
            DrawControlTexture = false;
            ForeColour = Color.White;
            BackColour = Color.Transparent;

            BottomCentre = CreateStretchedControl(BottomCentreIndex);
            BottomLeft = CreateStretchedControl(BottomLeftIndex);
            BottomRight = CreateStretchedControl(BottomRightIndex);
            Left = CreateStretchedControl(LeftIndex);
            Right = CreateStretchedControl(RightIndex);
            Top = CreateStretchedControl(TopIndex);

            BottomLeftCorner = CreateImageControl(BottomLeftCornerIndex);
            BottomRightCorner = CreateImageControl(BottomRightCornerIndex);
            TopLeftCorner = CreateImageControl(TopLeftCornerIndex);
            TopRightCorner = CreateImageControl(TopRightCornerIndex);

            Label = new MirLabel
            {
                AutoSize = true,
                BackColour = BackColour,
                ForeColour = ForeColour,
                OutLine = true,
                OutLineColour = Color.Black,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this
            };
            Label.SizeChanged += (o, e) => UpdateLayout();
        }

        protected override void OnBackColourChanged()
        {
            base.OnBackColourChanged();

            if (Label != null)
                Label.BackColour = BackColour;
        }

        protected override void OnForeColourChanged()
        {
            base.OnForeColourChanged();

            if (Label != null)
                Label.ForeColour = ForeColour;
        }

        private MirImageControl CreateImageControl(int index)
        {
            return new MirImageControl
            {
                Library = Libraries.Prguse,
                Index = index,
                Parent = this
            };
        }

        private MirImageControl CreateStretchedControl(int index)
        {
            MirImageControl control = new MirImageControl
            {
                AutoSize = false,
                DrawImage = false,
                Library = Libraries.Prguse,
                Index = index,
                ForeColour = Color.White,
                Parent = this
            };

            control.BeforeDraw += (o, e) =>
            {
                if (control.Size.Width <= 0 || control.Size.Height <= 0)
                    return;

                control.Library.Draw(control.Index, control.DisplayLocation, control.Size, control.ForeColour);
            };

            return control;
        }

        private Size GetImageSize(int index)
        {
            return Libraries.Prguse == null ? Size.Empty : Libraries.Prguse.GetSize(index);
        }

        private int Max(params int[] values)
        {
            int max = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max)
                    max = values[i];
            }

            return max;
        }

        private static int Clamp0(int v) => v < 0 ? 0 : v;

        private static int MaxW(params Size[] sizes)
        {
            int m = 0;
            for (int i = 0; i < sizes.Length; i++) m = Math.Max(m, sizes[i].Width);
            return m;
        }

        private static int MaxH(params Size[] sizes)
        {
            int m = 0;
            for (int i = 0; i < sizes.Length; i++) m = Math.Max(m, sizes[i].Height);
            return m;
        }

        private void UpdateLayout()
        {
            var labelSize = Label.Size;
            if (labelSize.IsEmpty)
            {
                Size = Size.Empty;
                return;
            }

            // Measure
            var tl = GetImageSize(TopLeftCornerIndex);
            var tr = GetImageSize(TopRightCornerIndex);
            var bl = GetImageSize(BottomLeftCornerIndex);
            var br = GetImageSize(BottomRightCornerIndex);

            var top = GetImageSize(TopIndex);
            var left = GetImageSize(LeftIndex);
            var right = GetImageSize(RightIndex);

            var botL = GetImageSize(BottomLeftIndex);
            var botC = GetImageSize(BottomCentreIndex);
            var botR = GetImageSize(BottomRightIndex);

            // Frame thickness
            int leftW = MaxW(left, tl, bl);
            int rightW = MaxW(right, tr, br);
            int topH = MaxH(top, tl, tr);
            int bottomH = MaxH(botC, bl, br, botL, botR);

            // Overall size
            int contentW = labelSize.Width;
            int contentH = labelSize.Height;

            int totalW = leftW + contentW + rightW;
            int totalH = topH + contentH + bottomH;

            Size = new Size(totalW, totalH);
            Label.Location = new Point(leftW, topH);

            int innerW = Clamp0(totalW - leftW - rightW);
            int innerH = Clamp0(totalH - topH - bottomH);
            int bottomY = totalH - bottomH;

            // Corners
            TopLeftCorner.Location = Point.Empty;
            TopRightCorner.Location = new Point(totalW - tr.Width, 0);
            BottomLeftCorner.Location = new Point(0, totalH - bl.Height);
            BottomRightCorner.Location = new Point(totalW - br.Width, totalH - br.Height);

            // Top / Left / Right
            Top.Location = new Point(leftW - 2, 0);
            Top.Size = new Size((int)(1.2 * innerW + 9), topH);

            Left.Location = new Point(0, topH);
            Left.Size = new Size(leftW, innerH);

            Right.Location = new Point(totalW - rightW, topH);
            Right.Size = new Size(rightW, innerH);

            // Bottom centre
            int botCW = botC.Width;
            int botCH = botC.Height;
            int botCX = Math.Max(0, (totalW - botCW) / 2);

            BottomCentre.Location = new Point(botCX, totalH - botCH);
            BottomCentre.Size = botC;

            // Bottom left strip
            int botLStart = tl.Width - 2;
            int botLWidth = Clamp0(botCX - botLStart);

            BottomLeft.Location = new Point(botLStart, bottomY);
            BottomLeft.Size = new Size((int)(1.3 * botLWidth), bottomH);

            // Bottom right strip
            int botRStart = botCX + botCW - 2;
            int botREnd = totalW - tr.Width + 2;
            int botRWidth = Clamp0(botREnd - botRStart);

            BottomRight.Location = new Point(botRStart - 2, bottomY);
            BottomRight.Size = new Size((int)(1.3 * Math.Max(16, botRWidth)), bottomH);
        }

    }
}
