using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.Prefab;

// Class for assigning component fields at runtime
internal class SealSubRuntimeSetup : MonoBehaviour, IAsyncPrefabSetupOperation
{
    [SerializeField] float waterLevelYOffset = 3;

    // Serialized fields can be set in SetupPrefabAsync
    public IEnumerator SetupPrefabAsync()
    {
        GetComponent<PingInstance>().SetType(Plugin.SealPingType);
        yield break;
    }

    // Non-serialized fields must be set in Start
    private void Start()
    {
        GetComponent<WorldForces>().waterDepth = Ocean.GetOceanLevel() + waterLevelYOffset;
    }
}