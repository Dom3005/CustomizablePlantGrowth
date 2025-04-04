using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(CustomizablePlantGrowth.Core), "CustomizablePlantGrowth", "1.0.1", "Dom3005", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace CustomizablePlantGrowth
{
    public class Core : MelonMod
    {
        private static MelonPreferences_Category plantGrowth;
        public static MelonPreferences_Entry<float> growthRate;
        public static MelonPreferences_Entry<float> yield;
        private static bool showMenu = false;
        private float growthSliderValue = 1;
        private float yieldSliderValue = 1;
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnApplicationStart()
        {
            plantGrowth = MelonPreferences.CreateCategory("CustomizablePlantGrowth", "Customizable Plant Growth");

            growthRate = plantGrowth.CreateEntry("GrowthRate", 1.0f, "Growth rate multiplier for plants (2.0 for double growth speed). Default is 1.0.");
            yield = plantGrowth.CreateEntry("Yield", 1.0f, "Yield multiplier for plants (2.0 for double yield). Default is 1.0.");
            growthSliderValue = growthRate.Value;

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("com.dom3005.customizableplantgrowth");
            harmony.PatchAll();
        }

        public override void OnGUI()
        {
            if (!showMenu) return;

            GUIStyle sliderStyle = CreateWhiteBorderSliderStyle();
            GUIStyle thumbStyle = GUI.skin.horizontalSliderThumb;

            GUI.Box(new Rect(20, 20, 300, 150), "Customizable Plant Growth: Mod Settings");

            GUI.Label(new Rect(30, 50, 200, 20), "Plant Growth Multiplier: ");
            GUI.Label(new Rect(250, 50, 50, 20), growthSliderValue.ToString("F1"));
            growthSliderValue = GUI.HorizontalSlider(new Rect(30, 70, 250, 20), growthSliderValue, 0.1f, 10.0f, sliderStyle, thumbStyle);

            GUI.Label(new Rect(30, 120, 200, 20), "Plant Yield Multiplier: ");
            GUI.Label(new Rect(250, 120, 50, 20), yieldSliderValue.ToString("F1"));
            yieldSliderValue = GUI.HorizontalSlider(new Rect(30, 140, 250, 20), yieldSliderValue, 0.1f, 10.0f, sliderStyle, thumbStyle);

            if (GUI.Button(new Rect(30, 100, 100, 20), "Apply"))
            {
                growthRate.Value = growthSliderValue;
                yield.Value = yieldSliderValue;
                MelonLogger.Msg($"Growth rate set to: {growthRate.Value} and {yield.Value}x yield");

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