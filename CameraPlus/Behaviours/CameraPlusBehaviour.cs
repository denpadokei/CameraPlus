using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using Screen = UnityEngine.Screen;
using CameraPlus.Configuration;
using CameraPlus.HarmonyPatches;
using CameraPlus.VMCProtocol;
using CameraPlus.Utilities;
using CameraPlus.UI;

namespace CameraPlus.Behaviours
{
    public class CameraPlusBehaviour : MonoBehaviour
    {
        public enum CursorType
        {
            None,
            Horizontal,
            Vertical,
            DiagonalLeft,
            DiagonalRight
        }

        public bool ThirdPerson { get => Config.thirdPerson; set { Config.thirdPerson = value; }}
        internal float FOV { get => _cam.fieldOfView; set { _cam.fieldOfView = value; } }

        public Vector3 ThirdPersonPos;
        public Vector3 ThirdPersonRot;
        public Vector3 OffsetPosition;
        public Vector3 OffsetAngle;
        public CameraConfig Config;

        internal Camera _cam;
        internal CameraPreviewQuad _quad;
        protected RenderTexture _camRenderTexture;
        protected ScreenCameraBehaviour _screenCamera;
        protected Camera _mainCamera = null;
        protected CameraMovement _cameraMovement = null;
        protected BeatLineManager _beatLineManager;
        protected EnvironmentSpawnRotation _environmentSpawnRotation;

        protected int _prevLayer;
        protected int _prevScreenPosX, _prevScreenPosY;
        protected float _yAngle;

        protected bool _wasWindowActive = false;
        protected bool _mouseHeld = false;
        protected bool _isResizing = false;
        protected bool _isMoving = false;
        protected bool _xAxisLocked = false;
        protected bool _yAxisLocked = false;
        protected bool _contextMenuOpen = false;
        internal bool _isCameraDestroyed = false;
        protected bool _isMainCamera = false;
        protected bool _isTopmostAtCursorPos = false;
        protected DateTime _lastRenderUpdate;
        protected Vector2 _initialOffset = new Vector2(0, 0);
        protected Vector2 _lastGrabPos = new Vector2(0, 0);
        protected Vector2 _lastScreenPos;
        protected bool _isBottom = false, _isLeft = false;
        protected static GameObject MenuObj = null;
        protected static UI.ContextMenu _contextMenu = null;
        public static CursorType currentCursor = CursorType.None;
        public static bool wasWithinBorder = false;
        public static bool anyInstanceBusy = false;
        private static bool _contextMenuEnabled = true;
        private GameObject adjustOffset;
        private GameObject adjustParent;
        private GUIStyle _multiplayerGUIStyle = null;
        private Vector3 prevMousePos = Vector3.zero;
        private Vector3 mouseRightDownPos = Vector3.zero;
        internal bool mouseMoveCamera = false;
        internal bool mouseMoveCameraSave = false;
        internal bool scriptEditMode = false;
        private Transform turnToTarget;
        internal bool turnToHead = false;
        internal Vector3 turnToHeadOffset = Vector3.zero;
        internal bool turnToHeadHorizontal = false;
        internal WebCamScreen webCamScreen = null;

#if WithVMCAvatar
        private VMCAvatarMarionette marionette = null;
#endif
        public virtual void Init(CameraConfig config)
        {
            DontDestroyOnLoad(gameObject);

            Config = config;
            Config.cam = this;
            _isMainCamera = Path.GetFileName(Config.FilePath) == $"{Plugin.MainCamera}.json";
            //_contextMenuEnabled = Array.IndexOf(Environment.GetCommandLineArgs(), "fpfc") == -1;

            StartCoroutine(DelayedInit());
        }

