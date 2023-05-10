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
            if (Plugin.CameraController.existsVMCAvatar)
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
            if (Plugin.CameraController.webCamDevices.Length > 0)
            {
                GUI.Box(new Rect(menuPos.x, menuPos.y + 145, 300, 150), "WebCamera");
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 170, 80, 25), new GUIContent("<")))
                {
                    if (contextMenu.webCameraPage > 0) contextMenu.webCameraPage--;
                }
                GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 170, 140, 25), new GUIContent($"{contextMenu.webCameraPage + 1} / {Math.Ceiling(Decimal.Parse(contextMenu.webCameraName.Length.ToString()) / 5)}"));
                if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 170, 80, 25), new GUIContent(">")))
                {
                    if (contextMenu.webCameraPage < Math.Ceiling(Decimal.Parse(contextMenu.webCameraName.Length.ToString()) / 5) - 1) contextMenu.webCameraPage++;
                }

                for (int i = contextMenu.webCameraPage * 5; i < contextMenu.webCameraPage * 5 + 5; i++)
                {
                    if (i < contextMenu.webCameraName.Length)
                    {
                        if (!parentBehaviour.webCamScreen && !Plugin.CameraController.inProgressCalibration())
                        {
                            if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - contextMenu.webCameraPage * 5) * 25 + 195, 300, 25), new GUIContent(Plugin.CameraController.webCamDevices[i].name),
                                    Plugin.CameraController.webCamDevices[i].name == parentBehaviour.Config.webCamera.name ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                                parentBehaviour.Config.webCamera.name = Plugin.CameraController.webCamDevices[i].name;
                        }else
                        {
                            GUI.Box(new Rect(menuPos.x, menuPos.y + (i - contextMenu.webCameraPage * 5) * 25 + 195, 300, 25), new GUIContent(Plugin.CameraController.webCamDevices[i].name));
                        }
                    }

                }
                if (!Plugin.CameraController.inProgressCalibration())
                {
                    if (!parentBehaviour.webCamScreen)
                    {
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + 325, 150, 30), "Connect"))
                            parentBehaviour.CreateWebCamScreen();
                    }
                    else
                    {
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + 325, 150, 30), "Disconnect"))
                            parentBehaviour.DisableWebCamScreen();
                    }
                }

                if (!parentBehaviour.webCamScreen)
                {
                    if (!Plugin.CameraController.inProgressCalibration())
                    {
                        if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 325, 150, 30), "Calibration"))
                            Plugin.CameraController.WebCameraCalibration(parentBehaviour);
                    }
                    else
                    {
                        if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 325, 150, 30), "Abort Cal"))
                            Plugin.CameraController.DestroyCalScreen();
                    }
                }
                if (parentBehaviour.webCamScreen)
                    if (GUI.Button(new Rect(menuPos.x , menuPos.y + 355, 300, 30), "Chroma Key"))
                        contextMenu.MenuMode = ContextMenu.MenuState.ChromaKey;
                /*
                GUI.Box(new Rect(menuPos.x, menuPos.y + 385, 300, 45), new GUIContent("Auto Connect"));
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 405, 150, 25), new GUIContent("Enable"), parentBehaviour.Config.webCamera.autoConnect ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.webCamera.autoConnect = true;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 405, 150, 25), new GUIContent("Disable"), !parentBehaviour.Config.webCamera.autoConnect ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.webCamera.autoConnect = false;
                    parentBehaviour.Config.Save();
                }
                */
            }

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close External linkage Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }
    }
}
