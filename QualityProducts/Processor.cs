using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Processors;
using SilentOak.QualityProducts.Utils;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts
{
    /// <summary>Handles processing logic for a particular machine type.</summary>
    public abstract class Processor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The current recipes for known machines.</summary>
        private IDictionary<SObject, Recipe> CurrentRecipes { get; } = new Dictionary<SObject, Recipe>(new ObjectReferenceComparer<SObject>());


        /*********
        ** Accessors
        *********/
        /// <summary>The processor type.</summary>
        public ProcessorTypes ProcessorType { get; }

        /// <summary>A human-readable name for the processor type.</summary>
        public string Name => this.ProcessorType.ToString();

        /// <summary>The available recipes for this processor type.</summary>
        public abstract IEnumerable<Recipe> Recipes { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Override the object drop-in action if needed.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="dropInItem">The item dropped in.</param>
        /// <param name="probe">Whether the game is only checking if an action is possible.</param>
        /// <param name="who">The player performing the action.</param>
        /// <param name="processed">Whether the input was handled by the machine.</param>
        /// <returns>Returns whether the method was successfully overridden.</returns>
        /// <remarks>Modified from <see cref="SObject.performObjectDropInAction"/>.</remarks>
        public bool TryPerformObjectDropInAction(SObject machine, Item dropInItem, bool probe, Farmer who, ref bool processed)
        {
            if (dropInItem is SObject input)
            {
                if (machine.heldObject.Value != null || input.bigCraftable.Value)
                {
                    processed = false;
                    return true;
                }

                if (!probe && machine.heldObject.Value == null)
                    machine.scale.X = 5f;

                /* 
                 * The base method will be called only if the object to be
                 * inserted is not an SObject, or a recipe for it was found but
                 * it was disabled in the config.
                 */
                if (!TryForRecipe(input, out Recipe recipe))
                {
                    processed = false;
                    return true;
                }

                if (!ModEntry.Config.IsEnabled(recipe, this))
                {
                    Util.Monitor.VerboseLog($"{recipe.Name} is disabled; fallback to default behaviour.");
                    return false;
                }

                if (probe)
                {
                    // awful, but it's what vanilla SDV does, so must be done for compatibility with other mods.
                    machine.heldObject.Value = recipe.Process(input);
                    processed = true;
                    return true;
                }

                if (PerformProcessing(machine, input, who, recipe))
                {
                    Vector2 tile = machine.TileLocation;
                    var output = machine.heldObject.Value;
                    Util.Monitor.VerboseLog($"Inserted {input.DisplayName} (quality {input.Quality}) into {machine.Name} @({tile.X},{tile.Y})");
                    Util.Monitor.VerboseLog($"{machine.Name} @({tile.X},{tile.Y}) is producing {output?.DisplayName} (quality {output?.Quality})");
                    processed = true;
                    return true;
                }

                processed = false;
                return true;
            }

            return false;
        }

        /// <summary>Override the action to check for action if needed.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="who">The player performing the action.</param>
        /// <param name="probe">Whether the game is only checking if an action is possible.</param>
        /// <param name="processed">Whether the action was handled by the machine.</param>
        /// <returns>Returns whether the method was successfully overridden.</returns>
        /// <remarks>Modified from <see cref="SObject.checkForAction"/>.</remarks>
        public bool TryCheckForAction(SObject machine, Farmer who, bool probe, ref bool processed)
        {
            if (!probe && who != null
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1)
                && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1)
                && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY())
                && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY())
                && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
            {
                machine.performToolAction(null, who.currentLocation);
            }

            if (machine.readyForHarvest.Value)
            {
                if (probe)
                {
                    processed = true;
                    return true;
                }

                if (who.IsLocalPlayer)
                {
                    SObject value2 = machine.heldObject.Value;
                    machine.heldObject.Value = null;
                    if (!who.addItemToInventoryBool(value2))
                    {
                        machine.heldObject.Value = value2;
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));

                        processed = false;
                        return true;
                    }
                    Game1.playSound("coin");
                    UpdateStats(value2);
                }

                machine.heldObject.Value = null;
                machine.readyForHarvest.Value = false;
                machine.showNextIndex.Value = false;

                processed = true;
                return true;
            }

            processed = false;
            return true;
        }

        /// <summary>Adds this entity's working animation to its location.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="environment">The in-game location containing the machine.</param>
        /// <returns>Returns whether the method was successfully overridden.</returns>
        /// <remarks>Modified from <see cref="SObject.addWorkingAnimation"/>.</remarks>
        public bool TryAddWorkingAnimation(SObject machine, GameLocation environment)
        {
            /* 
             * If not doing anything, then the recipe was disabled
             * and it should fall back on the game logic
             */
            Recipe recipe = this.GetCurrentRecipe(machine);
            if (recipe == null || environment == null || environment.farmers.Count == 0)
                return false;

            AddWorkingEffects(machine, environment, recipe);
            return true;
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="type">The processor type.</param>
        protected Processor(ProcessorTypes type)
        {
            this.ProcessorType = type;
        }

        /// <summary>Update the game stats.</summary>
        /// <param name="obj">The previously held object.</param>
        /// <remarks>Derived from <see cref="SObject.checkForAction"/>,</remarks>
        protected virtual void UpdateStats(SObject obj) { }

        /// <summary>The input effects to apply if a recipe doesn't specify any.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="location">The location containing the machine.</param>
        protected virtual void DefaultInputEffects(SObject machine, GameLocation location)
        {
            location.playSound("Ship");
        }

        /// <summary>The working effects to apply if a recipe doesn't specify any.</summary>
        /// <param name="machine">The machine being processed.</param>
        /// <param name="location">The location containing the machine.</param>
        protected virtual void DefaultWorkingEffects(SObject machine, GameLocation location) { }


        /// <summary>Get a recipe which accepts the given input.</summary>
        /// <param name="input">The ingredient for which to find a recipe.</param>
        /// <param name="foundRecipe">The recipe matching the input, if any.</param>
        /// <returns>Returns whether a recipe was found.</returns>
        private bool TryForRecipe(SObject input, out Recipe foundRecipe)
        {
            foundRecipe = Recipes.FirstOrDefault(recipe => recipe.AcceptsInput(input));
            return foundRecipe != null;
        }

        /// <summary>Process input for a machine.</summary>
        /// <param name="machine">The machine to process.</param>
        /// <param name="input">The ingredient to process.</param>
        /// <param name="who">The player who initiated processing.</param>
        /// <param name="recipe">The recipe to process.</param>
        /// <returns>Returns whether the machine started processing.</returns>
        private bool PerformProcessing(SObject machine, SObject input, Farmer who, Recipe recipe)
        {
            int amount = recipe.GetAmount(input);
            if (amount > input.Stack)
            {
                recipe.FailAmount();
                return false;
            }

            if (amount > 1)
            {
                input.Stack -= amount - 1;
                if (input.Stack <= 0)
                    who.removeItemFromInventory(input);
            }

            CurrentRecipes[machine] = recipe;
            machine.heldObject.Value = recipe.Process(input);
            machine.MinutesUntilReady = recipe.Minutes;

            /* Both of these need to be below the CurrentRecipe assignment above. */
            AddInputEffects(machine, who.currentLocation, recipe);
            AddWorkingEffects(machine, who.currentLocation, recipe);

            return true;
        }

        /// <summary>Trigger machine animations when an item is input.</summary>
        /// <param name="machine">The machine instance.</param>
        /// <param name="location">The location containing the machine.</param>
        /// <param name="recipe">The recipe being processed.</param>
        private void AddInputEffects(SObject machine, GameLocation location, Recipe recipe)
        {
            if (recipe?.InputEffects != null)
                recipe.InputEffects(location, machine.TileLocation);
            else
                this.DefaultInputEffects(machine, location);
        }

        /// <summary>Trigger machine animations when the machine is working.</summary>
        /// <param name="machine">The machine instance.</param>
        /// <param name="location">The location containing the machine.</param>
        /// <param name="recipe">The recipe being processed.</param>
        private void AddWorkingEffects(SObject machine, GameLocation location, Recipe recipe)
        {
            if (recipe?.WorkingEffects != null)
                recipe.WorkingEffects(location, machine.TileLocation);
            else
                this.DefaultWorkingEffects(machine, location);
        }

        /// <summary>Get the current recipe for a machine.</summary>
        /// <param name="obj">The machine instance.</param>
        private Recipe GetCurrentRecipe(SObject obj)
        {
            // no current recipe
            if (!this.CurrentRecipes.TryGetValue(obj, out Recipe recipe))
                return null;

            // recipe is outdated
            if (obj.heldObject.Value == null)
            {
                this.CurrentRecipes.Remove(obj);
                return null;
            }

            // recipe seems valid
            return recipe;
        }
    }
}
