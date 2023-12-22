using Microsoft.Xna.Framework;
using Mint.DataStorages;
using Mint.Server.Auth;
using MongoDB.Bson.Serialization.Attributes;
using Terraria;

namespace Mint.Server.Regions;

[BsonIgnoreExtraElements]
public class Region : DatabaseObject
{
    public Region(string name, int? worldSpecialized, int x, int y, int w, int h, string owner, List<string> members, RegionSettings settings) : base(name)
    {
        WorldSpecialized = worldSpecialized;
        X = x;
        Y = y;
        W = w;
        H = h;
        Owner = owner;
        Members = members;
        Settings = settings;
    }

    public bool CanBuildHere(Account account)
    {
        if (WorldSpecialized != null && Main.worldID != WorldSpecialized)
            return true;

        if (Settings.EnabledReadonly) 
            return false;

        if (!Settings.EnabledProtection) 
            return true;

        if (account.GetGroup()?.HasPermission("mint.regions.buildanywhere") == true)
            return true;

        if (Owner == account.UID)
            return true;

        if (Members.Any(p => p == account.UID))
            return true;

        return false;
    }

    public void Save()
    {
        RegionsModule.RegionsCollection.Push(this.Name, this);
        RegionsModule.ReloadRegions();
    }

    public int? WorldSpecialized;

    [BsonIgnore]
    public Rectangle Zone => new Rectangle(X, Y, W, H);

    public int X;
    public int Y;
    public int W;
    public int H;

    public string Owner;
    public List<string> Members;
    public RegionSettings Settings;
}