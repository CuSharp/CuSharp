using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class CallTests
{
    [Fact]
    public void Int_CallVoidMethod_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] c = new int[1];
        int[] result = new int[1];
        const int expectedResult = a + a + b;
        var devC = dev.Copy(c);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(CallKernels.CallVoidMethod, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_CallIntMethod_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = a + b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(CallKernels.CallIntMethod, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_CallIntMethodNested_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 12345;
        int[] result = new int[1];
        const int expectedResult = a + b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(CallKernels.CallIntMethodNested, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_CallIntMethodNestedIntMax_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = int.MaxValue;
        var devResult = dev.Copy(result);
        
        // Act
        dev.Launch<int, int, int[]>(CallKernels.CallIntMethodNested, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_CallIntReturnArrayWithKernelTools_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[] a = { 123, 456, 789 };
        int[] b = { 987, 654, 321 };
        int[] result = new int[3];
        int[] expectedResult = { a[0] + b[0], a[1] + b[1], a[2] + b[2] };
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int[], int[], int[]>(CallKernels.CallIntReturnArrayWithKernelTools, ((uint)result.Length, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);
        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}