using Microsoft.Xna.Framework;
using Mint.Core;
using Mint.Network;
using Mint.Network.Incoming;
using Mint.Server;
using Terraria;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

internal static partial class PacketHandlers
{
    internal static readonly IncomingPacketID[] TilePackets = new IncomingPacketID[]
    {
        IncomingPacketID.WorldTileEntityDisplayDollItem,
        IncomingPacketID.WorldTileAnimation,
        IncomingPacketID.WorldTileEntityDisplayDollItem,
        IncomingPacketID.WorldTileEntityFoodPlatterPlace,
        IncomingPacketID.WorldTileEntityHatRackItem,
        IncomingPacketID.WorldTileEntityInteractionAchor,
        IncomingPacketID.WorldTileEntityPlace,
        IncomingPacketID.WorldTileEntityPlaceItemFrame,
        IncomingPacketID.WorldTileEntityPlaceItemRack,
        IncomingPacketID.WorldTileInteraction,
        IncomingPacketID.WorldTileRectangle,
        IncomingPacketID.WorldTileToggleGemLock,
        IncomingPacketID.WiringMassTileOperation,
        IncomingPacketID.WorldPaintTile,
        IncomingPacketID.WorldPlaceTileV2,
        IncomingPacketID.PlayerSyncPickTile
    };

    // packets that dead player will not send
    internal static readonly IncomingPacketID[] AlivePlayerPackets = new IncomingPacketID[]
    {
        IncomingPacketID.PlayerSendHurt,
        IncomingPacketID.PlayerAndNpcTeleport,
        IncomingPacketID.PlayerDodge,
        IncomingPacketID.PlayerCreateSound,
        IncomingPacketID.PlayerEmoteBubble,
        IncomingPacketID.PlayerFinishedAnglerQuest,
        IncomingPacketID.PlayerGolferAndAnglerData,
        IncomingPacketID.PlayerHealLifeEffect,
        IncomingPacketID.PlayerHealManaEffect,
        IncomingPacketID.PlayerSetTalkNpc,
        IncomingPacketID.PlayerHealStatLife,
        IncomingPacketID.PlayerItemAnimation,
        IncomingPacketID.PlayerLucyMessage,
        IncomingPacketID.PlayerNebulaLevelUp,
        IncomingPacketID.PlayerPvPStatus,
        IncomingPacketID.PlayerSetTalkNpc,
        IncomingPacketID.PlayerSetTeam,
        IncomingPacketID.PlayerTeleportViaItem,
        IncomingPacketID.PlayerTeleportToPortal,
        IncomingPacketID.PlayerSetMinionTargetNPC,
        IncomingPacketID.PlayerSetMinionTargetPoint,

        IncomingPacketID.ChestAndDoorUnlock,
        IncomingPacketID.ChestGetNameIn,
        IncomingPacketID.ChestName,
        IncomingPacketID.ChestPlaceItem,
        IncomingPacketID.ChestPlaceOrDestroy,
        IncomingPacketID.ChestSetItem,
        IncomingPacketID.ChestUseTrap,

        IncomingPacketID.NpcAddBuff,
        IncomingPacketID.NpcCatch,
        IncomingPacketID.NpcCavernMonsterType,
        IncomingPacketID.NpcDryadStardewAnimation,
        IncomingPacketID.NpcExtraValue,
        IncomingPacketID.NpcRelease,
        IncomingPacketID.NpcRequestBuffRemoval,
        IncomingPacketID.NpcSetName,
        IncomingPacketID.NpcSpawnFromFishing,
        IncomingPacketID.NpcStrike,
        IncomingPacketID.NpcTownSetHome,
        IncomingPacketID.NpcTransformSlime,

        IncomingPacketID.ProjectileStrikeNpc,
        IncomingPacketID.ProjectileUpdate,

        IncomingPacketID.SignRead,
        IncomingPacketID.SignSetText,

        IncomingPacketID.ItemDropV1,
        IncomingPacketID.ItemDropV2,
        IncomingPacketID.ItemDropCannotBeTakenByMonsters,
        IncomingPacketID.ItemDropModifiedItem,
        IncomingPacketID.ItemDropShimmered,
        IncomingPacketID.ItemOwner,

        IncomingPacketID.EventsBossesNpcsV1,
        IncomingPacketID.EventsBossesNpcsV2,
        IncomingPacketID.EventsDD2SkipWaitTime,
        IncomingPacketID.EventsDD2SummonCrystal,
        IncomingPacketID.EventsReportProgress,
        IncomingPacketID.EventsToggleParty
    };

    internal static void Initialize()
    {
        InitializeTiles();

        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.WorldTileRectangle] = OnTileRectangle;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.WorldTileInteraction] = OnTileInteraction;  
        
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.NpcSpawnFromFishing] = OnFishOutNPC;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.NpcRelease] = OnReleaseNPC;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.NpcCatch] = OnCatchNPC;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.NpcStrike] = OnStrikeNPC;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.NpcCavernMonsterType] = OnDisabledPacket;  

        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.EventsBossesNpcsV1] = OnAnyBossPacket;  
        MintServer.Network.IncomingHijack[(byte)IncomingPacketID.EventsBossesNpcsV2] = OnAnyBossPacket;  

        NetworkBindDelegate<IncomingPacket> tileDelegate = VulnpatchModule.Config.DisableTilePackets ? OnDisabledPacket : OnAliveOnlyPacket;
        
        if (VulnpatchModule.Config.DisableTilePackets)
            foreach (IncomingPacketID packetId in TilePackets)
                MintServer.Network.IncomingPackets.Add((byte)packetId, tileDelegate);  

        foreach (IncomingPacketID packetId in AlivePlayerPackets)
                MintServer.Network.IncomingPackets.Add((byte)packetId, OnAliveOnlyPacket);  
    }

    static bool InWorld(Rectangle rectangle)
    {
        return rectangle.X >= 0 && rectangle.Y >= 0 && 
                rectangle.X < Main.maxTilesX && rectangle.Y < Main.maxTilesY &&
                rectangle.X + rectangle.Width < Main.maxTilesX && rectangle.Height + rectangle.Height < Main.maxTilesY;
    }

    static bool InWorld(int x, int y)
    {
        return x >= 0 && y >= 0 && 
                x < Main.maxTilesX && y < Main.maxTilesY;
    }

    private static void OnDisabledPacket(Player player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
    }

    private static void OnAliveOnlyPacket(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (!player.TPlayer.dead)
            ignore = true;
    }
}