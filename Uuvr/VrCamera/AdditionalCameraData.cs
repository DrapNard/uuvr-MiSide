using Il2CppSystem;
using System;
using Il2CppSystem.Reflection;
using UnityEngine;

namespace Uuvr.VrCamera
{
    /// <summary>
    /// Helper behavior for handling URP's Additional Camera Data without requiring a hard dependency.
    /// </summary>
    public class AdditionalCameraData : MonoBehaviour
    {
        public AdditionalCameraData(System.IntPtr pointer) : base(pointer) { }

        private const int RenderTypeBase = 0;
        private const int RenderTypeOverlay = 1;

        private static Il2CppSystem.Type? _additionalCameraDataType;
        private static PropertyInfo? _renderTypeProperty;
        private static PropertyInfo? _cameraStackProperty;
        private static PropertyInfo? _allowXrRenderingProperty;

        private Il2CppSystem.Object? _additionalCameraData;

        /// <summary>
        /// Initializes the AdditionalCameraData for a camera.
        /// </summary>
        /// <param name="camera">The camera to associate with AdditionalCameraData.</param>
        /// <returns>The AdditionalCameraData instance, or null if initialization failed.</returns>
        public static AdditionalCameraData? Create(Camera camera)
        {
            // Lazy initialization of the AdditionalCameraData type and associated properties
            if (_additionalCameraDataType == null)
            {
                _additionalCameraDataType = Il2CppSystem.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
                if (_additionalCameraDataType != null)
                {
                    _renderTypeProperty = _additionalCameraDataType.GetProperty("renderType");
                    _cameraStackProperty = _additionalCameraDataType.GetProperty("cameraStack");
                    _allowXrRenderingProperty = _additionalCameraDataType.GetProperty("allowXRRendering");
                }
            }

            if (_additionalCameraDataType == null) return null;

            // Attach or retrieve the AdditionalCameraData component
            return camera.gameObject.GetComponent<AdditionalCameraData>() ?? camera.gameObject.AddComponent<AdditionalCameraData>();
        }

        /// <summary>
        /// Unity's Awake method. Initializes the Additional Camera Data instance.
        /// </summary>
        private void Awake()
        {
            if (_additionalCameraDataType == null) return;

            // Attach or create the additional camera data component dynamically
            _additionalCameraData = gameObject.GetComponent(_additionalCameraDataType) ?? gameObject.AddComponent(_additionalCameraDataType);
        }

        /// <summary>
        /// Sets the render type to "Base".
        /// </summary>
        public void SetRenderTypeBase()
        {
            _renderTypeProperty?.SetValue(_additionalCameraData, RenderTypeBase);
        }

        /// <summary>
        /// Sets the render type to "Overlay".
        /// </summary>
        public void SetRenderTypeOverlay()
        {
            _renderTypeProperty?.SetValue(_additionalCameraData, RenderTypeOverlay);
        }

        /// <summary>
        /// Checks if the render type is set to "Overlay".
        /// </summary>
        /// <returns>True if the render type is "Overlay", otherwise false.</returns>

        /// <summary>
        /// Gets the camera stack associated with the Additional Camera Data.
        /// </summary>
        /// <returns>The list of cameras in the stack.</returns>
        public Il2CppSystem.Collections.Generic.List<Camera>? GetCameraStack()
        {
            return _cameraStackProperty?.GetValue(_additionalCameraData) as Il2CppSystem.Collections.Generic.List<Camera>;
        }

        /// <summary>
        /// Enables or disables XR rendering for the camera.
        /// </summary>
        /// <param name="allowXrRendering">True to allow XR rendering, false to disable.</param>
        public void SetAllowXrRendering(bool allowXrRendering)
        {
            _allowXrRenderingProperty?.SetValue(_additionalCameraData, allowXrRendering);
        }
    }
}
