using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class ComparisonTests
{
    [Fact]
    public void Int_Equals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a == b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.EqualsInt, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_Equals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a == b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.EqualsUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Float_Equals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const float a = 12345.1f;
        const float b = 54321.2f;
        bool[] result = new bool[1];
        const bool expectedResult = a == b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.EqualsFloat, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_NotEquals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a != b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.NotEqualsInt, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_NotEquals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a != b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.NotEqualsUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Float_NotEquals_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const float a = 12345.1f;
        const float b = 54321.2f;
        bool[] result = new bool[1];
        const bool expectedResult = a != b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.NotEqualsFloat, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_GreaterThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a > b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.GreaterThanInt, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_GreaterThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a > b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.GreaterThanUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Float_GreaterThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const float a = 12345.1f;
        const float b = 54321.2f;
        bool[] result = new bool[1];
        const bool expectedResult = a > b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.GreaterThanFloat, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_LessThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a < b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.LessThanInt, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_LessThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        bool[] result = new bool[1];
        const bool expectedResult = a < b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.LessThanUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Float_LessThan_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const float a = 12345.1f;
        const float b = 54321.2f;
        bool[] result = new bool[1];
        const bool expectedResult = a < b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(ComparisonKernels.LessThanFloat, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}