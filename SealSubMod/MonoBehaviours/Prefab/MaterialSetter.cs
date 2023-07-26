namespace SealSubMod.MonoBehaviours.Prefab;

internal class MaterialSetter : MonoBehaviour
{
    public Renderer renderer;
    public int[] materialIndexes = new[] { 0 };
    public MaterialType materialType;

    private static Material glassMaterial;
    private static Material exteriorGlassMaterial;
    private static Material shinyGlassMaterial;

    public enum MaterialType
    {
        WaterBarrier,
        ForceField,
        StasisField,
        Glass,
        ExteriorGlass,
        ShinyGlass
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
            case MaterialType.ShinyGlass:
                return shinyGlassMaterial;
            default:
                return null;
        }
    }

    public static IEnumerator LoadMaterialsAsync()
    {
        var seamothTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);

        yield return seamothTask;

        var seamothGlassMaterial = seamothTask.GetResult()
            .transform.Find("Model/Submersible_SeaMoth/Submersible_seaMoth_geo/Submersible_SeaMoth_glass_geo")
            .GetComponent<Renderer>().material;

        glassMaterial = new Material(seamothGlassMaterial);

        exteriorGlassMaterial = new Material(seamothGlassMaterial);
        exteriorGlassMaterial.SetFloat("_SpecInt", 100);
        exteriorGlassMaterial.SetFloat("_Shininess", 8);
        exteriorGlassMaterial.SetFloat("_Fresnel", 0.72f);
        exteriorGlassMaterial.SetColor("_SpecColor", new Color(0.52f, 0.76f, 1f));

        shinyGlassMaterial = new Material(seamothGlassMaterial);
        shinyGlassMaterial.SetColor("_Color", new Color(1, 1, 1, 0.1f));
        shinyGlassMaterial.SetFloat("_SpecInt", 25);
        shinyGlassMaterial.SetFloat("_Shininess", 8);
        shinyGlassMaterial.SetFloat("_Fresnel", 0.88f);
    }
}