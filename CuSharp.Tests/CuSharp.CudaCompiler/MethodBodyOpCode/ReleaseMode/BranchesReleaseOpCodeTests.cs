using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;
using CuSharp.Tests.TestHelper;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.ReleaseMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class BranchesReleaseOpCode
{
    [Fact]
    public void ScalarInt_BranchesWithInt32Target_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(BranchesWithIn32Target);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bge, actual[4].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bgt, actual[7].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ble, actual[10].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Blt, actual[13].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un, actual[16].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Beq, actual[19].Operand)
        };

        for (var i = 0; i < 32; i++)
        {
            expected.Add((ILOpCode.Ldloc_0, null));
            expected.Add((ILOpCode.Ldarg_1, null));
            expected.Add((ILOpCode.Add, null));
            expected.Add((ILOpCode.Stloc_0, null));
        }

        expected.Add((ILOpCode.Ret, null));

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarUint_BranchesWithInt64Target_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint>(BranchesWithInt32Target);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bge_un, actual[4].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bgt_un, actual[7].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ble_un, actual[10].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Blt_un, actual[13].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un, actual[16].Operand), (ILOpCode.Ldarg_0, null),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Beq, actual[19].Operand)
        };

        for (var i = 0; i < 32; i++)
        {
            expected.Add((ILOpCode.Ldloc_0, null));
            expected.Add((ILOpCode.Ldarg_1, null));
            expected.Add((ILOpCode.Add, null));
            expected.Add((ILOpCode.Stloc_0, null));
        }

        expected.Add((ILOpCode.Ret, null));

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarUint_BranchesInt8Target_OpCode()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<uint>(BranchesInt8TargetUnsigned);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bge_un_s, actual[2].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bgt_un_s, actual[5].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ble_un_s, actual[8].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Blt_un_s, actual[11].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un_s, actual[14].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[18].Operand), (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }
}