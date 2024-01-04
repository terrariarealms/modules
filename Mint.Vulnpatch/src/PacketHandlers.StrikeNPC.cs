using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    private static void OnStrikeNPC(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        var reader = packet.GetReader();
        short id = reader.ReadInt16();
        if (id < 0 || id >= 200)
        {
            ignore = true;
            return;
        }

        NPC npc = Main.npc[id];

        if ((npc.type == NPCID.EmpressButterfly || npc.type == NPCID.CultistDevote || npc.type == NPCID.CultistArcherBlue)
            && VulnpatchModule.Config.DisableBossAndEventsSpawn)
        {
            ignore = true;
            return;
        }
    }
}