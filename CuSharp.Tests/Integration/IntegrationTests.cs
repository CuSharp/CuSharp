using System.Linq;
using CuSharp.Tests.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
public class IntegrationTests
{
    private readonly ITestOutputHelper _output;

    public IntegrationTests(ITestOutputHelper output)
    {
        _output = output;
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
        dev.Launch(MethodsToCompile.ArrayIntAdditionWithKernelTools, (1,1,1), ((uint) length,1,1), devA, devB, devC);
        c = dev.Copy(devC);

        _output.WriteLine($"Used gpu device: '{dev.ToString()}'");
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
        dev.Launch(MethodsToCompile.ArrayIntScalarAdd, (1,1,1), ((uint) length,1,1), devA, devB);
        a = dev.Copy(devA);
        _output.WriteLine($"Used gpu device: '{dev.ToString()}'");
        Assert.True(a.SequenceEqual(expected));
    }

    [Fact]
    public void TestMatrixMultiplication()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        int matrixWidth = 100;
        uint gridDim = (uint) (matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint) (matrixWidth > 32 ? 32 : matrixWidth);
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
        dev.Launch(MethodsToCompile.IntMatrixMultiplication, (gridDim,gridDim,1),  (blockDim , blockDim,1), devA, devB, devC, devWidth);
        

        c = dev.Copy(devC);

        Assert.Equal(expectedC, c);
    }
}