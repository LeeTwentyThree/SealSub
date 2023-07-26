using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(SeaMoth))]
internal class SeamothPatches
{
    [HarmonyPatch(nameof(SeaMoth.UpdateDockedAnim))]
    public static bool Prefix(SeaMoth __instance)
    {
        if (!__instance.docked) 
            return true;

        if (Player.main.currentMountedVehicle != __instance)
            return true;

        if (!__instance.GetComponentInParent<SealSubRoot>())//maybe remove this check later. Seems unnecessary?
            return true;

        __instance.animator.SetBool("docked", false);
        return false;
    }
}
