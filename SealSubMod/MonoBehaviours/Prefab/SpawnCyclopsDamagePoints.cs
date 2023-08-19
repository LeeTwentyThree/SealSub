using Nautilus.Extensions;
using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SpawnCyclopsDamagePoints : PrefabModifierAsync
{
    private static CyclopsExternalDamageManager cyclopsManager;

    [SerializeField] CyclopsExternalDamageManager damageManager;

    public override IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        if (!cyclopsManager) yield return LoadCyclopsManager();

        var points = new List<CyclopsDamagePoint>();

        foreach (var slot in gameObject.GetComponentsInChildren<DamagePointSlot>())
        {
            if (slot.damagePrefabIndex <= -1) slot.damagePrefabIndex = Random.Range(0, cyclopsManager.damagePoints.Length);

            var prefab = cyclopsManager.damagePoints[slot.damagePrefabIndex].gameObject;
            var copy = Instantiate(prefab, transform.position, transform.rotation, transform);

            points.Add(copy.GetComponent<CyclopsDamagePoint>());
        }

        damageManager.damagePoints = points.ToArray();
    }

    public static IEnumerator LoadCyclopsManager()
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();

        cyclopsManager = CyclopsReferenceManager.CyclopsReference.GetComponentInChildren<CyclopsExternalDamageManager>();
    }
}
