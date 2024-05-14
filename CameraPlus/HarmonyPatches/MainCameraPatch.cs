using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch(typeof(MainCamera))]
	internal class MainCameraPatch
	{
		static MainCameraPatch()
		{
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
		}

        private static void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
			isGameCameraEnable = arg1.name == "GameCore";
        }

        [HarmonyPrepare]
        public static bool SetMultipliersPrefixPrepare(MethodBase original)
        {
            return typeof(MainCamera).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
        }

        internal static bool isGameCameraEnable { get; set; } = false;
		internal static Camera gameMainCamera { get; set; } = null;
		[HarmonyPostfix]
		//[HarmonyPatch("OnEnable", 0)]
		private static void OnEnablePostfix(Camera ____camera)
		{
			if (____camera.name == "MainCamera")
			{
				isGameCameraEnable = true;
				gameMainCamera = ____camera;
			}
		}

		[HarmonyTargetMethod]
		static MethodBase TargetMethod()
		{
			return typeof(MainCamera).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
	}
}
