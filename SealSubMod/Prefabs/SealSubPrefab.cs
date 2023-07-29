using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using SealSubMod.Interfaces;

namespace SealSubMod.Prefabs;

internal class SealSubPrefab
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("SealSub", "Seal Sub", "Seal Sub that makes me go yes");
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

        var asyncOperations = prefab.GetComponentsInChildren<IAsyncPrefabSetupOperation>(true);
        foreach (var task in asyncOperations)
        {
            yield return task.SetupPrefabAsync();
        }
        
        var onCompleted = prefab.GetComponentsInChildren<IOnAsyncPrefabTasksCompleted>(true);
        foreach (var leeʼsUnnamedVariable in onCompleted)
        {
            leeʼsUnnamedVariable.OnAsyncPrefabTasksCompleted();
        }

        gameObject.Set(prefab);
    }
}