using System.Reflection;
using CuSharp.CudaCompiler;
using ManagedCuda;
using ManagedCuda.VectorTypes;

namespace CuSharp;
public partial class CuDevice
{
    
    private static KernelDiscovery _discovery = new KernelDiscovery();
    private static CompilationDispatcher _compiler = new CompilationDispatcher();
    private CudaContext _cudaDeviceContext; 
    private static KernelDiscovery _kernelMethodDiscovery = new KernelDiscovery();

    public string ToString()
    {
        return _cudaDeviceContext.GetDeviceName();
    }
    
    internal CuDevice(int deviceId = 0)
    {
        _cudaDeviceContext = new CudaContext(deviceId);
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

    private CudaKernel compileAndGetKernel(MethodInfo methodInfo, (uint,uint,uint) GridSize, (uint,uint,uint) BlockSize)
    {
        var kernelName = $"{methodInfo.Name}";
        var ptxKernel = _compiler.Compile(kernelName, methodInfo);
        var cudaKernel = _cudaDeviceContext.LoadKernelPTX(ptxKernel.KernelBuffer, kernelName);
        cudaKernel.GridDimensions = new dim3(GridSize.Item1, GridSize.Item2, GridSize.Item3);
        cudaKernel.BlockDimensions = new dim3(BlockSize.Item1, BlockSize.Item2, BlockSize.Item3);
        return cudaKernel;
    }

}