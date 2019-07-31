using System;
using System.Linq;
using Harmony;
using Pathoschild.Stardew.Automate;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.AutoQualityPatch.Patches
{
    /// <summary>Handles patches for compatibility with the Automate mod.</summary>
    internal static class AutomatePatcher
    {
        /*********
        ** Fields
        *********/
        /// <summary>Writes messages to the console and log file.</summary>
        private static IMonitor Monitor;

        /// <summary>The full Automate machine type names to override.</summary>
        private static readonly string[] MachineTypeNames =
        {
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine",
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine",
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine",
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine",
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine",
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the patcher.</summary>
        /// <param name="harmony">The mod's Harmony instance.</param>
        /// <param name="monitor">Writes messages to the console and log file.</param>
        public static void Init(HarmonyInstance harmony, IMonitor monitor)
        {
            Monitor = monitor;

            foreach (string typeName in MachineTypeNames)
            {
                harmony.Patch(
                    original: AccessTools.Method($"{typeName}:SetInput"),
                    prefix: new HarmonyMethod(typeof(AutomatePatcher), nameof(AutomatePatcher.SetInput))
                );
            }
        }

        /// <summary>Replaces the recipes with the ones defined by the processor.</summary>
        public static bool SetInput(IMachine __instance, ref bool __result, IStorage input)
        {
            try
            {
                SObject machine = Util.Helper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
                if (QualityProducts.ModEntry.Factory.TryGetFor(machine, out Processor processor))
                {
                    IRecipe[] automateRecipes = Util.Helper.Reflection.GetField<IRecipe[]>(__instance, "Recipes").GetValue();
                    IRecipe[] recipes = RecipeManager.GetRecipeAdaptorsFor(processor, automateRecipes);

                    IConsumable consumable = null;
                    IRecipe recipe = null;

                    foreach (ITrackedStack item in input.GetItems())
                    {
                        IRecipe possibleRecipe = recipes.FirstOrDefault(rec => rec.AcceptsInput(item));
                        if (possibleRecipe != null && input.TryGetIngredient(item.Sample.ParentSheetIndex, possibleRecipe.InputCount, out consumable))
                        {
                            recipe = possibleRecipe;
                            break;
                        }
                    }

                    if (recipe != null && consumable != null)
                    {
                        Item inputStack = consumable.Take();
                        machine.heldObject.Value = recipe.Output(inputStack);
                        machine.MinutesUntilReady = recipe.Minutes;
                        __result = true;
                        return false;
                    }

                    __result = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed overriding Automate.\n{ex}", LogLevel.Error);
                return true; // run original code instead
            }
        }
    }
}
