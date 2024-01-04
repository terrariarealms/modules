using Terraria;

namespace Mint.Vulnpatch;

public struct NetTile
{
    public NetTile(BinaryReader reader)
    {
        var bitsBytes = (BitsByte)reader.ReadByte();
        var bitsBytes2 = (BitsByte)reader.ReadByte();
        var bitsBytes3 = (BitsByte)reader.ReadByte();

        Active = bitsBytes[0];

        if (bitsBytes[4])
            Wire = true;

        if (bitsBytes[5])
            HalfBrick = true;

        if (bitsBytes[6])
            Actuator = true;

        if (bitsBytes[7])
            Inactive = true;

        Wire2 = bitsBytes2[0];
        Wire3 = bitsBytes2[1];

        if (bitsBytes2[2])
            TileColor = reader.ReadByte();
        
        if (bitsBytes2[3])
            WallColor = reader.ReadByte();

        if (Active)
        {
            TileID = reader.ReadUInt16();
            if (Main.tileFrameImportant[TileID])
            {
                FrameX = reader.ReadInt16();
                FrameY = reader.ReadInt16();
            }

            if (bitsBytes2[4])
                Slope++;

            if (bitsBytes2[5])
                Slope += 2;

            if (bitsBytes2[6])
                Slope += 4;
        }

        if (bitsBytes[3])
        {
            Liquid = reader.ReadByte();
            LiquidID = reader.ReadByte();
        }

        Wire4 = bitsBytes2[7];

        Fullbright = bitsBytes3[0];
        FullbrightWall = bitsBytes3[1];
        Invisible = bitsBytes3[2];
        InvisibleWall = bitsBytes3[3];
        
        if (bitsBytes[2]) Wall = reader.ReadUInt16();

    }

    public bool Active;
    public ushort TileID;
    public short FrameX;
    public short FrameY;
    public ushort Wall;
    public byte Liquid;
    public byte LiquidID;
    public bool Wire;
    public bool Wire2;
    public bool Wire3;
    public bool Wire4;
    public bool Inactive;
    public bool HalfBrick;
    public bool Actuator;
    public byte TileColor;
    public byte WallColor;
    public byte Slope;
    public bool Fullbright;
    public bool FullbrightWall;
    public bool Invisible;
    public bool InvisibleWall;
}