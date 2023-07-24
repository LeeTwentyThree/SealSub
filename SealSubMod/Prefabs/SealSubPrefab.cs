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
        model.AddComponent<SphereCollider>();

        var asyncOperations = model.GetComponentsInChildren<IAsyncPrefabSetupOperation>();
        foreach (var task in asyncOperations)
        {
            yield return task;
        }

        var onCompleted = model.GetComponentsInChildren<IOnAsyncPrefabTasksCompleted>();
        foreach (var leeʼsUnnamedVariable in onCompleted)
        {
            leeʼsUnnamedVariable.OnAsyncPrefabTasksCompleted();
        }

        gameObject.Set(model);
    }
}