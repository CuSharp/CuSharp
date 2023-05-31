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
        var dev = Cu.GetDefaultDevice();
        var a = new int[,] {{5}};
        var devA = dev.Copy(a);
        dev.Launch<int[,], int>(MultiDimArrayKernels.MultiDimKernel, (1,1,1),(1,1,1), devA, 42);
        a = dev.Copy(devA);
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

        Assert.Equal(expected, c);
    }

    [Fact]
    public void TestAssignLocalToParam()
    {
        var dev = Cu.GetDefaultDevice();
        var a = dev.Allocate<int>(1, 1);
        dev.Launch(MultiDimArrayKernels.MultiDimArrayAssignToParam, (1,1,1), (1,1,1), a);
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
        var devC = dev.Allocate<int>(5,5);
        var devA = dev.Copy(a);
        dev.Launch(MultiDimArrayKernels.VeryNestedArrayAccess, (1,1,1), (1,1,1), devA, devC);
        c = dev.Copy(devC);
        Assert.Equal(42, c[1,1]);
    }
     [Fact]
     public void TestVeryNestedArrays2()
     {
         var dev = Cu.GetDefaultDevice();
         var a = new int[5];
         a[0] = 42; 
         var devA = dev.Copy(a);
         var devC = dev.Allocate<int>(5);
         dev.Launch(MultiDimArrayKernels.VeryNestedArrayAccess2, (1,1,1), (1,1,1), devA,devC);
         var c = dev.Copy(devC);
         Assert.Equal(42, c[0]);

     }   
}