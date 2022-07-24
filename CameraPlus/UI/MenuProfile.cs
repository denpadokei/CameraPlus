using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;
using CameraPlus.Configuration;

namespace CameraPlus.UI
{
    internal class MenuProfile
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 140, 25), new GUIContent("<")))
                CameraUtilities.TrySetLast(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 25, 140, 25), new GUIContent(">")))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 50, 230, 50), new GUIContent("Currently Selected:\n" + CameraUtilities.currentlySelected)))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 100, 140, 25), new GUIContent("Save")))
                CameraUtilities.SaveCurrent();
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 100, 140, 25), new GUIContent("Delete")))
            {
                if (!PluginConfig.Instance.ProfileLoadCopyMethod)
                    CameraUtilities.ProfileChange(null);
                CameraUtilities.DeleteProfile(CameraUtilities.currentlySelected);
                CameraUtilities.TrySetLast();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 125, 300, 30), new GUIContent("Load Selected")))
                CameraUtilities.ProfileChange(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 160, 145, 30), new GUIContent("SceneProfile On"), PluginConfig.Instance.ProfileSceneChange ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.ProfileSceneChange = true;
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 160, 145, 30), new GUIContent("SceneProfile Off"), !PluginConfig.Instance.ProfileSceneChange ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                PluginConfig.Instance.ProfileSceneChange = false;
            }

            if (PluginConfig.Instance.ProfileSceneChange)
            {
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 195, 270, 30), "Menu Scene  : " + (PluginConfig.Instance.MenuProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 225, 270, 30), "Game Scene  : " + (PluginConfig.Instance.GameProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 255, 270, 30), "Game 90/360 : " + (PluginConfig.Instance.RotateProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 285, 270, 30), "Multiplayer : " + (PluginConfig.Instance.MultiplayerProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 315, 270, 30), "with SongScript : " + (PluginConfig.Instance.SongSpecificScriptProfile), contextMenu.ProfileStyle);
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 195, 30, 30), "X"))
                {
                    if (PluginConfig.Instance.MenuProfile != string.Empty)
                        PluginConfig.Instance.MenuProfile = string.Empty;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 225, 30, 30), "X"))
                {
                    if (PluginConfig.Instance.GameProfile != string.Empty)
                        PluginConfig.Instance.GameProfile = string.Empty;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 255, 30, 30), "X"))
                {
                    if (PluginConfig.Instance.RotateProfile != string.Empty)
                        PluginConfig.Instance.RotateProfile = string.Empty;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 285, 30, 30), "X"))
                {
                    if (PluginConfig.Instance.MultiplayerProfile != string.Empty)
                        PluginConfig.Instance.MultiplayerProfile = string.Empty;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 315, 30, 30), "X"))
                {
                    if (PluginConfig.Instance.SongSpecificScriptProfile != string.Empty)
                        PluginConfig.Instance.SongSpecificScriptProfile = string.Empty;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 350, 145, 25), new GUIContent("Menu Selected")))
                {
                    PluginConfig.Instance.MenuProfile = CameraUtilities.currentlySelected;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 375, 145, 25), new GUIContent("Game Selected")))
                {
                    PluginConfig.Instance.GameProfile = CameraUtilities.currentlySelected;
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 375, 145, 25), new GUIContent("90/360 Selected")))
                {
                    PluginConfig.Instance.RotateProfile = CameraUtilities.currentlySelected;
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 400, 145, 25), new GUIContent("Multiplay Selected")))
                {
                    PluginConfig.Instance.MultiplayerProfile = CameraUtilities.currentlySelected;
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 400, 145, 25), new GUIContent("SongSciript Selected")))
                {
                    PluginConfig.Instance.SongSpecificScriptProfile = CameraUtilities.currentlySelected;
                }
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Profile Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }

    }
}
