using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using UnityEngine;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(Pot), "Start")]
    public class PotStartPatch
    {
        public static void Postfix(Pot __instance)
        {
            __instance.gameObject.AddComponent<PotBaseValues>().Init(__instance);
        }
    }

    [HarmonyPatch(typeof(Pot), "OnMinPass")]
    public class PotMinPassPatch
    {
        public static void Prefix(Pot __instance)
        {
            float baseDrain = __instance.gameObject.GetComponent<PotBaseValues>().baseWaterDrainPerHour;
            __instance.WaterDrainPerHour = baseDrain * Main.waterDrain.Value;
        }
    }

    [HarmonyPatch(typeof(Pot), "ResetPot")]
    public class PotSetSoilUsesPatch
    {
        public static void Prefix(Pot __instance)
        {
            if (Main.infiniteSoil.Value) __instance.RemainingSoilUses = 2;
        }
    }

    public class PotBaseValues : MonoBehaviour
    {
        public void Init(Pot pot)
        {
            baseWaterDrainPerHour = pot.WaterDrainPerHour;
        }
        public float baseWaterDrainPerHour;
    }
}