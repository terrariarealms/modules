using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    private static void OnReleaseNPC(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        if (VulnpatchModule.Config.DisableNPCRelease)
        {
            ignore = true;
            return;
        }

        var reader = packet.GetReader();

        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        short npc = reader.ReadInt16();

        if (player.TPlayer.HeldItem.makeNPC != npc)
        {
            ignore = true;
        }
    }
}