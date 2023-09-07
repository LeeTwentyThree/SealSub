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

    internal static PrefabInfo SolarChargeModuleInfo { get; } = PrefabInfo.WithTechType("SealSolarChargeModule", null, null)
    .WithIcon(SpriteManager.Get(TechType.SeamothSolarCharge));

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

        UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync());

        LanguageHandler.RegisterLocalizationFolder();

        SealPingType = EnumHandler.AddEntry<PingType>("SealSub")
            .WithIcon(new Atlas.Sprite(assets.LoadAsset<Sprite>("SealSubPing")));

        try
        {
            var component = GetComponentsInParent<Component>().Where(component => component.gameObject == gameObject && component is CreatureSizeSetter).FirstOrDefault(null);
        }
        catch (Exception _) { };


        for (int i = 0; i <= 8; i++)
        {
            Equipment.slotMapping.Add($"SealModule{i}", SealModuleEquipmentType);
        }

        foreach(var type in Assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<SealUpgradeModuleAttribute>();

            if (attribute == null) continue;

            if (!Enum.TryParse(attribute.ModuleTechType, out TechType moduleType) && !EnumHandler.TryGetValue(attribute.ModuleTechType, out moduleType)) continue;


            //remove line later, I'm just lazy
            CraftDataHandler.SetEquipmentType(moduleType, SealModuleEquipmentType);


            SealSubRoot.moduleFunctions.Add(moduleType, type);
        }

        CheckExternalModCompat(hamony);

        RegisterPrefabs();

        ModAudio.RegisterAudio(assets);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    public void CheckExternalModCompat(Harmony harmony)
    {
        if (Chainloader.PluginInfos.ContainsKey("qqqbbb.subnautica.tweaksAndFixes"))
        {
            TweaksAndFixesPatches.Patch(harmony);
        }
    }

    private void RegisterPrefabs()
    {
        SealSubPrefab.Register();

        RegisterUpgradeModulePrefab(DepthModuleMk1Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
        RegisterUpgradeModulePrefab(DepthModuleMk2Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
        RegisterUpgradeModulePrefab(DepthModuleMk3Info, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
        RegisterUpgradeModulePrefab(SolarChargeModuleInfo, new RecipeData(new CraftData.Ingredient(TechType.Titanium, 2)));
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