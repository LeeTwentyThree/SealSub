using SealSubMod.Attributes;
using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.UpgradeModules;


internal abstract class DepthModule : MonoBehaviour, IOnModuleChange
{
    public abstract float Depth { get; }
    private CrushDamage damage;
    public void Awake()
    {
        ErrorMessage.AddMessage($"Depth module awake {Depth}");
        damage = GetComponentInParent<CrushDamage>();
    }

    public void OnChange(TechType techType, bool added)
    {
        var depth = Mathf.Max(Depth, damage.extraCrushDepth);
        damage.SetExtraCrushDepth(depth);
    }

    public void OnDisable()
    {
        ErrorMessage.AddMessage($"Depth module destroy {Depth}");
        GetComponentInParent<CrushDamage>().SetExtraCrushDepth(0);
    }
}

[SealUpgradeModule("SealHullModule1")]
internal class DepthModule1 : DepthModule { public override float Depth => 900; }
[SealUpgradeModule("SealHullModule2")]
internal class DepthModule2 : DepthModule { public override float Depth => 1300; }
[SealUpgradeModule("SealHullModule3")]
internal class DepthModule3 : DepthModule { public override float Depth => 1800; }
