using System.Numerics;
using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace CuSharp;
public class CudaTensor<T> : Tensor<T>  
{
    public static Tensor<E[]> FromArray<E>(E[] value) where E : struct
    {
        CudaDeviceVariable<E> devVar = value;
        return new CudaTensor<E[]> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar};
    }

    public static Tensor<E> FromScalar<E>(E value) where E : struct
    {
        CudaDeviceVariable<E> devVar = value;
        return new CudaTensor<E> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar};
    }
    public object DeviceVariable { get; private set; }

    public CUdeviceptr DevicePointer { get; private set; }
    public void Dispose()
    {
        (DeviceVariable as IDisposable).Dispose();
    }
}