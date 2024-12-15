public class UserAttribute
{
    public Attribute Type;
    public uint Level, Points;
    public ulong Experience;

    public UserAttribute(Attribute type) 
    {
        Type = type;
    }
    public UserAttribute(BinaryReader reader, int version = int.MaxValue, int customVersion = int.MaxValue)
    {
        Type = (Attribute)reader.ReadByte();
        Level = reader.ReadUInt32();
        Points = reader.ReadUInt32();
        Experience = reader.ReadUInt64();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)Type);
        writer.Write(Level);
        writer.Write(Points);
        writer.Write(Experience);
    }
}
