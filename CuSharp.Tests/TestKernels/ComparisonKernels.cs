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
        if (a == b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void EqualsTo(int a, int b, int c)
    {
        if (a == b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
    }

    public static void NotEqualsInt(int a, int b, bool[] c)
    {
        if (a != b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void NotEqualsUint(uint a, uint b, bool[] c)
    {
        if (a != b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void NotEqualsFloat(float a, float b, bool[] c)
    {
        if (a != b)
        {
            c[0] = true;
        }
        else
        {
            c[0] = false;
        }
    }

    public static void NotEqualsTo(int a, int b, int c)
    {
        if (a >= b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
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

    public static void GreaterThan(int a, int b, int c)
    {
        if (a > b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
    }

    public static void GreaterThanOrEquals(int a, int b, int c)
    {
        if (a >= b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
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

    public static void LessThan(int a, int b, int c)
    {
        if (a < b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
    }

    public static void LessThanOrEquals(int a, int b, int c)
    {
        if (a <= b)
        {
            c = a;
        }
        else
        {
            c = b;
        }

        c *= c;
    }
}