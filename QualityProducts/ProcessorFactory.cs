using System.Collections.Generic;
using SilentOak.QualityProducts.Processors;
using StardewValley;

namespace SilentOak.QualityProducts
{
    /// <summary>Handles mapping object instances to processors.</summary>
    public class ProcessorFactory
    {
        /*********
        ** Fields
        *********/
        /// <summary>The machine processors by the machine ID they handle.</summary>
        private readonly IDictionary<ProcessorTypes, Processor> MachineProcessors = new Dictionary<ProcessorTypes, Processor>
        {
            [ProcessorTypes.Keg] = new Keg(),
            [ProcessorTypes.PreservesJar] = new PreservesJar(),
            [ProcessorTypes.CheesePress] = new CheesePress(),
            [ProcessorTypes.Loom] = new Loom(),
            [ProcessorTypes.OilMaker] = new OilMaker(),
            [ProcessorTypes.MayonnaiseMachine] = new MayonnaiseMachine()
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Get the processor for a machine, if applicable.</summary>
        /// <param name="machine">The potential machine instance.</param>
        /// <param name="processor">The processor instance for the machine.</param>
        /// <returns>Returns whether a processor was found.</returns>
        public bool TryGetFor(Object machine, out Processor processor)
        {
            if (!machine.bigCraftable.Value)
            {
                processor = null;
                return false;
            }

            ProcessorTypes type = (ProcessorTypes)machine.ParentSheetIndex;
            return this.MachineProcessors.TryGetValue(type, out processor);
        }
    }
}
