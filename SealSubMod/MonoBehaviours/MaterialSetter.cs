namespace SealSubMod.MonoBehaviours;

internal class MaterialSetter : MonoBehaviour
{
    public Renderer renderer;
    public int[] materialIndexes = new[] {0};
    public MaterialType materialType;

    public enum MaterialType
    {
        WaterBarrier,
        ForceField,
        StasisField,
    }

    public void AssignMaterial()
    {
        var mats = renderer.materials; // just setting index doesn't work because you get a different array than the actual one. It's basically passed by value rather than reference
        foreach (var index in materialIndexes)
            mats[index] = GetMaterial(materialType);
        renderer.materials = mats;
    }

    public static Material GetMaterial(MaterialType type)
    {
        switch(type)
        {
            case MaterialType.WaterBarrier:
                return MaterialUtils.AirWaterBarrierMaterial;
            case MaterialType.ForceField:
                return MaterialUtils.ForceFieldMaterial;
            case MaterialType.StasisField:
                return MaterialUtils.StasisFieldMaterial;
            default:
                return null;
        }
    }
}
