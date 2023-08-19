using HarmonyLib;
using SealSubMod.Interfaces;
using SealSubMod.MaterialModifiers;
using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealMaterialManager : PrefabModifier
{
    public override void OnAsyncPrefabTasksCompleted()
    {
        MaterialUtils.ApplySNShaders(gameObject, 6.5f, 1f, 1, modifiers: new UseDiffuseForSpecular());

        foreach (var materialSetter in gameObject.GetComponentsInChildren<MaterialSetter>(true))
        {
            materialSetter.AssignMaterials();
        }
    }
}