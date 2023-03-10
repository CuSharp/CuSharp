using System.Collections.Generic;
using CuSharp.CudaCompiler;
using ManagedCuda;

namespace CuSharp;
public static class CuSharp
{
    private static KernelDiscovery discovery = new KernelDiscovery();
    private static CompilationDispatcher compiler = new CompilationDispatcher();

    static CuSharp()
    {
        discovery.ScanAll();
    }
    
    public static IEnumerator<(int, string)> GetDeviceList()
    {
        var count = CudaContext.GetDeviceCount();
        for (int i = 0; i < count; i++)
        {
            yield return (i, CudaContext.GetDeviceName(i));
        }
    }

    public static CuDevice GetDeviceById(int deviceId)
    {
        if (deviceId >= CudaContext.GetDeviceCount())
        {
            /*throw*/
        }
        
        return new CuDevice(deviceId);
    }

    public static CuDevice GetDefaultDevice()
    {
        return new CuDevice();
    }
    
    public static PTXKernel CompileKernel(string methodName)
    {
        if (!discovery.IsMarked(methodName))
        {
            /*throw*/ 
        }

        return compiler.Compile(methodName, discovery.GetMethod(methodName));
    }
}