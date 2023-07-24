using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours;

internal class SealMaterialManager : MonoBehaviour, IOnAsyncPrefabTasksCompleted
{
    public void OnAsyncPrefabTasksCompleted()
    {
        MaterialUtils.ApplySNShaders(gameObject);

        foreach (var materialSetter in gameObject.GetComponentsInChildren<MaterialSetter>(true))
        {
            materialSetter.AssignMaterial();
        }
    }
}