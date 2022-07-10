﻿using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuLayout
    {
        internal float amountMove = 0.1f;
        internal float amountRot = 0.1f;

        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 300, 25), new GUIContent("Reset Camera Position and Rotation")))
            {
                parentBehaviour.Config.Position = parentBehaviour.Config.DefaultPosition;
                parentBehaviour.Config.Rotation = parentBehaviour.Config.DefaultRotation;
                parentBehaviour.Config.FirstPersonPositionOffset = parentBehaviour.Config.DefaultFirstPersonPositionOffset;
                parentBehaviour.Config.FirstPersonRotationOffset = parentBehaviour.Config.DefaultFirstPersonRotationOffset;
                parentBehaviour.ThirdPersonPos = parentBehaviour.Config.DefaultPosition;
                parentBehaviour.ThirdPersonRot = parentBehaviour.Config.DefaultRotation;
                parentBehaviour.Config.Save();
                parentBehaviour.CloseContextMenu();
            }

            //Layer
            GUI.Box(new Rect(menuPos.x, menuPos.y + 50, 90, 45), "Layer: " + parentBehaviour.Config.layer);
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 70, 40, 25), new GUIContent("-")))
            {
                parentBehaviour.Config.layer--;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 45, menuPos.y + 70, 40, 25), new GUIContent("+")))
            {
                parentBehaviour.Config.layer++;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //FOV
            GUI.Box(new Rect(menuPos.x + 90, menuPos.y + 50, 90, 45), "FOV: " + parentBehaviour.Config.fov.ToString("F2"));
            if (GUI.Button(new Rect(menuPos.x + 95, menuPos.y + 70, 40, 25), new GUIContent("-")))
            {
                parentBehaviour.Config.fov--;
                parentBehaviour._cam.fieldOfView = parentBehaviour.Config.fov;
                parentBehaviour._cam.orthographicSize = parentBehaviour.Config.fov;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 135, menuPos.y + 70, 40, 25), new GUIContent("+")))
            {
                parentBehaviour.Config.fov++;
                parentBehaviour._cam.fieldOfView = parentBehaviour.Config.fov;
                parentBehaviour._cam.orthographicSize = parentBehaviour.Config.fov;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Fit Canvas
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 50, 120, 45), "Fit to Canvas");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 70, 55, 25), new GUIContent("Enable"), parentBehaviour.Config.fitToCanvas ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.fitToCanvas = true;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 70, 55, 25), new GUIContent("Disable"), !parentBehaviour.Config.fitToCanvas ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.fitToCanvas = false;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }

            //Render Scale
            GUI.Box(new Rect(menuPos.x, menuPos.y + 95, 120, 45), "Render Scale: " + parentBehaviour.Config.renderScale.ToString("F1"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 115, 55, 25), new GUIContent("-")))
            {
                parentBehaviour.Config.renderScale -= 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 115, 55, 25), new GUIContent("+")))
            {
                parentBehaviour.Config.renderScale += 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Mouse Drag
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 95, 120, 45), "Mouse Drag");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 115, 55, 25), new GUIContent("Enable"), parentBehaviour.mouseMoveCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.mouseMoveCamera = true;
                parentBehaviour.mouseMoveCameraSave = true;
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 115, 55, 25), new GUIContent("Disable"), !parentBehaviour.mouseMoveCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.mouseMoveCamera = false;
                parentBehaviour.mouseMoveCameraSave = false;
            }
            //Turn to Head
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 140, 120, 45), "Turn to Head");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 160, 55, 25), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.turnToHead ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.turnToHead = true;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 160, 55, 25), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.turnToHead ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.turnToHead = false;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Turn to Head Horizontal
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 185, 120, 45), "Horizontal Only");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 205, 55, 25), new GUIContent("Enable"), parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal = true;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 205, 55, 25), new GUIContent("Disable"), !parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal = false;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Amount of Movemnet
            GUI.Box(new Rect(menuPos.x, menuPos.y + 185, 175, 45), "Amount movement : " + amountMove.ToString("F2"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 205, 55, 25), new GUIContent("0.01")))
            {
                amountMove = 0.01f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 205, 55, 25), new GUIContent("0.10")))
            {
                amountMove = 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 205, 55, 25), new GUIContent("1.00")))
            {
                amountMove = 1.0f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            //X Position
            GUI.Box(new Rect(menuPos.x, menuPos.y + 230, 100, 45), "X Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posx.ToString("F2") : parentBehaviour.Config.firstPersonPos.x.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 250, 45, 25), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posx -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.x -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 250, 45, 25), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posx += amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.x += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Y Position
            GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 230, 100, 45), "Y Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posy.ToString("F2") : parentBehaviour.Config.firstPersonPos.y.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 250, 45, 25), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posy -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.y -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 250, 45, 25), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posy += amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.y += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Z Position
            GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 230, 100, 45), "Z Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posz.ToString("F2") : parentBehaviour.Config.firstPersonPos.z.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 250, 45, 25), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posz -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.z -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 250, 45, 25), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posz += amountMove;
                else
                    parentBehaviour.Config.firstPersonPos.z += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (parentBehaviour.Config.cameraExtensions.turnToHead && !parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal)
            {
                //Turn to Head Offset
                if (!parentBehaviour.Config.cameraExtensions.turnToHeadHorizontal)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 275, 300, 65), "Turn to Head Offset");
                    //X Position
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 295, 100, 45), $"X Pos:{parentBehaviour.Config.turnToHeadOffsetTransform.x.ToString("F2")}");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 315, 45, 25), new GUIContent("-")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.x -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 315, 45, 25), new GUIContent("+")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.x += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Y Position
                    GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 295, 100, 45), $"Y Pos :{parentBehaviour.Config.turnToHeadOffsetTransform.y.ToString("F2")}");
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 315, 45, 25), new GUIContent("-")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.y -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 315, 45, 25), new GUIContent("+")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.y += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    //Z Position
                    GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 295, 100, 45), $"Z Pos :{parentBehaviour.Config.turnToHeadOffsetTransform.z.ToString("F2")}");
                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 315, 45, 25), new GUIContent("-")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.z -= amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 315, 45, 25), new GUIContent("+")))
                    {
                        parentBehaviour.Config.turnToHeadOffsetTransform.z += amountMove;
                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                }
            }
            else
            {
                //Amount of Rotation
                GUI.Box(new Rect(menuPos.x, menuPos.y + 275, 290, 45), "Amount rotation : " + amountRot.ToString("F2"));
                if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 295, 55, 25), new GUIContent("0.01")))
                {
                    amountRot = 0.01f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 295, 55, 25), new GUIContent("0.10")))
                {
                    amountRot = 0.1f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 295, 55, 25), new GUIContent("1.00")))
                {
                    amountRot = 1.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 170, menuPos.y + 295, 55, 25), new GUIContent("10")))
                {
                    amountRot = 10.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 295, 55, 25), new GUIContent("45")))
                {
                    amountRot = 45.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                //X Rotation
                GUI.Box(new Rect(menuPos.x, menuPos.y + 320, 100, 45), "X Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.thirdPersonRot.x.ToString("F2") : parentBehaviour.Config.firstPersonRot.x.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 340, 45, 25), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.x -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.x -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 340, 45, 25), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.x += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.x += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Y Rotation
                GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 320, 100, 45), "Y Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.thirdPersonRot.y.ToString("F2") : parentBehaviour.Config.firstPersonRot.y.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 340, 45, 25), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.y -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.y -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 340, 45, 25), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.y += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.y += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Z Rotation
                GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 320, 100, 45), "Z Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.thirdPersonRot.z.ToString("F2") : parentBehaviour.Config.firstPersonRot.z.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 340, 45, 25), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.z -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.z -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 340, 45, 25), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.thirdPersonRot.z += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRot.z += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
            }

            if (!parentBehaviour.Config.fitToCanvas)
            {
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 370, 75, 55), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 370, 75, 25), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = Screen.height - (Screen.height / 2);
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 400, 75, 25), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 370, 75, 25), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = Screen.height - (Screen.height / 2);
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 400, 75, 25), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 370, 75, 55), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
            }
            //Close
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Layout Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }

        }
    }
}
