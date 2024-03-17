using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using System;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class ModifyMaterialProperties : PrefabModifier
{
    public Renderer renderer;
    public int[] materialIndices = new[] { 0 };

    // These are for the editor only, they do NOT serialize and will be NULL!

    [SerializeField] ColorProperty[] colorProperties = new ColorProperty[0];
    [SerializeField] FloatProperty[] floatProperties = new FloatProperty[0];

    // The actual values should be accessed through these fields

    [SerializeField, HideInInspector] string[] colorPropertyNames;
    [SerializeField, HideInInspector] Color[] colorPropertyValues;

    [SerializeField, HideInInspector] string[] floatPropertyNames;
    [SerializeField, HideInInspector] float[] floatPropertyValues;

    // Editor-only operations to work around bugs

    private void OnValidate()
    {
        colorPropertyNames = new string[colorProperties.Length];
        colorPropertyValues = new Color[colorProperties.Length];

        for (int i = 0; i < colorProperties.Length; i++)
        {
            colorPropertyNames[i] = colorProperties[i].propertyName;
            colorPropertyValues[i] = colorProperties[i].color;
        }

        floatPropertyNames = new string[floatProperties.Length];
        floatPropertyValues = new float[floatProperties.Length];
        for (int i = 0; i < floatProperties.Length; i++)
        {
            floatPropertyNames[i] = floatProperties[i].propertyName;
            floatPropertyValues[i] = floatProperties[i].value;
        }

        if (renderer == null)
            renderer = GetComponent<Renderer>();
    }

    public override void OnLateMaterialOperation()
    {
        if (!renderer) throw new Exception($"Renderer is null on material setter {name}");

        var mats = renderer.materials; // just setting index doesn't work because you get a different array than the actual one. It's basically passed by value rather than reference)
        foreach (var index in materialIndices)
        {
            if(index >= mats.Length)
            {
                throw new Exception($"Material index too high on material property modifier {name}");
            }
            for (int i = 0; i < colorPropertyNames.Length; i++)
            {
                mats[index].SetColor(colorPropertyNames[i], colorPropertyValues[i]);
            }
            for (int i = 0; i < floatPropertyNames.Length; i++)
            {
                mats[index].SetFloat(floatPropertyNames[i], floatPropertyValues[i]);
            }
        }
        renderer.materials = mats;
    }

    [Serializable]
    public class ColorProperty
    {
        public string propertyName;
        [ColorUsage(true, true)]
        public Color color;
    }

    [Serializable]
    public class FloatProperty
    {
        public string propertyName;
        public float value;
    }
}