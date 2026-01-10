using Il2CppScheduleOne.Growing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(ShroomColony), nameof(ShroomColony.SetGrowthPercentage))]
    public class ShroomColonySetGrowthPercentage
    {
        public static void Prefix(ShroomColony __instance, ref float percent)
        {
            float change = percent - __instance.GrowthProgress;
            percent = __instance.GrowthProgress + (change * Main.growthRate.Value * Main.shroomSpeedMultiplier.Value);
        }
    }
}
