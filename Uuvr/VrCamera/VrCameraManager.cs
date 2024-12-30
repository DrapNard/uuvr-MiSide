using System;
using UnityEngine;

namespace Uuvr.VrCamera
{
    public class VrCameraManager : MonoBehaviour
    {
        private Camera[] _allCameras;

        private void Update()
        {
            if (_allCameras == null || _allCameras.Length < Camera.allCamerasCount)
            {
                _allCameras = new Camera[Camera.allCamerasCount];
            }
            Camera.GetAllCameras(_allCameras);

            for (var index = 0; index < Camera.allCamerasCount; index++)
            {
                var camera = _allCameras[index];
                if (camera == null || camera.targetTexture != null || camera.stereoTargetEye == StereoTargetEyeMask.None) continue;
                if (VrCamera.VrCameras.Contains(camera) || VrCamera.IgnoredCameras.Contains(camera)) continue;

                Debug.Log($"Creating VR camera {camera.name}");
                camera.gameObject.AddComponent<VrCamera>();
            }
        }
    }
}
