using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using CameraPlus.Behaviours;
using CameraPlus.HarmonyPatches;
using CameraPlus.Utilities;
using System.Globalization;

namespace CameraPlus.Configuration
{
    internal enum CameraType
    {
        FirstPesron,
        ThirdPerson
    }
    public enum DebriVisibility
    {
        Visible,
        Hidden,
        Link
    }
    internal enum VMCProtocolMode
    {
        Disable,
        Sender,
        Receiver
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class CameraConfig
    {
        internal CameraPlusBehaviour cam = null;
        internal string FilePath { get; }

        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("CameraType")]
        private CameraType _cameraType = CameraType.ThirdPerson;
        [JsonProperty("FieldOfView")]
        private float _fieldOfView = 60;
        [JsonProperty("VisibleObject")]
        private visibleObjectsElements _visibleObject = new visibleObjectsElements();
        [JsonProperty("Layer")]
        private int _layer = -1000;
        [JsonProperty("AntiAliasing")]
        private int _antiAliasing = 2;
        [JsonProperty("RenderScale")]
        private float _renderScale = 1;
        [JsonProperty("WindowRect")]
        private windowRectElement _windowRect = new windowRectElement();
        [JsonProperty("ThirdPersonPos")]
        private targetTransformElements _thirdPersonPos = new targetTransformElements();
        [JsonProperty("ThirdPersonRot")]
        private targetTransformElements _thirdPersonRot = new targetTransformElements();
        [JsonProperty("FirstPersonPos")]
        private targetTransformElements _firstPersonPos = new targetTransformElements();
        [JsonProperty("FirstPersonRot")]
        private targetTransformElements _firstPersonRot = new targetTransformElements();
        [JsonProperty("TurnToHeadOffset")]
        private targetTransformElements _turnToHeadOffset = new targetTransformElements();
        [JsonProperty("MovementScript")]
        private movementScriptElements _movementScript= new movementScriptElements();
        [JsonProperty("CameraLock")]
        private cameraLockElements _cameraLock = new cameraLockElements();
        [JsonProperty("CameraExtensions")]
        private cameraExtensionsElements _cameraExtensions = new cameraExtensionsElements();
        [JsonProperty("Multiplayer")]
        private multiplayerElemetns _multiplayer = new multiplayerElemetns();
        [JsonProperty("VMCProtocol")]
        private vmcProtocolElements _vmcProtocol = new vmcProtocolElements();

        public bool thirdPerson { 
            get { 
                return _cameraType == CameraType.ThirdPerson; 
            } 
            set {
                if (value)
                    _cameraType = CameraType.ThirdPerson;
                else
                    _cameraType = CameraType.FirstPesron;
                if (cam != null)
                {
                    cam._quad.gameObject.SetActive(thirdPerson && PreviewCamera);
                    SetCullingMask();
                }
            }
        }
        public float fov { get => _fieldOfView; set { _fieldOfView = value; } }
        public int layer { get => _layer; set { _layer = value; } }
        public int antiAliasing { get => _antiAliasing; set { _antiAliasing = value; } }
        public float renderScale { get => _renderScale; set { _renderScale = value; } }
        public bool fitToCanvas { get => _windowRect.fitToCanvas; set { _windowRect.fitToCanvas = value; } }
        public int screenPosX { get => _windowRect.x; set { _windowRect.x = value; } }
        public int screenPosY { get => _windowRect.y; set { _windowRect.y = value; } }
        public int screenWidth { get => _windowRect.width; set { _windowRect.width = value; } }
        public int screenHeight { get => _windowRect.height; set { _windowRect.height = value; } }
        public float posx { get => _thirdPersonPos.x; set { _thirdPersonPos.x = value; } }
        public float posy { get => _thirdPersonPos.y; set { _thirdPersonPos.y = value; } }
        public float posz { get => _thirdPersonPos.z; set { _thirdPersonPos.z = value; } }
        public targetTransformElements thirdPersonPos { get => _thirdPersonPos; set { _thirdPersonPos = value; } }
        public targetTransformElements thirdPersonRot { get => _thirdPersonRot; set { _thirdPersonRot = value; } }
        public targetTransformElements firstPersonPos { get => _firstPersonPos; set { _firstPersonPos = value; } }
        public targetTransformElements firstPersonRot { get => _firstPersonRot; set { _firstPersonRot = value; } }
        public targetTransformElements turnToHeadOffsetTransform { get => _turnToHeadOffset; set { _turnToHeadOffset = value; } }
        public visibleObjectsElements layerSetting { get => _visibleObject; set { _visibleObject = value; } }
        public movementScriptElements movementScript { get => _movementScript; set { _movementScript = value; } }
        public cameraLockElements cameraLock { get => _cameraLock; set { _cameraLock = value; } }
        public cameraExtensionsElements cameraExtensions { get => _cameraExtensions; set { _cameraExtensions = value; } }
        public multiplayerElemetns multiplayer { get => _multiplayer; set { _multiplayer = value; } }
        public vmcProtocolElements vmcProtocol { get => _vmcProtocol; set { _vmcProtocol = value; } }
        public bool PreviewCamera { get => _visibleObject.previewCamera; 
            set { 
                _visibleObject.previewCamera = value;
                cam?._quad.gameObject.SetActive(thirdPerson && PreviewCamera);
            }
        }
        public bool Avatar { get => _visibleObject.avatar; set { _visibleObject.avatar = value;SetCullingMask(); } }
        public bool UI { get => _visibleObject.ui; set { _visibleObject.ui = value; SetCullingMask(); } }
        public bool Wall { get => _visibleObject.wall; set { _visibleObject.wall = value; SetCullingMask(); } }
        public bool WallFrame { get => _visibleObject.wallFrame; set { _visibleObject.wallFrame = value; SetCullingMask(); } }
        public bool Saber { get => _visibleObject.saber; set { _visibleObject.saber = value; SetCullingMask(); } }
        public bool CutParticles { get => _visibleObject.cutParticles; set { _visibleObject.cutParticles = value; SetCullingMask(); } }
        public bool Notes { get => _visibleObject.notes; set { _visibleObject.notes = value; SetCullingMask(); } }
        public DebriVisibility Debris { get => _visibleObject.debris; set { _visibleObject.debris = value; SetCullingMask(); } }

