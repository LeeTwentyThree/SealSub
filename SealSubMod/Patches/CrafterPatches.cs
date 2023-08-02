using HarmonyLib;
using SealSubMod.Prefabs;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(Crafter))]
internal class CrafterPatches
{
    [HarmonyPatch(nameof(Crafter.Craft))]
    public static void Prefix(TechType techType, ref float duration)
    {
        if (techType == SealSubPrefab.SealType)
        {
            duration = 25;
        }
    }
}