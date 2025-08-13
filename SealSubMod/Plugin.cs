using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using SealSubMod.Attributes;
using SealSubMod.Commands;
using SealSubMod.MonoBehaviours;
using SealSubMod.MonoBehaviours.Prefab;
using SealSubMod.Patches.ModCompatPatches;
using SealSubMod.Prefabs;
using System;
using System.Reflection;

namespace SealSubMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", "1.0.0.42")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static readonly AssetBundle assets = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly, "sealsubassets");

    internal static SaveCache SaveCache { get; } = SaveDataHandler.RegisterSaveDataCache<SaveCache>();

    public static EquipmentType SealModuleEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("SealModule");
    public static CraftTree.Type SealFabricatorTree { get; } = EnumHandler.AddEntry<CraftTree.Type>("SealFabricator").CreateCraftTreeRoot(out _);

    public static TechGroup SealGroup { get; } = EnumHandler.AddEntry<TechGroup>("Seal").WithPdaInfo("Seal");
    public static TechCategory SealCategory { get; } = EnumHandler.AddEntry<TechCategory>("Seal").RegisterToTechGroup(SealGroup).WithPdaInfo("Seal");
    public static TechCategory SealModuleCategory { get; } = EnumHandler.AddEntry<TechCategory>("SealModules").RegisterToTechGroup(SealGroup).WithPdaInfo("SealModules");

    internal static PingType SealPingType { get; private set; }

    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch(nameof(Creature))]
    public class CreatureSizeSetter : MonoBehaviour
    {
        public void Awake()
        {
            gameObject.GetComponent(this.GetType()).transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        // register harmony patches, if there are any
        var hamony = Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

        ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));

        UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync(false));

        SaveUtils.RegisterOnStartLoadingEvent( () => UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync(true)));

        LanguageHandler.RegisterLocalizationFolder();

        SealPingType = EnumHandler.AddEntry<PingType>("SealSub")
            .WithIcon(assets.LoadAsset<Sprite>("SealSubPing"));


        try
        {
            var component = GetComponentsInParent<Component>().Where(component => component.gameObject == gameObject && component is CreatureSizeSetter).FirstOrDefault(null);
        }
        catch (Exception _) { };


        for (int i = 0; i <= 8; i++)
        {
            Equipment.slotMapping.Add($"SealModule{i}", SealModuleEquipmentType);
        }

        CheckExternalModCompat(hamony);

        RegisterPrefabs();
        RegisterUpgradeModuleFunctionalities(Assembly.GetExecutingAssembly());

        ModAudio.RegisterAudio(assets);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void CheckExternalModCompat(Harmony harmony)
    {
        if (Chainloader.PluginInfos.ContainsKey("qqqbbb.subnautica.tweaksAndFixes"))
        {
            TweaksAndFixesPatches.Patch(harmony);
        }
    }

    internal static PrefabInfo DepthModuleMk1Info = PrefabInfo.WithTechType("SealHullModule1", null, null)
            .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule1));//I don't like it but it's here just for the console command thing

    private void RegisterPrefabs()
    {
        PrefabInfo DepthModuleMk2Info = PrefabInfo.WithTechType("SealHullModule2", null, null)
        .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule2));

        PrefabInfo DepthModuleMk3Info = PrefabInfo.WithTechType("SealHullModule3", null, null)
        .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule3));

        PrefabInfo SolarChargeModuleInfo = PrefabInfo.WithTechType("SealSolarChargeModule", null, null)
        .WithIcon(SpriteManager.Get(TechType.SeamothSolarCharge));

        PrefabInfo ThermalChargeModuleInfo = PrefabInfo.WithTechType("SealThermalChargeModule", null, null)
        .WithIcon(SpriteManager.Get(TechType.CyclopsThermalReactorModule));

        PrefabInfo SpeedModuleInfo = PrefabInfo.WithTechType("SealSpeedModule", null, null)
        .WithIcon(SpriteManager.Get(TechType.CyclopsThermalReactorModule));


        RegisterUpgradeModulePrefab(DepthModuleMk1Info);
        RegisterUpgradeModulePrefab(DepthModuleMk2Info);
        RegisterUpgradeModulePrefab(DepthModuleMk3Info);
        RegisterUpgradeModulePrefab(SolarChargeModuleInfo);
        RegisterUpgradeModulePrefab(ThermalChargeModuleInfo);
        RegisterUpgradeModulePrefab(SpeedModuleInfo);


        SealSubPrefab.Register();
    }
    public static void RegisterUpgradeModulePrefab(PrefabInfo info, RecipeData recipe = null)
    {
        var prefab = new CustomPrefab(info);
        prefab.SetGameObject(new CloneTemplate(info, TechType.CyclopsHullModule1));
        prefab.SetPdaGroupCategory(SealGroup, SealModuleCategory);
        prefab.SetUnlock(SealSubPrefab.SealType);

        if (recipe == null) recipe = JsonUtils.GetRecipeFromJson(info.TechType);
        prefab.SetRecipe(recipe).WithFabricatorType(SealFabricatorTree);

        prefab.SetEquipment(SealModuleEquipmentType);
        prefab.Register();
    }
    public static void RegisterUpgradeModuleFunctionalities(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<SealUpgradeModuleAttribute>();

            if (attribute == null || !type.IsClass || type.IsAbstract) continue;

            if (!Enum.TryParse(attribute.ModuleTechType, out TechType moduleType) && !EnumHandler.TryGetValue(attribute.ModuleTechType, out moduleType)) continue;


            SealSubRoot.moduleFunctions.Add(moduleType, type);
        }
    }
}