        private bool _saving = false;
        public event Action<CameraConfig> ConfigChangedEvent;
        private readonly FileSystemWatcher _configWatcher;

        public Vector2 ScreenPosition{
            get{
                return new Vector2(_windowRect.x, _windowRect.y);
            }
        }
        public Vector2 ScreenSize{
            get{
                return new Vector2(_windowRect.width, _windowRect.height);
            }
        }
        public Vector3 Position{
            get{
                return new Vector3(_thirdPersonPos.x, _thirdPersonPos.y, _thirdPersonPos.z);
            }
            set{
                _thirdPersonPos.x = value.x;
                _thirdPersonPos.y = value.y;
                _thirdPersonPos.z = value.z;
            }
        }
        public Vector3 Rotation{
            get{
                return new Vector3(_thirdPersonRot.x, _thirdPersonRot.y, _thirdPersonRot.z);
            }
            set{
                _thirdPersonRot.x = value.x;
                _thirdPersonRot.y = value.y;
                _thirdPersonRot.z = value.z;
            }
        }
        public Vector3 DefaultPosition
        {
            get
            {
                return new Vector3(0f, 2f, -1.2f);
            }
        }
        public Vector3 DefaultRotation
        {
            get
            {
                return new Vector3(15f, 0f, 0f);
            }
        }

