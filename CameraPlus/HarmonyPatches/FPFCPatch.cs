using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch]
    internal class FPFCPatch
    {
		public static FirstPersonFlyingController instance { get; private set; } = null;
		public static bool isFPFC =false;
		public static Transform FPFCEventSystemTransform { get; private set; } = null;

        [HarmonyPatch(typeof(FirstPersonFlyingController), "OnEnable")]
		private static class FirstPersonFlyingControllerEnable
        {
			private static void Postfix(FirstPersonFlyingController __instance)
			{
				instance = __instance;
				FPFCEventSystemTransform = GameObject.Find("EventSystem").transform;
			}
		}
    }
}
