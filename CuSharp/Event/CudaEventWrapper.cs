using ManagedCuda;

namespace CuSharp.Event;

public class CudaEventWrapper : CuEvent
{
    private readonly CudaEvent _event = new();

    public void Dispose()
    {
        _event.Dispose();
    }

    public void Record()
    {
        _event.Record();
    }

    public float GetDeltaTo(CuEvent secondEvent)
    {
        var secondCudaEvent = secondEvent as CudaEventWrapper;
        secondCudaEvent._event.Synchronize();
        return CudaEvent.ElapsedTime(_event, secondCudaEvent._event);
    }
}