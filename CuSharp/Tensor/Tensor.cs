using System.Numerics;

namespace CuSharp;

public delegate void OnUpdate(object arg);
public abstract class Tensor<T> : IDisposable
{ /*Meant as an opaque Handle*/

    //implicit conversion for scalar values
    public static implicit operator Tensor<T>(T value)
    {
        return new CudaScalar<T>(value);
        if (typeof(T).IsArray)
        {
        }
        else
        {
        }
    }

    public abstract void Dispose();
}