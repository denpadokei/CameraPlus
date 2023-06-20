using UnityEngine;
using System;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;
using CameraPlus.Configuration;
using System.IO;

namespace CameraPlus.UI
{
    public class ContextMenu : MonoBehaviour
    {
        public enum MenuState
        {
            MenuTop,
            CameraSetting,
            PreviewQuad,
            Effect,
            Profile,
            MovementScript,
            SettingConverter,
            ExternalLink,
            ChromaKey
        }
        public enum CameraSettingState
        {
            Setting,
            Position,
            Visibility
        }

        public enum EffectSettingState
        {
            DoF,
            Wipe,
            Outline,
            Glitch
        }

        private bool _showMenu;
        private MenuState _menuMode = MenuState.MenuTop;
        private CameraSettingState _settingState = CameraSettingState.Setting;
        private EffectSettingState _effectState = EffectSettingState.DoF;

        internal CameraPlusBehaviour _cameraPlus;

        private int _selectedConfig = 0;
        private string[] _configList;
        private int _currentConfigPage;

        private int _selectedProfile = 0;
        private string[] _profileNameList;
        private int _currentProfilePage = 0;

        private int _selectedScript = 0;
        private string[] _scriptList;
        private int _currentScriptPage;

        private int _selectedWebCam = 0;
        private string[] _webCamList;
        private int _currentWebCamPage = 0;

        private int _selectedMultiplayerNum = 0;
        private string[] _multiplayerList = new string[] { "Player1", "Player2", "Player3", "Player4", "Player5" };

        public void EnableMenu(Vector2 mousePos, CameraPlusBehaviour parentBehaviour)
        {
            this.enabled = true;
            MenuUI.MousePosition = mousePos;
            _showMenu = true;
            this._cameraPlus = parentBehaviour;

            _profileNameList = CameraUtilities.ProfileList();
            _scriptList = CameraUtilities.MovementScriptList();
            _selectedScript = SelectedListNumber(_scriptList, _cameraPlus.Config.movementScript.movementScript);

            _configList = CameraUtilities.CameraSettingList(Plugin.cameraController.CurrentProfile);
            _selectedConfig = SelectedListNumber(_configList, Path.GetFileName(_cameraPlus.Config.FilePath));

            _webCamList = Plugin.cameraController.WebCameraList();
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
            _showMenu = false;
        }

        private int SelectedListNumber(string[] list, string selectedName)
        {
            for(int i=0; i<list.Length; i++)
            {
                if (list[i] == selectedName)
                    return i;
            }
            return 0;
        }

        void OnGUI()
        {
            if (!MenuUI.UIInitialize) MenuUI.Initialize();

            if (_showMenu)
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
                    GUI.Box(new Rect(MenuUI.MenuPos.x - 5, MenuUI.MenuPos.y, 310, 470), _cameraPlus.name);

                switch (_menuMode)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.MenuTop:
                        MenuUI.SetGrid(6, 43);

                        if (MenuUI.ToggleSwitch(0, 0, "Lock\nWindow", _cameraPlus.Config.cameraLock.lockScreen, 2, 3, 2))
                        {
                            _cameraPlus.Config.cameraLock.lockScreen = !_cameraPlus.Config.cameraLock.lockScreen;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(2, 0, "Grab\nCamera", !_cameraPlus.Config.cameraLock.lockCamera, 2, 3, 2))
                        {
                            _cameraPlus.Config.cameraLock.lockCamera = !_cameraPlus.Config.cameraLock.lockCamera;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(4, 0, "Save\nGrabPos", !_cameraPlus.Config.cameraLock.dontSaveDrag, 2, 3, 2))
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
                                Plugin.cameraController.CloseContextMenu();
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
                                Plugin.cameraController.CloseContextMenu();
                            }
                        }
                        if (MenuUI.Button(3, 8, "Remove\nSelected Camera", 3, 4))
                        {
                            if (CameraUtilities.RemoveCamera(_cameraPlus))
                            {
                                _cameraPlus._isCameraDestroyed = true;
                                Plugin.cameraController.CloseContextMenu();
                                Plugin.Log.Notice("Camera removed!");
                            }
                        }

