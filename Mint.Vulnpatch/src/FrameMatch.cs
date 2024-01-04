using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Mint.Vulnpatch;

public unsafe class FrameMatch : IMatch
{
    public FrameMatch(int height, int type, int maxX, int maxY, int sizeX, int sizeY)
    {
        _height = height;
        _type = type;

        _maxX = maxX;
        _maxY = maxY;
        _sizeX = sizeX;
        _sizeY = sizeY;
    }

    private int _height;
    private int _type;

    private int _maxX;
    private int _maxY;
    private int _sizeX;
    private int _sizeY;

    public bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Height != _height)
            return false;
            
        for (int i = 0; i < rectangle.Width; i++)
        for (int j = 0; j < rectangle.Height; j++)
        {
            NetTile tile = tiles[i, j];

            if (!tile.Active)
                return false;

            if (_maxX != -1 && (tile.FrameX < 0 || tile.FrameX > _maxX))
                return false;

            if (_maxY != -1 && (tile.FrameY < 0 || tile.FrameY > _maxY))
                return false;

            if (_sizeX != -1 && (tile.FrameX % _sizeX == 0))
                return false;

            if (_sizeY != -1 && (tile.FrameY % _sizeY == (_type == TileID.LunarMonolith ? 2 : 0)))
                return false;
        }

        int width = rectangle.X + rectangle.Width;
        int height = rectangle.Y + rectangle.Height;

        for (int i = rectangle.X; i < width; i++)
        for (int j = rectangle.Y; j < height; j++)
        {
            int fixedX = i - rectangle.X;
            int fixedY = j - rectangle.Y;

            NetTile tile = tiles[i, j];

            TileData* ptr = Main.tile[i, j].ptr;

            if (_maxX != -1)
                ptr->frameX = tile.FrameX;

            if (_maxY != -1)
                ptr->frameY = tile.FrameY;
        }

        return true;
    }
}