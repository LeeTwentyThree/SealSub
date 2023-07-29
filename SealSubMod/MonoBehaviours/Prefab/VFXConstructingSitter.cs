using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

// Sitter
// (Class designed by Lilly)
internal class VFXConstructingSitter : MonoBehaviour, IAsyncPrefabSetupOperation
{
    public VFXConstructing vfxConstructing;

    private void OnValidate()
    {
        if (vfxConstructing == null)
            vfxConstructing = GetComponent<VFXConstructing>();
    }

    public IEnumerator SetupPrefabAsync()
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        var cyclops = CyclopsReferenceManager.CyclopsReference.gameObject.GetComponent<VFXConstructing>();
        vfxConstructing.ghostMaterial = cyclops.ghostMaterial;
        vfxConstructing.alphaTexture = cyclops.alphaTexture;
        vfxConstructing.alphaDetailTexture = cyclops.alphaDetailTexture;
        vfxConstructing.transparentShaders = cyclops.transparentShaders;
        vfxConstructing.surfaceSplashFX = cyclops.surfaceSplashFX;
    }
}