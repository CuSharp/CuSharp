﻿using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;
using Xunit.Abstractions;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class LaunchTests
{

    private readonly ITestOutputHelper _output;

    public LaunchTests(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public void TestOptimizerInMatrixMultiplication()
    {
        const int matrixWidth = 3000;

        // Warm-up
        LaunchAndMeasureMatrixMultiplication(matrixWidth, null);

        // Measure with optimizer
        var resultWithOptimizer = LaunchAndMeasureMatrixMultiplication(matrixWidth, true);

        // Measure without optimizer
        var resultWithoutOptimizer = LaunchAndMeasureMatrixMultiplication(matrixWidth, false);

        Assert.True(resultWithOptimizer.measureResult < resultWithoutOptimizer.measureResult + 1); //mostly works for big matrixWidth
        Assert.Equal(resultWithOptimizer.matrixResult, resultWithoutOptimizer.matrixResult);
    }

    private (float measureResult, int[] matrixResult) LaunchAndMeasureMatrixMultiplication(int matrixWidth,
        bool? enableOptimizer)
    {
        if (enableOptimizer != null)
        {
            Cu.EnableOptimizer = (bool) enableOptimizer;
        }

        uint gridDim = (uint) (matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint) (matrixWidth > 32 ? 32 : matrixWidth);
        int[] a = new int[matrixWidth * matrixWidth];
        int[] b = new int[matrixWidth * matrixWidth];
        int[] c = new int[matrixWidth * matrixWidth];
        for (int i = 0; i < matrixWidth * matrixWidth; i++)
        {
            a[i] = i;
            b[i] = matrixWidth * matrixWidth - i;
        }

        var dev = Cu.GetDefaultDevice();
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        var devWidth = dev.CreateScalar(matrixWidth);

        var before = Cu.CreateEvent();
        var after = Cu.CreateEvent();

        before.Record();
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB,
            devC, devWidth);
        after.Record();

        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        devWidth.Dispose();
        var timeResult = before.GetDeltaTo(after);
        before.Dispose();
        after.Dispose();

        return (timeResult, c);
    }

    [Fact]
    public void TestLaunchKernelMultipleTimes()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int matrixWidth = 100;
        uint gridDim = (uint) (matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint) (matrixWidth > 32 ? 32 : matrixWidth);

        int[] a = new int[matrixWidth * matrixWidth];
        int[] b = new int[matrixWidth * matrixWidth];
        int[] c1 = new int[matrixWidth * matrixWidth];
        int[] c2 = new int[matrixWidth * matrixWidth];
        for (int i = 0; i < matrixWidth * matrixWidth; i++)
        {
            a[i] = i;
            b[i] = matrixWidth * matrixWidth - i;
        }

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC1 = dev.Copy(c1);
        var devC2 = dev.Copy(c2);
        var devWidth = dev.CreateScalar(matrixWidth);

        // Act 1
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB,
            devC1, devWidth);
        c1 = dev.Copy(devC1);

        // Act 2
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB,
            devC2, devWidth);
        c2 = dev.Copy(devC2);

        // Assert
        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.Equal(c1, c2);
    }

    [Fact]
    public void TestLaunchDifferentKernels()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[] a = {1, 2, 3};
        int[] b = {4, 5, 6};
        int[] c1 = new int[3];
        int[] c2 = new int[3];

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC1 = dev.Copy(c1);
        var devC2 = dev.Copy(c2);

        // Act 1
        int[] expectedC1 = {5, 7, 9};
        dev.Launch(MethodsToCompile.ArrayIntAdditionWithKernelTools, (1, 1, 1), (3, 1, 1), devA, devB, devC1);
        c1 = dev.Copy(devC1);

        // Act 2
        int[] expectedC2 = {4, 10, 18};
        dev.Launch(MethodsToCompile.ArrayIntMultiplicationWithKernelTools, (1, 1, 1), (3, 1, 1), devA, devB, devC2);
        c2 = dev.Copy(devC2);

        // Assert
        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.Equal(expectedC1, c1);
        Assert.Equal(expectedC2, c2);
    }
    
    [Fact]
    public void TestAOTC() 
    {
        Cu.AotKernelFolder = "./resources";
        var dev = Cu.GetDefaultDevice();
        var a = new int[] {1};
        var b = new int[] {2};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Allocate<int>(1);

        dev.Launch(MethodsToCompile.AOTCArrayIntAddition, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        var c = dev.Copy(devC);
        Assert.Equal(new int[] {3}, c);
    }
}