using Nautilus.Commands;
using SealSubMod.MonoBehaviours;
using SealSubMod.Utility;

namespace SealSubMod.Commands; // I thought it looked nicer with a folder :)

internal class ConsoleCommands
{
    [ConsoleCommand("Sealsub")]
    public static void SealSubCommand(bool setInside = false, string arg = "")
    {
        ErrorMessage.AddMessage($"Hello there, {(arg != "" ? arg : "bitch")}!");//worthless. I just thought would be funny

        UWE.CoroutineHost.StartCoroutine(SpawnSeal(setInside));
    }

    private static IEnumerator SpawnSeal(bool setInside, Vector3? pos = null)
    {
        if(pos == null) pos = Player.main.transform.position + (Camera.main.transform.forward * 50);

        var task = CraftData.GetPrefabForTechTypeAsync(Prefabs.SealSubPrefab.SealType);
        yield return task;
        var obj = GameObject.Instantiate(task.GetResult());

        obj.transform.position = (Vector3)pos;
        if(setInside)
            Player.main.currentSub = obj.GetComponent<SubRoot>();

        CrafterLogic.NotifyCraftEnd(obj, Prefabs.SealSubPrefab.SealType);
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
    public static void BitchCommand(bool upgrades = true)
    {
        WarpForwardShortcut(1000);
        SealSubCommand(true);
        WarpForwardShortcut(50);
        if(upgrades) SealUpgradesCommand();
    }

    [ConsoleCommand("GayShit")]
    public static void OnGayCommand() => Gaytilities.GayModeActive = true;

    [ConsoleCommand("SealUpgrades")]
    public static void SealUpgradesCommand()
    {
        CraftData.GetEquipmentType(Plugin.DepthModuleMk1Info.TechType);//just a dummy call to kick nautilus into gear and make it patch the dictionary as it should
        foreach (var pair in CraftData.equipmentTypes)
        {
            if (pair.Value == Plugin.SealModuleEquipmentType)
            {
                CraftData.AddToInventory(pair.Key);
            }
        }
    }
    [ConsoleCommand("ToggleAlarms")]
    public static void ToggleAlarmsCommand(float delay) => UWE.CoroutineHost.StartCoroutine(ToggleAlarmsCommandDelayed(delay));
    public static IEnumerator ToggleAlarmsCommandDelayed(float delay, bool? enabled = null)
    {
        yield return new WaitForSeconds(delay);

        foreach (var seal in GameObject.FindObjectsOfType<SealSubRoot>())
        {
            var alarms = seal.GetComponentInChildren<AlarmManager>(true);
            if (enabled != null)
                alarms.SetAlarmsEnabled((bool)enabled);
            else
                alarms.ToggleAlarms();

            seal.subLightsOn = enabled != null ? (bool)enabled : !seal.subLightsOn;
        }
    }
    [ConsoleCommand("UnstuckSeal")]
    public static string UnstuckCommand()
    {
        var sub = Player.main.currentSub;
        if (sub is not SealSubRoot) return "Must be in a seal to use this command";

        var pos = new Vector3(400, -20, 400);

        sub.transform.position = pos;
        Player.main.SetPosition(pos);

        return "You've been unstuck! Please mention this to the devs, so we can learn the most prevalent location issues with the sub";
    }
}
