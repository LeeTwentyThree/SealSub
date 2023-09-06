using HarmonyLib;
using SealSubMod.MonoBehaviours;
using SealSubMod.Utility;

namespace SealSubMod.Patches.ModCompatPatches;

internal class TweaksAndFixesPatches
{
    public static void Patch(Harmony harmony)
    {
        var prefix = AccessTools.Method(typeof(TweaksAndFixesPatches), nameof(Prefix));
        ReflectionUtils.PatchIfExists(harmony, "Tweaks_Fixes", "Tweaks_Fixes.SubControl_Patch", "StartPostfix", new HarmonyMethod(prefix));
    }
    public static bool Prefix(SubControl __0)//__0 is another way to specify an argument, needed here because the argument is named __instance normally
    {
        return __0.sub is not SealSubRoot;
    }
}
