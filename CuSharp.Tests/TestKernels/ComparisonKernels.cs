namespace CuSharp.Tests.TestKernels;

public class ComparisonKernels
{
    public static void EqualsInt(int a, int b, bool[] c)
    {
        if (a == b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void EqualsUint(uint a, uint b, bool[] c)
    {
        if (a == b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void EqualsFloat(float a, float b, bool[] c)
    {
        if (a > b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void GreaterThanInt(int a, int b, bool[] c)
    {
        if (a > b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void GreaterThanUint(uint a, uint b, bool[] c)
    {
        if (a > b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void GreaterThanFloat(float a, float b, bool[] c)
    {
        if (a > b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void LessThanInt(int a, int b, bool[] c)
    {
        if (a < b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void LessThanUint(uint a, uint b, bool[] c)
    {
        if (a < b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void LessThanFloat(float a, float b, bool[] c)
    {
        if (a < b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }
}