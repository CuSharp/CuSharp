using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Kernels;
using ManagedCuda;
using ManagedCuda.VectorTypes;

namespace CuSharp;
public partial class CuDevice : IDisposable
{
    private readonly CudaContext _cudaDeviceContext;
    private readonly MethodLauncher _launcher;
    
    internal CuDevice(int deviceId = 0, CompilationDispatcher? compiler = null, string aotKernelFolder = " ")
    {
        
        _cudaDeviceContext = new CudaContext(deviceId);
        _launcher = new MethodLauncher(aotKernelFolder, compiler, _cudaDeviceContext);
    }
    public override string ToString()
    {
        return _cudaDeviceContext.GetDeviceName();
    }

    public void Synchronize()
    {
        _cudaDeviceContext.Synchronize();
    }

    public Tensor<T[]> Allocate<T>(int size) where T : struct
    {
        return CudaTensor<T[]>.FromArrayAllocation<T>(size);
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

    public bool Copy(Tensor<bool> deviceTensor)
    {
        var cudaDeviceTensor = deviceTensor as CudaTensor<bool>;
        return Convert.ToBoolean(cudaDeviceTensor.DeviceVariable as CudaDeviceVariable<byte>);
    }
    /// <summary>
    /// This copies a scalar value back. Potentially, race conditions can occur.
    /// </summary>
    public T Copy<T>(Tensor<T> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaTensor<T>;
        return cudaDeviceTensor.DeviceVariable as CudaDeviceVariable<T>;
    }

    public void Dispose()
    {
        _cudaDeviceContext.Dispose();
    }
}