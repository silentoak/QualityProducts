using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Processors;
using SilentOak.QualityProducts.Util;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts
{
    /// <summary>
    /// An entity that is capable of processing items into products.
    /// </summary>
    public abstract class Processor : SObject
    {
        /*************
         * Properties
         *************/

        /// <summary>
        /// Gets the location this instance is in.
        /// </summary>
        /// <value>The location.</value>
        public GameLocation Location { get; }

        /// <summary>
        /// Gets the available recipes for this entity.
        /// </summary>
        /// <value>The recipes.</value>
        public abstract IEnumerable<Recipe> Recipes { get; }

        /// <summary>
        /// Gets or sets the current recipe.
        /// </summary>
        /// <value>The current recipe.</value>
        private Recipe CurrentRecipe { get; set; }


        /*********
         * Types
         ********/

        /// <summary>
        /// The available processor types.
        /// </summary>
        public enum ProcessorType
        {
            KEG = 12,
            PRESERVES_JAR = 15,
            CHEESE_PRESS = 16,
            LOOM = 17,
            OIL_MAKER = 19,
            MAYONNAISE_MACHINE = 24
        }


        /****************
         * Public methods
         ****************/

        /// <summary>
        /// Gets the type of the processor for the corresponding index.
        /// </summary>
        /// <returns>The processor type.</returns>
        /// <param name="parentSheetIndex">Parent sheet index.</param>
        public static ProcessorType? GetProcessorType(int parentSheetIndex)
        {
            if (Enum.IsDefined(typeof(ProcessorType), parentSheetIndex))
            {
                return (ProcessorType)Enum.ToObject(typeof(ProcessorType), parentSheetIndex);
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of the specified processor type.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="processorType">Processor type to be instantiated.</param>
        public static Processor Create(ProcessorType processorType, GameLocation location)
        {
            switch (processorType)
            {
                case ProcessorType.KEG:
                    return new Keg(location);
                case ProcessorType.PRESERVES_JAR:
                    return new PreservesJar(location);
                case ProcessorType.CHEESE_PRESS:
                    return new CheesePress(location);
                case ProcessorType.LOOM:
                    return new Loom(location);
                case ProcessorType.OIL_MAKER:
                    return new OilMaker(location);
                case ProcessorType.MAYONNAISE_MACHINE:
                    return new MayonnaiseMachine(location);
                default:
                    throw new UnimplementedCaseException($"Enum value {Enum.GetName(typeof(ProcessorType), processorType)} of Processor.ValidType has no corresponding case");
            }
        }

        /// <summary>
        /// Creates a new instance of the specified processor type, initializing it with the specified initializer.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="processorType">Processor type to be instantiated.</param>
        /// <param name="initializer">Initializer.</param>
        public static Processor Create(ProcessorType processorType, GameLocation location, Action<Processor> initializer)
        {
            Processor newObj = Create(processorType, location);
            initializer(newObj);
            return newObj;
        }

        /// <summary>
        /// Creates a processor instance based on the specified Stardew Valley object.
        /// </summary>
        /// <returns>The new processor instance.</returns>
        /// <param name="object">Reference object.</param>
        public static Processor FromObject(SObject @object, GameLocation location)
        {
            if (!@object.bigCraftable.Value)
            {
                return null;
            }

            ProcessorType? processorType = GetProcessorType(@object.ParentSheetIndex);
            if (processorType != null) {
                Processor processor = Create(processorType.Value, location,
                p => 
                {
                    p.TileLocation = @object.TileLocation;
                    p.IsRecipe = (bool)@object.isRecipe;
                    p.DisplayName = @object.DisplayName;
                    p.Scale = @object.Scale;
                    p.MinutesUntilReady = @object.MinutesUntilReady;
                });

                processor.owner.Value = @object.owner.Value;
                processor.heldObject.Value = @object.heldObject.Value;
                processor.readyForHarvest.Value = @object.readyForHarvest.Value;

                return processor;
            }
            return null;
        }

        /// <summary>
        /// Creates a new regular Stardew Valley object with the same attributes as this one.  
        /// </summary>
        /// <returns>The new object.</returns>
        public SObject ToObject()
        {
            SObject @object = new SObject(tileLocation, parentSheetIndex, false)
            {
                IsRecipe = (bool)isRecipe,
                Name = Name,
                DisplayName = base.DisplayName,
                Scale = Scale,
                MinutesUntilReady = MinutesUntilReady
            };

            @object.owner.Value = owner.Value;
            @object.heldObject.Value = heldObject.Value;
            @object.readyForHarvest.Value = readyForHarvest.Value;

            return @object;
        }

        /***
         * Modified from StardewValley.Object.performObjectDropInAction
         **/
        /// <summary>
        /// Performs the object drop in action.
        /// </summary>
        /// <returns><c>true</c>, if object drop in action was performed, <c>false</c> otherwise.</returns>
        /// <param name="dropInItem">Drop in item.</param>
        /// <param name="probe">If set to <c>true</c> probe.</param>
        /// <param name="who">Who.</param>
        public sealed override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (dropInItem is SObject @object)
            {
                if (heldObject.Value != null)
                {
                    ModEntry.StaticMonitor.VerboseLog($"{Name} already has object {heldObject.Value.Name}"); 
                    return false;
                }
                if (@object != null && (bool)@object.bigCraftable)
                {
                    return false;
                }
                if (!probe && @object != null && heldObject.Value == null)
                {
                    scale.X = 5f;
                }

                if (PerformProcessing(@object, probe, who))
                {
                    if (!probe)
                    {
                        ModEntry.StaticMonitor.VerboseLog($"Inserted {@object.DisplayName} (quality {@object.Quality}) into {Name} @({TileLocation.X},{TileLocation.Y})");
                        ModEntry.StaticMonitor.VerboseLog($"{Name} @({TileLocation.X},{TileLocation.Y}) is producing {heldObject.Value.DisplayName} (quality {heldObject.Value.Quality})");
                    }
                    return true;
                }
            }
            return false;
        }

        /***
         * Modified from StardewValley.Object.checkForAction
         ***/
        /// <summary>
        /// Checks for action, executing if available when <paramref name="justCheckingForActivity"/> is false.
        /// </summary>
        /// <returns><c>true</c>, if action was performed, <c>false</c> otherwise.</returns>
        /// <param name="who">Farmer that requested for action.</param>
        /// <param name="justCheckingForActivity">If set to <c>true</c>, doesn't execute any available action.</param>
        public sealed override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (!justCheckingForActivity && who != null
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1)
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1)
                && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY())
                && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY())
                && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
            {
                performToolAction(null, who.currentLocation);
            }

            if ((bool)readyForHarvest)
            {
                if (justCheckingForActivity)
                {
                    return true;
                }

                if (who.IsLocalPlayer)
                {
                    SObject value2 = heldObject.Value;
                    heldObject.Value = null;
                    if (!who.addItemToInventoryBool(value2, false))
                    {
                        heldObject.Value = value2;
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                        return false;
                    }
                    Game1.playSound("coin");
                    UpdateStats(value2);
                }

                heldObject.Value = null;
                readyForHarvest.Value = false;
                showNextIndex.Value = false;
                return true;
            }
            return false;
        }

        /***
         * Modified from StardewValley.Object.addWorkingAnimation
         **/
        /// <summary>
        /// Adds this entity's working animation to its location.
        /// </summary>
        /// <param name="environment">Game location.</param>
        public sealed override void addWorkingAnimation(GameLocation environment)
        {
            if (environment != null && environment.farmers.Count != 0)
            {
                AddWorkingEffects();
            }
        }


        /*******************
         * Internal methods
         *******************/

        /// <summary>
        /// Returns a function that executes the same process as the given one,
        /// but adds the ingredient's quality to the final product.
        /// </summary>
        /// <returns>The modified processing function.</returns>
        /// <param name="process">Function that transforms ingredients into products.</param>
        public static Func<SObject, SObject> WithQuality(Func<SObject, SObject> process)
        {
            return input =>
            {
                SObject output = process(input);
                output.Quality = input.Quality;
                return output;
            };
        }


        /*******************
         * Protected methods
         *******************/

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processor"/> of the given type.
        /// </summary>
        /// <param name="processorType">Processor type.</param>
        /// <param name="location">Where the entity is.</param>
        protected Processor(ProcessorType processorType, GameLocation location) : base(Vector2.Zero, (int)processorType, false)
        {
            Location = location;
        }

        /// <summary>
        /// Updates the game stats.
        /// </summary>
        /// <param name="object">Previously held object.</param>
        protected virtual void UpdateStats(SObject @object)
        {
            return; 
        }

        /// <summary>
        /// Executes if recipe doesn't specify any input effects
        /// </summary>
        protected virtual void DefaultInputEffects()
        {
            Location.playSound("Ship");
        }

        /// <summary>
        /// Executes if recipe doesn't specify any working effects
        /// </summary>
        protected virtual void DefaultWorkingEffects()
        {
            return;
        }


        /******************
         * Private methods
         ******************/

        /// <summary>
        /// Performs item processing.
        /// </summary>
        /// <returns><c>true</c> if started processing, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to be processed.</param>
        /// <param name="probe">If set to <c>true</c>, don't do anything.</param>
        /// <param name="who">Farmer that initiated processing.</param>
        private bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            CurrentRecipe = Recipes.FirstOrDefault(recipe => recipe.AcceptsInput(@object));

            if (CurrentRecipe == null)
            {
                return false;
            }

            if (!probe)
            {
                int amount = CurrentRecipe.GetAmount(@object);
                if (amount > @object.Stack)
                {
                    CurrentRecipe.FailAmount();
                    return false;
                }
    
                if (amount > 1)
                {
                    @object.Stack -= amount - 1;
                    if (@object.Stack <= 0)
                    {
                        who.removeItemFromInventory(@object);
                    }
                }

                heldObject.Value = WithQuality(CurrentRecipe.Process)(@object);
                minutesUntilReady.Value = CurrentRecipe.Minutes;
                AddInputEffects();
                AddWorkingEffects();
            }

            return true;
        }

        /// <summary>
        ///  Adds this entity's input animation to its location
        /// </summary>
        private void AddInputEffects()
        {
            if (CurrentRecipe != null)
            {
                if (CurrentRecipe.InputEffects != null)
                {
                    CurrentRecipe.InputEffects(Location, TileLocation);
                }
                else
                {
                    DefaultInputEffects();
                }
            }
        }

        /// <summary>
        /// Adds this entity's working animation to its location.
        /// </summary>
        private void AddWorkingEffects()
        {
            if (CurrentRecipe != null)
            {
                if (CurrentRecipe.WorkingEffects != null)
                {
                    CurrentRecipe.WorkingEffects(Location, TileLocation);
                }
                else
                {
                    DefaultWorkingEffects();
                }
            }
        }

    }
}
