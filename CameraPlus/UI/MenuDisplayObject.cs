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
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 45, 70, 30), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight = false;
                parentBehaviour.Config.Save();
            }
            //Camera Tracking to NoodleExtensions AssignPlayerToTrack
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 25, 150, 50), "Track NoodleExtension");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 45, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 45, 70, 30), new GUIContent("Disbale"), !parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack = false;
                parentBehaviour.Config.Save();
            }

            //Display Preview Quad
            GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 150, 50), "PrevewQuad");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 110, 70, 30), new GUIContent("Display"), parentBehaviour.Config.PreviewCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.PreviewCamera = true;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 110, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.PreviewCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.PreviewCamera = false;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            //Debri
            GUI.Box(new Rect(menuPos.x, menuPos.y + 140, 300, 50), "Display Debri");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 160, 95, 30), new GUIContent("Link InGame"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Link ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Link;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 160, 95, 30), new GUIContent("Forced Display"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Visible ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Visible;
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 160, 95, 30), new GUIContent("Forced Hide"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Hidden ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Hidden;
                parentBehaviour.Config.Save();
            }
            //Display UI
            GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 150, 50), "Display UI");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.ui ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.ui = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 210, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.ui ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.ui = false;
                parentBehaviour.Config.Save();
            }
            //Display Custom, VMC and nalulululuna Avatar
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 190, 150, 50), "Display Avatar");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 210, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.avatar = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 210, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.avatar = false;
                parentBehaviour.Config.Save();
            }
            //Display Saber
            GUI.Box(new Rect(menuPos.x, menuPos.y + 240, 150, 50), "Display Saber");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 260, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.saber = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 260, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.saber = false;
                parentBehaviour.Config.Save();
            }
            //Display Notes
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 240, 150, 50), "Display Notes");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 260, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.notes = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 260, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.notes = false;
                parentBehaviour.Config.Save();
            }
            //TransportWall
            GUI.Box(new Rect(menuPos.x, menuPos.y + 290, 150, 50), "Display Wall Inside");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 310, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.wall ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wall = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 310, 70, 30), new GUIContent("Transport"), !parentBehaviour.Config.layerSetting.wall ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wall = false;
                parentBehaviour.Config.Save();
            }
            //Display Wall Frame
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 290, 150, 50), "Display Wallframe");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 310, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.layerSetting.wallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wallFrame = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 310, 70, 30), new GUIContent("Disable"), !parentBehaviour.Config.layerSetting.wallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wallFrame = false;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close CameraMode Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }
        }
    }
}
