using System;
using System.Collections.Generic;
using StardewValley;

namespace QualityProducts
{
    public class GameLocationComparer : IEqualityComparer<GameLocation>
    {
        public bool Equals(GameLocation x, GameLocation y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(GameLocation obj)
        {
            return obj.Name.GetHashCode() ^ obj.uniqueName.GetHashCode() ^ obj.isStructure.GetHashCode();
        }
    }
}
