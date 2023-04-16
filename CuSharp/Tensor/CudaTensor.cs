using System.Numerics;
using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace CuSharp;
public class CudaTensor<T> : Tensor<T>  
{
    public static Tensor<E[]> FromArray<E>(E[] value) where E : struct
    {
        CudaDeviceVariable<E> devVar = value;
        
        return new CudaTensor<E[]> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar, Length = value.Length};
    }

    public static Tensor<E> FromScalar<E>(E value) where E : struct
    {
        CudaDeviceVariable<E> devVar = value;
        return new CudaTensor<E> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar, Length = 1};
    }
    public object DeviceVariable { get; private set; }

    public CUdeviceptr DevicePointer { get; private set; }
    public int Length { get; private set; }
    public void Dispose()
    {
        (DeviceVariable as IDisposable).Dispose();
    }
}