using System;
using UnityEngine;
using System.Diagnostics;

namespace Uuvr.VrTogglers
{
    public class VrTogglerManager
    {
        private VrToggler _toggler;

        public VrTogglerManager()
        {
            SetUpToggler();
        }

        private void SetUpToggler()
        {
            // Ensure any previously set toggler is disabled
            _toggler?.SetVrEnabled(false);

            // Configure VR toggler based on the preferred VR API
            switch (ModConfiguration.Instance.PreferredVrApi.Value)
            {
                case ModConfiguration.VrApi.OpenVr:
                    _toggler = new XrPluginOpenVrToggler();
                    UnityEngine.Debug.Log("UUVR: OpenVR");
                    break;

                case ModConfiguration.VrApi.OpenXr:
                    _toggler = new XrPluginOpenXrToggler();
                    UnityEngine.Debug.Log("UUVR: OpenXR");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ModConfiguration.Instance.PreferredVrApi),
                        ModConfiguration.Instance.PreferredVrApi.Value, "Invalid VR API configuration.");
            }
        }

        public void ToggleVr()
        {
            if (_toggler == null)
            {
                throw new InvalidOperationException("VR toggler is not initialized.");
            }

            // Toggle VR state
            _toggler.SetVrEnabled(!_toggler.IsVrEnabled);
        }
    }
}