        protected IEnumerator DelayedInit()
        {
            yield return StartCoroutine(GetMainCamera());

            if (_contextMenu == null)
            {
                MenuObj = new GameObject("CameraPlusMenu");
                _contextMenu = MenuObj.AddComponent<UI.ContextMenu>();
            }
            XRSettings.showDeviceView = false;

            var gameObj = Instantiate(_mainCamera.gameObject);

            Config.ConfigChangedEvent += PluginOnConfigChangedEvent;

            gameObj.SetActive(false);
            gameObj.name = "Camera Plus";
            gameObj.tag = "Untagged";

            _cam = gameObj.GetComponent<Camera>();
            _cam.stereoTargetEye = StereoTargetEyeMask.None;
            _cam.enabled = true;
            _cam.name = Path.GetFileName(Config.FilePath);

            foreach (var child in _cam.transform.Cast<Transform>())
                Destroy(child.gameObject);
            var destroyList = new string[] { "AudioListener", "LIV", "MainCamera", "MeshCollider" };
            foreach (var component in _cam.GetComponents<Behaviour>())
                if (destroyList.Contains(component.GetType().Name)) Destroy(component);

            _screenCamera = new GameObject("Screen Camera").AddComponent<ScreenCameraBehaviour>();

            gameObj.SetActive(true);

            var camera = _mainCamera.transform;
            transform.position = camera.position;
            transform.rotation = camera.rotation;

            gameObj.transform.parent = transform;
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localRotation = Quaternion.identity;
            gameObj.transform.localScale = Vector3.one;

            _quad = new GameObject("PreviewQuad").AddComponent<CameraPreviewQuad>();
            _quad.transform.SetParent(_cam.transform);
            _quad.transform.localPosition = Vector3.zero;
            _quad.transform.localEulerAngles = Vector3.zero;
            _quad.Init(this);

            ReadConfig();

            if (ThirdPerson)
            {
                ThirdPersonPos = Config.Position;
                ThirdPersonRot = Config.Rotation;

                transform.position = ThirdPersonPos;
                transform.eulerAngles = ThirdPersonRot;
            }

            AddMovementScript();

            Plugin.cameraController.ActiveSceneChanged += SceneManager_activeSceneChanged;
            SceneManager_activeSceneChanged(new Scene(), new Scene());
            Logger.log.Notice($"Camera \"{Path.GetFileName(Config.FilePath)}\" successfully initialized! {Convert.ToString(_cam.cullingMask, 16)}");

            if (Config.vmcProtocol.mode == VMCProtocolMode.Sender)
                InitExternalSender();

            Plugin.cameraController.OnFPFCToggleEvent.AddListener(OnFPFCToglleEvent);

            if (Config.webCamera.autoConnect)
                CreateWebCamScreen();
        }

        internal void CreateWebCamScreen()
        {
            if (webCamScreen)
            {
                ChangeWebCamScreen();
                return;
            }
            webCamScreen = new GameObject("WebCamScreen").AddComponent<WebCamScreen>();
            webCamScreen.transform.SetParent(transform);
            webCamScreen.AddWebCamScreen(Config.webCamera.name, this);
        }
        internal void ChangeWebCamScreen()
        {
            if (!webCamScreen)
            {
                CreateWebCamScreen();
                return;
            }
            webCamScreen.ChangeCamera(Config.webCamera.name);
        }
        internal void DisableWebCamScreen()
        {
            if (!webCamScreen) return;
            webCamScreen.DisconnectWebCam();
            Destroy(webCamScreen.gameObject);
            webCamScreen = null;
        }

        public void InitExternalSender()
        {
            if (Config.vmcProtocol.mode == VMCProtocolMode.Sender)
                Plugin.cameraController.externalSender.AddSendTask(this, Config.vmcProtocol.address, Config.vmcProtocol.port);
        }
        public void InitExternalReceiver()
        {
#if WithVMCAvatar
            if (Config.vmcProtocol.mode == VMCProtocolMode.Receiver && Plugin.cameraController.existsVMCAvatar)
            {
                marionette = this.gameObject.AddComponent<VMCProtocol.VMCAvatarMarionette>();
                ClearMovementScript();
            }
#endif
        }
        public void DestoryVMCProtocolObject()
        {
#if WithVMCAvatar
            if (marionette)
                Destroy(marionette);
#endif
            Plugin.cameraController.externalSender.RemoveTask(this);

            if (Config.movementScript.movementScript != String.Empty || Config.movementScript.songSpecificScript)
                AddMovementScript();
        }


        protected virtual void OnDestroy()
        {
            Config.ConfigChangedEvent -= PluginOnConfigChangedEvent;
            Plugin.cameraController.ActiveSceneChanged -= SceneManager_activeSceneChanged;

            _cameraMovement?.Shutdown();
            // Close our context menu if its open, and destroy all associated controls, otherwise the game will lock up
            CloseContextMenu();

            _camRenderTexture?.Release();

#if WithVMCAvatar
            if (marionette)
                Destroy(marionette);
#endif
            if (adjustParent)
                Destroy(adjustParent);

            Plugin.cameraController.externalSender.RemoveTask(this);

            if (webCamScreen)
                DisableWebCamScreen();

            if (_screenCamera)
                Destroy(_screenCamera.gameObject);
            if (_quad)
                Destroy(_quad);
        }

