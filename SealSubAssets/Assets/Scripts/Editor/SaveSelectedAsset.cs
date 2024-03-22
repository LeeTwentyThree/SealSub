using System.IO;
using UnityEditor;
using UnityEngine;

// I can't believe I have to make this, but it seems necessary to GENERATE assets through Thunderkit
// and SAVE assets through an editor script, due to bugs in either Thunderkit or Unity
public class SaveSelectedAsset : Editor
{
    [MenuItem("Assets/Save Selected Asset")]
    public static void SaveAsset()
    {
        var activeObj = Selection.activeObject;
        if (activeObj == null)
        {
            Debug.LogError("Cannot save an asset when no asset is selected!");
            return;
        }
        var path = Path.Combine("Assets", $"{activeObj.GetType()}.asset");
        AssetDatabase.CreateAsset(activeObj, AssetDatabase.GenerateUniqueAssetPath(path));
        ProjectWindowUtil.ShowCreatedAsset(activeObj);
    }
}