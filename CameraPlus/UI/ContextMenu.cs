using UnityEngine;
using System;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    public class ContextMenu : MonoBehaviour
    {
        public enum MenuState
        {
            MenuTop,
            CameraSetting,
            PreviewQuad,
            DisplayObject,
            Layout,
            Multiplayer,
            Effect,
            Profile,
            MovementScript,
            SettingConverter,
            ExternalLink,
            ChromaKey
        }

        internal bool showMenu;
        internal MenuState _menuMode = MenuState.MenuTop;
        internal CameraPlusBehaviour _cameraPlus;
        internal string[] scriptName;
        internal int scriptPage = 0;
        internal string[] webCameraName;
        internal int webCameraPage = 0;

        private MenuDisplayObject _menuDisplayObject = new MenuDisplayObject();
        private MenuLayout _menuLayout = new MenuLayout();
        private MenuMultiplayer _menuMultiplayer = new MenuMultiplayer();
        private MenuEffect _menuEffect = new MenuEffect();
        private MenuProfile _menuProfile = new MenuProfile();
        private MenuMovementScript _menuMovementScript = new MenuMovementScript();
        private MenuSettingConverter _menuSettingConverter = new MenuSettingConverter();
        private MenuExternalLink _menuExternalLink = new MenuExternalLink();
        private MenuChromakey _menuChromakey = new MenuChromakey();

        public void EnableMenu(Vector2 mousePos, CameraPlusBehaviour parentBehaviour)
        {
            this.enabled = true;
            MenuUI.MousePosition = mousePos;
            showMenu = true;
            this._cameraPlus = parentBehaviour;
            scriptName = CameraUtilities.MovementScriptList();
            webCameraName = Plugin.cameraController.WebCameraList();
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
            showMenu = false;
        }

        void OnGUI()
        {
            if (!MenuUI.UIInitialize) MenuUI.Initialize();

            if (showMenu)
            {
                Vector3 scale;
                float originalWidth = 1600f;
                float originalHeight = 900f;

                scale.x = Screen.width / originalWidth;
                scale.y = Screen.height / originalHeight;
                scale.z = 1;
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);
                //Layer boxes for Opacity
                for (int i = 0; i < 3; i++)
                    GUI.Box(new Rect(MenuUI.MenuPos.x - 5, MenuUI.MenuPos.y, 310, 470), $"CameraPlus {_cameraPlus.name}");

                switch (_menuMode)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.MenuTop:
                        MenuUI.SetGrid(6, 32);

                        if (MenuUI.ToggleSwitch(0, 0, "Lock\nWindow", !_cameraPlus.Config.cameraLock.lockScreen, 2, 3, 1.5f))
                        {
                            _cameraPlus.Config.cameraLock.lockScreen = !_cameraPlus.Config.cameraLock.lockScreen;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(2, 0, "Grab\nCamera", !_cameraPlus.Config.cameraLock.lockCamera, 2, 3, 1.5f))
                        {
                            _cameraPlus.Config.cameraLock.lockCamera = !_cameraPlus.Config.cameraLock.lockCamera;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(4, 0, "Save\nGrabPos", !_cameraPlus.Config.cameraLock.dontSaveDrag, 2, 3, 1.5f))
                        {
                            _cameraPlus.Config.cameraLock.dontSaveDrag = !_cameraPlus.Config.cameraLock.dontSaveDrag;
                            _cameraPlus.Config.Save();
                        }

                        if (MenuUI.Button(0, 4, "Add New Camera", 3, 4))
                        {
                            lock (Plugin.cameraController.Cameras)
                            {
                                string cameraName = CameraUtilities.GetNextCameraName();
                                Plugin.Log.Notice($"Adding new config with name {cameraName}.json");
                                CameraUtilities.AddNewCamera(cameraName);
                                CameraUtilities.LoadProfile(Plugin.cameraController.CurrentProfile);
                                _cameraPlus.CloseContextMenu();
                            }
                        }
                        if (MenuUI.Button(3, 4, "Duplicate\nSelected Camera", 3, 4))
                        {
                            lock (Plugin.cameraController.Cameras)
                            {
                                string cameraName = CameraUtilities.GetNextCameraName();
                                Plugin.Log.Notice($"Adding {cameraName}");
                                CameraUtilities.AddNewCamera(cameraName, _cameraPlus.Config);
                                CameraUtilities.LoadProfile(Plugin.cameraController.CurrentProfile);
                                _cameraPlus.CloseContextMenu();
                            }
                        }
                        if (MenuUI.Button(3, 8, "Remove\nSelected Camera", 3, 4))
                        {
                            lock (Plugin.cameraController.Cameras)
                            {
                                _cameraPlus._isCameraDestroyed = true;
                                _cameraPlus.CreateScreenRenderTexture();
                                _cameraPlus.CloseContextMenu();
                                Plugin.Log.Notice("Camera removed!");
                            }
                        }

                        if (MenuUI.Button(0, 13, "Camera Setting", 3, 3))
                            _menuMode = MenuState.CameraSetting;

                        if (MenuUI.Button(3, 13, "Preview Camera", 3, 3))
                            _menuMode = MenuState.PreviewQuad;
                        if (MenuUI.Button(0, 16, "Display Object", 3, 3))
                            _menuMode = MenuState.DisplayObject;
                        if (MenuUI.Button(3, 16, "Layout", 3, 3))
                            _menuMode = MenuState.Layout;
                        if (MenuUI.Button(0, 19, "Multiplayer", 3, 3))
                            _menuMode = MenuState.Multiplayer;
                        if (MenuUI.Button(3, 19, "Effect", 3, 3))
                            _menuMode = MenuState.Effect;
                        if (MenuUI.Button(0, 22, "Profile", 3, 3))
                            _menuMode = MenuState.Profile;
                        if (MenuUI.Button(3, 22, "MovementScript", 3, 3))
                            _menuMode = MenuState.MovementScript;
                        if (MenuUI.Button(0, 25, "Setting Converter", 3, 3))
                            _menuMode = MenuState.SettingConverter;
                        if (MenuUI.Button(3, 25, "External linkage", 3, 3))
                            _menuMode = MenuState.ExternalLink;
                        
                        if (MenuUI.Button(0, 30, "Camera Setting", 6, 2))
                            _cameraPlus.CloseContextMenu();
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.CameraSetting:
                        break;
                        /////////////////////////////////////////////////////////////////////////////////////
                }

                /*
                if (MenuMode == MenuState.MenuTop)
                {


                    //First Person, Third Person, 360degree
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 300, 55), "Camera Mode");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 95, 30), new GUIContent("First Person"), !parentBehaviour.Config.thirdPerson ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = false;
                        parentBehaviour.Config.cameraExtensions.follow360map = false;
                        parentBehaviour.ThirdPerson = parentBehaviour.Config.thirdPerson;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 210, 95, 30), new GUIContent("Third Person"), (parentBehaviour.Config.thirdPerson && !parentBehaviour.Config.cameraExtensions.follow360map) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.cameraExtensions.follow360map = false;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 210, 95, 30), new GUIContent("360 degree"), (parentBehaviour.Config.thirdPerson && parentBehaviour.Config.cameraExtensions.follow360map) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.cameraExtensions.follow360map = true;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 250, 145, 40), new GUIContent("Display Object")))
                        MenuMode = MenuState.DisplayObject;
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 250, 145, 40), new GUIContent("Layout")))
                        MenuMode = MenuState.Layout;
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 295, 145, 40), new GUIContent("Multiplayer")))
                        MenuMode = MenuState.Multiplayer;
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 295, 145, 40), new GUIContent("Effect")))
                        MenuMode = MenuState.Effect;
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 340, 145, 40), new GUIContent("Profile Saver")))
                        MenuMode = MenuState.Profile;
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 340, 145, 40), new GUIContent("MovementScript")))
                    {
                        MenuMode = MenuState.MovementScript;
                        scriptName = CameraUtilities.MovementScriptList();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 385, 145, 40), new GUIContent("Setting Converter")))
                    {
                        MenuMode = MenuState.SettingConverter;
                        Camera2ConfigExporter.Init();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 385, 145, 40), new GUIContent("External linkage")))
                        MenuMode = MenuState.ExternalLink;

                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Menu")))
                        parentBehaviour.CloseContextMenu();
                }
                else if (MenuMode == MenuState.DisplayObject)
                    _menuDisplayObject.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Layout)
                    _menuLayout.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Multiplayer)
                    _menuMultiplayer.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Effect)
                    _menuEffect.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Profile)
                    _menuProfile.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.MovementScript)
                    _menuMovementScript.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.SettingConverter)
                    _menuSettingConverter.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.ExternalLink)
                    _menuExternalLink.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.ChromaKey)
                    _menuChromakey.DiplayMenu(parentBehaviour, this, menuPos);
                */
                GUI.matrix = originalMatrix;
            }
        }

        private void UI_SelectionList(float top, float left, ref string[] contentList, ref int currentPageNo)
        {
            /*
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 140, 80, 30), new GUIContent("<")))
            {
                if (currentPageNo > 0) currentPageNo--;
            }
            GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 140, 140, 30), new GUIContent($"{currentPageNo + 1} / {Math.Ceiling(Decimal.Parse(contentList.Length.ToString()) / 5)}"));
            if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 140, 80, 30), new GUIContent(">")))
            {
                if (currentPageNo < Math.Ceiling(Decimal.Parse(contentList.Length.ToString()) / 5) - 1) currentPageNo++;
            }
            for (int i = currentPageNo * 5; i < currentPageNo * 5 + 5; i++)
            {
                if (i < contentList.Length)
                {
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - currentPageNo * 5) * 30 + 170, 300, 30), new GUIContent(contentList[i]), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScript.movementScript) == scriptName[i] ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementScript.movementScript = contentList[i];
                        parentBehaviour.Config.Save();
                        parentBehaviour.AddMovementScript();
                    }
                }
            }
            */
        }
    }
}
