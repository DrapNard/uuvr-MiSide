using System;
using System.Reflection;
using UnityEngine;
using Uuvr.VrTogglers;

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
            new GameObject(nameof(UuvrCore)).AddComponent<UuvrCoreWrapper>();
        }

        public void Start()
        {
            // Resolve XR refresh rate property
            var xrDeviceType = Type.GetType("UnityEngine.XR.XRDevice, UnityEngine.XRModule") ??
                               Type.GetType("UnityEngine.XR.XRDevice, UnityEngine.VRModule");
            _refreshRateProperty = xrDeviceType?.GetProperty("refreshRate");

            // Initialize VR toggler manager
            _vrTogglerManager = new VrTogglerManager();
        }

        public void Update()
        {
            // Toggle VR when the key is pressed
            if (_toggleVrKey.UpdateIsDown())
            {
                _vrTogglerManager?.ToggleVr();
            }

            // Update the physics rate based on the headset refresh rate
            UpdatePhysicsRate();
        }

        public void OnDestroy()
        {
            Debug.Log("UUVR has been destroyed. Recreating...");
            Create();
        }

        private void UpdatePhysicsRate()
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
    }

    public class UuvrCoreWrapper : MonoBehaviour
    {
        private UuvrCore _core;

        private void Awake()
        {
            _core = new UuvrCore();
        }

        private void Start()
        {
            _core.Start();
        }

        private void Update()
        {
            _core.Update();
        }

        private void OnDestroy()
        {
            _core.OnDestroy();
        }
    }
}
