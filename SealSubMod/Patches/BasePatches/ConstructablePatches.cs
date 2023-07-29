using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches.BasePatches;//not technically a base code patch, but it's being patches because of base code reasons

[HarmonyPatch(typeof(Constructable))]
internal class ConstructablePatches
{
    [HarmonyPatch(nameof(Constructable.InitializeModelCopy))]
    public static bool Prefix(Constructable __instance) => Player.main.currentSub is SealSubRoot && __instance.GetComponentInParent<BasePieceLocationMarker>();
}
