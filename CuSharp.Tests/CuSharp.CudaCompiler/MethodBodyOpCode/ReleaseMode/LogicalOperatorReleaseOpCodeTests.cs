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
public class LogicalOperatorReleaseOpCodeTests
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
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un_s, actual[2].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Bne_un_s, actual[5].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[13].Operand), (ILOpCode.Ret, null)
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
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Beq_s, actual[2].Operand),
            (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Bne_un_s, actual[5].Operand),
            (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
            (ILOpCode.Starg_s, actual[9].Operand), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
            (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[13].Operand), (ILOpCode.Ret, null)
        };

        // Assert
        Assert.Equal(expected, actual);
    }
}