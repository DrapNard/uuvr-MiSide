using System;
using UnityEngine;

namespace Uuvr.VrCamera
{
    /// <summary>
    /// Handles VR camera offsets and alignment settings.
    /// </summary>
    public class VrCameraOffset : UuvrBehaviour
    {
        public VrCameraOffset(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Called before rendering the frame. Ensures the transform is updated.
        /// </summary>
        protected override void OnBeforeRender()
        {
            base.OnBeforeRender();
            UpdateTransform();
        }

        /// <summary>
        /// Called when a configuration setting changes. Updates the local position based on offsets from configuration.
        /// </summary>
        protected override void OnSettingChanged()
        {
            base.OnSettingChanged();
            var config = ModConfiguration.Instance;

            // Update local position offsets based on configuration values
            transform.localPosition = new Vector3(
                config.CameraPositionOffsetX.Value,
                config.CameraPositionOffsetY.Value,
                config.CameraPositionOffsetZ.Value
            );

            Debug.Log($"VrCameraOffset: Updated local position to ({transform.localPosition.x}, {transform.localPosition.y}, {transform.localPosition.z}).");
        }

        /// <summary>
        /// Unity's Update method. Ensures the transform is updated every frame.
        /// </summary>
        private void Update()
        {
            UpdateTransform();
        }

        /// <summary>
        /// Unity's LateUpdate method. Ensures the transform is updated after other updates.
        /// </summary>
        private void LateUpdate()
        {
            UpdateTransform();
        }

        /// <summary>
        /// Updates the transform's rotation or alignment based on configuration settings.
        /// </summary>
        private void UpdateTransform()
        {
            if (ModConfiguration.Instance.AlignCameraToHorizon.Value)
            {
                // Align the forward direction of the camera to the horizon (no pitch/roll, only yaw)
                var forward = Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up);
                transform.LookAt(transform.position + forward, Vector3.up);
                Debug.Log("VrCameraOffset: Camera aligned to horizon.");
            }
            else
            {
                // Reset rotation to default (identity)
                transform.localRotation = Quaternion.identity;
                Debug.Log("VrCameraOffset: Camera rotation reset to identity.");
            }
        }
    }
}
