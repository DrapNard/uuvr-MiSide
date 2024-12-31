using System;
using System.Reflection;
using UnityEngine;
using Uuvr.VrCamera;
using Uuvr.VrTogglers;
using Uuvr.VrUi;

namespace Uuvr
{
    public class UuvrCore
    {
        private readonly KeyboardKey _toggleVrKey = new(KeyboardKey.KeyCode.F3);
        private float _originalFixedDeltaTime;
        private VrTogglerManager? _vrTogglerManager;
        private PropertyInfo? _refreshRateProperty;

        public static void Create()
        {
            var coreGameObject = new GameObject(nameof(UuvrCoreWrapper));
            UnityEngine.Object.DontDestroyOnLoad(coreGameObject); // Correct usage of DontDestroyOnLoad
            //new GameObject("UUVR").AddComponent<UuvrCore>();
            coreGameObject.AddComponent<UuvrCoreWrapper>();
            //Debug.Log("UuvrCore instance created.");
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
                //Debug.Log("UuvrCore started successfully.");
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCore.Start(): {ex.Message}");
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

    public class UuvrCoreWrapper : MonoBehaviour
    {
        private UuvrCore _core;

        private void Awake()
        {
            try
            {
                _core = new UuvrCore();
                //Debug.Log("UuvrCoreWrapper initialized.");
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCoreWrapper.Awake(): {ex.Message}");
            }
        }

        private void Start()
        {
            try
            {
                _core.Start();
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCoreWrapper.Start(): {ex.Message}");
            }
        }

        private void Update()
        {
            try
            {
                _core.Update();
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCoreWrapper.Update(): {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            try
            {
                _core.OnDestroy();
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error during UuvrCoreWrapper.OnDestroy(): {ex.Message}");
            }
        }
    }
}
