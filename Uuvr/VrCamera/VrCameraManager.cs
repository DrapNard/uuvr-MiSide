using UnityEngine;

namespace Uuvr.VrCamera
{
    public class VrCameraManager : MonoBehaviour
    {
        private void Update()
        {
            try
            {
                // Use FindObjectsOfType to get all active cameras
                var allCameras = Object.FindObjectsOfType<Camera>();

                // Iterate through the cameras and process them
                foreach (var camera in allCameras)
                {
                    if (camera == null || camera.targetTexture != null || camera.stereoTargetEye == StereoTargetEyeMask.None) continue;
                    if (VrCamera.VrCameras.Contains(camera) || VrCamera.IgnoredCameras.Contains(camera)) continue;

                    // Add VrCamera component to unprocessed cameras
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
