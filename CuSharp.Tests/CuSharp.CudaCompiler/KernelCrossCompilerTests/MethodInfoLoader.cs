using System;
using System.Reflection;

namespace CuSharp.Tests.CuSharp.CudaCompiler.KernelCrossCompilerTests
{
    public class MethodInfoLoader
    {
        public MethodInfo GetScalarIntMethodInfo(Action<int, int> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetScalarFloatMethodInfo(Action<float, float> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayIntMethodInfo(Action<int[], int[], int[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayFloatMethodInfo(Action<float[], float[], float[]> fn)
        {
            return fn.Method;
        }
    }
}
