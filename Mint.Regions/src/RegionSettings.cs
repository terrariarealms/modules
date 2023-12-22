namespace Mint.Server.Regions;

public struct RegionSettings
{
    /// <summary>
    /// Means that only members can build in that region.
    /// </summary>
    public bool EnabledProtection;

    /// <summary>
    /// Means that no one can build in that region.
    /// </summary>
    public bool EnabledReadonly;

    /// <summary>
    /// Means that everyone having godmode enabled when entered in that region.
    /// </summary>
    public bool EnabledGodMode;

    /// <summary>
    /// Means that everyone having ghost mode enabled when entered in that region.
    /// </summary>
    public bool EnabledGhostMode;

    /// <summary>
    /// List of commands that will be invoked when someone entering in that region.
    /// </summary>
    public List<string>? OnEnterCommands;

    /// <summary>
    /// List of commands that will be invoked when someone leaving from that region.
    /// </summary>
    public List<string>? OnLeaveCommands;
}