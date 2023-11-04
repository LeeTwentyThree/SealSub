using SealSubMod.Attributes;
using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.UpgradeModules;


public abstract class DepthModule : MonoBehaviour, IOnModuleChange
{
    public abstract float Depth { get; }
    protected CrushDamage Damage 
    { 
        get 
        { 
            if(!_damage) _damage = gameObject.GetComponentInParent<CrushDamage>(true);
            return _damage;
        } 
    }

    private CrushDamage _damage;

    public void OnChange(TechType techType, bool added)
    {
        var depth = Mathf.Max(Depth, Damage.extraCrushDepth);
        Damage.SetExtraCrushDepth(depth);
    }

    public void OnDisable()
    {
        GetComponentInParent<CrushDamage>().SetExtraCrushDepth(0);
    }
}

[SealUpgradeModule("SealHullModule1")]
internal class DepthModule1 : DepthModule { public override float Depth => 900; }
[SealUpgradeModule("SealHullModule2")]
internal class DepthModule2 : DepthModule { public override float Depth => 1300; }
[SealUpgradeModule("SealHullModule3")]
internal class DepthModule3 : DepthModule { public override float Depth => 1800; }
