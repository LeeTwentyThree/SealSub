using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SpawnSealHUD : MonoBehaviour, IAsyncPrefabSetupOperation
{
    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        var model = CyclopsReferenceManager.CyclopsReference.transform.Find("HelmHUD").gameObject;
        var spawned = Instantiate(model, transform);
        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        var manager = spawned.GetComponent<CyclopsHelmHUDManager>();
        var root = transform.parent.gameObject;
        manager.subRoot = root.GetComponent<SubRoot>();
        manager.subLiveMixin = root.GetComponent<LiveMixin>();
        manager.LOD = root.GetComponent<BehaviourLOD>();
        manager.motorMode = root.GetComponent<CyclopsMotorMode>();
    }
}