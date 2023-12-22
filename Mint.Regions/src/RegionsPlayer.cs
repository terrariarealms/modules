namespace Mint.Server.Regions;

public class RegionsPlayer : DynamicPlayer
{
    public RegionsPlayer() : base("regions", new Auth.Account("regions", Guid.NewGuid().ToString(), "root", null, null, new Dictionary<string, string>()), new RegionsMessenger())
    {

    }
}