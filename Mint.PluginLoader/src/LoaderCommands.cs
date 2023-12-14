using Mint.Server;
using Mint.Server.Commands;

namespace Mint.PluginLoader;

public static class LoaderCommands
{
    [StaticCommand("plugins list", "view all loaded and unloaded plugins", "[page]")]
    [CommandPermission("mint.plugins.list")]
    public static void PluginsList(CommandInvokeContext ctx, int page = 1)
    {
        if (LoaderModule.Loader == null) 
        {
            ctx.Messenger.Send(MessageMark.Error, "Plugins", "Plugins operation failed: initialization was failed.");
            return;
        }    

        // todo: remove linq

        IEnumerable<string> selected = LoaderModule.Loader._loadedPlugins
                .Where((p) => p != null)
                .Select((p) => $"{p?.Name}: IsLoaded={(p?.IsLoaded == true ? "true" : "false")}");
        List<string> plugins = new List<string>(selected);

        if (plugins.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Plugins", "There is no one plugin installed.");
            return;
        }

        ctx.Messenger.SendPage("Loaded plugins (total: {2}):", plugins, page, null, "Next page: /plugins list {1}.");
    }

    [StaticCommand("plugins unload", "unload all plugins", null)]
    [CommandPermission("mint.plugins.control")]
    [CommandFlags(CommandFlags.RootOnly)]
    public static void PluginsUnload(CommandInvokeContext ctx)
    {
        if (LoaderModule.Loader == null) 
        {
            ctx.Messenger.Send(MessageMark.Error, "Plugins", "Plugins operation failed: initialization was failed.");
            return;
        }    

        LoaderModule.Loader.UnloadPlugins();
        ctx.Messenger.Send(MessageMark.OK, "Plugins", "Successfully unloaded plugins.");
    }

    [StaticCommand("plugins load", "load all plugins", null)]
    [CommandPermission("mint.plugins.control")]
    [CommandFlags(CommandFlags.RootOnly)]
    public static void PluginsLoad(CommandInvokeContext ctx)
    {
        if (LoaderModule.Loader == null) 
        {
            ctx.Messenger.Send(MessageMark.Error, "Plugins", "Plugins operation failed: initialization was failed.");
            return;
        }    

        LoaderModule.Loader.LoadPlugins();
        ctx.Messenger.Send(MessageMark.OK, "Plugins", "Successfully loaded plugins.");
    }

    [StaticCommand("plugins reload", "reload all plugins", null)]
    [CommandPermission("mint.plugins.control")]
    [CommandFlags(CommandFlags.RootOnly)]
    public static void PluginsReload(CommandInvokeContext ctx)
    {
        if (LoaderModule.Loader == null) 
        {
            ctx.Messenger.Send(MessageMark.Error, "Plugins", "Plugins operation failed: initialization was failed.");
            return;
        }    

        LoaderModule.Loader.UnloadPlugins();
        LoaderModule.Loader.LoadPlugins();
        ctx.Messenger.Send(MessageMark.OK, "Plugins", "Successfully reloaded plugins.");
    }
}