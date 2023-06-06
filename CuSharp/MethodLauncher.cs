using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.Kernel;
using ManagedCuda;
using ManagedCuda.VectorTypes;

namespace CuSharp;

internal class MethodLauncher
{
    private readonly string _aotKernelFolder;
    private readonly CudaContext _cudaDeviceContext;
    private readonly CompilationDispatcher _compiler; 
    private readonly Dictionary<string, CudaKernel> _cache = new ();

    internal MethodLauncher(string aotKernelFolder, CompilationDispatcher compiler,CudaContext cudaDeviceContext)
    {
        _aotKernelFolder = aotKernelFolder;
        _cudaDeviceContext = cudaDeviceContext;
        _compiler = compiler;
    }
    
    internal void CompileAndLaunch(MethodInfo method, (uint, uint, uint) gridDimension, (uint, uint, uint) blockDimension,
        params object[] parameters)
    {
        var cudaKernel = CompileAndGetKernel(method, gridDimension, blockDimension);
        var lengths = new int [parameters.Length];
        CudaDeviceVariable<int> devLengths = lengths;
        var castParams = parameters
            .Select(p => p.GetType()
                             .GetProperty("DevicePointer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(p) ?? 
                            p.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(p))
            .ToArray();
        try
        {
            cudaKernel.Run(castParams);
        }
        catch (Exception e)
        {
            e.HandleUnrecoverable("Something went wrong during execution of a GPU kernel. Please check your kernel and/or report a bug on https://github.com/CuSharp/CuSharp");
        }
    }
    
    private CudaKernel CompileAndGetKernel(MethodInfo methodInfo, (uint,uint,uint) gridDimension, (uint,uint,uint) blockDimension)
    {
        if (_cache.TryGetValue(KernelHelpers.GetMethodIdentity(methodInfo), out var k))
        {
            SetGridDimensions(k, gridDimension, blockDimension);
            return k;
        } 

        CudaKernel cudaKernel = GetCompiledKernel(methodInfo, gridDimension, blockDimension);
        
        _cache.Add(KernelHelpers.GetMethodIdentity(methodInfo), cudaKernel);
        SetGridDimensions(cudaKernel, gridDimension, blockDimension);
        return cudaKernel;
    }

    private const string ValidMethodNameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_$";
    private CudaKernel GetCompiledKernel(MethodInfo method, (uint,uint,uint) gridDimension, (uint,uint,uint) blockDimension)
    {
        var kernelName =  new string(method.Name.Select(c => ValidMethodNameChars.Contains(c) ? c : '$').ToArray());
        if (HasPrecompiledKernel(method))
        {
            return GetPrecompiledKernel(method, kernelName, gridDimension, blockDimension);
        }
        return GetJITCompiledKernel(method, kernelName, gridDimension, blockDimension);
    }
    private bool HasPrecompiledKernel(MethodInfo method)
    {
        if (_aotKernelFolder == Cu.PathNotSet)
        {
            return false;
        }
        string fileName = KernelHelpers.GetMethodIdentity(method) + "ptx";
        return File.Exists($"{_aotKernelFolder}{Path.DirectorySeparatorChar}{fileName}");
    }

    private CudaKernel GetPrecompiledKernel(MethodInfo method,string kernelName, (uint,uint,uint) gridDimension, (uint,uint,uint) blockDimension) 
    {
        string fileName = KernelHelpers.GetMethodIdentity(method) + "ptx";
        byte[] bytes = File.ReadAllBytes($"{_aotKernelFolder}{Path.DirectorySeparatorChar}{fileName}");
        return _cudaDeviceContext.LoadKernelPTX(bytes, kernelName);
    }

    private CudaKernel GetJITCompiledKernel(MethodInfo method, string kernelName, (uint, uint, uint) gridDimension,
        (uint, uint, uint) blockDimension)
    {
        var nnvmConfiguration = CompilationConfiguration.NvvmConfiguration;
        var attributes = method.GetCustomAttributes(typeof(KernelAttribute)).ToList();
        
        if (attributes is { Count: 1 })
            nnvmConfiguration.DefaultArrayMemoryLocation = ((KernelAttribute)attributes[0]).ArrayMemoryLocation;

        var ptxKernel = _compiler.Compile(kernelName, method, nnvmConfiguration);
        return _cudaDeviceContext.LoadKernelPTX(ptxKernel.KernelBuffer, kernelName);   
    }

    private void SetGridDimensions(CudaKernel kernel, (uint, uint, uint) gridDimension, (uint, uint, uint) blockDimension)
    {
        kernel.GridDimensions = new dim3(gridDimension.Item1, gridDimension.Item2, gridDimension.Item3);
        kernel.BlockDimensions = new dim3(blockDimension.Item1, blockDimension.Item2, blockDimension.Item3);
    }
}