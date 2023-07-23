using Nautilus.Utility;
using UnityEngine;

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

    public void Awake()
    {
        foreach (var index in materialIndexes)
            renderer.materials[index] = GetMaterial(materialType);
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
