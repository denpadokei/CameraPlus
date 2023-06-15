using UnityEngine;
using CameraPlus.Behaviours;

namespace CameraPlus.UI
{
    internal class MenuMultiplayer
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            //MultiPlayerOffset
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 120), "Multiplayer tracking camera");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 55, 30), new GUIContent("Player1"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 1 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 1;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 65, menuPos.y + 45, 55, 30), new GUIContent("Player2"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 2 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 2;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 125, menuPos.y + 45, 55, 30), new GUIContent("Player3"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 3 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 3;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 45, 55, 30), new GUIContent("Player4"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 4 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 4;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 245, menuPos.y + 45, 55, 30), new GUIContent("Player5"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 5 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 5;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 80, 145, 30), new GUIContent("Extension"), parentBehaviour.Config.multiplayer.targetPlayerNumber > 5 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 6;
                parentBehaviour.Config.Save();
            }
            if (parentBehaviour.Config.multiplayer.targetPlayerNumber > 5)
            {
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 80, 50, 30), new GUIContent("<")))
                {
                    if (parentBehaviour.Config.multiplayer.targetPlayerNumber - 1 > 5)
                        parentBehaviour.Config.multiplayer.targetPlayerNumber--;
                    parentBehaviour.Config.Save();
                }
                GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 80, 50, 30), parentBehaviour.Config.multiplayer.targetPlayerNumber.ToString());
                if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 80, 50, 30), new GUIContent(">")))
                {
                    if (parentBehaviour.Config.multiplayer.targetPlayerNumber + 1 <= 100)
                        parentBehaviour.Config.multiplayer.targetPlayerNumber++;
                    parentBehaviour.Config.Save();
                }
            }

            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 115, 150, 30), new GUIContent("Tracking Camera Off"), parentBehaviour.Config.multiplayer.targetPlayerNumber == 0 ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.targetPlayerNumber = 0;
                parentBehaviour.Config.Save();
            }

            //Display Name, Rand and Score
            GUI.Box(new Rect(menuPos.x, menuPos.y + 170, 300, 55), "Display Multiplayer Name, Rank and Score");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 190, 145, 30), new GUIContent("Show Info"), parentBehaviour.Config.multiplayer.displayPlayerInfo ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.displayPlayerInfo = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 190, 145, 30), new GUIContent("Hide Info"), !parentBehaviour.Config.multiplayer.displayPlayerInfo ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.multiplayer.displayPlayerInfo = false;
                parentBehaviour.Config.Save();
            }

            //Close
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Multiplayer Menu")))
            {
                contextMenu._menuMode = ContextMenu.MenuState.MenuTop;
            }

        }

    }
}
