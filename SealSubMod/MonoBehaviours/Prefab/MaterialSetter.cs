using SealSubMod.Data;
using SealSubMod.Interfaces;
using UnityEngine.TextCore;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class MaterialSetter : MonoBehaviour, ILateMaterialAction
{
    public Renderer renderer;
    public int[] materialIndexes = new[] { 0 };
    public SealMaterialManager.MaterialType materialType;

    private void OnValidate()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();
    }

    public void SetUpMaterials(SealMaterialManager sealMaterialManager)
    {
        var mats = renderer.materials; // just setting index doesn't work because you get a different array than the actual one. It's basically passed by value rather than reference
        foreach (var index in materialIndexes)
            mats[index] = sealMaterialManager.GetMaterial(materialType);
        renderer.materials = mats;
    }
}
