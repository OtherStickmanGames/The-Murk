using System.IO;
using Game.App;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.EditorTools
{
    /// <summary>
    /// Builds the Bootstrap scene from scratch. Rerun instead of editing the scene by hand.
    /// </summary>
    public static class BootstrapSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Bootstrap.unity";
        private const string UiSettingsFolder = "Assets/Game/UI/Settings";
        private const string ThemePath = UiSettingsFolder + "/RuntimeTheme.tss";
        private const string PanelSettingsPath = UiSettingsFolder + "/GamePanelSettings.asset";

        // Portrait phone reference; UI scales by width so layouts hold on tall and short screens alike.
        private static readonly Vector2Int ReferenceResolution = new Vector2Int(1080, 1920);

        [MenuItem("The Murk/Rebuild Bootstrap Scene")]
        public static void Rebuild()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // After NewScene: opening a scene unloads unreferenced assets, which would null a freshly created PanelSettings.
            var panelSettings = EnsurePanelSettings();

            var cameraGo = new GameObject("Main Camera") { tag = "MainCamera" };
            cameraGo.transform.SetPositionAndRotation(new Vector3(0f, 20f, -14f), Quaternion.Euler(55f, 0f, 0f));
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.07f, 0.07f, 0.09f);
            cameraGo.AddComponent<AudioListener>();

            var lightGo = new GameObject("Sun");
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.Soft;

            var bootstrapGo = new GameObject("Bootstrap");
            var bootstrap = bootstrapGo.AddComponent<Bootstrap>();
            var serialized = new SerializedObject(bootstrap);
            serialized.FindProperty("_panelSettings").objectReferenceValue = panelSettings;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            if (serialized.FindProperty("_panelSettings").objectReferenceValue == null)
                throw new System.InvalidOperationException("Bootstrap lost its PanelSettings reference.");

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            Debug.Log($"[BootstrapSceneBuilder] Saved {ScenePath} as the only build scene.");
        }

        private static PanelSettings EnsurePanelSettings()
        {
            Directory.CreateDirectory(UiSettingsFolder);

            if (!File.Exists(ThemePath))
            {
                File.WriteAllText(ThemePath, "@import url(\"unity-theme://default\");\n");
                AssetDatabase.ImportAsset(ThemePath);
            }

            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, PanelSettingsPath);
            }

            panelSettings.themeStyleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);
            panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSettings.referenceResolution = ReferenceResolution;
            panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panelSettings.match = 0f;
            EditorUtility.SetDirty(panelSettings);
            AssetDatabase.SaveAssets();

            return panelSettings;
        }
    }
}
