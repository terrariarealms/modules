using System.Security.Cryptography.X509Certificates;
using Mint.Server.Auth;
using Mint.Server.Commands;

namespace Mint.Server.Regions;

public static class RegionCommands
{
#region Information/list
    [StaticCommand("region list", "view all active regions", "[page]")]
    [CommandPermission("mint.regions.list")]
    public static void RegionsList(CommandInvokeContext ctx, int page = 1)
    {
        IEnumerable<string> selected = RegionsModule.RegionsCollection.GetAll()
                .Where((p) => p != null)
                .Select((p) => $"{p?.Name} by {RegionsModuleExtensions.GetByUID(p?.Owner)?.Name ?? "unknown"} [{p?.WorldSpecialized?.ToString() ?? "global"}]");
        List<string> plugins = new List<string>(selected);

        if (plugins.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no one region created.");
            return;
        }

        ctx.Messenger.SendPage("Regions (total: {2}):", plugins, page, null, "Next page: /region list {1}.");
    }

    [StaticCommand("region info", "view information about region", "<region>")]
    [CommandPermission("mint.regions.info")]
    public static void RegionsInfo(CommandInvokeContext ctx, Region region)
    {
        ctx.Messenger.Send(MessageMark.Info, region.Name, "World specialized ID: {0}", region.WorldSpecialized?.ToString() ?? "global");
        ctx.Messenger.Send(MessageMark.Info, region.Name, "XY: {0}:{1}", region.X, region.Y);
        ctx.Messenger.Send(MessageMark.Info, region.Name, "WH: {0}:{1}", region.W, region.H);
        ctx.Messenger.Send(MessageMark.Info, region.Name, "Owner: {0}", RegionsModuleExtensions.GetByUID(region.Owner)?.Name ?? "unknown");
        ctx.Messenger.Send(MessageMark.Info, region.Name, "Enabled protection: {0}", region.Settings.EnabledProtection);
        ctx.Messenger.Send(MessageMark.Info, region.Name, "Enabled readonly: {0}", region.Settings.EnabledReadonly);
        ctx.Messenger.Send(MessageMark.Info, region.Name, "Enabled ghost-mode: {0}", region.Settings.EnabledGhostMode);
        ctx.Messenger.Send(MessageMark.Info, region.Name, "Enabled god-mode: {0}", region.Settings.EnabledGodMode);
        ctx.Messenger.Send(MessageMark.PageFooter, region.Name, "Enter /region enterlist {0} for viewing OnRegionEntered commands", region.Name);
        ctx.Messenger.Send(MessageMark.PageFooter, region.Name, "Enter /region leavelist {0} for viewing OnRegionLeft commands", region.Name);
    }
#endregion
#region Create/delete
    [StaticCommand("region create", "create new region", "<name>")]
    [CommandPermission("mint.regions.creating")]
    public static void RegionsCreate(CommandInvokeContext ctx, string name)
    {
        PlayerRegion rgData = ctx.Sender.GetRegion();
        if (rgData.isCreatingRegion)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "Cannot create new region: you are already in creating mode.");
            return;
        }

        if (RegionsModule.RegionsCollection.Get(name) != null)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "Region with that name already exists!");
            return;
        }

        rgData.creatingName = name;
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Almost done! Select zone for your region via The Grand Design (ID: 3611).");
    }

    [StaticCommand("region create-leave", "leave from creating mode", null)]
    [CommandPermission("mint.regions.creating")]
    public static void RegionsCreateLeave(CommandInvokeContext ctx)
    {
        PlayerRegion rgData = ctx.Sender.GetRegion();
        if (!rgData.isCreatingRegion)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "Cannot leave from creating mode: you are already left.");
            return;
        }

        rgData.isCreatingRegion = false;
        
        ctx.Messenger.Send(MessageMark.OK, "Regions", "You are left from region creating mode.");
    }

    [StaticCommand("region delete", "delete region", "<region>")]
    [CommandPermission("mint.regions.delete")]
    public static void RegionsDelete(CommandInvokeContext ctx, Region region)
    {
        RegionsModule.RegionsCollection.Pop(region.Name);
        RegionsModule.ReloadRegions();

        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region successfully deleted.");
    }
