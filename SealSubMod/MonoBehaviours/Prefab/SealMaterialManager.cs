using SealSubMod.Interfaces;
using SealSubMod.MaterialModifiers;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealMaterialManager : MonoBehaviour, IOnAsyncPrefabTasksCompleted
{
    public void OnAsyncPrefabTasksCompleted()
    {
        MaterialUtils.ApplySNShaders(gameObject, modifiers: new UseDiffuseForSpecular());

        foreach (var materialSetter in gameObject.GetComponentsInChildren<MaterialSetter>(true))
        {
            materialSetter.AssignMaterial();
        }
    }
}