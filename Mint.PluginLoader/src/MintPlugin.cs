namespace Mint.PluginLoader;

public abstract class MintPlugin
{
    public abstract string Name { get; }

    public abstract void Load();
    public abstract void Unload();
}