                        if (MenuUI.Button(0, 13, "Camera Setting", 3, 3))
                            _menuMode = MenuState.CameraSetting;
                        if (MenuUI.Button(3, 13, "Preview Camera", 3, 3))
                            _menuMode = MenuState.PreviewQuad;
                        if (MenuUI.Button(3, 16, "External linkage", 3, 3))
                            _menuMode = MenuState.ExternalLink;
                        if (MenuUI.Button(0, 19, "MovementScript", 3, 3))
                            _menuMode = MenuState.MovementScript;
                        if (MenuUI.Button(3, 19, "Effect", 3, 3))
                            _menuMode = MenuState.Effect;

                        if (MenuUI.Button(0, 23, "Profile", 6, 3))
                            _menuMode = MenuState.Profile;

                        _selectedConfig = MenuUI.SelectionGrid(0, 27, _selectedConfig, ref _currentConfigPage, _configList, Path.GetFileName(_cameraPlus.Config.FilePath), 4, 12);
                        if (MenuUI.Button(4, 27, "Change\ntarget\nconfig", 2, 5))
                        {
                            foreach(var c in Plugin.cameraController.Cameras.Values)
                            {
                                if (c.gameObject.activeInHierarchy && Path.GetFileName(c.Config.FilePath) == _configList[_selectedConfig])
                                    _cameraPlus = c;
                            }
                        }

                        if (MenuUI.Button(4, 35, "Setting\nConverter", 2, 4))
                            _menuMode = MenuState.SettingConverter;

