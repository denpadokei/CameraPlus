using HarmonyLib;
using IPA.Loader;
using System;
using System.Reflection;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
    internal class FPFCToggleEnable
    {
        //Temporarily monitor FPFCToggle of SiraUtil due to FPFC change in 1.29.4
        internal static MethodBase TargetMethod()
        {
            PluginMetadata siraUtil = PluginManager.GetPluginFromId("SiraUtil");
            if (siraUtil != null)
            {
                return siraUtil.Assembly.GetType("SiraUtil.Tools.FPFC.FPFCToggle").GetMethod("EnableFPFC", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else return null;
        }
        internal static void Postfix()
        {
            Plugin.cameraController.isFPFC = true;
#if DEBUG
            Plugin.Log.Notice("SiraUtil FPFC Toggle Enable");
#endif
            Plugin.cameraController.OnFPFCToggleEvent.Invoke();
        }
    }

    internal class FPFCToggleDisbale
    {
        internal static MethodBase TargetMethod()
        {
            PluginMetadata siraUtil = PluginManager.GetPluginFromId("SiraUtil");
            if (siraUtil != null)
            {
                return siraUtil.Assembly.GetType("SiraUtil.Tools.FPFC.FPFCToggle").GetMethod("DisableFPFC", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else return null;
        }
        internal static void Postfix()
        {
            Plugin.cameraController.isFPFC = false;
#if DEBUG
            Plugin.Log.Notice("SiraUtil FPFC Toggle False");
#endif
            Plugin.cameraController.OnFPFCToggleEvent.Invoke();
        }
    }
}
