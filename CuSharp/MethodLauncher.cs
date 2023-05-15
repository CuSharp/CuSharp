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
    
    internal void CompileAndLaunch(MethodInfo method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize,
        params object[] parameters)
    {
        var cudaKernel = CompileAndGetKernel(method, gridSize, blockSize);
        var lengths = new int [parameters.Length];
        CudaDeviceVariable<int> devLengths = lengths;
        var castParams = parameters
            .Select(p => p.GetType()
                             .GetProperty("DevicePointer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(p) ?? 
                            p.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(p))
            .Append(devLengths.DevicePointer)
            .ToArray();
        cudaKernel.Run(castParams);
    }
    
    private CudaKernel CompileAndGetKernel(MethodInfo methodInfo, (uint,uint,uint) gridSize, (uint,uint,uint) blockSize)
    {
        if (_cache.TryGetValue(KernelHelpers.GetMethodIdentity(methodInfo), out var k))
        {
            SetGridDimensions(k, gridSize, blockSize);
            return k;
        } 

        CudaKernel cudaKernel = GetCompiledKernel(methodInfo, gridSize, blockSize);
        
        _cache.Add(KernelHelpers.GetMethodIdentity(methodInfo), cudaKernel);
        SetGridDimensions(cudaKernel, gridSize, blockSize);
        return cudaKernel;
    }

    private CudaKernel GetCompiledKernel(MethodInfo method, (uint,uint,uint) gridSize, (uint,uint,uint) blockSize)
    {
        if (HasPrecompiledKernel(method))
        {
            return GetPrecompiledKernel(method, method.Name, gridSize, blockSize);
        }
        return GetJITCompiledKernel(method, method.Name, gridSize, blockSize);
    }
    private bool HasPrecompiledKernel(MethodInfo method)
    {
        if (_aotKernelFolder == CuSharp.PathNotSet)
        {
            return false;
        }
        string fileName = KernelHelpers.GetMethodIdentity(method) + "ptx";
        return File.Exists($"{_aotKernelFolder}{Path.DirectorySeparatorChar}{fileName}");
    }

    private CudaKernel GetPrecompiledKernel(MethodInfo method,string kernelName, (uint,uint,uint) gridSize, (uint,uint,uint) blockSize) 
    {
        string fileName = KernelHelpers.GetMethodIdentity(method) + "ptx";
        byte[] bytes = File.ReadAllBytes($"{_aotKernelFolder}{Path.DirectorySeparatorChar}{fileName}");
        return _cudaDeviceContext.LoadKernelPTX(bytes, kernelName);
    }

    private CudaKernel GetJITCompiledKernel(MethodInfo method, string kernelName, (uint, uint, uint) gridSize,
        (uint, uint, uint) blockSize)
    {
        var nnvmConfiguration = CompilationConfiguration.NvvmConfiguration;
        var attributes = method.GetCustomAttributes(typeof(KernelAttribute)).ToList();
        
        if (attributes is { Count: 1 })
            nnvmConfiguration.DefaultArrayMemoryLocation = ((KernelAttribute)attributes[0]).ArrayMemoryLocation;

        var ptxKernel = _compiler.Compile(kernelName, method, nnvmConfiguration);
        return _cudaDeviceContext.LoadKernelPTX(ptxKernel.KernelBuffer, kernelName);   
    }

    private void SetGridDimensions(CudaKernel kernel, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize)
    {
        kernel.GridDimensions = new dim3(gridSize.Item1, gridSize.Item2, gridSize.Item3);
        kernel.BlockDimensions = new dim3(blockSize.Item1, blockSize.Item2, blockSize.Item3);
    }
}