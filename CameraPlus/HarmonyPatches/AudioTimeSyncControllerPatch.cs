using HarmonyLib;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(AudioTimeSyncController), "Awake")]
    internal class AudioTimeSyncControllerPatch
    {
        public static AudioTimeSyncController Instance = null;
        static void Postfix(AudioTimeSyncController __instance)
        {
#if DEBUG
            Plugin.Log.Notice("AudioTimeSyncController Awake");
#endif
            Instance = __instance;
        }
    }
    [HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.StartSong))]
    static class HookAudioTimeSyncController
    {
        static void Postfix(AudioTimeSyncController __instance)
        {
#if DEBUG
            Plugin.Log.Notice("AudioTimeSyncController SetAllCameraCulling StartSong");
#endif
            Plugin.cameraController.OnSetCullingMask.Invoke();
        }
    }
    [HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.StopSong))]
    internal class AudioTimeSyncControllerPatch2
    {
        static void Postfix()
        {
            AudioTimeSyncControllerPatch.Instance = null;
        }

    }
}
