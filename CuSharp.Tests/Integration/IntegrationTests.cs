using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CuSharp.Tests.Integration;

public class IntegrationTests
{
    void SimpleArrayAdd(int[] a, int[] b, int[] c)
    {
        var idx = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
        c[idx] = a[idx] + b[idx];
    }
    
    [Fact]
    public void TestSimpleArrayAdd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var devName = dev.ToString();
        int[] a = new[] {1, 2, 3};
        int[] b = new[] {4, 5, 6};
        int[] c = new int[3];
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        global::CuSharp.CuSharp.StartTimer();
        int j = 0;

        dev.Launch(SimpleArrayAdd, (1,1,1), (3,1,1), devA, devB, devC);
        float time = global::CuSharp.CuSharp.GetTimeMS();
        c = dev.Copy(devC);
        Assert.True(c.SequenceEqual(new []{5,7,9}));
    }
}