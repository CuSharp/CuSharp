using CuSharp.Tests.TestHelper;
using System.Reflection;
using CuSharp.CudaCompiler.Compiler;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.BitwiseOperatorKernels;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
// Other Output in Debug and Release Mode. Therefore, Marked with both Traits
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class BitwiseOperatorLLVMTests
{
    private readonly TestValidator _validator = new();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Int_BitwiseAnd_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, int[]>(BitwiseAndInt);
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
    public void Uint_BitwiseAnd_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint, uint, uint[]>(BitwiseAndUint);
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
    public void Bool_BitwiseAnd_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<bool, bool, bool[]>(BitwiseAndBool);
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
    public void Int_BitwiseOr_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, int[]>(BitwiseOrInt);
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
    public void Uint_BitwiseOr_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint, uint, uint[]>(BitwiseOrUint);
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
    public void Bool_BitwiseOr_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<bool, bool, bool[]>(BitwiseOrBool);
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
    public void Int_Not_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int[]>(NotInt);
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
    public void Uint_Not_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint, uint[]>(NotUint);
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
    public void Int_ShiftLeft_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, int[]>(ShiftLeftSigned);
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
    public void Uint_ShiftLeft_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint, int, uint[]>(ShiftLeftUnsigned);
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
    public void Int_ShiftRight_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int, int, int[]>(ShiftRightSigned);
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
    public void Uint_ShiftRight_LLVM(bool enableOptimizer)
    {
        Cu.EnableOptimizer = enableOptimizer;
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint, int, uint[]>(ShiftRightUnsigned);
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