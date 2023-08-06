using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

// Class for assigning component fields at runtime
internal class SealSubRuntimeSetup : MonoBehaviour, IAsyncPrefabSetupOperation
{
    [SerializeField] float waterLevelYOffset = 3;

    // Serialized fields can be set in SetupPrefabAsync
    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        var cyclops = CyclopsReferenceManager.CyclopsReference.gameObject.GetComponent<VFXConstructing>();
        var vfxConstructing = GetComponent<VFXConstructing>();
        vfxConstructing.ghostMaterial = cyclops.ghostMaterial;
        vfxConstructing.alphaTexture = cyclops.alphaTexture;
        vfxConstructing.alphaDetailTexture = cyclops.alphaDetailTexture;
        vfxConstructing.transparentShaders = cyclops.transparentShaders;
        vfxConstructing.surfaceSplashFX = cyclops.surfaceSplashFX;
        GetComponent<PingInstance>().SetType(Plugin.SealPingType);
    }

    // Non-serialized fields must be set in Start
    private void Start()
    {
    }
}