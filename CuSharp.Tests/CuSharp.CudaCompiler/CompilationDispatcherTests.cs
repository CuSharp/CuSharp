using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Backend;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

public class CompilationDispatcherTests
{
    void TestFn(int[] x)
    {}

    private MethodInfo GetTestMethodInfo(Action<int[]> action)
    {
        return action.GetMethodInfo();
    }

    [Fact]
    public void TestFnIsCached()
    {
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);
        dispatcher.Compile("TestFn", GetTestMethodInfo(TestFn));
        Assert.Contains(GetTestMethodInfo(TestFn).GetHashCode(), cache.Keys);
    }

    [Fact]
    public void TestCacheIsUsedOnSecondCompile()
    {
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);
        dispatcher.Compile("TestFn", GetTestMethodInfo(TestFn));
        dispatcher.Compile("TestFn", GetTestMethodInfo(TestFn));
        Assert.Single(cache);
    }

    
}