        protected virtual void PluginOnConfigChangedEvent(CameraConfig config)
        {
            ReadConfig();
        }

        protected virtual void ReadConfig()
        {
            if (!ThirdPerson)
            {
                transform.position = _mainCamera.transform.position;
                transform.rotation = _mainCamera.transform.rotation;
            }
            else
            {
                ThirdPersonPos = Config.Position;
                ThirdPersonRot = Config.Rotation;
            }
            if(Config.cameraExtensions.dontDrawDesktop)
                _screenCamera.enabled = false;
            else
                _screenCamera.enabled = true;

            turnToHead = Config.cameraExtensions.turnToHead;
            turnToHeadOffset = Config.TurnToHeadOffset;
            turnToHeadHorizontal = Config.cameraExtensions.turnToHeadHorizontal;
            Config.SetCullingMask();
            _quad.gameObject.SetActive(ThirdPerson && Config.PreviewCamera);
            _cam.fieldOfView = Config.fov;
            _cam.orthographic = Config.cameraExtensions.orthographicMode;
            _cam.orthographicSize = Config.cameraExtensions.orthographicSize;
            _cam.farClipPlane = Config.cameraExtensions.farClipPlane;
            CreateScreenRenderTexture();
            _quad.SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition);
        }

        internal virtual void CreateScreenRenderTexture()
        {
            if (Config.fitToCanvas)
            {
                Config.screenPosX = 0;
                Config.screenPosY = 0;
                Config.screenWidth = Screen.width;
                Config.screenHeight = Screen.height;
            }
            var w = (int)Math.Round(Config.screenWidth * Config.renderScale);
            var h = (int)Math.Round(Config.screenHeight * Config.renderScale);

            var changed = _camRenderTexture?.width != w || _camRenderTexture?.height != h || 
                        _camRenderTexture?.antiAliasing != _camRenderTexture.antiAliasing;

            if (changed)
            {
                _camRenderTexture?.Release();
                _camRenderTexture = new RenderTexture(w, h, 24) {
                    useMipMap = false,
                    antiAliasing = Config.antiAliasing,
                    anisoLevel = 1,
                    useDynamicScale = false
                };
                _cam.targetTexture = _camRenderTexture;
                _quad._previewMaterial.SetTexture("_MainTex", _camRenderTexture);
                _screenCamera?.SetRenderTexture(_camRenderTexture);
            }

            if (changed || Config.screenPosX != _prevScreenPosX || Config.screenPosY != _prevScreenPosY || Config.layer != _prevLayer)
                _screenCamera?.SetCameraInfo(Config.ScreenPosition, Config.ScreenSize, Config.layer);

            _prevLayer = Config.layer;
            _prevScreenPosX = Config.screenPosX;
            _prevScreenPosY = Config.screenPosY;
        }
        public virtual void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            CloseContextMenu();
            StartCoroutine(GetMainCamera());
            Config.SetCullingMask();
        }

        private void OnFPFCToglleEvent()
        {
            if (FPFCPatch.instance != null)
            {
                if (FPFCPatch.isInstanceFPFC)
                {
                    turnToHead = false;
                    _screenCamera.SetLayer(Config.layer);
                }
                else
                {
                    turnToHead = Config.cameraExtensions.turnToHead;
                    _screenCamera.SetLayer(Config.layer + 1000);
                }
            }
        }

        protected virtual void Update()
        {
            // Only toggle the main camera in/out of third person with f1, not any extra cams
            if (_isMainCamera)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    ThirdPerson = !ThirdPerson;
                    if (!ThirdPerson)
                    {
                        transform.position = _mainCamera.transform.position;
                        transform.rotation = _mainCamera.transform.rotation;
                    }
                    else
                    {
                        ThirdPersonPos = Config.Position;
                        ThirdPersonRot = Config.Rotation;
                    }
                    Config.Save();
                }
            }
            HandleMouseEvents();
        }

        protected virtual void LateUpdate()
        {
            try
            {
                OffsetPosition = Vector3.zero;
                OffsetAngle = Vector3.zero;

                if (!_mainCamera) return;
                var camera = _mainCamera?.transform;

                if (ThirdPerson)
                {
#if WithVMCAvatar
                    if (Plugin.cameraController.existsVMCAvatar)
                        if (Config.vmcProtocol.mode == VMCProtocolMode.Receiver && marionette)
                            if (marionette.receivedData)
                            {
                                transform.position = marionette.position;
                                transform.rotation = marionette.rotate;
                                _cam.fieldOfView = marionette.fov > 0 ? marionette.fov : Config.fov;
                                return;
                            }
#endif

                    HandleMultiPlayerLobby();
                    HandleMultiPlayerGame();
                    HandleThirdPerson360();

                    if (Config.cameraExtensions.followNoodlePlayerTrack && Plugin.cameraController.origin)
                    {
                        if (adjustOffset == null)
                        {
                            adjustOffset = new GameObject("OriginTarget");
                            adjustParent = new GameObject("OriginParent");
                            adjustOffset.transform.SetParent(adjustParent.transform);
                        }
                        adjustParent.transform.position = Plugin.cameraController.origin.position;
                        adjustParent.transform.rotation = Plugin.cameraController.origin.rotation * Quaternion.Inverse(RoomAdjustPatch.rotation);

                        adjustOffset.transform.localPosition = ThirdPersonPos - RoomAdjustPatch.position;
                        adjustOffset.transform.localEulerAngles = ThirdPersonRot;

                        transform.position = adjustOffset.transform.position;
                        transform.eulerAngles = adjustOffset.transform.eulerAngles;
                    }
                    else
                    {
                        transform.position = ThirdPersonPos;
                        transform.eulerAngles = ThirdPersonRot;
                    }

                    if (OffsetPosition != Vector3.zero && OffsetAngle != Vector3.zero)
                    {
                        transform.position = ThirdPersonPos + OffsetPosition;
                        transform.eulerAngles = ThirdPersonRot + OffsetAngle;
                        Quaternion angle = Quaternion.AngleAxis(OffsetAngle.y, Vector3.up);
                        transform.position -= OffsetPosition;
                        transform.position = angle * transform.position;
                        transform.position += OffsetPosition;

                    }
                    if (Camera.main && turnToHead && !FPFCPatch.isInstanceFPFC && !Config.cameraExtensions.follow360map)
                    {
                        turnToTarget = Camera.main.transform;
                        turnToTarget.transform.position += turnToHeadOffset;
                        var direction = turnToTarget.position - transform.position;
                        var lookRotation = Quaternion.LookRotation(direction);
                        if (turnToHeadHorizontal)
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x,lookRotation.eulerAngles.y, transform.eulerAngles.z);
                        else
                            transform.rotation = lookRotation;
                        //transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Config.cameraExtensions.rotationSmooth);
                        turnToTarget.transform.position -= turnToHeadOffset;
                    }
                    return;
                }
                transform.position = Vector3.Lerp(transform.position, camera.position + Config.FirstPersonPositionOffset,
                    Config.cameraExtensions.positionSmooth * Time.unscaledDeltaTime);

                if (!Config.cameraExtensions.firstPersonCameraForceUpRight)
                    transform.rotation = Quaternion.Slerp(transform.rotation, camera.rotation * Quaternion.Euler(Config.FirstPersonRotationOffset),
                        Config.cameraExtensions.rotationSmooth * Time.unscaledDeltaTime);
                else

                {
                    Quaternion rot = Quaternion.Slerp(transform.rotation, camera.rotation * Quaternion.Euler(Config.FirstPersonRotationOffset),
                        Config.cameraExtensions.rotationSmooth * Time.unscaledDeltaTime);
                    transform.rotation = rot * Quaternion.Euler(0, 0, -(rot.eulerAngles.z));
                }
            }
            catch(Exception ex)
            {
                Logger.log.Error($"CameraPlus {this.name}, Error in LateUpdate : {ex}");
            }
        }

        private void HandleThirdPerson360()
        {
            if (!_beatLineManager || !Config.cameraExtensions.follow360map || !_environmentSpawnRotation)
            {
                _beatLineManager = BeatLineManagerPatch.Instance;
                _environmentSpawnRotation = EnvironmentSpawnRotationPatch.Instance;
                return;
            }
            float b;
            if (_beatLineManager.isMidRotationValid)
            {
                double midRotation = (double)this._beatLineManager.midRotation;
                float num1 = Mathf.DeltaAngle((float)midRotation, this._environmentSpawnRotation.targetRotation);
                float num2 = (float)(-(double)this._beatLineManager.rotationRange * 0.5);
                float num3 = this._beatLineManager.rotationRange * 0.5f;
                if ((double)num1 > (double)num3)
                    num3 = num1;
                else if ((double)num1 < (double)num2)
                    num2 = num1;
                b = (float)(midRotation + ((double)num2 + (double)num3) * 0.5);
            }
            else
                b = this._environmentSpawnRotation.targetRotation;

            if (!Config.cameraExtensions.follow360mapUseLegacyProcess)
                _yAngle = Mathf.LerpAngle(_yAngle, b, Mathf.Clamp(Time.deltaTime * Config.cameraExtensions.rotation360Smooth, 0f, 1f));
            else
                _yAngle = Mathf.Lerp(_yAngle, b, Mathf.Clamp(Time.deltaTime * Config.cameraExtensions.rotation360Smooth, 0f, 1f));

            ThirdPersonRot = new Vector3(Config.Rotation.x, _yAngle + Config.Rotation.y, Config.Rotation.z);

            ThirdPersonPos = (transform.forward * Config.posz) + (transform.right * Config.posx);
            ThirdPersonPos = new Vector3(ThirdPersonPos.x, Config.posy, ThirdPersonPos.z);
        }

        private void HandleMultiPlayerLobby()
        {
            try
            {
                if (!MultiplayerLobbyAvatarPlaceManagerPatch.Instance || !MultiplayerLobbyControllerPatch.Instance.isActiveAndEnabled || Config.multiplayer.targetPlayerNumber == 0) return;
                if (MultiplayerSession.LobbyAvatarPlaceList.Count == 0) MultiplayerSession.LoadLobbyAvatarPlace();

                for (int i = 0; i < MultiplayerSession.LobbyAvatarPlaceList.Count; i++)
                {
                    if (i == Config.multiplayer.targetPlayerNumber - 1)
                    {
                        OffsetPosition = MultiplayerSession.LobbyAvatarPlaceList[i].position;
                        OffsetAngle = MultiplayerSession.LobbyAvatarPlaceList[i].eulerAngles;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"HandleMultiPlayerLobby Error {ex.Message}");
            }
        }
        private void HandleMultiPlayerGame()
        {
            try
            {
                if (SceneManager.GetActiveScene().name == "GameCore" && MultiplayerSession.ConnectedMultiplay)
                {
                    MultiplayerConnectedPlayerFacade player = null;
                    bool TryPlayerFacade;
                    if (MultiplayerPlayersManagerPatch.Instance && Config.multiplayer.targetPlayerNumber != 0)
                        foreach (IConnectedPlayer connectedPlayer in MultiplayerSession.connectedPlayers)
                            if (Config.multiplayer.targetPlayerNumber - 1 == connectedPlayer.sortIndex)
                            {
                                TryPlayerFacade = MultiplayerPlayersManagerPatch.Instance.TryGetConnectedPlayerController(connectedPlayer.userId, out player);
                                if (TryPlayerFacade && player != null)
                                {
                                    OffsetPosition = player.transform.position;
                                    OffsetAngle = player.transform.eulerAngles;
                                }
                                break;
                            }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"{this.name} HandleMultiPlayerGame Error {ex.Message}");
            }
        }

        public string AddMovementScript()
        {
            string songScriptPath = String.Empty;
            if (Config.vmcProtocol.mode == VMCProtocolMode.Receiver) return "ExternalReceiver Enabled";

            if (Config.movementScript.movementScript != String.Empty || Config.movementScript.songSpecificScript)
            {
                if (_cameraMovement)
                    _cameraMovement.Shutdown();

                if (CustomPreviewBeatmapLevelPatch.customLevelPath != String.Empty && Config.movementScript.songSpecificScript)
                    songScriptPath = CustomPreviewBeatmapLevelPatch.customLevelPath;
                else if (File.Exists(Path.Combine(CameraUtilities.scriptPath, Path.GetFileName(Config.movementScript.movementScript))))
                    songScriptPath = Path.Combine(CameraUtilities.scriptPath, Path.GetFileName(Config.movementScript.movementScript));
                else
                    return "Not Find Script";

                _cameraMovement = _cam.gameObject.AddComponent<CameraMovement>();
                if (_cameraMovement.Init(this, songScriptPath))
                {
                    ThirdPersonPos = Config.Position;
                    ThirdPersonRot = Config.Rotation;
                    Config.thirdPerson = true;
                    ThirdPerson = true;
                    CreateScreenRenderTexture();
                }
                else
                    return "Fail CameraMovement Initialize";
                return songScriptPath;
            }
            return string.Empty;
        }
        public void ClearMovementScript()
        {
            if (_cameraMovement)
                _cameraMovement.Shutdown();
            _cameraMovement = null;
            ThirdPersonPos = Config.Position;
            ThirdPersonRot = Config.Rotation;
            _cam.fieldOfView = Config.fov;
            CreateScreenRenderTexture();
        }

        protected IEnumerator GetMainCamera()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
            {
                while (!MainCameraPatch.isGameCameraEnable)
                    yield return null;
            }
            else
            {
                while (Camera.main == null)
                    yield return null;
            }
            _mainCamera = Camera.main;
        }

        public bool IsWithinRenderArea(Vector2 mousePos, CameraConfig c)
        {
            if (mousePos.x < c.screenPosX) return false;
            if (mousePos.x > c.screenPosX + c.screenWidth) return false;
            if (mousePos.y < c.screenPosY) return false;
            if (mousePos.y > c.screenPosY + c.screenHeight) return false;
            return true;
        }

        public bool IsTopmostRenderAreaAtPos(Vector2 mousePos)
        {
            if (!IsWithinRenderArea(mousePos, Config)) return false;
            foreach (CameraPlusBehaviour c in Plugin.cameraController.Cameras.Values.ToArray())
            {
                if (c == this) continue;
                if (!IsWithinRenderArea(mousePos, c.Config) && !c._mouseHeld) continue;
                if (c.Config.layer > Config.layer)
                {
                    return false;
                }

                if (c._mouseHeld && (c._isMoving ||
                    c._isResizing || c._contextMenuOpen))
                {
                    return false;
                }
            }
            return true;
        }

        public static CameraPlusBehaviour GetTopmostInstanceAtCursorPos()
        {
            foreach (CameraPlusBehaviour c in Plugin.cameraController.Cameras.Values.ToArray())
            {
                if (c._isTopmostAtCursorPos)
                    return c;
            }
            return null;
        }

        internal void CloseContextMenu()
        {
            if (_contextMenu != null)
                _contextMenu.DisableMenu();
            Destroy(MenuObj);
            _contextMenuOpen = false;
        }

        public static void SetCursor(CursorType type)
        {
            if (type != currentCursor)
            {
                Texture2D texture = null;
                switch (type)
                {
                    case CursorType.Horizontal:
                        texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Resize_Horiz.png");
                        break;
                    case CursorType.Vertical:
                        texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Resize_Vert.png");
                        break;
                    case CursorType.DiagonalRight:
                        texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Resize_DiagRight.png");
                        break;
                    case CursorType.DiagonalLeft:
                        texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Resize_DiagLeft.png");
                        break;

                }
                UnityEngine.Cursor.SetCursor(texture, texture ? new Vector2(texture.width / 2, texture.height / 2) : new Vector2(0, 0), CursorMode.Auto);
                currentCursor = type;
            }
        }

        protected void HandleMouseEvents()
        {
            bool holdingLeftClick = Input.GetMouseButton(0);
            bool holdingRightClick = Input.GetMouseButton(1);
            bool holdingMiddleClick = Input.GetMouseButton(2);

            Vector3 mousePos = Input.mousePosition;

            _isTopmostAtCursorPos = IsTopmostRenderAreaAtPos(mousePos);
            if (_isTopmostAtCursorPos && mouseMoveCamera)
            {
                if (Input.GetMouseButtonDown(2))
                    prevMousePos = mousePos;

                if (holdingLeftClick)
                {
                    float scroll = Input.mouseScrollDelta.y;
                    if (scroll != 0)
                        ThirdPersonRot.z += scroll * CameraUtilities.mouseRotateSpeed[2];
                }

                if (holdingMiddleClick)
                    if (mousePos != prevMousePos)
                    {
                        Vector3 up = transform.TransformDirection(Vector3.up);
                        Vector3 right = transform.TransformDirection(Vector3.right);

                        ThirdPersonPos += right * (mousePos.x - prevMousePos.x) * CameraUtilities.mouseMoveSpeed[0] +
                                            up * (mousePos.y - prevMousePos.y) * CameraUtilities.mouseMoveSpeed[1];
                    }

                if (holdingRightClick)
                {
                    float scroll = Input.mouseScrollDelta.y;
                    if (scroll != 0)
                        ThirdPersonPos += transform.forward * scroll * CameraUtilities.mouseScrollSpeed;

                    if (mousePos != prevMousePos)
                    {
                        ThirdPersonRot.x += (mousePos.y - prevMousePos.y) * CameraUtilities.mouseRotateSpeed[0];
                        ThirdPersonRot.y += (mousePos.x - prevMousePos.x) * CameraUtilities.mouseRotateSpeed[1];
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    mouseRightDownPos = mousePos;
                    holdingRightClick = false;
                }
                else if (Input.GetMouseButtonUp(1))
                    holdingRightClick = (Vector3.Distance(mouseRightDownPos, mousePos) < 4);
                else
                    holdingRightClick = false;

                if (mouseMoveCameraSave && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)))
                {
                    Config.Position = ThirdPersonPos;
                    Config.Rotation = ThirdPersonRot;
                    Config.Save();
                }

                prevMousePos = mousePos;
            }

            // Only evaluate mouse events for the topmost render target at the mouse position
            if (!_mouseHeld && !_isTopmostAtCursorPos) return;

            int tolerance = 5;
            bool cursorWithinBorder = CustomUtils.WithinRange((int)mousePos.x, -tolerance, tolerance) || CustomUtils.WithinRange((int)mousePos.y, -tolerance, tolerance) ||
                CustomUtils.WithinRange((int)mousePos.x, Config.screenPosX + Config.screenWidth - tolerance, Config.screenPosX + Config.screenWidth + tolerance) ||
                CustomUtils.WithinRange((int)mousePos.x, Config.screenPosX - tolerance, Config.screenPosX + tolerance) ||
                CustomUtils.WithinRange((int)mousePos.y, Config.screenPosY + Config.screenHeight - tolerance, Config.screenPosY + Config.screenHeight + tolerance) ||
                CustomUtils.WithinRange((int)mousePos.y, Config.screenPosY - tolerance, Config.screenPosY + tolerance);

            float currentMouseOffsetX = mousePos.x - Config.screenPosX;
            float currentMouseOffsetY = mousePos.y - Config.screenPosY;
            if (!_mouseHeld)
            {
                if (cursorWithinBorder)
                {
                    var isLeft = currentMouseOffsetX <= Config.screenWidth / 2;
                    var isBottom = currentMouseOffsetY <= Config.screenHeight / 2;
                    var centerX = Config.screenPosX + (Config.screenWidth / 2);
                    var centerY = Config.screenPosY + (Config.screenHeight / 2);
                    var offsetX = Config.screenWidth / 2 - tolerance;
                    var offsetY = Config.screenHeight / 2 - tolerance;
                    _xAxisLocked = CustomUtils.WithinRange((int)mousePos.x, centerX - offsetX + 1, centerX + offsetX - 1);
                    _yAxisLocked = CustomUtils.WithinRange((int)mousePos.y, centerY - offsetY + 1, centerY + offsetY - 1);

                    if (!Config.fitToCanvas)
                    {
                        if (_xAxisLocked)
                            SetCursor(CursorType.Vertical);
                        else if (_yAxisLocked)
                            SetCursor(CursorType.Horizontal);
                        else if (isLeft && isBottom || !isLeft && !isBottom)
                            SetCursor(CursorType.DiagonalLeft);
                        else if (isLeft && !isBottom || !isLeft && isBottom)
                            SetCursor(CursorType.DiagonalRight);
                    }
                    wasWithinBorder = true;
                }
                else if (!cursorWithinBorder && wasWithinBorder)
                {
                    SetCursor(CursorType.None);
                    wasWithinBorder = false;
                }
            }

            if (holdingLeftClick && !Config.fitToCanvas && !Config.cameraLock.lockScreen)
            {
                if (!_mouseHeld)
                {
                    _initialOffset.x = currentMouseOffsetX;
                    _initialOffset.y = currentMouseOffsetY;

                    _lastScreenPos = Config.ScreenPosition;
                    _lastGrabPos = new Vector2(mousePos.x, mousePos.y);

                    _isLeft = _initialOffset.x <= Config.screenWidth / 2;
                    _isBottom = _initialOffset.y <= Config.screenHeight / 2;
                    anyInstanceBusy = true;
                }
                _mouseHeld = true;

                if (!_isMoving && (_isResizing || cursorWithinBorder))
                {
                    _isResizing = true;
                    if (!_xAxisLocked)
                    {
                        int changeX = _isLeft ? (int)(_lastGrabPos.x - mousePos.x) : (int)(mousePos.x - _lastGrabPos.x);
                        Config.screenWidth += changeX;
                        Config.screenPosX = ((int)_lastScreenPos.x - (_isLeft ? changeX : 0));
                    }
                    if (!_yAxisLocked)
                    {
                        int changeY = _isBottom ? (int)(mousePos.y - _lastGrabPos.y) : (int)(_lastGrabPos.y - mousePos.y);
                        Config.screenHeight -= changeY;
                        Config.screenPosY = ((int)_lastScreenPos.y + (_isBottom ? changeY : 0));
                    }
                    _lastGrabPos = mousePos;
                    _lastScreenPos = Config.ScreenPosition;
                }
                else
                {
                    _isMoving = true;
                    Config.screenPosX = (int)mousePos.x - (int)_initialOffset.x;
                    Config.screenPosY = (int)mousePos.y - (int)_initialOffset.y;
                }
                Config.screenWidth = Mathf.Clamp(Config.screenWidth, 100, Screen.width);
                Config.screenHeight = Mathf.Clamp(Config.screenHeight, 100, Screen.height);
                Config.screenPosX = Mathf.Clamp(Config.screenPosX, 0, Screen.width - Config.screenWidth);
                Config.screenPosY = Mathf.Clamp(Config.screenPosY, 0, Screen.height - Config.screenHeight);
                webCamScreen?.ChangeWebCamRectScale(Config.screenHeight);

                CreateScreenRenderTexture();
            }
            else if (holdingRightClick && _contextMenuEnabled)
            {
                if (_mouseHeld) return;
                DisplayContextMenu();
                _contextMenuOpen = true;
                anyInstanceBusy = true;
                _mouseHeld = true;
            }
            else if (_isResizing || _isMoving || _mouseHeld)
            {
                if (!_contextMenuOpen)
                {
                    if (!_isCameraDestroyed)
                    {
                        Config.Save();
                    }
                }
                _isResizing = false;
                _isMoving = false;
                _mouseHeld = false;
                anyInstanceBusy = false;
            }
        }
        void OnGUI()
        {
            if (MultiplayerSession.connectedPlayers != null && Config.multiplayer.displayPlayerInfo)
                foreach (IConnectedPlayer connectedPlayer in MultiplayerSession.connectedPlayers)
                    if (Config.multiplayer.targetPlayerNumber - 1 == connectedPlayer.sortIndex)
                    {
                        int size = 0;
                        var offsetY = Screen.height / 2;

                        GUI.skin.label.fontSize = Config.screenWidth / 8;
                        size = GUI.skin.label.fontSize + 15;

                        GUI.Label(new Rect(Config.screenPosX, Screen.height - Config.screenPosY - Config.screenHeight, Config.screenWidth, GUI.skin.label.fontSize + 15), connectedPlayer.userName);

                        if (SceneManager.GetActiveScene().name == "GameCore" && MultiplayerSession.ConnectedMultiplay)
                        {
                            if (MultiplayerScoreProviderPatch.Instance)
                            {
                                foreach (MultiplayerScoreProvider.RankedPlayer rankedPlayer in MultiplayerScoreProviderPatch.Instance.rankedPlayers)
                                    if (rankedPlayer.userId == connectedPlayer.userId)
                                    {
                                        if (_multiplayerGUIStyle == null)
                                            _multiplayerGUIStyle = new GUIStyle(GUI.skin.label);
                                        if (rankedPlayer.isFailed)
                                            _multiplayerGUIStyle.normal.textColor = Color.red;
                                        else
                                            _multiplayerGUIStyle.normal.textColor = Color.white;
                                        _multiplayerGUIStyle.fontSize = 30;
                                        GUI.Label(new Rect(Config.screenPosX, Screen.height - Config.screenPosY - Config.screenHeight + size + 45, Config.screenWidth, 40), String.Format("{0:#,0}", rankedPlayer.score), _multiplayerGUIStyle);
                                        GUI.Label(new Rect(Config.screenPosX, Screen.height - Config.screenPosY - Config.screenHeight + size + 5, Config.screenWidth, 40), "Rank " + MultiplayerScoreProviderPatch.Instance.GetPositionOfPlayer(connectedPlayer.userId).ToString(), _multiplayerGUIStyle);
                                        break;
                                    }
                                break;
                            }
                        }
                    }
        }
        void DisplayContextMenu()
        {
            if (scriptEditMode) return;
            if (_contextMenu == null)
            {
                MenuObj = new GameObject("CameraPlusMenu");
                _contextMenu = MenuObj.AddComponent<UI.ContextMenu>();
            }
            _contextMenu.EnableMenu(Input.mousePosition, this);
        }
    }
}
