using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;
using CustomizablePlantGrowth.Patches;


[assembly: MelonInfo(typeof(CustomizablePlantGrowth.Main), "CustomizablePlantGrowth", "1.3.0", "Dom3005", "https://www.nexusmods.com/schedule1/mods/233")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace CustomizablePlantGrowth
{
    public class Main : MelonMod
    {
        public static HarmonyLib.Harmony harmony;
        public static Main Instance;

        // config stuff
        public static MelonPreferences_Category menuConfig;
        public static MelonPreferences_Entry<KeyCode> keybindEntry;
        public static MelonPreferences_Entry<KeyCode> keybindAddition;

        public static MelonPreferences_Category plantGrowth;
        public static MelonPreferences_Entry<float> growthRate;
        public static MelonPreferences_Entry<float> yield;
        public static MelonPreferences_Entry<int> yieldPerBud;
        public static MelonPreferences_Entry<int> dryingSpeed;
        public static MelonPreferences_Entry<bool> modifyQuality;
        public static MelonPreferences_Entry<int> deliveryMax;
        public static MelonPreferences_Entry<float> waterDrain;
        public static MelonPreferences_Entry<bool> infiniteSoil;

        // gui stuff
        public static bool showMenu = false;
        public static float growthSliderValue = 1;
        public static float yieldSliderValue = 1;
        public static int yieldPerBudSliderValue = 1;
        public static int dryingSpeedSliderValue = 1;
        public static bool modifyQualityValue = false;
        public static int deliveryMaxSliderValue = 360;
        public static float waterDrainSliderValue = 1;
        public static bool infiniteSoilValue = false;

        public static bool recordingInput = false;

        private float debugVar = 0;


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            Instance = this;
        }

        public override void OnApplicationStart()
        {
            // config stuff
            plantGrowth = MelonPreferences.CreateCategory("CustomizablePlantGrowth", "Customizable Plant Growth");

            growthRate = plantGrowth.CreateEntry("GrowthRate", 1.0f, description: "Growth rate multiplier for plants (2.0 for double growth speed). Default is 1.0.");
            yield = plantGrowth.CreateEntry("Yield", 1.0f, description: "Yield multiplier for plants (2.0 for double yield). Default is 1.0.");
            yieldPerBud = plantGrowth.CreateEntry("YieldPerBud", 1, description: "Yield multiplier for buds (2 for double yield). Default is 1.");
            dryingSpeed = plantGrowth.CreateEntry("DryingSpeed", 1, description: "Drying speed (higher = faster). Default is 1.");
            modifyQuality = plantGrowth.CreateEntry("BestQuality", false, description: "Always best quality?. Default is false.");
            waterDrain = plantGrowth.CreateEntry("WaterDrain", 1.0f, description: "Water drain multiplier (lower = water lasts longer). Default is 1.0");
            infiniteSoil = plantGrowth.CreateEntry("InfiniteSoil", false, description: "Infinite soil? (true = infinite soil). Default is false.");

            deliveryMax = plantGrowth.CreateEntry("DeliveryMax", 360, description: "Longest a supplier delivery can take in seconds. Default is 360");

            menuConfig = MelonPreferences.CreateCategory("CustomizablePlantGrowthMenu", "Menu Config");
            keybindEntry = menuConfig.CreateEntry("Keybind", KeyCode.F1, description: "Keybind to open the menu. (Default is F1)");
            keybindAddition = menuConfig.CreateEntry("KeybindAddition", KeyCode.None, description: "Second key needed to open menu (e.g. alt) (Default is None)");

            // init gui values
            growthSliderValue = growthRate.Value;
            yieldSliderValue = yield.Value;
            yieldPerBudSliderValue = yieldPerBud.Value;
            dryingSpeedSliderValue = dryingSpeed.Value;
            modifyQualityValue = modifyQuality.Value;
            deliveryMaxSliderValue = deliveryMax.Value;
            waterDrainSliderValue = waterDrain.Value;
            infiniteSoilValue = infiniteSoil.Value;


            // register types
            ClassInjector.RegisterTypeInIl2Cpp<PlantModified>();
            ClassInjector.RegisterTypeInIl2Cpp<PotBaseValues>();

            // register patches
            harmony = new HarmonyLib.Harmony("com.dom3005.customizableplantgrowth");
            harmony.PatchAll();
        }

        public override void OnGUI()
        {
            if (!showMenu) return;
            UI.Manager.RenderUI();
            //UI.MenuPage page = new UI.MenuPage("Customizable Plant Growth: Mod Settings");
            //page.AddElement(new UI.MenuLabel("Customizable Plant Growth: Mod Settings"));
            //UI.MenuSlider slider = (UI.MenuSlider)page.AddElement(new UI.MenuSlider("Plant Growth Multiplier", debugVar, 0.1f, 10.0f));
            //debugVar = slider.currentValue;

            //page.RenderPage(20, 20);
            //return;


        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(keybindEntry.Value) && (keybindAddition.Value == KeyCode.None || Input.GetKey(keybindAddition.Value)) && !recordingInput)
            {
                showMenu = !showMenu;
                if (showMenu) OpenGUI();
                else CloseGUI();
            }
            if(recordingInput)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    recordingInput = false;
                    return;
                }

                if (Input.GetKey(KeyCode.LeftAlt)) keybindAddition.Value = KeyCode.LeftAlt;
                else if (Input.GetKey(KeyCode.LeftControl)) keybindAddition.Value = KeyCode.LeftControl;
                else if (Input.GetKey(KeyCode.LeftShift)) keybindAddition.Value = KeyCode.LeftShift;
                else keybindAddition.Value = KeyCode.None;

                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (key != KeyCode.None && key != KeyCode.Escape && key != KeyCode.LeftAlt && key != KeyCode.LeftControl && key != KeyCode.LeftShift)
                    {
                        if (Input.GetKey(key))
                        {
                            recordingInput = false;
                            keybindEntry.Value = key;
                            break;
                        }
                    }
                }
            }
        }
        public static void CloseGUI()
        {
            showMenu = false;
            PlayerSingleton<PlayerMovement>.Instance.canMove = true;
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(true);
            PlayerSingleton<PlayerCamera>.Instance.RemoveActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.LockMouse();
        }

        public static void OpenGUI()
        {
            showMenu = true;
            Singleton<GameInput>.Instance.ExitAll();
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(false);
            PlayerSingleton<PlayerMovement>.Instance.canMove = false;
            PlayerSingleton<PlayerCamera>.Instance.AddActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.FreeMouse();
        }
    }
}