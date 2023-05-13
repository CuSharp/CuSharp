using CuSharp.Tests.TestHelper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.ReleaseMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class ComparisonReleaseOpCodeTests
{
    [Fact]
    public void ScalarInt_LessThan_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LessThan);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bge_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_LessThanOrEquals_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LessThanOrEquals);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bgt_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_GreaterThan_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(GreaterThan);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ble_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_GreaterThanOrEquals_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(GreaterThanOrEquals);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Blt_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_EqualsTo_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(EqualsTo);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_NotEqualsTo_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(NotEqualsTo);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Blt_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Br_s, actual[5].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[7].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }
}