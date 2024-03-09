using System;
using System.IO;

namespace Map_Editor
{
    public class CellInfo
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        public short BackIndex;
        /// <summary>
        /// 资源内图片索引
        /// </summary>
        public int BackImage;
        public short MiddleIndex;
        public short MiddleImage;
        public short FrontIndex;
        public short FrontImage;

        public byte DoorIndex;
        public byte DoorOffset;

        public byte FrontAnimationFrame;
        public byte FrontAnimationTick;

        public byte MiddleAnimationFrame;
        public byte MiddleAnimationTick;

        public short TileAnimationImage;
        public short TileAnimationOffset;
        public byte TileAnimationFrames;

        public byte Light;
        public byte Unknown;


        public bool FishingCell;
        //public List<MapObject> CellObjects;
        //public void DrawObjects()
        //{
        //    if (CellObjects == null) return;
        //    for (int i = 0; i < CellObjects.Count; i++)
        //        CellObjects[i].Draw();
        //}
    }

    public class MapReader
    {
        public int Width, Height;
        public CellInfo[,] MapCells;
        private string FileName;
        private byte[] Bytes;

        public MapReader(string FileName)
        {
            this.FileName = FileName;
            initiate();
        }

        private void initiate()
        {
            if (File.Exists(FileName))
            {
                Bytes = File.ReadAllBytes(FileName);
            }
            else
            {
                Width = 1000;
                Height = 1000;
                MapCells = new CellInfo[Width, Height];

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        MapCells[x, y] = new CellInfo();
                    }
                return;
            }
            LoadMapType8();

        }
        private void LoadMapType8()
        {
            try
            {
                int offset = 0;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                MapCells = new CellInfo[Width, Height];

                offset = 56;

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        MapCells[x, y] = new CellInfo();
                        MapCells[x, y].MiddleIndex = 1;
                        MapCells[x, y].MiddleImage = (short)BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        MapCells[x, y].FrontIndex = 2;
                        MapCells[x, y].FrontImage = (short)BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        if ((MapCells[x, y].MiddleImage & 0x8000) != 0)
                            MapCells[x, y].BackImage = 0x20000000;
                        MapCells[x, y].MiddleImage = (short)(MapCells[x, y].MiddleImage & 0x7FFF);
                    }
            }
            catch (Exception ex)
            {
                // if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }
        }
    }
}
 