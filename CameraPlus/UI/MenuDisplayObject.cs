using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Configuration;
using CameraPlus.Utilities;

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
            GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 150, 50), "PreviewCamera");
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
            if (parentBehaviour.Config.PreviewCamera)
            {
                GUI.Box(new Rect(menuPos.x+150, menuPos.y + 90, 150, 50), "PreviewCameraVROnly");
                if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 110, 70, 30), new GUIContent("VR Only"), parentBehaviour.Config.PreviewCameraVROnly ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.PreviewCameraVROnly = true;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 110, 70, 30), new GUIContent("DisplayAll"), !parentBehaviour.Config.PreviewCameraVROnly ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.PreviewCameraVROnly = false;
                    parentBehaviour.Config.Save();
                }
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 140, 180, 80), "PreviewQuadPosition");
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 160, 60, 30), new GUIContent("Top"), PluginConfig.Instance.CameraQuadPosition == "Top" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadPosition = "Top";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 175, 60, 30), new GUIContent("Left"), PluginConfig.Instance.CameraQuadPosition == "Left" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadPosition = "Left";
                foreach(CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 120, menuPos.y + 175, 60, 30), new GUIContent("Right"), PluginConfig.Instance.CameraQuadPosition == "Right" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadPosition = "Right";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 190, 60, 30), new GUIContent("Bottom"), PluginConfig.Instance.CameraQuadPosition == "Bottom" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadPosition = "Bottom";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 140, 120, 40), $"CubeScale : {PluginConfig.Instance.CameraCubeSize.ToString("F2")}");
            if (GUI.Button(new Rect(menuPos.x + 180, menuPos.y + 160, 60, 20), new GUIContent("-")))
            {
                PluginConfig.Instance.CameraCubeSize -= 0.1f;
                if (PluginConfig.Instance.CameraCubeSize < 0.1)
                    PluginConfig.Instance.CameraCubeSize = 0.1f;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                {
                    cam._quad.SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                }
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 160, 60, 20), new GUIContent("+")))
            {
                PluginConfig.Instance.CameraCubeSize += 0.1f;
                if (PluginConfig.Instance.CameraCubeSize > 1)
                    PluginConfig.Instance.CameraCubeSize = 1;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                {
                    cam._quad.SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                }
            }
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 180, 120, 40), $"QuadStretch");
            if (GUI.Button(new Rect(menuPos.x + 180, menuPos.y + 200, 60, 20), new GUIContent("Enable"), PluginConfig.Instance.CameraQuadStretch ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadStretch = true;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 200, 60, 20), new GUIContent("Disable"), !PluginConfig.Instance.CameraQuadStretch ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.CameraQuadStretch = false;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            //Debri
            GUI.Box(new Rect(menuPos.x, menuPos.y + 220, 300, 50), "Display Debri");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 240, 95, 30), new GUIContent("Link InGame"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Link ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Link;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 240, 95, 30), new GUIContent("Forced Display"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Visible ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Visible;
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 240, 95, 30), new GUIContent("Forced Hide"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Hidden ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Hidden;
                parentBehaviour.Config.Save();
            }
            //Display UI
            GUI.Box(new Rect(menuPos.x, menuPos.y + 270, 150, 50), "Display UI");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 290, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.ui ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.ui = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 290, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.ui ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.ui = false;
                parentBehaviour.Config.Save();
            }
            //Display Custom, VMC and nalulululuna Avatar
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 270, 150, 50), "Display Avatar");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 290, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.avatar = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 290, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.avatar = false;
                parentBehaviour.Config.Save();
            }
            //Display Saber
            GUI.Box(new Rect(menuPos.x, menuPos.y + 320, 150, 50), "Display Saber");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 340, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.saber = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 340, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.saber ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.saber = false;
                parentBehaviour.Config.Save();
            }
            //Display Notes
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 320, 150, 50), "Display Notes");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 340, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.notes = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 340, 70, 30), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.notes ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.notes = false;
                parentBehaviour.Config.Save();
            }
            //TransparentWall
            GUI.Box(new Rect(menuPos.x, menuPos.y + 370, 150, 50), "Display Wall Inside");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 390, 70, 30), new GUIContent("Display"), parentBehaviour.Config.layerSetting.wall ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wall = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 390, 70, 30), new GUIContent("Trans"), !parentBehaviour.Config.layerSetting.wall ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wall = false;
                parentBehaviour.Config.Save();
            }
            //Display Wall Frame
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 370, 150, 50), "Display Wallframe");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 390, 70, 30), new GUIContent("Enable"), parentBehaviour.Config.layerSetting.wallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.layerSetting.wallFrame = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 390, 70, 30), new GUIContent("Disable"), !parentBehaviour.Config.layerSetting.wallFrame ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
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
