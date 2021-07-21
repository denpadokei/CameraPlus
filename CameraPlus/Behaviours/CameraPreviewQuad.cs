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

		public void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
		public void Init(CameraPlusBehaviour camera)
        {
			cam = camera;
			_cameraCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			DontDestroyOnLoad(_cameraCube);
			_cameraCube.transform.localScale = new Vector3(0.15f, 0.15f, 0.22f);
			_cameraCube.name = "CameraCube";
			_cameraCube.layer = PluginConfig.Instance.CameraQuadLayer;
			_cameraCube.transform.SetParent(transform);

			_cameraQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			DontDestroyOnLoad(_cameraQuad);
			DestroyImmediate(_cameraQuad.GetComponent<Collider>());
			_cameraQuad.GetComponent<MeshRenderer>().material = _previewMaterial;
			_cameraQuad.transform.SetParent(_cameraCube.transform);
			_cameraQuad.transform.localPosition = new Vector3(-1f * ((cam._cam.aspect - 1) / 2 + 1), 0, 0.22f);
			_cameraQuad.transform.localEulerAngles = new Vector3(0, 180, 0);
			_cameraQuad.transform.localScale = new Vector3(cam._cam.aspect, 1, 1);
			_cameraQuad.layer = PluginConfig.Instance.CameraQuadLayer;
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!(eventData.currentInputModule is VRUIControls.VRInputModule)) return;
			if (cam.Config.cameraLock.lockCamera) return;
			CameraMoverPointer.BeginDragCamera(cam);
		}
	}
}
