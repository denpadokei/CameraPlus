﻿using System;
using HarmonyLib;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch(typeof(MainCamera))]
	internal class MainCameraPatch
	{
		internal static bool isGameCameraEnable = false;
		internal static Camera gameMainCamera = null;
		[HarmonyPostfix]
		[HarmonyPatch("OnEnable", 0)]
		private static void OnEnablePostfix(Camera ____camera)
		{
			if (____camera.name == "MainCamera")
			{
				isGameCameraEnable = true;
				gameMainCamera = ____camera;
			}
		}
	}
}
