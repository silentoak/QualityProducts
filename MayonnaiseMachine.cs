﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace QualityProducts
{
    public class MayonnaiseMachine : Processor
    {
        public MayonnaiseMachine() : base(ProcessorType.MAYONNAISE_MACHINE)
        {
        }

        protected override bool PerformProcessing(Object @object, bool probe, Farmer who)
        {
            switch (@object.ParentSheetIndex)
            {
                case 107:
                case 174:
                case 182:
                    heldObject.Value = new Object(Vector2.Zero, 306, null, false, true, false, false)
                    {
                        Quality = 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 176:
                case 180:
                    heldObject.Value = new Object(Vector2.Zero, 306, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 442:
                    heldObject.Value = new Object(Vector2.Zero, 307, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 305:
                    heldObject.Value = new Object(Vector2.Zero, 308, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
            }
            return false;
        }
    }
}
