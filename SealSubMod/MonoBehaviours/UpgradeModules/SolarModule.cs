using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;

[SealUpgradeModule("SealSolarChargeModule")]
internal class SolarModule : MonoBehaviour
{
    private SolarChargerFunction chargerFunction;
    public void Awake()
    {
        chargerFunction = gameObject.EnsureComponent<SolarChargerFunction>();
        chargerFunction.modulesInstalled++;
    }
    public void OnDestroy()
    {
        chargerFunction.modulesInstalled--;
    }
}

internal class SolarChargerFunction : MonoBehaviour
{
    internal int modulesInstalled = 0;
    private PowerRelay relay;

    public void Awake()
    {
        relay = GetComponentInParent<PowerRelay>();
        InvokeRepeating("UpdateRecharge", 1f, 1f);
    }
    public void UpdateRecharge()
    {
        if(modulesInstalled <= 0) return;
        ErrorMessage.AddMessage($"Charger update modules: {modulesInstalled}");

        DayNightCycle main = DayNightCycle.main;
        if (main == null)
        {
            return;
        }
        float num = Mathf.Clamp01((200f + transform.position.y) / 200f);
        float localLightScalar = main.GetLocalLightScalar();
        float amount = 1f * localLightScalar * num * modulesInstalled;
        relay.AddEnergy(amount, out _);
    }
}