using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class Loom : Processor
    {
        /****************
         * Public methods
         ****************/

        public Loom() : base(ProcessorType.LOOM)
        {
        }


        /*******************
         * Protected methods
         *******************/

        /***
         * From StardewValley.Object.performObjectDropInAction
         ***/
        /// <summary>
        /// Performs item processing.
        /// </summary>
        /// <returns><c>true</c> if started processing, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to be processed.</param>
        /// <param name="probe">If set to <c>true</c> probe.</param>
        /// <param name="who">Farmer that initiated processing.</param>
        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            if (@object.ParentSheetIndex == 440)
            {
                heldObject.Value = new SObject(Vector2.Zero, 428, null, false, true, false, false);
                if (!probe)
                {
                    minutesUntilReady.Value = 240;
                    who.currentLocation.playSound("Ship");
                }
                return true;
            }
            return false;
        }
    }
}
