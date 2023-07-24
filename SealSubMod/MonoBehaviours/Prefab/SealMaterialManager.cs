using SealSubMod.Interfaces;
using SealSubMod.MaterialModifiers;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealMaterialManager : MonoBehaviour, IAsyncPrefabSetupOperation, IOnAsyncPrefabTasksCompleted
{
    private Dictionary<MaterialType, Material> _materials;

    IEnumerator IAsyncPrefabSetupOperation.SetupPrefabAsync()
    {
        _materials = new Dictionary<MaterialType, Material>()
        {
            { MaterialType.WaterBarrier, MaterialUtils.AirWaterBarrierMaterial },
            { MaterialType.ForceField, MaterialUtils.ForceFieldMaterial },
            { MaterialType.StasisField, MaterialUtils.StasisFieldMaterial }
        };

        var seamothTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);

        yield return seamothTask;

        var seamothGlassMaterial = seamothTask.GetResult()
            .transform.Find("Model/Submersible_SeaMoth/Submersible_seaMoth_geo/Submersible_SeaMoth_glass_geo")
            .GetComponent<Renderer>().material;

        _materials.Add(MaterialType.Glass, seamothGlassMaterial);
    }

    void IOnAsyncPrefabTasksCompleted.OnAsyncPrefabTasksCompleted()
    {
        MaterialUtils.ApplySNShaders(gameObject, modifiers: new UseDiffuseForSpecular());

        foreach (var lateAction in gameObject.GetComponentsInChildren<ILateMaterialAction>(true))
        {
            lateAction.SetUpMaterials(this);
        }
    }

    public Material GetMaterial(MaterialType type)
    {
        if (_materials == null)
        {
            Debug.LogError("Trying to access materials before they have been initialized.");
            return null;
        }
        return _materials[type];
    }

    public enum MaterialType
    {
        WaterBarrier,
        ForceField,
        StasisField,
        Glass
    }
}