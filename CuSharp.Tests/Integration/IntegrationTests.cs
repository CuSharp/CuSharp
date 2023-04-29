﻿using System.Linq;
using CuSharp.Tests.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class IntegrationTests
{
    private readonly ITestOutputHelper _output;

    public IntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestCallIntMethod()
    {
        // Arrange
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();

        var devA = dev.Copy(123);
        var devB = dev.Copy(321);
        var devC = dev.Copy(0);

        // Act
        dev.Launch(MethodsToCompile.CallIntMethod, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        var c = dev.Copy(devC);

        // Assert
        Assert.Equal(444, c);
    }

    [Fact]
    public void TestCallIntMethodNested()
    {
        // Arrange
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();

        var devA = dev.Copy(123);
        var devB = dev.Copy(321);
        var devC = dev.Copy(0);

        // Act
        dev.Launch(MethodsToCompile.CallIntMethodNested, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        var c = dev.Copy(devC);

        // Assert
        Assert.Equal(int.MaxValue, c);
    }

    [Fact]
    public void TestSimpleArrayAdd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int length = 1024;
        int[] a = new int[length];
        int[] b = new int[length];
        int[] expectedC = new int[length];
        for (int i = 0; i < length; i++)
        {
            a[i] = i;
            b[i] = i + 1;
            expectedC[i] = a[i] + b[i];
        }

        int[] c = new int[length];
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        dev.Launch(MethodsToCompile.ArrayIntAdditionWithKernelTools, (1, 1, 1), ((uint)length, 1, 1), devA, devB, devC);
        c = dev.Copy(devC);

        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.True(c.SequenceEqual(expectedC));
    }

    [Fact]
    public void TestArrayScalarAdd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int length = 1024;
        int[] a = new int[length];
        int b = 5;
        int[] expected = new int[length];
        for (int i = 0; i < length; i++)
        {
            expected[i] = i + b;
            a[i] = i;
        }

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MethodsToCompile.ArrayIntScalarAdd, (1, 1, 1), ((uint)length, 1, 1), devA, devB);
        a = dev.Copy(devA);
        _output.WriteLine($"Used gpu device: '{dev.ToString()}'");
        Assert.True(a.SequenceEqual(expected));
    }

    [Fact]
    public void TestMatrixMultiplication()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int matrixWidth = 100;
        uint gridDim = (uint)(matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint)(matrixWidth > 32 ? 32 : matrixWidth);
        int[] a = new int [matrixWidth * matrixWidth];
        int[] b = new int [matrixWidth * matrixWidth];
        int[] c = new int [matrixWidth * matrixWidth];
        for (int i = 0; i < matrixWidth * matrixWidth; i++)
        {
            a[i] = i;
            b[i] = matrixWidth * matrixWidth - i;
        }

        int[] expectedC = new int[matrixWidth * matrixWidth];
        for (int row = 0; row < gridDim * blockDim; row++)
        {
            for (int col = 0; col < gridDim * blockDim; col++)
            {
                if (row < matrixWidth && col < matrixWidth)
                {
                    int result = 0;
                    for (int i = 0; i < matrixWidth; i++)
                    {
                        result = result + a[matrixWidth * row + i] * b[i * matrixWidth + col];
                    }

                    expectedC[row * matrixWidth + col] = result;
                }
            }
        }

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        var devWidth = dev.Copy(matrixWidth);
        
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC, devWidth);
        c = dev.Copy(devC);

        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.Equal(expectedC, c);
    }

    [Fact]
    public void TestOptimizerInMatrixMultiplication()
    {
        const int matrixWidth = 1000;

        // Warm-up
        LaunchAndMeasureMatrixMultiplication(matrixWidth, null);

        // Measure with optimizer
        var resultWithOptimizer = LaunchAndMeasureMatrixMultiplication(matrixWidth, true);

        // Measure without optimizer
        var resultWithoutOptimizer = LaunchAndMeasureMatrixMultiplication(matrixWidth, false);

        Assert.True(resultWithOptimizer.measureResult < resultWithoutOptimizer.measureResult);
        Assert.Equal(resultWithOptimizer.matrixResult, resultWithoutOptimizer.matrixResult);
    }

    private (float measureResult, int[] matrixResult) LaunchAndMeasureMatrixMultiplication(int matrixWidth, bool? enableOptimizer)
    {
        if (enableOptimizer != null)
        {
            global::CuSharp.CuSharp.EnableOptimizer = (bool)enableOptimizer;
        }

        uint gridDim = (uint)(matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint)(matrixWidth > 32 ? 32 : matrixWidth);
        int[] a = new int[matrixWidth * matrixWidth];
        int[] b = new int[matrixWidth * matrixWidth];
        int[] c = new int[matrixWidth * matrixWidth];
        for (int i = 0; i < matrixWidth * matrixWidth; i++)
        {
            a[i] = i;
            b[i] = matrixWidth * matrixWidth - i;
        }

        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        var devWidth = dev.Copy(matrixWidth);

        var before = global::CuSharp.CuSharp.CreateEvent();
        var after = global::CuSharp.CuSharp.CreateEvent();

        before.Record();
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC, devWidth);
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
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int matrixWidth = 100;
        uint gridDim = (uint)(matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint)(matrixWidth > 32 ? 32 : matrixWidth);

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
        var devWidth = dev.Copy(matrixWidth);

        // Act 1
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC1, devWidth);
        c1 = dev.Copy(devC1);

        // Act 2
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC2, devWidth);
        c2 = dev.Copy(devC2);

        // Assert
        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.Equal(c1, c2);
    }

    [Fact]
    public void TestLaunchDifferentKernels()
    {
        // Arrange
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int[] a = { 1, 2, 3 };
        int[] b = { 4, 5, 6 };
        int[] c1 = new int[3];
        int[] c2 = new int[3];

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC1 = dev.Copy(c1);
        var devC2 = dev.Copy(c2);

        // Act 1
        int[] expectedC1 = { 5, 7, 9 };
        dev.Launch(MethodsToCompile.ArrayIntAdditionWithKernelTools, (1, 1, 1), (3, 1, 1), devA, devB, devC1);
        c1 = dev.Copy(devC1);

        // Act 2
        int[] expectedC2 = { 4, 10, 18 };
        dev.Launch(MethodsToCompile.ArrayIntMultiplicationWithKernelTools, (1, 1, 1), (3, 1, 1), devA, devB, devC2);
        c2 = dev.Copy(devC2);

        // Assert
        _output.WriteLine($"Used gpu device: '{dev}'");
        Assert.Equal(expectedC1, c1);
        Assert.Equal(expectedC2, c2);
    }

    /*[Fact]
    public void TestArrayLengthLaunch()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int[] a = new int[] {1, 2, 3};
        int b = 0;

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        
        dev.Launch(MethodsToCompile.ArrayLengthAttribute, (1,1,1), (1,1,1), devA, devB);
        b = dev.Copy(devB);
        Assert.Equal(3, b);
    }*/


    [Fact]
    public void TestNot()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        bool[] a = new bool[] {false, false};
        bool b = true;
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MethodsToCompile.NotTest, (1, 1, 1), (1, 1, 1), devA, devB);
        a = dev.Copy(devA);
        Assert.True(a[0]);
        Assert.True(!a[1]);
    }

    [Fact]
    public void TestNewArr()
    {
        global::CuSharp.CuSharp.EnableOptimizer = true;
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var b = new int[]{1,2,3,4,5};
        var devB = dev.Copy(b);
        dev.Launch(MethodsToCompile.Newarr, (1, 1, 1), (1, 1, 1), devB);
        b = dev.Copy(devB);
        Assert.Equal(5, b[0]);
    }

    /*
    static void GenericVectorAddition<T> (T[] a, T[] b, T[] c) where T : INumber<T>
    {
        var idx = KernelTools.ThreadIndex.X;
        c[idx] =  a[idx] + b[idx];
    }

    [Fact]
    public void TestGeneric()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[]{6,7,8,9,10};
        var b = new int[]{1,2,3,4,5};
        var expectedC = new int[] {7, 9, 11, 13, 15};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(new int[5]);
        dev.Launch(GenericVectorAddition, (1, 1, 1), (1, 1, 1), devA, devB, devC);
        a = dev.Copy(devA);
        b = dev.Copy(devB);
        var c = dev.Copy(devC);
        Assert.Equal(expectedC, c);
    }*/
    [Fact]
    public void TestAOTC() //TODO Write Unit tests that check if aotc output was actually used
    {
        global::CuSharp.CuSharp.AotKernelFolder = "./resources";
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[] {1, 2, 3};
        var b = new int[] {2, 2, 2};

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Allocate<int>(3);
        
        dev.Launch(MethodsToCompile.ArrayIntMultiplicationWithKernelTools, (3,3,3), (3,3,3), devA, devB, devC);
        var c = dev.Copy(devC);
        Assert.Equal(new int[]{2,4,6}, c);

    }
}