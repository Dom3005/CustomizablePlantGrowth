using MelonLoader;
using UnityEngine;


namespace CustomizablePlantGrowth.UI
{
    public static class Manager
    {
        private static bool showMenu = false;

        public static void ToggleMenu() => showMenu = !showMenu;
        public static void ToggleMenu(bool value) => showMenu = value;
        
        public static void RenderUI()
        {
            GUIStyle sliderStyle = Utility.CreateWhiteBorderSliderStyle();
            GUIStyle thumbStyle = GUI.skin.horizontalSliderThumb;

            GUI.Box(new Rect(20, 20, 300, 375), "Customizable Plant Growth: Mod Settings");

            GUI.Label(new Rect(30, 50, 200, 20), "Plant Growth Multiplier: ");
            GUI.Label(new Rect(250, 50, 50, 20), Main.growthSliderValue.ToString("F1"));
            Main.growthSliderValue = GUI.HorizontalSlider(new Rect(30, 70, 250, 20), Main.growthSliderValue, 0.1f, 10.0f, sliderStyle, thumbStyle);

            GUI.Label(new Rect(30, 80, 200, 20), "Plant Yield Multiplier: ");
            GUI.Label(new Rect(250, 80, 50, 20), Main.yieldSliderValue.ToString("F1"));
            Main.yieldSliderValue = GUI.HorizontalSlider(new Rect(30, 100, 250, 20), Main.yieldSliderValue, 0.1f, 3.0f, sliderStyle, thumbStyle);

            GUI.Label(new Rect(30, 110, 200, 20), "Plant Yield per Bud: ");
            GUI.Label(new Rect(250, 110, 50, 20), Main.yieldPerBudSliderValue.ToString("F0"));
            Main.yieldPerBudSliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(new Rect(30, 130, 250, 20), Main.yieldPerBudSliderValue, 1, 10, sliderStyle, thumbStyle));

            Main.modifyQualityValue = GUI.Toggle(new Rect(30, 150, 200, 30), Main.modifyQualityValue, " Always legendary plants?");

            GUI.Label(new Rect(30, 170, 200, 20), "Drying speed: ");
            GUI.Label(new Rect(250, 170, 50, 20), Main.dryingSpeedSliderValue.ToString("F0"));
            Main.dryingSpeedSliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(new Rect(30, 190, 250, 20), Main.dryingSpeedSliderValue, 1, 10, sliderStyle, thumbStyle));

