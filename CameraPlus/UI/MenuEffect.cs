using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuEffect
    {
        private bool _dofSlider = false;
        private Color _outlineColor;
        private Color _outlineBGColor;
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 140), "Depth Of Field");
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 65, 55, 20), new GUIContent("Enable"), parentBehaviour.Config.DoFEnable ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.DoFEnable = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 55, menuPos.y + 65, 55, 20), new GUIContent("Disbale"), !parentBehaviour.Config.DoFEnable ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.DoFEnable = false;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 105, 55, 20), new GUIContent("Button"), !_dofSlider ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                _dofSlider = false;
            }
            if (GUI.Button(new Rect(menuPos.x + 55, menuPos.y + 105, 55, 20), new GUIContent("Slider"), _dofSlider ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                _dofSlider = true;
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 125, 110, 40), "AutoDistance");
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 145, 55, 20), new GUIContent("Enable"), parentBehaviour.Config.DoFAutoDistance ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.DoFAutoDistance = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 55, menuPos.y + 145, 55, 20), new GUIContent("Disable"), !parentBehaviour.Config.DoFAutoDistance ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.DoFAutoDistance = false;
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x + 110, menuPos.y + 45, 190, 40), $"Focus Distance : {parentBehaviour.effectElements.dofFocusDistance.ToString("F2")}");
            if (!_dofSlider)
            {
                if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 65, 30, 20), new GUIContent("<<")))
                {
                    parentBehaviour.Config.DoFFocusDistance -= 1.0f;
                    if (parentBehaviour.Config.cameraEffect.dofFocusDistance < 0) parentBehaviour.Config.DoFFocusDistance = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 145, menuPos.y + 65, 30, 20), new GUIContent("<-")))
                {
                    parentBehaviour.Config.DoFFocusDistance -= 0.1f;
                    if (parentBehaviour.Config.DoFFocusDistance < 0) parentBehaviour.Config.DoFFocusDistance = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 175, menuPos.y + 65, 30, 20), new GUIContent("<")))
                {
                    parentBehaviour.Config.DoFFocusDistance -= 0.01f;
                    if (parentBehaviour.Config.cameraEffect.dofFocusDistance < 0) parentBehaviour.Config.DoFFocusDistance = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 210, menuPos.y + 65, 30, 20), new GUIContent(">")))
                {
                    parentBehaviour.Config.DoFFocusDistance += 0.01f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 65, 30, 20), new GUIContent("->")))
                {
                    parentBehaviour.Config.DoFFocusDistance += 0.1f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 270, menuPos.y + 65, 30, 20), new GUIContent(">>")))
                {
                    parentBehaviour.Config.DoFFocusDistance += 1.0f;
                    parentBehaviour.Config.Save();
                }
            }
            else
            {
                parentBehaviour.Config.DoFFocusDistance = GUI.HorizontalSlider(new Rect(menuPos.x + 115, menuPos.y + 65, 180, 20), parentBehaviour.Config.DoFFocusDistance, 0, 100);
                parentBehaviour.effectElements.dofFocusDistance = parentBehaviour.Config.DoFFocusDistance;
            }

            GUI.Box(new Rect(menuPos.x + 110, menuPos.y + 85, 190, 40), $"Focus Range : {parentBehaviour.Config.cameraEffect.dofFocusRange.ToString("F2")}");
            if (!_dofSlider)
            {
                if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 105, 30, 20), new GUIContent("<<")))
                {
                    parentBehaviour.Config.DoFFocusRange -= 1.0f;
                    if (parentBehaviour.Config.DoFFocusRange < 0) parentBehaviour.Config.DoFFocusRange = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 145, menuPos.y + 105, 30, 20), new GUIContent("<-")))
                {
                    parentBehaviour.Config.DoFFocusRange -= 0.1f;
                    if (parentBehaviour.Config.DoFFocusRange < 0) parentBehaviour.Config.DoFFocusRange = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 175, menuPos.y + 105, 30, 20), new GUIContent("<")))
                {
                    parentBehaviour.Config.DoFFocusRange -= 0.01f;
                    if (parentBehaviour.Config.DoFFocusRange < 0) parentBehaviour.Config.DoFFocusRange = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 210, menuPos.y + 105, 30, 20), new GUIContent(">")))
                {
                    parentBehaviour.Config.DoFFocusRange += 0.01f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 105, 30, 20), new GUIContent("->")))
                {
                    parentBehaviour.Config.DoFFocusRange += 0.1f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 270, menuPos.y + 105, 30, 20), new GUIContent(">>")))
                {
                    parentBehaviour.Config.DoFFocusRange += 1.0f;
                    parentBehaviour.Config.Save();
                }
            }
            else
            {
                parentBehaviour.Config.DoFFocusRange = GUI.HorizontalSlider(new Rect(menuPos.x + 115, menuPos.y + 105, 180, 20), parentBehaviour.Config.DoFFocusRange, 0, 100);
                parentBehaviour.effectElements.dofFocusRange = parentBehaviour.Config.DoFFocusRange;
            }

            GUI.Box(new Rect(menuPos.x + 110, menuPos.y + 125, 190, 40), $"Blur Size : {parentBehaviour.Config.DoFBlurRadius.ToString("F2")}");
            if (!_dofSlider)
            {
                if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 145, 30, 20), new GUIContent("<<")))
                {
                    parentBehaviour.Config.DoFBlurRadius -= 1.0f;
                    if (parentBehaviour.Config.cameraEffect.dofBlurRadius < 0) parentBehaviour.Config.DoFBlurRadius = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 145, menuPos.y + 145, 30, 20), new GUIContent("<-")))
                {
                    parentBehaviour.Config.DoFBlurRadius -= 0.1f;
                    if (parentBehaviour.Config.DoFBlurRadius < 0) parentBehaviour.Config.DoFBlurRadius = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 175, menuPos.y + 145, 30, 20), new GUIContent("<")))
                {
                    parentBehaviour.Config.DoFBlurRadius -= 0.01f;
                    if (parentBehaviour.Config.cameraEffect.dofBlurRadius < 0) parentBehaviour.Config.DoFBlurRadius = 0;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 210, menuPos.y + 145, 30, 20), new GUIContent(">")))
                {
                    parentBehaviour.Config.DoFBlurRadius += 0.01f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 145, 30, 20), new GUIContent("->")))
                {
                    parentBehaviour.Config.DoFBlurRadius += 0.1f;
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 270, menuPos.y + 145, 30, 20), new GUIContent(">>")))
                {
                    parentBehaviour.Config.DoFBlurRadius += 1.0f;
                    parentBehaviour.Config.Save();
                }
            }
            else
            {
                parentBehaviour.Config.DoFBlurRadius = GUI.HorizontalSlider(new Rect(menuPos.x + 115, menuPos.y + 145, 180, 20), parentBehaviour.Config.DoFBlurRadius, 0, 50);
                parentBehaviour.effectElements.dofBlurRadius = parentBehaviour.Config.DoFBlurRadius;
            }

            //OutlineEffect
            GUI.Box(new Rect(menuPos.x, menuPos.y + 165, 300, 100), "Outline Effect");
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 185, 55, 20), new GUIContent("Enable"), parentBehaviour.Config.OutlineEnable ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.OutlineEnable = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 55, menuPos.y + 185, 55, 20), new GUIContent("Disbale"), !parentBehaviour.Config.OutlineEnable ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.OutlineEnable = false;
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 205, 110, 40), $"OutlineOnly:{parentBehaviour.Config.OutlineOnly.ToString("F2")}");
            parentBehaviour.Config.OutlineOnly = parentBehaviour.effectElements.outlineOnly =
                GUI.HorizontalSlider(new Rect(menuPos.x + 5, menuPos.y + 225, 100, 20), parentBehaviour.Config.OutlineOnly, 0, 1);

            _outlineColor = parentBehaviour.Config.OutlineColor;
            GUI.Box(new Rect(menuPos.x + 120, menuPos.y + 185, 180, 40), "Outline Color");
            GUI.Box(new Rect(menuPos.x + 120, menuPos.y + 205, 60, 20), "R         ");
            _outlineColor.r = GUI.HorizontalSlider(new Rect(menuPos.x + 140, menuPos.y + 210, 40, 20), _outlineColor.r, 0, 1);
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 205, 60, 20), "G         ");
            _outlineColor.g = GUI.HorizontalSlider(new Rect(menuPos.x + 200, menuPos.y + 210, 40, 20), _outlineColor.g, 0, 1);
            GUI.Box(new Rect(menuPos.x + 240, menuPos.y + 205, 60, 20), "B         ");
            _outlineColor.b = GUI.HorizontalSlider(new Rect(menuPos.x + 260, menuPos.y + 210, 40, 20), _outlineColor.b, 0, 1);
            parentBehaviour.Config.OutlineColor = parentBehaviour.effectElements.outlineColor = _outlineColor;

            _outlineBGColor = parentBehaviour.Config.OutlineBackgroundColor;
            GUI.Box(new Rect(menuPos.x + 120, menuPos.y + 225, 180, 40), "background Color");
            GUI.Box(new Rect(menuPos.x + 120, menuPos.y + 245, 60, 20), "R         ");
            _outlineBGColor.r = GUI.HorizontalSlider(new Rect(menuPos.x + 140, menuPos.y + 250, 40, 20), _outlineBGColor.r, 0, 1);
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 245, 60, 20), "G         ");
            _outlineBGColor.g = GUI.HorizontalSlider(new Rect(menuPos.x + 200, menuPos.y + 250, 40, 20), _outlineBGColor.g, 0, 1);
            GUI.Box(new Rect(menuPos.x + 240, menuPos.y + 245, 60, 20), "B         ");
            _outlineBGColor.b = GUI.HorizontalSlider(new Rect(menuPos.x + 260, menuPos.y + 250, 40, 20), _outlineBGColor.b, 0, 1);
            parentBehaviour.Config.OutlineBackgroundColor = parentBehaviour.effectElements.outlineBGColor = _outlineBGColor;

            //Wipe
            GUI.Box(new Rect(menuPos.x, menuPos.y + 265, 300, 120), "Wipe Effect (Check the notes in the manual)");
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 285, 90, 40), new GUIContent("Circle"), parentBehaviour.Config.WipeType == "Circle" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.WipeType = "Circle";
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150 , menuPos.y + 285, 100, 20), new GUIContent("Top to Bottom"), parentBehaviour.Config.WipeType == "Top" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.WipeType = "Top";
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 305, 100, 20), new GUIContent("Left to Right"), parentBehaviour.Config.WipeType == "Left" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.WipeType = "Left";
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 200, menuPos.y + 305, 100, 20), new GUIContent("Right to Left"), parentBehaviour.Config.WipeType == "Right" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.WipeType = "Right";
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 325, 100, 20), new GUIContent("Bottom to Top"), parentBehaviour.Config.WipeType == "Bottom" ? MenuUI.CustomStyle[0] : MenuUI.CustomStyle[1]))
            {
                parentBehaviour.Config.WipeType = "Bottom";
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x, menuPos.y + 325, 150, 60), $"Center x:{parentBehaviour.Config.WipeCircleCenter.x.ToString("F1")}, y:{parentBehaviour.Config.WipeCircleCenter.y.ToString("F1")}");
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 345, 50, 20), new GUIContent("Up")))
            {
                Vector4 pos = new Vector4(parentBehaviour.Config.WipeCircleCenter.x,parentBehaviour.Config.WipeCircleCenter.y,0,0);
                pos.y -= 0.10f;
                if (pos.y < -0.50) pos.y = -0.50f;
                parentBehaviour.Config.WipeCircleCenter = pos;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 365, 50, 20), new GUIContent("Down")))
            {
                Vector4 pos = new Vector4(parentBehaviour.Config.WipeCircleCenter.x, parentBehaviour.Config.WipeCircleCenter.y, 0, 0);
                pos.y += 0.10f;
                if (pos.y > 0.5) pos.y = 0.50f;
                parentBehaviour.Config.WipeCircleCenter = pos;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 355, 50, 20), new GUIContent("Left")))
            {
                Vector4 pos = new Vector4(parentBehaviour.Config.WipeCircleCenter.x, parentBehaviour.Config.WipeCircleCenter.y, 0, 0);
                pos.x -= 0.10f;
                if (pos.x < -0.5) pos.x = -0.50f;
                parentBehaviour.Config.WipeCircleCenter = pos;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 355, 50, 20), new GUIContent("Right")))
            {
                Vector4 pos = new Vector4(parentBehaviour.Config.WipeCircleCenter.x, parentBehaviour.Config.WipeCircleCenter.y, 0, 0);
                pos.x += 0.1f;
                if (pos.x > 0.5) pos.x = 0.5f;
                parentBehaviour.Config.WipeCircleCenter = pos;
                parentBehaviour.Config.Save();
            }
            GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 345, 150, 40), $"Wipe Progress : {parentBehaviour.Config.WipeProgress.ToString("F2")}");
            parentBehaviour.Config.WipeProgress = 
                GUI.HorizontalSlider(new Rect(menuPos.x + 160, menuPos.y + 365, 130, 20), parentBehaviour.Config.WipeProgress, 0, 1);
            parentBehaviour.effectElements.wipeProgress = parentBehaviour.Config.WipeProgress;

            //Close
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Layout Menu")))
            {
                //contextMenu._menuMode = ContextMenu.MenuState.MenuTop;
                parentBehaviour.Config.Save();
            }

        }

    }
}
