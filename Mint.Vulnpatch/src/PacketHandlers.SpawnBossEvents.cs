using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    private static void OnAnyBossPacket(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        if (VulnpatchModule.Config.DisableBossAndEventsSpawn)
        {
            ignore = true;
            return;
        }
    }
}