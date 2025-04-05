using System.Reflection.Emit;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(CustomizablePlantGrowth.Core), "CustomizablePlantGrowth", "1.1.1", "Dom3005", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace CustomizablePlantGrowth
{
    public class Core : MelonMod
    {
        public static HarmonyLib.Harmony harmony;

        private static MelonPreferences_Category plantGrowth;
        public static MelonPreferences_Entry<float> growthRate;
        public static MelonPreferences_Entry<float> yield;
        public static MelonPreferences_Entry<int> yieldPerBud;
        public static MelonPreferences_Entry<int> dryingSpeed;
        public static MelonPreferences_Entry<bool> modifyQuality;
        private static bool showMenu = false;
        private float growthSliderValue = 1;
        private float yieldSliderValue = 1;
        private int yieldPerBudSliderValue = 1;
        private int dryingSpeedSliderValue = 1;
        private bool modifyQualityValue = false;
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnApplicationStart()
        {
            plantGrowth = MelonPreferences.CreateCategory("CustomizablePlantGrowth", "Customizable Plant Growth");

            growthRate = plantGrowth.CreateEntry("GrowthRate", 1.0f, "Growth rate multiplier for plants (2.0 for double growth speed). Default is 1.0.");
            yield = plantGrowth.CreateEntry("Yield", 1.0f, "Yield multiplier for plants (2.0 for double yield). Default is 1.0.");
            yieldPerBud = plantGrowth.CreateEntry("YieldPerBud", 1, "Yield multiplier for buds (2 for double yield). Default is 1.");
            dryingSpeed = plantGrowth.CreateEntry("DryingSpeed", 1, "Drying speed (higher = faster). Default is 1.");
            modifyQuality = plantGrowth.CreateEntry("BestQuality", false, "Always best quality?. Default is false.");

            growthSliderValue = growthRate.Value;
            yieldSliderValue = yield.Value;
            yieldPerBudSliderValue = yieldPerBud.Value;
            dryingSpeedSliderValue = dryingSpeed.Value;

            ClassInjector.RegisterTypeInIl2Cpp<PlantModified>();

            harmony = new HarmonyLib.Harmony("com.dom3005.customizableplantgrowth");
            harmony.PatchAll();
        }

        public override void OnGUI()
        {
            if (!showMenu) return;

            GUIStyle sliderStyle = CreateWhiteBorderSliderStyle();
            GUIStyle thumbStyle = GUI.skin.horizontalSliderThumb;

            GUI.Box(new Rect(20, 20, 300, 230), "Customizable Plant Growth: Mod Settings");

            GUI.Label(new Rect(30, 50, 200, 20), "Plant Growth Multiplier: ");
            GUI.Label(new Rect(250, 50, 50, 20), growthSliderValue.ToString("F1"));
            growthSliderValue = GUI.HorizontalSlider(new Rect(30, 70, 250, 20), growthSliderValue, 0.1f, 10.0f, sliderStyle, thumbStyle);

            GUI.Label(new Rect(30, 80, 200, 20), "Plant Yield Multiplier: ");
            GUI.Label(new Rect(250, 80, 50, 20), yieldSliderValue.ToString("F1"));
            yieldSliderValue = GUI.HorizontalSlider(new Rect(30, 100, 250, 20), yieldSliderValue, 0.1f, 3.0f, sliderStyle, thumbStyle);

            GUI.Label(new Rect(30, 110, 200, 20), "Plant Yield per Bud: ");
            GUI.Label(new Rect(250, 110, 50, 20), yieldPerBudSliderValue.ToString("F0"));
            yieldPerBudSliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(new Rect(30, 130, 250, 20), yieldPerBudSliderValue, 1, 10, sliderStyle, thumbStyle));

            modifyQualityValue = GUI.Toggle(new Rect(30, 150, 200, 30), modifyQualityValue, " Always legendary plants?");

            GUI.Label(new Rect(30, 170, 200, 20), "Drying speed: ");
            GUI.Label(new Rect(250, 170, 50, 20), dryingSpeedSliderValue.ToString("F0"));
            dryingSpeedSliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(new Rect(30, 190, 250, 20), dryingSpeedSliderValue, 1, 10, sliderStyle, thumbStyle));

            if (GUI.Button(new Rect(30, 210, 100, 20), "Apply"))
            {
                growthRate.Value = growthSliderValue;
                yield.Value = yieldSliderValue;
                yieldPerBud.Value = yieldPerBudSliderValue;
                modifyQuality.Value = modifyQualityValue;
                dryingSpeed.Value = dryingSpeedSliderValue;

                CloseGUI();
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
                showMenu = !showMenu;
                if (showMenu) OpenGUI();
                else CloseGUI();
            }
        }
        private void CloseGUI()
        {
            showMenu = false;
            PlayerSingleton<PlayerMovement>.Instance.canMove = true;
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(true);
            PlayerSingleton<PlayerCamera>.Instance.RemoveActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.LockMouse();
        }

        private void OpenGUI()
        {
            showMenu = true;
            Singleton<GameInput>.Instance.ExitAll();
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(false);
            PlayerSingleton<PlayerMovement>.Instance.canMove = false;
            PlayerSingleton<PlayerCamera>.Instance.AddActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.FreeMouse();
        }
    }


    [HarmonyPatch(typeof(Pot), "GetAdditiveGrowthMultiplier")]
    public class PlantGetAdditiveGrowthMultiplierPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            __result *= Core.growthRate.Value;
        }
    }

    [HarmonyPatch(typeof(Plant), "GrowthDone")]
    public class PlantGrowthDonePatch
    {
        public static void Prefix(Plant __instance)
        {
            if (__instance == null) return;
            if (__instance.GetComponent<PlantModified>() != null) return;

            __instance.gameObject.AddComponent<PlantModified>();
            __instance.YieldLevel *= Core.yield.Value;
            if(Core.modifyQuality.Value) __instance.QualityLevel = 10;
        }
    }

    // Control class, to not modify the same plant twice
    public class PlantModified : MonoBehaviour
    { }

    [HarmonyPatch(typeof(PlantHarvestable), "Harvest")]
    public class PlantHarvestableHarvestPatch
    {
        public static void Prefix(PlantHarvestable __instance)
        {
            __instance.ProductQuantity = Core.yieldPerBud.Value;
        }
    }

    [HarmonyPatch(typeof(DryingRack), "MinPass")]
    public class DryingRackMinPassPatch
    {
        public static void Postfix(DryingRack __instance)
        {
            foreach(DryingOperation dryingOperation in __instance.DryingOperations)
            {
                if (dryingOperation != null)
                {
                    dryingOperation.Time += Core.dryingSpeed.Value - 1;
                }
            }
        }
    }
}