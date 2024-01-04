using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe class KillWallOperation : ITileOperation
{
    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, 32))
            return true;

        // this was added because i dont know how that shit can secure from mass tile edit
        if (VulnpatchModule.Config.DisableTileSecure)
            return false;

        Item held = player.TPlayer.HeldItem;

        TileData* ptr = Main.tile[x, y].ptr;

        if (held.hammer > 0 && ptr->wall > 0)
            return false;

        return true;
    }
}