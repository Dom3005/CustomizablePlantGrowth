using UnityEngine;


namespace CustomizablePlantGrowth.UI
{
    public static class Manager
    {
        private static bool showMenu = false;

        public static void ToggleMenu() => showMenu = !showMenu;
        public static void ToggleMenu(bool value) => showMenu = value;
        

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