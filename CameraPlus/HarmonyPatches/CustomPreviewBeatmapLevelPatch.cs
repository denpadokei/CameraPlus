using System.IO;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomPreviewBeatmapLevel), nameof(CustomPreviewBeatmapLevel.GetCoverImageAsync))]
    internal class CustomPreviewBeatmapLevelPatch
    {
        private static string _latestSelectedSong = string.Empty;
        public static string customLevelPath = string.Empty;
        static void Postfix(CustomPreviewBeatmapLevel __instance)
        {
            if (__instance.customLevelPath != _latestSelectedSong)
            {
                _latestSelectedSong = __instance.customLevelPath;
#if DEBUG
                Plugin.Log.Notice($"Selected CustomLevel Path :\n {__instance.customLevelPath}");
#endif
                if (File.Exists(Path.Combine(__instance.customLevelPath, "SongScript.json")))
                    customLevelPath = Path.Combine(__instance.customLevelPath, "SongScript.json");
                else
                    customLevelPath = string.Empty;
            }
        }
    }
}
