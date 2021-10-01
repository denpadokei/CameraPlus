using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuChromakey
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 25), "ChromaKey");

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 50, 300, 30), new GUIContent("Pick Chroma Key Color")))
            {
                parentBehaviour.webCamScreen.colorPickState = true;
                Texture2D texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ColorPick.png"); ;
                UnityEngine.Cursor.SetCursor(texture, new Vector2(0, 0), CursorMode.Auto);
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 80, 300, 40), "R");
            parentBehaviour.webCamScreen.ChromakeyR = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 100, 300, 20), parentBehaviour.webCamScreen.ChromakeyR, 0, 1);

            GUI.Box(new Rect(menuPos.x, menuPos.y + 120, 300, 40), "G");
            parentBehaviour.webCamScreen.ChromakeyG = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 140, 300, 20), parentBehaviour.webCamScreen.ChromakeyG, 0, 1);

            GUI.Box(new Rect(menuPos.x, menuPos.y + 160, 300, 40), "B");
            parentBehaviour.webCamScreen.ChromakeyB = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 180, 300, 20), parentBehaviour.webCamScreen.ChromakeyB, 0, 1);

            GUI.Box(new Rect(menuPos.x, menuPos.y + 210, 300, 40), "Hue");
            parentBehaviour.webCamScreen.ChromakeyHue = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 230, 300, 20), parentBehaviour.webCamScreen.ChromakeyHue, 0, 1);
            GUI.Box(new Rect(menuPos.x, menuPos.y + 250, 300, 40), "Saturation");
            parentBehaviour.webCamScreen.ChromakeySaturation = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 270, 300, 20), parentBehaviour.webCamScreen.ChromakeySaturation, 0, 1);
            GUI.Box(new Rect(menuPos.x, menuPos.y + 290, 300, 40), "Brightness");
            parentBehaviour.webCamScreen.ChromakeyBrightness = GUI.HorizontalSlider(new Rect(menuPos.x, menuPos.y + 310, 300, 20), parentBehaviour.webCamScreen.ChromakeyBrightness, 0, 1);


            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Chroma Key Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.ExternalLink;
        }
    }
}
