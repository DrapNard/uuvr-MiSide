using System.IO;
using System.Reflection;
using BepInEx;
using Uuvr.OpenVR;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;

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
            // Set the static instance of this plugin
            _instance = this;

            // Determine the plugin's folder path
            ModFolderPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(UuvrPlugin)).Location);

            // Initialize configuration
            new ModConfiguration(Config);

            // Register Il2Cpp types
            RegisterIl2CppTypes();

            // Create the core instance
            UuvrCore.Create();
        }

        private void RegisterIl2CppTypes()
        {
            // Register all required Il2Cpp types
            ClassInjector.RegisterTypeInIl2Cpp<OpenVrManager>();
            ClassInjector.RegisterTypeInIl2Cpp<UuvrCore>();
        }
    }
}
