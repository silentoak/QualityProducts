using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles oil maker machines.</summary>
    internal class OilMaker : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Corn => Oil
            new Recipe(
                name: "Oil",
                inputID: 270,
                inputAmount: 1,
                minutes: 1000,
                process: _ => new SObject(247, 1)
            ),

            // Sunflower => Oil
            new Recipe(
                name: "Oil",
                inputID: 421,
                inputAmount: 1,
                minutes: 60,
                process: _ => new SObject(247, 1)
            ),

            // Sunflower Seeds => Oil
            new Recipe(
                name: "Oil",
                inputID: 431,
                inputAmount: 1,
                minutes: 3200,
                process: _ => new SObject(247, 1)
            ),

            // Truffle => Truffle Oil
            new Recipe(
                name: "Truffle Oil",
                inputID: 430,
                inputAmount: 1,
                minutes: 360,
                process: _ => new SObject(432, 1)
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public OilMaker()
            : base(ProcessorTypes.OilMaker) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>The input effects to apply if a recipe doesn't specify any.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="location">The location containing the machine.</param>
        protected override void DefaultInputEffects(SObject machine, GameLocation location)
        {
            location.playSound("bubbles");
            location.playSound("sipTea");
        }

        /// <summary>The working effects to apply if a recipe doesn't specify any.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="location">The location containing the machine.</param>
        protected override void DefaultWorkingEffects(SObject machine, GameLocation location)
        {
            Animation.PerformGraphics(location, Animation.Bubbles(machine.TileLocation, Color.Yellow));
        }
    }
}
