using Nautilus.Extensions;
using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SpawnCyclopsDamagePoint : MonoBehaviour, IAsyncPrefabSetupOperation
{
    //idfk how to do paragraphs
    [Header("The specific child index of cyclops damage")]
    [Header("prefabs to use for this object")]
    [Header("use -1 to pick a random one")]
    [SerializeField] int damagePrefabIndex = -1;

    [SerializeField] CyclopsExternalDamageManager damageManager;

    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();

        var cyclopsManager = CyclopsReferenceManager.CyclopsReference.GetComponentInChildren<CyclopsExternalDamageManager>();

        if (damagePrefabIndex <= -1) damagePrefabIndex = Random.Range(0, cyclopsManager.damagePoints.Length);

        var prefab = cyclopsManager.damagePoints[damagePrefabIndex].gameObject;
        var copy = Instantiate(prefab, transform.position, transform.rotation, transform);

        var points = damageManager.damagePoints.ToList();
        points.Add(copy.GetComponent<CyclopsDamagePoint>());
        damageManager.damagePoints = points.ToArray();
    }
}
