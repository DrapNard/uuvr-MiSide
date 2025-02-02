using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.Management;
using System;

namespace Uuvr.VrTogglers
{
    public class XrPluginOpenXrToggler : XrPluginToggler
    {
        protected override XRLoader CreateLoader()
        {
            try
            {
                var loaderType = Type.GetType("UnityEngine.XR.OpenXR.OpenXRLoader, Unity.XR.OpenXR");
                if (loaderType == null)
                {
                    Debug.LogError("Failed to get OpenXRLoader type.");
                    throw new Exception("OpenXRLoader creation failed.");
                }

                var xrLoader = Activator.CreateInstance(loaderType) as XRLoader;
                if (xrLoader == null)
                {
                    Debug.LogError("Failed to create OpenXRLoader instance.");
                    throw new Exception("OpenXRLoader creation failed.");
                }

                Debug.Log("Successfully created OpenXRLoader.");
                return xrLoader;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating XRLoader: {ex.Message}");
                throw;
            }
        }
    }
}
