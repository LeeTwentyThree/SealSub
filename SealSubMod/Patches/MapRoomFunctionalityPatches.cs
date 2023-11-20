using HarmonyLib;
using System;
using System.Reflection.Emit;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(MapRoomFunctionality))]
internal class MapRoomFunctionalityPatches
{
    [HarmonyPatch(nameof(MapRoomFunctionality.Start))]
    public static IEnumerable<CodeInstruction> Transpiler/*? I hardly kn-*/(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        var meth = AccessTools.Method(typeof(Component), nameof(Component.GetComponentInParent), Array.Empty<Type>(), new Type[1] { typeof(Base) });
        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Call, meth));

        matcher.RemoveInstructionsInRange(matcher.Pos + 1, matcher.Pos + 5);
        matcher.Advance(1);
        matcher.Insert(Transpilers.EmitDelegate<Action<MapRoomFunctionality>>((functionality) =>
        {
            if (functionality.GetComponentInParent<Base>())//original method didn't have a safety check here, it assumed it would always find a base, and threw an error
                functionality.GetComponentInParent<Base>().onPostRebuildGeometry += functionality.OnPostRebuildGeometry;
        }));

        return matcher.InstructionEnumeration();
    }
}
