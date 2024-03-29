﻿using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class UnaryArithmeticOperatorTests
{
    [Fact]
    public void Int_Neg_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 12345;
        int[] result = new int[1];
        const int expectedResult = -a;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int[]>(UnaryArithmeticOperatorKernels.NegInt, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();
        
        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Long_Neg_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const long a = 5147483647;
        long[] result = new long[1];
        const long expectedResult = -a;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<long, long[]>(UnaryArithmeticOperatorKernels.NegLong, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Float_Neg_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const float a = 123.45f;
        float [] result = new float[1];
        const float expectedResult = -a;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<float, float[]>(UnaryArithmeticOperatorKernels.NegFloat, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Double_Neg_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const double a = 5147483647.12;
        double[] result = new double[1];
        const double expectedResult = -a;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<double, double[]>(UnaryArithmeticOperatorKernels.NegDouble, (1, 1, 1), (1, 1, 1), a, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}