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

        private static Rect _uiRect;
        private static Rect _swRect;

        private static float _menuWidth = 300;
        private static float _menuHeight = 440;

        public static int MenuGridCol = 8;
        public static int MenuGridRow = 16;

        private static float _gridCellWidth { get { return _menuWidth / (MenuGridCol >= 1 ? MenuGridCol : 1); } }
        private static float _gridCellHeight { get { return _menuHeight / (MenuGridRow >= 1 ? MenuGridRow : 1); } }
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
            IconTexture = new Texture2D[6]
            {
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.sw_off.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.sw_on.png")
            };
            CustomStyle = new GUIStyle[4]
            {
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft},
                new GUIStyle(GUI.skin.button) {normal = new GUIStyleState() {textColor = Color.black } },
            };
            CustomStyle[0].normal.background = CustomStyle[0].active.background;
            CustomStyle[0].hover.background = CustomStyle[0].active.background;
            CustomToggleButtonStyle = new GUIStyle[2]
            {
                new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft },
                new GUIStyle(GUI.skin.label)
            };
            _swRect = new Rect(0, 0, IconTexture[4].width, IconTexture[4].height);
            _uiRect = new Rect();
            UIInitialize = true;
        }

        public static bool ToggleSwitch(int col, int row, string content, bool value = false, int colSpan = 0, int rowSpan = 0, float swScale = 0)
        {
            var screenRect = GridRect(col, row, colSpan, rowSpan);
            bool result;
            float scale = (swScale == 0 ? screenRect.height : _gridCellHeight * swScale) / IconTexture[4].height;
            _swRect.width = IconTexture[4].width * scale;
            _swRect.height = IconTexture[4].height * scale;
            _swRect.x = screenRect.x + screenRect.width - _swRect.width + 10;
            _swRect.y = screenRect.y + (swScale == 0 ?  0 : (screenRect.height - _swRect.height) / 2);
            result = GUI.Button(screenRect, content, CustomToggleButtonStyle[0]);
            GUI.Box(_swRect, value ? IconTexture[4] : IconTexture[5], CustomToggleButtonStyle[1]);
            return result;
        }

        public static void SetGrid(int col, int row)
        {
            MenuGridCol = col;
            MenuGridRow = row;
        }

        public static Rect GridRect(int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            _uiRect.width = _gridCellWidth * (colSpan >= 1 ? colSpan : 1);
            _uiRect.height = _gridCellHeight * (rowSpan >= 1 ? rowSpan : 1);
            _uiRect.x = MenuPos.x + _gridCellWidth * col;
            _uiRect.y = MenuPos.y + _gridCellHeight * row + 25;

            return _uiRect;
        }

        public static int SelectionGrid(int col, int row, int selected, ref int currentPage, string[] texts, int colSpan = 0, int rowSpan = 0)
        {
            int selection = selected;
            _uiRect = GridRect(col, row, colSpan, rowSpan);

            if (GUI.Button(new Rect(_uiRect.x, _uiRect.y, _uiRect.width / 3, _uiRect.height / 6), "<"))
                if (currentPage > 0) currentPage--;
            
            GUI.Box(new Rect(_uiRect.x + _uiRect.width / 3, _uiRect.y, _uiRect.width / 3, _uiRect.height / 6), $"{currentPage + 1} / {Math.Ceiling(Decimal.Parse(texts.Length.ToString()) / 5)}");
            
            if (GUI.Button(new Rect(_uiRect.x + _uiRect.width / 3 * 2, _uiRect.y, _uiRect.width / 3, _uiRect.height / 6), ">"))
                if (currentPage < Math.Ceiling(Decimal.Parse(texts.Length.ToString()) / 5) - 1) currentPage++;
            
            for (int i = currentPage * 5; i < currentPage * 5 + 5; i++)
            {
                if (i < texts.Length)
                {
                    if (GUI.Button(new Rect(_uiRect.x, _uiRect.y + _uiRect.height / 6 * (i - currentPage * 5 + 1), _uiRect.width, _uiRect.height / 6), new GUIContent(texts[i]), selection == i ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
                        selection = i;
                }
            }
            return selection;
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
    }
}
