using System;
using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class PK_Integration
{
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
        var devTileSize = dev.CreateScalar(tileSize);

        var gridDimX = (uint)((cols + tileSize - 1) / tileSize);
        var gridDimY = (uint)((rows + tileSize - 1) / tileSize);

        // Act
        dev.Launch(PK_Kernels.MatrixTranspose, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)blockRows, 1),
            devInput, devBlockRows, devRows, devCols, devOutput);
        output = dev.Copy(devOutput);

        devInput.Dispose();
        devOutput.Dispose();
        devBlockRows.Dispose();
        devRows.Dispose();
        devCols.Dispose();
        devTileSize.Dispose();

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
        dev.Launch(PK_Kernels.MatrixProduct, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)tileSize, 1), devP, devA, devB, devARows, devBCols, devACols);
        p = dev.Copy(devP);

        devP.Dispose();
        devA.Dispose();
        devB.Dispose();
        devARows.Dispose();
        devBCols.Dispose();
        devACols.Dispose();

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
        dev.Launch(PK_Kernels.MatrixVectorProduct, (gridDimX, gridDimY, 1), ((uint)tileSize, (uint)tileSize, 1), devP, devA, devB, devPaRows, devACols);
        p = dev.Copy(devP);

        devA.Dispose();
        devB.Dispose();
        devPaRows.Dispose();
        devACols.Dispose();
        devP.Dispose();

        // Assert
        Assert.Equal(expectedOutput, p);
    }
}