using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class MultiDimArrayTests
{

    [Fact]
    public void TestMultiDimKernels()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[,] {{5}};
        var devA = dev.Copy(a);
        dev.Launch<int[,], int>(MultiDimArrayKernels.MultiDimKernel, (1,1,1),(1,1,1), devA, 42);
        a = dev.Copy(devA);
        Assert.Equal(42, a[0,0]);
    }
    
    [Fact]
    public void TestMultiDimArrayAdditionKernel()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[,] {{1,2,3},{4,5,6}, {7,8,9}};
        var b = new int[,] {{9, 8, 7}, {6, 5, 4}, {3, 2, 1}};
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MultiDimArrayKernels.MultiDimArrayAddition, (1,1,1),(3,3,1), devA, devB);
        a = dev.Copy(devA);
        var expected = new [,] {{10, 10, 10}, {10, 10, 10}, {10, 10, 10}};
        Assert.Equal(expected, a);
    }

    [Fact]
    public void TestMultiDimLocalArrayAdditionKernel()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}};
        var b = new int[,] {{9, 8, 7}, {6, 5, 4}, {3, 2, 1}};
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(MultiDimArrayKernels.MultiDimLocalArrayTest, (1, 1, 1), (3, 3, 1), devA, devB);
        b = dev.Copy(devB);
        Assert.Equal(a, b);
    }
    [Fact]
    public void TestMultiDimMatrixMultiplication()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
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

        Assert.Equal(expected, c);
    } 
}