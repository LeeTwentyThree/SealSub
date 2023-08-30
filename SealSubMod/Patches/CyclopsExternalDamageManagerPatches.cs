using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class CyclopsExternalDamageManagerPatches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.OnEnable))]
    public static bool Prefix(CyclopsExternalDamageManager __instance) => __instance.subRoot is not SealSubRoot;//lee we need to remove this
    //I'm lazy
    //it just uses the sub fire
    //which we don't set
    //and the subfire class sucks
    //so we don't want to set it
    //transpiler shouldn't be hard but I'm tired rn
}
