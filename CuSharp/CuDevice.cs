using System.Reflection;
using CuSharp.CudaCompiler;
using ManagedCuda;
using ManagedCuda.VectorTypes;

namespace CuSharp;
public partial class CuDevice : IDisposable
{
    private readonly CompilationDispatcher Compiler; 
    private readonly CudaContext _cudaDeviceContext;

    internal CuDevice(int deviceId = 0, CompilationDispatcher? compiler = null)
    {
        _cudaDeviceContext = new CudaContext(deviceId);
        if (compiler == null)
        {
            Compiler = new CompilationDispatcher();
        }
        else
        {
            Compiler = compiler;
        }
    }
    public override string ToString()
    {
        return _cudaDeviceContext.GetDeviceName();
    }

    public void Synchronize()
    {
        _cudaDeviceContext.Synchronize();
    }
    public Tensor<T[]> Copy<T>(T[] hostTensor) where T : struct
    {
        return CudaTensor<T[]>.FromArray(hostTensor);
    }

    public Tensor<T> Copy<T>(T hostScalar) where T : struct
    {
        return CudaTensor<T>.FromScalar(hostScalar);
    }
    
    public T[] Copy<T>(Tensor<T[]> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaTensor<T[]>;
        return  cudaDeviceTensor.DeviceVariable as CudaDeviceVariable<T>;
    }

    /// <summary>
    /// This copies a scalar value back. Potentially, race conditions can occur.
    /// </summary>
    public T Copy<T>(Tensor<T> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaTensor<T>;
        return cudaDeviceTensor.DeviceVariable as CudaDeviceVariable<T>;
    }

    private Dictionary<string, CudaKernel> _cache = new ();
    private CudaKernel CompileAndGetKernel(MethodInfo methodInfo, (uint,uint,uint) gridSize, (uint,uint,uint) blockSize)
    {
        if (_cache.TryGetValue(GetMethodIdentity(methodInfo), out var k))
        {
            k.GridDimensions = new dim3(gridSize.Item1, gridSize.Item2, gridSize.Item3);
            k.BlockDimensions = new dim3(blockSize.Item1, blockSize.Item2, blockSize.Item3);
            return k;
        } 
        var kernelName = $"{methodInfo.Name}";
        var ptxKernel = Compiler.Compile(kernelName, methodInfo);
        var cudaKernel = _cudaDeviceContext.LoadKernelPTX(ptxKernel.KernelBuffer, kernelName);
        _cache.Add(GetMethodIdentity(methodInfo), cudaKernel);
        cudaKernel.GridDimensions = new dim3(gridSize.Item1, gridSize.Item2, gridSize.Item3);
        cudaKernel.BlockDimensions = new dim3(blockSize.Item1, blockSize.Item2, blockSize.Item3);
        return cudaKernel;
    }
    
    private static string GetMethodIdentity(MethodInfo method)
    {
        string paramString = "";
        foreach(var param in method.GetParameters())
        {
            paramString += param.ParameterType + ";";
        }
        return $"{method.DeclaringType.FullName}.{method.Name}:{paramString}";
    }

    public void Dispose()
    {
        _cudaDeviceContext.Dispose();
    }
}