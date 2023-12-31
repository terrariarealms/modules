using System.Reflection;
using System.Reflection.Metadata;
using Serilog;

namespace Mint.PluginLoader;

public class PluginContainer
{
    internal PluginContainer(Assembly assembly, int index)
    {
        _assembly = assembly;
    }

    private Assembly _assembly;
    private MintPlugin? _instance;
    private bool _isLoaded;
    private int _pluginIndex;

    public Assembly PluginAssembly => _assembly;
    public MintPlugin? Instance => _instance;
    public string Name => _assembly.GetName().Name ?? "unknown";
    public bool IsLoaded => _isLoaded;
    public int PluginIndex => _pluginIndex;


    internal MintPlugin? CreateInstance()
    {
        foreach (Type type in _assembly.GetTypes())
        {
            if (type.BaseType?.FullName != typeof(MintPlugin).FullName)
                continue;

            object? instance = Activator.CreateInstance(type);
            if (instance == null)
                continue;

            return instance as MintPlugin; 
        }

        return null;
    }
    internal void DestroyInstance()
    {
        _instance = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    internal void LoadPlugin()
    {
        if (_instance == null)
            _instance = CreateInstance();

        _instance?.Load();
        _isLoaded = true;
    }

    internal void UnloadPlugin()
    {
        _instance?.Unload();
        DestroyInstance();

        _isLoaded = false;
    }
}