#endregion
#region Members
    [StaticCommand("region memberlist", "view region members", "<region> [page]")]
    [CommandPermission("mint.regions.info")]
    public static void RegionsMemberList(CommandInvokeContext ctx, Region region, int page = 1)
    {
        if (region.Members.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no members.");
            return;
        }

        List<string> list = new List<string>(GetEnumeratedLines(region.Members.Select(p => $"{RegionsModuleExtensions.GetByUID(p)?.Name ?? "unknown"}")));
        ctx.Messenger.SendPage("Region members (total: {2}):", list, page, null, "Next page: /region members <region> {1}.");
    }

    [StaticCommand("region +member", "add member to region", "<region> <account>")]
    [CommandPermission("mint.regions.members")]
    public static void RegionsMemberAdd(CommandInvokeContext ctx, Region region, Account member)
    {
        if (region.Members.Any((p) => p == member.UID))
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "That member is already added.");
            return;
        }

        region.Members.Add(member.UID);
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region -member", "remove member from region", "<region> <account>")]
    [CommandPermission("mint.regions.members")]
    public static void RegionsMemberRemove(CommandInvokeContext ctx, Region region, Account member)
    {
        bool removed = region.Members.Remove(member.UID);
        if (region.Members.Any((p) => p == member.UID))
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "That member is not added in region.");
            return;
        }

        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region --member", "remove all members from region", "<region>")]
    [CommandPermission("mint.regions.members")]
    public static void RegionsMemberRemoveAll(CommandInvokeContext ctx, Region region)
    {
        region.Members.Clear();
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
#endregion
#region Owner
    [StaticCommand("region setowner", "set owner for region", "<region> <account>")]
    [CommandPermission("mint.regions.setowner")]
    public static void RegionsSetOwner(CommandInvokeContext ctx, Region region, Account account)
    {
        region.Owner = account.UID;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
#endregion
#region OnRegionEntered
    [StaticCommand("region enterlist", "view OnRegionEntered commands", "<region> [page]")]
    [CommandPermission("mint.regions.info")]
    public static void RegionsEnterList(CommandInvokeContext ctx, Region region, int page = 1)
    {
        if (region.Settings.OnEnterCommands == null || region.Settings.OnEnterCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        List<string> list = new List<string>(GetEnumeratedLines(region.Settings.OnEnterCommands));
        ctx.Messenger.SendPage("OnRegionEntered commands (total: {2}):", list, page, null, "Next page: /region enterlist <region> {1}.");
    }

    [StaticCommand("region +enter", "add OnRegionEntered command", "<region> <command> [index]")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsEnterAdd(CommandInvokeContext ctx, Region region, string command, int index = 1)
    {
        if (region.Settings.OnEnterCommands == null)
            region.Settings.OnEnterCommands = new List<string>();

        region.Settings.OnEnterCommands.Add(command);
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region -enter", "remove OnRegionEntered command", "<region> <index>")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsEnterRemove(CommandInvokeContext ctx, Region region, int index)
    {
        if (region.Settings.OnEnterCommands == null || region.Settings.OnEnterCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        region.Settings.OnEnterCommands.RemoveAt(index);
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region --enter", "remove all OnRegionEntered commands", "<region>")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsEnterRemoveAll(CommandInvokeContext ctx, Region region)
    {
        if (region.Settings.OnEnterCommands == null || region.Settings.OnEnterCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        region.Settings.OnEnterCommands.Clear();
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

#endregion
#region OnRegionLeft
    [StaticCommand("region leavelist", "view OnRegionLeft commands", "<region> [page]")]
    [CommandPermission("mint.regions.info")]
    public static void RegionsLeaveList(CommandInvokeContext ctx, Region region, int page = 1)
    {
        if (region.Settings.OnLeaveCommands == null || region.Settings.OnLeaveCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        List<string> list = new List<string>(GetEnumeratedLines(region.Settings.OnLeaveCommands));
        ctx.Messenger.SendPage("OnRegionLeft commands (total: {2}):", list, page, null, "Next page: /region leavelist <region> {1}.");
    }

    [StaticCommand("region +leave", "add OnRegionLeft command", "<region> <command> [index]")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsLeaveAdd(CommandInvokeContext ctx, Region region, string command, int index = 1)
    {
        if (region.Settings.OnLeaveCommands == null)
            region.Settings.OnLeaveCommands = new List<string>();

        region.Settings.OnLeaveCommands.Add(command);
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region -leave", "remove OnRegionLeft command", "<region> <index>")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsLeaveRemove(CommandInvokeContext ctx, Region region, int index)
    {
        if (region.Settings.OnLeaveCommands == null || region.Settings.OnLeaveCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        region.Settings.OnLeaveCommands.RemoveAt(index);
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region --leave", "remove all OnRegionLeft commands", "<region>")]
    [CommandPermission("mint.regions.events")]
    public static void RegionsLeaveRemoveAll(CommandInvokeContext ctx, Region region)
    {
        if (region.Settings.OnLeaveCommands == null || region.Settings.OnLeaveCommands.Count == 0)
        {
            ctx.Messenger.Send(MessageMark.Error, "Regions", "There is no event commands.");
            return;
        }

        region.Settings.OnLeaveCommands.Clear();
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
#endregion
#region Settings
    [StaticCommand("region ghostmode", "change auto-ghostmode state for region [Server-Side-Characters]", "<region> <value true/false>")]
    [CommandPermission("mint.regions.settings")]
    public static void RegionsEnabledGhostMode(CommandInvokeContext ctx, Region region, bool state)
    {
        region.Settings.EnabledGhostMode = state;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region protection", "change protection state for region", "<region> <value true/false>")]
    [CommandPermission("mint.regions.settings")]
    public static void RegionsEnabledProtection(CommandInvokeContext ctx, Region region, bool state)
    {
        region.Settings.EnabledProtection = state;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region godmode", "change auto-godmode state for region", "<region> <value true/false>")]
    [CommandPermission("mint.regions.settings")]
    public static void RegionsEnabledGodMode(CommandInvokeContext ctx, Region region, bool state)
    {
        region.Settings.EnabledGodMode = state;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region readonly", "change readonly state for region", "<region> <value true/false>")]
    [CommandPermission("mint.regions.settings")]
    public static void Regions(CommandInvokeContext ctx, Region region, bool state)
    {
        region.Settings.EnabledReadonly = state;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
#endregion
#region Resize
    [StaticCommand("region +x", "resize region by X (offset)", "<region> <X>")]
    [CommandPermission("mint.regions.setowner")]
    public static void RegionsResizeByX(CommandInvokeContext ctx, Region region, int x)
    {
        region.X += x;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
    
    [StaticCommand("region +y", "resize region by X (offset)", "<region> <Y>")]
    [CommandPermission("mint.regions.setowner")]
    public static void RegionsResizeByY(CommandInvokeContext ctx, Region region, int y)
    {
        region.Y += y;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }

    [StaticCommand("region +w", "resize region by Width", "<region> <width>")]
    [CommandPermission("mint.regions.setowner")]
    public static void RegionsResizeByW(CommandInvokeContext ctx, Region region, int w)
    {
        region.W += w;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
    
    [StaticCommand("region +h", "resize region by Height", "<region> <height>")]
    [CommandPermission("mint.regions.setowner")]
    public static void RegionsResizeByH(CommandInvokeContext ctx, Region region, int h)
    {
        region.H += h;
        region.Save();
        ctx.Messenger.Send(MessageMark.OK, "Regions", "Region updated successfully.");
    }
#endregion

    internal static IEnumerable<string> GetEnumeratedLines(IEnumerable<string> commands)
    {
        int index = 0;

        foreach (string command in commands)
        {
            string text = $"[{index}] {command}";
            index++;
            yield return text;
        }
    }
}
