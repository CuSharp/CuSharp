namespace CuSharp.Tests.TestKernels;

public static class UnaryArithmeticOperatorKernels
{
    public static void NegInt(int a, int[] result)
    {
        result[0] = -a;
    }

    public static void NegLong(long a, long[] result)
    {
        result[0] = -a;
    }

    public static void NegFloat(float a, float[] result)
    {
        result[0] = -a;
    }

    public static void NegDouble(double a, double[] result)
    {
        result[0] = -a;
    }
}