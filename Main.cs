using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;
using CustomizablePlantGrowth.Patches;


[assembly: MelonInfo(typeof(CustomizablePlantGrowth.Main), "CustomizablePlantGrowth", "1.4.0", "Dom3005", "https://www.nexusmods.com/schedule1/mods/233")]
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

        public static MelonPreferences_Entry<float> potYieldMultiplier;

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

        public static bool phoneModManagerLoaded = false;

        private float debugVar = 0;

        private static Action saveAction = ReloadVariables;
        public static event Action OnSave;
        

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");

            Instance = this;
        }

        public override void OnApplicationStart()
        {
            // config stuff
            plantGrowth = MelonPreferences.CreateCategory("CustomizablePlantGrowth", "Customizable Plant Growth");

            growthRate      = plantGrowth.CreateEntry("GrowthRate",     1.0f,   display_name: "Growth Rate",        description: "Growth rate multiplier for plants (2.0 for double growth speed). Default is 1.0.");
            yield           = plantGrowth.CreateEntry("Yield",          1.0f,   display_name: "Global Plant Yield",        description: "Global Yield multiplier for plants (2.0 for double yield). Default is 1.0.");
            yieldPerBud     = plantGrowth.CreateEntry("YieldPerBud",    1,      display_name: "Yield per bud",      description: "Yield multiplier for buds (2 for double yield). Default is 1.");
            dryingSpeed     = plantGrowth.CreateEntry("DryingSpeed",    1,      display_name: "Drying Speed",       description: "Drying speed (higher = faster). Default is 1.");
            modifyQuality   = plantGrowth.CreateEntry("BestQuality",    false,  display_name: "Always Legendary?",  description: "Always best quality?. Default is false.");
            waterDrain      = plantGrowth.CreateEntry("WaterDrain",     1.0f,   display_name: "Water Drainrate",    description: "Water drain multiplier (lower = water lasts longer). Default is 1.0");
            infiniteSoil    = plantGrowth.CreateEntry("InfiniteSoil",   false,  display_name: "Infinite Soil?",     description: "Infinite soil? (true = infinite soil). Default is false.");

            potYieldMultiplier = plantGrowth.CreateEntry("PotYieldMultiplier", 1.0f, display_name: "Pot Yield Multiplier", description: "Yield multiplier for pots (2.0 for double yield). Default is 1.0.");

            deliveryMax = plantGrowth.CreateEntry("DeliveryMax", 360, display_name: "Supplier longest delivery time (in s)", description: "Longest a supplier delivery can take in seconds. Default is 360");

            menuConfig = MelonPreferences.CreateCategory("CustomizablePlantGrowthMenu", "Menu Config");
            keybindEntry    = menuConfig.CreateEntry("Keybind",         KeyCode.F1,     display_name: "Menu Keybind",                       description: "Keybind to open the menu. (Default is F1)");
            keybindAddition = menuConfig.CreateEntry("KeybindAddition", KeyCode.None,   display_name: "Keybind Addition, can be (none)",    description: "Second key needed to open menu (e.g. alt, so its alt+F1) (Default is None)");

            InitGuiValues();


            // register types
            ClassInjector.RegisterTypeInIl2Cpp<PlantModified>();
            ClassInjector.RegisterTypeInIl2Cpp<GrowContainerBaseValues>();

            // register patches
            harmony = new HarmonyLib.Harmony("com.dom3005.customizableplantgrowth");
            harmony.PatchAll();
        }

        private static void InitGuiValues()
        {
            // init gui values
            growthSliderValue = growthRate.Value;
            yieldSliderValue = yield.Value;
            yieldPerBudSliderValue = yieldPerBud.Value;
            dryingSpeedSliderValue = dryingSpeed.Value;
            modifyQualityValue = modifyQuality.Value;
            deliveryMaxSliderValue = deliveryMax.Value;
            waterDrainSliderValue = waterDrain.Value;
            infiniteSoilValue = infiniteSoil.Value;
        }

        public override void OnGUI()
        {
            if (!showMenu) return;
            UI.Manager.RenderUI();
        }

        public static void ReloadVariables()
        {
            MelonPreferences.Load();
            InitGuiValues();
            MelonLogger.Msg("Reloaded variables from config.");
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
            PlayerSingleton<PlayerMovement>.Instance.CanMove = true;
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(true);
            PlayerSingleton<PlayerCamera>.Instance.RemoveActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.LockMouse();
        }

        public static void OpenGUI()
        {
            showMenu = true;
            InitGuiValues();
            Singleton<GameInput>.Instance.ExitAll();
            PlayerSingleton<PlayerCamera>.Instance.SetCanLook(false);
            PlayerSingleton<PlayerMovement>.Instance.CanMove = false;
            PlayerSingleton<PlayerCamera>.Instance.AddActiveUIElement("growthmod_settings");
            PlayerSingleton<PlayerCamera>.Instance.FreeMouse();
        }
    }
}