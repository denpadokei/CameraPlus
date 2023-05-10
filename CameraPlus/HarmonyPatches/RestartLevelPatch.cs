using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelRestartController))]
    [HarmonyPatch("RestartLevel")]
    internal class RestartLevelPatch
    {
        static void Prefix()
        {
            Plugin.CameraController.isRestartingSong = true;
        }
    }
}
