using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    public class ContextMenu : MonoBehaviour
    {
        internal enum MenuState
        {
            MenuTop,
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
        internal Vector2 menuPos
        {
            get
            {
                return new Vector2(
                   Mathf.Min(mousePosition.x / (Screen.width / 1600f), (Screen.width * ( 0.806249998f / (Screen.width / 1600f)))),
                   Mathf.Min((Screen.height - mousePosition.y) / (Screen.height / 900f), (Screen.height * (0.555555556f / (Screen.height / 900f))))
                    );
            }
        }
        internal Vector2 mousePosition;
        internal bool showMenu;
        internal MenuState MenuMode = MenuState.MenuTop;
        internal CameraPlusBehaviour parentBehaviour;
        internal string[] scriptName;
        internal int scriptPage = 0;
        internal string[] webCameraName;
        internal int webCameraPage = 0;
        internal Texture2D texture = null;
        internal Texture2D Cameratexture = null;
        internal GUIStyle CustomEnableStyle = null;
        internal GUIStyle CustomDisableStyle = null;
        internal GUIStyle ProfileStyle = null;

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
            mousePosition = mousePos;
            showMenu = true;
            this.parentBehaviour = parentBehaviour;
            scriptName = CameraUtilities.MovementScriptList();
            webCameraName = Plugin.cameraController.WebCameraList();

            if (this.parentBehaviour.Config.cameraLock.lockScreen)
                texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
            else
                texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
            if (this.parentBehaviour.Config.cameraLock.lockCamera || this.parentBehaviour.Config.cameraLock.dontSaveDrag)
                Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
            else
                Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png");
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
            showMenu = false;
        }
        void OnGUI()
        {

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
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");

                CustomEnableStyle = new GUIStyle(GUI.skin.button);
                CustomEnableStyle.normal.background = CustomEnableStyle.active.background;
                CustomEnableStyle.hover.background = CustomEnableStyle.active.background;
                CustomDisableStyle = new GUIStyle(GUI.skin.button);
                ProfileStyle = new GUIStyle(GUI.skin.box);
                ProfileStyle.alignment = UnityEngine.TextAnchor.MiddleLeft;

                if (MenuMode == MenuState.MenuTop)
                {
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 25, 30, 30), texture))
                    {
                        parentBehaviour.Config.cameraLock.lockScreen = !parentBehaviour.Config.cameraLock.lockScreen;
                        parentBehaviour.Config.Save();
                        if (this.parentBehaviour.Config.cameraLock.lockScreen)
                            texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
                        else
                            texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
                    }
                    GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 25, 115, 30), new GUIContent(parentBehaviour.Config.cameraLock.lockScreen ? "Locked Screen" : "Unlocked Screen"), ProfileStyle);

                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 25, 30, 30), Cameratexture))
                    {
                        if (!parentBehaviour.Config.cameraLock.lockCamera && !parentBehaviour.Config.cameraLock.dontSaveDrag)
                        {
                            parentBehaviour.Config.cameraLock.lockCamera = true;
                            parentBehaviour.Config.cameraLock.dontSaveDrag = false;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
                        }
                        else if (parentBehaviour.Config.cameraLock.lockCamera && !parentBehaviour.Config.cameraLock.dontSaveDrag)
                        {
                            parentBehaviour.Config.cameraLock.lockCamera = false;
                            parentBehaviour.Config.cameraLock.dontSaveDrag = true;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
                        }
                        else
                        {
                            parentBehaviour.Config.cameraLock.lockCamera = false;
                            parentBehaviour.Config.cameraLock.dontSaveDrag = false;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png");
                        }
                        parentBehaviour.Config.Save();
                    }
                    GUI.Box(new Rect(menuPos.x + 185, menuPos.y + 25, 115, 30), new GUIContent(parentBehaviour.Config.cameraLock.dontSaveDrag ? "ResetDrag Camera" : (parentBehaviour.Config.cameraLock.lockCamera ? "Locked Camera" : "Unlocked Camera")), ProfileStyle);

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 60, 145, 60), new GUIContent("Add New Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.log.Notice($"Adding new config with name {cameraName}.json");
                            CameraUtilities.AddNewCamera(cameraName);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 60, 145, 60), new GUIContent("Duplicate\nSelected Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.log.Notice($"Adding {cameraName}");
                            CameraUtilities.AddNewCamera(cameraName, parentBehaviour.Config);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 130, 145, 50), new GUIContent("Remove\nSelected Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            if (CameraUtilities.RemoveCamera(parentBehaviour))
                            {
                                parentBehaviour._isCameraDestroyed = true;
                                parentBehaviour.CreateScreenRenderTexture();
                                parentBehaviour.CloseContextMenu();
                                Logger.log.Notice("Camera removed!");
                            }
                        }
                    }

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
                GUI.matrix = originalMatrix;
            }
        }
    }
}
