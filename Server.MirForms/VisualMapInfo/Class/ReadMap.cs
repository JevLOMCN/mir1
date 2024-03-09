namespace Server.MirForms.VisualMapInfo.Class
{
    public class ReadMap
    {
        public int Width, Height, MonsterCount;
        public Cell[,] Cells;
        public long LightningTime, FireTime;
        public Bitmap clippingZone;
        public string mapFile;

        private void LoadMapCells(byte[] fileBytes)
        {
            int offSet = 0;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];

            offSet = 56;

            clippingZone = new Bitmap(Width, Height);

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)                              
                {
                    clippingZone.SetPixel(x, y, Color.WhiteSmoke);

                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        clippingZone.SetPixel(x, y, Color.Black);
                    offSet += 2;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        clippingZone.SetPixel(x, y, Color.Black);
                    offSet += 2;                        
                }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(Path.Combine("Maps", mapFile + ".map")))
                {
                    byte[] fileBytes = File.ReadAllBytes(Path.Combine("Maps", mapFile + ".map"));
                    LoadMapCells(fileBytes);
                }
            }

            catch (Exception) { }

            VisualizerGlobal.ClippingMap = clippingZone;
        }

        public Cell GetCell(Point location)
        {
            return Cells[location.X, location.Y];
        }

        public Cell GetCell(int x, int y)
        {
            return Cells[x, y];
        }
    }

    public class Cell
    {
        public static readonly Cell HighWall;
        public static readonly Cell LowWall;
    }
}
