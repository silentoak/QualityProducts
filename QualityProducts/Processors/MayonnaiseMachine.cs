using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles oil maker machines.</summary>
    internal class MayonnaiseMachine : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Large Egg (Brown or White) or Dinosaur Egg => 2 Mayo
            new Recipe(
                name: "Mayonnaise",
                inputIDs: new int[] {107, 174, 182},
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(306, 2)
            ),

            // Egg (Brown or White) => Mayo
            new Recipe(
                name: "Mayonnaise",
                inputIDs: new int[] {176, 180},
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(306, 1)
            ),

            // Duck Egg => Duck Mayo
            new Recipe(
                name: "Duck Mayonnaise",
                inputID: 442,
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(307, 1)
            ),

            // Void Egg => Void Mayo
            new Recipe(
                name: "Void Mayonnaise",
                inputID: 305,
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(308, 1)
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public MayonnaiseMachine()
            : base(ProcessorTypes.MayonnaiseMachine) { }
    }
}
