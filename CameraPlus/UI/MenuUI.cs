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
        public static void Initialize()
        {
            IconTexture = new Texture2D[4]
            {
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png"),
                CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png")
            };
            CustomStyle = new GUIStyle[3] 
            {
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.button),
                new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft}
            };
            CustomStyle[0].normal.background = CustomStyle[0].active.background;
            CustomStyle[0].hover.background = CustomStyle[0].active.background;
        }

        public static bool CustomButton(Rect screenRect, string content, bool value = false)
        {
            return GUI.Button(screenRect, content, value ? CustomStyle[0] : CustomStyle[1]);
        }
        public static bool CustomButton(Rect screenRect, Texture2D content)
        {
            return GUI.Button(screenRect, content);
        }
    }
}
