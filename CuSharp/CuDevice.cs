using System.IO;
using System.IO.Compression;
using System.Linq;
using CuSharp.CudaCompiler.Backend;
using ManagedCuda;

namespace CuSharp;
public class CuDevice
{
    private CudaContext cudaDeviceContext; 
    private static KernelDiscovery kernelMethodDiscovery = new KernelDiscovery();
    internal CuDevice(int deviceId = 0)
    {
        cudaDeviceContext = new CudaContext(deviceId);
    }

    public void Launch<T>(PTXKernel kernel, (uint, uint, uint) GridSize, (uint, uint, uint) BlockSize , params T[] parameters) where T : struct
    {
        var cudaKernel = cudaDeviceContext.LoadKernelPTX(kernel.KernelBuffer, kernel.Name);
        cudaKernel.Run(parameters);
    }
    
    public Tensor<T> Copy<T>(T[] hostTensor) where T : struct
    {
        CudaDeviceVariable<T> deviceVariable = hostTensor;
        return new CudaTensor<T>(deviceVariable);
    }

    public T[] Copy<T>(Tensor<T> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaTensor<T>;
        return cudaDeviceTensor.DeviceVariable;
    }
}