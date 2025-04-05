using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using UnityEngine;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(Plant), "GrowthDone")]
    public class PlantGrowthDonePatch
    {
        public static void Prefix(Plant __instance)
        {
            if (__instance == null) return;
            if (__instance.GetComponent<PlantModified>() != null) return;

            __instance.gameObject.AddComponent<PlantModified>();
            __instance.YieldLevel *= Main.yield.Value;
            if (Main.modifyQuality.Value) __instance.QualityLevel = 10;
        }
    }

    // Control class, to not modify the same plant twice
    public class PlantModified : MonoBehaviour { }

    [HarmonyPatch(typeof(PlantHarvestable), "Harvest")]
    public class PlantHarvestableHarvestPatch
    {
        public static void Prefix(PlantHarvestable __instance)
        {
            __instance.ProductQuantity = Main.yieldPerBud.Value;
        }
    }

    [HarmonyPatch(typeof(Pot), "GetAdditiveGrowthMultiplier")]
    public class PlantGetAdditiveGrowthMultiplierPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            __result *= Main.growthRate.Value;
        }
    }
}