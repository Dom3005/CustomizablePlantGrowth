using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(CustomizablePlantGrowth.Core), "CustomizablePlantGrowth", "1.0.0", "Dom3005", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace CustomizablePlantGrowth
{
    public class Core : MelonMod
    {
        private static MelonPreferences_Category plantGrowth;
        public static MelonPreferences_Entry<float> growthRate;
        private static bool showMenu = false;
        private float sliderValue = 1;
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnApplicationStart()
        {
            plantGrowth = MelonPreferences.CreateCategory("CustomizablePlantGrowth", "Customizable Plant Growth");

            growthRate = plantGrowth.CreateEntry("GrowthRate", 100.0f, "Growth rate multiplier for plants (2.0 for double growth speed). Default is 1.0.");

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("com.dom3005.customizableplantgrowth");
            harmony.PatchAll();
        }

        public override void OnGUI()
        {
            if (!showMenu) return;

            GUI.Box(new Rect(20, 20, 500, 150), "Customizable Plant Growth: Mod Settings");
            GUI.Label(new Rect(30, 50, 200, 20), "Plant Growth Multiplier: ");

            GUI.Label(new Rect(250, 50, 50, 20), sliderValue.ToString("F1"));
            GUIStyle sliderStyle = CreateWhiteBorderSliderStyle();
            GUIStyle thumbStyle = GUI.skin.horizontalSliderThumb;

            sliderValue = GUI.HorizontalSlider(new Rect(30, 70, 250, 20), sliderValue, 0.1f, 10.0f, sliderStyle, thumbStyle);

            if(GUI.Button(new Rect(30, 100, 100, 20), "Apply"))
            {
                growthRate.Value = sliderValue;
                MelonLogger.Msg($"Growth rate set to: {growthRate.Value}");

                showMenu = false;
                PlayerSingleton<PlayerMovement>.Instance.canMove = true;
                PlayerSingleton<PlayerCamera>.Instance.SetCanLook(true);
                PlayerSingleton<PlayerCamera>.Instance.RemoveActiveUIElement("growthmod_settings");
                PlayerSingleton<PlayerCamera>.Instance.LockMouse();
            }
        }

        private Texture2D CreateWhiteBorderTexture(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color borderColor = Color.white;
            Color fillColor = Color.gray;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                    tex.SetPixel(x, y, isBorder ? borderColor : fillColor);
                }
            }
            tex.Apply();
            return tex;
        }

        private GUIStyle CreateWhiteBorderSliderStyle()
        {
            Texture2D bg = CreateWhiteBorderTexture(250, 20);
            GUIStyle style = new GUIStyle(GUI.skin.horizontalSlider);
            style.normal.background = bg;
            return style;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                MelonLogger.Msg("F1 pressed! Toggling menu visibility.");
                showMenu = !showMenu;
                if (showMenu)
                {
                    Singleton<GameInput>.Instance.ExitAll();
                    PlayerSingleton<PlayerCamera>.Instance.SetCanLook(false);
                    PlayerSingleton<PlayerMovement>.Instance.canMove = false;
                    PlayerSingleton<PlayerCamera>.Instance.AddActiveUIElement("growthmod_settings");
                    PlayerSingleton<PlayerCamera>.Instance.FreeMouse();

                }
                else
                {
                    PlayerSingleton<PlayerMovement>.Instance.canMove = true;
                    PlayerSingleton<PlayerCamera>.Instance.SetCanLook(true);
                    PlayerSingleton<PlayerCamera>.Instance.RemoveActiveUIElement("growthmod_settings");
                    PlayerSingleton<PlayerCamera>.Instance.LockMouse();
                }
            }
        }
    }

    //[HarmonyPatch(typeof(Plant), "MinPass")]
    //public class PlantMinPassPatch
    //{
    //    [HarmonyTranspiler]
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var codes = new List<CodeInstruction>(instructions);

    //        for (int i = 0; i < codes.Count; i++)
    //        {
    //            MelonLogger.Msg($"Instruction {i}: {codes[i].opcode} {codes[i].operand}");
    //            // Looking for the instruction that loads the local variable before the method call
    //            if ((codes[i].opcode == OpCodes.Ldloc || codes[i].opcode == OpCodes.Ldloc_S ||
    //                 codes[i].opcode == OpCodes.Ldloc_0 || codes[i].opcode == OpCodes.Ldloc_1 ||
    //                 codes[i].opcode == OpCodes.Ldloc_2 || codes[i].opcode == OpCodes.Ldloc_3) &&
    //                i + 1 < codes.Count && codes[i + 1].Calls(AccessTools.Method(typeof(Plant), "SetNormalizedGrowthProgress")))
    //            {
    //                // Insert our own code to modify the value on the stack
    //                codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_R4, Core.growthRate.Value + 100)); // For a float
    //                codes.Insert(i + 2, new CodeInstruction(OpCodes.Mul)); // Multiply by 2
    //                MelonLogger.Msg("Modified parameter value before method call");
    //                break;
    //            }
    //        }

    //        return codes;
    //    }
    //}

    [HarmonyPatch(typeof(Il2CppScheduleOne.ObjectScripts.Pot), "GetAdditiveGrowthMultiplier")]
    public class PlantGetAdditiveGrowthMultiplierPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            __result *= Core.growthRate.Value;
        }
    }

}