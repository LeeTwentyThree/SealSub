using HarmonyLib;
using UnityEngine.UI;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_EquipmentPatches
{
    [HarmonyPatch(nameof(uGUI_Equipment.Awake))]
    [HarmonyPrefix]
    public static void AwakePrefix(uGUI_Equipment __instance)
    {
        // Left modules
        var slot1 = CloneSlot(__instance, "CyclopsModule1", "SealModule1");
        ApplyChangesToSealSubModuleImage(slot1.transform.Find("Cyclops").GetComponent<Image>());
        CloneSlot(__instance, "CyclopsModule3", "SealModule2");
        CloneSlot(__instance, "CyclopsModule4", "SealModule3");
        CloneSlot(__instance, "CyclopsModule6", "SealModule4");

        // Right modules
        var slot5 = CloneSlot(__instance, "CyclopsModule1", "SealModule5");
        ApplyChangesToSealSubModuleImage(slot5.transform.Find("Cyclops").GetComponent<Image>());
        CloneSlot(__instance, "CyclopsModule3", "SealModule6");
        CloneSlot(__instance, "CyclopsModule4", "SealModule7");
        CloneSlot(__instance, "CyclopsModule6", "SealModule8");
    }

    private static void ApplyChangesToSealSubModuleImage(Image sealSubModuleImage)
    {
        sealSubModuleImage.sprite = Plugin.assets.LoadAsset<Sprite>("SealSubModulesBackground");
        sealSubModuleImage.gameObject.name = "Seal";
        sealSubModuleImage.transform.localScale = new Vector3(1.3f, 1.3f, 1);
        sealSubModuleImage.transform.localPosition = new Vector3(210, -135, 0);
    }

    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
    {
        var slotObj = Object.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        slotObj.name = newSlotName;
        var slot = slotObj.GetComponent<uGUI_EquipmentSlot>();
        slot.slot = newSlotName;
        return slot;
    }
}