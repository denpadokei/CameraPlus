using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using HarmonyLib;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using CameraPlus.Configuration;

namespace CameraPlus
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Plugin Instance { get; private set; }
        public static IPALogger Log { get; private set; }
        public static string Name => "CameraPlus";
        public static string MainCamera => "cameraplus";

        private Harmony _harmony;
        public static CameraPlusController CameraController;
        [Init]

        public void Init(Config conf, IPALogger logger)
        {
            Instance = this;
            PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log= logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            _harmony = new Harmony("com.brian91292.beatsaber.cameraplus");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            CameraController = new GameObject("CameraPlusController").AddComponent<CameraPlusController>();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            if(CameraController)
                GameObject.Destroy(CameraController);
            _harmony.UnpatchSelf();
        }
    }
}