        public Vector3 FirstPersonPositionOffset
        {
            get
            {
                return new Vector3(_firstPersonPos.x, _firstPersonPos.y, _firstPersonPos.z);
            }
            set
            {
                _firstPersonPos.x = value.x;
                _firstPersonPos.y = value.y;
                _firstPersonPos.z = value.z;
            }
        }
        public Vector3 FirstPersonRotationOffset
        {
            get
            {
                return new Vector3(_firstPersonRot.x, _firstPersonRot.y, _firstPersonRot.z);
            }
            set
            {
                _firstPersonRot.x = value.x;
                _firstPersonRot.y = value.y;
                _firstPersonRot.z = value.z;
            }
        }
        public Vector3 DefaultFirstPersonPositionOffset
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }
        public Vector3 DefaultFirstPersonRotationOffset
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }
        public Vector3 TurnToHeadOffset
        {
            get{
                return new Vector3(_turnToHeadOffset.x, _turnToHeadOffset.y, _turnToHeadOffset.z);
            }
            set{
                _turnToHeadOffset.x = value.x;
                _turnToHeadOffset.y = value.y;
                _turnToHeadOffset.z = value.z;
            }
        }
        public CameraConfig(string configPath)
        {
            FilePath = configPath;
            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            if (File.Exists(FilePath))
                Load();
            else
            {
                _thirdPersonPos.y = 2.0f;
                _thirdPersonPos.z = -3.0f;
                _thirdPersonRot.x = 15.0f;
            }
            Save();

            _configWatcher = new FileSystemWatcher(Path.GetDirectoryName(FilePath))
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(FilePath),
                EnableRaisingEvents = true
            };
            _configWatcher.Changed += ConfigWatcherOnChanged;
        }
        ~CameraConfig()
        {
            _configWatcher.Changed -= ConfigWatcherOnChanged;
        }

        public void Save()
        {
            _saving = true;
            try{
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex){
                Logger.log.Error($"Failed to save {cam.name}\n{ex}");
            }
            _saving = false;
        }
        public void Load()
        {
            if (File.Exists(FilePath))
                JsonConvert.PopulateObject(File.ReadAllText(FilePath), this);
        }

        private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (_saving){
                _saving = false;
                return;
            }

            Load();

            if (ConfigChangedEvent != null){
                ConfigChangedEvent(this);
            }
        }

        internal virtual void SetCullingMask()
        {
            if (!cam) return;
            int builder = Camera.main.cullingMask;

            if (layerSetting.avatar)
            {
                if (thirdPerson)
                {
                    builder |= 1 << Layer.OnlyInThirdPerson;
                    builder &= ~(1 << Layer.OnlyInFirstPerson);
                }
                else
                {
                    builder |= 1 << Layer.OnlyInFirstPerson;
                    builder &= ~(1 << Layer.OnlyInThirdPerson);
                }
                builder |= 1 << Layer.AlwaysVisible;
            }
            else
            {
                builder &= ~(1 << Layer.OnlyInThirdPerson);
                builder &= ~(1 << Layer.OnlyInFirstPerson);
                builder &= ~(1 << Layer.AlwaysVisible);
            }

            if (layerSetting.debris != DebriVisibility.Link)
            {
                if (layerSetting.debris == DebriVisibility.Visible)
                    builder |= (1 << Layer.NotesDebriLayer);
                else
                    builder &= ~(1 << Layer.NotesDebriLayer);
            }
            else
            {
                if (PlayerSettingPatch.playerSetting!=null)
                {
                    if (!PlayerSettingPatch.playerSetting.reduceDebris)
                        builder |= (1 << Layer.NotesDebriLayer);
                    else
                        builder &= ~(1 << Layer.NotesDebriLayer);
                    Logger.log.Notice($"Debris {PlayerSettingPatch.playerSetting.reduceDebris.ToString()}");
                }
            }

            if (layerSetting.ui)
                builder |= (1 << Layer.UI);
            else
                builder &= ~(1 << Layer.UI);

            if (layerSetting.saber)
                builder |= 1 << Layer.Saber;
            else
                builder &= ~(1 << Layer.Saber);

            if (layerSetting.wall)
                builder |= 1 << TransparentWallsPatch.WallLayerMask;
            else
                builder &= ~(1 << TransparentWallsPatch.WallLayerMask);

            if (layerSetting.wallFrame)
                builder |= 1 << Layer.Obstracle;
            else
                builder &= ~(1 << Layer.Obstracle);

            if (layerSetting.notes)
            {
                builder &= ~(1 << Layer.CustomNoteLayer);
                builder |= 1 << Layer.Notes;
            }
            else
            {
                builder &= ~(1 << Layer.CustomNoteLayer);
                builder &= ~(1 << Layer.Notes);
            }

            cam._cam.cullingMask = builder;
        }
    }



    public class cameraLockElements
    {
        [JsonProperty("LockScreen")]
        public bool lockScreen = false;
        [JsonProperty("LockCamera")]
        public bool lockCamera = false;
        [JsonProperty("DontSaveCameraDrag")]
        public bool dontSaveDrag = false;
    }
    public class visibleObjectsElements
    {
        [JsonProperty("PreviewCamera")]
        public bool previewCamera = true;
        [JsonProperty("Avatar")]
        public bool avatar = true;
        [JsonProperty("UI")]
        public bool ui = true;
        [JsonProperty("Wall")]
        public bool wall = true;
        [JsonProperty("WallFrame")]
        public bool wallFrame = true;
        [JsonProperty("Saber")]
        public bool saber = true;
        [JsonProperty("CutParticles")]
        public bool cutParticles = true;
        [JsonProperty("Notes")]
        public bool notes = true;
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("Debris")]
        internal DebriVisibility debris = DebriVisibility.Link;
    }
    public class cameraExtensionsElements
    {
        [JsonProperty("PositionSmooth")]
        public float positionSmooth = 10.0f;
        [JsonProperty("RotationSmooth")]
        public float rotationSmooth = 5.0f;
        [JsonProperty("Rotation360Smooth")]
        public float rotation360Smooth = 2.0f;
        [JsonProperty("FirstPersonCameraForceUpRight")]
        public bool firstPersonCameraForceUpRight = false;
        [JsonProperty("Follow360Map")]
        public bool follow360map = false;
        [JsonProperty("Follow360MapUseLegacyProcess")]
        public bool follow360mapUseLegacyProcess = false;
        [JsonProperty("FollowNoodlePlayerTrack")]
        public bool followNoodlePlayerTrack = true;
        [JsonProperty("TurnToHead")]
        public bool turnToHead = false;
    }
    public class windowRectElement
    {
        [JsonProperty("FitToCanvas")]
        public bool fitToCanvas = false;
        public int x = 0;
        public int y = 0;
        public int width = Screen.width;
        public int height = Screen.height;
    }
    public class targetTransformElements
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
    public class movementScriptElements
    {
        [JsonProperty("MovementScript")]
        public string movementScript = string.Empty;
        [JsonProperty("UseAudioSync")]
        public bool useAudioSync = true;
        [JsonProperty("SongSpecificScript")]
        public bool songSpecificScript = false;
    }
    public class multiplayerElemetns
    {
        [JsonProperty("TargetPlayerNumber")]
        public int targetPlayerNumber = 0;
        [JsonProperty("DisplayPlayerInfo")]
        public bool displayPlayerInfo = false;
    }

    public class vmcProtocolElements
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("Mode")]
        internal VMCProtocolMode mode = VMCProtocolMode.Disable;
        [JsonProperty("Address")]
        public string address = "127.0.0.1";
        [JsonProperty("Port")]
        public int port = 39540;
    }


}
