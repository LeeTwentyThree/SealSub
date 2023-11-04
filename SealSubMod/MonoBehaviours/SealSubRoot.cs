using Nautilus.Json;
using SealSubMod.Interfaces;
using System;

namespace SealSubMod.MonoBehaviours;

public class SealSubRoot : SubRoot, IProtoEventListener
{
    internal static Dictionary<TechType, Type> moduleFunctions = new();


    private SaveData _saveData;
    public SaveData SaveData { get => _saveData; }


    [SerializeField] GameObject moduleFunctionsRoot;

    [SerializeField] SealUpgradeConsole[] _consoles;
    public SealUpgradeConsole[] Consoles { get => _consoles; }

    public static void RegisterModuleFunction(TechType techType, Type moduleFunction)
    {
        moduleFunctions[techType] = moduleFunction;
        Plugin.Logger.LogInfo($"Module type {techType} registered");
    }

    public static void RegisterModuleFunction<T>(TechType techType) where T : MonoBehaviour => RegisterModuleFunction(techType, typeof(T));



    public void AddIsAllowedToAddListener(IsAllowedToAdd @delegate)
    {
        foreach(var console in Consoles) console.modules.isAllowedToAdd += @delegate;
    }
    public void RemoveIsAllowedToAddListener(IsAllowedToAdd @delegate)
    {
        foreach(var console in Consoles) console.modules.isAllowedToAdd -= @delegate;
    }

    public override void Start()
    {
        foreach (var console in Consoles)
        {
            console.modules.onEquip += OnEquip;
            console.modules.onUnequip += OnUnequip;
        }
        base.Start();
    }

    public override void Awake()
    {
        base.Awake();
        _saveData = new SaveData();//make a new entry for the sub
    }
    

    private void OnEnable() => Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
    private void OnDisable() => Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;

    private void OnBeforeSave(object _, JsonFileEventArgs __)
    {
        Plugin.SaveCache.saves[GetComponent<PrefabIdentifier>().Id] = SaveData;
    }

    internal void LoadSaveData(string id)
    {
        if (!Plugin.SaveCache.saves.TryGetValue(id, out var data))
        {
            Plugin.Logger.LogError($"Seal {id} load save data but no save data found! Object was loaded from existing save, but corresponding save data for unique identifier id was not found. This could mean there's a mismatch in the ids, and possibly a race condition");
            return;
        }
        _saveData = data;

        foreach (var saveListener in GetComponentsInChildren<IOnSaveDataLoaded>(true)) saveListener.OnSaveDataLoaded(SaveData);
        //its an interface because we can't guarantee that this method runs before all others that need the save data, so better to just let them do their thing once its loaded
        //avoids race conditions
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        var type = moduleFunctions[item.item.GetTechType()];
        var function = moduleFunctionsRoot.GetComponent(type);
        (function as MonoBehaviour).enabled = false;
        Destroy(function);
        NotifyOnChange(item.item.GetTechType(), false);
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        var type = moduleFunctions[item.item.GetTechType()];
        var function = moduleFunctionsRoot.AddComponent(type);
        NotifyOnChange(item.item.GetTechType(), true);
    }

    private void NotifyOnChange(TechType type, bool added)
    {
        foreach(var onChange in moduleFunctionsRoot.GetComponents<IOnModuleChange>())
        {
            if (!(onChange as MonoBehaviour).enabled) continue;
            onChange.OnChange(type, added);
        }
    }
}
