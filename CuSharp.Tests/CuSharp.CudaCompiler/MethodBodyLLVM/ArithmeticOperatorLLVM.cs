using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class ArithmeticOperatorLLVM
{
    private readonly LLVMRepresentationLoader _llvmLoader = new();
    private readonly TestValidator _validator = new();

    #region Addition

    [Fact]
    public void ScalarInt_AdditionWithConst_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntAdditionWithConst);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntAdditionWithEachConstTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarLong_AdditionWithConst_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<long>(ScalarLongAdditionWithConst);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarLongAdditionWithConstTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarInt_Addition_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntAddition);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntAdditionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_AdditionWithConst_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatAdditionWithConst);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatAdditionWithConstTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarDouble_AdditionWithConst_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<double>(ScalarDoubleAdditionWithConst);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarDoubleAdditionWithConstTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_Addition_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatAddition);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatAdditionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayInt_Addition_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntAddition);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetArrayIntAdditionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayFloat_Addition_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float[]>(ArrayFloatAddition);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetArrayFloatAdditionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayInt_AdditionWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntAdditionWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetArrayIntAdditionWithKernelToolsTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayFloat_AdditionWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float[]>(ArrayFloatAdditionWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetArrayFloatAdditionWithKernelToolsTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayInt_ShortHandOperationsWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntShortHandOperationsWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected =
            _llvmLoader.GetArrayShortHandOperationsWithKernelToolsTestResult(kernelName, TypesAsString.Int32Type);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayLong_ShortHandOperationsWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<long[]>(ArrayLongShortHandOperationsWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected =
            _llvmLoader.GetArrayShortHandOperationsWithKernelToolsTestResult(kernelName, TypesAsString.Int64Type);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayFloat_ShortHandOperationsWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float[]>(ArrayFloatShortHandOperationsWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected =
            _llvmLoader.GetArrayShortHandOperationsWithKernelToolsTestResult(kernelName, TypesAsString.FloatType);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ArrayDouble_ShortHandOperationsWithKernelTools_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<double[]>(ArrayDoubleShortHandOperationsWithKernelTools);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected =
            _llvmLoader.GetArrayShortHandOperationsWithKernelToolsTestResult(kernelName, TypesAsString.DoubleType);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    #endregion

    #region Subtraction

    [Fact]
    public void ScalarInt_Subtraction_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntSubtraction);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntSubtractionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_Subtraction_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatSubtraction);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatSubtractionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Multiplication

    [Fact]
    public void ScalarInt_Multiplication_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntMultiplication);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntMultiplicationTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_Multiplication_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatMultiplication);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatMultiplicationTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    #endregion

    #region Division

    [Fact]
    public void ScalarInt_Division_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntDivision);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntDivisionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_Division_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatDivision);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatDivisionTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    #endregion

    #region Remainder

    [Fact]
    public void ScalarInt_Remainder_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntRemainder);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarIntRemainderTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    [Fact]
    public void ScalarFloat_Remainder_LLVM()
    {
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatRemainder);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = kernelName;
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

        var expected = _llvmLoader.GetScalarFloatRemainderTestResult(kernelName);
        var actual = llvmKernel.KernelBuffer;

        Assert.Equal(expected, actual);
        Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    }

    #endregion
}