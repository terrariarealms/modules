using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;

namespace Mint.Vulnpatch;

// DoBootsEffect_PlaceFlowersOnTile
// tshock based
internal static partial class PacketHandlers
{
    const int FRAME_NONE = -1;
    const int DEFAULT_FRAME = 18;
    const int DISTANCE = 32 * 16;

    // rectangle packet matches with X = 1
    readonly static IMatch[] MatchesX1 = new IMatch[]
    {
        new FlowersMatch(),
        new ConversionMatch(),
        new ChristmasTreeMatch(),
        new GrassMatch(),

        new RemoveMatch(2, TileID.Firework),
        new RemoveMatch(1, TileID.LandMine),

        new FrameMatch(1, TileID.Traps, 90, 90, 18, 18),

        Place(1, TileID.LogicSensor, 108, 0),
        Place(1, TileID.FoodPlatter, 18, 0),
        
        Candle(TileID.PeaceCandle),
        Candle(TileID.Candles),
        Candle(TileID.WaterCandle),
        Candle(TileID.ShadowCandle),
        Candle(TileID.PlatinumCandle),

        FrameX(1, TileID.WirePipe, 36, 18),
        FrameX(1, TileID.ProjectilePressurePad, 66, 22),
        FrameX(1, TileID.Plants, 792, 18),
        FrameX(1, TileID.MinecartTrack, 36, 1),
    };

    // rectangle packet matches with X = 2
    readonly static IMatch[] MatchesX2 = new IMatch[]
    {
        Place(3, TileID.HatRack, 9, 54),
        Place(3, TileID.DisplayDoll, 126, 36),
        Place(2, TileID.ItemFrame, 162, 18),
        Place(3, TileID.TargetDummy, 54, 36),

        FrameY(4, TileID.WaterFountain, 126, 18),
        FrameY(3, TileID.ShimmerMonolith, 144, 18),
        FrameY(2, TileID.ArrowSign, 270, 18),
        FrameY(3, TileID.VoidMonolith, 90, 18),
        FrameY(2, TileID.PaintedArrowSign, 270, 18),
        FrameY(2, TileID.MusicBoxes, 54, 18),
        FrameY(3, TileID.LunarMonolith, 92, 18),
        FrameY(3, TileID.BloodMoonMonolith, 90, 18),
        FrameY(3, TileID.EchoMonolith, 90, 18),
    };

    // rectangle packet matches with X = 3
    readonly static IMatch[] MatchesX3 = new IMatch[]
    {
        new PlaceMatch(4, TileID.TeleportationPylon, 468, 54, 18, 18),
        new PlaceMatch(3, TileID.WeaponsRack2, 90, 36, 18, 18),

        FrameY(2, TileID.Campfire, 54, 18),
    };

    // rectangle packet match with X = 4: cannon only
    readonly static IMatch MatchCannonX4 = FrameY(3, TileID.Cannon, 468, 18);

    static PlaceMatch Place(int height, int tileId, int maxX, int maxY) => new PlaceMatch(height, tileId, maxX, maxY, DEFAULT_FRAME, DEFAULT_FRAME);
    static FrameMatch Candle(int tileId) => new FrameMatch(1, tileId, 18, FRAME_NONE, 18, FRAME_NONE);
    static FrameMatch FrameX(int height, int tileId, int max, int size) => new FrameMatch(height, tileId, max, FRAME_NONE, size, FRAME_NONE);
    static FrameMatch FrameY(int height, int tileId, int max, int size) => new FrameMatch(height, tileId, FRAME_NONE, max, FRAME_NONE, size);

    private static void OnTileRectangle(Server.Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        var reader = packet.GetReader();

        short x = reader.ReadInt16();
        short y = reader.ReadInt16();

        if (player.TPlayer.position.Distance(new Vector2(x, y)) < DISTANCE)
        {
            ignore = true;
            return;
        }

        byte w = reader.ReadByte();
        if (w > 4)
        {
            player.CloseConnection();
            ignore = true;
            return;
        }
        byte h = reader.ReadByte();
        if (h > 4)
        {
            player.CloseConnection();
            ignore = true;
            return;
        }

        if (VulnpatchModule.Config.DisableTileRectPacket)
        {
            player.SendTileRectangle(x, y, w, h);
            ignore = true;
            return;
        }

        Rectangle rectangle = new Rectangle(x, y, w, h);

        if (!TileExtensions.InWorld(rectangle))
        {
            ignore = true;
            return;
        }
    
        NetTile[,] tiles = new NetTile[w, h];

        for (int i = 0; i < w; i++)
        for (int j = 0; j < h; j++)
            tiles[i, j] = new NetTile(reader);

        ignore = true;

        switch (w)
        {
            case 1: HandleMatches(MatchesX1, tiles, rectangle); break;
            case 2: HandleMatches(MatchesX2, tiles, rectangle); break;
            case 3: HandleMatches(MatchesX3, tiles, rectangle); break;
            
            case 4: 
                if (MatchCannonX4.IsMatches(rectangle, tiles))
                NetMessage.SendTileSquare(-1, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                break;
        }
    }

    static void HandleMatches(IMatch[] matches, NetTile[,] tiles, Rectangle rectangle)
    {
        foreach (IMatch match in matches)
        {
            if (match.IsMatches(rectangle, tiles))
            {
                NetMessage.SendTileSquare(-1, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                return;
            }
        }
    }
}