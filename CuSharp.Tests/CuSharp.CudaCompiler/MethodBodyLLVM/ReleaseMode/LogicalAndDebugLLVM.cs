using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM.ReleaseMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
public class LogicalAndDebugLLVM
{
    private readonly LLVMRepresentationLoader _llvmLoader = new();
    private readonly TestValidator _validator = new();

    [Fact]
    public void ScalarInt_LogicalAnd_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LogicalAnd);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = string.Empty; // TODO: Load expected output
        var actual = llvmKernel.KernelBuffer;

        //Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarInt_LogicalOr_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LogicalOr);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = string.Empty; // TODO: Load expected output
        var actual = llvmKernel.KernelBuffer;

        //Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }
}