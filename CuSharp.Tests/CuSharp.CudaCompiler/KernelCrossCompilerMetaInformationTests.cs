using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

public class KernelCrossCompilerMetaInformationTests
{
    private readonly MethodInfoLoader _methodLoader = new();
    private readonly LLVMRepresentationLoader _llvmLoader = new();
    private readonly TestValidator _validator = new();

    [Fact]
    public void TestEmptyMethodCompiles()
    {
        const string kernelName = "EmptyMethodKernel";
        var method = _methodLoader.GetMethodInfo(MethodsToCompile.EmptyMethod);
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
        var method = _methodLoader.GetMethodInfo(MethodsToCompile.EmptyMethod);
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
        var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyTwoIntArrayMethod);
        var config = new CompilationConfiguration { KernelName = kernelName };
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel(kernelName, method));

        Assert.Equal(_llvmLoader.GetMinimalLLVMRepresentation(kernelName), llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestArrayParameterMethodIsCorrectIR()
    {
        const string kernelName = "ArrayParameterMethod";
        var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.EmptyTwoIntArrayMethod);
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
        var method = _methodLoader.GetMixedMethodInfo(MethodsToCompile.EmptyMixedParameterMethod);
        var config = new CompilationConfiguration { KernelName = kernelName };
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel(kernelName, method));

        Assert.Equal(_llvmLoader.GetMinimalMixedParamLLVMRepresentation(kernelName), llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestMixedParameterMethodIsCorrectIR()
    {
        var method = _methodLoader.GetMixedMethodInfo(MethodsToCompile.EmptyMixedParameterMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "MixedParameterMethod";
        var compiler = new KernelCrossCompiler(config);

        var llvmKernel = compiler.Compile(new MSILKernel("MixedParameterMethod", method));

        Assert.True(_validator.KernelIsCorrect(llvmKernel.KernelBuffer, llvmKernel.Name));
    }
}