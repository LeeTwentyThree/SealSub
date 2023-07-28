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
    [HarmonyPatch(nameof(Base.BuildRoomGeometry))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Int3), nameof(Int3.one))));

        while(matcher.Opcode != OpCodes.Stloc_3)
        {
            matcher.RemoveInstruction();
        }
        matcher.ThrowIfInvalid("Idfk it was invalid, tf you want me to say here?");
        matcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BasePatches), nameof(BasePatches.GetBuildPosition))));

        return matcher.InstructionEnumeration();
    }

    public static Vector3 GetBuildPosition(Int3 @int)
    {
        ErrorMessage.AddMessage("Getting build position, player sub " + Player.main.currentSub);
        if(Player.main.currentSub is not SealSubRoot root) return Int3.Scale(@int - Int3.one, Base.halfCellSize);

        var cam = MainCamera.camera;
        var marker = BasePieceLocationMarker.GetNearest(cam.transform.position, cam.transform.forward, root.GetComponentsInChildren<BasePieceLocationMarker>(true));
        if (!marker) throw new InvalidOperationException("Shis fucked.");
        ErrorMessage.AddMessage("Returning position " + marker.pos);
        return new Vector3(0, 0, 0);
    }
}
