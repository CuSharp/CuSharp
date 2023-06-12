using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.CallKernels;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
// Other Output in Debug and Release Mode. Therefore, Marked with both Traits
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class CallLVVMTests
{
    private readonly TestValidator _validator = new();
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_CallIntMethod_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(CallIntMethod);
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
    public void ScalarInt_CallIntMethodNested_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(CallIntMethodNested);
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
    public void ArrayInt_CallIntReturnArrayWithKernelTools_LLVM(bool enableOptimizer)
    {
        // Arrange
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(CallIntReturnArrayWithKernelTools);
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