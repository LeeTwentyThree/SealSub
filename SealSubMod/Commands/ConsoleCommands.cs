using Nautilus.Commands;

namespace SealSubMod.Commands; // I thought it looked nicer with a folder :)

internal class ConsoleCommands
{
    [ConsoleCommand("Sealsub")]
    public static void SealSubCommand(bool killLights = true, string arg = "", bool debugColliders = false)
    {
        ErrorMessage.AddMessage($"Hello there, {(arg != "" ? arg : "bitch")}!");//worthless. I just thought would be funny

        var obj = Object.Instantiate(Plugin.assets.LoadAsset<GameObject>("SealSubPrefab"));
        obj.SetActive(true);
        obj.transform.position = Player.main.transform.position + (Camera.main.transform.forward * 50);

        var shader = Shader.Find("MarmosetUBER");
        obj.GetComponentsInChildren<Renderer>(true).ForEach(rend => rend.materials.ForEach(mat => mat.shader = shader));

        if(debugColliders) obj.GetComponentsInChildren<Collider>(true).ForEach(candy => ErrorMessage.AddMessage($"Found candy {candy.name}"));

        if (killLights) obj.GetComponentsInChildren<Light>(true).ForEach(ligd => GameObject.Destroy(ligd));
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
}
