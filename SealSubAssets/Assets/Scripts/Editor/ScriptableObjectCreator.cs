using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UWE.TUXOIL;

public class ScriptableObjectCreator : EditorWindow
{
    private string objectName;
    private string typeName;
    private Type loadedType;
    private List<Type> loadedTypes = new List<Type>();
    private Vector2 scrollPos;

    [MenuItem("Assets/Scriptable Object Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ScriptableObjectCreator));
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("Object Name");

        objectName = EditorGUILayout.TextField(objectName);

        EditorGUILayout.LabelField("Type Name");

        typeName = EditorGUILayout.TextField(typeName);

        loadedType = GetTheType();

        if (loadedType == null)
            EditorGUILayout.HelpBox("Failed to load type! View available type names below.", MessageType.Warning);

        if (GUILayout.Button("Create"))
            CreateAsset();

        EditorGUILayout.LabelField("Available type names:");

        if (GUILayout.Button("Refresh ScriptableObject types"))
            RefreshTypes();

        if (loadedTypes.Count == 0)
            EditorGUILayout.HelpBox("Please refresh to view all types", MessageType.Info);

        foreach (var loadedType in loadedTypes)
        {
            EditorGUILayout.LabelField(loadedType.Name);
        }

        EditorGUILayout.EndScrollView();
    }

    private Type GetTheType()
    {
        if (string.IsNullOrEmpty(typeName))
            return null;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }

    private void RefreshTypes()
    {
        loadedTypes.Clear();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type != null && typeof(ScriptableObject).IsAssignableFrom(type))
                    loadedTypes.Add(type);
            }
        }
    }

    private void CreateAsset()
    {
        var asset = CreateInstance(loadedType);
        AssetDatabase.CreateAsset(asset, Path.Combine("Assets", "ScriptableObjects", $"{objectName}.asset"));
    }
}
