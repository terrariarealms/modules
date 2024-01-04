using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public class WiringKillOperation : ITileOperation
{
    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, 32))
            return true;
        
        Item held = player.TPlayer.HeldItem;

		if (held.type == ItemID.WireCutter || 
            held.type == ItemID.MulticolorWrench)
            return false;

        return false;
    }
}