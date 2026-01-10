using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(MushroomBed), nameof(MushroomBed.ClearSoil))]
    public class MushroomBedClearSoil
    {
        public static bool Prefix(MushroomBed __instance)
        {
            if (Main.infiniteSoil.Value)
            {
                __instance._remainingSoilUses = 2;
                return false; // dont run original method
            }
            return true; // run original method
        }
    }
}
