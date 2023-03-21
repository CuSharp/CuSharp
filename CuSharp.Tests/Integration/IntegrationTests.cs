using System;
using System.Linq;
using System.Reflection;
using ManagedCuda;
using Xunit;

namespace CuSharp.Tests.Integration;

public class IntegrationTests
{
    static void SimpleArrayAdd(int[] a, int[] b, int[] c)
    {
        var idx = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
        c[idx] = a[idx] + b[idx];
    }

    [Fact]
    public void TestSimpleArrayAdd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int length = 1024;
        int[] a = new int[length];
        int[] b = new int[length];
        int[] expectedC = new int[length];
        for (int i = 0; i < length; i++)
        {
            a[i] = i;
            b[i] = i + 1;
            expectedC[i] = a[i] + b[i];
        }
        int[] c = new int[length];
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        dev.Launch(SimpleArrayAdd, (1,1,1), ((uint) length,1,1), devA, devB, devC);
        c = dev.Copy(devC);
        Assert.True(c.SequenceEqual(expectedC));
    }

    static void ArrayScalarAdd(int[] a, int b)
    {
        var idx = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
        a[idx] = a[idx] + b;
    }

    [Fact]
    public void TestArrayScalarAdd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int length = 1024;
        int[] a = new int[length];
        int b = 5;
        int[] expected = new int[length];
        for (int i = 0; i < length; i++)
        {
            expected[i] = i + b;
            a[i] = i;
        }

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(ArrayScalarAdd, (1,1,1), ((uint) length,1,1), devA, devB);
        a = dev.Copy(devA);
        Assert.True(a.SequenceEqual(expected));
    }

}