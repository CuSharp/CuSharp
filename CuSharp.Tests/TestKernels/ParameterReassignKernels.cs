namespace CuSharp.Tests.TestKernels;

public class ParameterReassignKernels
{
    
    public static void ArrayReassign(int[] a, int[] b, int c)
    {
        b = a;
        b[0] = c;
    }

    public static void ArrayReassignIfBranchedC(int[] a, int[] b, int c)
    {
        b = a;
        if (c > 100)
        {
            c += 1;
        }
        else
        {
            c += 2;
        }

        b[0] = c;
    }

    public static void BranchedArrayReassign(int[] a, int[] b, int c)
    {
        if (c > 100)
        {
            b = a;
        }

        b[0] = c;
    }

    public static void LocalArrayVariables(int[] a, int[] b, int c)
    {
        int[] localArray;
        if (c > 100)
        {
            localArray = b;
        }
        else
        {
            localArray = a;
        }

        localArray[0] = c;
    }
    
    public static void AssignLocalVariableToArg(int[] a, int[] b, int c)
    {
        int[] localArray = new int [100];
        localArray[0] = c;
        if (c > 100)
        {
            b = localArray;
            a[0] = b[0];
        }
        else
        {
            a = localArray;
            b[0] = a[0];
        }
    }
}