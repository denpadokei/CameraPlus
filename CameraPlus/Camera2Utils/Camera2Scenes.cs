using System.Collections.Generic;
using Newtonsoft.Json;

namespace CameraPlus.Camera2Utils
{
    //refer from Camera2 v0.5.3.
    public enum SceneTypes
    {
        Menu,
        MultiplayerMenu,
        Playing,
        Playing360,
        PlayingModmap,
        PlayingMulti,
        SpectatingMulti,
        Replay,
        FPFC
    }
    public class customScenesElement { }
    public class sceneBindingsElement{ }
    [JsonObject("Camera2Scenes")]
    public class Camera2Scenes
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<SceneTypes, List<string>> scenes = new Dictionary<SceneTypes, List<string>>();
        [JsonProperty("customScenes")]
        public customScenesElement customScenes { get; set; }
        [JsonProperty("sceneBindings")]
        public sceneBindingsElement sceneBindings { get; set; }
        public bool enableAutoSwitch { get; set; }
        public bool autoswitchFromCustom { get; set; }
    }

}
