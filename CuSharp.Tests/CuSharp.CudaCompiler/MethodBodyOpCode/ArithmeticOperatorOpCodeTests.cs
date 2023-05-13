using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using CuSharp.Tests.TestHelper;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;
using static CuSharp.Tests.TestHelper.ReleaseAdaption;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class ArithmeticOperatorOpCode
{
    #region Addition

    [Fact]
    public void ScalarInt_Addition_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntAddition);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_Addition5Args_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntAddition5Args);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_s, actual[GetReleaseIndex(1, 1)].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Add, null), (ILOpCode.Ldarg_3, null), (ILOpCode.Add, null),
            (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[GetReleaseIndex(10, 1)].Operand), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_AdditionWithConst_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_i4, 12345), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Add, null), (ILOpCode.Ldloc_1, null), (ILOpCode.Add, null),
            (ILOpCode.Ldc_i4_m1, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_1, null),
            (ILOpCode.Add, null), (ILOpCode.Ldc_i4_2, null), (ILOpCode.Add, null),
            (ILOpCode.Ldc_i4_3, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_4, null),
            (ILOpCode.Add, null), (ILOpCode.Ldc_i4_5, null), (ILOpCode.Add, null),
            (ILOpCode.Ldc_i4_6, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_7, null),
            (ILOpCode.Add, null), (ILOpCode.Ldc_i4_8, null), (ILOpCode.Add, null),
            (ILOpCode.Stloc_2, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntAdditionWithConst);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarLong_AdditionWithConst_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_i8, 1234567890987), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null), (ILOpCode.Stloc_1, null),
            (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<long>(ScalarLongAdditionWithConst);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_AdditionWithConst_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_r4, 1234.321F), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null), (ILOpCode.Stloc_1, null),
            (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatAdditionWithConst);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarDouble_AdditionWithConst_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_r8, 123456.54321), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null), (ILOpCode.Stloc_1, null),
            (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<double>(ScalarDoubleAdditionWithConst);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_AdditionOpCode_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatAddition);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ArrayInt_Addition_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null),
            (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntAddition);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ArrayFloat_Addition_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null),
            (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float[]>(ArrayFloatAddition);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ArrayInt_AdditionWithKernelTools_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntAdditionWithKernelTools);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetReleaseIndex(1, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(2, 1)].Operand),
            (ILOpCode.Call, actual[GetReleaseIndex(3, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(4, 1)].Operand), (ILOpCode.Mul, null),
            (ILOpCode.Call, actual[GetReleaseIndex(6, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(7, 1)].Operand), (ILOpCode.Add, null),
            (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null),
            (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ArrayFloat_AdditionWithKernelTools_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float[]>(ArrayFloatAdditionWithKernelTools);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetReleaseIndex(1, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(2, 1)].Operand),
            (ILOpCode.Call, actual[GetReleaseIndex(3, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(4, 1)].Operand), (ILOpCode.Mul, null),
            (ILOpCode.Call, actual[GetReleaseIndex(6, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(7, 1)].Operand), (ILOpCode.Add, null),
            (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_r4, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_r4, null),
            (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Subtraction

    [Fact]
    public void ScalarInt_Subtraction_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Sub, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntSubtraction);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_Subtraction_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Sub, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatSubtraction);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Multiplication

    [Fact]
    public void ScalarInt_Multiplication_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntMultiplication);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_Multiplication_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatMultiplication);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Division

    [Fact]
    public void ScalarInt_Division_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Div, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntDivision);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_Division_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Div, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatDivision);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Remainder

    [Fact]
    public void ScalarInt_Remainder_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Rem, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(ScalarIntRemainder);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarFloat_Remainder_OpCodes()
    {
        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Rem, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<float>(ScalarFloatRemainder);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Mixed Operators

    [Fact]
    public void ArrayInt_ShortHandOperationsWithKernelTools_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int[]>(ArrayIntShortHandOperationsWithKernelTools);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetReleaseIndex(1, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(2, 1)].Operand),
            (ILOpCode.Call, actual[GetReleaseIndex(3, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(4, 1)].Operand), (ILOpCode.Mul, null),
            (ILOpCode.Call, actual[GetReleaseIndex(6, 1)].Operand),
            (ILOpCode.Ldfld, actual[GetReleaseIndex(7, 1)].Operand), (ILOpCode.Add, null),
            (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldelema, actual[GetReleaseIndex(12, 1)].Operand), (ILOpCode.Dup, null),
            (ILOpCode.Ldind_i4, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null),
            (ILOpCode.Add, null), (ILOpCode.Stind_i4, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetReleaseIndex(22, 1)].Operand),
            (ILOpCode.Dup, null),
            (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldelem_i4, null), (ILOpCode.Sub, null), (ILOpCode.Stind_i4, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldelema, actual[GetReleaseIndex(32, 1)].Operand),
            (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null), (ILOpCode.Mul, null),
            (ILOpCode.Stind_i4, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldelema, actual[GetReleaseIndex(42, 1)].Operand), (ILOpCode.Dup, null),
            (ILOpCode.Ldind_i4, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null),
            (ILOpCode.Div, null), (ILOpCode.Stind_i4, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetReleaseIndex(52, 1)].Operand),
            (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldelem_i4, null), (ILOpCode.Rem, null), (ILOpCode.Stind_i4, null), (ILOpCode.Ret, null)
        };
        expected = RemoveNopInReleaseMode(expected);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion
}