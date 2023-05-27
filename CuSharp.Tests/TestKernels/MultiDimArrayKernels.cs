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

    public static void MultiDimArrayAssignToParam(int[,] a)
    {
        var x = new int[1, 1];
        a = x;
    }
    
    public static void MultiDimArrayAssignToLocal(int[,] a)
    {
        var x = a;
        a[0, 0] = 1337;
    }

    public static void VeryNestedArrayAccess(int[,] a, int[,] c)
    {
        int[,] b = new int[5, 5];
            b[0,0] = a[0,0];
        if (true)
        {
        }
        c[0, 0] = b[0, 0];
    }
    
    public static void VeryNestedArrayAccess2(int[] a, int[] c)
    {
        var b = new int[5];
        if (a[0] == 42)
        {
            b[0] = a[0];
        }

        c[0] = b[0];
    }
}