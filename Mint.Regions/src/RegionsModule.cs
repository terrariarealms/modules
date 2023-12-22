using Microsoft.Xna.Framework;
using Mint.Assemblies.Modules;
using Mint.Core;
using Mint.DataStorages;
using Mint.Localization;
using Mint.Server.Commands;
using Serilog;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;

namespace Mint.Server.Regions;

public class RegionsModule : MintModule
{
    /// <summary>
    /// Fires when someone entering region.
    /// </summary>
    public static event EventPlayerRegionDelegate? OnRegionEntered;

    /// <summary>
    /// Fires when someone leaving region.
    /// </summary>
    public static event EventPlayerRegionDelegate? OnRegionLeft;

    public override string ModuleName => "mint_regions";

    public override string ModuleVersion => "1.0";

    public override string[]? ModuleReferences => null;

    public override int ModuleArchitecture => 1;

    public override int Priority => 0;

    public static DatabaseStorage<Region> RegionsCollection { get; } = MongoDatabase.Get<Region>();
    public static List<Region> LoadedRegions = new List<Region>(0); 
    public static RegionsPlayer RegionsPlayer = new RegionsPlayer();

    public static void ReloadRegions() => LoadedRegions = RegionsCollection.GetAll();

    public override void Setup()
    {
        LocalizationContainer russianLang = new LocalizationContainer();
        russianLang.ImportFrom(File.ReadAllText("modules/mint_regions_ru.json"), false, true);
        MintServer.Localization.AddContainer(LanguageID.Russian, russianLang);
        
        ReloadRegions();

        MintServer.Commands.AddCustomParser(typeof(Region), (text) =>
        {
            Region? region = RegionsCollection.Get(text);
            if (region == null) return new InvalidParameterValue();

            return region;
        });
    }
    
    private Task? commandHandlerTask;

    public override void Initialize()
    {
        TileHandlers.Initialize();

        CommandSection commandSection = MintServer.Commands.CreateSection("mint_regions", 26);
        commandSection.ImportFrom(typeof(RegionCommands));

        Hooks.MainHooks.OnGamePreUpdate += OnUpdate;

        commandHandlerTask = new Task(CommandHandler);
        commandHandlerTask.Start();
    }

    private void CommandHandler()
    {
        while (true)
        {
            string command = RegionsPlayer.CommandsQueue.Take();
            MintServer.Commands.InvokeCommand(RegionsPlayer, command, true);
        }
    }

    private DateTime _timeTick;
    private void OnUpdate(GameTime time)
    {
        if (DateTime.UtcNow < _timeTick)
            return;

        _timeTick = DateTime.UtcNow.AddSeconds(1);

        List<Region> regions = LoadedRegions;
        MintServer.Players.QuickForEach((p) =>
        {
            int tileX = (int)(p.TPlayer.position.X / 16f);
            int tileY = (int)(p.TPlayer.position.Y / 16f);

            PlayerRegion rgData = p.GetRegion();
            bool saveRequired = false;

            regions.ForEach((region) => UpdateRegion(p, rgData, region, tileX, tileY, ref saveRequired));

            if (saveRequired)
                p.PushRegion(rgData);
        });
    }

    internal void UpdateRegion(Player player, PlayerRegion rgData, Region region, int x, int y, ref bool saveRequired)
    {
        string name = region.Name;
        bool inZone = region.Zone.Contains(x, y);

        if (rgData.EnteredRegions.Contains(name))
        {   
            if (!inZone)
            {
                rgData.EnteredRegions.Remove(name);
                OnRegionLeft?.Invoke(player, region);
                saveRequired = true;

                region.Settings.OnLeaveCommands?.ForEach((c) => PushCommand(player, c));

                if (region.Settings.EnabledGodMode)
                    CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, true);

                if (region.Settings.EnabledGhostMode && Main.ServerSideCharacter)
                {
                    player.TPlayer.ghost = true;
                    NetMessage.SendData(13, -1, -1, NetworkText.Empty, player.Index);
                }
            }
        }
        else
        {
            if (inZone)
            {
                rgData.EnteredRegions.Add(name);
                OnRegionEntered?.Invoke(player, region);
                saveRequired = true;

                region.Settings.OnEnterCommands?.ForEach((c) => PushCommand(player, c));

                if (region.Settings.EnabledGodMode)
                    CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, false);

                if (region.Settings.EnabledGhostMode && Main.ServerSideCharacter)
                {
                    player.TPlayer.ghost = false;
                    NetMessage.SendData(13, -1, -1, NetworkText.Empty, player.Index);
                }
            }
        }
    }
    private void PushCommand(Player player, string command)
    {
        command = command.Replace("@plr", $"\"{player.Name}\"");
        RegionsPlayer.CommandsQueue.Add(command);
    }
}
