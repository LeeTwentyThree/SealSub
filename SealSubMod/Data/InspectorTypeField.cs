using System;
using System.Reflection;

namespace SealSubMod.Data;

[Serializable]
public class InspectorTypeField
{
    [Tooltip("Name of the Assembly. Default value is Assembly-CSharp.")]
    [SerializeField] string assemblyName = "Assembly-CSharp";

    [Tooltip("Name of the Type. Case-sensitive.")]
    [SerializeField] string typeName;

    private Type _cachedType;

    private static Dictionary<string, Assembly> _cachedAssemblies = new Dictionary<string, Assembly>();

    public Type AssignedType
    {
        get
        {
            if (_cachedType == null)
            {
                _cachedType = AssignedAssembly.GetType(typeName);
                if (_cachedType == null)
                {
                    Debug.LogError($"Type not found by name '{typeName}'!");
                }
            }
            return _cachedType;
        }
    }

    public Assembly AssignedAssembly
    {
        get
        {
            if (_cachedAssemblies.TryGetValue(assemblyName, out var assembly))
            {
                return assembly;
            }
            assembly = AppDomain.CurrentDomain.GetAssemblies().First((a) => a.GetName().Name == assemblyName);
            _cachedAssemblies.Add(assemblyName, assembly);
            return assembly;
        }
    }
}