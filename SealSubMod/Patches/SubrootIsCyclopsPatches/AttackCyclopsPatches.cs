using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System.Reflection.Emit;

namespace SealSubMod.Patches.SubrootIsCyclopsPatches;

[HarmonyPatch(typeof(AttackCyclops))]
internal class AttackCyclopsPatches
{
    public static readonly CodeMatch[] matches = new[]
    {
            new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Player), nameof(Player.main))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.currentSub))),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SubRoot), nameof(SubRoot.isCyclops)))
    };
    public static bool IsCyclopsBool()
    {
        return (Player.main.currentSub is SealSubRoot) || Player.main.currentSub.isCyclops;
    }

    [HarmonyPatch(nameof(AttackCyclops.UpdateAggression))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);
        matcher.MatchForward(false, matches);

        matcher.SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(AttackCyclopsPatches), nameof(AttackCyclopsPatches.IsCyclopsBool)));
        matcher.RemoveInstructions(2);

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(AttackCyclops.OnCollisionEnter))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)//generator isn't used, I'm just using this to allow for another overload
    {
        var matcher = new CodeMatcher(instructions);
        matcher.MatchForward(false, matches);

        matcher.SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(AttackCyclopsPatches), nameof(AttackCyclopsPatches.IsCyclopsBool)));
        matcher.RemoveInstructions(2);

        return matcher.InstructionEnumeration();
    }
}
