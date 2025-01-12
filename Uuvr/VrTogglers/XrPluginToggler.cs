using UnityEngine;
using UnityEngine.XR.Management;

namespace Uuvr.VrTogglers
{
    public abstract class XrPluginToggler : VrToggler
    {
        protected XRGeneralSettings _generalSettings;
        protected XRManagerSettings _managerSettings;

        protected override bool SetUp()
        {
            // Initialize XR General Settings and Manager Settings
            _generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            _managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
            _generalSettings.Manager = _managerSettings;

#pragma warning disable CS0618
            // Add XR loader to the manager settings
            _managerSettings.loaders.Add(CreateLoader());
#pragma warning restore CS0618

            // Initialize the XR loader
            _managerSettings.InitializeLoaderSync();

            if (_managerSettings.activeLoader == null)
            {
                Debug.LogError("Failed to initialize XR Loader. Ensure VR headset is connected.");
                return false;
            }

            return true;
        }

        protected override bool EnableVr()
        {
            // Start the XR subsystems and initialize/start the loader
            _managerSettings.StartSubsystems();
            var initSuccess = _managerSettings.activeLoader.Initialize();
            var startSuccess = _managerSettings.activeLoader.Start();
            return initSuccess && startSuccess;
        }

        protected override bool DisableVr()
        {
            // Stop and deinitialize XR loader
            var stopSuccess = _managerSettings.activeLoader.Stop();
            var deinitSuccess = _managerSettings.activeLoader.Deinitialize();
            return stopSuccess && deinitSuccess;
        }

        protected abstract XRLoader CreateLoader();
    }
}