using System;

namespace PlayerAgents.Map;

internal static class MapParser
{
    internal readonly struct MapCells
    {
        public readonly bool[,] Walk;
        public readonly byte[,] Doors;

        public MapCells(bool[,] walk, byte[,] doors)
        {
            Walk = walk;
            Doors = doors;
        }
    }
    public static byte FindType(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 4) return 0;
        if (bytes[2] == 0x43 && bytes[3] == 0x23) return 100;
        if (bytes[0] == 0) return 5;
        if (bytes[0] == 0x0F && bytes[5] == 0x53 && bytes[14] == 0x33) return 6;
        if (bytes[0] == 0x15 && bytes[4] == 0x32 && bytes[6] == 0x41 && bytes[19] == 0x31) return 4;
        if (bytes[0] == 0x10 && bytes[2] == 0x61 && bytes[7] == 0x31 && bytes[14] == 0x31) return 1;
        if ((bytes[4] == 0x0F || bytes[4] == 0x03) && bytes[18] == 0x0D && bytes[19] == 0x0A)
        {
            int w = bytes[0] + (bytes[1] << 8);
            int h = bytes[2] + (bytes[3] << 8);
            if (bytes.Length > (52 + (w * h * 14))) return 3;
            return 2;
        }
        if (bytes[0] == 0x0D && bytes[1] == 0x4C && bytes[7] == 0x20 && bytes[11] == 0x6D) return 7;
        return 0;
    }

    public static MapCells LoadV0(byte[] bytes)
    {
        int offset = 0;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 52;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 4;

                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 3;

                offset += 1; // light

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV1(byte[] bytes)
    {
        int offset = 21;
        int w = BitConverter.ToInt16(bytes, offset); offset += 2;
        int xor = BitConverter.ToInt16(bytes, offset); offset += 2;
        int h = BitConverter.ToInt16(bytes, offset); offset += 2;
        int width = w ^ xor;
        int height = h ^ xor;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 54;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if (((BitConverter.ToInt32(bytes, offset) ^ 0xAA38AA38) & 0x20000000) != 0) walkable = false;
                offset += 6;
                if (((BitConverter.ToInt16(bytes, offset) ^ xor) & 0x8000) != 0) walkable = false;
                offset += 2;

                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 5;

                offset += 1; // light
                offset += 1; // unknown

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV2(byte[] bytes)
    {
        int offset = 0;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 52;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;

                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 5;

                offset += 1; // light
                offset += 2;

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV3(byte[] bytes)
    {
        int offset = 0;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 52;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;

                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 12;

                offset += 1; // light
                offset += 17;

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV4(byte[] bytes)
    {
        int offset = 31;
        int w = BitConverter.ToInt16(bytes, offset); offset += 2;
        int xor = BitConverter.ToInt16(bytes, offset); offset += 2;
        int h = BitConverter.ToInt16(bytes, offset); offset += 2;
        int width = w ^ xor;
        int height = h ^ xor;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 64;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;

                offset += 4;
                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 6;
                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV5(byte[] bytes)
    {
        int offset = 22;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 28 + (3 * ((width / 2) + (width % 2)) * (height / 2));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                byte flag = bytes[offset];
                if ((flag & 0x01) != 1) walkable = false;
                else if ((flag & 0x02) != 2) walkable = false;
                offset += 14;
                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV6(byte[] bytes)
    {
        int offset = 16;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 40;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                byte flag = bytes[offset];
                if ((flag & 0x01) != 1) walkable = false;
                else if ((flag & 0x02) != 2) walkable = false;
                offset += 20;
                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV7(byte[] bytes)
    {
        int offset = 21;
        int width = BitConverter.ToInt16(bytes, offset); offset += 4;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 54;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 6;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;

                offset += 2;
                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 4;

                offset += 1; // light
                offset += 2;

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }

    public static MapCells LoadV100(byte[] bytes)
    {
        int offset = 4;
        int width = BitConverter.ToInt16(bytes, offset); offset += 2;
        int height = BitConverter.ToInt16(bytes, offset); offset += 2;
        bool[,] walk = new bool[width, height];
        byte[,] doors = new byte[width, height];
        offset = 8;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool walkable = true;
                offset += 2;
                if ((BitConverter.ToInt32(bytes, offset) & 0x20000000) != 0) walkable = false;
                offset += 10;
                if ((BitConverter.ToInt16(bytes, offset) & 0x8000) != 0) walkable = false;
                offset += 2;

                byte door = bytes[offset];
                if (door > 0) doors[x, y] = door;
                offset += 11;

                offset += 1; // light

                walk[x, y] = walkable;
            }
        }
        return new MapCells(walk, doors);
    }
}
