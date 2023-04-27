using System.Numerics;
using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace CuSharp;
internal class CudaTensor<T> : Tensor<T>  
{
    internal static Tensor<E[]> FromArrayAllocation<E>(int size) where E : struct
    {
        var devVar = new CudaDeviceVariable<E>(size);
        return new CudaTensor<E[]>() {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar};
    }

    internal static Tensor<E> FromScalarAllocation<E>() where E : struct //TODO Check if necesary
    {
        var devVar = new CudaDeviceVariable<E>(1);
        return new CudaTensor<E> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar};
    }
    internal static Tensor<E[]> FromArray<E>(E[] value) where E : struct
    {
        CudaDeviceVariable<E> devVar = value;
        return new CudaTensor<E[]> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar, Length = value.Length};
    }

    internal static Tensor<E> FromScalar<E>(E value) where E : struct
    {
        if (typeof(E) == typeof(bool))
        {
            CudaDeviceVariable<byte> devVar = Convert.ToByte(value);
            return new CudaTensor<E> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar, Length = 1};
        }
        else
        {
            CudaDeviceVariable<E> devVar = value; 
            return new CudaTensor<E> {DevicePointer = devVar.DevicePointer, DeviceVariable = devVar, Length = 1};           
        }
    }
    internal object DeviceVariable { get; private set; }

    internal CUdeviceptr DevicePointer { get; private set; }
    
    internal int Length { get; private set; }
    
    public void Dispose()
    {
        (DeviceVariable as IDisposable).Dispose();
    }
}