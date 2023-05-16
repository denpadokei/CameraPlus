using UnityEngine;

namespace CameraPlus.Behaviours
{
    /// <summary>
    /// This is the monobehaviour that goes on the camera that handles
    /// displaying the actual feed from the camera to the screen.
    /// </summary>
    public class ScreenCameraBehaviour : MonoBehaviour
    {
        private Camera _cam;
        private Camera _parentCam;
        private CameraPlusBehaviour _parent;
        private RenderTexture _renderTexture;
        private Material _dofMaterial;
        private Material _wipeMaterial;
        private Material _outlineMaterial;
        private Material _glitchMaterial;
        public void SetRenderTexture(RenderTexture renderTexture,CameraPlusBehaviour parent=null)
        {
            _renderTexture = renderTexture;
            Plugin.Log.Debug($"[Rendertexture in SetRenderTexture] size w:{renderTexture.width}, h:{renderTexture.height}");
            if(parent != null)
            {
                _parent = parent;
                _parentCam = _parent._cam;
            }
            else
                _parentCam = _cam;
        }

        public void SetCameraInfo(Vector2 position, Vector2 size, int layer)
        {
            Plugin.Log.Debug($"[RenderTexture in SetCameraInfo] position x:{position.x}, y:{position.y} / size w:{size.x}, h:{size.y}");
            _cam.pixelRect = new Rect(position, size);
            _cam.depth = layer;
        }

        public void SetLayer(int layer)
        {
            _cam.depth = layer;
        }

        public void Awake()
        {
#if DEBUG
            Plugin.Log.Notice("Created new screen camera behaviour component!");
#endif
            _cam = gameObject.AddComponent<Camera>();
            _cam.clearFlags = CameraClearFlags.Nothing;
            _cam.cullingMask = 0;
            _cam.stereoTargetEye = StereoTargetEyeMask.None;
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_renderTexture == null) return;

