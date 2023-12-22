using Microsoft.Xna.Framework;
using Mint.Core;
using Mint.Network;
using Mint.Network.Incoming;
using Terraria;

namespace Mint.Server.Regions;

internal static partial class TileHandlers
{
    internal static void Initialize()
    {
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WiringMassTileOperation, OnWiringMassTileOperation);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldTileRectangle, OnTileRectangle);  

        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldPlaceTileV2, BasicCheck);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.ChestPlaceOrDestroy, BasicCheck);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.ChestAndDoorUnlock, BasicCheck);   
        MintServer.Network.IncomingPackets.Add(31, BasicCheck); // chest open
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldSetLiquidOutdated, BasicCheck);   

        MintServer.Network.IncomingPackets.Add(133, BasicCheck); // food platter
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldTileEntityPlace, BasicCheck);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldTileEntityPlaceItemFrame, BasicCheck);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldTileEntityPlaceItemRack, BasicCheck);   

        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldPaintTile, BasicCheck);   
        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldPaintWall, BasicCheck);   

        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.WorldTileInteraction,
        (Player plr, IncomingPacket packet, ref bool ignore) => BasicCheckOffset(plr, packet, ref ignore, sizeof(byte)));    

        MintServer.Network.IncomingPackets.Add((byte)IncomingPacketID.SignSetText,
        (Player plr, IncomingPacket packet, ref bool ignore) => BasicCheckOffset(plr, packet, ref ignore, sizeof(short)));   
    }

    static void BasicCheck(Player plr, IncomingPacket packet, ref bool ignore) 
    {
        var reader = packet.GetReader();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();

        if (TileUtils.HandleTilePoint(plr, new(x, y)))
            ignore = true;
    }

    static void BasicCheckOffset(Player plr, IncomingPacket packet, ref bool ignore, int offset) 
    {
        var reader = packet.GetReader();
        reader.BaseStream.Position += offset;
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();

        if (TileUtils.HandleTilePoint(plr, new(x, y)))
            ignore = true;
    }
    
    static void OnWiringMassTileOperation(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (player.Account == null) return;

        var reader = packet.GetReader();
        Point point1 = new Point(reader.ReadInt16(), reader.ReadInt16());
        Point point2 = new Point(reader.ReadInt16(), reader.ReadInt16());

        Rectangle zone = TileUtils.ToRectangle(point1, point2);
        PlayerRegion rgData = player.GetRegion();
        if (rgData.creatingName != null)
        {
            Region region = new Region(rgData.creatingName, Main.worldID, zone.X, zone.Y, zone.Width, zone.Height, player.Account.UID, new List<string>(), default);
            rgData.creatingName = null;
            RegionsModule.RegionsCollection.Push(region.Name, region);
            RegionsModule.ReloadRegions();

            player.Messenger.Send(MessageMark.OK, "Regions", "Region succesfully created!");
        }

        if (!TileUtils.HandleTileRectangle(player, zone))
            ignore = true;
    }

    static void OnTileRectangle(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (player.Account == null || ignore) return;

        var reader = packet.GetReader();

        short x = reader.ReadInt16();
        short y = reader.ReadInt16();
        byte w = reader.ReadByte();
        if (w > 4)
        {
            ignore = true;
            return;
        }
        byte h = reader.ReadByte();
        if (h > 4)
        {
            ignore = true;
            return;
        }

        Rectangle zone = new Rectangle(x, y, w, h);
        if (!TileUtils.HandleTileRectangle(player, zone))
            ignore = true;
    }
}