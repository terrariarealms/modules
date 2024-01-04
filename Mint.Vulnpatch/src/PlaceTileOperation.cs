using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe class PlaceTileOperation : ITileOperation
{
    public PlaceTileOperation(bool replace)
    {
        _isReplace = replace;
    }

    private readonly bool _isReplace;

    public bool IsValid(Player player, ref short type, ref byte style)
    {
        Item held = player.TPlayer.HeldItem;
        if (!_isReplace && type == TileID.MagicalIceBlock && held.type == ItemID.IceRod)
            return true;

        if (held.createTile != type)
            return false;

        if (!Main.tileFrameImportant[type])
            return true;

        switch (type)
        {
            case 4: // TileID.Torches:
                int heldStyle = held.placeStyle;
                if (player.Character.Stats.ExtraFirst.HasFlag(CharacterExtraFirst.UnlockedBiomeTorches) 
                &&  player.Character.Stats.ExtraFirst.HasFlag(CharacterExtraFirst.BiomeTorches))
                {
                    return style == player.TPlayer.BiomeTorchPlaceStyle(heldStyle);
                }

                return heldStyle == style;

            case 36: // TileID.Presents:
                return style < 7;

            case 129: // TileID.Crystals:
                return style < 18;

            case 141: // TileID.Explosives:
                return style < 2;
        }

        return false;
    }


    public bool HandleTile(Player player, int x, int y, short type, byte style)
    {
        if (!player.InRange(x, y, 32))
            return true;

        if (type < 0 || type >= TileID.Count)
            return true;

        if (Main.tile[x, y].ptr->active() != _isReplace)
            return true;

        if (!IsValid(player, ref type, ref style))
            return true;

        return false;
    }
}