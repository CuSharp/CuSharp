using System.Reflection;

namespace CuSharp.CudaCompiler.Kernels;

public static class KernelHelpers
{
    public static string GetMethodIdentity(MethodInfo method)
    {
        string paramString = "_";
        foreach(var param in method.GetParameters())
        {
            paramString += param.ParameterType + ".";
        }
        return $"{method.DeclaringType.FullName}.{method.Name}{(paramString != "_" ? paramString : "")}";
    }
}