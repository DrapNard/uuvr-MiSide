using Il2CppInterop.Runtime.Injection;
using System;
using System.Reflection;
using UnityEngine;
using Uuvr.VrCamera;
using Uuvr.VrTogglers;

namespace Uuvr
{
    public class UuvrCore : MonoBehaviour
    {
        public UuvrCore(IntPtr pointer) : base(pointer) { } // Required for IL2CPP compatibility

        private readonly KeyboardKey _toggleVrKey = new(KeyboardKey.KeyCode.F3);
        private float _originalFixedDeltaTime;
        private VrTogglerManager? _vrTogglerManager;
        private PropertyInfo? _refreshRateProperty;

        // Static initialization of UuvrCore
        public static void Initialize()
        {
            // Ensure the type is registered with IL2CPP
            ClassInjector.RegisterTypeInIl2Cpp<UuvrCore>();
            ClassInjector.RegisterTypeInIl2Cpp<VrCameraManager>();

            // Create the core GameObject and attach the component
            var coreGameObject = new GameObject("UUVR");
            coreGameObject.AddComponent<UuvrCore>();
            DontDestroyOnLoad(coreGameObject);

            Debug.Log("UuvrCore initialized.");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject); // Make sure the object persists across scenes
            gameObject.AddComponent<VrCameraManager>(); // Attach dependent manager
        }

        private void Start()
        {
            try
            {
                // Resolve XR refresh rate property
                var xrDeviceType = Type.GetType("UnityEngine.XR.XRDevice, UnityEngine.XRModule") ??
                                   Type.GetType("UnityEngine.XR.XRDevice, UnityEngine.VRModule");
                _refreshRateProperty = xrDeviceType?.GetProperty("refreshRate");

                // Initialize VR toggler manager
                _vrTogglerManager = new VrTogglerManager();
                Debug.Log("UuvrCore started successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during UuvrCore.Start(): {ex.Message}");
            }
        }

        private void Update()
        {
            try
            {
                // Toggle VR when the key is pressed
                if (_toggleVrKey.UpdateIsDown())
                {
                    _vrTogglerManager?.ToggleVr();
                }

                // Update the physics rate based on the headset refresh rate
                UpdatePhysicsRate();
            }
            catch (Exception ex)
            {
                Debug.Log($"Error during UuvrCore.Update(): {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            try
            {
                Debug.Log("UUVR instance destroyed. Recreating...");
                Initialize();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during UuvrCore.OnDestroy(): {ex.Message}");
            }
        }

        private void UpdatePhysicsRate()
        {
            try
            {
                if (_originalFixedDeltaTime == 0)
                {
                    _originalFixedDeltaTime = Time.fixedDeltaTime;
                }

                if (_refreshRateProperty == null) return;

                var headsetRefreshRate = (float)_refreshRateProperty.GetValue(null, null);
                if (headsetRefreshRate <= 0) return;

                if (ModConfiguration.Instance.PhysicsMatchHeadsetRefreshRate.Value)
                {
                    Time.fixedDeltaTime = 1f / headsetRefreshRate;
                }
                else
                {
                    Time.fixedDeltaTime = _originalFixedDeltaTime;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during UpdatePhysicsRate(): {ex.Message}");
            }
        }
    }
}
