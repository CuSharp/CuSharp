namespace CuSharp.Tests.TestKernels;

public class OperatorTestKernels
{
    public static void FloatAddition(float[] a, float[] b)
    {
        a[0] += b[0];
    }

    public static void FloatSubtraction(float[] a, float[] b)
    {
        a[0] -= b[0];
    }

    public static void FloatMultiplication(float[] a, float[] b)
    {
        a[0] *= b[0];
    }

    public static void UintDivision(uint a, uint b, uint[] result)
    {
        result[0] = a / b;
    }

    public static void FloatDivision(float[] a, float[] b)
    {
        a[0] /= b[0];
    }

    public static void UintModulo(uint a, uint b, uint[] result)
    {
        result[0] = a % b;
    }

    public static void FloatModulo(float[] a, float[] b)
    {
        a[0] %= b[0];
    }

    public static void NotTest(bool[] a, bool b)
    {
        a[0] = b;
        a[1] = !b;
    }
}