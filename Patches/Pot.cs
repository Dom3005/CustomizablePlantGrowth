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
    [HarmonyPatch(typeof(Pot), nameof(Pot.OnMinPass))]
    public class PotDrainMoisturePatch
    {
        public static void Prefix(GrowContainer __instance)
        {
            float baseDrain = __instance.gameObject.GetComponent<GrowContainerBaseValues>().baseWaterDrainPerHour;
            __instance._moistureDrainPerHour = baseDrain * Main.waterDrain.Value;
        }
    }

    
    [HarmonyPatch(typeof(Pot), "OnPlantFullyHarvested")]
    public class PotOnPlantFullyHarvestedPatch
    {
        public static void Prefix(Pot __instance)
        {
            if (Main.infiniteSoil.Value) __instance._remainingSoilUses = 2;
        }
    }
   
}
