using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatLineManager), "Start")]
    internal class BeatLineManagerPatch
    {
        public static BeatLineManager Instance { get; private set; }
        static void Postfix(BeatLineManager __Instance)
        {
#if DEBUG
            Plugin.Log.Notice("BeatLineManager Start");
#endif
            Instance = __Instance;
        }
    }
    [HarmonyPatch(typeof(EnvironmentSpawnRotation), "OnEnable")]
    internal class EnvironmentSpawnRotationPatch
    {
        public static EnvironmentSpawnRotation Instance { get; private set; }
        static void Postfix(EnvironmentSpawnRotation __Instance)
        {
#if DEBUG
            Plugin.Log.Notice("EnvironmentSpawnRotation Start");
#endif
            Instance = __Instance;
        }
    }
}
