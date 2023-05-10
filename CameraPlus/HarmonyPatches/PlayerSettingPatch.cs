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
        internal static void Postfix(PlayerData __Instance)
        {
            playerSetting = __Instance.playerSpecificSettings;
            if (!MultiplayerSession.ConnectedMultiplay)
                CameraUtilities.SetAllCameraCulling();
        }
    }
}
