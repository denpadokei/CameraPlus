using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	internal class RoomAdjustPatch
    {
		internal static Vector3 position = new Vector3();
		internal static Quaternion rotation = new Quaternion();

		static void Postfix(MonoBehaviour __Instance)
		{
			position = __Instance.transform.position;
			rotation = __Instance.transform.rotation;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(VRCenterAdjust), "Start");
			yield return AccessTools.Method(typeof(VRCenterAdjust), "HandleRoomCenterDidChange");
		}
	}
}
