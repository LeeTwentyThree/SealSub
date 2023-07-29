namespace SealSubMod.Interfaces;

internal interface ILateMaterialOperation
{
    // Called after all other material operations and replacements have been completed
    void OnLateMaterialOperation();
}