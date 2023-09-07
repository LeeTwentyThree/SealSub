using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.Prefabs;

internal class SealSubPrefab
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("SealSub", null, null);
    public static TechType SealType { get; } = Info.TechType;

    public static void Register()
    {
        CustomPrefab prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetGameObject);
        prefab.SetRecipe(new Nautilus.Crafting.RecipeData(
            new CraftData.Ingredient(TechType.PlasteelIngot, 6),
            new CraftData.Ingredient(TechType.EnameledGlass, 4),
            new CraftData.Ingredient(TechType.AdvancedWiringKit, 2),
            new CraftData.Ingredient(TechType.Benzene, 2),
            new CraftData.Ingredient(TechType.Aerogel, 2),
            new CraftData.Ingredient(TechType.Magnetite, 3),
            new CraftData.Ingredient(TechType.Nickel, 4)
            ))
            .WithFabricatorType(CraftTree.Type.Constructor)
            .WithStepsToFabricatorTab("Vehicles");
        prefab.SetPdaGroupCategory(TechGroup.Constructor, TechCategory.Constructor);
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
