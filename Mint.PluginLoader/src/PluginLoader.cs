using System.Reflection;
using System.Runtime.Loader;
using Mint.Server;
using Serilog;

namespace Mint.PluginLoader;

internal class PluginLoader
{
    internal PluginLoader()
    {
        //_asmLoadContext = new AssemblyLoadContext("plugin_ldr", true);
        _loadedPlugins = Array.Empty<PluginContainer?>();
    }

    //internal AssemblyLoadContext _asmLoadContext;
    internal PluginContainer?[] _loadedPlugins;

    internal void LoadPlugins()
    {
        List<PluginContainer> assemblies = new List<PluginContainer>(LoadContainers()); 
        _loadedPlugins = new PluginContainer?[assemblies.Count];
        assemblies.AsParallel().ForAll(LoadFrom);
    }

    internal void UnloadPlugins()
    {
        for (int i = 0; i < _loadedPlugins.Length; i++)
            UnloadFrom(_loadedPlugins[i]);

        _loadedPlugins = Array.Empty<PluginContainer?>();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        //_asmLoadContext.Unload();
    }

    private void LoadFrom(PluginContainer container)
    {
        Log.Information("PluginLoader: Loading {Name}", container.Name);
        container.LoadPlugin();
        Log.Information("PluginLoader: Loaded {Name}", container.Name);
        _loadedPlugins[container.PluginIndex] = container;
    }

    private void UnloadFrom(PluginContainer? container)
    {
        if (container == null) return;

        Log.Information("PluginLoader: Unloading {Name}", container.Name);
        container.UnloadPlugin();
        Log.Information("PluginLoader: Unloaded {Name}", container.Name);
        _loadedPlugins[container.PluginIndex] = null;
    }

    private IEnumerable<PluginContainer> LoadContainers()
    {
        /*
        if (_asmLoadContext.Assemblies.Any())
            _asmLoadContext.Unload();
        */

        int pluginId = 0;
        foreach (string file in Directory.EnumerateFiles(LoaderModule.WorkingDirectory, "*.dll"))
        {
            PluginContainer? container = null;

            try
            {
                //container = new PluginContainer(_asmLoadContext.LoadFromAssemblyPath(Path.Combine(Environment.CurrentDirectory, file)), pluginId);
                
                container = new PluginContainer(Assembly.Load(File.ReadAllBytes(file)), pluginId);
                pluginId++;
            }
            catch (BadImageFormatException ex)
            {
                Log.Error("PluginLoader: Exception in loading .NET (CIL) assembly from {File}:", file);
                Log.Error("PluginLoader: If you are not TerraZ Fan (gay) then keep only .NET assemblies in directory {PluginsPath}.", LoaderModule.WorkingDirectory);
                Log.Error("PluginLoader: {Exception}", ex.ToString());
            }

            if (container != null)
                yield return container;
        }
    }
}