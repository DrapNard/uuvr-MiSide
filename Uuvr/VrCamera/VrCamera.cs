using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uuvr.VrCamera
{
    public class VrCamera : UuvrBehaviour
    {
        public static readonly HashSet<Camera> VrCameras = new();
        public static readonly HashSet<Camera> IgnoredCameras = new();
        public static VrCamera? HighestDepthVrCamera { get; private set; }

        private Quaternion _rotationBeforeRender;

        public Camera? ParentCamera { get; private set; }

        public Camera? CameraInUse =>
            ModConfiguration.Instance.CameraTracking.Value == ModConfiguration.CameraTrackingMode.Child ?
            _childCamera :
            ParentCamera;

        private UuvrPoseDriver? _parentCameraPoseDriver;
        private Camera? _childCamera;
        private UuvrPoseDriver? _childCameraPoseDriver;
        private LineRenderer? _forwardLine;

        public VrCamera(IntPtr pointer) : base(pointer)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            ParentCamera = GetComponent<Camera>();
            if (ParentCamera != null)
            {
                VrCameras.Add(ParentCamera);
            }
        }

        private void OnDestroy()
        {
            VrCameras.Remove(ParentCamera);
        }

        private void Start()
        {
            // Setting up the VR camera offset and child components
            var rotationNullifier = Create<VrCameraOffset>(transform);
            _parentCameraPoseDriver = ParentCamera?.gameObject.AddComponent<UuvrPoseDriver>();

            _childCameraPoseDriver = Create<UuvrPoseDriver>(rotationNullifier.transform);
            _childCameraPoseDriver.name = "VrChildCamera";
            _childCamera = _childCameraPoseDriver.gameObject.AddComponent<Camera>();
            IgnoredCameras.Add(_childCamera);

            if (ParentCamera != null)
            {
                _childCamera.CopyFrom(ParentCamera);
            }

            // Optional: Set up forward line rendering
            SetUpForwardLine();
        }

        protected override void OnBeforeRender()
        {
            UpdateRelativeMatrix();
        }

        private void LateUpdate()
        {
            UpdateRelativeMatrix();
        }

        private void Update()
        {
            if (ModConfiguration.Instance.OverrideDepth.Value && ParentCamera != null)
            {
                ParentCamera.depth = ModConfiguration.Instance.VrCameraDepth.Value;
            }

            UpdateCameraTrackingMode();
            UpdateHighestDepthCamera();
        }

        private void UpdateCameraTrackingMode()
        {
            var cameraTrackingMode = ModConfiguration.Instance.CameraTracking.Value;

            if (_parentCameraPoseDriver != null)
            {
                _parentCameraPoseDriver.enabled = cameraTrackingMode == ModConfiguration.CameraTrackingMode.Absolute;
            }

            if (_childCameraPoseDriver != null)
            {
                _childCameraPoseDriver.gameObject.SetActive(cameraTrackingMode != ModConfiguration.CameraTrackingMode.Absolute);
            }

            if (cameraTrackingMode == ModConfiguration.CameraTrackingMode.Child && ParentCamera != null)
            {
                if (_childCamera != null)
                {
                    _childCamera.cullingMask = ParentCamera.cullingMask;
                    _childCamera.clearFlags = ParentCamera.clearFlags;
                    _childCamera.depth = ParentCamera.depth;
                }
            }
            else
            {
                if (_childCamera != null)
                {
                    _childCamera.cullingMask = 0;
                    _childCamera.clearFlags = CameraClearFlags.Nothing;
                    _childCamera.depth = -100;
                }
            }
        }

        private void UpdateHighestDepthCamera()
        {
            if (ParentCamera == null) return;

            if (HighestDepthVrCamera == null || ParentCamera.depth > HighestDepthVrCamera.CameraInUse?.depth)
            {
                HighestDepthVrCamera = this;
            }
        }

        private void UpdateRelativeMatrix()
        {
            if (ModConfiguration.Instance.CameraTracking.Value != ModConfiguration.CameraTrackingMode.RelativeMatrix || ParentCamera == null) return;

            var eye = ParentCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left
                ? Camera.StereoscopicEye.Left
                : Camera.StereoscopicEye.Right;

            ParentCamera.worldToCameraMatrix = _childCamera?.GetStereoViewMatrix(eye) ?? ParentCamera.worldToCameraMatrix;

            if (ModConfiguration.Instance.RelativeCameraSetStereoView.Value)
            {
                ParentCamera.SetStereoViewMatrix(eye, ParentCamera.worldToCameraMatrix);
            }
        }

        private void SetUpForwardLine()
        {
            _forwardLine = new GameObject("VrCameraForwardLine").AddComponent<LineRenderer>();
            _forwardLine.transform.SetParent(transform, false);
            _forwardLine.useWorldSpace = false;
            _forwardLine.SetPositions(new[] { Vector3.forward * 2f, Vector3.forward * 10f });
            _forwardLine.startWidth = 0.1f;
            _forwardLine.endWidth = 0f;

            Debug.Log("Forward line set up for VR camera.");
        }
    }
}