            if (_parent)
            {
                if (_parent.effectElements.enableDOF)
                {
                    if (_dofMaterial == null) _dofMaterial = new Material(Plugin.cameraController.Shaders["Effect/DepthOfFieldGauss"]);
                    if (_parentCam.depthTextureMode != (DepthTextureMode.Depth))
                        _parentCam.depthTextureMode = DepthTextureMode.Depth;

                    RenderTexture temp1 = RenderTexture.GetTemporary(_parentCam.pixelWidth, _parentCam.pixelHeight, 0, RenderTextureFormat.Default);

                    _dofMaterial.SetFloat("_FocusDistance", _parent.effectElements.dofFocusDistance);
                    _dofMaterial.SetFloat("_FocusRange", _parent.effectElements.dofFocusRange);
                    _dofMaterial.SetFloat("_BlurRadius", _parent.effectElements.dofBlurRadius);

                    Graphics.Blit(_renderTexture, temp1, _dofMaterial, 0);
                    Graphics.Blit(temp1, _renderTexture, _dofMaterial, 1);
                    RenderTexture.ReleaseTemporary(temp1);
                }
                /*
                //If the Gauss filter is too heavy, try this one.
                if (_parent.effectElements.enableDOF)
                {
                    if (_dofMaterial == null) _dofMaterial = new Material(Plugin.cameraController.Shaders["Effect/DepthOfFieldDownSample"]);
                    if (_parentCam.depthTextureMode != (DepthTextureMode.Depth))
                        _parentCam.depthTextureMode = DepthTextureMode.Depth;
                    var _nearBlurScale = 1;
                    var _downSample = 1;
                    var _samplerScale = 1;
                    Mathf.Clamp(_nearBlurScale, _parentCam.nearClipPlane, _parentCam.farClipPlane);
                    RenderTexture temp1 = RenderTexture.GetTemporary(_renderTexture.width >> _downSample, _renderTexture.height >> _downSample, 0, _renderTexture.format);
                    RenderTexture temp2 = RenderTexture.GetTemporary(_renderTexture.width >> _downSample, _renderTexture.height >> _downSample, 0, _renderTexture.format);

                    Graphics.Blit(_renderTexture, temp1);

                    _dofMaterial.SetVector("_offsets", new Vector4(0, _samplerScale, 0, 0));
                    Graphics.Blit(temp1, temp2, _dofMaterial, 0);
                    _dofMaterial.SetVector("_offsets", new Vector4(_samplerScale, 0, 0, 0));
                    Graphics.Blit(temp2, temp1, _dofMaterial, 0);

                    _dofMaterial.SetTexture("_BlurTex", temp1);
                    _dofMaterial.SetFloat("_focalDistance", _parentCam.WorldToViewportPoint((_parent.effectElements.dofFocusDistance - _parentCam.nearClipPlane) * _parentCam.transform.forward + _parentCam.transform.position).z / (_parentCam.farClipPlane - _parentCam.nearClipPlane));
                    _dofMaterial.SetFloat("_nearBlurScale", _parent.effectElements.dofFocusRange);
                    _dofMaterial.SetFloat("_farBlurScale", _parent.effectElements.dofBlurRadius);

                    Graphics.Blit(_renderTexture, _renderTexture, _dofMaterial, 1);

                    RenderTexture.ReleaseTemporary(temp1);
                    RenderTexture.ReleaseTemporary(temp2);
                }
                */
                if (_parent.effectElements.enableGlitch)
                {
                    if (_glitchMaterial == null) _glitchMaterial = new Material(Plugin.cameraController.Shaders["Effect/Glitch"]);
                    if (_parentCam.depthTextureMode != (DepthTextureMode.Depth))
                        _parentCam.depthTextureMode = DepthTextureMode.Depth;

                    _glitchMaterial.SetFloat("_LineSpeed", _parent.effectElements.glitchLineSpeed);
                    _glitchMaterial.SetFloat("_LineSize", _parent.effectElements.glitchLineSize);
                    _glitchMaterial.SetFloat("_ColorGap", _parent.effectElements.glitchColorGap);
                    _glitchMaterial.SetFloat("_FrameRate", _parent.effectElements.glitchFrameRate);
                    _glitchMaterial.SetFloat("_Frequency", _parent.effectElements.glitchFrequency);
                    _glitchMaterial.SetFloat("_GlitchScale", _parent.effectElements.glitchScale);

                    Graphics.Blit(_renderTexture, _renderTexture, _glitchMaterial);
                }

                if (_parent.effectElements.enableOutline)
                {
                    if (_parentCam.depthTextureMode != DepthTextureMode.DepthNormals)
                        _parentCam.depthTextureMode = DepthTextureMode.DepthNormals;

                    if (_outlineMaterial == null) _outlineMaterial = new Material(Plugin.cameraController.Shaders["Effect/Outline"]);
                    _outlineMaterial.SetFloat("_EdgeOnly", _parent.effectElements.outlineOnly);
                    _outlineMaterial.SetColor("_EdgeColor", _parent.effectElements.outlineColor);
                    _outlineMaterial.SetColor("_BackgroundColor", _parent.effectElements.outlineBGColor);

                    Graphics.Blit(_renderTexture, _renderTexture, _outlineMaterial);
                }

                if (_parent.effectElements.wipeProgress > 0)
                {
                    if (_wipeMaterial == null) _wipeMaterial = new Material(Plugin.cameraController.Shaders["Effect/Wipe"]);
                    _wipeMaterial.SetFloat("_Progress", _parent.effectElements.wipeProgress);
                    _wipeMaterial.SetVector("_Center", _parent.effectElements.wipeCircleCenter);
                    Graphics.Blit(_renderTexture, dest, _wipeMaterial,
                        _parent.effectElements.wipeType == "Circle" ? 0 :
                        _parent.effectElements.wipeType == "Left" ? 1 :
                        _parent.effectElements.wipeType == "Right" ? 2 :
                        _parent.effectElements.wipeType == "Top" ? 3 : 4);
                    return;
                }
            }
            Graphics.Blit(_renderTexture, dest);
        }
    }
}
