using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Uuvr.VrTogglers
{
    public abstract class XrPluginToggler : VrToggler
    {
        private XRManagerSettings _managerSettings;
        private object _generalSettings;

        protected override bool SetUp()
        {
            try
            {
                // Attempt to retrieve the XRGeneralSettings instance directly
                _generalSettings = XRGeneralSettings.Instance as XRGeneralSettings;

                if (_generalSettings == null)
                {
                    // Reflection fallback if the direct access fails
                    var generalSettingsType = typeof(XRGeneralSettings);
                    var instanceProperty = generalSettingsType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    _generalSettings = instanceProperty?.GetValue(null) as XRGeneralSettings;

                    if (_generalSettings == null)
                    {
                        Debug.LogError("XRGeneralSettings.Instance is null or could not be accessed.");
                        return false;
                    }
                }

#pragma warning disable CS0618
                _managerSettings.loaders.Add(CreateLoader());
#pragma warning restore CS0618

                _managerSettings.InitializeLoaderSync();

                if (_managerSettings.activeLoader == null)
                {
                    Debug.LogError("Failed to initialize XR Loader.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during XR setup: {ex.Message}");
                return false;
            }
        }

        protected override bool EnableVr()
        {
            if (_managerSettings == null || _managerSettings.activeLoader == null)
            {
                Debug.LogError("Cannot enable VR. Manager or Loader is null.");
                return false;
            }

            _managerSettings.StartSubsystems();
            return _managerSettings.activeLoader.Initialize() && _managerSettings.activeLoader.Start();
        }

        protected override bool DisableVr()
        {
            if (_managerSettings == null || _managerSettings.activeLoader == null)
            {
                Debug.LogError("Cannot disable VR. Manager or Loader is null.");
                return false;
            }

            return _managerSettings.activeLoader.Stop() && _managerSettings.activeLoader.Deinitialize();
        }

        protected abstract XRLoader CreateLoader();
    }
}
