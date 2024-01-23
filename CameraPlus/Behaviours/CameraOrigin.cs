using UnityEngine;
using CameraPlus.HarmonyPatches;
using UnityEngine.UIElements;

namespace CameraPlus.Behaviours
{
    public class CameraOrigin : MonoBehaviour
    {
        internal CameraPlusBehaviour _cameraPlus;
        private Vector3 _position = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private float _yAngle;

        protected virtual void LateUpdate()
        {
            if (_cameraPlus.Config.cameraExtensions.followNoodlePlayerTrack && Plugin.cameraController.origin)
            {
                transform.position = Plugin.cameraController.origin.position;
                transform.rotation = Plugin.cameraController.origin.rotation * Quaternion.Inverse(RoomAdjustPatch.rotation);

                _cameraPlus._originOffset.transform.localPosition = Vector3.zero - RoomAdjustPatch.position;
                _cameraPlus._originOffset.transform.localRotation = Quaternion.identity;

                _position = _cameraPlus._originOffset.transform.position;
                _rotation = _cameraPlus._originOffset.transform.rotation;
            }
            else
            {
                _position = Vector3.zero;
                _rotation = Quaternion.identity;
            }

            if (_cameraPlus.Config.cameraExtensions.follow360map)
            {
                _yAngle = Mathf.LerpAngle(_yAngle, CameraPlusController.instance._beatLineManagerYAngle, Mathf.Clamp(Time.deltaTime * _cameraPlus.Config.cameraExtensions.rotation360Smooth, 0f, 1f));

                _rotation.eulerAngles = new Vector3(_rotation.x, _yAngle + _rotation.y, _rotation.z);
            }

            transform.localPosition = _position;
            transform.localRotation = _rotation;
        }
    }
}
