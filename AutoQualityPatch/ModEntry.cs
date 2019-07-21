using SilentOak.AutoQualityPatch.Patches.AutomateCompat;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.Patching;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SilentOak.AutoQualityPatch
{
    internal class ModEntry : Mod
    {
        /// <summary>Writes messages to the console and log file.</summary>
        internal static IMonitor StaticMonitor;

        public override void Entry(IModHelper helper)
        {
            StaticMonitor = this.Monitor;
            Util.Init(Helper, Monitor);

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IQualityProductsAPI qualityProductsAPI = Helper.ModRegistry.GetApi<IQualityProductsAPI>("SilentOak.QualityProducts");
            if (qualityProductsAPI == null)
            {
                Monitor.Log("Could not find Quality Products' API. This mod will be disabled.", LogLevel.Error);
                return;
            }

            RecipeManager.Init(qualityProductsAPI);

            PatchManager.Apply(typeof(MachineGenericPullRecipePatch));
        }
    }
}
