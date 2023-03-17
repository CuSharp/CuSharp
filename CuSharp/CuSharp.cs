using System.Collections.Generic;
using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Backend;
using ManagedCuda;

namespace CuSharp;
public static class CuSharp
{

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
            /*throw*/
        }
        
        return new CuDevice(deviceId);
    }

    public static CuDevice GetDefaultDevice()
    {
        return new CuDevice();
    }

    private static CudaEvent startEvent;
    public static void  StartTimer()
    {
        startEvent = new CudaEvent();
        startEvent.Record();
    }

    public static float GetTimeMS()
    {
        var cuEvent = new CudaEvent();
        cuEvent.Record();
        cuEvent.Synchronize();
        return CudaEvent.ElapsedTime(startEvent, cuEvent);
    }

}