using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch]
    internal class FPFCPatch
    {
		public static FirstPersonFlyingController instance { get; private set; } = null;
		//public static bool isInstanceFPFC => instance != null;
		public static bool isInstanceFPFC =false;
		static void Postfix(FirstPersonFlyingController __instance)
		{
			instance = __instance;
		}
		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "Start");
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "OnEnable");
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "OnDisable");
		}
		[HarmonyPatch(typeof(FirstPersonFlyingController), "OnEnable")]
		private static class FirstPersonFlyingControllerEnable
        {
			private static void Postfix()
			{
				isInstanceFPFC = true;
				Plugin.cameraController.OnFPFCToggleEvent?.Invoke();
			}
		}
		[HarmonyPatch(typeof(FirstPersonFlyingController),"OnDisable")]
		private static class FirstPersonFlyingControllerDisable
        {
			private static void Postfix()
			{
				isInstanceFPFC = false;
				Plugin.cameraController.OnFPFCToggleEvent?.Invoke();
			}
		}
	}
}
