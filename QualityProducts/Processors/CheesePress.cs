using System.Collections.Generic;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles cheese press machines.</summary>
    internal class CheesePress : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Goat Milk => Goat Cheese
            new Recipe(
                name: "Goat Cheese",
                inputID: 436,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(426, 1)
            ),

            // L. Goat Milk => 2 Goat Cheese
            new Recipe(
                name: "Goat Cheese",
                inputID: 438,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(426, 2)
            ),

            // Milk => Cheese
            new Recipe(
                name: "Cheese",
                inputID: 184,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(424, 1)
            ),

            // Large Milk => 2 Cheese
            new Recipe(
                name: "Cheese",
                inputID: 186,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(424, 2)
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public CheesePress()
            : base(ProcessorTypes.CheesePress) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Update the game stats.</summary>
        /// <param name="obj">The previously held object.</param>
        /// <remarks>Derived from <see cref="SObject.checkForAction"/>,</remarks>
        protected override void UpdateStats(SObject obj)
        {
            if (obj.ParentSheetIndex == 426)
                Game1.stats.GoatCheeseMade++;
            else
                Game1.stats.CheeseMade++;
        }
    }
}
