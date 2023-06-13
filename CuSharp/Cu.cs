using CuSharp.CudaCompiler;
using CuSharp.Event;
using ManagedCuda;

namespace CuSharp;

public static class Cu
{

    internal const string PathNotSet = "/../";
    public static bool EnableOptimizer { get; set; }
    public static string? AotKernelFolder { get; set; } = PathNotSet;

    static Cu()
    {
        #if DEBUG
            EnableOptimizer = false;
        #else
            EnableOptimizer = true;
        #endif
    }
    public static IEnumerator<(int, string)> GetDeviceList()
    {
        var count = CudaContext.GetDeviceCount();
        for (var i = 0; i < count; i++)
        {
            yield return (i, CudaContext.GetDeviceName(i));
        }
    }

    public static CuDevice GetDeviceById(int deviceId)
    {
        if (deviceId >= CudaContext.GetDeviceCount())
        {
            throw new ArgumentException("Device ID does not exist");
        }
        
        //caching disabled because CuDevice caches
        return new CuDevice(deviceId, new CompilationDispatcher(null, EnableOptimizer, true), AotKernelFolder);
    }

    public static CuDevice GetDefaultDevice()
    {
        //caching disabled because CuDevice caches 
        return new CuDevice(compiler: new CompilationDispatcher(null, EnableOptimizer, true), aotKernelFolder:AotKernelFolder);
    }

    public static ICuEvent CreateEvent()
    {
        return new CudaEventWrapper();
    }
}