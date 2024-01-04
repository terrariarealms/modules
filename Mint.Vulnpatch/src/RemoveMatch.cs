using Microsoft.Xna.Framework;
using Terraria;

namespace Mint.Vulnpatch;

public unsafe class RemoveMatch : IMatch
{
    public RemoveMatch(int height, int type)
    {
        _height = height;
        _type = type;
    }

    private int _height;
    private int _type;

    public bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Height != _height)
            return false;
            
        for (int i = 0; i < rectangle.Width; i++)
        for (int j = 0; j < rectangle.Height; j++)
            if (tiles[i, j].Active)
                return false;

        int width = rectangle.X + rectangle.Width;
        int height = rectangle.Y + rectangle.Height;

        for (int i = rectangle.X; i < width; i++)
        for (int j = rectangle.Y; j < height; j++)
        {
            TileData* ptr = Main.tile[i, j].ptr;
            if (ptr->type != _type || ptr->active())
                return false;
            else *ptr = new TileData();
        }

        return true;
    }
}