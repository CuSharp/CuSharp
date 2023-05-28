using System;
using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class OperatorIntegrationTests
{
    [Fact]
    public void AdditionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatAddition, new float[] {42}, new float[]{2});
        Assert.Equal(44, result[0]);
    }

    [Fact]
    public void SubtractionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatSubtraction, new float[] {42}, new float[]{2});
        Assert.Equal(40, result[0]);
    }

    [Fact]
    public void MultiplicationTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatMultiplication, new float[] {42}, new float[]{2});
        Assert.Equal(84, result[0]);
    }

    [Fact]
    public void UintDivisionTest()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 12345;
        const uint b = 54321;
        uint[] result = new uint[1];
        const uint expectedResult = 12345 / 54321;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, uint, uint[]>(OperatorTestKernels.UintDivision, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void DivisionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatDivision, new float[] {42}, new float[]{2});
        Assert.Equal(21, result[0]);
    }

    [Fact]
    public void UintModuloTest()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const uint a = 54321;
        const uint b = 12345;
        uint[] result = new uint[1];
        const uint expectedResult = 54321 % 12345;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<uint, uint, uint[]>(OperatorTestKernels.UintModulo, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void ModuloTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatModulo, new float[] {42}, new float[]{4});
        Assert.Equal(2, result[0]);
    }

    private float[] RunFloatArrayTest(Action<float[], float[]> kernel, float[] a, float[] b)
    {
        var dev = Cu.GetDefaultDevice();
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(kernel, (1,1,1), (1,1,1), devA, devB);
        a = dev.Copy(devA);
        return a;
    }
    
    [Fact]
    public void TestNot()
    {
        var dev = Cu.GetDefaultDevice();
        bool[] a = new bool[] {false, false};
        bool b = true;
        var devA = dev.Copy(a);
        var devB = dev.CreateScalar(b);
        dev.Launch(OperatorTestKernels.NotTest, (1, 1, 1), (1, 1, 1), devA, devB);
        a = dev.Copy(devA);
        Assert.True(a[0]);
        Assert.True(!a[1]);
    }
}