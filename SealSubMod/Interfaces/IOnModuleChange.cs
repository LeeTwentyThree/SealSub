namespace SealSubMod.Interfaces;

internal interface IOnModuleChange
{
    /// <summary>
    /// Called when any module is added or removed. Called after it is added, and before it is destroyed
    /// </summary>
    /// <param name="techType">The techtype of the module that was added/removed</param>
    /// <param name="">Whether the module was added or if it was removed</param>
    public void OnChange(TechType techType, bool added);
}
