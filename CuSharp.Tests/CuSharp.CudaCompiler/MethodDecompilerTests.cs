using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using LLVMSharp;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    public class MethodDecompilerTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly TestFunctionBuilder _functionBuilder = new();

        [Fact]
        public void TestScalarIntAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4, 12345), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_0, null),
                (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };

            // Arrange
            const string kernelName = "TestScalarIntAdditionWithConst";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarLongAdditionWithConst";
            var method = _methodLoader.GetScalarLongMethodInfo(MethodsToCompile.ScalarLongAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarIntAdditionOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarIntSubtractionOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarIntMultiplicationOpCode";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarFloatAdditionWithConst";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarDoubleAdditionWithConst";
            var method = _methodLoader.GetScalarDoubleMethodInfo(MethodsToCompile.ScalarDoubleAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarFloatAdditionOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarFloatSubtractionOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestScalarFloatMultiplicationOpCode";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestArrayIntAdditionOpCode";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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

            // Arrange
            const string kernelName = "TestArrayFloatAdditionOpCode";
            var method = _methodLoader.GetArrayFloatMethodInfo(MethodsToCompile.ArrayFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

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
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[1].Item2), (ILOpCode.Ldfld, actual[2].Item2),
                (ILOpCode.Call, actual[3].Item2), (ILOpCode.Ldfld, actual[4].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[6].Item2), (ILOpCode.Ldfld, actual[7].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
            };

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
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

            // Act
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[1].Item2), (ILOpCode.Ldfld, actual[2].Item2),
                (ILOpCode.Call, actual[3].Item2), (ILOpCode.Ldfld, actual[4].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[6].Item2), (ILOpCode.Ldfld, actual[7].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_0, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
            };

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNotSupportedNestedCall()
        {
            // Arrange
            const string kernelName = "TestNotSupportedNestedCall";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.NotSupportedNestedCall);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());

            // Assert
            Assert.Throws<NotSupportedException>(() =>
                // Act
                new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody());
        }
    }
}
