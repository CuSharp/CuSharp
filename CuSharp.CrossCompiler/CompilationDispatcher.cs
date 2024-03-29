﻿using System.Reflection;
using System.Text;
using CuSharp.CudaCompiler.Compiler;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using LibNVVMBinder;

namespace CuSharp.CudaCompiler;

public class CompilationDispatcher
{
    private readonly Dictionary<string, PTXKernel> _kernelCache;
    private readonly bool _enableOptimizer;
    private readonly bool _disableCaching;

    public CompilationDispatcher(Dictionary<string, PTXKernel>? kernelCache = null, bool enableOptimizer = false, bool disableCaching = false)
    {
        _kernelCache = kernelCache ?? new Dictionary<string, PTXKernel>();
        _enableOptimizer = enableOptimizer;
        _disableCaching = disableCaching;
    }
    public PTXKernel Compile(string kernelName, MethodInfo methodInfo, CompilationConfiguration? nvvmConfiguration = null)
    {
        PTXKernel ptxKernel = null!;
        if (!_disableCaching && _kernelCache.TryGetValue(KernelHelpers.GetMethodIdentity(methodInfo), out ptxKernel!)) return ptxKernel;
 
        var kernel = new MSILKernel(kernelName, methodInfo);
        nvvmConfiguration ??= CompilationConfiguration.NvvmConfiguration;
        nvvmConfiguration.KernelName = kernelName;
        
        var msilToLlvmCrosscompiler = new KernelCrossCompiler(nvvmConfiguration);
        LLVMKernel? llvmKernel = null;
        try
        {
            llvmKernel = msilToLlvmCrosscompiler.Compile(kernel, _enableOptimizer);
        }
        catch (Exception e)
        {
            e.HandleUnrecoverable("Something went wrong while compiling MSIL to NVVM IR. This may occur when using unsupported features in the kernel.");
        }

        try
        {
            ptxKernel = CompileLlvmToPtx(llvmKernel!);
        }
        catch (Exception e)
        {
            e.HandleUnrecoverable("Something went wrong while compiling NVVM IR to PTX ISA. This is most likely a bug in the compiler. Please report this on http://github.com/CuSharp/CuSharp.");
        }
        
        _kernelCache.Add(KernelHelpers.GetMethodIdentity(methodInfo), ptxKernel);
        return ptxKernel;
    }

    private static PTXKernel CompileLlvmToPtx(LLVMKernel llvmKernel)
    {
        var nvvmHandle = new NVVMProgram();
        nvvmHandle.AddModule(llvmKernel.KernelBuffer, llvmKernel.Name);
        
        #if DEBUG
            var verifyResult = nvvmHandle.Verify(Array.Empty<string>());
            if (verifyResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
            {
                nvvmHandle.GetProgramLog(out string log);
                throw new Exception(log);
            }
        #endif 
        
        var compilationResult = nvvmHandle.Compile(new string[]{});
        if (compilationResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
        {
            nvvmHandle.GetProgramLog(out string log);
            throw new Exception(log);
        }

        nvvmHandle.GetCompiledResult(out string ptx);
        return new PTXKernel(llvmKernel.Name, Encoding.UTF8.GetBytes(ptx));
    }
   
}