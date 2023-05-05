using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Backend;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.Tests.TestHelper;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class CompilationDispatcherTests
{
    [Fact]
    public void TestMethodIsCached()
    {
        const string kernelName = "TestFn";
        var cache = new Dictionary<string, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);


        var ptxKernel = dispatcher.Compile(kernelName, GetMethodInfo<int[]>(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Contains(KernelHelpers.GetMethodIdentity(GetMethodInfo<int[]>(MethodsToCompile.EmptyIntArrayMethod)), cache.Keys);
        Assert.Equal(kernelName, ptxKernel.Name);
    }

    [Fact]
    public void TestCacheIsUsedOnSecondCompile()
    {
        const string kernelName = "TestFn";
        var cache = new Dictionary<string, PTXKernel>();
        var dispatcher = new CompilationDispatcher(cache);

        var ptxKernel1 = dispatcher.Compile(kernelName, GetMethodInfo<int[]>(MethodsToCompile.EmptyIntArrayMethod));
        var ptxKernel2 = dispatcher.Compile(kernelName, GetMethodInfo<int[]>(MethodsToCompile.EmptyIntArrayMethod));

        Assert.Single(cache);
        Assert.Equal(cache.First().Value, ptxKernel1);
        Assert.Equal(cache.First().Value, ptxKernel2);
        Assert.Equal(ptxKernel1, ptxKernel2);
    }
}