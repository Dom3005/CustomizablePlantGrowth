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

    public class GrowContainerBaseValues : MonoBehaviour
    {
        public void Init(GrowContainer container)
        {
            baseWaterDrainPerHour = container._moistureDrainPerHour;
        }
        public float baseWaterDrainPerHour;
    }

    [HarmonyPatch(typeof(GrowContainer), nameof(GrowContainer.SetMoistureAmount), new Type[] { typeof(float) })]
    public class GrowContainerSetMoistureAmountPatch
    {
        public static void Prefix(GrowContainer __instance, ref float amount)
        {
            if (__instance == null) return;

            var baseValues = __instance.gameObject.GetComponent<GrowContainerBaseValues>();
            float baseDrain = baseValues != null ? baseValues.baseWaterDrainPerHour : __instance._moistureDrainPerHour;

            __instance._moistureDrainPerHour = baseDrain * Main.waterDrain.Value;

            Main.Instance.LoggerInstance.Msg($"[SetMoistureAmountPatch] Adjusted moisture amount to {amount} (baseDrain: {baseDrain}, multiplier: {Main.waterDrain.Value})");
        }
    }
}