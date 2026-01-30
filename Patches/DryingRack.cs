using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(DryingRack), nameof(DryingRack.OnTimePass))]
    public class DryingRackOnTimePassPatch
    {
        public static void Prefix(DryingRack __instance, ref int minutes)
        {
            minutes = Main.dryingSpeed.Value;
        }
    }
}