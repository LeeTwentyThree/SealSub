using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch]
internal class BasePieceGeometryPatches
{

    [HarmonyPatch(typeof(BaseNuclearReactorGeometry), nameof(BaseNuclearReactorGeometry.GetModule))]
    [HarmonyPatch(typeof(BaseBioReactorGeometry), nameof(BaseBioReactorGeometry.GetModule))]
    [HarmonyPatch(typeof(WaterParkGeometry), nameof(WaterParkGeometry.GetModule))]
    public static void Postfix(MonoBehaviour __instance, ref IBaseModule __result)
    {
        if (__instance.GetComponentInParent<BasePieceLocationMarker>()) __result = __instance.GetComponentInChildren<IBaseModule>(true);
    }
    [HarmonyPatch(typeof(BaseNuclearReactor), nameof(BaseNuclearReactor.GetModel))]
    [HarmonyPatch(typeof(BaseBioReactor), nameof(BaseBioReactor.GetModel))]
    public static void Postfix(MonoBehaviour __instance, ref IBaseModuleGeometry __result)
    {
        if (__instance.GetComponentInParent<BasePieceLocationMarker>()) __result = __instance.GetComponentInParent<IBaseModuleGeometry>();
    }
}
