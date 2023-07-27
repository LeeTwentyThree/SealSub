using System.IO;
using UnityEditor;
using UnityEngine;

public class FMODAssetCreator : EditorWindow
{
    private string name;
    private string soundPath;
    private string soundId;

    [MenuItem("Assets/FMOD Asset Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FMODAssetCreator));
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Name");
        name = EditorGUILayout.TextField(name);
        EditorGUILayout.LabelField("Sound Path");
        soundPath = EditorGUILayout.TextField(soundPath);
        EditorGUILayout.LabelField("Sound ID");
        soundId = EditorGUILayout.TextField(soundId);
        if (GUILayout.Button("Create"))
            CreateAsset();
    }
    
    private void CreateAsset()
    {
        var asset = CreateInstance<FMODAsset>();
        asset.name = name;
        asset.path = soundPath;
        asset.id = soundId;
        AssetDatabase.CreateAsset(asset, Path.Combine("Assets", "FMODAssets", $"{name}.asset"));
    }
}
