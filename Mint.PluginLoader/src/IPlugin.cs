namespace Mint.PluginLoader;

public interface IPlugin
{
    public string Name { get; }

    public void Load();
    public void Unload();
}