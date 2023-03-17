using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using CuSharp.CudaCompiler.Frontend;
using LLVMSharp;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler.KernelCrossCompilerTests
{
    public class MethodDecompilerTests
    {
        private readonly MethodsToCompile _methods = new();
        private readonly MethodInfoLoader _methodInfo = new();

        [Fact]
        public void TestScalarIntAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4, 12345), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarIntAdditionWithConst";
            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);
            
            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarLongAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i8, 1234567890987), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarLongAdditionWithConst";
            var method = _methodInfo.GetScalarLongMethodInfo(_methods.ScalarLongAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarIntAdditionOpCode";
            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntSubtractionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Sub, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarIntSubtractionOpCode";
            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntMultiplicationOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarIntMultiplicationOpCode";
            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_r4, 1234.321F), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarFloatAdditionWithConst";
            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarDoubleAdditionWithConst()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_r8, 123456.54321), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_1, null),
                (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_1, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarDoubleAdditionWithConst";
            var method = _methodInfo.GetScalarDoubleMethodInfo(_methods.ScalarDoubleAdditionWithConst);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Add, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarFloatAdditionOpCode";
            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatSubtractionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Sub, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarFloatSubtractionOpCode";
            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatSubtraction);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatMultiplicationOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Mul, null),
                (ILOpCode.Stloc_0, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestScalarFloatMultiplicationOpCode";
            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatMultiplication);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayIntAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_3, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestArrayIntAdditionOpCode";
            var method = _methodInfo.GetArrayIntMethodInfo(_methods.ArrayIntAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayFloatAdditionOpCode()
        {
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Ldc_i4_0, null), (ILOpCode.Stloc_0, null), (ILOpCode.Ldarg_3, null),
                (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
            };

            var kernelName = "TestArrayFloatAdditionOpCode";
            var method = _methodInfo.GetArrayFloatMethodInfo(_methods.ArrayFloatAddition);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayIntAdditionWithKernelToolsOpCode()
        {
            var kernelName = "TestArrayIntAdditionWithKernelToolsOpCode";
            var method = _methodInfo.GetArrayIntMethodInfo(_methods.ArrayIntAdditionWithKernelTools);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            // Some values are machine-dependent and must therefore be loaded from the actual result
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[1].Item2), (ILOpCode.Ldfld, actual[2].Item2),
                (ILOpCode.Call, actual[3].Item2), (ILOpCode.Ldfld, actual[4].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[6].Item2), (ILOpCode.Ldfld, actual[7].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_3, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_i4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_i4, null), (ILOpCode.Ret, null)
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayFloatAdditionWithKernelToolsOpCode()
        {
            var kernelName = "TestArrayFloatAdditionWithKernelToolsOpCode";
            var method = _methodInfo.GetArrayFloatMethodInfo(_methods.ArrayFloatAdditionWithKernelTools);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);

            var actual = new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody().ToList();

            // Some values are machine-dependent and must therefore be loaded from the actual result
            var expected = new List<(ILOpCode, object?)>
            {
                (ILOpCode.Nop, null), (ILOpCode.Call, actual[1].Item2), (ILOpCode.Ldfld, actual[2].Item2),
                (ILOpCode.Call, actual[3].Item2), (ILOpCode.Ldfld, actual[4].Item2), (ILOpCode.Mul, null),
                (ILOpCode.Call, actual[6].Item2), (ILOpCode.Ldfld, actual[7].Item2), (ILOpCode.Add, null), (ILOpCode.Stloc_0, null),
                (ILOpCode.Ldarg_3, null), (ILOpCode.Ldloc_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Ldarg_2, null), (ILOpCode.Ldloc_0, null),
                (ILOpCode.Ldelem_r4, null), (ILOpCode.Add, null), (ILOpCode.Stelem_r4, null), (ILOpCode.Ret, null)
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNotSupportedNestedCall()
        {
            var kernelName = "TestNotSupportedNestedCall";
            var method = _methodInfo.GetScalarIntMethodInfo(_methods.NotSupportedNestedCall);
            var kernel = new MSILKernel(kernelName, method);
            var builder = LLVM.CreateBuilder();
            var function = GetFunction(kernelName, method.GetParameters());
            var externalFunctions = GetExternalFunctions(kernelName);
            var functionsDto = new FunctionsDto(function, externalFunctions);
            
            Assert.Throws<NotSupportedException>(() => new MethodBodyCompiler(kernel, builder, functionsDto).CompileMethodBody());
        }

        private LLVMValueRef GetFunction(string kernelName, ParameterInfo[] parameterInfos)
        {
            var module = LLVM.ModuleCreateWithName(kernelName);
            var paramsListBuilder = new List<LLVMTypeRef>();
            foreach (var paramInfo in parameterInfos)
            {
                var type = paramInfo.ParameterType.IsArray
                    ? LLVMTypeRef.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(), 0)
                    : paramInfo.ParameterType.ToLLVMType();
                paramsListBuilder.Add(type);
            }
            var paramType = paramsListBuilder.ToArray();
            return LLVM.AddFunction(module, kernelName, LLVM.FunctionType(LLVM.VoidType(), paramType, false));
        }

        private (string, LLVMValueRef)[] GetExternalFunctions(string kernelName)
        {
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var externalFunctions = new (string, LLVMValueRef)[config.DeclareExternalFunctions.Length];
            var module = LLVM.ModuleCreateWithName(config.KernelName + "MODULE");

            var counter = 0;
            foreach(var declarationGenerator in config.DeclareExternalFunctions)
            {
                externalFunctions[counter] = declarationGenerator(module);
                counter++;
            }

            return externalFunctions;
        }
    }
}
