using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System.Reflection.Emit;

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

    [HarmonyPatch(nameof(Vehicle.Undock), MethodType.Enumerator)]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), nameof(Vector3.down))));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
        matcher.SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehiclePatches), nameof(VehiclePatches.GetForceDirection))));

        return matcher.InstructionEnumeration();
    }
    public static Vector3 GetForceDirection(VehicleDockingBay bay)
    {
        return bay is SealDockingBay seal ? seal.GetOutDirection() : Vector3.down;
    }
}