using Mint.Server;
using Mint.Server.Regions;
using Mint.Server.Auth;
using Mint.Core;
using Microsoft.Xna.Framework;

public static class RegionsModuleExtensions
{
    /// <summary>
    /// Check for build permission in target point.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Is player have building permission here</returns>
    public static bool CanBuildIn(this Player player, int x, int y)
    {   
        if (player.Account == null) return false;

        return RegionsModule.LoadedRegions.Find((p) => p.Zone.Contains(x, y) && !p.CanBuildHere(player.Account)) == null;
    }
    
    /// <summary>
    /// Check for build permission in target rectangle.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Is player have building permission here</returns>
    public static bool CanBuildIn(this Player player, Rectangle rectangle)
    {   
        if (player.Account == null) return false;

        return RegionsModule.LoadedRegions.Find((p) => p.Zone.Intersects(rectangle) && !p.CanBuildHere(player.Account)) == null;
    }

    /// <summary>
    /// Get player account by UID.
    /// </summary>
    /// <param name="uid">UID</param>
    /// <returns>Account</returns>
    public static Account? GetByUID(string? uid)
    {
        if (uid == null) return null;

        Account? account = MintServer.AccountsCollection.GetBy(p => p.UID == uid);
        return account;
    }

    /// <summary>
    /// Add/update player region data.
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="data">Region data</param>
    public static void PushRegion(this Player player, PlayerRegion data)
    {
        player.Storage.Push("region", data);
    }

    /// <summary>
    /// Get player region data.
    /// </summary>
    /// <param name="player">Player</param>
    /// <returns>Region data</returns>
    public static PlayerRegion GetRegion(this Player player)
    {
        PlayerRegion? region = player.Storage.Get("region") as PlayerRegion;
        if (region == null)
        {
            PlayerRegion newRegion = new PlayerRegion();
            player.Storage.Push("region", newRegion);
            return newRegion;
        }

        return region;
    }
}