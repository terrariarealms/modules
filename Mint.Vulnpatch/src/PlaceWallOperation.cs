using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe class PlaceWallOperation : ITileOperation
{
    public PlaceWallOperation(bool replace)
    {
        _isReplace = replace;
    }

    private readonly bool _isReplace;

    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, 32))
            return true;

        if (Main.tile[x, y].ptr->wall > 0 != _isReplace)
            return true;
        
        Item held = player.TPlayer.HeldItem;
        if (held.createWall != type)
            return true;

        return false;
    }
}