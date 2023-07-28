using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;
using System.Reflection.Emit;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(Base))]
internal class BasePatches
{
    [HarmonyPatch(nameof(Base.NormalizeCell))]
    public static void Postfix(ref Int3 __result)
    {
        if (Player.main.currentSub is SealSubRoot) __result = new Int3(0, 0, 0);//little bit hacky, but for some reason nothing else worked
    }
}
