using System.Collections.Generic;
using System.Linq;
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
        const string kernelName = "TestFn";
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);


        var ptxKernel = dispatcher.Compile(kernelName, _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Contains(_methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod).GetHashCode(), cache.Keys);
        Assert.Equal(kernelName, ptxKernel.Name);
    }

    [Fact]
    public void TestCacheIsUsedOnSecondCompile()
    {
        const string kernelName = "TestFn";
        var cache = new Dictionary<int, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);

        var ptxKernel1 = dispatcher.Compile(kernelName, _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));
        var ptxKernel2 = dispatcher.Compile(kernelName, _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Single(cache);
        Assert.Equal(cache.First().Value, ptxKernel1);
        Assert.Equal(cache.First().Value, ptxKernel2);
        Assert.Equal(ptxKernel1, ptxKernel2);
    }
}