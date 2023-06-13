namespace CuSharp.Tensor;

public class CudaScalar<T> : Tensor<T>
{

    internal CudaScalar(T value)
    {
        Value = value;
    }

    internal T Value { get; private set; }

    public override void Dispose()
    {
        // Nothing to dispose
    }
}