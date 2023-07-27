using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerPatches
{
    [HarmonyPatch(nameof(Player.CanEject))]
    public static void Postfix(ref bool __result)
    {
        if(Player.main.currentSub is SealSubRoot) __result = true;
    }

    [HarmonyPatch(nameof(Player.SetMotorMode))]//the game checks for subroot before vehicle, this fixes that
    public static bool Prefix(Player.MotorMode newMotorMode)
    {
        if (Player.main.currentSub is not SealSubRoot) return true;//not a seal
        if (!Player.main.currentMountedVehicle) return true;//not in a docked vehicle
        if (newMotorMode != Player.MotorMode.Walk) return true;//honestly not necessary but safety check works ig

        Player.main.SetMotorMode(Player.MotorMode.Vehicle);
        //want to be safe here, it's better to accidentally return true than accidentally return false
        return false;
    }

    [HarmonyPatch(nameof(Player.ExitLockedMode))]
    public static void Prefix(ref bool findNewPosition)
    {
        if (Player.main.currentSub is not SealSubRoot) return;
        if (!Player.main.currentMountedVehicle) return; 

        findNewPosition = true;
    }
}
