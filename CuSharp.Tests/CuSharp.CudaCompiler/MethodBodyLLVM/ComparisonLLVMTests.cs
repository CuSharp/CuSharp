﻿using System.Reflection;
using CuSharp.CudaCompiler.Compiler;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using CuSharp.Tests.TestHelper;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.ComparisonKernels;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
// Other Output in Debug and Release Mode. Therefore, Marked with both Traits
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class ComparisonLLVMTests
{
    private readonly TestValidator _validator = new();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_LessThan_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, bool[]>(LessThanInt);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_LessThanOrEquals_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LessThanOrEquals);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_GreaterThan_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, bool[]>(GreaterThanInt);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_GreaterThanOrEquals_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(GreaterThanOrEquals);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_EqualsTo_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, bool[]>(EqualsInt);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_NotEqualsTo_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, bool[]>(NotEqualsInt);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }
}
