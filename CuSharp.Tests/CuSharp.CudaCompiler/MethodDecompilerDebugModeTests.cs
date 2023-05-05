using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using LLVMSharp;
using ManagedCuda.BasicTypes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    /// <summary>
    /// These tests only work in DEBUG MODE,
    /// because Roslyn compiler uses partially different op-codes in debug and release mode.
    /// </summary>
    [Collection("Sequential")]
    [Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
    public class MethodDecompilerDebugModeTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly TestFunctionBuilder _functionBuilder = new();

        [Fact]
        public void TestLogicalAndOpCode()
        {
            // Arrange
            const string kernelName = "TestLogicalAndOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LogicalAnd);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Bne_un_s, actual[3].Item2),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ceq, null), (ILOpCode.Br_s, actual[7].Item2),
                (ILOpCode.Ldc_i4_0,  null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
                (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLogicalOrOpCode()
        {
            // Arrange
            const string kernelName = "TestLogicalOrOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LogicalOr);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Beq_s, actual[3].Item2),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ceq, null), (ILOpCode.Br_s, actual[7].Item2),
                (ILOpCode.Ldc_i4_1,  null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
                (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLessThanOpCode()
        {
            // Arrange
            const string kernelName = "TestLessThanOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LessThan);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Item2), (ILOpCode.Nop, null), 
                (ILOpCode.Br_s, actual[11].Item2), (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null), 
                (ILOpCode.Starg_s, actual[14].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_2, null), 
                (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Item2), (ILOpCode.Ret, null)

            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLessThanOrEqualsOpCode()
        {
            // Arrange
            const string kernelName = "TestLessThanOrEqualsOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LessThanOrEquals);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Cgt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Item2), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Item2),
                (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Item2),
                (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGreaterThanOpCode()
        {
            // Arrange
            const string kernelName = "TestGreaterThanOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.GreaterThan);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Cgt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Br_s, actual[11].Item2), (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null),
                (ILOpCode.Starg_s, actual[14].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Item2), (ILOpCode.Ret, null)

            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGreaterThanOrEqualsOpCode()
        {
            // Arrange
            const string kernelName = "TestGreaterThanOrEqualsOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.GreaterThanOrEquals);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Item2), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Item2),
                (ILOpCode.Nop, null),  (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Item2),
                (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestEqualsToOpCode()
        {
            // Arrange
            const string kernelName = "TestEqualsToOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.EqualsTo);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[6].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[9].Item2), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[11].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[14].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[19].Item2),
                (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNotEqualsToOpCode()
        {
            // Arrange
            const string kernelName = "TestNotEqualsToOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.NotEqualsTo);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Brfalse_s, actual[8].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[11].Item2), (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null), (ILOpCode.Starg_s, actual[21].Item2),
                (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSwitchOpCode()
        {
            // Arrange
            const string kernelName = "TestSwitchOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.Switch);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Beq_s, actual[7].Item2),
                (ILOpCode.Br_s, actual[8].Item2), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_2, null),
                (ILOpCode.Beq_s, actual[11].Item2), (ILOpCode.Br_s, actual[12].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[16].Item2), (ILOpCode.Br_s, actual[17].Item2),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Sub, null), (ILOpCode.Starg_s, actual[21].Item2),
                (ILOpCode.Br_s, actual[22].Item2), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Mul, null),
                (ILOpCode.Starg_s, actual[26].Item2), (ILOpCode.Br_s, actual[27].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[29].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestWhileOpCode()
        {
            // Arrange
            const string kernelName = "TestWhileOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.While);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[6].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_0, null), 
                (ILOpCode.Ldloc_0, null), (ILOpCode.Brtrue_s, actual[13].Item2), (ILOpCode.Ldarg_0, null), 
                (ILOpCode.Starg_s, actual[15].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDoWhileOpCode()
        {
            // Arrange
            const string kernelName = "TestDoWhileOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.DoWhile);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldc_i4_1, null),
                (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[5].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Brtrue_s, actual[12].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[14].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestForOpCode()
        {
            // Arrange
            const string kernelName = "TestForOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.For);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Br_s, actual[3].Item2), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[8].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldc_i4_1, null),
                (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null),
                (ILOpCode.Ldloc_1, null), (ILOpCode.Brtrue_s, actual[19].Item2), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[23].Item2),
                (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestWhileWithContinueOpCode()
        {
            // Arrange
            const string kernelName = "TestWhileWithContinueOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.WhileWithContinue);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[8].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[10].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[14].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Brtrue_s, actual[21].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[23].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestWhileWithBreakOpCode()
        {
            // Arrange
            const string kernelName = "TestWhileWithBreakOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.WhileWithBreak);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[1].Item2), (ILOpCode.Nop, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Ceq, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[8].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[10].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[14].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Clt, null), (ILOpCode.Stloc_1, null), (ILOpCode.Ldloc_1, null),
                (ILOpCode.Brtrue_s, actual[21].Item2), (ILOpCode.Ldarg_0, null), (ILOpCode.Starg_s, actual[23].Item2),
                (ILOpCode.Ret, null),
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestForeachOpCode()
        {
            // Arrange
            const string kernelName = "TestForeachOpCode";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.Foreach);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Stloc_1, null),
                (ILOpCode.Ldc_i4_0 , null), (ILOpCode.Stloc_2, null), (ILOpCode.Br_s, actual[8].Item2),
                (ILOpCode.Ldloc_1, null), (ILOpCode.Ldloc_2, null), (ILOpCode.Ldelem_i4, null),
                (ILOpCode.Stloc_3, null), (ILOpCode.Nop, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Nop, null), (ILOpCode.Ldloc_2, null), (ILOpCode.Ldc_i4_1, null),
                (ILOpCode.Add, null), (ILOpCode.Stloc_2, null), (ILOpCode.Ldloc_2, null),
                (ILOpCode.Ldloc_1, null), (ILOpCode.Ldlen, null), (ILOpCode.Conv_i4, null),
                (ILOpCode.Blt_s, actual[27].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGotoOpCode()
        {
            // Arrange
            const string kernelName = "TestGotoOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.Goto);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null), (ILOpCode.Starg_s, actual[5].Item2),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Clt, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Brfalse_s, actual[11].Item2),
                (ILOpCode.Nop, null), (ILOpCode.Br_s, actual[13].Item2), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Starg_s, actual[15].Item2), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestCallIntMethod()
        {
            // Arrange
            const string kernelName = "TestCallIntMethodOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.CallIntMethod);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);
            var functionGenerator = new FunctionGenerator(LLVM.ModuleCreateWithName(kernelName + "MODULE"), builder);

            // Act Main
            var actualMain = new MethodBodyCompiler(kernel, builder, functionsDto, functionGenerator).CompileMethodBody().ToList();

            // Act Call
            var call = functionGenerator.FunctionsToBuild[0];
            functionGenerator.AppendFunction(call.llvmFunction);
            functionsDto.Function = call.llvmFunction;
            var actualCall = new MethodBodyCompiler(call.msilFunction, builder, functionsDto, functionGenerator)
                .CompileMethodBody().ToList();

            var expectedMain = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Call, actualMain[3].Item2), (ILOpCode.Starg_s, actualMain[4].Item2), (ILOpCode.Ret, null)
            };

            var expectedCall = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Add, null), (ILOpCode.Stloc_0, null), (ILOpCode.Br_s, actualCall[5].Item2),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Ret, null)
            };

            Assert.Single(functionGenerator.FunctionsToBuild);
            Assert.Equal(expectedMain, actualMain);
            Assert.Equal(expectedCall, actualCall);
        }
    }
}
