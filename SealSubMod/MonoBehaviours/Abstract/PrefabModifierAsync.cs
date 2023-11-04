namespace SealSubMod.MonoBehaviours.Abstract;

public abstract class PrefabModifierAsync : PrefabModifier
{
    public virtual IEnumerator SetupPrefabAsync() { yield break; }
}
