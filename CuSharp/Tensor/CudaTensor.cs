using System.Numerics;
using ManagedCuda;

namespace CuSharp;
public class CudaTensor<T> : Tensor<T> where T : struct
{
    public CudaTensor(CudaDeviceVariable<T> cudaDeviceVariable)
    {
        DeviceVariable = cudaDeviceVariable;
    }
    public CudaDeviceVariable<T>  DeviceVariable { get; }
}