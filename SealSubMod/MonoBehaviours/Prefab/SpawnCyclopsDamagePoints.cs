using Nautilus.Extensions;
using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SpawnCyclopsDamagePoints : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] CyclopsExternalDamageManager damageManager;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsManager = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>();

        var points = new List<CyclopsDamagePoint>();

        foreach (var slot in gameObject.GetComponentsInChildren<DamagePointSlot>())
        {
            if (slot.damagePrefabIndex <= -1) slot.damagePrefabIndex = Random.Range(0, cyclopsManager.damagePoints.Length);

            var prefab = cyclopsManager.damagePoints[slot.damagePrefabIndex].gameObject;
            var copy = Instantiate(prefab, slot.transform.position, slot.transform.rotation, slot.transform);

            points.Add(copy.GetComponent<CyclopsDamagePoint>());
        }

        damageManager.damagePoints = points.ToArray();
    }
}
