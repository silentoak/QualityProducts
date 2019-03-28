using SilentOak.QualityProducts.Utils;
using StardewModdingAPI;
using StardewValley;

namespace SilentOak.QualityProducts.Extensions
{
    public static class BuffExtensions
    {
        public static int[] GetBuffData(this Buff buff)
        {
            IReflectedField<int[]> buffAttributes = Util.Helper.Reflection.GetField<int[]>(buff, "buffAttributes");
            return buffAttributes?.GetValue();
        }
    }
}
