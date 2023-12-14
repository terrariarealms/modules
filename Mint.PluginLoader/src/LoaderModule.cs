using Mint.Assemblies.Modules;
using Mint.Core;
using Mint.Server.Commands;
using Serilog;

namespace Mint.PluginLoader;

public class LoaderModule : MintModule
{
    public const string WorkingDirectory = "plugins";

    internal static PluginLoader? Loader;

#region Template
    public override string ModuleName => "mint_pluginloader";
    public override string ModuleVersion => "1.0";
    public override string[]? ModuleReferences => null;
    public override int ModuleArchitecture => 1;
    public override int Priority => 255;
#endregion

    public override void Setup()
    {
        Log.Information("LoaderModule -> Setup()");

        if (!Directory.Exists(WorkingDirectory))
            Directory.CreateDirectory(WorkingDirectory);
        
        Loader = new PluginLoader();
    }

    public override void Initialize()
    {
        Log.Information("LoaderModule -> Initialize()");

        CommandSection section = MintServer.Commands.CreateSection("mint_module_pluginloader", 4);
        section.ImportFrom(typeof(LoaderCommands));

        Loader?.LoadPlugins();
    }
}
