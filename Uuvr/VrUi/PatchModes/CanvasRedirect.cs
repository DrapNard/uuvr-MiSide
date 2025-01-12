using System;
using UnityEngine;

namespace Uuvr.VrUi.PatchModes
{
    /// <summary>
    /// Redirects the rendering of a canvas to a specified capture camera for VR compatibility.
    /// </summary>
    public class CanvasRedirect : UuvrBehaviour
    {
        private Canvas _canvas;
        private Camera _uiCaptureCamera;
        private bool _isPatched;
        private RenderMode _originalRenderMode;
        private Camera _originalWorldCamera;
        private float _originalPlaneDistance;
        private int _originalLayer;

        public CanvasRedirect(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Creates a new CanvasRedirect instance and attaches it to the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas to redirect.</param>
        /// <param name="uiCaptureCamera">The capture camera for redirected rendering.</param>
        public static void Create(Canvas canvas, Camera uiCaptureCamera)
        {
            var instance = canvas.gameObject.AddComponent<CanvasRedirect>();
            instance._canvas = canvas;
            instance._uiCaptureCamera = uiCaptureCamera;
        }

        /// <summary>
        /// Unity's Start method. Applies initial settings.
        /// </summary>
        private void Start()
        {
            OnSettingChanged();
        }

        /// <summary>
        /// Called when a configuration setting changes. Determines whether to patch or undo the patch.
        /// </summary>
        protected override void OnSettingChanged()
        {
            var shouldPatch = ShouldPatchCanvas();

            if (shouldPatch && !_isPatched)
            {
                Patch();
            }
            else if (!shouldPatch && _isPatched)
            {
                UndoPatch();
            }
        }

        /// <summary>
        /// Determines whether the canvas should be patched based on the configuration and current state.
        /// </summary>
        /// <returns>True if the canvas should be patched, false otherwise.</returns>
        private bool ShouldPatchCanvas()
        {
            if (ModConfiguration.Instance.PreferredUiPatchMode.Value != ModConfiguration.UiPatchMode.CanvasRedirect)
            {
                return false;
            }

            var isScreenSpace = _canvas.renderMode == RenderMode.ScreenSpaceCamera;

            return ModConfiguration.Instance.ScreenSpaceCanvasTypesToPatch.Value switch
            {
                ModConfiguration.ScreenSpaceCanvasType.None => !isScreenSpace,
                ModConfiguration.ScreenSpaceCanvasType.NotToTexture => !isScreenSpace ||
                                                                       (isScreenSpace && _canvas.worldCamera?.targetTexture == null),
                ModConfiguration.ScreenSpaceCanvasType.All => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Applies the patch to redirect the canvas to the capture camera.
        /// </summary>
        private void Patch()
        {
            // Store original settings
            _originalRenderMode = _canvas.renderMode;
            _originalWorldCamera = _canvas.worldCamera;
            _originalPlaneDistance = _canvas.planeDistance;
            _originalLayer = _canvas.gameObject.layer;

            // Update layer recursively
            LayerHelper.SetLayerRecursive(transform, LayerHelper.GetVrUiLayer());

            // Update canvas render mode and camera
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = _uiCaptureCamera;

            if (_originalRenderMode == RenderMode.ScreenSpaceCamera)
            {
                AdjustCameraClippingPlanes();
            }
            else
            {
                _canvas.planeDistance = 1f;
            }

            _isPatched = true;

            Debug.Log($"CanvasRedirect: Patched canvas '{_canvas.name}' for VR rendering.");
        }

        /// <summary>
        /// Adjusts the clipping planes of the capture camera to ensure proper rendering.
        /// </summary>
        private void AdjustCameraClippingPlanes()
        {
            if (_originalPlaneDistance < _uiCaptureCamera.nearClipPlane)
            {
                _uiCaptureCamera.nearClipPlane = Mathf.Max(0.01f, _originalPlaneDistance - 0.1f);
            }
            if (_originalPlaneDistance > _uiCaptureCamera.farClipPlane)
            {
                _uiCaptureCamera.farClipPlane = _originalPlaneDistance + 0.1f;
            }
        }

        /// <summary>
        /// Restores the canvas to its original state, undoing the patch.
        /// </summary>
        private void UndoPatch()
        {
            // Restore original layer
            LayerHelper.SetLayerRecursive(transform, _originalLayer);

            // Restore canvas properties
            _canvas.renderMode = _originalRenderMode;
            _canvas.worldCamera = _originalWorldCamera;
            _canvas.planeDistance = _originalPlaneDistance;

            _isPatched = false;

            Debug.Log($"CanvasRedirect: Restored canvas '{_canvas.name}' to its original state.");
        }
    }
}
