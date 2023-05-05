using System.Security.Cryptography;

namespace CuSharp;

public class CudaScalar<T> : Tensor<T> 
{

    internal CudaScalar(T value)
    {
        Value = value;
    }
    internal T Value { get; private set; }
    
    public override void Dispose()
    {
        //nothing to dispose
    }
}