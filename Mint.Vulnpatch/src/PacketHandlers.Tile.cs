using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    static void InitializeTiles()
    {
        TileOperations[TileOperationID.KillTile] = new KillTileOperation();
        TileOperations[TileOperationID.KillWall] = new KillWallOperation();
        TileOperations[TileOperationID.PlaceTile] = new PlaceTileOperation(false);
        TileOperations[TileOperationID.ReplaceTile] = new PlaceTileOperation(true);
        TileOperations[TileOperationID.PlaceWall] = new PlaceWallOperation(false);
        TileOperations[TileOperationID.ReplaceWall] = new PlaceWallOperation(true);
        TileOperations[TileOperationID.PlaceWire] = new WiringPlaceOperation(false);
        TileOperations[TileOperationID.PlaceWire2] = new WiringPlaceOperation(false);
        TileOperations[TileOperationID.PlaceWire3] = new WiringPlaceOperation(false);
        TileOperations[TileOperationID.PlaceWire4] = new WiringPlaceOperation(false);
        TileOperations[TileOperationID.PlaceActuator] = new WiringPlaceOperation(true);
        TileOperations[TileOperationID.KillWire] = new WiringKillOperation();
        TileOperations[TileOperationID.KillWire2] = new WiringKillOperation();
        TileOperations[TileOperationID.KillWire3] = new WiringKillOperation();
        TileOperations[TileOperationID.KillWire4] = new WiringKillOperation();
        TileOperations[TileOperationID.KillActuator] = new WiringKillOperation();
    }

    static ITileOperation?[] TileOperations = new ITileOperation[24];

    private static void OnTileInteraction(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        var reader = packet.GetReader();

        byte operation = reader.ReadByte();
        if (operation >= TileOperations.Length)
        {
            player.CloseConnection();
            ignore = true;
            return;
        }

        int x = reader.ReadInt16();
        int y = reader.ReadInt16();

        if (!TileExtensions.InWorld(x, y))
        {
            ignore = true;
            return;
        }

        short type = reader.ReadInt16();
        byte style = reader.ReadByte();

        ignore = TileOperations[operation]?.HandleTile(player, x, y, type, style) == true;
    }
}