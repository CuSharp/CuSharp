using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class BitwiseOperatorTests
{
    [Fact]
    public void Int_BitwiseAnd_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = a & b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseAndInt, (1,1,1), (1,1,1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_BitwiseAnd_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        uint[] result = new uint[1];
        const uint expectedResult = a & b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseAndUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Bool_BitwiseAnd_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const bool a = true;
        const bool b = false;
        bool[] result = new bool[1];
        const bool expectedResult = a & b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseAndBool, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_BitwiseOr_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = a | b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseOrInt, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_BitwiseOr_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        uint[] result = new uint[1];
        const uint expectedResult = a | b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseOrUint, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Bool_BitwiseOr_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const bool a = true;
        const bool b = false;
        bool[] result = new bool[1];
        const bool expectedResult = a | b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.BitwiseOrBool, (1, 1, 1), (1, 1, 1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_Not_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        int[] result = new int[1];
        const int expectedResult = ~a;

        var devA = dev.CreateScalar(a);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.NotInt, (1, 1, 1), (1, 1, 1), devA, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Uint_Not_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        uint[] result = new uint[1];
        const uint expectedResult = ~a;

        var devA = dev.CreateScalar(a);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.NotUint, (1, 1, 1), (1, 1, 1), devA, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void ShiftLeftSigned_Integration()
    {
        var dev = Cu.GetDefaultDevice();
        const int a = 1234567;
        const int shiftAmount = 3;
        int[] result = new int[1];
        const int expectedResult = a << 3;

        var devA = dev.CreateScalar(a);
        var devShiftAmount = dev.CreateScalar(shiftAmount);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.ShiftLeftSigned, (1, 1, 1), (1, 1, 1), devA, devShiftAmount, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devShiftAmount.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void ShiftLeftUnsigned_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 1234567;
        const int shiftAmount = 3;
        uint[] result = new uint[1];
        const uint expectedResult = a << 3;

        var devA = dev.CreateScalar(a);
        var devShiftAmount = dev.CreateScalar(shiftAmount);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.ShiftLeftUnsigned, (1, 1, 1), (1, 1, 1), devA, devShiftAmount, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devShiftAmount.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void ShiftRightArithmetic_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 1234567;
        const int shiftAmount = 3;
        uint[] result = new uint[1];
        const uint expectedResult = a >> 3;

        var devA = dev.CreateScalar(a);
        var devShiftAmount = dev.CreateScalar(shiftAmount);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.ShiftRightUnsigned, (1, 1, 1), (1, 1, 1), devA, devShiftAmount, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devShiftAmount.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void ShiftRightLogical_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1234567;
        const int shiftAmount = 3;
        int[] result = new int[1];
        const int expectedResult = a >> 3;

        var devA = dev.CreateScalar(a);
        var devShiftAmount = dev.CreateScalar(shiftAmount);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.ShiftRightSigned, (1, 1, 1), (1, 1, 1), devA, devShiftAmount, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devShiftAmount.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Xor_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = a ^ b;

        var devA = dev.CreateScalar(a);
        var devB = dev.CreateScalar(b);
        var devResult = dev.Copy(result);

        // Act
        dev.Launch(BitwiseOperatorKernels.Xor, (1,1,1), (1,1,1), devA, devB, devResult);
        result = dev.Copy(devResult);

        devA.Dispose();
        devB.Dispose();
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}