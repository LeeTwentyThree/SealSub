namespace SealSubMod.Interfaces;

internal interface IAsyncPrefabSetupOperation
{
    IEnumerator SetupPrefabAsync(GameObject prefabRoot);
}