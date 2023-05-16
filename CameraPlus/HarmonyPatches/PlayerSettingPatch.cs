using System;
using HarmonyLib;
using UnityEngine;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(PlayerData), "SetPlayerSpecificSettings")]
    internal class PlayerSettingPatch
    {
        internal static PlayerSpecificSettings playerSetting = null;
        internal static void Postfix(PlayerData __instance)
        {
            playerSetting = __instance.playerSpecificSettings;
            if (!MultiplayerSession.ConnectedMultiplay)
            {
#if DEBUG
                Plugin.Log.Notice("PlayerSpecificSettings SetAllCameraCulling");
#endif
                Plugin.cameraController.OnSetCullingMask.Invoke();

            }
        }
    }
}
