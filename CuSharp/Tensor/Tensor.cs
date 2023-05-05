using System.Numerics;

namespace CuSharp;

public delegate void OnUpdate(object arg);
public abstract class Tensor<T> : IDisposable
{ /*Meant as an opaque Handle*/

    //implicit conversion for scalar values
    public int fib(int n)
    {
        if (n < 3)
        {
            return 1;
        }

        return fib(n - 1) + fib(n - 2);
    }

    public int fib2(int n)
    {
        int x = 0;
        int y = 1;
        int z = 0;
        for (int i = 0; i < n; i++)
        {
            z = x + y;
            x = y;
            y = z;
        }

        return z;
    }
    
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