using HarmonyLib;
using UnityEngine.XR.OpenXR;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(OpenXRSettings), nameof(OpenXRSettings.ApplyRenderSettings))]
    internal class OpenXRSettingPatch
    {
        public static OpenXRSettings Instance { get; set; } = null;
        public static void Prefix(OpenXRSettings __instance)
        {
            Instance = __instance;
        }
    }
}
