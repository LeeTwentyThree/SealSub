using Nautilus.Assets;
using SealSubMod.Interfaces;

namespace SealSubMod.Prefabs;

internal class SealSubPrefab
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("SealSub", "Seal Sub", "Seal Sub that makes me go yes.");

    public static void Register()
    {
        CustomPrefab prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetGameObject);
        prefab.Register();
    }

    private static IEnumerator GetGameObject(IOut<GameObject> gameObject)
    {
        var model = Plugin.assets.LoadAsset<GameObject>("SealSubPrefab");
        model.SetActive(false);
        var prefab = Object.Instantiate(model);

        var asyncOperations = prefab.GetComponentsInChildren<IAsyncPrefabSetupOperation>(true);
        foreach (var task in asyncOperations)
        {
            yield return task;
        }
        
        var onCompleted = prefab.GetComponentsInChildren<IOnAsyncPrefabTasksCompleted>(true);
        foreach (var leeʼsUnnamedVariable in onCompleted)
        {
            leeʼsUnnamedVariable.OnAsyncPrefabTasksCompleted();
        }

        gameObject.Set(prefab);
    }
}