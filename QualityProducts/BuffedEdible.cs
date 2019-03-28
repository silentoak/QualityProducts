using System;
using System.Linq;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts
{
    internal static class BuffedEdible
    {
        internal static bool TryGetBuff(SObject @object, out Buff buff)
        {
            if (!TryGetBuffData(@object, out int[] buffData, out int buffDuration))
            {
                buff = null;
                return false;
            }

            int bonusFactor = Math.Min(@object.Quality, 3);
            for (int i = 0; i < buffData.Length; i++)
            {
                if (buffData[i] <= 3)
                {
                    if (buffData[i] > 0)
                    {
                        buffData[i] += bonusFactor - 1;
                    }
                   
                    continue;
                }

                buffData[i] += (int)Math.Round(bonusFactor * 0.5 * buffData[i]);
            }

            buff = new Buff(
                farming:        buffData[0],
                fishing:        buffData[1],
                mining:         buffData[2],
                digging:        buffData[3],
                luck:           buffData[4],
                foraging:       buffData[5],
                crafting:       buffData[6],
                maxStamina:     buffData[7],
                magneticRadius: buffData[8],
                speed:          buffData[9],
                defense:        buffData[10],
                attack: (buffData.Length > 11) ?
                    buffData[11] : 0,
                minutesDuration: buffDuration,
                source:        @object.Name,
                displaySource: @object.DisplayName
            );

            if (buff.total == 0)
            {
                buff = null;
                return false;
            }

            return true;
        }

        private static bool TryGetBuffData(SObject @object, out int[] buffData, out int buffDuration)
        {
            buffData = null;
            buffDuration = default;
            if (@object.Edibility < 0)
            {
                return false;
            }

            string[] objectData = Game1.objectInformation[@object.ParentSheetIndex].Split('/');
            if (objectData.Length <= 7)
            {
                return false;
            }

            buffData = objectData[7].Split(' ').Select(s => Convert.ToInt32(s)).ToArray();
            buffDuration = (objectData.Length > 8) ? Convert.ToInt32(objectData[8]) : -1;
            return true;
        }
    }
}
