namespace SealSubMod.MonoBehaviours.UpgradeModules;

public abstract class BaseChargerModule<T> : MonoBehaviour where T : BaseChargerFunction
{
    protected T chargerFunction;
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
