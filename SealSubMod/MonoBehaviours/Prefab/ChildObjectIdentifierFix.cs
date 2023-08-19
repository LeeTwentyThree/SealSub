using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

// If a child object identifier is not assigned a consistent Class ID, its contents will be reset with every session!!!

internal class ChildObjectIdentifierFix : PrefabModifier
{
    public string classId;
    public ChildObjectIdentifier childObjectIdentifier;

    public override void OnAsyncPrefabTasksCompleted()
    {
        childObjectIdentifier.ClassId = classId;
    }

    private void OnValidate()
    {
        if (childObjectIdentifier == null)
        {
            childObjectIdentifier = GetComponent<ChildObjectIdentifier>();
        }
    }
}
