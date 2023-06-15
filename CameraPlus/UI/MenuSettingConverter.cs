using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuSettingConverter
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 120), "Select scene to import from Camera2");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 50, 140, 25), new GUIContent("<")))
                Camera2ConfigExporter.TrySceneSetLast(Camera2ConfigExporter.currentlyScenesSelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 50, 140, 25), new GUIContent(">")))
                Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 80, 230, 60), new GUIContent("Currently Selected:\n" + Camera2ConfigExporter.currentlyScenesSelected)))
                Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);

            GUI.Box(new Rect(menuPos.x, menuPos.y + 150, 300, 120), "Select profile Export to Scene in Camera2");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 175, 140, 25), new GUIContent("<")))
                CameraUtilities.TrySetLast(CameraUtilities.CurrentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 175, 140, 25), new GUIContent(">")))
                CameraUtilities.SetNext(CameraUtilities.CurrentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 205, 230, 60), new GUIContent("Currently Selected:\n" + CameraUtilities.CurrentlySelected)))
                CameraUtilities.SetNext(CameraUtilities.CurrentlySelected);

            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 280, 295, 25), new GUIContent("Export to Selected Scene")))
                Camera2ConfigExporter.ExportCamera2Scene();

            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 310, 295, 25), new GUIContent("Import to New Profile")))
                Camera2ConfigExporter.LoadCamera2Scene();

            //if (GUI.Button(new Rect(menuPos.x + 10, menuPos.y + 360, 280, 40), new GUIContent("Import Setting\nLIV externalcamera.cfg")))

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Setting Convert Menu")))
                contextMenu._menuMode = ContextMenu.MenuState.MenuTop;

        }

    }
}
