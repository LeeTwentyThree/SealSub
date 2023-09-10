namespace SealSubMod.MonoBehaviours.UpgradeModules;

internal abstract class BaseChargerModule<T> : MonoBehaviour where T : BaseChargerFunction
{
    private T chargerFunction;
    public virtual void Awake()
    {
        chargerFunction = gameObject.EnsureComponent<T>();
        chargerFunction.modulesInstalled++;
    }

    public virtual void OnDestroy()
    {
        chargerFunction.modulesInstalled--;
    }
}
