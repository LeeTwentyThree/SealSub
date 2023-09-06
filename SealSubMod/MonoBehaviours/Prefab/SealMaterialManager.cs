using HarmonyLib;
using Nautilus.Utility.MaterialModifiers;
using SealSubMod.Interfaces;
using SealSubMod.MaterialModifiers;
using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealMaterialManager : PrefabModifier
{
    public override void OnAsyncPrefabTasksCompleted()
    {
        MaterialUtils.ApplySNShaders(gameObject, 6.5f, 1f, 1, modifiers: new MaterialModifier[] { new UseDiffuseForSpecular(), new BlockLightShaderConversion() });

        foreach (var materialSetter in gameObject.GetComponentsInChildren<MaterialSetter>(true))
        {
            materialSetter.AssignMaterials();
        }
    }
}