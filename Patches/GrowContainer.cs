using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using UnityEngine;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(GrowContainer), nameof(GrowContainer.Awake))]
    public class GrowContainerAwakePatch
    {
        public static void Postfix(GrowContainer __instance)
        {
            __instance.gameObject.AddComponent<GrowContainerBaseValues>().Init(__instance);
        }
    }

    [HarmonyPatch(typeof(Pot), nameof(Pot.OnMinPass))]
    public class PotDrainMoisturePatch
    {
        public static void Prefix(GrowContainer __instance)
        {
            float baseDrain = __instance.gameObject.GetComponent<GrowContainerBaseValues>().baseWaterDrainPerHour;
            __instance._moistureDrainPerHour = baseDrain * Main.waterDrain.Value;
        }
    }

    public class GrowContainerBaseValues : MonoBehaviour
    {
        public void Init(GrowContainer container)
        {
            baseWaterDrainPerHour = container._moistureDrainPerHour;
        }
        public float baseWaterDrainPerHour;
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