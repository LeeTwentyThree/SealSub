namespace SealSubMod.MonoBehaviours;

internal class SubEnterTrigger : MonoBehaviour
{
    public bool setWalk;
    public SubRoot subRoot;
    public void OnTriggerEnter(Collider c)
    {
        var player = UWE.Utils.GetComponentInHierarchy<Player>(c.gameObject);
        if(!player)
        {
            //For collisions that contain the player but aren't the player (vehicles)
            //you'd think it not necessary, but you'd apparently be wrong
            var root = UWE.Utils.GetEntityRoot(c.gameObject);
            if (!root)
                return;

            player = root.GetComponentInChildren<Player>();
        }
        
        if (!player) return;

        if(!subRoot) // if subroot isn't set, assume this is debug mode and just use precursor out of water
        {
            Plugin.Logger.LogInfo($"Subroot not set when {(setWalk ? "entering" : "existing")} sub, using precursor walk instead");
            player.SetPrecursorOutOfWater(setWalk);
            return;
        }

        player.SetCurrentSub(setWalk ? subRoot : null, true);
    }
}