                        if (MenuUI.Button(0, 40, "Close Menu", 6, 3))
                            Plugin.cameraController.CloseContextMenu();
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.CameraSetting:
                        MenuUI.SetGrid(12, 34);
                        _settingState = (CameraSettingState)Enum.ToObject(typeof(CameraSettingState), MenuUI.Toolbar(0, 0, (int)_settingState, Enum.GetNames(typeof(CameraSettingState)), 12, 2));
                        switch (_settingState)
                        {
                            case CameraSettingState.Setting:
                                if (MenuUI.ToggleSwitch(0, 3, "Third person", _cameraPlus.Config.thirdPerson, 6, 2, 1.5f))
                                {
                                    _cameraPlus.Config.thirdPerson = !_cameraPlus.Config.thirdPerson;
                                    _cameraPlus.Config.Save();
                                }
                                if (_cameraPlus.Config.thirdPerson)
                                {
                                    if (MenuUI.ToggleSwitch(0, 5, "Follow Noodle", _cameraPlus.Config.cameraExtensions.followNoodlePlayerTrack, 6, 2, 1.5f))
                                    {
                                        _cameraPlus.Config.cameraExtensions.followNoodlePlayerTrack = !_cameraPlus.Config.cameraExtensions.followNoodlePlayerTrack;
                                        _cameraPlus.Config.Save();
                                    }
                                    if (MenuUI.ToggleSwitch(6, 5, "Follow 90/360", _cameraPlus.Config.cameraExtensions.follow360map, 6, 2, 1.5f))
                                    {
                                        _cameraPlus.Config.cameraExtensions.follow360map = !_cameraPlus.Config.cameraExtensions.follow360map;
                                        _cameraPlus.Config.Save();
                                    }
                                }
                                else
                                {
                                    if (MenuUI.ToggleSwitch(0, 5, "Force Upright", _cameraPlus.Config.cameraExtensions.firstPersonCameraForceUpRight, 6, 2, 1.5f))
                                    {
                                        _cameraPlus.Config.cameraExtensions.firstPersonCameraForceUpRight = !_cameraPlus.Config.cameraExtensions.firstPersonCameraForceUpRight;
                                        _cameraPlus.Config.Save();
                                    }
                                }

                                if (MenuUI.ToggleSwitch(0, 10, "Desktop screen", !_cameraPlus.Config.cameraExtensions.dontDrawDesktop, 6, 2, 1.5f))
                                {
                                    _cameraPlus.Config.DontDrawDesktop = !_cameraPlus.Config.DontDrawDesktop;
                                    _cameraPlus.Config.Save();
                                }

                                if (MenuUI.ToggleSwitch(0, 25, "Multiplayer", _cameraPlus.Config.multiplayer.targetPlayerNumber != 0, 6, 2, 1.5f))
                                {
                                    if (_cameraPlus.Config.multiplayer.targetPlayerNumber == 0)
                                        _cameraPlus.Config.multiplayer.targetPlayerNumber = 1;
                                    else
                                        _cameraPlus.Config.multiplayer.targetPlayerNumber = 0;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(6, 25, "Display info", _cameraPlus.Config.multiplayer.displayPlayerInfo, 6, 2, 1.5f))
                                {
                                    _cameraPlus.Config.multiplayer.displayPlayerInfo = !_cameraPlus.Config.multiplayer.displayPlayerInfo;
                                    _cameraPlus.Config.Save();
                                }

                                _selectedMultiplayerNum = _cameraPlus.Config.multiplayer.targetPlayerNumber - 1;
                                if (MenuUI.HorizontalSelection(0, 27, ref _selectedMultiplayerNum, _multiplayerList, 12, 2))
                                {
                                    _cameraPlus.Config.multiplayer.targetPlayerNumber = _selectedMultiplayerNum + 1;
                                    _cameraPlus.Config.Save();
                                }
                                MenuUI.Box(0, 29, "Number Extension", 6, 2);
                                if(MenuUI.Button(6, 29, "<", 2, 2))
                                {
                                    if(_cameraPlus.Config.multiplayer.targetPlayerNumber > 0)
                                        _cameraPlus.Config.multiplayer.targetPlayerNumber -= 1;
                                    _cameraPlus.Config.Save();
                                }
                                MenuUI.Box(8, 29, _cameraPlus.Config.multiplayer.targetPlayerNumber > 0 ? _cameraPlus.Config.multiplayer.targetPlayerNumber.ToString() : "-", 2, 2);
                                if (MenuUI.Button(10, 29, ">", 2, 2))
                                {
                                    if (_cameraPlus.Config.multiplayer.targetPlayerNumber <= 100)
                                        _cameraPlus.Config.multiplayer.targetPlayerNumber += 1;
                                    _cameraPlus.Config.Save();
                                }

                                break;
                            case CameraSettingState.Position:

                                break;
                            case CameraSettingState.Visibility:
                                if (MenuUI.ToggleSwitch(0, 3, "Avatar", _cameraPlus.Config.Avatar, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.Avatar = !_cameraPlus.Config.Avatar;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(0, 6, "Saber", _cameraPlus.Config.Saber, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.Saber = !_cameraPlus.Config.Saber;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(0, 9, "Notes", _cameraPlus.Config.Notes, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.Notes = !_cameraPlus.Config.Notes;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(0, 12, "Debri - LinkGame", _cameraPlus.Config.Debris == DebriVisibility.Link, 6, 3, 1.5f))
                                {
                                    if (_cameraPlus.Config.Debris == DebriVisibility.Link)
                                        _cameraPlus.Config.Debris = DebriVisibility.Visible;
                                    else
                                        _cameraPlus.Config.Debris = DebriVisibility.Link;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(6, 12, "Debri - AlwaysOn", _cameraPlus.Config.Debris == DebriVisibility.Visible, 6, 3, 1.5f))
                                {
                                    if (_cameraPlus.Config.Debris == DebriVisibility.Visible)
                                        _cameraPlus.Config.Debris = DebriVisibility.Hidden;
                                    else
                                        _cameraPlus.Config.Debris = DebriVisibility.Visible;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(0, 15, "Wall inside", _cameraPlus.Config.Wall, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.Wall = !_cameraPlus.Config.Wall;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(6, 15, "Wall inside", _cameraPlus.Config.WallFrame, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.WallFrame = !_cameraPlus.Config.WallFrame;
                                    _cameraPlus.Config.Save();
                                }
                                if (MenuUI.ToggleSwitch(0, 18, "UI", _cameraPlus.Config.UI, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.UI = !_cameraPlus.Config.UI;
                                    _cameraPlus.Config.Save();
                                }

                                if (MenuUI.ToggleSwitch(0, 22, "PreviewCamera", _cameraPlus.Config.PreviewCamera, 6, 3, 1.5f))
                                {
                                    _cameraPlus.Config.PreviewCamera = !_cameraPlus.Config.PreviewCamera;
                                    _cameraPlus.Config.Save();
                                }
                                if (_cameraPlus.Config.PreviewCamera)
                                {
                                    if (MenuUI.ToggleSwitch(0, 25, "Hide Cube", _cameraPlus.Config.PreviewCameraQuadOnly, 6, 3, 1.5f))
                                    {
                                        _cameraPlus.Config.PreviewCameraQuadOnly = !_cameraPlus.Config.PreviewCameraQuadOnly;
                                        _cameraPlus.Config.Save();
                                    }
                                    if (MenuUI.ToggleSwitch(6, 25, "Show VR only", _cameraPlus.Config.PreviewCameraVROnly, 6, 3, 1.5f))
                                    {
                                        _cameraPlus.Config.PreviewCameraVROnly = !_cameraPlus.Config.PreviewCameraVROnly;
                                        _cameraPlus.Config.Save();
                                    }
                                }

                                break;
                        }
                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.PreviewQuad:
                        MenuUI.SetGrid(12, 34);

                        MenuUI.Label(0, 0, "Preview Position", 6, 2);
                        var quadPos = PluginConfig.Instance.CameraQuadPosition == string.Empty ? "Right" : PluginConfig.Instance.CameraQuadPosition;
                        if (MenuUI.CrossSelection(0, 2, ref quadPos, 7, 7))
                        {
                            PluginConfig.Instance.CameraQuadPosition = quadPos;
                            foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                                cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                        }
                        if (MenuUI.ToggleSwitch(7, 2, "Preview\nstretch", PluginConfig.Instance.CameraQuadStretch, 5, 3, 1.5f))
                        {
                            PluginConfig.Instance.CameraQuadStretch = !PluginConfig.Instance.CameraQuadStretch;
                            foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                                cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                        }

                        MenuUI.Label(0, 9, "Cube Scale", 6, 2);
                        var cubeSize = PluginConfig.Instance.CameraCubeSize;
                        if(MenuUI.SpinBox(0, 11, ref cubeSize, 0.1f, 0.1f, 1, 1, 6, 2))
                        {
                            PluginConfig.Instance.CameraCubeSize = cubeSize;
                            foreach (CameraPlusBehaviour cam in Plugin.cameraController.Cameras.Values)
                            {
                                cam._quad.SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
                                cam._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                            }
                        }

                        MenuUI.Label(6, 9, "Preview Scale", 6, 2);
                        var quadScale = _cameraPlus.Config.cameraExtensions.previewCameraQuadScale;
                        if (MenuUI.DoubleSpinBox(6, 11, ref quadScale, 0.1f, 1.0f, 0.1f, 100, 1, 6, 2))
                        {
                            _cameraPlus.Config.cameraExtensions.previewCameraQuadScale = quadScale;
                            _cameraPlus.Config.Save();
                            _cameraPlus._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                        }

                        if (MenuUI.ToggleSwitch(0, 14, "Preview camera", _cameraPlus.Config.PreviewCamera, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.PreviewCamera = !_cameraPlus.Config.PreviewCamera;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(6, 14, "Mirror mode", _cameraPlus.Config.cameraExtensions.previewCameraMirrorMode, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.cameraExtensions.previewCameraMirrorMode = !_cameraPlus.Config.cameraExtensions.previewCameraMirrorMode;
                            _cameraPlus._quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(0, 16, "Hide Cube", _cameraPlus.Config.PreviewCameraQuadOnly, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.PreviewCameraQuadOnly = !_cameraPlus.Config.PreviewCameraQuadOnly;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(6, 16, "Show VR only", _cameraPlus.Config.PreviewCameraVROnly, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.PreviewCameraVROnly = !_cameraPlus.Config.PreviewCameraVROnly;
                            _cameraPlus.Config.Save();
                        }

                        if (MenuUI.ToggleSwitch(0, 19, "Preview separate", _cameraPlus.Config.cameraExtensions.previewQuadSeparate, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.cameraExtensions.previewQuadSeparate = !_cameraPlus.Config.cameraExtensions.previewQuadSeparate;
                            _cameraPlus.Config.Save();
                        }
                        if (_cameraPlus.Config.cameraExtensions.previewQuadSeparate)
                        {

                        }

                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.ExternalLink:
                        MenuUI.SetGrid(12, 34);


                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.MovementScript:
                        MenuUI.SetGrid(6, 34);
                        if (MenuUI.ToggleSwitch(0, 0, "Use audio sync", _cameraPlus.Config.movementScript.useAudioSync, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.movementScript.useAudioSync = !_cameraPlus.Config.movementScript.useAudioSync;
                            _cameraPlus.Config.Save();
                        }
                        if (MenuUI.ToggleSwitch(0, 2, "Song specific script", _cameraPlus.Config.movementScript.songSpecificScript, 6, 2, 1.5f))
                        {
                            _cameraPlus.Config.movementScript.songSpecificScript = !_cameraPlus.Config.movementScript.songSpecificScript;
                            _cameraPlus.Config.Save();
                        }

                        _selectedScript = MenuUI.SelectionGrid(0, 5, _selectedScript, ref _currentScriptPage, _scriptList, 
                            (_cameraPlus.Config.movementScript.movementScript == string.Empty ? "[ MovementScript Off ]" : Path.GetFileName(_cameraPlus.Config.movementScript.movementScript)), 6, 12);

                        if (MenuUI.Button(0, 18, "Set MovementScript", 6, 2))
                        {
                            if (_selectedScript == 0)
                            {
                                _cameraPlus.Config.movementScript.movementScript = string.Empty;
                                _cameraPlus.ClearMovementScript();
                            }
                            else
                            {
                                _cameraPlus.Config.movementScript.movementScript = _scriptList[_selectedScript];
                                _cameraPlus.AddMovementScript();
                            }
                            _cameraPlus.Config.Save();
                        }
                        MenuUI.Box(0, 21, "Ignore script visibility setting", 6, 10);
                        if (MenuUI.ToggleSwitch(0, 23, "UI", _cameraPlus.Config.movementScript.ignoreScriptUIDisplay, 3, 2, 1.5f))
                        {
                            _cameraPlus.Config.movementScript.ignoreScriptUIDisplay = !_cameraPlus.Config.movementScript.ignoreScriptUIDisplay;
                            _cameraPlus.Config.Save();
                        }

                        if (MenuUI.Button(0, 32, "Back top menu", 6, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.Effect:
                        MenuUI.SetGrid(12, 34);
                        _effectState = (EffectSettingState)Enum.ToObject(typeof(EffectSettingState), MenuUI.Toolbar(0, 0, (int)_effectState, Enum.GetNames(typeof(EffectSettingState)), 12, 2));


                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.Profile:
                        MenuUI.SetGrid(12, 34);

                        if(MenuUI.Button(0, 0, "Create\nnew profile", 4, 3))
                        {
                            CameraUtilities.SaveNewProfile();
                            _selectedProfile = 0;
                            _profileNameList = CameraUtilities.ProfileList();
                        }
                        if(MenuUI.Button(4, 0, "Save as\ncurrent profile", 4, 3))
                        {
                            CameraUtilities.SaveAsCurrentProfile();
                            _selectedProfile = 0;
                            _profileNameList = CameraUtilities.ProfileList();
                        }
                        if (MenuUI.Button(8, 0, "Delete\nselected profile", 4, 3))
                        {
                            CameraUtilities.DeleteProfile(_profileNameList[_selectedProfile]);
                            _selectedProfile = 0;
                            _profileNameList = CameraUtilities.ProfileList();
                        }

                        _selectedProfile = MenuUI.SelectionGrid(0, 4, _selectedProfile, ref _currentProfilePage, _profileNameList, string.Empty, 12, 10);

                        if(MenuUI.Button(1,15,"Load Profile", 10, 2))
                            CameraUtilities.ProfileChange(_profileNameList[_selectedProfile]);

                        if(MenuUI.ToggleSwitch(0, 18, "Load profile on scene change", PluginConfig.Instance.ProfileSceneChange, 12, 2, 1.5f))
                            PluginConfig.Instance.ProfileSceneChange = !PluginConfig.Instance.ProfileSceneChange;

                        if (MenuUI.Button(0, 21, "Menu Scene", 4, 2))
                            PluginConfig.Instance.MenuProfile = _profileNameList[_selectedProfile];
                        MenuUI.Box(4, 21, PluginConfig.Instance.MenuProfile, MenuUI.CustomStyle[2], 7, 2);
                        if (MenuUI.Button(11, 21, "x", 1, 2))
                            PluginConfig.Instance.MenuProfile = string.Empty;

                        if (MenuUI.Button(0, 23, "Game Scene", 4, 2))
                            PluginConfig.Instance.GameProfile = _profileNameList[_selectedProfile];
                        MenuUI.Box(4, 23, PluginConfig.Instance.GameProfile, MenuUI.CustomStyle[2], 7, 2);
                        if (MenuUI.Button(11, 23, "x", 1, 2))
                            PluginConfig.Instance.GameProfile = string.Empty;

                        if (MenuUI.Button(0, 25, "Game 90/360", 4, 2))
                            PluginConfig.Instance.RotateProfile = _profileNameList[_selectedProfile];
                        MenuUI.Box(4, 25, PluginConfig.Instance.RotateProfile, MenuUI.CustomStyle[2], 7, 2);
                        if (MenuUI.Button(11, 25, "x", 1, 2))
                            PluginConfig.Instance.RotateProfile = string.Empty;

                        if (MenuUI.Button(0, 27, "Multiplayer", 4, 2))
                            PluginConfig.Instance.MultiplayerProfile = _profileNameList[_selectedProfile];
                        MenuUI.Box(4, 27, PluginConfig.Instance.MultiplayerProfile, MenuUI.CustomStyle[2], 7, 2);
                        if (MenuUI.Button(11, 27, "x", 1, 2))
                            PluginConfig.Instance.MultiplayerProfile = string.Empty;

                        if (MenuUI.Button(0, 29, "SongScript", 4, 2))
                            PluginConfig.Instance.SongSpecificScriptProfile = _profileNameList[_selectedProfile];
                        MenuUI.Box(4, 29, PluginConfig.Instance.SongSpecificScriptProfile, MenuUI.CustomStyle[2], 7, 2);
                        if (MenuUI.Button(11, 29, "x", 1, 2))
                            PluginConfig.Instance.SongSpecificScriptProfile = string.Empty;

                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                    case MenuState.SettingConverter:
                        MenuUI.SetGrid(12, 34);

                        if (MenuUI.Button(0, 32, "Back top menu", 12, 2))
                            _menuMode = MenuState.MenuTop;
                        break;
                    /////////////////////////////////////////////////////////////////////////////////////
                }
                GUI.matrix = originalMatrix;
            }
        }

    }
}
