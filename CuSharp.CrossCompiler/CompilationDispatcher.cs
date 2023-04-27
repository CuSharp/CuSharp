using System.Reflection;
using System.Text;
using CuSharp.CudaCompiler.Backend;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.CudaCompiler.Kernels;
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
    public PTXKernel Compile(string kernelName, MethodInfo methodInfo)
    {
        if ((!_disableCaching) && _kernelCache.TryGetValue(KernelHelpers.GetMethodIdentity(methodInfo), out var ptxKernel)) return ptxKernel;

        var kernel = new MSILKernel(kernelName, methodInfo, true);
        var nvvmConfiguration = CompilationConfiguration.NvvmConfiguration;
        nvvmConfiguration.KernelName = kernelName;
        
        var msilToLlvmCrosscompiler = new KernelCrossCompiler(nvvmConfiguration);
        var llvmKernel = msilToLlvmCrosscompiler.Compile(kernel, _enableOptimizer);
        ptxKernel = CompileLlvmToPtx(llvmKernel);
        _kernelCache.Add(KernelHelpers.GetMethodIdentity(methodInfo), ptxKernel);
        return ptxKernel;
    }

    private static PTXKernel CompileLlvmToPtx(LLVMKernel llvmKernel)
    {
        var nvvmHandle = new NVVMProgram();
        nvvmHandle.AddModule(llvmKernel.KernelBuffer, llvmKernel.Name);
#if DEBUG
        var verifyResult = nvvmHandle.Verify(new string[0]);
        if (verifyResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
        {
            nvvmHandle.GetProgramLog(out string log);
            throw new Exception(log);
        }
#endif 
        var compilationResult = nvvmHandle.Compile(new string[]{"-fma=1"});
        if (compilationResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
        {
            nvvmHandle.GetProgramLog(out string log);
            throw new Exception(log);
        }

        nvvmHandle.GetCompiledResult(out string ptx);
        return new PTXKernel(llvmKernel.Name, Encoding.UTF8.GetBytes(ptx));
    }
   
}