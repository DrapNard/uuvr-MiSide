using System;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

namespace Uuvr.VrTogglers
{
    /// <summary>
    /// This class configures and toggles OpenXR support in Unity using XR Plugins.
    /// </summary>
    public class XrPluginOpenXrToggler : XrPluginToggler
    {
        protected override XRLoader CreateLoader()
        {
            try
            {
                // Create and initialize OpenXR Loader
                var xrLoader = ScriptableObject.CreateInstance<OpenXRLoader>();

                var openXRSettings = OpenXRSettings.Instance;
                if (openXRSettings == null)
                {
                    throw new Exception("Failed to retrieve OpenXRSettings instance.");
                }

                // Configure OpenXRSettings (adjust based on your plugin version)
                openXRSettings.renderMode = OpenXRSettings.RenderMode.MultiPass;

                Debug.Log("Successfully created and configured OpenXRLoader.");

                return xrLoader;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in CreateLoader: {ex.Message}");
                throw;
            }
        }
    }
}