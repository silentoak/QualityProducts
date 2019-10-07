using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    /// <summary>A processor which handles loom machines.</summary>
    internal class Loom : Processor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The available recipes for this processor type.</summary>
        public override IEnumerable<Recipe> Recipes { get; } = new[]
        {
            // Wool => Cloth
            new Recipe(
                name: "Cloth",
                inputID: 440,
                inputAmount: 1,
                minutes: 240,
                process: _ => new SObject(428, 1)
            )
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Loom()
            : base(ProcessorTypes.Loom) { }
    }
}
