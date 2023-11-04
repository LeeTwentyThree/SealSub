using System;

namespace SealSubMod.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SealUpgradeModuleAttribute : Attribute
{
    readonly string moduleTechType;


    public SealUpgradeModuleAttribute(string ModuleTechType)
    {
        this.moduleTechType = ModuleTechType;
    }

    public string ModuleTechType
    {
        get { return moduleTechType; }
    }
}