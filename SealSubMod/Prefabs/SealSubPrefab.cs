using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using SealSubMod.MonoBehaviours.Abstract;

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

        var prefabModifiers = prefab.GetComponentsInChildren<PrefabModifier>(true);
        foreach (var modifier in prefabModifiers.Where(modifier => modifier is PrefabModifierAsync).Cast<PrefabModifierAsync>())
        {
            yield return modifier.SetupPrefabAsync(prefab);
        }

        foreach (var leeʼsUnnamedVariable in prefabModifiers)
        {//hate you lee
            leeʼsUnnamedVariable.OnAsyncPrefabTasksCompleted();
        }

        foreach (var lateOperation in prefabModifiers)
        {
            lateOperation.OnLateMaterialOperation();//moved here because I thought having all the prefab modifying happening in the same place was a bit nicer
        }

        gameObject.Set(prefab);
    }
}