using System.Reflection;
using System.Text;
using CuSharp.CudaCompiler.Backend;
using CuSharp.CudaCompiler.Frontend;
using LibNVVMBinder;

namespace CuSharp.CudaCompiler;
public class CompilationDispatcher
{
    private readonly Dictionary<string, PTXKernel> _kernelCache;

    public CompilationDispatcher(Dictionary<string, PTXKernel>? kernelCache = null)
    {
        _kernelCache = kernelCache ?? new Dictionary<string, PTXKernel>();
    }
    public PTXKernel Compile(string kernelName, MethodInfo methodInfo)
    {
        if (_kernelCache.TryGetValue(GetMethodIdentity(methodInfo), out var ptxKernel)) return ptxKernel;
        
        var kernel = new MSILKernel(kernelName, methodInfo);
        var nvvmConfiguration = CompilationConfiguration.NvvmConfiguration;
        nvvmConfiguration.KernelName = kernelName;
        
        var msilToLlvmCrosscompiler = new KernelCrossCompiler(nvvmConfiguration);
        var llvmKernel = msilToLlvmCrosscompiler.Compile(kernel);
        ptxKernel = CompileLlvmToPtx(llvmKernel);
        _kernelCache.Add(GetMethodIdentity(methodInfo), ptxKernel);
        return ptxKernel;
    }

    private string GetMethodIdentity(MethodInfo method)
    {
        string paramString = "";
        foreach(var param in method.GetParameters())
        {
            paramString += param.ParameterType + ";";
        }
        return $"{method.DeclaringType.FullName}.{method.Name}:{paramString}";
    }
    private static PTXKernel CompileLlvmToPtx(LLVMKernel llvmKernel)
    {
        var nvvmHandle = new NVVMProgram();
        nvvmHandle.AddModule(llvmKernel.KernelBuffer, llvmKernel.Name);
        var verifyResult = nvvmHandle.Verify(new string[0]);
        if (verifyResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
        {
            nvvmHandle.GetProgramLog(out string log);
            throw new Exception(log);
        }
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