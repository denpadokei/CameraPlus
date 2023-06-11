using UnityEngine;
using System;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;
using IPA.Loader.Features;

namespace CameraPlus.UI
{
    public static class MenuUI
    {
        public static Texture2D[] IconTexture;
        public static GUIStyle[] CustomStyle;
        public static GUIStyle[] CustomToggleButtonStyle;
        public static bool UIInitialize = false;

        private static Rect _swRect;
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
            CustomStyle = new GUIStyle[3]
            {
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft}
            };
            CustomStyle[0].normal.background = CustomStyle[0].active.background;
            CustomStyle[0].hover.background = CustomStyle[0].active.background;
            CustomToggleButtonStyle = new GUIStyle[2]
            {
                new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft },
                new GUIStyle(GUI.skin.label)
            };
            _swRect = new Rect(0, 0, IconTexture[4].width, IconTexture[4].height);
            UIInitialize = true;
        }

        public static bool CustomButton(Rect screenRect, string content, bool value = false)
        {
            return GUI.Button(screenRect, content, value ? CustomStyle[0] : CustomStyle[1]);
        }
        public static bool CustomButton(Rect screenRect, Texture2D content)
        {
            return GUI.Button(screenRect, content);
        }

        public static bool CustomToggleButton(Rect screenRect, string content, bool value = false)
        {
            bool result;
            float scale = screenRect.height / IconTexture[4].height;
            _swRect.width = IconTexture[4].width * scale;
            _swRect.height = IconTexture[4].height * scale;
            _swRect.x = screenRect.x + screenRect.width - _swRect.width + 10;
            _swRect.y = screenRect.y;
            result = GUI.Button(screenRect, content, CustomToggleButtonStyle[0]);
            GUI.Box(_swRect, value ? IconTexture[4] : IconTexture[5], CustomToggleButtonStyle[1]);
            return result;
        }
    }
}
