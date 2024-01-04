using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe class KillTileOperation : ITileOperation
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

        if (ptr->type == TileID.MagicalIceBlock || ptr->type == TileID.MysticSnakeRope || ptr->type == TileID.ItemFrame)
            return false;

        switch (player.TPlayer.mount._type)
        {
            case MountID.DiggingMoleMinecart:
                if (!player.InRange(x, y, 8))
                    break;

                if (Main.tileSand[ptr->type])
                    break;

                return false;

            case MountID.Drill:
                return false;
        }

        if (TileID.Sets.CrackedBricks[ptr->type] || Main.tileCut[ptr->type])
            return false;

        if (held.type == ItemID.GravediggerShovel && TileID.Sets.CanBeDugByShovel[ptr->type])
            return false;

        bool hammerTile = Main.tileHammer[ptr->type];
        bool axeTile = Main.tileAxe[ptr->type];

        if (held.axe > 0 && axeTile)
            return false;

        if (held.hammer > 0 && hammerTile)
            return false;

        if (held.pick > 0 && !hammerTile && !axeTile)
            return false;

        return true;
    }
}