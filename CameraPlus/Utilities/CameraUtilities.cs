using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using IPA.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;
using CameraPlus.Configuration;
using CameraPlus.Behaviours;
using CameraPlus.HarmonyPatches;

namespace CameraPlus.Utilities
{
    public static class CameraUtilities
    {
        #region ** DefineStatic **
        public static string ProfilePath = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Profiles");
        public static string ConfigPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name);
        public static string ScriptPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts");
        public static string CurrentlySelected = "None";
        public static string RootProfile = "__Root__";

        internal static float[] mouseMoveSpeed = { -0.01f, -0.01f };//x, y
        internal static float mouseScrollSpeed = 0.5f;
        internal static float[] mouseRotateSpeed = { -0.05f, 0.05f, 1f };//x, y, z

        public static int BaseCullingMask = 0;
        #endregion

        private static bool CameraExists(string cameraName)
        {
            return Plugin.cameraController.Cameras.Keys.Where(c => c == $"{cameraName}.json").Count() > 0;
        }

        internal static void AddNewCamera(string cameraName, CameraConfig CopyConfig = null)
        {
            string path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, $"{cameraName}.json");
            if (Plugin.cameraController.CurrentProfile != null)
                path = Path.Combine(ProfilePath, Plugin.cameraController.CurrentProfile, $"{cameraName}.json");

