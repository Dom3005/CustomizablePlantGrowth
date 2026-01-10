using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(ShroomColony), nameof(ShroomColony.GetHarvestedShroom))]
    public class ShroomColonyGetHarvestedShroomPatch
    {
        public static void Prefix(ShroomColony __instance)
        {
            if (Main.modifyQuality.Value) __instance.NormalizedQuality = 1; // 1 is best, 0 is worst
        }
    }
}
