using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// Player settings for the Android target, applied from code so they are reproducible.
    /// </summary>
    public static class ProjectSetup
    {
        private const string CompanyName = "OtherStickmanGames";
        private const string ProductName = "The Murk";
        private const string ApplicationId = "com.OtherStickmanGames.TheMurk";

        [MenuItem("The Murk/Apply Project Settings")]
        public static void ApplyProjectSettings()
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationId);

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            AssetDatabase.SaveAssets();
            Debug.Log("[ProjectSetup] Android player settings applied.");
        }

        /// <summary>Batch-mode entry: settings plus the bootstrap scene in one editor launch.</summary>
        public static void SetupAll()
        {
            ApplyProjectSettings();
            BootstrapSceneBuilder.Rebuild();
        }
    }
}
