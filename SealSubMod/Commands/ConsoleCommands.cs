using Nautilus.Commands;
using UnityEngine;

namespace SealSubMod.Commands;//I thought it looked nicer with a folder :)

internal class ConsoleCommands
{
    [ConsoleCommand("Sealsub")]
    public static void SealSubCommand(string arg = "")
    {
        if (arg != null) ErrorMessage.AddMessage($"Hello there, {arg}!");//worthless. I just thought would be funny

        var obj = Object.Instantiate(Plugin.assets.LoadAsset<GameObject>("Seal Sub"));
        obj.SetActive(true);
        obj.transform.position = Player.main.transform.position + (Camera.main.transform.forward * 30);
    }
}
