using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Terraria;

namespace Mint.Vulnpatch;

public unsafe class FlowersMatch : IMatch
{
    private static readonly int[] FlowersTypes = new int[]
    {
        3, 110, 113, 74, 637
    };
    
    public bool IsMatches(Rectangle rectangle, NetTile[,] tiles)
    {
        if (rectangle.Width != 1 || rectangle.Height != 1)
            return false;

        NetTile flower = tiles[0, 0];
        foreach (int type in FlowersTypes)
            if (flower.TileID == type)
            {
                if (PlaceFlower(rectangle.X, rectangle.Y))
                NetMessage.SendTileSquare(-1, rectangle.X, rectangle.Y);
                
                return true;
            }

        return false;
    }

    bool PlaceFlower(int X, int Y)
	{
		TileData* tile = Main.tile[X, Y].ptr;
        TileData* grass = Main.tile[X, Y + 1].ptr;
		if (tile == null || grass == null)
		{
			return false;
		}
		if (!tile->active() && tile->liquid == 0 && WorldGen.SolidTile(X, Y + 1))
		{
			tile->frameY = 0;
			tile->slope(0);
			tile->halfBrick(halfBrick: false);
			if (grass->type == 2 || grass->type == 477)
			{
				int num = Main.rand.NextFromList<int>(6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24, 27, 30, 33, 36, 39, 42);
				switch (num)
				{
					case 21:
					case 24:
					case 27:
					case 30:
					case 33:
					case 36:
					case 39:
					case 42:
						num += Main.rand.Next(3);
						break;
				}
				tile->active(active: true);
				tile->type = 3;
				tile->frameX = (short)(num * 18);
				tile->CopyPaintAndCoating(Main.tile[X, Y + 1]);
				return true;
			}
			if (grass->type == 109 || grass->type == 492)
			{
				if (Main.rand.Next(2) == 0)
				{
					tile->active(active: true);
					tile->type = 110;
					tile->frameX = (short)(18 * Main.rand.Next(4, 7));
					tile->CopyPaintAndCoating(Main.tile[X, Y + 1]);
					while (tile->frameX == 90)
					{
						tile->frameX = (short)(18 * Main.rand.Next(4, 7));
					}
				}
				else
				{
					tile->active(active: true);
					tile->type = 113;
					tile->frameX = (short)(18 * Main.rand.Next(2, 8));
					tile->CopyPaintAndCoating(Main.tile[X, Y + 1]);
					while (tile->frameX == 90)
					{
						tile->frameX = (short)(18 * Main.rand.Next(2, 8));
					}
				}
				return true;
			}
			if (grass->type == 60)
			{
				tile->active(active: true);
				tile->type = 74;
				tile->frameX = (short)(18 * Main.rand.Next(9, 17));
				tile->CopyPaintAndCoating(Main.tile[X, Y + 1]);
				return true;
			}
			if (grass->type == 633)
			{
				tile->active(active: true);
				tile->type = 637;
				tile->frameX = (short)(18 * Main.rand.Next(6, 11));
				tile->CopyPaintAndCoating(Main.tile[X, Y + 1]);
				return true;
			}
		}
		return false;
	}
}