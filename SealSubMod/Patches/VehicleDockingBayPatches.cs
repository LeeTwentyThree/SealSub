using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System.Reflection.Emit;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(VehicleDockingBay))]
internal class VehicleDockingBayPatches
{
    /*
    [HarmonyPatch(nameof(VehicleDockingBay.LateUpdate))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(true, new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(VehicleDockingBay), nameof(VehicleDockingBay.interpolatingVehicle))));
        matcher.Insert(new CodeInstruction(OpCodes.Ldarg_0));
        matcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehicleDockingBayPatches), nameof(VehicleDockingBayPatches.IsSealBay))));
        //OpCodes.ret

        return matcher.InstructionEnumeration();
    }
    public static bool IsSealBay(VehicleDockingBay bay) => bay is SealDockingBay;
    */
    [HarmonyPatch(nameof(VehicleDockingBay.UpdateDockedPosition))]
    public static bool Prefix(VehicleDockingBay __instance, Vehicle vehicle)
    {
        if (__instance is not SealDockingBay seal) return true;

        seal.UpdateVehclPos(vehicle);

        return false;
    }
}
