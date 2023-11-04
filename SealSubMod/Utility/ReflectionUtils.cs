using HarmonyLib;
using System.Reflection;
using System;

namespace SealSubMod.Utility;

public static class ReflectionUtils
{
    public static Assembly FindAssembly(string assemblyName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            if (assembly.FullName.StartsWith(assemblyName + ","))
                return assembly;

        return null;
    }
    public static MethodInfo FindMethod(string assemblyName, string typeName, string methodName)
    {
        var assembly = FindAssembly(assemblyName);
        if (assembly == null)
        {
            return null;
        }

        Type targetType = assembly.GetType(typeName);
        if (targetType == null)
        {
            return null;
        }

        var targetMethod = AccessTools.Method(targetType, methodName);
        if (targetMethod == null)
        {

        }
        return targetMethod;
    }
    public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
    {
        var assembly = FindAssembly(assemblyName);
        if (assembly == null)
        {
            return;
        }

        Type targetType = assembly.GetType(typeName);
        if (targetType == null)
        {
            return;
        }

        var targetMethod = AccessTools.Method(targetType, methodName);
        if (targetMethod != null)
        {
            harmony.Patch(targetMethod, prefix, postfix, transpiler);
        }
    }
}
