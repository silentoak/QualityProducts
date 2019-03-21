using SilentOak.AutoQualityPatch.Patches.AutomateCompat;
using SilentOak.Patching;
using StardewModdingAPI;

namespace SilentOak.AutoQualityPatch
{
    internal class ModEntry : Mod
    {
        internal static IMonitor StaticMonitor { get; private set; }
        internal static IModHelper StaticHelper { get; private set; }

        public override void Entry(IModHelper helper)
        {
            StaticMonitor = Monitor;
            StaticHelper = Helper; 

            PatchManager.Apply(typeof(MachineSetInputPatch));
        }
    }
}
