using System.Reflection;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM.DebugMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
public class ComparisonDebugLLVM
{
    private readonly TestValidator _validator = new();

    [Fact]
    public void ScalarInt_LessThan_LLVM()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LessThan);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarInt_LessThanOrEquals_LLVM()
    {
        // Arrange
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

    [Fact]
    public void ScalarInt_GreaterThan_LLVM()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(GreaterThan);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarInt_GreaterThanOrEquals_LLVM()
    {
        // Arrange
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

    [Fact]
    public void ScalarInt_EqualsTo_LLVM()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(EqualsTo);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        // Act
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
        var actual = llvmKernel.KernelBuffer;

        // Assert
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarInt_NotEqualsTo_LLVM()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(NotEqualsTo);
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
