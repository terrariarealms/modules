using Microsoft.Xna.Framework;
using Terraria;

namespace Mint.Vulnpatch;

// grabbed from tshock because im super lazy
public class ConversionMatch : IMatch
{
    public unsafe bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Height != 1) return false;

        NetTile tile = tiles[0, 0];
        TileData* ptr = Main.tile[rectangle.X, rectangle.Y].ptr;
        if (tile.TileID == ptr->type && tile.Wall == ptr->wall)
            return false;
        
        WorldGenMock.SimulateConversionChange(rectangle.X, rectangle.Y, out var validTiles, out var validWalls);
        if (validTiles.Contains(tile.TileID))
        {
            ptr->type = tile.TileID;
            if (Main.tileFrameImportant[tile.TileID])
            {
                ptr->frameX = tile.FrameX;
                ptr->frameY = tile.FrameY;
            }
            return true;
        }


        if (validWalls.Contains(tile.Wall))
        {
            ptr->wall = tile.Wall;
            return true;
        }

        return false;
    }
}