using System.Numerics;
using System.Runtime.InteropServices;
using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace CuSharp;
internal class CudaMatrix<T> : Tensor<T[,]> where T : struct
{
    internal CudaMatrix(int sizeX, int sizeY)
    {
        FlatDeviceVariable = new CudaDeviceVariable<T>(sizeX * sizeY);
        
        var vectorPointers = new SizeT [sizeX];
        for (int i = 0; i < vectorPointers.Length; i++)
        {
            vectorPointers[i] = i * sizeY * Marshal.SizeOf(typeof(T));
        }

        MatrixDeviceVariable = vectorPointers;
    }
    
    internal CudaMatrix(T[,] value)
    {
        var flatVector = new T[value.Length];
        Buffer.BlockCopy(value,0,flatVector,0, value.Length * Marshal.SizeOf(typeof(T))); 
        FlatDeviceVariable = flatVector;

        
        var vectorPointers = new SizeT[value.GetLength(0)];
        for (int i = 0; i < vectorPointers.Length; i++)
        {
            vectorPointers[i] = FlatDeviceVariable.DevicePointer.Pointer + i * value.GetLength(1) * Marshal.SizeOf(typeof(T));
        }

        MatrixDeviceVariable = vectorPointers;
        DevicePointer = MatrixDeviceVariable.DevicePointer;
    }

    internal CudaDeviceVariable<T> FlatDeviceVariable { get; private set; }

    internal CudaDeviceVariable<SizeT> MatrixDeviceVariable { get; private set; }

    internal CUdeviceptr DevicePointer { get; private set; }

    public override void Dispose()
    {
        (FlatDeviceVariable as IDisposable).Dispose();
        (MatrixDeviceVariable as IDisposable).Dispose();
    }
}