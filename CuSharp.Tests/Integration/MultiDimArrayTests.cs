using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using System;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class MultiDimArrayTests
{
    [Fact]
    public void TestMultiDimKernels()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[,] {{5}};
        var devA = dev.Copy(a);
        dev.Launch<int[,], int>(MultiDimArrayKernels.MultiDimKernel, (1,1,1),(1,1,1), devA, 42);
        a = dev.Copy(devA);
        devA.Dispose();
        dev.Dispose();
        Assert.Equal(42, a[0,0]);
    }
    
    [Fact]
    public void TestMultiDimArrayAdditionKernel()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[,] {{1,2,3},{4,5,6}, {7,8,9}};
        var b = new int[,] {{9, 8, 7}, {6, 5, 4}, {3, 2, 1}};
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MultiDimArrayKernels.MultiDimArrayAddition, (1,1,1),(3,3,1), devA, devB);
        a = dev.Copy(devA);
        devA.Dispose();
        devB.Dispose();
        dev.Dispose();
        var expected = new [,] {{10, 10, 10}, {10, 10, 10}, {10, 10, 10}};
        Assert.Equal(expected, a);
    }

    [Fact]
    public void TestMultiDimLocalArrayAdditionKernel()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}};
        var b = new int[,] {{9, 8, 7}, {6, 5, 4}, {3, 2, 1}};
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MultiDimArrayKernels.MultiDimLocalArrayTest, (1, 1, 1), (3, 3, 1), devA, devB);
        b = dev.Copy(devB);
        devA.Dispose();
        devB.Dispose();
        dev.Dispose();
        Assert.Equal(a, b);
    }

    [Fact]
    public void TestMultiDimMatrixMultiplication()
    {
        var dev = Cu.GetDefaultDevice();
        int matrixWidth = 5;
        int[,] a = new int [matrixWidth , matrixWidth];
        int[,] b = new int [matrixWidth , matrixWidth];
        int[,] c = new int [matrixWidth , matrixWidth];

        for (int i = 0; i < matrixWidth; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                a[i, j] = i * j;
                b[i, j] = (matrixWidth - i) * (matrixWidth - j);
            }
        }

        int[,] expected = new int[matrixWidth, matrixWidth];

        for (int i = 0; i < matrixWidth; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                int sum = 0;
                for (int k = 0; k < matrixWidth; k++)
                {
                    sum += a[k, j] * b[i, k];
                }

                expected[i, j] = sum;
            }
        }
        
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);

        dev.Launch(MultiDimArrayKernels.MultiDimMatrixMultiplication, (1, 1, 1), ((uint) matrixWidth, (uint) matrixWidth, 1), devA, devB, devC);
        c = dev.Copy(devC);

        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        dev.Dispose();

        Assert.Equal(expected, c);
    }

    [Fact]
    public void TestAssignLocalToParam()
    {
        var dev = Cu.GetDefaultDevice();
        var a = dev.Allocate<int>(1, 1);
        dev.Launch(MultiDimArrayKernels.MultiDimArrayAssignToParam, (1,1,1), (1,1,1), a);
        a.Dispose();
        //Sucess if no exception
    }

    [Fact]
    public void TestAssignParamToLocal()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[1, 1];
        a[0, 0] = 42;
        var devA = dev.Copy(a);
        dev.Launch(MultiDimArrayKernels.MultiDimArrayAssignToLocal, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);
        devA.Dispose();
        dev.Dispose();
        Assert.Equal(1337, a[0,0]);
    }

    [Fact]
    public void TestVeryNestedArrays()
    {
        Cu.EnableOptimizer = false;
        var dev = Cu.GetDefaultDevice();
        var a = new int[5, 5];
        a[1, 1] = 42;
        var c = new int[5, 5];
        var devC = dev.Allocate<int>(5, 5);
        var devA = dev.Copy(a);
        dev.Launch(MultiDimArrayKernels.VeryNestedArrayAccess, (1, 1, 1), (1, 1, 1), devA, devC);
        c = dev.Copy(devC);
        devC.Dispose();
        devA.Dispose();
        dev.Dispose();
        Assert.Equal(42, c[1, 1]);
    }

    [Fact]
    public void TestVeryNestedArrays2()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new int[5];
        a[0] = 42;
        var devA = dev.Copy(a);
        var devC = dev.Allocate<int>(5);
        dev.Launch(MultiDimArrayKernels.VeryNestedArrayAccess2, (1, 1, 1), (1, 1, 1), devA, devC);
        var c = dev.Copy(devC);
        devA.Dispose();
        devC.Dispose();
        dev.Dispose();
        Assert.Equal(42, c[0]);
    }

    [Fact]
    public void ArrayConvert1Dto2D_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[] input = { 1, 2, 3, 4, 5 };
        int[,] output = new int[input.Length, 1];
        int[,] expectedOutput = { { 1 }, { 2 }, { 3 }, { 4 }, { 5 } };

        var devInput = dev.Copy(input);
        var devOutput = dev.Copy(output);

        // Act
        dev.Launch<int[], int[,], int>(MultiDimArrayKernels.ArrayConvert1Dto2D, (1, 1, 1), (1, 1, 1), devInput, devOutput, input.Length);
        output = dev.Copy(devOutput);

        devInput.Dispose();
        devOutput.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void ArrayConvert2Dto1D_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[,] input = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
        int length0 = input.GetLength(0);
        int length1 = input.GetLength(1);
        int[] output = new int[length0 * length1];
        int[] expectedOutput = { 1, 2, 3, 4, 5, 6, 7, 8 };

        var devInput = dev.Copy(input);
        var devOutput = dev.Copy(output);

        // Act
        dev.Launch<int[,], int, int, int[]>(MultiDimArrayKernels.ArrayConvert2Dto1D, (1, 1, 1), (1, 1, 1), devInput, length0, length1, devOutput);
        output = dev.Copy(devOutput);

        devInput.Dispose();
        devOutput.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void MatrixTranspose_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[,] expectedOutput = { { 1, 3, 5, 7 }, { 2, 4, 6, 8 } };

        int tileSize = 32;
        int[,] input = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
        int blockRows = tileSize / 4;
        int rows = input.GetLength(0);
        int cols = input.GetLength(1);
        int[,] output = new int[cols, rows];

        var devInput = dev.Copy(input);
        var devOutput = dev.Copy(output);
        var devBlockRows = dev.CreateScalar(blockRows);
        var devRows = dev.CreateScalar(rows);
        var devCols = dev.CreateScalar(cols);

        var gridDimX = (uint)((cols + tileSize - 1) / tileSize);
        var gridDimY = (uint)((rows + tileSize - 1) / tileSize);

        // Act
        dev.Launch(MultiDimArrayKernels.MatrixTranspose, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)blockRows, 1),
            devInput, devBlockRows, devRows, devCols, devOutput);
        output = dev.Copy(devOutput);

        devInput.Dispose();
        devOutput.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void MatrixProduct_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[,] expectedOutput =
        {
            { 9, 12, 15 },
            { 19, 26, 33 },
            { 29, 40, 51 },
            { 39, 54, 69 }
        };

        int tileSize = 16;
        int[,] a = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
        int[,] b = { { 1, 2, 3 }, { 4, 5, 6 } };
        int aRows = a.GetLength(0);
        int aCols = a.GetLength(1);
        int bCols = b.GetLength(1);
        int pRows = aRows;
        int pCols = bCols;
        int[,] p = new int[pRows, pCols];

        var devP = dev.Copy(p);
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devARows = dev.CreateScalar(aRows);
        var devBCols = dev.CreateScalar(bCols);
        var devACols = dev.CreateScalar(aCols);

        var gridDimX = (uint)Math.Min((pCols + tileSize - 1) / tileSize, 16);
        var gridDimY = (uint)Math.Min((pRows + tileSize - 1) / tileSize, 16);

        // Act
        dev.Launch(MultiDimArrayKernels.MatrixProduct, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)tileSize, 1), devP, devA, devB, devARows, devBCols, devACols);
        p = dev.Copy(devP);

        devP.Dispose();
        devA.Dispose();
        devB.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedOutput, p);
    }

    [Fact]
    public void MatrixVectorProduct_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        int[] expectedOutput = { 46, 118, 190 };

        int[,] a = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        int[] b = { 9, 8, 7 };
        int paRows = a.GetLength(0);
        int aCols = a.GetLength(1);
        int[] p = new int[paRows];
        int tileSize = 16;

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devPaRows = dev.CreateScalar(paRows);
        var devACols = dev.CreateScalar(aCols);
        var devP = dev.Copy(p);

        var gridDimX = (uint)((1 + tileSize - 1) / tileSize);
        var gridDimY = (uint)((paRows + tileSize - 1) / tileSize);

        // Act
        dev.Launch(MultiDimArrayKernels.MatrixVectorProduct, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)tileSize, 1), devP, devA, devB, devPaRows, devACols);
        p = dev.Copy(devP);

        devA.Dispose();
        devB.Dispose();
        devP.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedOutput, p);
    }
}