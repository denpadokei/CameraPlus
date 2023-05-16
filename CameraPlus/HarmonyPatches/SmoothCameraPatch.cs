using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	internal class DisableSmoothCamera
	{
		static bool Prefix()
		{
			return false;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(SmoothCameraController), "ActivateSmoothCameraIfNeeded");
			yield return AccessTools.Method(typeof(SmoothCamera), "OnEnable");
		}
	}

    [HarmonyPatch(typeof(SmoothCameraController), nameof(SmoothCameraController.Start))]
    static class InitOnMainAvailable
    {
        static void Postfix(MainSettingsModelSO ____mainSettingsModel)
        {
            if (!Plugin.cameraController.Initialized)
            {
#if DEBUG
                Plugin.Log.Notice("Initialize in SmoothCameraController");
#endif
				CameraUtilities.BaseCullingMask = Camera.main.cullingMask;
                CameraUtilities.ProfileChange(string.Empty);
                Plugin.cameraController.Initialized = true;
            }
        }
    }
}
