﻿using CuSharp.CudaCompiler.Frontend;
using LLVMSharp;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CuSharp.Tests.TestHelper
{
    public static class TestFunctionBuilder
    {
        public static FunctionsDto BuildFunctionsDto(string kernelName, ParameterInfo[] parameterInfos)
        {
            var function = GetFunction(kernelName, parameterInfos);
            var externalFunctions = GetExternalFunctions(kernelName);
            return new FunctionsDto(function, externalFunctions);
        }

        public static LLVMBuilderRef GetBuilderWithEntryBlock(LLVMValueRef function)
        {
            var builder = LLVM.CreateBuilder();
            var entryBlock = LLVM.AppendBasicBlock(function, "entry");
            LLVM.PositionBuilderAtEnd(builder, entryBlock);
            return builder;
        }

        private static LLVMValueRef GetFunction(string kernelName, ParameterInfo[] parameterInfos)
        {
            var module = LLVM.ModuleCreateWithName(kernelName);
            var paramsListBuilder = new List<LLVMTypeRef>();
            foreach (var paramInfo in parameterInfos)
            {
                    var type = paramInfo.ParameterType.IsArray
                        ? LLVM.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(), 0)
                        : paramInfo.ParameterType.ToLLVMType();
                paramsListBuilder.Add(type);
            }
            //if(paramsListBuilder.Any()) paramsListBuilder.Add(LLVMTypeRef.PointerType(LLVMTypeRef.Int32Type(), 0)); //array length list
            var paramType = paramsListBuilder.ToArray();
            return LLVM.AddFunction(module, kernelName, LLVM.FunctionType(LLVM.VoidType(), paramType, false));
        }

        private static (string, LLVMValueRef, LLVMValueRef[])[] GetExternalFunctions(string kernelName)
        {
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var externalFunctions = new (string, LLVMValueRef, LLVMValueRef[])[config.DeclareExternalFunctions.Length];
            var module = LLVM.ModuleCreateWithName(config.KernelName + "MODULE");

            var counter = 0;
            foreach (var declarationGenerator in config.DeclareExternalFunctions)
            {
                externalFunctions[counter] = declarationGenerator(module);
                counter++;
            }

            return externalFunctions;
        }
    }
}
