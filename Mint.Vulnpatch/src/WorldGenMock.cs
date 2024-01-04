using Terraria;
using Terraria.ID;

namespace Mint.Vulnpatch;

// that was stolen from tshock because im too lazy for writing that fucking 500 lines just for one powders/clentaminators
internal unsafe static class WorldGenMock
{
    private sealed class MockTile
    {
		private readonly HashSet<ushort> _setTypes;
		private readonly HashSet<ushort> _setWalls;

		private ushort _type;
		private ushort _wall;

		public MockTile(ushort type, ushort wall, HashSet<ushort> setTypes, HashSet<ushort> setWalls)
		{
			_setTypes = setTypes;
			_setWalls = setWalls;
			_type = type;
			_wall = wall;
		}


		public ushort type
		{
			get => _type;
			set
			{
				_setTypes.Add(value);
				_type = value;
			}
		}

		public ushort wall
		{
			get => _wall;
			set
			{
				_setWalls.Add(value);
				_wall = value;
			}
		}

		}

		public static void SimulateConversionChange(int x, int y, out HashSet<ushort> validTiles, out HashSet<ushort> validWalls)
		{
            validTiles = new HashSet<ushort>();
            validWalls = new HashSet<ushort>();

            foreach (int conversionType in new int[] { 0, 1, 2, 3, 4, 5, 6, 7 })
            {
                MockTile mock = new(Main.tile[x, y].ptr->type, Main.tile[x, y].ptr->wall, validTiles, validWalls);
                Convert(mock, x, y, conversionType);
            }
		}


