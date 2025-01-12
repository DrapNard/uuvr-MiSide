using System;
using UnityEngine;

namespace Uuvr.VrUi
{
    /// <summary>
    /// Handles the overlay render mode for UI, projecting the UI onto a specific camera layer.
    /// </summary>
    public class UiOverlayRenderMode : UuvrBehaviour
    {
        // Overlay camera that sees the UI quad where the captured UI is projected.
        private Camera? _uiSceneCamera;

        public UiOverlayRenderMode(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Called when a configuration setting changes. Updates the culling mask for the overlay camera.
        /// </summary>
        protected override void OnSettingChanged()
        {
            base.OnSettingChanged();

            if (_uiSceneCamera != null)
            {
                _uiSceneCamera.cullingMask = 1 << LayerHelper.GetVrUiLayer();
                Debug.Log("UiOverlayRenderMode: Updated culling mask for UI scene camera.");
            }
        }

        /// <summary>
        /// Unity's Awake method. Sets up the overlay camera.
        /// </summary>
        private void Awake()
        {
            // Create and configure the UI scene camera
            var poseDriver = Create<UuvrPoseDriver>(transform);
            _uiSceneCamera = poseDriver.gameObject.AddComponent<Camera>();

            // Ensure the camera is ignored by the main VR camera logic
            VrCamera.VrCamera.IgnoredCameras.Add(_uiSceneCamera);

            // Configure camera properties
            _uiSceneCamera.clearFlags = CameraClearFlags.Depth;
            _uiSceneCamera.depth = 100;
            _uiSceneCamera.cullingMask = 1 << LayerHelper.GetVrUiLayer();

            Debug.Log("UiOverlayRenderMode: UI scene camera initialized.");
        }
    }
}
