using UnityEngine;
using UnityEngine.EventSystems;
using CameraPlus.Configuration;

namespace CameraPlus.Behaviours
{
	internal class CameraPreviewQuad : MonoBehaviour, IPointerClickHandler
	{
		internal CameraPlusBehaviour cam { get; private set; }
		private GameObject _cameraCube;
		private GameObject _cameraQuad;

		internal Material _previewMaterial = new Material(Plugin.cameraController.Shaders["BeatSaber/BlitCopyWithDepth"]);
		internal Material _cubeMaterial = new Material(Plugin.cameraController.Shaders["BeatSaber/BlitCopyWithDepth"]);

		public bool IsDisplayMaterialVROnly
		{
			get{
				return _previewMaterial.GetFloat("_IsVRCameraOnly") == 1;
			}
			set{
				if (value)
				{
					_previewMaterial.SetFloat("_IsVRCameraOnly", 1);
					_cubeMaterial.SetFloat("_IsVRCameraOnly", 1);
				}
				else
				{
					_previewMaterial.SetFloat("_IsVRCameraOnly", 0);
					_cubeMaterial.SetFloat("_IsVRCameraOnly", 0);
				}
			}
		}

		public void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
		public void Init(CameraPlusBehaviour camera)
        {
			cam = camera;
			_cameraCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			DontDestroyOnLoad(_cameraCube);
			_cameraCube.GetComponent<MeshRenderer>().material = _cubeMaterial;
			SetCameraCubeSize(PluginConfig.Instance.CameraCubeSize);
			Plugin.Log.Notice($"Camera Aspect {cam._cam.aspect}");
			_cameraCube.name = "CameraCube";
			_cameraCube.transform.SetParent(transform);
			_cameraCube.transform.localPosition = Vector3.zero;
			_cameraCube.transform.localEulerAngles = Vector3.zero;

			_cameraQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			DontDestroyOnLoad(_cameraQuad);
			DestroyImmediate(_cameraQuad.GetComponent<Collider>());
			_cameraQuad.GetComponent<MeshRenderer>().material = _previewMaterial;
			_cameraQuad.transform.SetParent(transform);
			SetCameraQuadPosition(PluginConfig.Instance.CameraQuadPosition, PluginConfig.Instance.CameraCubeSize);
			IsDisplayMaterialVROnly = cam.Config.PreviewCameraVROnly;
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!(eventData.currentInputModule is VRUIControls.VRInputModule)) return;
			if (cam.Config.cameraLock.lockCamera) return;
			CameraMoverPointer.BeginDragCamera(cam);
		}
		public void SetCameraQuadPosition(string quadPosition, float cubeScale = 1)
        {
			_cameraQuad.transform.localEulerAngles = new Vector3(0, 180, 0);
			SetCameraQuadSize(cam.Config.cameraExtensions.previewCameraQuadScale, cam.Config.cameraExtensions.previewCameraMirrorMode);
			if (quadPosition == "Top")
				_cameraQuad.transform.localPosition = new Vector3(0, _cameraQuad.transform.localScale.y / 2 + _cameraCube.transform.localScale.y / 2, 0.05f * cubeScale);
			else if (quadPosition == "Bottom")
				_cameraQuad.transform.localPosition = new Vector3(0, -_cameraQuad.transform.localScale.y / 2 - _cameraCube.transform.localScale.y / 2, 0.05f * cubeScale);
			else if (quadPosition == "Left")
				_cameraQuad.transform.localPosition = new Vector3(Mathf.Abs(_cameraQuad.transform.localScale.x) / 2 + _cameraCube.transform.localScale.x / 2, 0, 0.05f * cubeScale);
			else if (quadPosition == "Center")
				_cameraQuad.transform.localPosition = new Vector3(0, 0, 0.15f * cubeScale);
			else
				_cameraQuad.transform.localPosition = new Vector3(-Mathf.Abs(_cameraQuad.transform.localScale.x) / 2 - _cameraCube.transform.localScale.x / 2, 0, 0.05f * cubeScale);
		}
		public void SetCameraCubeSize(float cubeScale)
        {
			_cameraCube.transform.localScale = new Vector3(0.15f * cubeScale, 0.15f * cubeScale, 0.22f * cubeScale);
		}

		public void SetCameraQuadSize(float quadScale, bool isMirror=false)
        {
			if(isMirror)
				_cameraQuad.transform.localScale = new Vector3(-1 * 0.15f * (PluginConfig.Instance.CameraQuadStretch ? 1.7778f : cam._cam.aspect) * quadScale, 0.15f * 1 * quadScale, 1); //1.7778f = 16 / 9
			else
				_cameraQuad.transform.localScale = new Vector3(0.15f * (PluginConfig.Instance.CameraQuadStretch ? 1.7778f : cam._cam.aspect) * quadScale, 0.15f * 1 * quadScale, 1); //1.7778f = 16 / 9
		}
	}
}
