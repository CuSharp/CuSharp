namespace CuSharp.Tests.TestKernels;

public static class ControlFlowKernels
{
    public static void Switch(int a, int b, int[] c)
    {
        switch (a)
        {
            case 1:
                a += b;
                break;
            case 2:
                a -= b;
                break;
            default:
                a *= b;
                break;
        }

        c[0] = a;
    }

    public static void Switch(int a, int b, int c)
    {
        switch (a)
        {
            case 1:
                a += b;
                break;
            case 2:
                a -= b;
                break;
            default:
                a *= b;
                break;
        }

        c = a;
    }

    public static void For(int a, int b, int[] c)
    {
        for (int i = 0; i < a; i++)
        {
            c[i] = a + b + i;
        }
    }

    public static void For(int a, int b, int c)
    {
        for (int i = 0; i < a; i++)
        {
            c = a + b;
        }

        c++;
    }

    public static void While(int a, int b, int[] c)
    {
        while (a < b)
        {
            a++;
        }

        c[0] = a;
    }

    public static void While(int a, int b, int c)
    {
        while (a < b)
        {
            a++;
        }

        c = a;
    }

    public static void DoWhile(int a, int b, int[] c)
    {
        do
        {
            a++;
        } while (a < b);

        c[0] = a;
    }

    public static void DoWhile(int a, int b, int c)
    {
        do
        {
            a++;
        } while (a < b);

        c = a;
    }

    public static void WhileWithBreak(int a, int b, int c, int[] d)
    {
        while (a < b)
        {
            a++;
            if (c == 0)
            {
                break;
            }
        }

        d[0] = a;
    }

    public static void WhileWithBreak(int a, int b, int c)
    {
        while (a < b)
        {
            if (c == 0)
            {
                break;
            }

            a++;
        }

        c = a;
    }

    public static void WhileWithContinue(int a, int b, int c, int[] d)
    {
        while (a < b)
        {
            if (c == 0)
            {
                a++;
                continue;
            }
            c++;
        }

        d[0] = a;
    }

    public static void WhileWithContinue(int a, int b, int c)
    {
        while (a < b)
        {
            if (c == 0)
            {
                continue;
            }

            a++;
        }

        c = a;
    }

    public static void Goto(int a, int b, int[] c)
    {
    start:
        a++;

        if (a < b)
        {
            goto start;
        }
        a--;

        c[0] = a;
    }

    public static void Goto(int a, int b, int c)
    {
        start:
        a++;

        if (a < b)
        {
            goto start;
        }

        c = a;
    }
}
