using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;
using System.Reflection;
using CameraPlus.HarmonyPatches;
using CameraPlus.Configuration;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;
using CameraPlus.VMCProtocol;

namespace CameraPlus
{
    public class CameraPlusController : MonoBehaviour
    {
        public Action<Scene, Scene> ActiveSceneChanged;
        internal static CameraPlusController instance { get; private set; }
        public ConcurrentDictionary<string, GameObject> LoadedProfile = new ConcurrentDictionary<string, GameObject>();
        public ConcurrentDictionary<string, CameraPlusBehaviour> Cameras = new ConcurrentDictionary<string, CameraPlusBehaviour>();

        public string CurrentProfile = string.Empty;

        internal bool MultiplayerSessionInit;
        internal bool existsVMCAvatar = false;
        internal bool isRestartingSong = false;
        internal Transform origin;

        internal Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        private RenderTexture _renderTexture;
        private ScreenCameraBehaviour _screenCameraBehaviour;
        private CameraMoverPointer _cameraMovePointer;
        private bool initialized = false;

        internal UnityEvent OnFPFCToggleEvent = new UnityEvent();
        public bool isFPFC = false;

        internal ExternalSender externalSender = null;

        private WebCamTexture _webCamTexture = null;
        internal WebCamDevice[] webCamDevices;
        private WebCamCalibrator _webCamCal;

        private void Awake()
        {
            if (instance != null)
            {
                Plugin.Log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            instance = this;

            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
            CameraUtilities.CreateMainDirectory();
            CameraUtilities.CreateExampleScript();
        }
        private void Start()
        {
            _renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _screenCameraBehaviour = this.gameObject.AddComponent<ScreenCameraBehaviour>();
            _screenCameraBehaviour.SetCameraInfo(new Vector2(0, 0), new Vector2(Screen.width, Screen.height), -2000);
            _screenCameraBehaviour.SetRenderTexture(_renderTexture);

            ShaderLoad();
            _cameraMovePointer = this.gameObject.AddComponent<CameraMoverPointer>();
            CameraUtilities.AddNewCamera(Plugin.MainCamera);
            MultiplayerSessionInit = false;

            externalSender = new GameObject("ExternalSender").AddComponent<ExternalSender>();
            externalSender.transform.SetParent(transform);

            if (CustomUtils.IsModInstalled("VMCAvatar","0.99.0"))
                existsVMCAvatar = true;
            _webCamTexture = new WebCamTexture();
            webCamDevices = WebCamTexture.devices;
        }

        private void ShaderLoad()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraPlus.Resources.Shader.customshader"));
            Shaders = assetBundle.LoadAllAssets<Shader>().ToDictionary(x => x.name);
        }

        private void OnDestroy()
        {
            MultiplayerSession.Close();
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.
        }

        public void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (isRestartingSong && to.name != "GameCore") return;
            if (initialized || (!initialized && (to.name == "HealthWarning" || to.name == "MainMenu")))
                SharedCoroutineStarter.instance.StartCoroutine(DelayedActiveSceneChanged(from, to));
#if DEBUG
            Plugin.Log.Info($"Scene Change {from.name} to {to.name}");
#endif
        }

        private IEnumerator DelayedActiveSceneChanged(Scene from, Scene to)
        {
            bool isLevelEditor = false;

            yield return waitMainCamera();

            if (!initialized)
            {
                CameraUtilities.ProfileChange(string.Empty);
            }

            initialized = true;

            IEnumerator waitForcam()
            {
                yield return new WaitForSeconds(0.1f);
                while (Camera.main == null) yield return new WaitForSeconds(0.05f);
            }

            if (PluginConfig.Instance.ProfileSceneChange)
            {
                if (!MultiplayerSession.ConnectedMultiplay || PluginConfig.Instance.MultiplayerProfile == "")
                {
                    if (to.name == "GameCore" && PluginConfig.Instance.SongSpecificScriptProfile != "" && CustomPreviewBeatmapLevelPatch.customLevelPath != "")
                        CameraUtilities.ProfileChange(PluginConfig.Instance.SongSpecificScriptProfile);
                    else if (to.name == "GameCore" && PluginConfig.Instance.RotateProfile != "" && LevelDataPatch.is360Level)
                        CameraUtilities.ProfileChange(PluginConfig.Instance.RotateProfile);
                    else if (to.name == "GameCore" && PluginConfig.Instance.GameProfile != "")
                        CameraUtilities.ProfileChange(PluginConfig.Instance.GameProfile);
                    else if ((to.name == "MainMenu" || to.name == "MenuCore" || to.name == "HealthWarning") && PluginConfig.Instance.MenuProfile != "")
                        CameraUtilities.ProfileChange(PluginConfig.Instance.MenuProfile);
                    else if (to.name == "BeatmapEditor3D" || to.name == "BeatmapLevelEditorWorldUi")
                    {
                        CameraUtilities.ClearCameras();
                        isLevelEditor = true;
                    }
                }
            }
            if (!isLevelEditor)
            {
                if (ActiveSceneChanged != null)
                {
                    yield return waitForcam();
                    // Invoke each activeSceneChanged event
                    foreach (var func in ActiveSceneChanged?.GetInvocationList())
                    {
                        try
                        {
                            func?.DynamicInvoke(from, to);
                        }
                        catch (Exception ex)
                        {
                            Plugin.Log.Error($"Exception while invoking ActiveSceneChanged:" +
                                $" {ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
                else if (PluginConfig.Instance.ProfileSceneChange && to.name == "HealthWarning" && PluginConfig.Instance.MenuProfile != "")
                    CameraUtilities.ProfileChange(PluginConfig.Instance.MenuProfile);

                yield return waitForcam();

                if (to.name == "GameCore")
                    origin = GameObject.Find("LocalPlayerGameCore/Origin")?.transform;
                if (to.name == "MainMenu")
                {
                    var chat = GameObject.Find("ChatDisplay");
                    if (chat)
                        chat.layer = Layer.UI;
                }
            }
        }

        internal IEnumerator waitMainCamera()
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
        }

        internal string[] WebCameraList()
        {
            string[] webcamera = new string[] { };
            List<string> list = new List<string>();
            if (_webCamTexture)
            {
                for (int i = 0; i < webCamDevices.Length; i++)
                    list.Add(webCamDevices[i].name);
                webcamera = list.ToArray();
            }
            return webcamera;
        }

        internal void WebCameraCalibration(CameraPlusBehaviour camplus)
        {
            _webCamCal = new GameObject("WebCamCalScreen").AddComponent<WebCamCalibrator>();
            _webCamCal.transform.SetParent(this.transform);
            _webCamCal.Init();
            _webCamCal.AddCalibrationScreen(camplus, Camera.main);
        }
        internal bool inProgressCalibration()
        {
            if (_webCamCal)
                return true;
            return false;
        }
        internal void DestroyCalScreen()
        {
            if (_webCamCal)
                Destroy(_webCamCal.gameObject);
        }
    }
}
