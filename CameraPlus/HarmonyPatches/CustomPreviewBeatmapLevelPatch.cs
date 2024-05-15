using HarmonyLib;
using System.IO;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(LevelSelectionNavigationController), nameof(LevelSelectionNavigationController.HandleLevelCollectionNavigationControllerDidChangeLevelDetailContent))]
    internal static class SongScriptBeatmapPatch
    {
        private static readonly string _keyPrefix = "custom_level_";
        private static readonly string _customLevelRoot = CustomLevelPathHelper.customLevelsDirectoryPath;

        private static string _latestSelectedLevelPath = string.Empty;
        public static string customLevelPath = string.Empty;

        private static void Postfix(LevelSelectionNavigationController __instance)
        {
            try
            {
                if (CustomLevelLoaderPatch.Instance._loadedBeatmapSaveData.ContainsKey(__instance.beatmapLevel.levelID))
                {
                    var currentLevelPath = CustomLevelLoaderPatch.Instance._loadedBeatmapSaveData[__instance.beatmapLevel.levelID].customLevelFolderInfo.folderPath;
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
                else
                {
                    customLevelPath = string.Empty;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Log.Error(e);
                customLevelPath = string.Empty;
            }
        }
    }
}
