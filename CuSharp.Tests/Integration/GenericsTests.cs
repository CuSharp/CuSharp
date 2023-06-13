using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class GenericsTests
{
    [Fact]
    public void TestGeneric()
    {
        var dev = Cu.GetDefaultDevice();
        int[] a = new int[] {42};
        int[] b = new int[] {1337};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MethodsToCompile.IAmGeneric, (1, 1, 1), (1, 1, 1), devA, devB);
        a = dev.Copy(devA);
        devA.Dispose();
        devB.Dispose();
        dev.Dispose();
        Assert.Equal(42 + 1337, a[0]);
    }

    [Fact]
    public void TestGenericNewArr()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[] {0, 1, 2, 3, 4};
        var devA = dev.Copy(a);
        dev.Launch(MethodsToCompile.GenericNewArray, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);
        devA.Dispose();
        dev.Dispose();
        Assert.Equal(1, a[0]);
    }
    
    [Fact]
    public void TestGenericMultiDimNewArr()
    {
        var dev = Cu.GetDefaultDevice();
        var hostA = new [,] {{0, 1, 2, 3, 4}};
        var a = dev.Copy(hostA);
        dev.Launch(MethodsToCompile.GenericMultiDimNewArray, (1,1,1), (1,1,1), a);
        hostA = dev.Copy(a);
        a.Dispose();
        dev.Dispose();
        Assert.Equal(1,hostA[0,0]);
    }
}