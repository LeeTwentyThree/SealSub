using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.Prefabs;

public class SealSubPrefab
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("SealSub", null, null);
    public static TechType SealType { get; } = Info.TechType;

    internal static void Register()
    {
        CustomPrefab prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetGameObject);
        prefab.SetRecipe(JsonUtils.GetRecipeFromJson(SealType))
            .WithFabricatorType(CraftTree.Type.Constructor)
            .WithStepsToFabricatorTab("Vehicles");

        prefab.SetPdaGroupCategory(Plugin.SealGroup, Plugin.SealCategory);
        var scanGadget = prefab.SetUnlock(TechType.Constructor);//temporary, until we add something like a wreck or fragments
        scanGadget.WithAnalysisTech(null, null, null, null);//required by nautilus update
        prefab.Register();
    }

    private static IEnumerator GetGameObject(IOut<GameObject> gameObject)
    {
        var model = Plugin.assets.LoadAsset<GameObject>("SealSubPrefab");
        model.SetActive(false);
        var prefab = Object.Instantiate(model);


        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();

        var cyclopsReferencers = prefab.GetComponentsInChildren<ICyclopsReferencer>();
        foreach(var referencer in cyclopsReferencers)
            referencer.OnCyclopsReferenceFinished(CyclopsReferenceManager.CyclopsReference);


        var prefabModifiers = prefab.GetComponentsInChildren<PrefabModifier>(true);
        foreach (var modifier in prefabModifiers.Where(modifier => modifier is PrefabModifierAsync).Cast<PrefabModifierAsync>())
            yield return modifier.SetupPrefabAsync();


        foreach (var leeʼsUnnamedVariable in prefabModifiers)//hate you lee
            leeʼsUnnamedVariable.OnAsyncPrefabTasksCompleted();


        foreach (var lateOperation in prefabModifiers)
            lateOperation.OnLateMaterialOperation();


        if (System.DateTime.Now.Month == 6 || Gaytilities.GayModeActive) Gaytilities.EngaygeGayification(prefab);

        gameObject.Set(prefab);
    }
}
