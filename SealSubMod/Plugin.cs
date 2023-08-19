using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using SealSubMod.Commands;
using SealSubMod.MonoBehaviours.Prefab;
using SealSubMod.Prefabs;
using System.Reflection;

namespace SealSubMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static readonly AssetBundle assets = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly, "sealsubassets");

    internal static EquipmentType SealModuleEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("SealModule");

    internal static PingType SealPingType { get; private set; }

    internal static PrefabInfo DepthModuleMk1Info { get; } = PrefabInfo.WithTechType("SealHullModule1", null, null)
        .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule1));

    internal static PrefabInfo DepthModuleMk2Info { get; } = PrefabInfo.WithTechType("SealHullModule2", null, null)
    .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule2));

    internal static PrefabInfo DepthModuleMk3Info { get; } = PrefabInfo.WithTechType("SealHullModule3", null, null)
    .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule3));

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

        ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));

        UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync());

        LanguageHandler.RegisterLocalizationFolder();

        SealPingType = EnumHandler.AddEntry<PingType>("SealSub")
            .WithIcon(new Atlas.Sprite(assets.LoadAsset<Sprite>("SealSubPing")));

        for (int i = 0; i <= 8; i++)
        {
            Equipment.slotMapping.Add($"SealModule{i}", SealModuleEquipmentType);
        }

        RegisterPrefabs();

        ModAudio.RegisterAudio(assets);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void RegisterPrefabs()
    {
        SealSubPrefab.Register();

        RegisterUpgradeModulePrefab(DepthModuleMk1Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
        RegisterUpgradeModulePrefab(DepthModuleMk2Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
        RegisterUpgradeModulePrefab(DepthModuleMk3Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
    }

    private static void RegisterUpgradeModulePrefab(PrefabInfo info, RecipeData recipe)
    {
        var prefab = new CustomPrefab(info);
        prefab.SetGameObject(new CloneTemplate(info, TechType.CyclopsHullModule1));
        prefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
        prefab.SetRecipe(recipe);
        prefab.SetEquipment(SealModuleEquipmentType);
        prefab.Register();
    }
}