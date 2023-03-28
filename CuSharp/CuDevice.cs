using System.Reflection;
using CuSharp.CudaCompiler;
using ManagedCuda;
using ManagedCuda.VectorTypes;

namespace CuSharp;
public partial class CuDevice
{
    private static readonly CompilationDispatcher Compiler = new();
    private readonly CudaContext _cudaDeviceContext;

    public override string ToString()
    {
        return _cudaDeviceContext.GetDeviceName();
    }

    internal CuDevice(int deviceId = 0)
    {
        _cudaDeviceContext = new CudaContext(deviceId);
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

    private CudaKernel CompileAndGetKernel(MethodInfo methodInfo, (uint,uint,uint) gridSize, (uint,uint,uint) blockSize)
    {
        var kernelName = $"{methodInfo.Name}";
        var ptxKernel = Compiler.Compile(kernelName, methodInfo);
        var cudaKernel = _cudaDeviceContext.LoadKernelPTX(ptxKernel.KernelBuffer, kernelName);
        cudaKernel.GridDimensions = new dim3(gridSize.Item1, gridSize.Item2, gridSize.Item3);
        cudaKernel.BlockDimensions = new dim3(blockSize.Item1, blockSize.Item2, blockSize.Item3);
        return cudaKernel;
    }

}