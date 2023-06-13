namespace CuSharp.Event;

public interface ICuEvent : IDisposable
{
    public void Record();
    public float GetDeltaTo(ICuEvent secondEvent);
}