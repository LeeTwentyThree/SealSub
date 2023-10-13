using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(SubRoot))]
internal class SubrootPatches
{
    [HarmonyPatch(nameof(SubRoot.OnProtoDeserialize))]
    public static bool Prefix(SubRoot __instance)
    {
        if (__instance is not SealSubRoot seal) return true;

        Plugin.Logger.LogMessage($"id: {seal.GetComponent<PrefabIdentifier>().Id}");
        seal.LoadSaveData(seal.GetComponent<PrefabIdentifier>().Id);

        return false;//Temporary fix until subname is properly implemented
    }
}