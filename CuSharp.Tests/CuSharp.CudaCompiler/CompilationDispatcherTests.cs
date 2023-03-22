using System.Collections.Generic;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Backend;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

[Collection("Sequential")]
public class CompilationDispatcherTests
{
    private readonly MethodInfoLoader _methodLoader = new();


    [Fact]
    public void TestMethodIsCached()
    {
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);

        dispatcher.Compile("TestFn", _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Contains(_methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod).GetHashCode(), cache.Keys);
    }

    [Fact]
    public void TestCacheIsUsedOnSecondCompile()
    {
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);

        dispatcher.Compile("TestFn", _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));
        dispatcher.Compile("TestFn", _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Single(cache);
    }
}