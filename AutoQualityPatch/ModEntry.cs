using Harmony;
using SilentOak.AutoQualityPatch.Patches;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SilentOak.AutoQualityPatch
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Util.Init(Helper, Monitor);

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // init recipes
            IQualityProductsAPI api = Helper.ModRegistry.GetApi<IQualityProductsAPI>("SilentOak.QualityProducts");
            if (api == null)
            {
                Monitor.Log("Could not find Quality Products' API. This mod will be disabled.", LogLevel.Error);
                return;
            }
            RecipeManager.Init(api);

            // init patches
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            AutomatePatcher.Init(harmony, this.Monitor);
        }
    }
}
