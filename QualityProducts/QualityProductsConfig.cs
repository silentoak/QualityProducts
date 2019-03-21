using System;
namespace SilentOak.QualityProducts
{
    public class QualityProductsConfig
    {
        public bool EnableQualityCooking { get; set; } = true;
        public bool EnableMeadTextures { get; set; } = true;
        public string TextureForMeadTypes { get; set; } = "assets/mead-coloredbottles.png";
    }
}
