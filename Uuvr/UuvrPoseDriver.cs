using System;
using System.Reflection;
using UnityEngine;
using Uuvr.UnityTypesHelper;

namespace Uuvr
{
    public class UuvrPoseDriver : UuvrBehaviour
    {
        private MethodInfo? _trackingRotationMethod;
        private readonly object[] _trackingRotationMethodArgs = { 2 }; // Enum value for XRNode.CenterEye

        public UuvrPoseDriver(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Unity's Awake method. Initializes tracking methods and disables auto camera tracking.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Attempt to find the InputTracking.GetLocalRotation method
            var inputTrackingType = Type.GetType("UnityEngine.XR.InputTracking, UnityEngine.XRModule") ??
                                    Type.GetType("UnityEngine.XR.InputTracking, UnityEngine.VRModule");

            _trackingRotationMethod = inputTrackingType?.GetMethod("GetLocalRotation");

            if (_trackingRotationMethod == null)
            {
                Debug.LogError("Failed to find InputTracking.GetLocalRotation. Destroying UUVR Pose Driver.");
                Destroy(this);
                return;
            }

            // Disable camera auto-tracking
            DisableCameraAutoTracking();
        }

        /// <summary>
        /// Called before rendering. Updates the object's transform.
        /// </summary>
        protected override void OnBeforeRender()
        {
            base.OnBeforeRender();
            UpdateTransform();
        }

        /// <summary>
        /// Unity's Update method. Keeps the transform in sync during gameplay.
        /// </summary>
        private void Update()
        {
            UpdateTransform();
        }

        /// <summary>
        /// Unity's LateUpdate method. Ensures the transform updates after all other changes.
        /// </summary>
        private void LateUpdate()
        {
            UpdateTransform();
        }

        /// <summary>
        /// Updates the object's local rotation based on tracking data.
        /// </summary>
        private void UpdateTransform()
        {
            if (_trackingRotationMethod != null)
            {
                try
                {
                    transform.localRotation = (Quaternion)_trackingRotationMethod.Invoke(null, _trackingRotationMethodArgs);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error updating transform rotation: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Disables automatic camera tracking by Unity.
        /// </summary>
        private void DisableCameraAutoTracking()
        {
            var camera = GetComponent<Camera>();
            if (!camera) return;

            // Attempt to disable auto camera tracking
            var cameraTrackingDisablingMethod = UuvrXrDevice.XrDeviceType?.GetMethod("DisableAutoXRCameraTracking");

            if (cameraTrackingDisablingMethod != null)
            {
                cameraTrackingDisablingMethod.Invoke(null, new object[] { camera, true });
                Debug.Log("Camera auto-tracking disabled using DisableAutoXRCameraTracking.");
            }
            else
            {
                Debug.LogWarning("Failed to find DisableAutoXRCameraTracking method. Using fallback solutions.");
                FallbackDisableCameraAutoTracking(camera);
            }
        }

        /// <summary>
        /// Fallback method to disable camera auto-tracking.
        /// </summary>
        /// <param name="camera">The camera to disable auto-tracking for.</param>
        private void FallbackDisableCameraAutoTracking(Camera camera)
        {
            try
            {
                // Use SetStereoViewMatrix as a fallback
                camera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, camera.worldToCameraMatrix);
                camera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, camera.worldToCameraMatrix);
                Debug.Log("Fallback: SetStereoViewMatrix applied to disable auto camera tracking.");
            }
            catch (Exception e)
            {
                Debug.LogError($"SetStereoViewMatrix failed. Error: {e.Message}");
            }

            // Final fallback: Completely disable stereo rendering
            DisableStereoRendering(camera);
        }

        /// <summary>
        /// Completely disables stereo rendering as a last fallback.
        /// </summary>
        /// <param name="camera">The camera to disable stereo rendering for.</param>
        private void DisableStereoRendering(Camera camera)
        {
            try
            {
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                Debug.Log("Stereo rendering disabled as a fallback solution.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to disable stereo rendering. Error: {e.Message}");
            }
        }
    }
}
