using Nautilus.Utility.MaterialModifiers;

namespace SealSubMod.MaterialModifiers;

internal class BlockLightShaderConversion : MaterialModifier
{
    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {

    }
    public override bool BlockShaderConversion(Material material, Renderer renderer, MaterialUtils.MaterialType materialType) => renderer.name.Contains("Volumletric");//yea that typo is intentional
}
