using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(Builder))]
internal class BuilderPatches//Allows you to build base pieces even though the game thinks they intersect with an ACU built on another floor
{
    [HarmonyPatch(nameof(Builder.GetObstacles))]
    public static void Prefix(ref Func<Collider, bool> filter)
    {
        //This will technically affect all base pieces, but in this case that's not a big deal because only pieces placed in the seal would ever be colliding with another piece inside the seal anyway
        filter += col => col.GetComponentInParent<BasePieceLocationMarker>();//the interfering collider is part of another base piece, so ignore it
    }
}
