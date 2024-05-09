using HarmonyLib;
using System;
using System.Reflection.Emit;
using static SealSubMod.MonoBehaviours.Prefab.MapRoomFunctionalitySpawner;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(MiniWorld))]
internal class MiniWorldPatches
{
    [HarmonyPatch(nameof(MiniWorld.UpdatePosition))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        var transformProperty = AccessTools.PropertyGetter(typeof(Component), nameof(Component.transform));
        var positionProperty = AccessTools.PropertyGetter(typeof(Transform), nameof(Transform.position));

        Func<MiniWorld, Vector3> del = (mini) => mini.TryGetComponent<MiniWorldPosition>(out var pos) ? (mini.transform.position + (pos.Offset * -1)) : mini.transform.position;

        matcher.MatchForward(true, new CodeMatch(OpCodes.Call, transformProperty), new CodeMatch(OpCodes.Callvirt, positionProperty));
        matcher.Advance(1);
        
        matcher.MatchForward(false, new CodeMatch(OpCodes.Call, transformProperty), new CodeMatch(OpCodes.Callvirt, positionProperty));//There's two of them, we only need to replace the second one
        matcher.RemoveInstruction();
        matcher.SetInstruction(Transpilers.EmitDelegate(del));

        return matcher.InstructionEnumeration();
    }
}
