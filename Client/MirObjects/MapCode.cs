namespace Client.MirObjects
{
    public class CellInfo
    {
        public short BackIndex;
        public int BackImage;
        public short MiddleIndex;
        public int MiddleImage;
        public short FrontIndex;
        public int FrontImage;

        public byte DoorIndex;
        public byte DoorOffset;

        public byte FrontAnimationFrame;
        public byte FrontAnimationTick;

        public byte MiddleAnimationFrame;
        public byte MiddleAnimationTick;

        public short TileAnimationImage;
        public short TileAnimationOffset;
        public byte  TileAnimationFrames;

        public byte Light;
        public byte Unknown;
        public List<MapObject> CellObjects;

        public void AddObject(MapObject ob)
        {
            if (CellObjects == null) CellObjects = new List<MapObject>();

            CellObjects.Insert(0, ob);
            Sort();
        }
        public void RemoveObject(MapObject ob)
        {
            if (CellObjects == null) return;

            CellObjects.Remove(ob);

            if (CellObjects.Count == 0) CellObjects = null;
            else Sort();
        }
        public MapObject FindObject(uint ObjectID)
        {
            return CellObjects.Find(
                delegate(MapObject mo)
            {
                return mo.ObjectID == ObjectID;
            });
        }
        public void DrawObjects()
        {
            if (CellObjects == null) return;

            for (int i = 0; i < CellObjects.Count; i++)
            {
                if (!CellObjects[i].Dead)
                {
                    CellObjects[i].Draw();
                    continue;
                }                
            }
        }
        public void DrawDeadObjects()
        {
            if (CellObjects == null) return;
            for (int i = 0; i < CellObjects.Count; i++)
            {
                if (!CellObjects[i].Dead) continue;

                CellObjects[i].Draw();
            }
        }

        public void Sort()
        {
            CellObjects.Sort(delegate(MapObject ob1, MapObject ob2)
            {
                if (ob1.Race == ObjectType.Item && ob2.Race != ObjectType.Item)
                    return -1;
                if (ob2.Race == ObjectType.Item && ob1.Race != ObjectType.Item)
                    return 1;
                if (ob1.Race == ObjectType.Spell && ob2.Race != ObjectType.Spell)
                    return -1;
                if (ob2.Race == ObjectType.Spell && ob1.Race != ObjectType.Spell)
                    return 1;

                int i = ob2.Dead.CompareTo(ob1.Dead);
                return i == 0 ? ob1.ObjectID.CompareTo(ob2.ObjectID) : i;
            });
        }
    }

    class MapReader
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

            LoadMap();
        }

        private void LoadMap()
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
                        MapCells[x, y].MiddleImage = (MapCells[x, y].MiddleImage & 0x7FFF);
                    }
            }
            catch (Exception ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }
        }
    }
}
