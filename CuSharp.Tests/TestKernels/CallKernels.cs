using CuSharp.Kernel;

namespace CuSharp.Tests.TestKernels;

public static class CallKernels
{
    public static void CallVoidMethod(int a, int b, int[] c)
    {
        VoidMethod(c);
        c[0] += a + b;
    }

    private static void VoidMethod(int[] c)
    {
        c[0] = 12345;
    }

    public static void CallIntMethod(int a, int b, int[] c)
    {
        c[0] = AddTwoIntegers(a, b);
    }

    public static void CallIntMethod(int a, int b, int c)
    {
        c = AddTwoIntegers(a, b);
    }

    private static int AddTwoIntegers(int a, int b)
    {
        return a + b;
    }

    public static void CallIntMethodNested(int a, int b, int[] c)
    {
        c[0] = AddTwoIntegersNested(a, b);
    }

    public static void CallIntMethodNested(int a, int b, int c)
    {
        c = AddTwoIntegersNested(a, b);
    }

    private static int AddTwoIntegersNested(int a, int b)
    {
        int c;

        if (AreEqual(a, b))
        {
            c = a + b;
        }
        else
        {
            c = int.MaxValue;
        }

        return c;
    }

    private static bool AreEqual(int a, int b)
    {
        return a == b;
    }

    public static void CallIntReturnArrayWithKernelTools(int[] a, int[] b, int[] c)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        c = AddTwoIntArray(a, b, c, i);
    }

    private static int[] AddTwoIntArray(int[] a, int[] b, int[] c, int i)
    {
        c[i] = a[i] + b[i];
        return c;
    }

    public static void NestedArrayCall(int[] a, int[] b, int[] c)
    {
        ArrayAdditionNested(a, b, c);
    }

    public static void ArrayAdditionNested(int[] a, int[] b, int[] c)
    {
        c[0] = AddForNested(a[0], b[0]);
    }

    public static int AddForNested(int a, int b)
    {
        return a + b;
    }
}
