using System;
using System.Linq;
using Netcode;
using SilentOak.Patching;
using SilentOak.QualityProducts.Utils;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Patches.QualityBuffs
{
    public static class FarmerDoneEatingPatch
    {
        public static PatchData PatchData = new PatchData(
            type: typeof(Farmer),
            originalMethodName: "doneEating"
        );

        public static bool Prefix(Farmer __instance)
        {
            if (!__instance.IsLocalPlayer || __instance.mostRecentlyGrabbedItem == null ||
                __instance.itemToEat.ParentSheetIndex == 434)
            {
                return true;
            }

            SObject objectToEat = __instance.itemToEat as SObject;
            if (!BuffedEdible.TryGetBuff(objectToEat, out Buff objectBuff))
            {
                return true;
            }

            string[] itemData = Game1.objectInformation[objectToEat.ParentSheetIndex].Split('/');

            __instance.isEating = false;
            __instance.completelyStopAnimatingOrDoingAction();
            __instance.forceCanMove();

            if (itemData[6].Equals("drink"))
            {
                Game1.buffsDisplay.tryToAddDrinkBuff(objectBuff);
            }
            else if (itemData[6].Equals("food"))
            {
                Game1.buffsDisplay.tryToAddFoodBuff(objectBuff, 0 /* this seems to be useless */);
            }

            float stamina = __instance.Stamina;
            int health = __instance.health;
            int energyBonus = (int)Math.Ceiling(objectToEat.Edibility * 2.5) + objectToEat.Quality * objectToEat.Edibility;

            __instance.Stamina = Math.Min(__instance.MaxStamina, __instance.Stamina + energyBonus);
            __instance.health = Math.Min(__instance.maxHealth, __instance.health + (int)(energyBonus * 0.45f));

            return false;
        }
    }
}
