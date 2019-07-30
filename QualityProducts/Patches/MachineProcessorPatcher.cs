using Harmony;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Patches
{
    /// <summary>Handles machine processor patches.</summary>
    internal static class MachineProcessorPatcher
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Handles mapping object instances to processors.</summary>
        private static ProcessorFactory Factory;


        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the patcher.</summary>
        /// <param name="harmony">The mod's Harmony instance.</param>
        /// <param name="factory">The processor factory from which to get processors.</param>
        public static void Init(HarmonyInstance harmony, ProcessorFactory factory)
        {
            MachineProcessorPatcher.Factory = factory;

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(MachineProcessorPatcher), nameof(MachineProcessorPatcher.PerformObjectDropInAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                prefix: new HarmonyMethod(typeof(MachineProcessorPatcher), nameof(MachineProcessorPatcher.CheckForAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.addWorkingAnimation)),
                prefix: new HarmonyMethod(typeof(MachineProcessorPatcher), nameof(MachineProcessorPatcher.AddWorkingAnimation))
            );
        }

        /// <summary>Override the object drop-in action if needed.</summary>
        /// <param name="__instance">The machine instance.</param>
        /// <param name="dropInItem">The item dropped in.</param>
        /// <param name="probe">Whether the game is only checking if an action is possible.</param>
        /// <param name="who">The player performing the action.</param>
        /// <param name="__result">The method result.</param>
        /// <returns>Returns true if the original method should be invoked, else false.</returns>
        public static bool PerformObjectDropInAction(SObject __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (!Factory.TryGetFor(__instance, out Processor processor))
                return true;

            return !processor.TryPerformObjectDropInAction(__instance, dropInItem, probe, who, ref __result);
        }

        /// <summary>Override the action to check for action if needed.</summary>
        /// <param name="__instance">The machine instance.</param>
        /// <param name="who">The player performing the action.</param>
        /// <param name="justCheckingForActivity">Whether the game is only checking if an action is possible.</param>
        /// <param name="__result">The method result.</param>
        /// <returns>Returns true if the original method should be invoked, else false.</returns>
        public static bool CheckForAction(SObject __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (!Factory.TryGetFor(__instance, out Processor processor))
                return true;

            return !processor.TryCheckForAction(__instance, who, justCheckingForActivity, ref __result);
        }

        /// <summary>Override the machine's working animation if needed.</summary>
        /// <param name="__instance">The machine instance.</param>
        /// <param name="environment">The in-game location containing the machine.</param>
        /// <returns>Returns true if the original method should be invoked, else false.</returns>
        public static bool AddWorkingAnimation(SObject __instance, GameLocation environment)
        {
            if (!Factory.TryGetFor(__instance, out Processor processor))
                return true;

            return !processor.TryAddWorkingAnimation(__instance, environment);
        }
    }
}
