using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod.MonoBehaviours;

internal class BlockType : MonoBehaviour//test class, WIP
{
    [SerializeField]
    public Type type;

    private const float pushVelocity = 3f;

    private Dictionary<Rigidbody, int> rigidBodyColliderCounts;

    private void FixedUpdate()
    {
        foreach(var pair in rigidBodyColliderCounts)
        {
            pair.Key.AddForce(transform.forward * pushVelocity, ForceMode.VelocityChange);
        }
    }

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

        return other.GetComponentInParent(type);
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
