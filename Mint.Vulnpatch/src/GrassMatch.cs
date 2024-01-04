using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Mint.Vulnpatch;

public class GrassMatch : IMatch
{
    public unsafe bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Height != 1) return false;

        NetTile tile = tiles[0, 0];
        TileData* ptr = Main.tile[rectangle.X, rectangle.Y].ptr;
        if (tile.TileID == ptr->type)
            return false;

        ushort? grass = GetGolfGrass(ptr->type);
        if (grass == tile.TileID)
        {
            ptr->type = tile.TileID;
            ptr->frameX = -1;
            ptr->frameY = -1;

			if (TileID.Sets.IsVine[ptr->type])
				WorldGen.KillTile(rectangle.X, rectangle.Y + 1);

            return true;
        }
            
        return false;
    }

    ushort? GetGolfGrass(ushort tileId)
    {
        switch (tileId)
        {
            case TileID.Grass: return TileID.GolfGrass;
            case TileID.HallowedGrass: return TileID.GolfGrassHallowed;
            default: return null;
        }
    }
}