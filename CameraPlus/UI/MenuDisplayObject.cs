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
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 150, 45), "Force Upright");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 70, 25), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 45, 70, 25), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.firstPersonCameraForceUpRight = false;
                parentBehaviour.Config.Save();
            }
            //Camera Tracking to NoodleExtensions AssignPlayerToTrack
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 25, 150, 45), "Track NoodleExtension");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 45, 70, 25), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 45, 70, 25), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.followNoodlePlayerTrack = false;
                parentBehaviour.Config.Save();
            }

            //Display Preview Quad
            GUI.Box(new Rect(menuPos.x, menuPos.y + 70, 150, 45), "PreviewCamera");
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 90, 50, 25), new GUIContent("Display"), parentBehaviour.Config.PreviewCamera && !parentBehaviour.Config.PreviewCameraQuadOnly ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.PreviewCamera = true;
                parentBehaviour.Config.PreviewCameraQuadOnly = false;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 90, 50, 25), new GUIContent("Hide"), !parentBehaviour.Config.PreviewCamera ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.PreviewCamera = false;
                parentBehaviour.Config.PreviewCameraQuadOnly = false;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 90, 50, 25), new GUIContent("QuadOnly"), parentBehaviour.Config.PreviewCameraQuadOnly ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.PreviewCamera = true;
                parentBehaviour.Config.PreviewCameraQuadOnly = true;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (parentBehaviour.Config.PreviewCamera)
            {
                GUI.Box(new Rect(menuPos.x+150, menuPos.y + 70, 150, 45), "PreviewCameraVROnly");
                if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 90, 70, 25), new GUIContent("VR Only"), parentBehaviour.Config.PreviewCameraVROnly ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
                {
                    parentBehaviour.Config.PreviewCameraVROnly = true;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 90, 70, 25), new GUIContent("DisplayAll"), !parentBehaviour.Config.PreviewCameraVROnly ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
                {
                    parentBehaviour.Config.PreviewCameraVROnly = false;
                    parentBehaviour.Config.Save();
                }
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 115, 150, 95), "PreviewQuadPosition");
            
            if (GUI.Button(new Rect(menuPos.x , menuPos.y + 135, 50, 25), new GUIContent("Separate"), parentBehaviour.Config.cameraExtensions.previewQuadSeparate ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.previewQuadSeparate = true;
                parentBehaviour._quad.SeparateQuad();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 135, 50, 25), new GUIContent("Combine"), !parentBehaviour.Config.cameraExtensions.previewQuadSeparate ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.previewQuadSeparate = false;
                parentBehaviour._quad.CombineQuad();
                parentBehaviour.Config.Save();
            }


            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 135, 50, 25), new GUIContent("Top"), PluginConfig.Instance.CameraQuadPosition == "Top" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadPosition = "Top";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 160, 50, 25), new GUIContent("Left"), PluginConfig.Instance.CameraQuadPosition == "Left" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadPosition = "Left";
                foreach(CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 160, 50, 25), new GUIContent("Center"), PluginConfig.Instance.CameraQuadPosition == "Center" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadPosition = "Center";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 160, 50, 25), new GUIContent("Right"), PluginConfig.Instance.CameraQuadPosition == "Right" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadPosition = "Right";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 185, 50, 25), new GUIContent("Bottom"), PluginConfig.Instance.CameraQuadPosition == "Bottom" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadPosition = "Bottom";
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 115, 150, 55), $"Scale");
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 132, 75, 38), $"Cube:{PluginConfig.Instance.CameraCubeSize.ToString("F1")}");
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 150, 37, 20), new GUIContent("-")))
            {
                PluginConfig.Instance.CameraCubeSize -= 0.1f;
                if (PluginConfig.Instance.CameraCubeSize < 0.1)
                    PluginConfig.Instance.CameraCubeSize = 0.1f;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                {
                    if (cam.gameObject.activeInHierarchy)
                    {
                        cam._quad.SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
                        cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                    }
                }
            }
            if (GUI.Button(new Rect(menuPos.x + 187, menuPos.y + 150, 38, 20), new GUIContent("+")))
            {
                PluginConfig.Instance.CameraCubeSize += 0.1f;
                if (PluginConfig.Instance.CameraCubeSize > 1)
                    PluginConfig.Instance.CameraCubeSize = 1;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                {
                    if (cam.gameObject.activeInHierarchy)
                    {
                        cam._quad.SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
                        cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                    }
                }
            }
            GUI.Box(new Rect(menuPos.x + 225, menuPos.y + 132, 75, 38), $"Quad:{parentBehaviour.Config.cameraExtensions.previewCameraQuadScale.ToString("F1")}");
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 150, 37, 20), new GUIContent("-")))
            {
                parentBehaviour.Config.cameraExtensions.previewCameraQuadScale -= 0.1f;
                if (parentBehaviour.Config.cameraExtensions.previewCameraQuadScale < 0.1)
                    parentBehaviour.Config.cameraExtensions.previewCameraQuadScale = 0.1f;
                parentBehaviour._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 262, menuPos.y + 150, 38, 20), new GUIContent("+")))
            {
                parentBehaviour.Config.cameraExtensions.previewCameraQuadScale += 0.1f;
                parentBehaviour._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 168, 150, 35), $"QuadStretch");
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 185, 75, 20), new GUIContent("Enable"), PluginConfig.Instance.CameraQuadStretch ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadStretch = true;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 185, 75, 20), new GUIContent("Disable"), !PluginConfig.Instance.CameraQuadStretch ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                PluginConfig.Instance.CameraQuadStretch = false;
                foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                    cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
            }
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 203, 150, 35), $"Desktop screen");
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 220, 75, 20), new GUIContent("Enable"), !parentBehaviour.Config.cameraExtensions.dontDrawDesktop ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.dontDrawDesktop = false;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 220, 75, 20), new GUIContent("Disable"), parentBehaviour.Config.cameraExtensions.dontDrawDesktop ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.dontDrawDesktop = true;
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x + 0, menuPos.y + 203, 150, 35), $"Display Mirror");
            if (GUI.Button(new Rect(menuPos.x + 0, menuPos.y + 220, 75, 20), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.previewCameraMirrorMode ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.previewCameraMirrorMode = true;
                parentBehaviour._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 220, 75, 20), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.previewCameraMirrorMode ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.cameraExtensions.previewCameraMirrorMode = false;
                parentBehaviour._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                parentBehaviour.Config.Save();
            }

            //Debri
            GUI.Box(new Rect(menuPos.x, menuPos.y + 240, 300, 45), "Display Debri");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 260, 95, 25), new GUIContent("Link InGame"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Link ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Link;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 260, 95, 25), new GUIContent("Forced Display"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Visible ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Visible;
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 260, 95, 25), new GUIContent("Forced Hide"), parentBehaviour.Config.layerSetting.debris == Configuration.DebriVisibility.Hidden ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.debris = Configuration.DebriVisibility.Hidden;
                parentBehaviour.Config.Save();
            }
            //Display UI
            GUI.Box(new Rect(menuPos.x, menuPos.y + 285, 150, 45), "Display UI");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 305, 70, 25), new GUIContent("Display"), parentBehaviour.Config.layerSetting.ui ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.ui = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 305, 70, 25), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.ui ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.ui = false;
                parentBehaviour.Config.Save();
            }
            //Display Custom, VMC and nalulululuna Avatar
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 285, 150, 45), "Display Avatar");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 305, 70, 25), new GUIContent("Display"), parentBehaviour.Config.layerSetting.avatar ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.avatar = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 305, 70, 25), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.avatar ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.avatar = false;
                parentBehaviour.Config.Save();
            }
            //Display Saber
            GUI.Box(new Rect(menuPos.x, menuPos.y + 330, 150, 45), "Display Saber");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 350, 70, 25), new GUIContent("Display"), parentBehaviour.Config.layerSetting.saber ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.saber = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 350, 70, 25), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.saber ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.saber = false;
                parentBehaviour.Config.Save();
            }
            //Display Notes
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 330, 150, 45), "Display Notes");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 350, 70, 25), new GUIContent("Display"), parentBehaviour.Config.layerSetting.notes ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.notes = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 350, 70, 25), new GUIContent("Hide"), !parentBehaviour.Config.layerSetting.notes ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.notes = false;
                parentBehaviour.Config.Save();
            }
            //TransparentWall
            GUI.Box(new Rect(menuPos.x, menuPos.y + 375, 150, 45), "Display Wall Inside");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 395, 70, 25), new GUIContent("Display"), parentBehaviour.Config.layerSetting.wall ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.wall = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 395, 70, 25), new GUIContent("Trans"), !parentBehaviour.Config.layerSetting.wall ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.wall = false;
                parentBehaviour.Config.Save();
            }
            //Display Wall Frame
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 375, 150, 45), "Display Wallframe");
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 395, 70, 25), new GUIContent("Enable"), parentBehaviour.Config.layerSetting.wallFrame ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.wallFrame = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 395, 70, 25), new GUIContent("Disable"), !parentBehaviour.Config.layerSetting.wallFrame ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.layerSetting.wallFrame = false;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close CameraMode Menu")))
            {
                //._menuMode = ContextMenu.MenuState.MenuTop;
            }
        }
    }
}
