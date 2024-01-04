using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    private static void OnCatchNPC(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        if (VulnpatchModule.Config.DisableNPCRelease)
        {
            ignore = true;
            return;
        }

        var reader = packet.GetReader();

        short npcId = reader.ReadInt16();

        if (npcId < 0 || npcId >= 200)
        {
            ignore = true;
            return;
        }

        NPC npc = Main.npc[npcId];
        if (!npc.active || npc.catchItem <= 0)
        {
            ignore = true;
            return;
        }
    }
}