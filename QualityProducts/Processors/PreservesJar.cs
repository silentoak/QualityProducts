using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles preserves jar machines.</summary>
    internal class PreservesJar : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Vegetable => Pickles
            new Recipe(
                name: "Pickles",
                inputID: SObject.VegetableCategory,
                inputAmount: 1,
                minutes: 4000,
                process: input =>
                {
                    SObject output = new SObject(342, 1)
                    {
                        Price = 50 + input.Price * 2,
                        Name = "Pickled " + input.Name
                    };

                    output.preserve.Value = SObject.PreserveType.Pickle;
                    output.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return output;
                },
                workingEffects: (location, tile) =>
                {
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.White));
                }
            ),

            // Fruit => Jelly
            new Recipe(
                name: "Jelly",
                inputID: SObject.FruitsCategory,
                inputAmount: 1,
                minutes: 4000,
                process: input =>
                {
                    SObject output = new SObject(344, 1)
                    {
                        Price = 50 + input.Price * 2,
                        Name = input.Name + " Jelly"
                    };

                    output.preserve.Value = SObject.PreserveType.Jelly;
                    output.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return output;
                },
                workingEffects: (location, tile) =>
                {
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.LightBlue));
                }
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public PreservesJar()
            : base(ProcessorTypes.PreservesJar) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Update the game stats.</summary>
        /// <param name="obj">The previously held object.</param>
        /// <remarks>Derived from <see cref="SObject.checkForAction"/>,</remarks>
        protected override void UpdateStats(SObject obj)
        {
            Game1.stats.PreservesMade++;
        }
    }
}