            if (!File.Exists(path))
            {
                CameraConfig config = null;
                if (CopyConfig != null)
                    File.Copy(CopyConfig.FilePath, path, true);

                config = new CameraConfig(path);
                foreach (CameraPlusBehaviour c in Plugin.cameraController.Cameras.Values.OrderBy(i => i.Config.layer))
                {
                    if (c.Config.layer > config.layer)
                        config.layer += (c.Config.layer - config.layer);
                    else if (c.Config.layer == config.layer)
                        config.layer++;
                }

                if (cameraName == Plugin.MainCamera)
                    config.fitToCanvas = true;

                if (CopyConfig == null && cameraName != Plugin.MainCamera)
                {
                    config.screenHeight /= 4;
                    config.screenWidth /= 4;
                }

                config.Position = config.DefaultPosition;
                config.Rotation = config.DefaultRotation;
                config.FirstPersonPositionOffset = config.DefaultFirstPersonPositionOffset;
                config.FirstPersonRotationOffset = config.DefaultFirstPersonRotationOffset;
                config.Save();
                Plugin.Log.Notice($"Success creating new camera \"{cameraName}\"");
            }
            else
            {
                Plugin.Log.Notice($"Camera \"{cameraName}\" already exists!");
            }
        }

        internal static string GetNextCameraName()
        {
            int index = 1;
            string cameraName = String.Empty;
            while (true)
            {
                cameraName = $"customcamera{index.ToString()}";
                if (!CameraUtilities.CameraExists(cameraName))
                    break;

                index++;
            }
            return cameraName;
        }

        internal static bool RemoveCamera(CameraPlusBehaviour instance, bool delete = true)
        {
            try
            {
                if (Path.GetFileName(instance.Config.FilePath) != $"{Plugin.MainCamera}.json")
                {
                    if (Plugin.cameraController.Cameras.TryRemove(Plugin.cameraController.Cameras.Where(c => c.Value == instance && c.Key != $"{Plugin.MainCamera}.json")?.First().Key, out var removedEntry))
                    {
                        if (delete)
                        {
                            if (File.Exists(removedEntry.Config.FilePath))
                                File.Delete(removedEntry.Config.FilePath);
                        }

                        GL.Clear(false, true, Color.black, 0);
                        GameObject.Destroy(removedEntry.gameObject);
                        return true;
                    }
                }
                else
                {
                    Plugin.Log.Warn("One does not simply remove the main camera!");
                }
            }
            catch (Exception ex)
            {
                string msg
                    = ((instance != null && instance.Config != null && instance.Config.FilePath != null)
                    ? $"Could not remove camera with configuration: '{Path.GetFileName(instance.Config.FilePath)}'."
                    : $"Could not remove camera.");

                Plugin.Log.Error($"{msg} CameraUtilities.RemoveCamera() threw an exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
            return false;
        }

        internal static void SetAllCameraCulling()
        {
            try
            {
                foreach (CameraPlusBehaviour c in Plugin.cameraController.Cameras.Values.ToArray())
                {
                    c.Config.SetCullingMask();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"Exception cameras culling! Exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void LoadProfile(string profileName)
        {
            string workProfile;
            GameObject profileObject;

            workProfile = (profileName != string.Empty ? profileName : RootProfile);

            if (!Plugin.cameraController.LoadedProfile.ContainsKey(workProfile))
            {
                profileObject = new GameObject(workProfile);
                Plugin.cameraController.LoadedProfile.TryAdd(workProfile, profileObject);
                profileObject.name = workProfile;
                profileObject.transform.SetParent(Plugin.cameraController.gameObject.transform);
                profileObject.transform.localPosition = Vector3.zero;
                profileObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                profileObject = Plugin.cameraController.LoadedProfile[workProfile];
            }

            try
            {
                if (!Directory.Exists(ConfigPath))
                    Directory.CreateDirectory(ConfigPath);

                string[] files = (profileName != string.Empty ? Directory.GetFiles(Path.Combine(ProfilePath, profileName)) : Directory.GetFiles(ConfigPath));
                string fileName, dictKey;
                foreach (string filePath in files)
                {
                    fileName = Path.GetFileName(filePath);
                    dictKey = (profileName == string.Empty ? $"{Plugin.Name}_{fileName}" : $"{profileName}_{fileName}");
                    if (fileName.EndsWith(".json") && !Plugin.cameraController.Cameras.ContainsKey(dictKey))
                    {
                        Plugin.Log.Notice($"Found new CameraConfig {filePath}!");

                        CameraConfig Config = new CameraConfig(filePath);
                        if (Config.configLoaded)
                        {
                            var cam = new GameObject(dictKey).AddComponent<CameraPlusBehaviour>();
                            cam.transform.SetParent(profileObject.transform);
                            cam.Init(Config);
                            Plugin.cameraController.Cameras.TryAdd(dictKey, cam);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"Exception while loading profile cameras! Exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
        }

        internal static string[] MovementScriptList()
        {
            string[] spath = Directory.GetFiles(Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts"), "*.json");
            string[] scriptList = new string[spath.Length];
            for (int i = 0; i < spath.Length; i++)
                scriptList[i] = Path.GetFileName(spath[i]);
            return scriptList;
        }
        internal static string CurrentMovementScript(string scriptPath)
        {
            return Path.GetFileName(scriptPath);
        }

        internal static string[] ProfileList()
        {
            return Directory.GetDirectories(ProfilePath);
        }

        internal static void ProfileChange(String ProfileName)
        {
            string profile = (ProfileName == string.Empty ? RootProfile : ProfileName);
            string currentprofile = (Plugin.cameraController.CurrentProfile == string.Empty ? RootProfile : Plugin.cameraController.CurrentProfile);
            Plugin.Log.Notice($"ProfileChange {currentprofile} to {profile}");
            if (!Plugin.cameraController.LoadedProfile.ContainsKey(profile))
            {
                LoadProfile(ProfileName);
            }
            Plugin.cameraController.LoadedProfile[currentprofile].SetActive(false);
            Plugin.cameraController.LoadedProfile[profile].SetActive(true);
            Plugin.cameraController.CurrentProfile = ProfileName;
        }

        internal static void ClearCameras()
        {
            var cs = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>();

            foreach (var csi in Plugin.cameraController.Cameras.Values)
                GameObject.Destroy(csi.gameObject);
            Plugin.cameraController.Cameras.Clear();
        }

        public static void CreateExampleScript()
        {
            if (!Directory.Exists(ScriptPath))
                Directory.CreateDirectory(ScriptPath);
            string defaultScript = Path.Combine(ScriptPath, "ExampleMovementScript.json");
            if (!File.Exists(defaultScript))
                File.WriteAllBytes(defaultScript, CustomUtils.GetResource(Assembly.GetExecutingAssembly(), "CameraPlus.Resources.ExampleMovementScript.json"));
        }

        internal static void CreateMainDirectory()
        {
            if(!Directory.Exists(ProfilePath))
                Directory.CreateDirectory(ProfilePath);
            
            var a = new DirectoryInfo(ProfilePath).GetDirectories();
            if (a.Length > 0)
                CurrentlySelected = a.First().Name;
        }

        internal static void SaveNewProfile()
        {
            string cPath = ConfigPath;
            if (Plugin.cameraController.CurrentProfile != null)
            {
                cPath = Path.Combine(ProfilePath, Plugin.cameraController.CurrentProfile);
            }
            DirectoryCopy(cPath, Path.Combine(ProfilePath, GetNextProfileName()), false);
        }

        internal static void SetNext(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(ProfilePath).GetDirectories();
            if (now == null)
            {
                CurrentlySelected = "None";
                if (dis.Length > 0)
                    CurrentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index < dis.Count() - 1)
                    CurrentlySelected = dis.ElementAtOrDefault(index + 1).Name;
                else
                    CurrentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                CurrentlySelected = "None";
                if (dis.Length > 0)
                    CurrentlySelected = dis.First().Name;
            }
        }

        internal static void TrySetLast(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(ProfilePath).GetDirectories();
            if (now == null)
            {
                CurrentlySelected = "None";
                if (dis.Length > 0)
                    CurrentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index == 0 && dis.Length >= 2)
                    CurrentlySelected = dis.ElementAtOrDefault(dis.Count() - 1).Name;
                else if (index < dis.Count() && dis.Length >= 2)
                    CurrentlySelected = dis.ElementAtOrDefault(index - 1).Name;
                else
                    CurrentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                CurrentlySelected = "None";
                if (dis.Length > 0)
                    CurrentlySelected = dis.First().Name;
            }
        }

        internal static void DeleteProfile(string name)
        {
            if (Directory.Exists(Path.Combine(ProfilePath, name)))
                Directory.Delete(Path.Combine(ProfilePath, name), true);
        }

        internal static string GetNextProfileName(string BaseName = "")
        {
            int index = 1;
            string folName = "CameraPlusProfile";
            string bname;
            if (BaseName == "")
                bname = "CameraPlusProfile";
            else
                bname = BaseName;
            DirectoryInfo dir = new DirectoryInfo(ProfilePath);
            DirectoryInfo[] dirs = dir.GetDirectories($"{bname}*");
            foreach (var dire in dirs)
            {
                folName = $"{bname}{index.ToString()}";
                index++;
            }
            return folName;
        }

        internal static void SetProfile(string name)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(ProfilePath, name));
            if (!dir.Exists)
                return;
            DirectoryInfo di = new DirectoryInfo(ConfigPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            DirectoryCopy(dir.FullName, ConfigPath, false);
        }

        internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                return;

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        internal static void DirectoryCreate(string sourceDirName)
        {
            if (!Directory.Exists(sourceDirName))
                Directory.CreateDirectory(sourceDirName);
        }

        public static string GetFullPath(this GameObject obj)
        {
            return GetFullPath(obj.transform);
        }

        public static string GetFullPath(this Transform t)
        {
            string path = t.name;
            var parent = t.parent;
            while (parent)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }
    }
}
