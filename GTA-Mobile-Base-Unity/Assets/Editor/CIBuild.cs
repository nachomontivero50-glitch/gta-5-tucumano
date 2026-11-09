using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
// Arma la escena y la configuración de Android sola, para compilar el APK en GitHub Actions sin abrir Unity.
public static class CIBuild {
    public static void BuildAndroid() {
        const string scenePath = "Assets/Scenes/Main.unity";
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("Juego").AddComponent<GameBootstrap>();
        EditorSceneManager.SaveScene(scene, scenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };

        PlayerSettings.companyName = "TucumanGames"; PlayerSettings.productName = "Tucuman Abierto";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.tucumangames.abierto");
        PlayerSettings.bundleVersion = "0.1"; PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false; PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true; PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
        EditorUserBuildSettings.buildAppBundle = false;
        AddShader("Standard");

        string path = Arg("-customBuildPath") ?? "build/Android/TucumanAbierto.apk";
        if (!path.EndsWith(".apk")) path = Path.Combine(path, "TucumanAbierto.apk");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { scenePath }, locationPathName = path, target = BuildTarget.Android, options = BuildOptions.None });
        bool ok = report.summary.result == BuildResult.Succeeded;
        Debug.Log(ok ? "Build OK: " + path : "Build FALLÓ");
        EditorApplication.Exit(ok ? 0 : 1);
    }
    static string Arg(string name) { var a = Environment.GetCommandLineArgs(); for (int i = 0; i < a.Length - 1; i++) if (a[i] == name) return a[i + 1]; return null; }
    static void AddShader(string name) {
        try {
            var shader = Shader.Find(name); var all = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
            if (shader == null || all == null || all.Length == 0) return;
            var so = new SerializedObject(all[0]); var arr = so.FindProperty("m_AlwaysIncludedShaders");
            for (int i = 0; i < arr.arraySize; i++) if (arr.GetArrayElementAtIndex(i).objectReferenceValue == shader) return;
            arr.InsertArrayElementAtIndex(arr.arraySize); arr.GetArrayElementAtIndex(arr.arraySize - 1).objectReferenceValue = shader; so.ApplyModifiedProperties();
        } catch (Exception e) { Debug.LogWarning("AddShader: " + e.Message); }
    }
}
