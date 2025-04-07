using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.DevUtilities;
using UnityEngine;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;

namespace CustomizablePlantGrowth.Patches
{
    [HarmonyPatch(typeof(Supplier), "SetDeaddrop", [typeof(Il2CppReferenceArray<StringIntPair>), typeof(int)])]
    public class SupplierSetDeaddropPatch
    {
        public static void Prefix(Il2CppReferenceArray<StringIntPair> items, ref int minsUntilReady)
        {
            minsUntilReady = Mathf.Min(minsUntilReady, Main.deliveryMax.Value);
        }
    }
}