using System.Linq;
using SilentOak.QualityProducts.Extensions;
using SilentOak.QualityProducts.Processors;

namespace SilentOak.QualityProducts
{
    public class QualityProductsConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The processors or recipes to enable.</summary>
        public string[] EnableQuality { get; set; } = new string[0];

        /// <summary>The processors or recipes to disable.</summary>
        public string[] DisableQuality { get; set; } = new string[0];

        /// <summary>Whether the custom mead texture should be enabled.</summary>
        public bool EnableMeadTextures { get; set; } = true;

        /// <summary>The custom texture to use for mead types.</summary>
        public string TextureForMeadTypes { get; set; } = "assets/mead-coloredbottles.png";


        /*********
        ** Public methods
        *********/
        /// <summary>Whether the <see cref="EnableQuality"/> property should be serialized.</summary>
        public bool ShouldSerializeEnableQuality()
        {
            return EnableQuality != null;
        }

        /// <summary>Whether the <see cref="DisableQuality"/> property should be serialized.</summary>
        public bool ShouldSerializeDisableQuality()
        {
            return DisableQuality != null;
        }

        /// <summary>Get whether a given processor should be enabled.</summary>
        /// <param name="processor">The processor to check.</param>
        public bool IsEnabled(Processor processor)
        {
            if (!EnableQuality.IsNullOrEmpty())
                return EnableQuality.Contains("All") || EnableQuality.Contains(processor.Name) || processor.Recipes.Any(recipe => EnableQuality.Contains(recipe.Name));

            if (!DisableQuality.IsNullOrEmpty())
                return !DisableQuality.Contains("All") && !DisableQuality.Contains(processor.Name) && processor.Recipes.Any(recipe => !DisableQuality.Contains(recipe.Name));

            return true;
        }

        /// <summary>Get whether a given recipe should be enabled.</summary>
        /// <param name="recipe">The recipe to check.</param>
        /// <param name="processor">The processor which handles the recipe.</param>
        public bool IsEnabled(Recipe recipe, Processor processor)
        {
            if (!EnableQuality.IsNullOrEmpty())
                return EnableQuality.Contains("All") || EnableQuality.Contains(processor.Name) || EnableQuality.Contains(recipe.Name);

            if (!DisableQuality.IsNullOrEmpty())
                return !DisableQuality.Contains("All") && !DisableQuality.Contains(processor.Name) && !DisableQuality.Contains(recipe.Name);

            return true;
        }

        /// <summary>Get whether cooking effects should be enabled.</summary>
        public bool IsCookingEnabled()
        {
            if (!EnableQuality.IsNullOrEmpty())
                return EnableQuality.Contains("All") || EnableQuality.Contains("Cooking");

            if (!DisableQuality.IsNullOrEmpty())
                return !DisableQuality.Contains("All") && !DisableQuality.Contains("Cooking");

            return true;
        }

        /// <summary>Get whether any features are enabled.</summary>
        public bool IsAnythingEnabled()
        {
            return
                !EnableQuality.IsNullOrEmpty()
                || DisableQuality.IsNullOrEmpty()
                || !DisableQuality.Contains("All");
        }
    }
}
