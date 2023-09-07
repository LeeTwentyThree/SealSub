using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

// Class for assigning component fields at runtime
internal class SealSubRuntimeSetup : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] float waterLevelYOffset = 3;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsConstructing = cyclops.gameObject.GetComponent<VFXConstructing>();
        var vfxConstructing = GetComponent<VFXConstructing>();
        vfxConstructing.ghostMaterial = cyclopsConstructing.ghostMaterial;
        vfxConstructing.alphaTexture = cyclopsConstructing.alphaTexture;
        vfxConstructing.alphaDetailTexture = cyclopsConstructing.alphaDetailTexture;
        vfxConstructing.transparentShaders = cyclopsConstructing.transparentShaders;
        vfxConstructing.surfaceSplashFX = cyclopsConstructing.surfaceSplashFX;
        GetComponent<PingInstance>().SetType(Plugin.SealPingType);
    }

    // Non-serialized fields must be set in Start
    private void Start()
    {
    }
}