using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Extensions;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles keg machines.</summary>
    internal class Keg : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Wheat => Beer
            new Recipe(
                name: "Beer",
                inputID: 262,
                inputAmount: 1,
                minutes: 1750,
                process: _ => new SObject(346, 1),
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // Hops => Pale Ale
            new Recipe(
                name: "Pale Ale",
                inputID: 304,
                inputAmount: 1,
                minutes: 2250,
                process: _ => new SObject(303, 1),
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // 5 Coffee Beans => Coffee
            new Recipe(
                name: "Coffee",
                inputID: 433,
                inputAmount: 5,
                minutes: 120,
                process: input => new SObject(395, 1),
                failAmount: () =>
                {
                    Game1.showRedMessage(
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12721")
                    );
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.DarkGray));
                }
            ),

            // Honey => Mead
            new Recipe(
                name: "Mead",
                inputID: 340,
                inputAmount: 1,
                minutes: 600,
                process: input =>
                {
                    SObject mead = new SObject(459, 1);
                    SObject.HoneyType? maybeHoneyType = input.honeyType.Value;
                    if (maybeHoneyType.HasValue && maybeHoneyType.Value != SObject.HoneyType.Wild)
                    {
                        mead.Name = maybeHoneyType.Value.ToString().SplitCamelCase(join: " ") + " Mead";
                        mead.Price = 2 * input.Price;
                    }

                    mead.honeyType.Value = maybeHoneyType.Value;
                    return mead;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // Vegetable => Juice
            new Recipe(
                name: "Juice",
                inputID: SObject.VegetableCategory,
                inputAmount: 1,
                minutes: 6000,
                process: input =>
                {
                    SObject juice = new SObject(350, 1)
                    {
                        Name = input.Name + " Juice",
                        Price = (int) (2.25 * input.Price)
                    };
                    juice.preserve.Value = SObject.PreserveType.Juice;
                    juice.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return juice;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.White));
                }
            ),

            // Fruit => Wine
            new Recipe(
                name: "Wine",
                inputID: SObject.FruitsCategory,
                inputAmount: 1,
                minutes: 10000,
                process: input =>
                {
                    SObject wine = new SObject(348, 1)
                    {
                        Name = input.Name + " Wine",
                        Price = 3 * input.Price
                    };
                    wine.preserve.Value = SObject.PreserveType.Wine;
                    wine.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return wine;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Lavender));
                }
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Keg()
           : base(ProcessorTypes.Keg) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Update the game stats.</summary>
        /// <param name="obj">The previously held object.</param>
        /// <remarks>Derived from <see cref="SObject.checkForAction"/>,</remarks>
        protected override void UpdateStats(SObject obj)
        {
            Game1.stats.BeveragesMade++;
        }
    }
}
