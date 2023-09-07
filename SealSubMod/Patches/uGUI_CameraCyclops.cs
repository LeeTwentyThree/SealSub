using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(uGUI_CameraCyclops))]
internal class uGUI_CameraCyclopsPatches
{
    private static string[] _cameraNames = new string[]
    {
        "SealExternalCam1",
        "SealExternalCam2",
        "SealExternalCam3",
        "SealExternalCam4"
    };

    [HarmonyPatch(nameof(uGUI_CameraCyclops.SetCamera))]
    [HarmonyPostfix]
    public static void SetCameraPostfix(uGUI_CameraCyclops __instance)
    {
        if (Player.main.GetCurrentSub() is not SealSubRoot)
            return;

        if (__instance.cameraIndex >= 0 && __instance.cameraIndex < _cameraNames.Length)
        {
            __instance.textTitle.text = Language.main.Get(_cameraNames[__instance.cameraIndex]);
        }
        else
        {
            __instance.textTitle.text = string.Empty;
        }
    }
}
