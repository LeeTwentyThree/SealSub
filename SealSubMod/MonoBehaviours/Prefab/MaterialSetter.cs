namespace SealSubMod.MonoBehaviours.Prefab;

internal class MaterialSetter : MonoBehaviour
{
    public Renderer renderer;
    public int[] materialIndexes = new[] { 0 };
    public MaterialType materialType;

    private static Material glassMaterial;
    private static Material exteriorGlassMaterial;

    public enum MaterialType
    {
        WaterBarrier,
        ForceField,
        StasisField,
        Glass,
        ExteriorGlass
    }

    private void OnValidate()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();
    }

    public void AssignMaterial()
    {
        if (!renderer) throw new System.Exception($"Renderer is null on material setter {name}");

        var mats = renderer.materials; // just setting index doesn't work because you get a different array than the actual one. It's basically passed by value rather than reference
        foreach (var index in materialIndexes)
            mats[index] = GetMaterial(materialType);
        renderer.materials = mats;
    }

    public void Start() => AssignMaterial();//Here for now ig yea sure idk idc 

    public static Material GetMaterial(MaterialType type)
    {
        switch (type)
        {
            case MaterialType.WaterBarrier:
                return MaterialUtils.AirWaterBarrierMaterial;
            case MaterialType.ForceField:
                return MaterialUtils.ForceFieldMaterial;
            case MaterialType.StasisField:
                return MaterialUtils.StasisFieldMaterial;
            case MaterialType.Glass:
                return glassMaterial;
            case MaterialType.ExteriorGlass:
                return exteriorGlassMaterial;
            default:
                return null;
        }
    }

    public static IEnumerator LoadMaterialsAsync()
    {
        var seamothTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);

        yield return seamothTask;

        var material = seamothTask.GetResult()
            .transform.Find("Model/Submersible_SeaMoth/Submersible_seaMoth_geo/Submersible_SeaMoth_glass_geo")
            .GetComponent<Renderer>().material;

        glassMaterial = new Material(material);
        exteriorGlassMaterial = new Material(material);
        exteriorGlassMaterial.SetFloat("_SpecInt", 50);
    }
}
