using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;

[SealUpgradeModule("SealSpeedModule")]
public class SpeedModule : MonoBehaviour
{
    private SubControl control;
    public void Awake()
    {
        control = GetComponentInParent<SubControl>();
        control.BaseForwardAccel += 5;
    }

    public void OnDestroy()
    {
        control.BaseForwardAccel -= 5;
    }
}
