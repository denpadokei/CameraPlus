using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using IPA.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(LevelSelectionNavigationController), nameof(LevelSelectionNavigationController.HandleLevelCollectionNavigationControllerDidChangeLevelDetailContent))]
    internal static class SongScriptBeatmapPatch
    {
        private static string _keyPrefix = "custom_level_";
        private static string _customLevelRoot = CustomLevelPathHelper.customLevelsDirectoryPath;

        private static string _latestSelectedLevelPath = string.Empty;
        public static string customLevelPath = string.Empty;
        static void Postfix(LevelSelectionNavigationController __instance)
        {
            if (__instance.beatmapLevel.levelID.Contains(_keyPrefix))
            {
                string currentLevelPath = Path.Combine(_customLevelRoot, __instance.beatmapLevel.levelID.Substring(_keyPrefix.Length));
                if (currentLevelPath != _latestSelectedLevelPath)
                {
                    _latestSelectedLevelPath = currentLevelPath;
#if DEBUG
                    Plugin.Log.Notice($"Selected CustomLevel Path :\n {currentLevelPath}");
#endif
                    if (File.Exists(Path.Combine(currentLevelPath, "SongScript.json")))
                    {
                        customLevelPath = Path.Combine(currentLevelPath, "SongScript.json");
                        Plugin.Log.Notice($"Found SongScript path : \n{currentLevelPath}");
                    }
                    else
                    {
                        customLevelPath = string.Empty;
                    }
                }
            }
        }
    }
}
