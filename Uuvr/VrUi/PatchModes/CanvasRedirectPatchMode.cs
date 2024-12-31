using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Uuvr.VrUi.PatchModes
{
    /// <summary>
    /// Redirects UI canvases to render in the VR environment by patching the rendering behavior.
    /// </summary>
    public class CanvasRedirectPatchMode : UuvrBehaviour, VrUiPatchMode
    {
        private readonly List<string> _ignoredCanvases = new()
        {
            "unityexplorer",  // Unity Explorer canvas
            "universelib"     // UniverseLib-related canvases
        };

        private Camera? _uiCaptureCamera;

        /// <summary>
        /// Called when a configuration setting changes. Updates the culling mask for the capture camera.
        /// </summary>
        protected override void OnSettingChanged()
        {
            base.OnSettingChanged();
            if (_uiCaptureCamera != null)
            {
                _uiCaptureCamera.cullingMask = 1 << LayerHelper.GetVrUiLayer();
                Debug.Log("CanvasRedirectPatchMode: Updated culling mask for the UI capture camera.");
            }
        }

        /// <summary>
        /// Unity's Start method. Applies the initial settings.
        /// </summary>
        private void Start()
        {
            OnSettingChanged();
        }

        /// <summary>
        /// Unity's OnEnable method. Ensures the capture camera is active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_uiCaptureCamera != null)
            {
                _uiCaptureCamera.enabled = true;
            }
        }

        /// <summary>
        /// Unity's OnDisable method. Disables the capture camera.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_uiCaptureCamera != null)
            {
                _uiCaptureCamera.enabled = false;
            }
        }

        /// <summary>
        /// Unity's Awake method. Initializes the UI capture camera.
        /// </summary>
        private void Awake()
        {
            _uiCaptureCamera = new GameObject("VrUiCaptureCamera").AddComponent<Camera>();
            VrCamera.VrCamera.IgnoredCameras.Add(_uiCaptureCamera);
            _uiCaptureCamera.transform.parent = transform;
            _uiCaptureCamera.clearFlags = CameraClearFlags.SolidColor;
            _uiCaptureCamera.backgroundColor = Color.clear;
            _uiCaptureCamera.depth = 100;

            Debug.Log("CanvasRedirectPatchMode: UI capture camera initialized.");
        }

        /// <summary>
        /// Sets the target texture for the capture camera.
        /// </summary>
        /// <param name="targetTexture">The render texture for UI output.</param>
        public void SetUpTargetTexture(RenderTexture targetTexture)
        {
            if (_uiCaptureCamera != null)
            {
                _uiCaptureCamera.targetTexture = targetTexture;
                Debug.Log("CanvasRedirectPatchMode: Target texture set for the UI capture camera.");
            }
        }

        /// <summary>
        /// Unity's Update method. Redirects canvases to the VR environment each frame.
        /// </summary>
        private void Update()
        {
            var canvases = GraphicRegistry.instance.m_Graphics.Keys;

            foreach (var canvas in canvases)
            {
                PatchCanvas(canvas);
            }

            // Move the capture camera to prevent it from capturing the projected UI.
            if (_uiCaptureCamera != null)
            {
                _uiCaptureCamera.transform.localPosition = Vector3.right * 10;
            }
        }

        /// <summary>
        /// Patches the given canvas to render in the VR environment.
        /// </summary>
        /// <param name="canvas">The canvas to patch.</param>
        private void PatchCanvas(Canvas canvas)
        {
            if (canvas == null || _uiCaptureCamera == null) return;

            // Skip world-space canvases; they already work in VR.
            if (canvas.renderMode == RenderMode.WorldSpace) return;

            // Process only root canvases.
            if (!canvas.isRootCanvas)
            {
                PatchCanvas(canvas.rootCanvas);
                return;
            }

            // Skip ignored canvases.
            if (_ignoredCanvases.Any(ignored => canvas.name.ToLower().Contains(ignored.ToLower())))
            {
                return;
            }

            // Check if the canvas is already patched.
            if (canvas.GetComponent<CanvasRedirect>()) return;

            // Redirect the canvas to the capture camera.
            CanvasRedirect.Create(canvas, _uiCaptureCamera);
            Debug.Log($"CanvasRedirectPatchMode: Patched canvas '{canvas.name}' for VR rendering.");
        }
    }
}
