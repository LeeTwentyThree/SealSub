namespace SealSubMod.MonoBehaviours.UpgradeModules;

internal abstract class BaseChargerFunction : MonoBehaviour
{
    internal int modulesInstalled = 0;
    protected PowerRelay relay;

    public virtual float updateCooldown => -1;

    public virtual void Awake()
    {
        relay = GetComponentInParent<PowerRelay>();

        if (updateCooldown > 0) //if the update cooldown is greater than zero repeat it every time the cooldown is over
            InvokeRepeating(nameof(UpdateCharge), 1, updateCooldown);
    }

    public virtual void Update()
    {
        if (updateCooldown <= 0) //if there's no update cooldown just update it every frame
            UpdateCharge();
    }

    public void UpdateCharge()
    {
        ErrorMessage.AddMessage($"{GetType()}: {modulesInstalled}");
        if(modulesInstalled <= 0) return;

        relay.AddEnergy(GetCharge() * modulesInstalled, out _);
    }

    /// <summary>
    /// the fuck summary do I put here
    /// </summary>
    /// <returns>The current value to charge, which will then be multiplied by the number of installed modules and then added to the power relay</returns>
    public abstract float GetCharge();
}
