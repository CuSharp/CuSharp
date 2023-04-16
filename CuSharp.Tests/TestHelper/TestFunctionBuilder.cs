using CuSharp.CudaCompiler.Frontend;
using LLVMSharp;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CuSharp.Tests.TestHelper
{
    public class TestFunctionBuilder
    {
        public FunctionsDto BuildFunctionsDto(string kernelName, ParameterInfo[] parameterInfos)
        {
            var function = GetFunction(kernelName, parameterInfos);
            var externalFunctions = GetExternalFunctions(kernelName);
            return new FunctionsDto(function, externalFunctions, parameterInfos.Count(p => p.ParameterType.IsArray));
        }

        public LLVMBuilderRef GetBuilderWithEntryBlock(LLVMValueRef function)
        {
            var builder = LLVM.CreateBuilder();
            var entryBlock = LLVM.AppendBasicBlock(function, "entry");
            LLVM.PositionBuilderAtEnd(builder, entryBlock);
            return builder;
        }

        private LLVMValueRef GetFunction(string kernelName, ParameterInfo[] parameterInfos)
        {
            var module = LLVM.ModuleCreateWithName(kernelName);
            var paramsListBuilder = new List<LLVMTypeRef>();
            foreach (var paramInfo in parameterInfos)
            {
                var type = LLVMTypeRef.PointerType(
                    paramInfo.ParameterType.IsArray
                        ? paramInfo.ParameterType.GetElementType().ToLLVMType()
                        : paramInfo.ParameterType.ToLLVMType(), 0);
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
            foreach (var declarationGenerator in config.DeclareExternalFunctions)
            {
                externalFunctions[counter] = declarationGenerator(module);
                counter++;
            }

            return externalFunctions;
        }
    }
}
