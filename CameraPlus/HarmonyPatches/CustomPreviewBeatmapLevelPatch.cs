using System.IO;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomPreviewBeatmapLevel), nameof(CustomPreviewBeatmapLevel.GetCoverImageAsync))]
    internal class CustomPreviewBeatmapLevelPatch
    {
        private static string _latestSelectedSong = string.Empty;
        public static string customLevelPath = string.Empty;
        static void Postfix(CustomPreviewBeatmapLevel __Instance)
        {
            if (__Instance.customLevelPath != _latestSelectedSong)
            {
                _latestSelectedSong = __Instance.customLevelPath;
#if DEBUG
                Plugin.Log.Notice($"Selected CustomLevel Path :\n {__Instance.customLevelPath}");
#endif
                if (File.Exists(Path.Combine(__Instance.customLevelPath, "SongScript.json")))
                    customLevelPath = Path.Combine(__Instance.customLevelPath, "SongScript.json");
                else
                    customLevelPath = string.Empty;
            }
        }
    }
}
