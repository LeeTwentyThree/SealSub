using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System.Reflection.Emit;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(VehicleDockingBay))]
internal class DockingBayPatches
{
    [HarmonyPatch(nameof(VehicleDockingBay.UpdateDockedPosition))]
    public static bool Prefix(VehicleDockingBay __instance, Vehicle vehicle, float interpfraction)
    {
        if (__instance is not SealDockingBay seal) return true;

        seal.UpdateVehclPos(vehicle, interpfraction);

        return false;
    }
    [HarmonyPatch(nameof(VehicleDockingBay.DockVehicle))]
    public static void Postfix(VehicleDockingBay __instance, Vehicle vehicle)
    {
        if (__instance is not SealDockingBay seal) return;

        Player.main.currentMountedVehicle = vehicle;//The player vehicle is set null, we don't want that because the player is still in the vehicle while docked
    }
}