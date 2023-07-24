using Nautilus.Utility.MaterialModifiers;

namespace SealSubMod.MaterialModifiers;

internal class UseDiffuseForSpecular : MaterialModifier
{
    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {
        material.SetTexture("_SpecTex", material.GetTexture("_MainTex"));
    }
}