namespace CuSharp.Event;

public interface CuEvent : IDisposable
{
    public void Record();
    public float GetDeltaTo(CuEvent secondEvent);
}