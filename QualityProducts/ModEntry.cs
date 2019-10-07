using System;
using System.Runtime.CompilerServices;
using Harmony;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QualityProducts.Cooking;
using SilentOak.QualityProducts.API;
using SilentOak.QualityProducts.Patches;
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
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            Util.Init(helper, this.Monitor);
            if (Config.IsAnythingEnabled())
            {
                if (Config.IsCookingEnabled())
                    helper.Events.Display.MenuChanged += OnMenuChanged;

                if (Config.EnableMeadTextures)
                {
                    Texture2D meadTexture = this.GetMeadTexture(Config);
                    if (meadTexture != null)
                        BetterMeadIconsPatcher.Init(harmony, meadTexture);
                }
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

        /// <summary>Get the mead icon texture.</summary>
        /// <param name="config">The mod configuration.</param>
        private Texture2D GetMeadTexture(QualityProductsConfig config)
        {
            try
            {
                Texture2D meadTexture = this.Helper.Content.Load<Texture2D>(config.TextureForMeadTypes);
                this.Monitor.Log($"Custom texture \"{config.TextureForMeadTypes}\" loaded.", LogLevel.Trace);
                return meadTexture;
            }
            catch (ArgumentException)
            {
                this.Monitor.Log($"Invalid path \"{config.TextureForMeadTypes}\" for texture. Custom textures disabled.", LogLevel.Warn);
                return null;
            }
            catch (ContentLoadException)
            {
                this.Monitor.Log($"File \"{config.TextureForMeadTypes}\" could not be loaded. Custom textures disabled.", LogLevel.Warn);
                return null;
            }
        }
    }
}
