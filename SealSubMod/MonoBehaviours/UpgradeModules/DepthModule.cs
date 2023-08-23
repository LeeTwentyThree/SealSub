using SealSubMod.Attributes;

namespace SealSubMod.MonoBehaviours.UpgradeModules;


internal abstract class DepthModule : MonoBehaviour
{
    public abstract float Depth { get; }

    public void Awake()
    {
        ErrorMessage.AddMessage($"Depth module awake {Depth}");
        GetComponentInParent<CrushDamage>().SetExtraCrushDepth(Depth);
        GetComponentInParent<SealSubRoot>().AddIsAllowedToAddListener(IsAllowed);
    }

    public bool IsAllowed(Pickupable pickup, bool verbose) => !SealSubRoot.moduleFunctions.TryGetValue(pickup.GetTechType(), out var type) || !type.IsSubclassOf(typeof(DepthModule));

    public void OnDestroy()
    {
        ErrorMessage.AddMessage($"Depth module destroy {Depth}");
        GetComponentInParent<CrushDamage>().SetExtraCrushDepth(0);
        GetComponentInParent<SealSubRoot>().RemoveIsAllowedToAddListener(IsAllowed);
    }
}

[SealUpgradeModule("SealHullModule1")]
internal class DepthModule1 : DepthModule { public override float Depth => 900; }
[SealUpgradeModule("SealHullModule2")]
internal class DepthModule2 : DepthModule { public override float Depth => 1300; }
[SealUpgradeModule("SealHullModule3")]
internal class DepthModule3 : DepthModule { public override float Depth => 1800; }
