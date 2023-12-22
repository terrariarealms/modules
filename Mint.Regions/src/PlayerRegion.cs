using System.Drawing;
using Mint.DataStorages;

namespace Mint.Server.Regions;

public class PlayerRegion : IMemoryObject
{

    internal PlayerRegion()
    {
        EnteredRegions = new List<string>();
    }

    /// <summary>
    /// Current region that player entered.
    /// </summary>
    public List<string> EnteredRegions { get; internal set; }

    internal DateTime warningThreshold;

    internal bool isCreatingRegion;
    internal string? creatingName;
}