using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.EditorTools
{
    /// <summary>
    /// APK build usable from the menu and from the command line (-executeMethod Game.EditorTools.AndroidBuild.Build).
    /// </summary>
    public static class AndroidBuild
    {
        private const string OutputPath = "Builds/Android/TheMurk.apk";

        [MenuItem("The Murk/Build Android APK")]
        public static void Build()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("No scenes in Build Settings. Run 'The Murk/Rebuild Bootstrap Scene'.");

            EditorUserBuildSettings.buildAppBundle = false;

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

            var summary = report.summary;
            if (summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Android build {summary.result}: {summary.totalErrors} errors.");

            Debug.Log($"[AndroidBuild] {OutputPath}, {summary.totalSize / (1024 * 1024)} MB, {summary.totalTime.TotalSeconds:0} s.");
        }
    }
}
