using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class RecursionTests
{
    [Fact]
    private void TestFibonacciRecursion()
    {
        var dev = global::CuSharp.Cu.GetDefaultDevice();
        int n = 5;
        var devResults = dev.Allocate<int>(n);
        dev.Launch<int[], int>(RecursiveKernels.FibonacciLauncher, (1,1,1), (1,1,1), devResults, n);
        var result = dev.Copy(devResults);

        var expected = new int[n];
        RecursiveKernels.Fibonacci(expected, n);
        
        Assert.Equal(expected, result);
    }
}