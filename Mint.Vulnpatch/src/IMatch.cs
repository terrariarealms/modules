using Microsoft.Xna.Framework;

namespace Mint.Vulnpatch;

public interface IMatch
{
    public bool IsMatches(Rectangle rectangle, NetTile[,] tiles);
}