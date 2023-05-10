using HarmonyLib;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(AudioTimeSyncController), "Awake")]
    internal class AudioTimeSyncControllerPatch
    {
        public static AudioTimeSyncController Instance = null;
        static void Postfix(AudioTimeSyncController __Instance)
        {
#if DEBUG
            Plugin.Log.Notice("AudioTimeSyncController Awake");
#endif
            Instance = __Instance;
        }
    }
    [HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.StartSong))]
    static class HookAudioTimeSyncController
    {
        static void Postfix(AudioTimeSyncController __Instance)
        {
#if DEBUG
            Plugin.Log.Notice("AudioTimeSyncController StartSong");
#endif
            CameraUtilities.SetAllCameraCulling();
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
