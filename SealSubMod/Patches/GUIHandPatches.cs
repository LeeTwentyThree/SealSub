using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(GUIHand))]
internal class GUIHandPatches
{
    [HarmonyPatch(nameof(GUIHand.UpdateActiveTarget))]
    public static void Postfix(GUIHand __instance)
    {
        if (!__instance.activeTarget || !__instance.activeTarget.TryGetComponent<VentMoveable>(out _)) return;
        __instance.suppressTooltip = true;
    }
}
