using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using CuSharp.Tests.TestHelper;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.ReleaseMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class ControlFlowReleaseOpCode
{
    [Fact]
    public void ScalarInt_Switch_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(Switch);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Beq_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_2, null), (ILOpCode.Beq_s, actual[5].Operand),
            (ILOpCode.Br_s, actual[6].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[10].Operand), (ILOpCode.Br_s, actual[11].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Sub, null),
            (ILOpCode.Starg_s, actual[15].Operand), (ILOpCode.Br_s, actual[16].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[20].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[22].Operand), (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_While_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(While);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Br_s, actual[0].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null),
            (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[4].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Blt_s, actual[7].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Ret, null),
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_DoWhile_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(DoWhile);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[3].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Blt_s, actual[6].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[8].Operand),
            (ILOpCode.Ret, null),
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_For_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(For);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Br_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[6].Operand), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_1, null),
            (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Blt_s, actual[13].Operand), (ILOpCode.Ldarg_2, null),
            (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[17].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_WhileWithContinue_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(WhileWithContinue);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Br_s, actual[0].Operand), (ILOpCode.Ldarg_2, null), (ILOpCode.Brfalse_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[6].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Blt_s, actual[9].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null),
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_WhileWithBreak_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(WhileWithBreak);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Br_s, actual[0].Operand), (ILOpCode.Ldarg_2, null), (ILOpCode.Brfalse_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[6].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Blt_s, actual[9].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[11].Operand),
            (ILOpCode.Ret, null),
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_Goto_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(Goto);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[3].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Blt_s, actual[6].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[8].Operand),
            (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }
}