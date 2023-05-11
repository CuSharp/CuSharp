namespace CuSharp.Tests.TestHelper;

public class TypeTestKernels
{
    public static void FloatAddition(float[] a, float[] b)
    {
        a[0] += b[0];
    }

    public static void DoubleAddition(double[] a, double[] b)
    {
        a[0] += b[0];
    }

    public static void IntAddition(int[] a, int[] b)
    {
        a[0] += b[0];
    }

    public static void ByteAddition(byte[] a, byte[] b)
    {
        a[0] += b[0];
    }
    
    public static void ShortAddition(short[] a, short[] b)
    {
        a[0] += b[0];
    }
    
    public static void LongAddition(long[] a, long[] b)
    {
        a[0] += b[0];
    }

    public static void BoolAnd(bool[] a, bool[] b)
    {
        a[0] = a[0] && b[0];
    }
}