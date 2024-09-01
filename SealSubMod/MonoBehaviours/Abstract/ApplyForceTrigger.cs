using System;

namespace SealSubMod.MonoBehaviours.Abstract;

public abstract class ApplyForceTrigger : MonoBehaviour
{
    public static float maxDistanceBeforeSafetyCheck = 20;
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
        var safetyChecked = new List<Rigidbody>();
        foreach (var pair in rigidBodyColliderCounts)
        {
            //Safety check, unfortunately kinda needed because unity's OnTriggerExit can't be trusted to activate
            if((pair.Key.transform.position - transform.position).sqrMagnitude >= (maxDistanceBeforeSafetyCheck * maxDistanceBeforeSafetyCheck))
            {
                safetyChecked.Add(pair.Key);
                return;
            }

            pair.Key.AddForce(GetPushDirection(pair.Key) * pushVelocity, ForceMode.VelocityChange);
        }

        safetyChecked.ForEach(@checked => rigidBodyColliderCounts.Remove(@checked));
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
