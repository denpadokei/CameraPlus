using UnityEngine;
using System;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    public static class MenuUI
    {
        public static Texture2D[] IconTexture;
        public static GUIStyle[] CustomStyle;
        public static GUIStyle[] CustomToggleButtonStyle;
        public static bool UIInitialize = false;

        private static Rect s_uiRect;
        private static Rect s_swRect;

        private static float s_menuWidth = 300;
        private static float s_menuHeight = 440;

        public static int MenuGridCol = 8;
        public static int MenuGridRow = 16;

        private static float s_gridCellWidth { get { return s_menuWidth / (MenuGridCol >= 1 ? MenuGridCol : 1); } }
        private static float s_gridCellHeight { get { return s_menuHeight / (MenuGridRow >= 1 ? MenuGridRow : 1); } }
        public static Vector2 MenuPos
        {
            get
            {
                return new Vector2(
                   Mathf.Min(MousePosition.x / (Screen.width / 1600f), (Screen.width * (0.806249998f / (Screen.width / 1600f)))),
                   Mathf.Min((Screen.height - MousePosition.y) / (Screen.height / 900f), (Screen.height * (0.475555556f / (Screen.height / 900f))))
                    );
            }
        }
        public static Vector2 MousePosition;
        public static void Initialize()
        {
            IconTexture = new Texture2D[8]
            {
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.sw_off.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.sw_on.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenLeftDock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopLeftDock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomLeftDock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopRightDock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomRightDock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenRightDock.png")
            };
            CustomStyle = new GUIStyle[7]
            {
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft},
                new GUIStyle(GUI.skin.button) {normal = new GUIStyleState() {textColor = Color.black } },
                new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft},
                new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleCenter},
                new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter}
            };
            CustomStyle[0].normal.background = CustomStyle[0].active.background;
            CustomStyle[0].hover.background = CustomStyle[0].active.background;
            CustomToggleButtonStyle = new GUIStyle[2]
            {
                new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft },
                new GUIStyle(GUI.skin.label)
            };
            s_swRect = new Rect(0, 0, IconTexture[0].width, IconTexture[0].height);
            s_uiRect = new Rect();
            UIInitialize = true;
        }

        public static bool ToggleSwitch(int col, int row, string content, bool value = false, int colSpan = 0, int rowSpan = 0, float swScale = 0)
        {
            var screenRect = GridRect(col, row, colSpan, rowSpan);
            bool result;
            float scale = (swScale == 0 ? screenRect.height : s_gridCellHeight * swScale) / IconTexture[0].height;
            s_swRect.width = IconTexture[0].width * scale;
            s_swRect.height = IconTexture[0].height * scale;
            s_swRect.x = screenRect.x + screenRect.width - s_swRect.width + 10;
            s_swRect.y = screenRect.y + (swScale == 0 ?  0 : (screenRect.height - s_swRect.height) / 2);
            result = GUI.Button(screenRect, content, CustomToggleButtonStyle[0]);
            GUI.Box(s_swRect, value ? IconTexture[1] : IconTexture[0], CustomToggleButtonStyle[1]);
            return result;
        }

        public static void SetGrid(int col, int row)
        {
            MenuGridCol = col;
            MenuGridRow = row;
        }

        public static Rect GridRect(int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            s_uiRect.width = s_gridCellWidth * (colSpan >= 1 ? colSpan : 1);
            s_uiRect.height = s_gridCellHeight * (rowSpan >= 1 ? rowSpan : 1);
            s_uiRect.x = MenuPos.x + s_gridCellWidth * col;
            s_uiRect.y = MenuPos.y + s_gridCellHeight * row + 25;

            return s_uiRect;
        }

        public static int SelectionGrid(int col, int row, int selected, ref int currentPage, string[] texts, string currentSelected = "", int colSpan = 0, int rowSpan = 0)
        {
            int selection = selected;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);

            if (GUI.Button(new Rect(s_uiRect.x, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 6), "<"))
                if (currentPage > 0) currentPage--;
            
            GUI.Box(new Rect(s_uiRect.x + s_uiRect.width / 3, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 6), $"{currentPage + 1} / {Math.Ceiling(Decimal.Parse(texts.Length.ToString()) / 5)}");
            
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 3 * 2, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 6), ">"))
                if (currentPage < Math.Ceiling(Decimal.Parse(texts.Length.ToString()) / 5) - 1) currentPage++;
            
            for (int i = currentPage * 5; i < currentPage * 5 + 5; i++)
            {
                if (i < texts.Length)
                {
                    if (GUI.Button(new Rect(s_uiRect.x, s_uiRect.y + s_uiRect.height / 6 * (i - currentPage * 5 + 1), s_uiRect.width, s_uiRect.height / 6), 
                        $"{(texts[i] == Plugin.cameraController.CurrentProfile ||(currentSelected != string.Empty && currentSelected == texts[i]) ? "* " : string.Empty)}{(texts[i] == string.Empty ? "Default" : texts[i])}", selection == i ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
                        selection = i;
                }
            }
            return selection;
        }

        public static bool SliderComponent(int col, int row, ref float value, string title, float min, float max, int colSpan = 0, int rowSpan = 0)
        {
            bool changed = false;
            float tmpvalue = value;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);

            return changed;
        }

        public static bool AxizEdit(int col, int row, ref float[] axiz, float delta, int colSpan = 0, int rowSpan = 0, bool isRotation = false)
        {
            bool pushButton = false;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);
            GUI.Label(new Rect(s_uiRect.x, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 2), $"X {(isRotation ? "rot" : "pos")} : {axiz[0].ToString("F2")}", CustomStyle[6]);
            GUI.Label(new Rect(s_uiRect.x + s_uiRect.width / 3, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 2), $"Y {(isRotation ? "rot" : "pos")} : {axiz[1].ToString("F2")}", CustomStyle[6]);
            GUI.Label(new Rect(s_uiRect.x + s_uiRect.width / 3 * 2, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height / 2), $"Z {(isRotation ? "rot" : "pos")} : {axiz[2].ToString("F2")}", CustomStyle[6]);
            if(GUI.Button(new Rect(s_uiRect.x + 2, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "-"))
            {
                axiz[0] -= delta;
                if(isRotation && axiz[0] < 0)
                    axiz[0] += 360;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "+"))
            {
                axiz[0] += delta;
                if (isRotation && axiz[0] >= 360)
                    axiz[0] -= 360;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 2 + 2, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "-"))
            {
                axiz[1] -= delta;
                if (isRotation && axiz[1] < 0)
                    axiz[1] += 360;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 3, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "+"))
            {
                axiz[1] += delta;
                if (isRotation && axiz[1] >= 360)
                    axiz[1] -= 360;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 4 + 2, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "-"))
            {
                axiz[2] -= delta;
                if (isRotation && axiz[2] < 0)
                    axiz[2] += 360;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 5, s_uiRect.y + s_uiRect.height / 2, s_uiRect.width / 6 - 2, s_uiRect.height / 2), "+"))
            {
                axiz[2] += delta;
                if (isRotation && axiz[2] >= 360)
                    axiz[2] -= 360;
                pushButton = true;
            }
            return pushButton;
        }

        public static bool CrossSelection(int col, int row, ref string selected, int colSpan = 0, int rowSpan = 0)
        {
            bool pushButton = false;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);
            if(GUI.Button(new Rect(s_uiRect.x +  (s_uiRect.width / 3), s_uiRect.y, s_uiRect.width /3, s_uiRect.height / 3), "Top", selected == "Top" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                pushButton = true;
                selected = "Top";
            }
            if (GUI.Button(new Rect(s_uiRect.x, s_uiRect.y + (s_uiRect.height / 3), s_uiRect.width / 3, s_uiRect.height / 3), "Left", selected == "Left" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                pushButton = true;
                selected = "Left";
            }
            if (GUI.Button(new Rect(s_uiRect.x + (s_uiRect.width / 3), s_uiRect.y + (s_uiRect.height / 3), s_uiRect.width / 3, s_uiRect.height / 3), "Center", selected == "Center" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                pushButton = true;
                selected = "Center";
            }
            if (GUI.Button(new Rect(s_uiRect.x + (s_uiRect.width / 3) * 2, s_uiRect.y + (s_uiRect.height / 3), s_uiRect.width / 3, s_uiRect.height / 3), "Right", selected == "Right" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                pushButton = true;
                selected = "Right";
            }
            if (GUI.Button(new Rect(s_uiRect.x + (s_uiRect.width / 3), s_uiRect.y + (s_uiRect.height / 3) * 2, s_uiRect.width / 3, s_uiRect.height / 3), "Bottom", selected == "Bottom" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                pushButton = true;
                selected = "Bottom";
            }
            return pushButton;
        }
        public static bool SpinBox(int col, int row, ref float value, float delta, float min, float max, int digit = 0, int colSpan = 0, int rowSpan = 0)
        {
            bool pushButton = false;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);
            if(GUI.Button(new Rect(s_uiRect.x + 2, s_uiRect.y, s_uiRect.width / 3 - 2, s_uiRect.height), "-"))
            {
                if (value - delta >= min)
                    value -= delta;
                else
                    value = min;
                pushButton = true;
            }
            GUI.Box(new Rect(s_uiRect.x + s_uiRect.width / 3, s_uiRect.y, s_uiRect.width / 3, s_uiRect.height), value.ToString($"F{digit}"), CustomStyle[5]);
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 3 * 2, s_uiRect.y, s_uiRect.width / 3 - 2, s_uiRect.height), "+"))
            {
                if(value + delta <= max)
                    value += delta;
                else
                    value = max;
                pushButton = true;
            }
            return pushButton;
        }
        public static bool DoubleSpinBox(int col, int row, ref float value, float delta, float bigDelta, float min, float max, int digit = 0, int colSpan = 0, int rowSpan = 0)
        {
            bool pushButton = false;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);
            if (GUI.Button(new Rect(s_uiRect.x + 2, s_uiRect.y, s_uiRect.width / 6 - 2, s_uiRect.height), "<<"))
            {
                if (value - bigDelta >= min)
                    value -= bigDelta;
                else
                    value = min;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6, s_uiRect.y, s_uiRect.width / 6, s_uiRect.height), "<"))
            {
                if (value - delta >= min)
                    value -= delta;
                else
                    value = min;
                pushButton = true;
            }
            GUI.Box(new Rect(s_uiRect.x + s_uiRect.width / 6 * 2, s_uiRect.y, s_uiRect.width / 6 * 2, s_uiRect.height), value.ToString($"F{digit}"), CustomStyle[5]);
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 4, s_uiRect.y, s_uiRect.width / 6, s_uiRect.height), ">"))
            {
                if (value + delta <= max)
                    value += delta;
                else
                    value = max;
                pushButton = true;
            }
            if (GUI.Button(new Rect(s_uiRect.x + s_uiRect.width / 6 * 5, s_uiRect.y, s_uiRect.width / 6 - 2, s_uiRect.height), ">>"))
            {
                if (value + bigDelta <= max)
                    value += bigDelta;
                else
                    value = max;
                pushButton = true;
            }
            return pushButton;
        }
        public static bool HorizontalSelection(int col, int row, ref int selected, string[] texts, int colSpan = 0, int rowSpan = 0)
        {
            bool pushButton = false;
            s_uiRect = GridRect(col, row, colSpan, rowSpan);
            for(int i=0; i<texts.Length; i++)
            {
                if(GUI.Button(new Rect(s_uiRect.x + (s_uiRect.width / texts.Length * i),s_uiRect.y, s_uiRect.width / texts.Length, s_uiRect.height), texts[i], selected == i ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
                {
                    selected = i;
                    pushButton = true;
                }
            }
            return pushButton;
        }
        public static int Toolbar(int col, int row, int selected, string[] texts, int colSpan = 0, int rowSpan = 0)
        {
            return GUI.Toolbar(GridRect(col, row, colSpan, rowSpan), selected, texts);
        }

        public static bool Button(int col, int row, GUIContent content, int colSpan = 0, int rowSpan = 0)
        {
            return GUI.Button(GridRect(col, row, colSpan, rowSpan), content);
        }
        public static bool Button(int col, int row, Texture2D content, int colSpan = 0, int rowSpan = 0)
        {
            return GUI.Button(GridRect(col, row, colSpan, rowSpan), content);
        }
        public static bool Button(int col, int row, string content, int colSpan = 0, int rowSpan = 0)
        {
            return GUI.Button(GridRect(col, row, colSpan, rowSpan), content);
        }

        public static void Box(int col, int row, string content, int colSpan = 0, int rowSpan = 0)
        {
            GUI.Box(GridRect(col, row, colSpan, rowSpan), content);
        }
        public static void Box(int col, int row, string content, GUIStyle style, int colSpan = 0, int rowSpan = 0)
        {
            GUI.Box(GridRect(col, row, colSpan, rowSpan), content, style);
        }
        public static void Label(int col, int row, string content, int colSpan = 0, int rowSpan = 0)
        {
            GUI.Label(GridRect(col, row, colSpan, rowSpan), content, CustomStyle[4]);
        }
    }
}
