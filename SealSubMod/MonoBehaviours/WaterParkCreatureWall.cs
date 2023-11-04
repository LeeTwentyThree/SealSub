using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours;

public class WaterParkCreatureWall : ApplyForceTrigger
{
    public Transform center;
    internal override Vector3 GetPushDirection(Rigidbody rigidbody)
    {
        return (center.position - rigidbody.transform.position).normalized;
    }
}
