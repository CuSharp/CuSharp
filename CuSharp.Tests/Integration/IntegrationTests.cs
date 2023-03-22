using System.Linq;
using CuSharp.Tests.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace CuSharp.Tests.Integration;

public class IntegrationTests
{
    private readonly ITestOutputHelper _output;

    public IntegrationTests(ITestOutputHelper output)
    {
        _output = output;
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
        dev.Launch(MethodsToCompile.ArrayIntAdditionWithKernelTools, (1,1,1), ((uint) length,1,1), devA, devB, devC);
        c = dev.Copy(devC);

        _output.WriteLine($"Used gpu device: '{dev.ToString()}'");
        Assert.True(c.SequenceEqual(expectedC));
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
        dev.Launch(MethodsToCompile.ArrayIntScalarAdd, (1,1,1), ((uint) length,1,1), devA, devB);
        a = dev.Copy(devA);
        _output.WriteLine($"Used gpu device: '{dev.ToString()}'");
        Assert.True(a.SequenceEqual(expected));
    }

}