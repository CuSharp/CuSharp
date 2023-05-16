using CuSharp.Kernel;

namespace CuSharp.Tests.TestHelper;

public class RecursiveKernels
{
    [Kernel]
    public static void FibonacciLauncher(int[] results, int n)
    {
        Fibonacci(results, n);
    }
    public static int Fibonacci(int[] results, int n) {
        if (n <= 2)
        {
            results[n] = 1;
            return 1;
        }
        results[n-1] = Fibonacci(results, n-1) + Fibonacci(results, n-2);
        return results[n - 1];
    }
    
}