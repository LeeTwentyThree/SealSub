using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class DamageManagerPrefabSetter : PrefabModifierAsync
{
    [SerializeField] CyclopsExternalDamageManager manager;

    public override IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        if(!CyclopsReferenceManager.CyclopsReference)
            yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        manager.fxPrefabs = CyclopsReferenceManager.CyclopsReference.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
