namespace CuSharp.Tests.TestKernels;

public static class BitwiseOperatorKernels
{
    public static void BitwiseAndInt(int a, int b, int[] result)
    {
        result[0] = a & b;
    }

    public static void BitwiseAndUint(uint a, uint b, uint[] result)
    {
        result[0] = a & b;
    }

    public static void BitwiseAndBool(bool a, bool b, bool[] result)
    {
        result[0] = a & b;
    }

    public static void BitwiseOrInt(int a, int b, int[] result)
    {
        result[0] = a | b;
    }

    public static void BitwiseOrUint(uint a, uint b, uint[] result)
    {
        result[0] = a | b;
    }

    public static void BitwiseOrBool(bool a, bool b, bool[] result)
    {
        result[0] = a | b;
    }

    public static void NotInt(int a, int[] result)
    {
        result[0] = ~a;
    }

    public static void NotUint(uint a, uint[] result)
    {
        result[0] = ~a;
    }

    public static void ShiftLeftSigned(int a, int shiftAmount, int[] result)
    {
        result[0] = a << shiftAmount;
    }

    public static void ShiftLeftUnsigned(uint a, int shiftAmount, uint[] result)
    {
        result[0] = a << shiftAmount;
    }

    public static void ShiftRightSigned(int a, int shiftAmount, int[] result)
    {
        result[0] = a >> shiftAmount;
    }

    public static void ShiftRightUnsigned(uint a, int shiftAmount, uint[] result)
    {
        result[0] = a >> shiftAmount;
    }

    public static void Xor(int a, int b, int[] result)
    {
        result[0] = a ^ b;
    }
}