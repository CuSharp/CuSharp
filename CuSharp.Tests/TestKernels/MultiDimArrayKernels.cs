using CuSharp.Kernel;

namespace CuSharp.Tests.TestKernels;

public class MultiDimArrayKernels
{
    [Kernel]
    public static void MultiDimKernel(int[,] a, int b)
    {
        a[0, 0] = b;
    }

    [Kernel]
    public static void MultiDimArrayAddition(int[,] a, int[,] b)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        a[x, y] += b[x, y];
    }

    public static void MultiDimLocalArrayTest(int[,] a, int[,] b)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        int[,] arr = new int[3, 3];
        arr[x, y] = a[y, x];
        b[y, x] = arr[x, y];
    }

    public static void MultiDimMatrixMultiplication(int[,] a, int[,] b, int[,] c)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        int sum = 0;
        for (int i = 0; i < KernelTools.BlockDimension.X; i++)
        {
            sum += a[i,y] * b[x,i];
        }

        c[x, y] = sum;
    }

    
}