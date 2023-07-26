namespace SealSubMod.MonoBehaviours;

internal class SubEnterTrigger : MonoBehaviour
{
    public bool setWalk;
    public SubRoot subRoot;
    public void OnTriggerEnter(Collider c)
    {
        var player = c.gameObject.GetComponentInParent<Player>();
        if (!player) return;

        if(!subRoot) // if subroot isn't set, assume this is debug mode and just use precursor out of water
        {
            Plugin.Logger.LogInfo($"Subroot not set when {(setWalk ? "entering" : "existing")} sub, using precursor walk instead");
            player.SetPrecursorOutOfWater(setWalk);
            return;
        }
        player.SetCurrentSub(setWalk ? subRoot : null);
    }
}
