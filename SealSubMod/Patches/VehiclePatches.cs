using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(Vehicle))]
internal class VehiclePatches
{
    [HarmonyPatch(nameof(Vehicle.UpdateCollidersForDocking))]
    public static void Postfix(Vehicle __instance, bool docked)
    {
        if (!docked || !__instance) return;

        if (!__instance.GetComponentInParent<SealSubRoot>(true)) return;

        //it's set inactive when docking, but we want it active at all times for the seal dock
        __instance.disableDockedColliders.ForEach(col => col.enabled = true);
        __instance.collisionModel.SetActive(true);
    }

    [HarmonyPatch(nameof(Vehicle.ShouldSetKinematic))]
    public static bool Prefix(Vehicle __instance, ref bool __result)
    {
        if (!__instance.docked) return true;
        if (!__instance.GetComponentInParent<SealSubRoot>()) return true;
        __result = false;
        return false;
    }

    [HarmonyPatch(nameof(Vehicle.EnterVehicle))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, 
            new CodeMatch(instruction => 
                instruction.opcode == OpCodes.Callvirt && 
                instruction.operand is MethodInfo methodInfo && 
                methodInfo.Name == nameof(Player.SetCurrentSub)
            ));

        matcher.SetInstruction(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(VehiclePatches), nameof(VehiclePatches.SetPlayerSub))));

        return matcher.InstructionEnumeration();
    }
    public static void SetPlayerSub(Player player, SubRoot sub, bool forced)
    {
        if (player.currentSub is not SealSubRoot) player.SetCurrentSub(sub, forced);
    }

    private static SubRoot bayLastVehicleIn;//here to store information between the start of the method and the end of it

    [HarmonyPatch(nameof(Vehicle.Undock), MethodType.Enumerator)]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        //store information
        matcher.MatchForward(true, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Vehicle), nameof(Vehicle.EnterVehicle))));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1));
        matcher.Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehiclePatches), nameof(VehiclePatches.SetDockingBay))));


        //read information and edit value
        matcher.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), nameof(Vector3.down))));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1));
        matcher.SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehiclePatches), nameof(VehiclePatches.GetForceDirection))));

        return matcher.InstructionEnumeration();
    }

    public static void SetDockingBay(Vehicle seapickle) => bayLastVehicleIn = seapickle.GetComponentInParent<SubRoot>();

    public static Vector3 GetForceDirection(Vehicle vehcle)
    {
        return bayLastVehicleIn is SubRoot seal ? seal.GetComponentInChildren<SealDockingBay>().GetOutDirection() * 3 : Vector3.down;
    }
}