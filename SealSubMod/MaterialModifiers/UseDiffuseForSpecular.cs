using Nautilus.Utility.MaterialModifiers;

namespace SealSubMod.MaterialModifiers;

internal class UseDiffuseForSpecular : MaterialModifier
{
    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {
        if (material.GetTexture(ShaderPropertyID._SpecTex) == null)
            material.SetTexture(ShaderPropertyID._SpecTex, material.GetTexture(ShaderPropertyID._MainTex));
    }
}