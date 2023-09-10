using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;

[SealUpgradeModule("SealSolarChargeModule")]
internal class SolarModule : BaseChargerModule<SolarChargerFunction> { }

internal class SolarChargerFunction : BaseChargerFunction
{
    public override float updateCooldown => 1;

    public override float GetCharge()
    {
        DayNightCycle main = DayNightCycle.main;
        if (main == null)
        {
            return 0;
        }

        float num = Mathf.Clamp01((200f + transform.position.y) / 200f);
        float localLightScalar = main.GetLocalLightScalar();
        float amount = 1f * localLightScalar * num;

        return amount;
    }
}