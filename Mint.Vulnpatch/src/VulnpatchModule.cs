using Mint.Assemblies.Modules;
using Mint.DataStorages;

namespace Mint.Vulnpatch;

public class VulnpatchModule : MintModule
{
    public override string ModuleName => "mint_vulnpatch";

    public override string ModuleVersion => "1.0";

    public override string[]? ModuleReferences => null;

    public override int ModuleArchitecture => 1;

    public override int Priority => 0;

    public static VulnpatchConfig Config { get; private set; }

    public override void Setup()
    {
        Config = Config<VulnpatchConfig>.GetData();
        PacketHandlers.Initialize();
    }

    public override void Initialize()
    {
    }
}
