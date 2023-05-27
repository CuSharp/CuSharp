using CuSharp.Tests.TestHelper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.ComparisonKernels;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.DebugMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
public class ComparisonDebugOpCodeTests
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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Br_s, actual[11].Operand), (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null),
                (ILOpCode.Starg_s, actual[14].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Operand), (ILOpCode.Ret, null)

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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Cgt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Operand), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Operand),
                (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Operand),
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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Cgt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Br_s, actual[11].Operand), (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null),
                (ILOpCode.Starg_s, actual[14].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Operand), (ILOpCode.Ret, null)

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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Operand), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Operand),
                (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Operand),
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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[11].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[14].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Operand),
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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Operand), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Operand),
                (ILOpCode.Ret, null)
            };

        // Assert
        Assert.Equal(expected, actual);
    }
}