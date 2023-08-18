using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class DamageManagerPrefabSetter : MonoBehaviour, IAsyncPrefabSetupOperation
{
    [SerializeField] CyclopsExternalDamageManager manager;

    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        manager.fxPrefabs = CyclopsReferenceManager.CyclopsReference.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
