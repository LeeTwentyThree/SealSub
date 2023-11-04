namespace SealSubMod.MonoBehaviours;

public class WaterParkEnterTrigger : MonoBehaviour
{
    public bool setInside;
    public WaterPark waterPark;
    public void OnTriggerEnter(Collider c)
    {
        var player = UWE.Utils.GetComponentInHierarchy<Player>(c.gameObject);
        if (!player)
        {
            //For collisions that contain the player but aren't the player (vehicles)
            //you'd think it not necessary, but you'd apparently be wrong
            var root = UWE.Utils.GetEntityRoot(c.gameObject);
            if (!root)
                return;

            player = root.GetComponentInChildren<Player>();
        }

        if (!player) return;

        player.currentWaterPark = setInside ? waterPark : null;
    }
}
