using CuSharp.Tests.TestHelper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.ControlFlowKernels;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.DebugMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
public class ControlFlowDebugOpCodeTests
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
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Beq_s, actual[7].Operand),
                (ILOpCode.Br_s, actual[8].Operand), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_2, null),
                (ILOpCode.Beq_s, actual[11].Operand), (ILOpCode.Br_s, actual[12].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Br_s, actual[17].Operand),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Sub, null), (ILOpCode.Starg_s, actual[21].Operand),
                (ILOpCode.Br_s, actual[22].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Mul, null),
                (ILOpCode.Starg_s, actual[26].Operand), (ILOpCode.Br_s, actual[27].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[29].Operand), (ILOpCode.Ret, null)
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
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[6].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Brtrue_s, actual[13].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[15].Operand), (ILOpCode.Ret, null)
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
                (ILOpCode.Nop, null), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null),
                (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[5].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Brtrue_s, actual[12].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[14].Operand), (ILOpCode.Ret, null)
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
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Br_s, actual[3].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[8].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_1, null),
                (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null),
                (ILOpCode.Ldloc_1, null), (ILOpCode.Brtrue_s, actual[19].Operand), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[23].Operand),
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
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[8].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[10].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[14].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Brtrue_s, actual[21].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[23].Operand), (ILOpCode.Ret, null)
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
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Operand), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[8].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[10].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[14].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Brtrue_s, actual[21].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[23].Operand),
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
                (ILOpCode.Nop, null), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[5].Operand),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Operand),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Operand), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[15].Operand), (ILOpCode.Ret, null)
            };

        // Assert
        Assert.Equal(expected, actual);
    }
}