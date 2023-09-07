using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class DamageManagerPrefabSetter : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] CyclopsExternalDamageManager manager;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        manager.fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
