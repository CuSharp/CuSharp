using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CuSharp.CudaCompiler.Backend;
using CuSharp.CudaCompiler.Frontend;
using LibNVVMBinder;
using LLVMSharp;

namespace CuSharp.CudaCompiler;
public class CompilationDispatcher
{
    private readonly Dictionary<int, PTXKernel> _kernelCache;

    public CompilationDispatcher(Dictionary<int, PTXKernel>? kernelCache = null)
    {
        _kernelCache = kernelCache ?? new Dictionary<int, PTXKernel>();
    }
    public PTXKernel Compile(string kernelName, MethodInfo methodInfo)
    {
        if (_kernelCache.TryGetValue(methodInfo.GetHashCode(), out var ptxKernel)) return ptxKernel;
        
        var kernel = new MSILKernel(kernelName, methodInfo);
        var nvvmConfiguration = CompilationConfiguration.NvvmConfiguration;
        nvvmConfiguration.KernelName = kernelName;
        
        var msilToLlvmCrosscompiler = new KernelCrossCompiler(nvvmConfiguration);
        var llvmKernel = msilToLlvmCrosscompiler.Compile(kernel);
        ptxKernel = CompileLlvmToPtx(llvmKernel);
        _kernelCache.Add(methodInfo.GetHashCode(), ptxKernel);
        return ptxKernel;
    }

    private static PTXKernel CompileLlvmToPtx(LLVMKernel llvmKernel)
    {
        var nvvmHandle = new NVVMProgram();
        nvvmHandle.AddModule(llvmKernel.KernelBuffer, llvmKernel.Name);
        var compilationResult = nvvmHandle.Compile(new string[0]);
        if (compilationResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
        {
            nvvmHandle.GetProgramLog(out string log);
            throw new Exception(log);
        }

        nvvmHandle.GetCompiledResult(out string ptx);
        return new PTXKernel(llvmKernel.Name, Encoding.UTF8.GetBytes(ptx));
    }
   
}