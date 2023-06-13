namespace CuSharp.Tensor;

public delegate void OnUpdate(object arg);

public abstract class Tensor<T> : IDisposable
{
    /*Meant as an opaque Handle*/

    // Implicit conversion for scalar values
    public static implicit operator Tensor<T>(T value)
    {
        if (typeof(T).IsArray)
        {
            throw new Exception("Cannot implicitly convert arrays. Create and allocate arrays with CuDevice.Copy(T[])");
        }

        return new CudaScalar<T>(value);
    }

    public abstract void Dispose();
}