using CuSharp.CudaCompiler.Compiler;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using CuSharp.Tests.TestHelper;
using Xunit;

using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class KernelCrossCompilerMetaInformationTests
{
    private readonly LLVMRepresentationLoader _llvmLoader = new();
    private readonly TestValidator _validator = new();

    [Fact]
    public void TestEmptyMethodCompiles()
    {
        const string kernelName = "EmptyMethodKernel";
        var method = GetMethodInfo(EmptyMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        Assert.Equal(_llvmLoader.GetEmptyMethodBodyRepresentation(kernelName),llvmKernel.KernelBuffer);
    }
    
    [Fact]
    public void TestEmptyMethodIsCorrectIR()
    {
        const string kernelName = "EmptyMethodKernel";
        var method = GetMethodInfo(EmptyMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);

        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var isCorrect = _validator.KernelIsCorrect(llvmKernel.KernelBuffer, config.KernelName);
        Assert.True(isCorrect);
    }

    [Fact]
    public void TestArrayParameterMethod()
    {
        const string kernelName = "ArrayParameterMethod";
        var method = GetMethodInfo<int[]>(EmptyTwoIntArrayMethod);
        var config = new CompilationConfiguration { KernelName = kernelName };
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel(kernelName, method));

        Assert.Equal(_llvmLoader.GetMinimalLLVMRepresentation(kernelName), llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestArrayParameterMethodIsCorrectIR()
    {
        const string kernelName = "ArrayParameterMethod";
        var method = GetMethodInfo<int[]>(EmptyTwoIntArrayMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel(kernelName, method));

        Assert.True(_validator.KernelIsCorrect(llvmKernel.KernelBuffer, llvmKernel.Name));
    }

    [Fact]
    public void TestMixedParameterMethod()
    {
        const string kernelName = "MixedParameterMethod";
        var method = GetMethodInfo<int[], bool, int>(EmptyMixedParameterMethod);
        var config = new CompilationConfiguration { KernelName = kernelName };
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel(kernelName, method));

        Assert.Equal(_llvmLoader.GetMinimalMixedParamLLVMRepresentation(kernelName), llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestMixedParameterMethodIsCorrectIR()
    {
        var method = GetMethodInfo<int[], bool, int>(EmptyMixedParameterMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "MixedParameterMethod";
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel("MixedParameterMethod", method));

        Assert.True(_validator.KernelIsCorrect(llvmKernel.KernelBuffer, llvmKernel.Name));
    }
}