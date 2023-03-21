using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class KernelCrossCompiler
{
    private readonly CompilationConfiguration _config;
    private readonly LLVMModuleRef _module;
    private readonly LLVMBuilderRef _builder;
    public KernelCrossCompiler(CompilationConfiguration configuration)
    {
        _config = configuration;
        
        _module = LLVM.ModuleCreateWithName(_config.KernelName + "MODULE");
        _builder = LLVM.CreateBuilder();
    }

    public LLVMKernel Compile(MSILKernel inputKernel)
    {
        GenerateDataLayoutAndTarget();
        
        var function = GenerateFunctionAndPositionBuilderAtEntry(inputKernel.ParameterInfos);
        var externalFunctions = GenerateDeviceIntrinsicFunctions();

        var functionsDto = new FunctionsDto(function, externalFunctions);

        new MethodBodyCompiler(inputKernel, _builder, functionsDto).CompileMethodBody();
        GenerateAnnotations(function);

        return new LLVMKernel(inputKernel.Name, GetModuleAsString());
    }

    
    private string GetModuleAsString()
    {
        var unmanagedString = LLVM.PrintModuleToString(_module);
        var kernelString = Marshal.PtrToStringAnsi(unmanagedString);
        return kernelString;
    }

    private void GenerateAnnotations(LLVMValueRef function)
    {
        foreach (var annotationGenerator in _config.DeclareAnnotations)
        {
            annotationGenerator(_module, function);
        }

    }

    private (string, LLVMValueRef)[] GenerateDeviceIntrinsicFunctions()
    {
        var externalFunctions = new (string, LLVMValueRef)[_config.DeclareExternalFunctions.Length];
        var counter = 0;
        foreach (var declarationGenerator in _config.DeclareExternalFunctions)
        {
            externalFunctions[counter] = declarationGenerator(_module);
            counter++;
        }

        return externalFunctions;
    }
    private LLVMValueRef GenerateFunctionAndPositionBuilderAtEntry(ParameterInfo[] parameterInfos)
    {
        var paramsListBuilder = new List<LLVMTypeRef>();
        foreach (var paramInfo in parameterInfos)
        {
            LLVMTypeRef type;
            if (paramInfo.ParameterType.IsArray)
            {
                type = LLVMTypeRef.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(), 0);
            }
            else
            {
                type = LLVMTypeRef.PointerType(paramInfo.ParameterType.ToLLVMType(), 0);
            }
            paramsListBuilder.Add(type);
        }

        var paramType = paramsListBuilder.ToArray();
        var function = LLVM.AddFunction(_module, _config.KernelName, LLVM.FunctionType(LLVM.VoidType(), paramType, false));
        LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
        NameFunctionParameters(function, "param");
        
        var entryBlock = LLVM.AppendBasicBlock(function, "entry");
        LLVM.PositionBuilderAtEnd(_builder, entryBlock);
        
        return function;
    }

    private void NameFunctionParameters(LLVMValueRef function, string prefix)
    {
        var parameters = LLVM.GetParams(function);
                
        var counter = 0;
        foreach (var param in parameters)
        {
            LLVM.SetValueName(param, $"{prefix}{counter}");
            counter++;
        }
    }
    private void GenerateDataLayoutAndTarget()
    {
        if (_config.DataLayout != "")
        {
            LLVM.SetDataLayout(_module, _config.DataLayout);
        }
        if (_config.Target != "")
        {
            LLVM.SetTarget(_module, _config.Target);
        }
    }
}