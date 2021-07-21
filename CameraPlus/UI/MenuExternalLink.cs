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
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 65), "VMCProtocol");
            if (parentBehaviour.Config.fitToCanvas)
            {
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 45, 100, 40), new GUIContent("Sender"), parentBehaviour.Config.vmcProtocol.mode == Configuration.VMCProtocolMode.Sender ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                {
                    parentBehaviour.Config.vmcProtocol.mode = Configuration.VMCProtocolMode.Sender;
                    parentBehaviour.Config.Save();
                    parentBehaviour.DestoryVMCProtocolObject();
                    parentBehaviour.InitExternalSender();
                }

            }
            else
                GUI.Box(new Rect(menuPos.x, menuPos.y + 45, 100, 40), new GUIContent("Require\nFitToCanvas"));
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
                GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 150, 45), new GUIContent("Address"));
                var addr = GUI.TextField(new Rect(menuPos.x, menuPos.y + 110, 150, 25), parentBehaviour.Config.vmcProtocol.address);
                if (Regex.IsMatch(addr, ("^" + ipNum + "\\." + ipNum + "\\." + ipNum + "\\." + ipNum + "$")))
                {
                    parentBehaviour.Config.vmcProtocol.address = addr;
                    parentBehaviour.Config.Save();
                }
                GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 90, 150, 45), new GUIContent("Port"));
                var port = GUI.TextField(new Rect(menuPos.x + 150, menuPos.y + 110, 150, 25), parentBehaviour.Config.vmcProtocol.port.ToString());
                if (int.TryParse(port, out int result))
                {
                    parentBehaviour.Config.vmcProtocol.port = result;
                    parentBehaviour.Config.Save();
                }
            }

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close External linkage Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }
    }
}
