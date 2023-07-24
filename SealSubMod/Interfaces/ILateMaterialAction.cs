using SealSubMod.MonoBehaviours.Prefab;

namespace SealSubMod.Interfaces;

/// <summary>
/// Called by the SealMaterialManager after all other async tasks and material setting has been completed 
/// </summary>
internal interface ILateMaterialAction
{
    void SetUpMaterials(SealMaterialManager sealMaterialManager);
}
