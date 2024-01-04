using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public class WiringPlaceOperation : ITileOperation
{
    public WiringPlaceOperation(bool actuator)
    {
        _isActuator = actuator;
    }

    private readonly bool _isActuator;

    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, 32))
            return true;
        
        Item held = player.TPlayer.HeldItem;

        if (_isActuator)
            return held.type != ItemID.Actuator && !player.TPlayer.autoActuator;

		if (held.type == ItemID.Wrench || 
            held.type == ItemID.BlueWrench || 
            held.type == ItemID.GreenWrench || 
            held.type == ItemID.YellowWrench || 
            held.type == ItemID.MulticolorWrench)
            return false;

        return false;
    }
}