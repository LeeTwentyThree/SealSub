namespace SealSubMod.MonoBehaviours.Abstract;

internal abstract class PrefabModifierAsync : PrefabModifier
{
    public virtual IEnumerator SetupPrefabAsync() { yield break; }
}