		private static void Convert(MockTile tile, int k, int l, int conversionType)
		{
		int type = tile.type;
		int wall = tile.wall;
		switch (conversionType)
		{
			case 4:
				if (WallID.Sets.Conversion.Grass[wall] && wall != 81)
				{
                    tile.wall = 81;
				}
				else if (WallID.Sets.Conversion.Stone[wall] && wall != 83)
				{
				    tile.wall = 83;
				}
				else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 218)
				{
				    tile.wall = 218;
				}
				else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 221)
				{
				    tile.wall = 221;
				}
				else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 192)
				{
				    tile.wall = 192;
				}
				else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 193)
				{
				    tile.wall = 193;
				}
				else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 194)
				{
			    	tile.wall = 194;
				}
				else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 195)
				{
			    	tile.wall = 195;
				}
				if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 203)
				{
			    	tile.type = 203;
				}
				else if (TileID.Sets.Conversion.JungleGrass[type] && type != 662)
				{
					tile.type = 662;
				}
				else if (TileID.Sets.Conversion.Grass[type] && type != 199)
				{
					tile.type = 199;
				}
				else if (TileID.Sets.Conversion.Ice[type] && type != 200)
				{
					tile.type = 200;
				}
				else if (TileID.Sets.Conversion.Sand[type] && type != 234)
				{
					tile.type = 234;
				}
				else if (TileID.Sets.Conversion.HardenedSand[type] && type != 399)
				{
					tile.type = 399;
				}
				else if (TileID.Sets.Conversion.Sandstone[type] && type != 401)
				{
					tile.type = 401;
				}
				else if (TileID.Sets.Conversion.Thorn[type] && type != 352)
				{
					tile.type = 352;
				}
				break;
			case 2:
				if (WallID.Sets.Conversion.Grass[wall] && wall != 70)
				{
					tile.wall = 70;
				}
				else if (WallID.Sets.Conversion.Stone[wall] && wall != 28)
				{
					tile.wall = 28;
				}
				else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 219)
				{
					tile.wall = 219;
				}
				else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 222)
				{
					tile.wall = 222;
				}
				else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 200)
				{
					tile.wall = 200;
				}
				else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 201)
				{
					tile.wall = 201;
				}
				else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 202)
				{
					tile.wall = 202;
				}
				else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 203)
				{
					tile.wall = 203;
				}
				if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 117)
				{
					tile.type = 117;
				}
				else if (TileID.Sets.Conversion.GolfGrass[type] && type != 492)
				{
					tile.type = 492;
				}
				else if (TileID.Sets.Conversion.Grass[type] && type != 109 && type != 492)
				{
					tile.type = 109;
				}
				else if (TileID.Sets.Conversion.Ice[type] && type != 164)
				{
					tile.type = 164;
				}
				else if (TileID.Sets.Conversion.Sand[type] && type != 116)
				{
					tile.type = 116;
				}
				else if (TileID.Sets.Conversion.HardenedSand[type] && type != 402)
				{
					tile.type = 402;
				}
				else if (TileID.Sets.Conversion.Sandstone[type] && type != 403)
				{
					tile.type = 403;
				}
				if (type == 59 && (Main.tile[k - 1, l].type == 109 || Main.tile[k + 1, l].type == 109 || Main.tile[k, l - 1].type == 109 || Main.tile[k, l + 1].type == 109))
				{
					tile.type = 0;
				}
				break;
			case 1:
				if (WallID.Sets.Conversion.Grass[wall] && wall != 69)
				{
					tile.wall = 69;
				}
				else if (TileID.Sets.Conversion.JungleGrass[type] && type != 661)
				{
					tile.type = 661;
				}
				else if (WallID.Sets.Conversion.Stone[wall] && wall != 3)
				{
					tile.wall = 3;
				}
				else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 217)
				{
					tile.wall = 217;
				}
				else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 220)
				{
					tile.wall = 220;
				}
				else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 188)
				{
					tile.wall = 188;
				}
				else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 189)
				{
					tile.wall = 189;
				}
				else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 190)
				{
					tile.wall = 190;
				}
				else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 191)
				{
					tile.wall = 191;
				}
				if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 25)
				{
					tile.type = 25;
				}
				else if (TileID.Sets.Conversion.Grass[type] && type != 23)
				{
					tile.type = 23;
				}
				else if (TileID.Sets.Conversion.Ice[type] && type != 163)
				{
					tile.type = 163;
				}
				else if (TileID.Sets.Conversion.Sand[type] && type != 112)
				{
					tile.type = 112;
				}
				else if (TileID.Sets.Conversion.HardenedSand[type] && type != 398)
				{
					tile.type = 398;
				}
				else if (TileID.Sets.Conversion.Sandstone[type] && type != 400)
				{
					tile.type = 400;
				}
				else if (TileID.Sets.Conversion.Thorn[type] && type != 32)
				{
					tile.type = 32;
				}
				break;
			case 3:
				if (WallID.Sets.CanBeConvertedToGlowingMushroom[wall])
				{
					tile.wall = 80;
				}
				if (tile.type == 60)
				{
					tile.type = 70;
				}
				break;
			case 5:
				if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.NewWall1[wall] || WallID.Sets.Conversion.NewWall2[wall] || WallID.Sets.Conversion.NewWall3[wall] || WallID.Sets.Conversion.NewWall4[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 187)
				{
					tile.wall = 187;
				}
				else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Dirt[wall] || WallID.Sets.Conversion.Snow[wall]) && wall != 216)
				{
					tile.wall = 216;
				}
				if ((TileID.Sets.Conversion.Grass[type] || TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 53)
				{
				    int num = 53;
                    if (WorldGen.BlockBelowMakesSandConvertIntoHardenedSand(k, l))
                    {
                        num = 397;
                    }
					tile.type = (ushort)num;
				}
				else if (TileID.Sets.Conversion.HardenedSand[type] && type != 397)
				{
					tile.type = 397;
				}
				else if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 396)
				{
					tile.type = 396;
				}
				break;
			case 6:
				if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.NewWall1[wall] || WallID.Sets.Conversion.NewWall2[wall] || WallID.Sets.Conversion.NewWall3[wall] || WallID.Sets.Conversion.NewWall4[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 71)
				{
					tile.wall = 71;
				}
				else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Dirt[wall] || WallID.Sets.Conversion.Snow[wall]) && wall != 40)
				{
					tile.wall = 40;
				}
				if ((TileID.Sets.Conversion.Grass[type] || TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 147)
				{
					tile.type = 147;
				}
				else if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 161)
				{
					tile.type = 161;
				}
				break;
			case 7:
				if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 1)
				{
					tile.wall = 1;
				}
				else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Snow[wall] || WallID.Sets.Conversion.Dirt[wall]) && wall != 2)
				{
					tile.wall = 2;
				}
				else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 196)
				{
					tile.wall = 196;
				}
				else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 197)
				{
					tile.wall = 197;
				}
				else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 198)
				{
					tile.wall = 198;
				}
				else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 199)
				{
					tile.wall = 199;
				}
				if ((TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 1)
				{
					tile.type = 1;
				}
				else if (TileID.Sets.Conversion.GolfGrass[type] && type != 477)
				{
					tile.type = 477;
				}
				else if (TileID.Sets.Conversion.Grass[type] && type != 2 && type != 477)
				{
					tile.type = 2;
				}
				else if ((TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 0)
				{
                    int num2 = 0;
                    if (WorldGen.TileIsExposedToAir(k, l))
                    {
                        num2 = 2;
                    }
					tile.type = (ushort)num2;
				}
				break;
		}
		if (tile.wall == 69 || tile.wall == 70 || tile.wall == 81)
		{
			if (l < Main.worldSurface)
			{
				tile.wall = 65;
				tile.wall = 63;
			}
			else
			{
				tile.wall = 64;
			}
		}
		else if (WallID.Sets.Conversion.Stone[wall] && wall != 1 && wall != 262 && wall != 274 && wall != 61 && wall != 185)
		{
			tile.wall = 1;
		}
		else if (WallID.Sets.Conversion.Stone[wall] && wall == 262)
		{
			tile.wall = 61;
		}
		else if (WallID.Sets.Conversion.Stone[wall] && wall == 274)
		{
			tile.wall = 185;
		}
		if (WallID.Sets.Conversion.NewWall1[wall] && wall != 212)
		{
			tile.wall = 212;
		}
		else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 213)
		{
			tile.wall = 213;
		}
		else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 214)
		{
			tile.wall = 214;
		}
		else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 215)
		{
			tile.wall = 215;
		}
		else if (tile.wall == 80)
		{
			tile.wall = 15;
			tile.wall = 64;
		}
		else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 216)
		{
			tile.wall = 216;
		}
		else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 187)
		{
			tile.wall = 187;
		}
		if (tile.type == 492)
		{
			tile.type = 477;
		}
		else if (TileID.Sets.Conversion.JungleGrass[type] && type != 60)
		{
			tile.type = 60;
		}
		else if (TileID.Sets.Conversion.Grass[type] && type != 2 && type != 477)
		{
			tile.type = 2;
		}
		else if (TileID.Sets.Conversion.Stone[type] && type != 1)
		{
			tile.type = 1;
		}
		else if (TileID.Sets.Conversion.Sand[type] && type != 53)
		{
			tile.type = 53;
		}
		else if (TileID.Sets.Conversion.HardenedSand[type] && type != 397)
		{
			tile.type = 397;
		}
		else if (TileID.Sets.Conversion.Sandstone[type] && type != 396)
		{
			tile.type = 396;
		}
		else if (TileID.Sets.Conversion.Ice[type] && type != 161)
		{
			tile.type = 161;
		}
		else if (TileID.Sets.Conversion.MushroomGrass[type])
		{
			tile.type = 60;
		}
	}
}