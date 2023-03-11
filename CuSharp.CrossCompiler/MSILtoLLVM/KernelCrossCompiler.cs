using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LLVMSharp;

namespace CuSharp.MSILtoLLVMCompiler;

public class KernelCrossCompiler
{
    private CompilationConfiguration _config;
    private LLVMModuleRef _module;
    private LLVMBuilderRef _builder;
    public KernelCrossCompiler(CompilationConfiguration configuration)
    {
        _config = configuration;
        _module = LLVM.ModuleCreateWithName(_config.KernelName + "MODULE");
        _builder = LLVM.CreateBuilder();

    }


    public LLVMKernel Compile(MSILKernel inputKernel)
    {
        GenerateDataLayoutAndTarget();

        new MethodBodyCompiler(inputKernel.KernelBuffer, _module, _builder).CompileMethodBody();
        var function = GenerateFunction(inputKernel.ParameterInfos);
        LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
        LLVMValueRef[] parameters = LLVM.GetParams(function);
        int counter = 0;
        foreach (var param in parameters)
        {
            LLVM.SetValueName(param, "param" + counter);
            counter++;
        }

        var entryBlock = LLVM.AppendBasicBlock(function,"entry");
        LLVM.PositionBuilderAtEnd(_builder, entryBlock);

        LLVM.BuildRetVoid(_builder); //TODO move to body compiler
        foreach (var annotationGenerator in _config.DeclareAnnotations)
        {
            annotationGenerator(_module, function);
        }

        var externalFunctions = new LLVMValueRef[_config.DeclareExternalFunctions.Length];
        counter = 0;
        foreach (var declarationGenerator in _config.DeclareExternalFunctions)
        {
            externalFunctions[counter] = declarationGenerator(_module);
            counter++;
        }

        var unmanagedString = LLVM.PrintModuleToString(_module) ;
        var kernelString = Marshal.PtrToStringAnsi(unmanagedString);
        return new LLVMKernel(inputKernel.Name, kernelString);
    }

    private LLVMValueRef  GenerateFunction(ParameterInfo[] parameterInfos)
    {
        var paramsListBuilder = new List<LLVMTypeRef>();
        foreach (var paramInfo in parameterInfos)
        {
            LLVMTypeRef type;
            if (paramInfo.ParameterType.IsArray)
            {
                type = LLVMTypeRef.PointerType(paramInfo.ParameterType.ToLLVMType(), 0);
            }
            else
            {
                type = paramInfo.ParameterType.ToLLVMType();
            }
            paramsListBuilder.Add(type);
        }

        var paramType = paramsListBuilder.ToArray();
        return LLVM.AddFunction(_module, _config.KernelName, LLVM.FunctionType(LLVM.VoidType(), paramType, false));

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