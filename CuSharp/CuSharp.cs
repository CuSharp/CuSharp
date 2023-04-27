using CuSharp.CudaCompiler;
using CuSharp.Event;
using ManagedCuda;

namespace CuSharp;
public static class CuSharp
{

    public static bool EnableOptimizer { get; set; }
    public static string? AotKernelFolder { get; set; } = " ";

    static CuSharp()
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
        
        return new CuDevice(deviceId, new CompilationDispatcher(null, EnableOptimizer), AotKernelFolder);
    }

    public static CuDevice GetDefaultDevice()
    {
        return new CuDevice(compiler: new CompilationDispatcher(null, EnableOptimizer), aotKernelFolder:AotKernelFolder);
    }

    public static CuEvent CreateEvent()
    {
        return new CudaEventWrapper();
    }
    
    private static CudaEvent startEvent;

}