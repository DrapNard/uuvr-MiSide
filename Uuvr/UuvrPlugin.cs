using System.IO;
using System.Reflection;
using BepInEx;
using Uuvr.OpenVR;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using Uuvr.VrCamera;
using Uuvr.VrUi.PatchModes;
using Uuvr.VrUi;
using HarmonyLib;
using UnityEngine;
using System;

namespace Uuvr
{
    [BepInPlugin(
        "raicuparta.uuvr-modern",
        "UUVR",
        "0.3.1")]
    public class UuvrPlugin : BasePlugin
    {
        private static UuvrPlugin _instance;
        public static string ModFolderPath { get; private set; }

        public override void Load()
        { 
            _instance = this;
            ModFolderPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(UuvrPlugin)).Location);

            new ModConfiguration(Config);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Assembly openVrAssembly = Assembly.Load("Uuvr.OpenVR");
            ClassInjector.RegisterTypeInIl2Cpp<OpenVrManager>();
            ClassInjector.RegisterTypeInIl2Cpp<VrCamera.VrCamera>();
            ClassInjector.RegisterTypeInIl2Cpp<VrCamera.VrCamera>();
            ClassInjector.RegisterTypeInIl2Cpp<VrCameraOffset>();
            ClassInjector.RegisterTypeInIl2Cpp<CanvasRedirect>();
            ClassInjector.RegisterTypeInIl2Cpp<UiOverlayRenderMode>();
            ClassInjector.RegisterTypeInIl2Cpp<VrUiCursor>();
            ClassInjector.RegisterTypeInIl2Cpp<VrUiManager>();
            ClassInjector.RegisterTypeInIl2Cpp<FollowTarget>();
            ClassInjector.RegisterTypeInIl2Cpp<UuvrInput>();
            ClassInjector.RegisterTypeInIl2Cpp<UuvrPoseDriver>();
            ClassInjector.RegisterTypeInIl2Cpp<UuvrBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<AdditionalCameraData>();
            ClassInjector.RegisterTypeInIl2Cpp<ScreenMirrorPatchMode>();
            ClassInjector.RegisterTypeInIl2Cpp<VrCameraManager>();
            ClassInjector.RegisterTypeInIl2Cpp<CanvasRedirectPatchMode>();
            UuvrCore.Create();
        }
    }
}
