using CuSharp.Kernel;
using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class NestedFunctionTests
{

    [Fact]
    public void TestCallIntMethod()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();

        var devA = dev.CreateScalar(123);
        var devB = dev.CreateScalar(321);
        var devC = dev.CreateScalar(0);

        // Act
        dev.Launch(CallKernels.CallIntMethod, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        dev.Dispose();
    }

    [Fact]
    public void TestCallIntMethodNested()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();

        var devA = dev.CreateScalar(123);
        var devB = dev.CreateScalar(321);
        var devC = dev.CreateScalar(0);

        // Act
        dev.Launch(CallKernels.CallIntMethodNested, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        dev.Dispose();
    }

    [Fact]
    public void TestArrayAdditionNested()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[] {1};
        var b = new int[] {2};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Allocate<int>(1);
        
        dev.Launch(CallKernels.ArrayAdditionNested, (1,1,1), (1,1,1), devA, devB, devC);
        var c = dev.Copy(devC);

        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        dev.Dispose();

        Assert.Equal(3, c[0]);
    }
    
    [Fact]
    public void TestNestedArrayAdditionNested()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[] {1};
        var b = new int[] {2};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Allocate<int>(1);
        
        dev.Launch(CallKernels.NestedArrayCall, (1,1,1), (1,1,1), devA, devB, devC);
        var c = dev.Copy(devC);

        devA.Dispose();
        devB.Dispose(); 
        devC.Dispose();
        dev.Dispose();

        Assert.Equal(3, c[0]);
    }
    
    [Fact]
    public void TestLocalFunction()
    {
        var dev = Cu.GetDefaultDevice();

        static void Kernel(uint[] a)
        {
            var idx = KernelTools.ThreadIndex.X;
            a[idx] = idx;
        }

        var devA = dev.Allocate<uint>(5);
        dev.Launch<uint[]>(Kernel, (1,1,1), (5,1,1), devA);
        var a = dev.Copy(devA);
        devA.Dispose();
        dev.Dispose();
        Assert.Equal(new uint[]{0,1,2,3,4}, a);
    }
}