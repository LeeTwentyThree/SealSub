using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.Prefab;

// If a child object identifier is not assigned a consistent Class ID, its contents will be reset with every session!!!

internal class ChildObjectIdentifierFix : MonoBehaviour, IAsyncPrefabSetupOperation
{
    public string classId;
    public ChildObjectIdentifier childObjectIdentifier;

    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        childObjectIdentifier.ClassId = classId;
        yield break;
    }

    private void OnValidate()
    {
        if (childObjectIdentifier == null)
        {
            childObjectIdentifier = GetComponent<ChildObjectIdentifier>();
        }
    }
}
