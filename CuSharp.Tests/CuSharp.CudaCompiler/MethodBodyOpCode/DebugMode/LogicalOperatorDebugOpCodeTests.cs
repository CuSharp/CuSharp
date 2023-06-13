using CuSharp.Tests.TestHelper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using Xunit;
using static CuSharp.Tests.TestHelper.CompilerCreator;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestKernels.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.DebugMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
public class LogicalOperatorDebugOpCodeTests
{
    [Fact]
    public void ScalarInt_LogicalAnd_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LogicalAnd);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un_s, actual[3].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ceq, null), (ILOpCode.Br_s, actual[7].Operand),
            (ILOpCode.Ldc_i4_0,  null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Operand),
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Operand), (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ScalarInt_LogicalOr_OpCodes()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo<int>(LogicalOr);
        var methodBodyCompiler = GetMethodBodyCompiler(kernelName, method);

        // Act
        var actual = methodBodyCompiler.CompileMethodBody().ToList();

        var expected = new List<(ILOpCode, object?)>
        {
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Beq_s, actual[3].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ceq, null), (ILOpCode.Br_s, actual[7].Operand),
            (ILOpCode.Ldc_i4_1,  null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Operand),
            (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[16].Operand), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Operand), (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }
}