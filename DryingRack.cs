using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(DryingRack), "MinPass")]
    public class DryingRackMinPassPatch
    {
        public static void Postfix(DryingRack __instance)
        {
            foreach (DryingOperation dryingOperation in __instance.DryingOperations)
            {
                if (dryingOperation != null)
                {
                    dryingOperation.Time += Main.dryingSpeed.Value - 1;
                }
            }
        }
    }
}