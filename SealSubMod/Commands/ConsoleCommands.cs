﻿using Nautilus.Commands;

namespace SealSubMod.Commands; // I thought it looked nicer with a folder :)

internal class ConsoleCommands
{
    [ConsoleCommand("Sealsub")]
    public static void SealSubCommand(string arg = "")
    {
        ErrorMessage.AddMessage($"Hello there, {(arg != "" ? arg : "bitch")}!");//worthless. I just thought would be funny

        UWE.CoroutineHost.StartCoroutine(SpawnSeal());
    }

    private static IEnumerator SpawnSeal(Vector3? pos = null)
    {
        if(pos == null) pos = Player.main.transform.position + (Camera.main.transform.forward * 50);

        var task = CraftData.GetPrefabForTechTypeAsync(Prefabs.SealSubPrefab.SealType);
        yield return task;
        var obj = GameObject.Instantiate(task.GetResult());

        obj.transform.position = (Vector3)pos;
    }

    [ConsoleCommand("SetWalking")]
    public static void SetWalking(bool walking = true)
    {
        if (!Player.main)
        {
            ErrorMessage.AddMessage("Player.main is null when trying to set walking!!!!!");
            ErrorMessage.AddMessage("(don't use command when not in game)");
            ErrorMessage.AddMessage("((if you are in game this is a very confusing situation))");
            return;
        }
        Player.main.precursorOutOfWater = walking;
    }

    [ConsoleCommand("W")]
    public static void WarpForwardShortcut(float distance, bool setWalk = false)
    {
        Transform aimingTransform = Player.main.camRoot.GetAimingTransform();
        Player.main.SetPosition(Player.main.transform.position + aimingTransform.forward * distance);
        Player.main.OnPlayerPositionCheat();
        Player.main.precursorOutOfWater = setWalk;
    }

    [ConsoleCommand("Bitch")]
    public static void BitchCommand(bool lights = false)
    {
        WarpForwardShortcut(1000);
        SealSubCommand();
        WarpForwardShortcut(50, true);
    }
}
