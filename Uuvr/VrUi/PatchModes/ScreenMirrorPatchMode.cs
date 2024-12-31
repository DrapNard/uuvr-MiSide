using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Uuvr.VrUi.PatchModes
{
    /// <summary>
    /// Handles screen mirroring for VR UI, projecting the UI onto a texture while managing rendering behavior.
    /// </summary>
    public class ScreenMirrorPatchMode : UuvrBehaviour, VrUiPatchMode
    {
        private CommandBuffer? _commandBuffer;
        private Camera? _clearCamera;
        private RenderTexture? _targetTexture;
        private Coroutine? _endOfFrameCoroutine;

        /// <summary>
        /// Unity's OnEnable method. Sets up the screen mirroring process.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SetXrMirror(false);
            _endOfFrameCoroutine = StartCoroutine(EndOfFrameCoroutine());
        }

        /// <summary>
        /// Unity's OnDisable method. Resets screen mirroring behavior.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SetXrMirror(true);
            if (_endOfFrameCoroutine != null)
            {
                StopCoroutine(_endOfFrameCoroutine);
            }
            Reset();
        }

        /// <summary>
        /// Unity's Awake method. Initializes the clear camera and rendering settings.
        /// </summary>
        private void Awake()
        {
            Debug.Log("ScreenMirrorPatchMode: Initializing clear camera.");

            // Initialize the clear camera
            _clearCamera = gameObject.AddComponent<Camera>();
            _clearCamera.stereoTargetEye = StereoTargetEyeMask.None;
            _clearCamera.depth = -100;
            _clearCamera.cullingMask = 0;
            _clearCamera.clearFlags = CameraClearFlags.SolidColor;
            _clearCamera.backgroundColor = Color.clear;

            // Disable HDR and MSAA to prevent rendering issues
            _clearCamera.allowHDR = false;
            _clearCamera.allowMSAA = false;
        }

        /// <summary>
        /// Sets up the target texture for rendering the UI.
        /// </summary>
        /// <param name="targetTexture">The target render texture.</param>
        public void SetUpTargetTexture(RenderTexture targetTexture)
        {
            _targetTexture = targetTexture;

            if (!enabled) return;

            _commandBuffer = CreateCommandBuffer();
            _commandBuffer.name = "UUVR UI";
            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, targetTexture);
        }

        /// <summary>
        /// Resets the command buffer and rendering state.
        /// </summary>
        private void Reset()
        {
            if (_commandBuffer != null)
            {
                _commandBuffer.Dispose();
                _commandBuffer = null;
            }
        }

        /// <summary>
        /// Toggles XR mirroring for the screen.
        /// </summary>
        /// <param name="mirror">True to enable mirroring, false to disable.</param>
        private void SetXrMirror(bool mirror)
        {
            var xrSettingsType =
                Type.GetType("UnityEngine.XR.XRSettings, UnityEngine.XRModule") ??
                Type.GetType("UnityEngine.VR.VRSettings, UnityEngine");

            if (xrSettingsType == null) return;

            var showDeviceViewProperty = xrSettingsType.GetProperty("showDeviceView");
            showDeviceViewProperty?.SetValue(null, mirror);
        }

        /// <summary>
        /// Creates a new command buffer for rendering operations.
        /// </summary>
        /// <returns>A new CommandBuffer instance.</returns>
        private static CommandBuffer CreateCommandBuffer()
        {
            return new CommandBuffer();
        }

        /// <summary>
        /// Coroutine executed at the end of each frame to manage rendering behavior.
        /// </summary>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        private IEnumerator EndOfFrameCoroutine()
        {
            while (true)
            {
                if (_targetTexture != null &&
                    (_commandBuffer == null || Screen.width != _targetTexture.width || Screen.height != _targetTexture.height))
                {
                    SetUpTargetTexture(_targetTexture);
                }

                yield return new WaitForEndOfFrame();

                if (_commandBuffer != null && _targetTexture != null)
                {
                    Graphics.ExecuteCommandBuffer(_commandBuffer);
                }
            }
        }
    }
}
