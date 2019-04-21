using System.Collections.Generic;
using StardewValley;

namespace SilentOak.QualityProducts.API
{
    /// <summary>
    /// API For getting the list with items 
    /// </summary>
    public interface IRemoteFridgeAPI
    {
        IList<Item> Fridge();
        void UseCustomCraftingMenu(bool enabled);
    }
}