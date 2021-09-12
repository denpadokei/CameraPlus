using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CameraPlus.Utilities;
using VRUIControls;

namespace CameraPlus.Behaviours
{
    internal class WebCamCalibrator : MonoBehaviour
    {
        private enum CalState
        {
            None,
            WebCamPosition,
            LeftPosition,
            RightPosition,
            CalFinish
        }
        private CalState _state = CalState.None;
        internal Vector3 webCamPosition;
        internal Vector3 webCamRotation;

        private VRPointer _vrPointer = null;
        private float _triggerTime;

        private WebCamTexture _webCamTexture;
        internal GameObject webCamObject = null;
        internal Canvas webCamCanvas = null;
        private RawImage _cursorImage;
        private RectTransform _rectCursor;
        private Vector3 _camPosition;
        private Vector3 _leftPosition;
        private Vector3 _rightPosition;
        private CameraPlusBehaviour _targetBehaviour;
        private bool _vrControllerTriggerd;
        internal void Init()
        {
            _vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().First();
            _triggerTime = 0;
            _vrControllerTriggerd = false;
            _state = CalState.WebCamPosition;
            if (_vrPointer == null)
                Logger.log.Error($"VRController Null");
        }

        private void Update()
        {
            if (_vrPointer != null)
            {
                if (_vrPointer.vrController.triggerValue > 0.9f)
                {
                    _vrControllerTriggerd = true;
                    if (_triggerTime == 0)
                        _triggerTime = Time.time;

                }
                else if (_vrControllerTriggerd)
                {
                    _vrControllerTriggerd = false;
                    _triggerTime = 0;
                    switch (_state)
                    {
                        case CalState.None:
                            break;
                        case CalState.WebCamPosition:
                            _camPosition = _vrPointer.vrController.transform.position;
                            _state = CalState.LeftPosition;
                            _targetBehaviour.ThirdPersonPos = _camPosition;
                            _targetBehaviour.Config.Position = _camPosition;
                            CalibratorLeft();
                            break;
                        case CalState.LeftPosition:
                            _leftPosition = _vrPointer.vrController.transform.position;
                            _state = CalState.RightPosition;
                            CalibratorRight();
                            break;
                        case CalState.RightPosition:
                            _rightPosition = _vrPointer.vrController.transform.position;

                            _state = CalState.None;

                            float leftDistance = Vector3.Distance(_camPosition, _leftPosition);
                            float rightDistance = Vector3.Distance(_camPosition, _rightPosition);
                            float dist = leftDistance <= rightDistance ? leftDistance / rightDistance : rightDistance / leftDistance;
                            Vector3 leftNormalize;
                            Vector3 rightNormalize;
                            if (leftDistance <= rightDistance)
                            {
                                leftNormalize = _leftPosition;
                                rightNormalize = Vector3.MoveTowards(_camPosition,_rightPosition,dist);
                            }
                            else
                            {
                                leftNormalize = Vector3.MoveTowards(_camPosition, _leftPosition, dist);
                                rightNormalize = (_rightPosition);
                            }
                            float fov = Vector3.Angle(leftNormalize - _camPosition, rightNormalize - _camPosition);
                            Vector3 center = (leftNormalize + rightNormalize) * 0.5f;
                            _targetBehaviour.ThirdPersonRot = Quaternion.LookRotation(center - _camPosition).eulerAngles;
                            _targetBehaviour.Config.Rotation = Quaternion.LookRotation(center - _camPosition).eulerAngles;
                            _targetBehaviour.FOV = fov;
                            _targetBehaviour.Config.fov = fov;
                            Plugin.cameraController.DestroyCalScreen();
                            break;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if(_webCamTexture)
                _webCamTexture.Stop();
        }

        private void CalibratorLeft()
        {
            //-240, 135
            _rectCursor.localPosition = new Vector3(-135, 0, -0.1f);
        }
        private void CalibratorRight()
        {
            //+240
            _rectCursor.localPosition = new Vector3(135, 0, -0.1f);
        }

        internal void AddCalibrationScreen(CameraPlusBehaviour camplus, Camera camera)
        {
            _targetBehaviour = camplus;

            webCamObject = new GameObject("WebCamCanvas");
            webCamObject.transform.SetParent(this.transform);

            webCamCanvas = webCamObject.gameObject.AddComponent<Canvas>();
            webCamCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            webCamCanvas.worldCamera = camera;

            webCamCanvas.planeDistance = 1;
            CanvasScaler canvasScaler = webCamObject.gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.matchWidthOrHeight = 1;
            GameObject canvObj = new GameObject("RawImage");
            canvObj.transform.SetParent(webCamObject.transform);
            canvObj.transform.localPosition = Vector3.zero;
            canvObj.transform.localEulerAngles = Vector3.zero;
            RawImage raw = canvObj.AddComponent<RawImage>();

            var rect = canvObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = new Vector3(-1f, 1f, 1);
            rect.anchoredPosition = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(Screen.width/4, Screen.height/4);
            rect.localPosition = new Vector3(0, 0, 0);

            _webCamTexture = new WebCamTexture(camplus.Config.webCamera.name);
            raw.texture = _webCamTexture;
            Material rawMaterial = new Material(Plugin.cameraController.Shaders["ChromaKey/Unlit/Cutout"]);
            rawMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0));
            rawMaterial.SetColor("_ChromaKeyColor", Color.blue);
            rawMaterial.SetFloat("_Cull", 0);
            raw.material = rawMaterial;
            _webCamTexture.Play();

            _cursorImage = new GameObject("CursorImage").AddComponent<RawImage>();
            _cursorImage.transform.SetParent(webCamCanvas.transform);
            _cursorImage.transform.localPosition = Vector3.zero;
            _cursorImage.transform.localEulerAngles = Vector3.zero;
            _cursorImage.texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Xross.png");
            Material cursorMat = new Material(Plugin.cameraController.Shaders["ChromaKey/Unlit/Cutout"]);
            cursorMat.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0));
            cursorMat.SetColor("_ChromaKeyColor", Color.white);
            cursorMat.SetFloat("_ChromaKeyHueRange", 0.5f);
            _cursorImage.material = cursorMat;
            _rectCursor = _cursorImage.GetComponent<RectTransform>();
            _rectCursor.anchorMin = new Vector2(0.5f, 0.5f);
            _rectCursor.anchorMax = new Vector2(0.5f, 0.5f);
            _rectCursor.pivot = new Vector2(0.5f, 0.5f);
            _rectCursor.localScale = new Vector3(1f, 1f, 1);
            _rectCursor.anchoredPosition = new Vector2(0, 0);
            _rectCursor.sizeDelta = new Vector2(Screen.width / 16, Screen.height / 9);
            _rectCursor.localPosition = new Vector3(0, 0, -0.1f);
        }
    }
}
