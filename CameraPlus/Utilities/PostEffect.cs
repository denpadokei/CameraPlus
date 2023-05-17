using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Configuration;
using System.Security.Cryptography;

namespace CameraPlus.Utilities
{
    public static class PostEffect
    {
        public static void DepthOfField(CameraPlusBehaviour cameraPlus,RenderTexture renderTexture, Material material)
        {
            if(cameraPlus == null) return;
            if (material == null) material = new Material(Plugin.cameraController.Shaders["Effect/DepthOfFieldGauss"]);
            if (cameraPlus._cam.depthTextureMode != (DepthTextureMode.Depth))
                cameraPlus._cam.depthTextureMode = DepthTextureMode.Depth;

            RenderTexture temp1 = RenderTexture.GetTemporary(cameraPlus._cam.pixelWidth, cameraPlus._cam.pixelHeight, 0, RenderTextureFormat.Default);

            material.SetFloat("_FocusDistance", cameraPlus.effectElements.dofFocusDistance);
            material.SetFloat("_FocusRange", cameraPlus.effectElements.dofFocusRange);
            material.SetFloat("_BlurRadius", cameraPlus.effectElements.dofBlurRadius);

            Graphics.Blit(renderTexture, temp1, material, 0);
            Graphics.Blit(temp1, renderTexture, material, 1);
            RenderTexture.ReleaseTemporary(temp1);
        }

        public static void Glitch(CameraPlusBehaviour cameraPlus, RenderTexture renderTexture, Material material)
        {
            if (cameraPlus == null) return;
            if (material == null) material = new Material(Plugin.cameraController.Shaders["Effect/Glitch"]);
            if (cameraPlus._cam.depthTextureMode != (DepthTextureMode.Depth))
                cameraPlus._cam.depthTextureMode = DepthTextureMode.Depth;

            material.SetFloat("_LineSpeed", cameraPlus.effectElements.glitchLineSpeed);
            material.SetFloat("_LineSize", cameraPlus.effectElements.glitchLineSize);
            material.SetFloat("_ColorGap", cameraPlus.effectElements.glitchColorGap);
            material.SetFloat("_FrameRate", cameraPlus.effectElements.glitchFrameRate);
            material.SetFloat("_Frequency", cameraPlus.effectElements.glitchFrequency);
            material.SetFloat("_GlitchScale", cameraPlus.effectElements.glitchScale);

            Graphics.Blit(renderTexture, renderTexture, material);
        }
        public static void Outline(CameraPlusBehaviour cameraPlus, RenderTexture renderTexture, Material material)
        {
            if (cameraPlus == null) return;
            if (cameraPlus._cam.depthTextureMode != DepthTextureMode.DepthNormals)
                cameraPlus._cam.depthTextureMode = DepthTextureMode.DepthNormals;

            if (material == null) material = new Material(Plugin.cameraController.Shaders["Effect/Outline"]);
            material.SetFloat("_EdgeOnly", cameraPlus.effectElements.outlineOnly);
            material.SetColor("_EdgeColor", cameraPlus.effectElements.outlineColor);
            material.SetColor("_BackgroundColor", cameraPlus.effectElements.outlineBGColor);

            Graphics.Blit(renderTexture, renderTexture, material);
        }
        public static void Wipe(CameraPlusBehaviour cameraPlus, RenderTexture srcRenderTexture, RenderTexture destRenderTexture, Material material)
        {
            if (cameraPlus == null) return;
            if (material == null) material = new Material(Plugin.cameraController.Shaders["Effect/Wipe"]);
            material.SetFloat("_Progress", cameraPlus.effectElements.wipeProgress);
            material.SetVector("_Center", cameraPlus.effectElements.wipeCircleCenter);
            Graphics.Blit(srcRenderTexture, destRenderTexture, material,
                cameraPlus.effectElements.wipeType == "Circle" ? 0 :
                cameraPlus.effectElements.wipeType == "Left" ? 1 :
                cameraPlus.effectElements.wipeType == "Right" ? 2 :
                cameraPlus.effectElements.wipeType == "Top" ? 3 : 4);
        }

    }
}
