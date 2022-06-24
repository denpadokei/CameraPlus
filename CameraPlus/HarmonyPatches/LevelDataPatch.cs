using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	internal class LevelDataPatch
	{
		public static IDifficultyBeatmap difficultyBeatmap;
		public static GameplayModifiers gameplayModifiers;
		public static bool is360Level = false;

		static void Prefix(IDifficultyBeatmap difficultyBeatmap, GameplayModifiers gameplayModifiers)
		{
#if DEBUG
			Logger.log.Notice("Got level data!");
#endif
			LevelDataPatch.difficultyBeatmap = difficultyBeatmap;

			is360Level = difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.containsRotationEvents;
		}

		internal static void Reset()
		{
			is360Level = false;
			difficultyBeatmap = null;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (var t in new Type[] { typeof(StandardLevelScenesTransitionSetupDataSO), typeof(MissionLevelScenesTransitionSetupDataSO), typeof(MultiplayerLevelScenesTransitionSetupDataSO) })
				yield return t.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
		}
	}
}
