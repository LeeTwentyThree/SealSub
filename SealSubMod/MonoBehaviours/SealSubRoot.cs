using SealSubMod.Interfaces;
using System;

namespace SealSubMod.MonoBehaviours;

internal class SealSubRoot : SubRoot
{
    public static Dictionary<TechType, Type> moduleFunctions = new();

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
        foreach(var console in Consoles)
        {
            console.modules.onEquip += OnEquip;
            console.modules.onUnequip += OnUnequip;
        }
        base.Start();
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

    public void NotifyOnChange(TechType type, bool added)
    {
        foreach(var onChange in moduleFunctionsRoot.GetComponents<IOnModuleChange>())
        {
            onChange.OnChange(type, added);
        }
    }
}
