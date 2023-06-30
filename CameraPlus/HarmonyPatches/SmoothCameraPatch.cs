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
        [HarmonyPatch(typeof(SmoothCameraController), "ActivateSmoothCameraIfNeeded")]
        static bool Prefix()
		{
			return false;
		}

	}

    [HarmonyPatch(typeof(SmoothCameraController), "Start")]
    static class InitOnMainAvailable
    {
        static void Postfix(MainSettingsModelSO ____mainSettingsModel)
        {
            if (!Plugin.cameraController.Initialized)
            {
                CameraUtilities.BaseCullingMask = Camera.main.cullingMask;
                CameraUtilities.ProfileChange(string.Empty);
                Plugin.cameraController.Initialized = true;
#if DEBUG
                Plugin.Log.Notice("Profile Initialize in SmoothCameraController.Start");
#endif
            }
        }
    }
}
