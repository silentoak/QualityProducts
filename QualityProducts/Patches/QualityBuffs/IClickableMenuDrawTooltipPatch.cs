using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using SilentOak.Patching;
using SilentOak.QualityProducts.Extensions;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Patches.QualityBuffs
{
    public static class IClickableMenuDrawToolTipPatch
    {
        public static PatchData PatchData = new PatchData(
            type: typeof(IClickableMenu),
            originalMethodName: "drawToolTip",
            originalMethodParams: new Type[] {
                typeof(SpriteBatch),
                typeof(string),
                typeof(string),
                typeof(Item),
                typeof(bool),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(CraftingRecipe),
                typeof(int)
            }
        );

        public static bool Prefix(
            SpriteBatch b,
            string hoverText,
            string hoverTitle,
            Item hoveredItem,
            bool heldItem = false,
            int healAmountToDisplay = -1,
            int currencySymbol = 0,
            int extraItemToShowIndex = -1,
            int extraItemToShowAmount = -1,
            CraftingRecipe craftingIngredients = null,
            int moneyAmountToShowAtBottom = -1
        )
        {
            if (hoveredItem == null ||
                !(hoveredItem is SObject))
            {
                return true;
            }

            SObject hoveredObject = hoveredItem as SObject;
            if (!BuffedEdible.TryGetBuff(hoveredObject, out Buff buff))
            {
                return true;
            }

            IClickableMenu.drawHoverText(
                b,
                text: hoverText,
                font: Game1.smallFont,
                xOffset: heldItem ? 40 : 0,
                yOffset: heldItem ? 40 : 0,
                moneyAmountToDisplayAtBottom: moneyAmountToShowAtBottom,
                boldTitleText: hoverTitle,
                healAmountToDisplay: hoveredObject.Edibility,
                buffIconsToDisplay: buff.GetBuffData().Select(val => val.ToString()).ToArray(), 
                hoveredItem: hoveredItem,
                currencySymbol: currencySymbol,
                extraItemToShowIndex: extraItemToShowIndex,
                extraItemToShowAmount: extraItemToShowAmount,
                overrideX: -1,
                overrideY: -1,
                alpha: 1f,
                craftingIngredients: craftingIngredients
            );
            return false;
        }
    }
}
