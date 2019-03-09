using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace QualityProducts
{
    public class QualityProducts : Mod
    {
        internal static QualityProducts Instance { get; private set; }

        private readonly Dictionary<GameLocation, List<Processor>> locationProcessors = new Dictionary<GameLocation, List<Processor>>(new GameLocationComparer());

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Helper.Events.Display.MenuChanged += OnCrafting;
            Helper.Events.GameLoop.Saved += OnSaved;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.World.LocationListChanged += OnLoadLocation;
            Helper.Events.World.ObjectListChanged += OnPlacingProcessor;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            locationProcessors.Clear();
        }

        private void OnLoadLocation(object sender, LocationListChangedEventArgs e)
        {
            foreach (GameLocation gameLocation in e.Added)
            {
                if (!locationProcessors.ContainsKey(gameLocation))
                {
                    locationProcessors.Add(gameLocation, new List<Processor>());
                }

                List<Processor> processors = new List<Processor>();
                foreach (Object @object in gameLocation.Objects.Values)
                {
                    if (@object.bigCraftable.Value && Processor.WhichProcessor(@object.ParentSheetIndex) != null && !(@object is Processor))
                    {
                        Processor processor = Processor.FromObject(@object);
                        processors.Add(processor);
                    }
                }

                foreach (Processor processor in processors)
                {
                    gameLocation.setObject(processor.TileLocation, processor);
                }
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
                List<Object> objects = new List<Object>();
                foreach (Object @object in kv.Key.Objects.Values)
                {
                    if (Processor.WhichProcessor(@object.ParentSheetIndex) != null && @object is Processor processor)
                    {
                        kv.Value.Add(processor);
                        Object objectClone = processor.ToObject();
                        objects.Add(objectClone);
                    }
                }

                foreach (Object @object in objects)
                {
                    kv.Key.setObject(@object.TileLocation, @object);
                }
            }
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            foreach (KeyValuePair<GameLocation, List<Processor>> kv in locationProcessors)
            {
                foreach (Processor processor in kv.Value)
                {
                    kv.Key.setObject(processor.TileLocation, processor);
                }
                kv.Value.Clear();
            }
        }

        private void OnPlacingProcessor(object sender, ObjectListChangedEventArgs e)
        {
            List<Processor> processors = new List<Processor>();
            foreach (KeyValuePair<Vector2, Object> kv in e.Added)
            {
                if (!(kv.Value is Processor))
                {
                    Processor processor = Processor.FromObject(kv.Value);
                    if (processor != null)
                    {
                        processors.Add(processor);
                    }
                }
            }

            foreach (Processor processor in processors)
            {
                e.Location.setObject(processor.TileLocation, processor);
            }
        }

        private void OnCrafting(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CraftingPage menu)
            {
                bool cooking = Helper.Reflection.GetField<bool>(menu, "cooking").GetValue();
                Game1.activeClickableMenu = new ModdedCraftingPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height, cooking);
            }
        }
    }
}
