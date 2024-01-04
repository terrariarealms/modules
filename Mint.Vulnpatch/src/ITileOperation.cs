using Mint.Server;

namespace Mint.Vulnpatch;

public interface ITileOperation
{
    public bool HandleTile(Player player, int x, int y, short type, byte style);
}