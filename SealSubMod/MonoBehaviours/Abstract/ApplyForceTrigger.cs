using System;

namespace SealSubMod.MonoBehaviours.Abstract;

public abstract class ApplyForceTrigger : MonoBehaviour
{
    protected static List<Type> types = new List<Type>()
    {
        typeof(Creature),
        typeof(Vehicle),
        typeof(SubRoot),
    };

    public float pushVelocity = 2f;

    private Dictionary<Rigidbody, int> rigidBodyColliderCounts = new();

    protected virtual void FixedUpdate()
    {
        foreach (var pair in rigidBodyColliderCounts)
        {
            pair.Key.AddForce(GetPushDirection(pair.Key) * pushVelocity, ForceMode.VelocityChange);
        }
    }

    internal abstract Vector3 GetPushDirection(Rigidbody rigidbody);

    private void OnTriggerEnter(Collider other)
    {
        if (!Get(other))
            return;

        Incr(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Get(other))
            return;

        Decr(other.attachedRigidbody);
    }

    private Component Get(Collider other)
    {
        if (other.isTrigger)
            return null;

        foreach (var type in types)
        {
            var comp = other.GetComponentInParent(type);
            if (comp) return comp;
        }
        return null;
    }

    private void Incr(Rigidbody rb)
    {
        if (rigidBodyColliderCounts.TryGetValue(rb, out var val))
            rigidBodyColliderCounts[rb] = val + 1;
        else
            rigidBodyColliderCounts[rb] = 1;
    }

    private void Decr(Rigidbody rb)
    {
        if (rigidBodyColliderCounts.TryGetValue(rb, out var val))
        {
            if (val > 1)
                rigidBodyColliderCounts[rb] = val - 1;
            else
                rigidBodyColliderCounts.Remove(rb);
        }
        else
            rigidBodyColliderCounts[rb] = 1;
    }
}
