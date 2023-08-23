using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;

[SealUpgradeModule("AcidMushroom")]
internal class SolarModule : MonoBehaviour
{
    private EnergyMixin energyMixin;

    public void Awake()
    {
        energyMixin = GetComponentInParent<EnergyMixin>();
        ErrorMessage.AddMessage("Solar awake");
    }

    public void Update()
    {
        ErrorMessage.AddMessage("Solar update");
    }
}
