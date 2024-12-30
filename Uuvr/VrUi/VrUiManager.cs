using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uuvr.VrUi.PatchModes;

namespace Uuvr.VrUi
{
    /// <summary>
    /// Manages the VR UI, including rendering and interaction modes.
    /// </summary>
    public class VrUiManager : UuvrBehaviour
    {
        private RenderTexture? _uiTexture;
        private GameObject? _vrUiQuad;
        private GameObject? _uiContainer;
        private CanvasRedirectPatchMode? _canvasRedirectPatchMode;
        private ScreenMirrorPatchMode? _screenMirrorPatchMode;
        private FollowTarget? _worldRenderModeFollowTarget;
        private UiOverlayRenderMode? _uiOverlayRenderMode;

        /// <summary>
        /// Unity's Start method. Sets up the UI and applies initial settings.
        /// </summary>
        private void Start()
        {
            SetUpUi();
            OnSettingChanged();
            Create<VrUiCursor>(transform);
        }

        /// <summary>
        /// Called when a configuration setting changes. Updates UI rendering and behavior.
        /// </summary>
        protected override void OnSettingChanged()
        {
            base.OnSettingChanged();
            if (_vrUiQuad == null || _uiOverlayRenderMode == null || _worldRenderModeFollowTarget == null || _canvasRedirectPatchMode == null || _screenMirrorPatchMode == null) return;

            var config = ModConfiguration.Instance;

            // Set layer for VR UI
            var uiLayer = LayerHelper.GetVrUiLayer();
            _vrUiQuad.layer = uiLayer;

            // Update render mode
            _uiOverlayRenderMode.gameObject.SetActive(config.PreferredUiRenderMode.Value == ModConfiguration.UiRenderMode.OverlayCamera);
            _worldRenderModeFollowTarget.enabled = config.PreferredUiRenderMode.Value == ModConfiguration.UiRenderMode.InWorld;

            if (config.PreferredUiRenderMode.Value != ModConfiguration.UiRenderMode.InWorld)
            {
                _worldRenderModeFollowTarget.transform.localPosition = Vector3.zero;
                _worldRenderModeFollowTarget.transform.localRotation = Quaternion.identity;
            }

            // Update patch modes
            _screenMirrorPatchMode.enabled = config.PreferredUiPatchMode.Value == ModConfiguration.UiPatchMode.Mirror;
            _canvasRedirectPatchMode.enabled = config.PreferredUiPatchMode.Value == ModConfiguration.UiPatchMode.CanvasRedirect;

            // Adjust quad scale for mirror mode
            var yScale = Mathf.Abs(_vrUiQuad.transform.localScale.y);
            if (config.PreferredUiPatchMode.Value == ModConfiguration.UiPatchMode.Mirror)
            {
                yScale *= -1;
            }
            _vrUiQuad.transform.localScale = new Vector3(_vrUiQuad.transform.localScale.x, yScale, _vrUiQuad.transform.localScale.z);

            UpdateFollowTarget();
        }

        /// <summary>
        /// Sets up the VR UI elements and render textures.
        /// </summary>
        private void SetUpUi()
        {
            // Create UI render texture
            _uiTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            var uiTextureAspectRatio = (float)_uiTexture.height / _uiTexture.width;

            // Create UI container
            _uiContainer = new GameObject("VrUiContainer")
            {
                transform = { parent = transform }
            };

            // Create quad for UI
            _vrUiQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(_vrUiQuad.GetComponent<Collider>()); // Remove unnecessary collider
            _vrUiQuad.name = "VrUiQuad";
            _vrUiQuad.transform.parent = _uiContainer.transform;
            _vrUiQuad.transform.localPosition = Vector3.forward * 2f;

            var quadWidth = 1.8f;
            var quadHeight = quadWidth * uiTextureAspectRatio;
            _vrUiQuad.transform.localScale = new Vector3(quadWidth, quadHeight, 1f);

            var renderer = _vrUiQuad.GetComponent<Renderer>();
            renderer.material = Canvas.GetDefaultCanvasMaterial();
            renderer.material.mainTexture = _uiTexture;
            renderer.material.renderQueue = 5000;

            // Add and configure patch modes
            _canvasRedirectPatchMode = gameObject.AddComponent<CanvasRedirectPatchMode>();
            _canvasRedirectPatchMode.SetUpTargetTexture(_uiTexture);

            _screenMirrorPatchMode = gameObject.AddComponent<ScreenMirrorPatchMode>();
            _screenMirrorPatchMode.SetUpTargetTexture(_uiTexture);

            // Set up render modes
            _uiOverlayRenderMode = Create<UiOverlayRenderMode>(transform);
            _worldRenderModeFollowTarget = _uiContainer.AddComponent<FollowTarget>();
        }

        /// <summary>
        /// Unity's Update method. Ensures UI elements are up-to-date.
        /// </summary>
        private void Update()
        {
            if (_uiTexture == null)
            {
                SetUpUi();
            }

            if (
                VrCamera.VrCamera.HighestDepthVrCamera != null &&
                VrCamera.VrCamera.HighestDepthVrCamera.ParentCamera != null &&
                _uiContainer != null &&
                _uiContainer.transform.parent != VrCamera.VrCamera.HighestDepthVrCamera.ParentCamera.transform &&
                _worldRenderModeFollowTarget != null)
            {
                UpdateFollowTarget();
            }
        }

        /// <summary>
        /// Updates the follow target for UI elements based on the highest depth VR camera.
        /// </summary>
        private void UpdateFollowTarget()
        {
            if (VrCamera.VrCamera.HighestDepthVrCamera == null || VrCamera.VrCamera.HighestDepthVrCamera.ParentCamera == null) return;

            _worldRenderModeFollowTarget.Target = ModConfiguration.Instance.PreferredUiRenderMode.Value == ModConfiguration.UiRenderMode.InWorld
                ? VrCamera.VrCamera.HighestDepthVrCamera.ParentCamera.transform
                : null;
        }
    }
}
