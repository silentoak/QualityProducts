using System.Runtime.CompilerServices;
using Harmony;
using QualityProducts.Cooking;
using SilentOak.Patching;
using SilentOak.QualityProducts.API;
using SilentOak.QualityProducts.Patches;
using SilentOak.QualityProducts.Patches.BetterMeadIcons;
using SilentOak.QualityProducts.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

[assembly: InternalsVisibleTo("AutoQualityPatch")]
namespace SilentOak.QualityProducts
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration from the player.</summary>
        internal static QualityProductsConfig Config { get; set; }

        /// <summary>Handles mapping object instances to processors.</summary>
        internal static ProcessorFactory Factory { get; set; } = new ProcessorFactory();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // read config
            Config = helper.ReadConfig<QualityProductsConfig>();

            // initialise
            Util.Init(helper, this.Monitor);
            if (Config.IsAnythingEnabled())
            {
                if (Config.IsCookingEnabled())
                    helper.Events.Display.MenuChanged += OnMenuChanged;

                if (Config.EnableMeadTextures && SpriteLoader.Init(helper, this.Monitor, Config))
                {
                    PatchManager.ApplyAll(
                        typeof(SObjectDrawPatch),
                        typeof(SObjectDraw2Patch),
                        typeof(SObjectDrawInMenuPatch),
                        typeof(SObjectDrawWhenHeld),
                        typeof(FurnitureDrawPatch)
                    );
                }

                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                MachineProcessorPatcher.Init(harmony, Factory);
            }
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
        public override object GetApi()
        {
            return new QualityProductsAPI(Config);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>Based on https://github.com/spacechase0/CookingSkill/blob/162be2dd01f2fb728f2e375f83152fe67f4da811/Mod.cs </remarks>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CraftingPage menu)
            {
                Monitor.VerboseLog("Cooking menu opened. Swapping to custom cooking menu...");
                bool cooking = Helper.Reflection.GetField<bool>(menu, "cooking").GetValue();
                Game1.activeClickableMenu = new ModdedCraftingPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height, cooking);
            }
        }
    }
}
