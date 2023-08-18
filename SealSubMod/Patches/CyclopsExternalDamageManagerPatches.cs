using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class CyclopsExternalDamageManagerPatches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.OnEnable))]
    public static bool Prefix(CyclopsExternalDamageManager __instance) => __instance.subRoot is not SealSubRoot;
}
