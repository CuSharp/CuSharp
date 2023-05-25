using System.Reflection;
using System.Runtime.InteropServices;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Kernels;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;

namespace CuSharp;
public partial class CuDevice : IDisposable
{
    private readonly CudaContext _cudaDeviceContext;
    private readonly MethodLauncher _launcher;
    
    internal CuDevice(int deviceId = 0, CompilationDispatcher? compiler = null, string aotKernelFolder = CuSharp.PathNotSet)
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
        return new CudaVector<T>(size);
    }
    
    public Tensor<T[,]> Allocate<T>(int sizeX, int sizeY) where T : struct
    {
        return new CudaMatrix<T>(sizeX, sizeY);
    }
    
    public Tensor<T[]> Copy<T>(T[] hostTensor) where T : struct
    {
        return new CudaVector<T>(hostTensor);
    }
    public Tensor<T[,]> Copy<T>(T[,] hostTensor) where T : struct
    {
        return new CudaMatrix<T>(hostTensor);
    }
    
    public Tensor<T> CreateScalar<T>(T hostScalar) where T : struct
    {
        return new CudaScalar<T>(hostScalar);
    }
    
    public T[] Copy<T>(Tensor<T[]> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaVector<T>;
        return  cudaDeviceTensor.DeviceVariable as CudaDeviceVariable<T>;
    }

    public T[,] Copy<T>(Tensor<T[,]> deviceTensor) where T : struct
    {
        var cudaDeviceTensor = deviceTensor as CudaMatrix<T>;
        SizeT[] matrixDevPtr = cudaDeviceTensor.MatrixDeviceVariable;
        T[] flatVector = cudaDeviceTensor.FlatDeviceVariable;
        T[,] matrix = new T[matrixDevPtr.Length, flatVector.Length / matrixDevPtr.Length];
        Buffer.BlockCopy(flatVector, 0, matrix,0, flatVector.Length * Marshal.SizeOf(typeof(T)));
        return matrix;
    }
    public void Dispose()
    {
        _cudaDeviceContext.Dispose();
    }
}