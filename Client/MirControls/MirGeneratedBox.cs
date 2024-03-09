using Client.MirGraphics;

namespace Client.MirControls
{
    public enum GeneratedBoxSections    
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Middle,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public sealed class MirGeneratedBox : MirImageControl
    {
        private GeneratedBoxSections[,] boxArray;
        private int SectionOffset;
        public Size FixedSize;
        public MirGeneratedBox(int offset, Size size)
        {
            SectionOffset = offset;
            DrawImage = false;
            FixedSize = new Size(size.Width / 40 * 40, size.Height / 40 * 40);
            boxArray = new GeneratedBoxSections[FixedSize.Width / 40, FixedSize.Height / 40];

            for (int y = 0; y < boxArray.GetLength(1); y++)
                for (int x = 0; x < boxArray.GetLength(0); x++)
                {
                    GeneratedBoxSections section = GetSectionForRowAndCol(y, x);
                    boxArray[x, y] = section;
                }

            BeforeDraw += (o,e) => DrawSections();
        }

        private GeneratedBoxSections GetSectionForRowAndCol(int row, int col)
        {
            int width = boxArray.GetLength(0);
            int height = boxArray.GetLength(1);

            if (row == 0)
            {
                if (col == 0)
                    return GeneratedBoxSections.TopLeft;
                else if (col == width - 1)
                    return GeneratedBoxSections.TopRight;
                else
                    return GeneratedBoxSections.Top;
            }
            else if (row == height - 1)
            {
                if (col == 0)
                    return GeneratedBoxSections.BottomLeft;
                else if (col == width - 1)
                    return GeneratedBoxSections.BottomRight;
                else
                    return GeneratedBoxSections.Bottom;
            }
            else
            {
                if (col == 0)
                    return GeneratedBoxSections.Left;
                else if (col == width - 1)
                    return GeneratedBoxSections.Right;
                else
                    return GeneratedBoxSections.Middle;
            }
        }

        private void DrawSections()
        {
            for (int y = 0; y < boxArray.GetLength(1); y++)
                for (int x = 0; x < boxArray.GetLength(0); x++)
                {
                    GeneratedBoxSections section = boxArray[x, y];
                    Libraries.Prguse.Draw(SectionOffset + (int)section, Parent.DisplayLocation.X + Location.X + x * 40, Parent.DisplayLocation.Y + Location.Y + y * 40);
                }
        }
    }
}
