using MelonLoader;
using UnityEngine;

namespace CustomizablePlantGrowth
{
    public class Utility
    {
        public static Texture2D CreateWhiteBorderTexture(int width, int height)
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

        public static GUIStyle CreateWhiteBorderSliderStyle()
        {
            Texture2D bg = CreateWhiteBorderTexture(250, 20);
            GUIStyle style = new GUIStyle(GUI.skin.horizontalSlider);
            style.normal.background = bg;
            return style;
        }
    }
}