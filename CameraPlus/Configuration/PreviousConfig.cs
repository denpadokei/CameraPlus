using System;
using System.IO;
using UnityEngine;
using CameraPlus.Camera2Utils;

namespace CameraPlus.Configuration
{
    public class PreviousConfig
    {
        public string FilePath { get; }
        public bool LockScreen = false;
        public bool LockCamera = false;
        public bool LockCameraDrag = false;
        public float fov = 50;
        public int antiAliasing = 2;
        public float renderScale = 1;
        public float positionSmooth = 10;
        public float rotationSmooth = 5;
        public float cam360Smoothness = 2;

        public bool cam360RotateControlNew = true;

        public bool thirdPerson = false;
        public bool showThirdPersonCamera = true;
        public bool use360Camera = false;
        public bool turnToHead = false;

        public float posx;
        public float posy = 2;
        public float posz = -3.0f;
        public float angx = 15;
        public float angy;
        public float angz;

        public float firstPersonPosOffsetX;
        public float firstPersonPosOffsetY;
        public float firstPersonPosOffsetZ;
        public float firstPersonRotOffsetX;
        public float firstPersonRotOffsetY;
        public float firstPersonRotOffsetZ;

        public float turnToHeadOffsetX;
        public float turnToHeadOffsetY;
        public float turnToHeadOffsetZ;

        public bool NoodleTrack = false;

        public int screenWidth = Screen.width;
        public int screenHeight = Screen.height;
        public int screenPosX;
        public int screenPosY;

        public int MultiPlayerNumber = 0;
        public bool DisplayMultiPlayerNameInfo = false;

        public int layer = -1000;

        public bool fitToCanvas = false;
        public bool transparentWalls = false;
        public bool forceFirstPersonUpRight = false;
        public bool avatar = true;
        public string debri = "link";
        public bool HideUI = false;
        public bool Saber = true;
        public bool Notes = true;
        public bool WallFrame = true;
        public string movementScriptPath = String.Empty;
        public bool movementAudioSync = true;
        public bool songSpecificScript = false;
        public string VMCProtocolMode = "disable";
        public string VMCProtocolAddress = "127.0.0.1";
        public int VMCProtocolPort = 39540;

        public PreviousConfig(string filePath)
        {
            FilePath = filePath;

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
                ConfigSerializer.LoadConfig(this, FilePath);
        }
    }
}