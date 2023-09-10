using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;

[SealUpgradeModule("SealThermalChargeModule")]

internal class ThermalModule : BaseChargerModule<ThermalChargerFunction> { }

internal class ThermalChargerFunction : BaseChargerFunction
{
    private AnimationCurve thermalCharge;
    
    public override void Awake()
    {
        base.Awake();
        thermalCharge = GetComponentInParent<SubRoot>().thermalReactorCharge;//maybe just replace this with a hardcoded animation curve created in code? Sounds bit easier? Not sure
    }

    public override float GetCharge()
    {
        float temperature = GetTemperature();
        float amount = thermalCharge.Evaluate(temperature) * 1.5f * Time.deltaTime;
        return amount;
    }
    private float GetTemperature()
    {
        WaterTemperatureSimulation main = WaterTemperatureSimulation.main;
        if (!(main != null))
        {
            return 0f;
        }
        return main.GetTemperature(base.transform.position);
    }
}