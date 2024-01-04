using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Mint.Vulnpatch;

public class ChristmasTreeMatch : IMatch
{
    public unsafe bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Height != 1) return false;

        NetTile tile = tiles[0, 0];
        TileData* ptr = Main.tile[rectangle.X, rectangle.Y].ptr;
        if (ptr->type != TileID.ChristmasTree) return false;

        if (tile.FrameX < 0 || tile.FrameX > 64 || tile.FrameX % 16 != 0)
            return false;

        if (tile.FrameY < 0 || tile.FrameY > 128 || tile.FrameY % 16 != 0)
            return false;

        return true;
    }
}