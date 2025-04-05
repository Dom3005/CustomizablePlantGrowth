using System.Reflection.Emit;
using HarmonyLib;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(PotActionBehaviour), "CompleteAction")]
    public class PotActionBehaviorPatch
    {
        public static void Postfix(PotActionBehaviour __instance)
        {
            if (__instance.CurrentActionType != PotActionBehaviour.EActionType.Harvest) return;
            if (__instance.botanist == null || __instance.botanist.Inventory == null) return;


            ItemInstance item = __instance.botanist.MoveItemBehaviour.itemToRetrieveTemplate;
            item.Quantity = Math.Min(item.Quantity * Main.yieldPerBud.Value, item.StackLimit);

            MelonLogger.Msg("Grabbed: " + __instance.botanist.MoveItemBehaviour.grabbedAmount);
            MelonLogger.Msg("Amount: " + __instance.botanist.MoveItemBehaviour.itemToRetrieveTemplate.Quantity);

        }
    }
}