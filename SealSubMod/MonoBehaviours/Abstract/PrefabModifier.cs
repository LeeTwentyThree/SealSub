namespace SealSubMod.MonoBehaviours.Abstract;

public abstract class PrefabModifier : MonoBehaviour
{
    public virtual void OnAsyncPrefabTasksCompleted() { }
    public virtual void OnLateMaterialOperation() { }
}
