#define WITH_VMCA
using UnityEngine;

#if WITH_VMCA
using VMCP = global::VMCProtocol;
#endif

namespace CameraPlus.VMCProtocol
{
#if WITH_VMCA
    public class VMCAvatarMarionette : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotate;
        public float fov;
        public bool receivedData = false;
        public virtual void OnEnable()
        {
            if (Plugin.cameraController.existsVMCAvatar)
            {
                var vmcProtocol = GameObject.Find("VMCProtocol");
                if (vmcProtocol != null)
                {
                    var marionette = vmcProtocol.GetComponent<VMCP.IMarionette>();
                    marionette.CameraTransformAndFov += OnCameraPosition;
                }
            }
        }

        private void OnCameraPosition(Pose _pose, float _fov)
        {
            position = _pose.position;
            rotate = _pose.rotation;
            fov = _fov;
            receivedData = true;
        }
    }
#endif
}