            GUI.Label(new Rect(30, 200, 200, 20), "Max Supplier delivery time: ");
            GUI.Label(new Rect(250, 200, 50, 20), Main.deliveryMaxSliderValue.ToString("F0") + "s");
            Main.deliveryMaxSliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(new Rect(30, 220, 250, 20), Main.deliveryMaxSliderValue, 0, 360, sliderStyle, thumbStyle));

            GUI.Label(new Rect(30, 230, 200, 20), "Water drain multiplier: ");
            GUI.Label(new Rect(250, 230, 50, 20), Main.waterDrainSliderValue.ToString("F1"));
            Main.waterDrainSliderValue = GUI.HorizontalSlider(new Rect(30, 250, 250, 20), Main.waterDrainSliderValue, 0, 2, sliderStyle, thumbStyle);

            Main.infiniteSoilValue = GUI.Toggle(new Rect(30, 270, 200, 30), Main.infiniteSoilValue, " Infinite soil?");

            if (!Main.recordingInput && GUI.Button(new Rect(30, 310, 200, 20), "Menu Keybind: " + (Main.keybindAddition.Value != KeyCode.None ? Main.keybindAddition.Value + "+" : "") + Main.keybindEntry.Value))
            {
                Main.recordingInput = !Main.recordingInput;
            }
            else if (Main.recordingInput && GUI.Button(new Rect(30, 310, 200, 20), "Recording.. (Esc to cancel)"))
            {
                Main.recordingInput = !Main.recordingInput;
            }

            if (GUI.Button(new Rect(240, 310, 50, 20), "Apply"))
            {
                // apply changes to config
                Main.growthRate.Value = Main.growthSliderValue;
                Main.yield.Value = Main.yieldSliderValue;
                Main.yieldPerBud.Value = Main.yieldPerBudSliderValue;
                Main.modifyQuality.Value = Main.modifyQualityValue;
                Main.dryingSpeed.Value = Main.dryingSpeedSliderValue;
                Main.waterDrain.Value = Main.waterDrainSliderValue;
                Main.infiniteSoil.Value = Main.infiniteSoilValue;
                Main.deliveryMax.Value = Main.deliveryMaxSliderValue;
                MelonPreferences.Save();

                Main.CloseGUI();
            }
        }
    }

    public class MenuPage
    {
        public int paddingTop = 40;
        public int paddingSides = 10;
        public int width = 300;
        public int height = 400;
        public int elementHeight = 20;
        public int elementHeightPadding = 20;

        public string title;

        public List<MenuElement> elements = new List<MenuElement>();


        public MenuPage(string title)
        {
            this.title = title;
        }

        public void RenderPage(int offsetX, int offsetY)
        {
            GUI.Box(new Rect(offsetX, offsetY, width, height), title);
            foreach (MenuElement element in elements)
            {
                Rect elementRect = new Rect(offsetX + paddingSides, offsetY + paddingTop + (elements.IndexOf(element) * (elementHeightPadding + elementHeight)), width - (paddingSides * 2), elementHeight);
                switch (element.menuType)
                {
                    case EMenuType.Slider:
                        MenuSlider slider = (MenuSlider)element;
                        slider.currentValue = GUI.HorizontalSlider(elementRect, slider.currentValue, slider.minValue, slider.maxValue);
                        elementRect.width -= 50;
                        GUI.Label(elementRect, slider.currentValue.ToString("F1"));
                        break;
                    case EMenuType.Toggle:
                        MenuToggle toggle = (MenuToggle)element;
                        toggle.currentValue = GUI.Toggle(elementRect, toggle.currentValue, toggle.text);
                        break;
                    case EMenuType.Button:
                        MenuButton button = (MenuButton)element;
                        if (GUI.Button(elementRect, button.text))
                            button.onClick?.Invoke();
                        break;
                    case EMenuType.Label:
                        MenuLabel label = (MenuLabel)element;
                        GUI.Label(elementRect, label.text);
                        break;
                }
            }
        }

        public MenuElement AddElement(MenuElement element)
        {
            elements.Add(element);
            return element;
        }
    }

    public class MenuElement
    {
        public EMenuType menuType;
        public string text;
    }

    public class MenuSlider : MenuElement
    {
        public float minValue;
        public float maxValue;
        public float currentValue;
        public string display = "F1"; // default display format (single decimal place)

        public MenuSlider(string _text, float value, float _min=0f, float _max=1f, string display="F1")
        {
            menuType = EMenuType.Slider;
            minValue = _min;
            maxValue = _max;
            currentValue = value;

            text = _text;
            this.display = display;
        }
    }

    public class MenuToggle : MenuElement
    {
        public bool currentValue;
        public MenuToggle(string _text, bool defaultValue)
        {
            menuType = EMenuType.Toggle;
            currentValue = defaultValue;
            text = _text;
        }
    }

    public class MenuButton : MenuElement
    {
        public Action onClick;
        public MenuButton(string _text, Action _onClick)
        {
            menuType = EMenuType.Button;
            text = _text;
            onClick = _onClick;
        }
    }

    public class MenuLabel : MenuElement
    {
        public MenuLabel(string _text)
        {
            menuType = EMenuType.Label;
            text = _text;
        }
    }

    public enum EMenuType
    {
        Slider,
        Toggle,
        Button,
        Label
    }
}