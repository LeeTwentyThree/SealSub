using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;
using System.Reflection.Emit;
using System.Security.Policy;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(Vehicle))]
internal class VehiclePatches
{
    [HarmonyPatch(nameof(Vehicle.UpdateCollidersForDocking))]
    public static bool Prefix(Vehicle __instance, bool docked)
    {
        if (!docked || !__instance) return true;

        if (!__instance.GetComponentInParent<SealSubRoot>()) return true;

        __instance.collisionModel.SetActive(true);//it's set inactive when docking, but we want it active at all times for the seal dock
        return false;
    }

    [HarmonyPatch(nameof(Vehicle.ShouldSetKinematic))]
    public static bool Prefix(Vehicle __instance, ref bool __result)
    {
        return true;//Patch doesn't really work very well for some reason
        //vehicles seem to refuse to move in the seal dock for some reason
        //yes its annoying

        if (!__instance.docked) return true;
        if (!__instance.GetComponentInParent<SealSubRoot>()) return true;
        __result = false;
        return false;
    }

    [HarmonyPatch(nameof(Vehicle.EnterVehicle))]
    public static bool Prefix(Vehicle __instance, Player player, bool teleport, bool playEnterAnimation = true)
    {
        if (player == null || player.currentSub is not SealSubRoot) return true;

        //this is the only line we don't want
        //player.SetCurrentSub(null, false);


        player.playerController.UpdateController();
        player.EnterLockedMode(__instance.playerPosition.transform, teleport);
        player.sitting = __instance.playerSits;
        player.currentMountedVehicle = __instance;
        player.playerController.ForceControllerSize();
        if (!string.IsNullOrEmpty(__instance.customGoalOnEnter))
        {
            GoalManager.main.OnCustomGoalEvent(__instance.customGoalOnEnter);
        }
        if (!__instance.energyInterface.hasCharge)
        {
            if (__instance.noPowerWelcomeNotification)
            {
                __instance.noPowerWelcomeNotification.Play();
            }
        }
        else if (__instance.welcomeNotification)
        {
            __instance.welcomeNotification.Play();
        }
        __instance.pilotId = player.GetComponent<UniqueIdentifier>().Id;
        __instance.mainAnimator.SetBool("enterAnimation", playEnterAnimation);

        return false;
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