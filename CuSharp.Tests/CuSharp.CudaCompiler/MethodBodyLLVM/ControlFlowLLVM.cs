using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
// Other Output in Debug and Release Mode. Therefore, Marked with both Traits
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class ControlFlowLLVM
{
    private readonly TestValidator _validator = new();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarInt_Switch_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(Switch);
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
    public void ScalarInt_While_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(While);
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
    public void ScalarInt_DoWhile_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(DoWhile);
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
    public void ScalarInt_For_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(For);
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
    public void ScalarInt_WhileWithContinue_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(WhileWithContinue);
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
    public void ScalarInt_WhileWithBreak_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(WhileWithBreak);
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
    public void ScalarInt_Goto_LLVM(bool enableOptimizer)
    {
        // Arrange
        global::CuSharp.CuSharp.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(Goto);
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