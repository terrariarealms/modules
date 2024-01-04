using Microsoft.Xna.Framework;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    internal static HashSet<short> FishableNPC = new HashSet<short>()
    {
        // town
        NPCID.TownSlimeRed,

        // bloodmoon
        NPCID.GoblinShark,
        NPCID.EyeballFlyingFish,
        NPCID.ZombieMerman,
        NPCID.BloodEelHead,
        NPCID.BloodEelBody,
        NPCID.BloodEelTail,
        NPCID.BloodNautilus
    };
    
    private static void OnFishOutNPC(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (ignore) return;

        if (VulnpatchModule.Config.DisableNPCFishing)
        {
            ignore = true;
            return;
        }

        var reader = packet.GetReader();

        ushort x = reader.ReadUInt16();
        ushort y = reader.ReadUInt16();
        short npc = reader.ReadInt16();

        if (!player.InRange(x, y, 64))
        {

            return;
        }

        if (FishableNPC.Contains(npc))
        {
            if (player.Group?.HasPermission("mint.game.fishing.fishoutnpc") == false)
            {
                player.Messenger.Send(MessageMark.Error, "System", "You do not have permissions to perform this action.");
                ignore = true;
                return;
            }
        }

        if (npc == NPCID.DukeFishron)
        {
            if (player.Group?.HasPermission("mint.game.fishing.spawnboss") == false)
            {
                player.Messenger.Send(MessageMark.Error, "System", "You do not have permissions to perform this action.");
                ignore = true;
                return;
            }
        }
    }

    
    static bool IsFishingRod(Item item)
    {
        return item.fishingPole > 0;
    }
}