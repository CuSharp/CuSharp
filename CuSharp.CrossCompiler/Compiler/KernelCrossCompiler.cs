using System.Runtime.InteropServices;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Compiler;

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

    public LLVMKernel Compile(MSILKernel entryKernel, bool optimize = false)
    {
        GenerateDataLayoutAndTarget();
        
        var functionGenerator = new FunctionGenerator(_module, _builder);
        var intrinsicFunctions = GenerateDeviceIntrinsicFunctions();
        
        var entryFunction = functionGenerator.GetOrDeclareFunction(entryKernel);
        var functionsDto = new FunctionsDto(entryFunction, intrinsicFunctions);


        foreach (var current in functionGenerator.AllFunctionsToCompile())
        {
            functionsDto.Function = current.llvmFunction;
            new MethodBodyCompiler(current.msilFunction, _builder, functionsDto, functionGenerator)
                {
                    Module = _module,
                    ArrayMemoryLocation = _config.DefaultArrayMemoryLocation
                }
                .CompileMethodBody(); //TODO change module input

            if (optimize) RunOptimization(current.llvmFunction);
        }

        GenerateAnnotations(entryFunction);
        return new LLVMKernel(entryKernel.Name, GetModuleAsString());
    }

    private void RunOptimization(LLVMValueRef function)
    {
        //FROM: https://github.com/dotnet/LLVMSharp/blob/main/samples/KaleidoscopeTutorial/Chapter4/KaleidoscopeLLVM/Program.cs
        //FROM: https://llvm.org/docs/NewPassManager.html
        
        LLVMPassManagerRef passManager = LLVM.CreateFunctionPassManagerForModule(_module);
        
        LLVM.AddBasicAliasAnalysisPass(passManager);
        LLVM.AddPromoteMemoryToRegisterPass(passManager);
        LLVM.AddInstructionCombiningPass(passManager);
        LLVM.AddReassociatePass(passManager);
        LLVM.AddCFGSimplificationPass(passManager);
        LLVM.AddGVNPass(passManager);
        
        LLVM.InitializeFunctionPassManager(passManager);
        LLVM.RunFunctionPassManager(passManager, function);
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

    private (string, LLVMValueRef, LLVMValueRef[])[] GenerateDeviceIntrinsicFunctions()
    {
        var externalFunctions = new (string, LLVMValueRef, LLVMValueRef[])[_config.DeclareExternalFunctions.Length];
        var counter = 0;
        foreach (var declarationGenerator in _config.DeclareExternalFunctions)
        {
            externalFunctions[counter] = declarationGenerator(_module);
            counter++;
        }

        return externalFunctions;
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