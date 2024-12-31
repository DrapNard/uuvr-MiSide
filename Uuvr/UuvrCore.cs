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
        private readonly KeyboardKey _toggleVrKey = new(KeyboardKey.KeyCode.F3);
        private float _originalFixedDeltaTime;
        private VrTogglerManager? _vrTogglerManager;
        private PropertyInfo? _refreshRateProperty;

        public static void Create()
        {
            var coreGameObject = new GameObject("UUVR");
            ClassInjector.RegisterTypeInIl2Cpp<UuvrCore>(); // Ensure type is registered
            coreGameObject.AddComponent<UuvrCore>();
            UnityEngine.Object.DontDestroyOnLoad(coreGameObject);
            //Debug.Log("UuvrCore instance created.");
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<VrCameraManager>();
        }

        public void Start()
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

        public void Update()
        {
            try
            {
                // Toggle VR when the key is pressed
                if (_toggleVrKey.UpdateIsDown())
                {
                    _vrTogglerManager?.ToggleVr();
                    //Debug.Log("Toggled VR mode.");
                }

                // Update the physics rate based on the headset refresh rate
                UpdatePhysicsRate();
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCore.Update(): {ex.Message}");
            }
        }

        public void OnDestroy()
        {
            try
            {
                //Debug.Log("UUVR instance destroyed. Recreating...");
                Create();
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCore.OnDestroy(): {ex.Message}");
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
                //Debug.LogError($"Error during UpdatePhysicsRate(): {ex.Message}");
            }
        }
    }
}
