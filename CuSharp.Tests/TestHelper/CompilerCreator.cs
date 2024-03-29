﻿using LLVMSharp;
using System.Reflection;
using CuSharp.CudaCompiler.Compiler;
using CuSharp.CudaCompiler.Kernels;

namespace CuSharp.Tests.TestHelper
{
    public class CompilerCreator
    {
        public static MethodBodyCompiler GetMethodBodyCompiler(string kernelName, MethodInfo method)
        {
            var kernel = new MSILKernel(kernelName, method);
            var functionsDto = TestFunctionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
            var builder = LLVM.CreateBuilder();
            return new MethodBodyCompiler(kernel, builder, functionsDto);
        }
    }
}
