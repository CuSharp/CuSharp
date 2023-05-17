﻿using System.Numerics;
using System.Runtime.InteropServices;
using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace CuSharp;
internal class CudaVector<T> : Tensor<T[]> where T : struct
{
    internal CudaVector(int size)
    {
        var devVar = new CudaDeviceVariable<T>(size);
        DevicePointer = devVar.DevicePointer;
        DeviceVariable = devVar;
        Length = size;
    }
    
    internal CudaVector(T[] value) 
    {
        CudaDeviceVariable<T> devVar = value;
        DevicePointer = devVar.DevicePointer;
        DeviceVariable = devVar;
        Length = value.Length;
    }

    internal object DeviceVariable { get; private set; }

    internal CUdeviceptr DevicePointer { get; private set; } 
    
    internal int Length { get; private set; }
    
    public override void Dispose()
    {
        (DeviceVariable as IDisposable).Dispose();
    }
}