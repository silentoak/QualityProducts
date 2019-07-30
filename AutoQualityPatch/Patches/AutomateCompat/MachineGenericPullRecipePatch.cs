using System;
using System.Linq;
using Pathoschild.Stardew.Automate;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.Patching;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.AutoQualityPatch.Patches.AutomateCompat
{
    /// <summary>
    /// Patch for Automate compatibility.
    /// </summary>
    internal static class MachineGenericPullRecipePatch
    {
        public static PatchData PatchData = new PatchData(
            assembly: typeof(IRecipe).Assembly,
            typeNames: new[]
            {
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            },
            originalMethodName: "SetInput",
            originalMethodParams: new[] { typeof(IStorage) }
        );

        /// <summary>
        /// Replaces the recipes with the ones defined by the processor.
        /// </summary>
        public static bool Prefix(IMachine __instance, ref bool __result, IStorage input)
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
                ModEntry.StaticMonitor.Log($"Failed overriding Automate.\n{ex}", LogLevel.Error);
                return true; // run original code instead
            }
        }
    }
}
