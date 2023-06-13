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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(BitwiseOperatorKernels.BitwiseAndInt, (1,1,1), (1,1,1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, uint, uint[]>(BitwiseOperatorKernels.BitwiseAndUint, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<bool, bool, bool[]>(BitwiseOperatorKernels.BitwiseAndBool, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(BitwiseOperatorKernels.BitwiseOrInt, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, uint, uint[]>(BitwiseOperatorKernels.BitwiseOrUint, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<bool, bool, bool[]>(BitwiseOperatorKernels.BitwiseOrBool, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int[]>(BitwiseOperatorKernels.NotInt, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, uint[]>(BitwiseOperatorKernels.NotUint, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        const int expectedResult = a << shiftAmount;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(BitwiseOperatorKernels.ShiftLeftSigned, (1, 1, 1), (1, 1, 1), a, shiftAmount, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        const uint expectedResult = a << shiftAmount;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, int, uint[]>(BitwiseOperatorKernels.ShiftLeftUnsigned, (1, 1, 1), (1, 1, 1), a, shiftAmount, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        const uint expectedResult = a >> shiftAmount;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, int, uint[]>(BitwiseOperatorKernels.ShiftRightUnsigned, (1, 1, 1), (1, 1, 1), a, shiftAmount, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        const int expectedResult = a >> shiftAmount;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(BitwiseOperatorKernels.ShiftRightSigned, (1, 1, 1), (1, 1, 1), a, shiftAmount, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

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
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(BitwiseOperatorKernels.Xor, (1,1,1), (1,1,1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}