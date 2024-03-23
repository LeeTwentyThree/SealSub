using Nautilus.Utility.MaterialModifiers;

namespace SealSubMod.MaterialModifiers;

internal class BlockShaderConversions : MaterialModifier
{
    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {

    }
    public override bool BlockShaderConversion(Material material, Renderer renderer, MaterialUtils.MaterialType materialType)
    {
        //yea that typo is intentional
        return renderer.name.Contains("Volumletric") || renderer.name.Contains("x_BaseWater");
    }
}
