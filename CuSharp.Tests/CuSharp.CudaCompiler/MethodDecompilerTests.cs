﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    [Collection("Sequential")]
    public class MethodDecompilerTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly TestFunctionBuilder _functionBuilder = new();

        [Fact]
        public void TestScalarIntAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4, 12345), (ILOpCode.Stloc_0, null), (ILOpCode.Ldc_i4_0, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), 
                (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_1, null), (ILOpCode.Add, null),
                (ILOpCode.Ldc_i4_m1, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_1, null), (ILOpCode.Add, null),
                (ILOpCode.Ldc_i4_2, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_3, null), (ILOpCode.Add, null),
                (ILOpCode.Ldc_i4_4, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_5, null), (ILOpCode.Add, null),
                (ILOpCode.Ldc_i4_6, null), (ILOpCode.Add, null), (ILOpCode.Ldc_i4_7, null), (ILOpCode.Add, null),
                (ILOpCode.Ldc_i4_8, null), (ILOpCode.Add, null), (ILOpCode.Stloc_2, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntAdditionWithConst";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarLongAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i8, 1234567890987), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarLongAdditionWithConst";
            var method = _methodLoader.GetScalarLongMethodInfo(MethodsToCompile.ScalarLongAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntAdditionOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntSubtractionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Sub, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntSubtractionOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntMultiplicationOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Mul, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntMultiplicationOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntDivisionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Div, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntDivisionOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntDivision);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntRemainderOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Rem, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarIntRemainderOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntRemainder);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void TestScalarFloatAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_r4, 1234.321F), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatAdditionWithConst";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarDoubleAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_r8, 123456.54321), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarDoubleAdditionWithConst";
            var method = _methodLoader.GetScalarDoubleMethodInfo(MethodsToCompile.ScalarDoubleAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatAdditionOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatSubtractionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Sub, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatSubtractionOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatMultiplicationOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Mul, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatMultiplicationOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFloatDivisionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Div, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatDivisionOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatDivision);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatRemainderOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Rem, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestScalarFloatRemainderOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatRemainder);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayIntAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestArrayIntAdditionOpCode";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayFloatAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_2, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Arrange
            const string kernelName = "TestArrayFloatAdditionOpCode";
            var method = _methodLoader.GetArrayFloatMethodInfo(MethodsToCompile.ArrayFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayIntAdditionWithKernelToolsOpCode()
        {
            // Arrange
            const string kernelName = "TestArrayIntAdditionWithKernelToolsOpCode";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntAdditionWithKernelTools);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetIndexInReleaseMode(1, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(2, 1)].Item2),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(3, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(4, 1)].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(6, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(7, 1)].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayFloatAdditionWithKernelToolsOpCode()
        {
            // Arrange
            const string kernelName = "TestArrayFloatAdditionWithKernelToolsOpCode";
            var method = _methodLoader.GetArrayFloatMethodInfo(MethodsToCompile.ArrayFloatAdditionWithKernelTools);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetIndexInReleaseMode(1, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(2, 1)].Item2),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(3, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(4, 1)].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(6, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(7, 1)].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayShortHandOperationsWithKernelToolsOpCode()
        {
            // Arrange
            const string kernelName = "TestArrayShortHandAdditionWithKernelToolsOpCode";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntShortHandOperationsWithKernelTools);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[GetIndexInReleaseMode(1, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(2, 1)].Item2),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(3, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(4, 1)].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[GetIndexInReleaseMode(6, 1)].Item2), (ILOpCode.Ldfld, actual[GetIndexInReleaseMode(7, 1)].Item2), (ILOpCode.Add, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelema, actual[GetIndexInReleaseMode(12, 1)].Item2), (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null),
                (ILOpCode.Stind_i4, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetIndexInReleaseMode(22, 1)].Item2),
                (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Sub, null), (ILOpCode.Stind_i4, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetIndexInReleaseMode(32, 1)].Item2),
                (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Mul, null), (ILOpCode.Stind_i4, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetIndexInReleaseMode(42, 1)].Item2),
                (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Div, null), (ILOpCode.Stind_i4, null),
                (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldelema, actual[GetIndexInReleaseMode(52, 1)].Item2),
                (ILOpCode.Dup, null), (ILOpCode.Ldind_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Rem, null), (ILOpCode.Stind_i4, null), (ILOpCode.Ret, null)
            };
            expected = RemoveNopInReleaseMode(expected);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNotSupportedNonStaticCall()
        {
            // Arrange
            const string kernelName = "TestNotSupportedNonStaticCall";
            var method = _methodLoader.GetMethodInfo(new MethodsToCompile().NonStaticEmptyMethod);
            
            // Assert
            Assert.Throws<NotSupportedException>(() => 
                // Act
                new MSILKernel(kernelName, method));
        }

        [Fact]
        public void TestNotSupportedNestedCall()
        {
            // Arrange
            const string kernelName = "TestNotSupportedNestedCall";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.NotSupportedNestedCall);
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);

            // Assert
            Assert.Throws<NotSupportedException>(() =>
                // Act
                new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody());
        }

        private static List<(ILOpCode, object?)> RemoveNopInReleaseMode(List<(ILOpCode, object?)> opCodesWithOperands)
        {
            #if (RELEASE)
            opCodesWithOperands = opCodesWithOperands.Where(inst => inst.Item1 != ILOpCode.Nop).ToList();
            #endif
            return opCodesWithOperands;
        }

        private static int GetIndexInReleaseMode(int index, int amountToDecrease)
        {
            #if (RELEASE)
            index -= amountToDecrease;
            #endif
            return index;
        }
    }
}
