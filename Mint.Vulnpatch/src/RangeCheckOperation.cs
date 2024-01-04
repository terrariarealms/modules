using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe class RangeCheckOperation : ITileOperation
{
    public RangeCheckOperation(int radius)
    {
        _radius = radius;
    }

    private readonly int _radius;

    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, _radius))
            return true;

        return false;
    }
}