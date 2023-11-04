namespace SealSubMod.MonoBehaviours.UpgradeModules;

public abstract class BaseChargerFunction : MonoBehaviour
{
    public int modulesInstalled = 0;
    protected PowerRelay relay;

    /// <summary>
    /// If greater than 0, calls InvokeRepeating on Awake based on this delay. Otherwise, if 0 or less, UpdateCharge is called every frame on Update.
    /// </summary>
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
        if(modulesInstalled <= 0) return;

        relay.AddEnergy(GetCharge() * modulesInstalled, out _);
    }

    /// <summary>
    /// the fuck summary do I put here
    /// </summary>
    /// <returns>The current value to charge, which will then be multiplied by the number of installed modules and then added to the power relay</returns>
    public abstract float GetCharge();
}
