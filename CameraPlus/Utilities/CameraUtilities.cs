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
    public class CameraUtilities
    {
        #region ** DefineStatic **
        internal static string profilePath = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Profiles");
        internal static string configPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name);
        internal static string scriptPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts");
        internal static string currentlySelected = "None";

        internal static float[] mouseMoveSpeed = { -0.01f, -0.01f };//x, y
        internal static float mouseScrollSpeed = 0.5f;
        internal static float[] mouseRotateSpeed = { -0.05f, 0.05f, 1f };//x, y, z
        internal static bool movementScriptEditMode = false;

        internal static Texture2D seekBarBackground = null;
        internal static Texture2D seekBar = null;
        #endregion

        private static bool CameraExists(string cameraName)
        {
            return Plugin.cameraController.Cameras.Keys.Where(c => c == $"{cameraName}.json").Count() > 0;
        }

        internal static void AddNewCamera(string cameraName, CameraConfig CopyConfig = null)
        {
            string path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, $"{cameraName}.json");
            if (!PluginConfig.Instance.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
                path = Path.Combine(profilePath, Plugin.cameraController.currentProfile, $"{cameraName}.json");

            if (!File.Exists(path))
            {
                // Try to copy their old config file into the new camera location
                if (cameraName == Plugin.MainCamera)
                {
                    string oldPath = Path.Combine(Environment.CurrentDirectory, $"{Plugin.MainCamera}.json");
                    if (File.Exists(oldPath))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            Directory.CreateDirectory(Path.GetDirectoryName(path));

                        File.Move(oldPath, path);
                        Logger.log.Notice($"Copied old {Plugin.MainCamera}.json into new {Plugin.Name} folder in UserData");
                    }
                }

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
                Logger.log.Notice($"Success creating new camera \"{cameraName}\"");
            }
            else
            {
                Logger.log.Notice($"Camera \"{cameraName}\" already exists!");
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
                    Logger.log.Warn("One does not simply remove the main camera!");
                }
            }
            catch (Exception ex)
            {
                string msg
                    = ((instance != null && instance.Config != null && instance.Config.FilePath != null)
                    ? $"Could not remove camera with configuration: '{Path.GetFileName(instance.Config.FilePath)}'."
                    : $"Could not remove camera.");

                Logger.log.Error($"{msg} CameraUtilities.RemoveCamera() threw an exception:" +
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
                Logger.log.Error($"Exception cameras culling! Exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
        }

        internal static void ReloadCameras()
        {
            try
            {
                if (!Directory.Exists(configPath))
                    Directory.CreateDirectory(configPath);

                string[] files = Directory.GetFiles(configPath);

                if (!PluginConfig.Instance.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
                    files = Directory.GetFiles(Path.Combine(profilePath, Plugin.cameraController.currentProfile));

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".json") && !Plugin.cameraController.Cameras.ContainsKey(fileName))
                    {
                        Logger.log.Notice($"Found config {filePath}!");

                        CameraConfig Config = new CameraConfig(filePath);
                        if (Config.configLoaded)
                        {
                            var cam = new GameObject($"CamPlus_{fileName}").AddComponent<CameraPlusBehaviour>();
                            Plugin.cameraController.Cameras.TryAdd(fileName, cam);
                            cam.Init(Config);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Exception while reloading cameras! Exception:" +
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

        internal static void ProfileChange(String ProfileName)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(profilePath, ProfileName));
            if (!dir.Exists)
                return;
            ClearCameras();
            Plugin.cameraController.currentProfile = ProfileName;

            if (PluginConfig.Instance.ProfileLoadCopyMethod && ProfileName != null)
                SetProfile(ProfileName);

            CameraUtilities.ReloadCameras();
        }

        internal static void ClearCameras()
        {
            var cs = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>();

            if (PluginConfig.Instance.ProfileLoadCopyMethod)
            {
                foreach (var c in cs)
                    CameraUtilities.RemoveCamera(c);
            }
            foreach (var csi in Plugin.cameraController.Cameras.Values)
                GameObject.Destroy(csi.gameObject);
            Plugin.cameraController.Cameras.Clear();
        }

        public static void CreateExampleScript()
        {
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);
            string defaultScript = Path.Combine(scriptPath, "ExampleMovementScript.json");
            if (!File.Exists(defaultScript))
                File.WriteAllBytes(defaultScript, CustomUtils.GetResource(Assembly.GetExecutingAssembly(), "CameraPlus.Resources.ExampleMovementScript.json"));
        }

        #region ** Profile **

        internal static void CreateMainDirectory()
        {
            DirectoryInfo di = Directory.CreateDirectory(profilePath);
            //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(profilePath);
            var a = new DirectoryInfo(profilePath).GetDirectories();
            if (a.Length > 0)
                currentlySelected = a.First().Name;
        }

        internal static void SaveCurrent()
        {
            string cPath = configPath;
            if (!PluginConfig.Instance.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
            {
                cPath = Path.Combine(profilePath, Plugin.cameraController.currentProfile);
            }
            DirectoryCopy(cPath, Path.Combine(profilePath, GetNextProfileName()), false);
        }

        internal static void SetNext(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(profilePath).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index < dis.Count() - 1)
                    currentlySelected = dis.ElementAtOrDefault(index + 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        internal static void TrySetLast(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(profilePath).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index == 0 && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(dis.Count() - 1).Name;
                else if (index < dis.Count() && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(index - 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        internal static void DeleteProfile(string name)
        {
            if (Directory.Exists(Path.Combine(profilePath, name)))
                Directory.Delete(Path.Combine(profilePath, name), true);
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
            DirectoryInfo dir = new DirectoryInfo(profilePath);
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
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(profilePath, name));
            if (!dir.Exists)
                return;
            DirectoryInfo di = new DirectoryInfo(configPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            DirectoryCopy(dir.FullName, configPath, false);
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
        #endregion
    }
}
