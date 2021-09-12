using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Behaviours;

namespace CameraPlus.UI
{
    internal class MenuExternalLink
    {
        private string ipNum = @"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)";
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 45, 100, 40), new GUIContent("Sender"), parentBehaviour.Config.vmcProtocol.mode == Configuration.VMCProtocolMode.Sender ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.vmcProtocol.mode = Configuration.VMCProtocolMode.Sender;
                parentBehaviour.Config.Save();
                parentBehaviour.DestoryVMCProtocolObject();
                parentBehaviour.InitExternalSender();
            }
            if (Plugin.cameraController.existsVMCAvatar)
            {
                if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 45, 100, 40), new GUIContent("Receiver"), parentBehaviour.Config.vmcProtocol.mode == Configuration.VMCProtocolMode.Receiver ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.vmcProtocol.mode = Configuration.VMCProtocolMode.Receiver;
                    parentBehaviour.Config.Save();
                    parentBehaviour.DestoryVMCProtocolObject();
                    parentBehaviour.InitExternalReceiver();
                }
            }
            else
                GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 45, 100, 40), new GUIContent("Require\nVMCAvatar Mod"));

            if (GUI.Button(new Rect(menuPos.x + 200, menuPos.y + 45, 100, 40), new GUIContent("Disable"), parentBehaviour.Config.vmcProtocol.mode == Configuration.VMCProtocolMode.Disable ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.vmcProtocol.mode = Configuration.VMCProtocolMode.Disable;
                parentBehaviour.Config.Save();
                parentBehaviour.DestoryVMCProtocolObject();
            }

            if (parentBehaviour.Config.vmcProtocol.mode == Configuration.VMCProtocolMode.Sender)
            {
                GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 100, 45), new GUIContent("Address"));
                var addr = GUI.TextField(new Rect(menuPos.x, menuPos.y + 110, 100, 25), parentBehaviour.Config.vmcProtocol.address);
                if (Regex.IsMatch(addr, ("^" + ipNum + "\\." + ipNum + "\\." + ipNum + "\\." + ipNum + "$")))
                    parentBehaviour.Config.vmcProtocol.address = addr;
                GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 90, 100, 45), new GUIContent("Port"));
                var port = GUI.TextField(new Rect(menuPos.x + 100, menuPos.y + 110, 100, 25), parentBehaviour.Config.vmcProtocol.port.ToString());
                if (int.TryParse(port, out int result))
                    parentBehaviour.Config.vmcProtocol.port = result;
                if(GUI.Button(new Rect(menuPos.x + 200, menuPos.y + 105, 100, 30), "Save"))
                    parentBehaviour.Config.Save();
            }
            if (Plugin.cameraController.webCamDevices.Length > 0)
            {
                GUI.Box(new Rect(menuPos.x, menuPos.y + 145, 300, 150), "WebCamera");
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 170, 80, 25), new GUIContent("<")))
                {
                    if (contextMenu.webCameraPage > 0) contextMenu.webCameraPage--;
                }
                GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 170, 140, 25), new GUIContent($"{contextMenu.webCameraPage + 1} / {Math.Ceiling(Decimal.Parse(contextMenu.webCameraName.Length.ToString()) / 4)}"));
                if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 170, 80, 25), new GUIContent(">")))
                {
                    if (contextMenu.webCameraPage < Math.Ceiling(Decimal.Parse(contextMenu.webCameraName.Length.ToString()) / 4) - 1) contextMenu.webCameraPage++;
                }

                for (int i = contextMenu.webCameraPage * 4; i < contextMenu.webCameraPage * 4 + 4; i++)
                {
                    if (i < contextMenu.webCameraName.Length)
                    {
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - contextMenu.webCameraPage * 4) * 25 + 195, 300, 25), new GUIContent(Plugin.cameraController.webCamDevices[i].name),
                                Plugin.cameraController.webCamDevices[i].name == parentBehaviour.Config.webCamera.name ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                            parentBehaviour.Config.webCamera.name = Plugin.cameraController.webCamDevices[i].name;
                    }

                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 295, 100, 30), "Connect"))
                {
                    parentBehaviour.CreateWebCamScreen();
                }
                if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 295, 100, 30), "Calibration"))
                {
                    Plugin.cameraController.WebCameraCalibration(parentBehaviour);
                }
                if (GUI.Button(new Rect(menuPos.x + 200, menuPos.y + 295, 100, 30), "Disconnect"))
                {
                    parentBehaviour.DisableWebCamScreen();
                }
                /*
                if (parentBehaviour.webCamScreen)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 325, 300, 120), "ChromaKey");
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 345, 100, 40), "R");
                    parentBehaviour.webCamScreen.ChromakeyR = GUI.HorizontalSlider(new Rect(menuPos.x + 5, menuPos.y + 365, 90, 20), parentBehaviour.webCamScreen.ChromakeyR, 0, 1);
                    GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 345, 100, 40), "G");
                    parentBehaviour.webCamScreen.ChromakeyG = GUI.HorizontalSlider(new Rect(menuPos.x + 105, menuPos.y + 365, 90, 20), parentBehaviour.webCamScreen.ChromakeyG, 0, 1);
                    GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 345, 100, 40), "B");
                    parentBehaviour.webCamScreen.ChromakeyB = GUI.HorizontalSlider(new Rect(menuPos.x + 205, menuPos.y + 365, 90, 20), parentBehaviour.webCamScreen.ChromakeyB, 0, 1);

                    GUI.Box(new Rect(menuPos.x, menuPos.y + 385, 100, 40), "Hue");
                    parentBehaviour.webCamScreen.ChromakeyHue = GUI.HorizontalSlider(new Rect(menuPos.x + 5, menuPos.y + 405, 90, 20), parentBehaviour.webCamScreen.ChromakeyHue, 0, 1);
                    GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 385, 100, 40), "Saturation");
                    parentBehaviour.webCamScreen.ChromakeySaturation = GUI.HorizontalSlider(new Rect(menuPos.x + 105, menuPos.y + 405, 90, 20), parentBehaviour.webCamScreen.ChromakeySaturation, 0, 1);
                    GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 385, 100, 40), "Brightness");
                    parentBehaviour.webCamScreen.ChromakeyBrightness = GUI.HorizontalSlider(new Rect(menuPos.x + 205, menuPos.y + 405, 90, 20), parentBehaviour.webCamScreen.ChromakeyBrightness, 0, 1);
                }
                */

            }

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close External linkage Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }
    }
}
