using HarmonyLib;
using SealSubMod.MonoBehaviours.Prefab;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(MapRoomFunctionality))]
internal class MapRoomFunctionalityPatches
{
    [HarmonyPatch(nameof(MapRoomFunctionality.Start))]//Make the map move properly with the offset
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


    [HarmonyPatch(nameof(MapRoomFunctionality.UpdateScanning))]
    [HarmonyTranspiler]//Make range upgrades increase the available offset distance rather than the hologram size, but only for the seal
    public static IEnumerable<CodeInstruction> SetRangeUpgradeFunctionality(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        var match = new CodeMatch(inst => inst.opcode == OpCodes.Ldsfld && inst.operand is FieldInfo info && info.Name == "_FadeRadius");
        matcher.MatchForward(false, match);
        matcher.Set(OpCodes.Ldarg_0, null);

        matcher.MatchForward(false, new CodeMatch(inst => inst.opcode == OpCodes.Callvirt));

        var meth = AccessTools.Method(typeof(MapRoomFunctionalityPatches), nameof(SetRange));
        matcher.SetOperandAndAdvance(meth);

        return matcher.InstructionEnumeration();
    }
    public static void SetRange(Material mat, MapRoomFunctionality funck, float rangeValue)
    {
        if (funck == null) return;
        
        var sealSpawner = funck.GetComponentInParent<MapRoomFunctionalitySpawner>();
        if (sealSpawner == null)
        {
            mat.SetFloat(ShaderPropertyID._FadeRadius, rangeValue);
            return;
        }
        var miniworldPos = sealSpawner.GetComponentInChildren<MapRoomFunctionalitySpawner.MiniWorldPosition>();
        miniworldPos.maxOffset = (offsetPerUpgrade * funck.storageContainer.container.GetCount(TechType.MapRoomUpgradeScanRange)) + 1;
    }
    public static float offsetPerUpgrade = 0.5f;
}
