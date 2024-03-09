using Server.MirEnvir;

namespace Server.MirDatabase
{
    public class MovementInfo
    {
        public int MapIndex;
        public Point Source, Destination;
        public bool NeedMove, ShowOnBigMap;
        public int Icon;

        public MovementInfo()
        {

        }

        public MovementInfo(BinaryReader reader)
        {
            MapIndex = reader.ReadInt32();
            Source = new Point(reader.ReadInt32(), reader.ReadInt32());
            Destination = new Point(reader.ReadInt32(), reader.ReadInt32());

            NeedMove = reader.ReadBoolean();

            if (Envir.LoadVersion < 95) return;
            ShowOnBigMap = reader.ReadBoolean();
            Icon = reader.ReadInt32();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(MapIndex);
            writer.Write(Source.X);
            writer.Write(Source.Y);
            writer.Write(Destination.X);
            writer.Write(Destination.Y);
            writer.Write(NeedMove);
            writer.Write(ShowOnBigMap);
            writer.Write(Icon);
        }


        public override string ToString()
        {
            return string.Format("{0} -> Map :{1} - {2}", Source, MapIndex, Destination);
        }
    }
}
