using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using UnityEngine;

namespace Uuvr.VrCamera
{
    public class VrCameraManager : UuvrBehaviour
    {
        private Camera[] _managedCameras;

        public VrCameraManager(System.IntPtr pointer) : base(pointer)
        {
        }

        private void Update()
        {
            try
            {
                // Ensure the array matches the current number of active cameras
                if (_managedCameras == null || _managedCameras.Length < Camera.allCamerasCount)
                {
                    _managedCameras = new Camera[Camera.allCamerasCount];
                }

                // Populate the array with active cameras
                int cameraCount = Camera.GetAllCameras(_managedCameras);

                for (int index = 0; index < cameraCount; index++)
                {
                    var camera = _managedCameras[index];

                    if (camera == null || camera.targetTexture != null || camera.stereoTargetEye == StereoTargetEyeMask.None)
                        continue;

                    if (VrCamera.VrCameras.Contains(camera) || VrCamera.IgnoredCameras.Contains(camera))
                        continue;

                    Debug.Log($"Creating VR camera: {camera.name}");
                    camera.gameObject.AddComponent<VrCamera>();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in VrCameraManager.Update: {ex.Message}");
            }
        }
    }
}
