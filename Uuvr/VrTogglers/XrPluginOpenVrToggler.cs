using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.Management;

namespace Uuvr.VrTogglers
{
    public class XrPluginOpenVrToggler : XrPluginToggler
    {
        protected override XRLoader CreateLoader()
        {
            try
            {
                // Create and initialize OpenXR Loader
                var xrLoader = ScriptableObject.CreateInstance<OpenXRLoader>();
                xrLoader.Initialize();

                var openXRSettings = OpenXRSettings.Instance;
                if (openXRSettings == null)
                {
                    throw new Exception("Failed to retrieve OpenXRSettings instance.");
                }

                // Configure OpenXRSettings if necessary
                Debug.Log("OpenXRSettings successfully retrieved.");

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
