using System;
using System.Reflection;

namespace CuSharp.Tests.TestHelper
{
    public class MethodInfoLoader
    {
        public MethodInfo GetMethodInfo(Action fn)
        {
            return fn.Method;
        }

        public MethodInfo GetMixedMethodInfo(Action<int[], int[], bool, int> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetScalarIntMethodInfo(Action<int, int> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetScalarLongMethodInfo(Action<long, long> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetScalarFloatMethodInfo(Action<float, float> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetScalarDoubleMethodInfo(Action<double, double> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayIntMethodInfo(Action<int[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayIntMethodInfo(Action<int[], int[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayIntMethodInfo(Action<int[], int[], int[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayLongMethodInfo(Action<long[], long[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayFloatMethodInfo(Action<float[], float[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayFloatMethodInfo(Action<float[], float[], float[]> fn)
        {
            return fn.Method;
        }

        public MethodInfo GetArrayDoubleMethodInfo(Action<double[], double[]> fn)
        {
            return fn.Method;
        }
    }
}
