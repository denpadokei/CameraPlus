using UnityEngine;
using CameraPlus.Behaviours;

namespace CameraPlus.UI
{
    internal class MenuDisplayObject
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour,ContextMenu contextMenu, Vector2 menuPos)
        {
            //FirstPerson Camera Upright
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 150, 50), "Force Upright");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.forceFirstPersonUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.forceFirstPersonUpRight = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 45, 70, 30), new GUIContent("Disable"), !parentBehaviour.Config.forceFirstPersonUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.forceFirstPersonUpRight = false;
                parentBehaviour.Config.Save();
            }
            //Camera Tracking to NoodleExtensions AssignPlayerToTrack
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 25, 150, 50), "Track NoodleExtension");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 45, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.NoodleTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.NoodleTrack = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 45, 70, 30), new GUIContent("Disbale"), !parentBehaviour.Config.NoodleTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.NoodleTrack = false;
                parentBehaviour.Config.Save();
            }

            //Display Preview Quad
            GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 150, 50), "PrevewQuad");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 110, 70, 30), new GUIContent("Display"), parentBehaviour.Config.showThirdPersonCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.showThirdPersonCamera = true;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 110, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.showThirdPersonCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.showThirdPersonCamera = false;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            //Debri
            GUI.Box(new Rect(menuPos.x, menuPos.y + 140, 300, 50), "Display Debri");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 160, 95, 30), new GUIContent("Link InGame"), parentBehaviour.Config.debri == "link" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "link";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 160, 95, 30), new GUIContent("Forced Display"), parentBehaviour.Config.debri == "show" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "show";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 160, 95, 30), new GUIContent("Forced Hide"), parentBehaviour.Config.debri == "hide" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "hide";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //Display UI
            GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 150, 50), "Display UI");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 70, 30), new GUIContent("Display"), !parentBehaviour.Config.HideUI ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.HideUI = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 210, 70, 30), new GUIContent("Hide"), parentBehaviour.Config.HideUI ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.HideUI = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //Display Custom, VMC and nalulululuna Avatar
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 190, 150, 50), "Display Avatar");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 210, 70, 30), new GUIContent("Display"), parentBehaviour.Config.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.avatar = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 210, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.avatar = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //Display Saber
            GUI.Box(new Rect(menuPos.x, menuPos.y + 240, 150, 50), "Display Saber");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 260, 70, 30), new GUIContent("Display"), parentBehaviour.Config.Saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.Saber = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 260, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.Saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.Saber = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //Display Notes
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 240, 150, 50), "Display Notes");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 260, 70, 30), new GUIContent("Display"), parentBehaviour.Config.Notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.Notes = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 260, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.Notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.Notes = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //TransportWall
            GUI.Box(new Rect(menuPos.x, menuPos.y + 290, 150, 50), "Transport Wall");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 310, 70, 30), new GUIContent("Display"), !parentBehaviour.Config.transparentWalls ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.transparentWalls = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 310, 70, 30), new GUIContent("Transport"), parentBehaviour.Config.transparentWalls ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.transparentWalls = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            //Display Wall Frame
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 290, 150, 50), "Display Wallframe");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 310, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.WallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.WallFrame = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 310, 70, 30), new GUIContent("Disable"), !parentBehaviour.Config.WallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.WallFrame = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close CameraMode Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }
        }
    }
}
