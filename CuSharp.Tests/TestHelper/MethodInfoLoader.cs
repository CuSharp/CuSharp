using System;
using System.Reflection;

namespace CuSharp.Tests.TestHelper
{
    public static class MethodInfoLoader
    {
        public static MethodInfo GetMethodInfo(Action fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Action<T> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Action<T, T> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Action<T, T, T> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Action<T, T, T, T> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Action<T, T, T, T, T> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T0, T1>(Action<T0, T1> fn)
        {
            return fn.Method;
        }

        public static MethodInfo GetMethodInfo<T0, T1, T2>(Action<T0, T0, T1, T2> fn)
        {
            return fn.Method;
        }
    }
}
