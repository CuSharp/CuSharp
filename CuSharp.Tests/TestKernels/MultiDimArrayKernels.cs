using System.Numerics;
using CuSharp.CudaCompiler.LLVMConfiguration;
using CuSharp.Kernel;

namespace CuSharp.Tests.TestKernels;

public class MultiDimArrayKernels
{
    [Kernel]
    public static void MultiDimKernel(int[,] a, int b)
    {
        a[0, 0] = b;
    }

    [Kernel]
    public static void MultiDimArrayAddition(int[,] a, int[,] b)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        a[x, y] += b[x, y];
    }

    public static void MultiDimLocalArrayTest(int[,] a, int[,] b)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        int[,] arr = new int[3, 3];
        arr[x, y] = a[y, x];
        b[y, x] = arr[x, y];
    }

    public static void MultiDimMatrixMultiplication(int[,] a, int[,] b, int[,] c)
    {
        var x = KernelTools.ThreadIndex.X;
        var y = KernelTools.ThreadIndex.Y;
        int sum = 0;
        for (int i = 0; i < KernelTools.BlockDimension.X; i++)
        {
            sum += a[i,y] * b[x,i];
        }

        c[x, y] = sum;
    }

    public static void MultiDimArrayAssignToParam(int[,] a)
    {
        var x = new int[1, 1];
        a = x;
    }
    
    public static void MultiDimArrayAssignToLocal(int[,] a)
    {
        var x = a;
        a[0, 0] = 1337;
    }

    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void VeryNestedArrayAccess(int[,] a, int[,] c)
    {
        int[,] b = new int[5, 5];
        KernelTools.SyncThreads();
        if (a[1,1] == 42)
        {
            for (int i = 0; i < a[1, 1]; i++)
            {
                if (a[1, 1] == 42)
                {
                    b[1,1] = a[1,1];
                }
            }
        }
        c[1, 1] = b[1, 1];
    }
    
    public static void VeryNestedArrayAccess2(int[] a, int[] c)
    {
        var b = new int[5];
        if (a[0] == 42)
        {
            b[0] = a[0];
        }

        c[0] = b[0];
    }

    public static void ArrayConvert1Dto2D<T>(T[] input, T[,] output, int length) where T : INumber<T>
    {
        var start = KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X;
        var stride = KernelTools.GridDimension.X * KernelTools.BlockDimension.X;

        for (var index = start; index < length; index += stride)
        {
            int index0 = (int)index % length;
            int index1 = (int)index / length;
            output[index0, index1] = input[index];
        }
    }

    public static void ArrayConvert2Dto1D<T>(T[,] input, int length1, int length2, T[] output) where T : INumber<T>
    {
        var start = KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X;
        var stride = KernelTools.GridDimension.X * KernelTools.BlockDimension.X;
        var index = 0;
        for (var i = start; i < length1; i++)
        {
            for (int j = 0; j < length2; j++)
            {
                output[index] = input[i, j];
                index++;
            }
        }
    }

    private const int TileSize32 = 32;
    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void MatrixTranspose<T>(T[,] m, int blockRows, int rows, int cols, T[,] t) where T : INumber<T>
    {
        T[,] shared = new T[TileSize32, TileSize32 + 1]; // avoid bank conflicts

        uint x = KernelTools.BlockIndex.X * TileSize32 + KernelTools.ThreadIndex.X;
        uint y = KernelTools.BlockIndex.Y * TileSize32 + KernelTools.ThreadIndex.Y;

        if (x < cols)
        {
            for (int j = 0; j < TileSize32; j += blockRows)
            {
                if (y + j < rows)
                {
                    shared[KernelTools.ThreadIndex.Y + j, KernelTools.ThreadIndex.X] = m[y + j, x];
                }
            }
        }

        KernelTools.SyncThreads();

        x = KernelTools.BlockIndex.Y * TileSize32 + KernelTools.ThreadIndex.X; // transpose block offset
        y = KernelTools.BlockIndex.X * TileSize32 + KernelTools.ThreadIndex.Y;

        if (x < rows)
        {
            for (int j = 0; j < TileSize32; j += blockRows)
            {
                if (y + j < cols)
                {
                    t[y + j, x] = shared[KernelTools.ThreadIndex.X, KernelTools.ThreadIndex.Y + j];
                }
            }
        }
    }

    private const int TileSize16 = 16;
    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void MatrixProduct<T>(T[,] p, T[,] a, T[,] b, int paRows, int pbCols, int aCols_bRows) where T : INumber<T>, new()
    {
        int tx = (int)KernelTools.ThreadIndex.X;
        int ty = (int)KernelTools.ThreadIndex.Y;

        var totalBlocksY = (paRows + TileSize16 - 1) / TileSize16;
        for (int by = (int)KernelTools.BlockIndex.Y; by < totalBlocksY; by += (int)KernelTools.GridDimension.Y)
        {
            var totalBlocksX = (pbCols + TileSize16 - 1) / TileSize16;
            for (int bx = (int)KernelTools.BlockIndex.X; bx < totalBlocksX; bx += (int)KernelTools.GridDimension.X)
            {
                int pRow = by * TileSize16 + ty;
                int pCol = bx * TileSize16 + tx;
                var sA = new T[TileSize16, TileSize16];
                var sB = new T[TileSize16, TileSize16];

                T sum = new T();
                for (int tile = 0; tile < (aCols_bRows + TileSize16 - 1) / TileSize16; tile++)
                {
                    int tileOffset = tile * TileSize16;
                    if (pRow < paRows && tileOffset + tx < aCols_bRows)
                    {
                        sA[ty, tx] = a[pRow, tileOffset + tx];
                    }
                    if (pCol < pbCols && tileOffset + ty < aCols_bRows)
                    {
                        sB[ty, tx] = b[tileOffset + ty, pCol];
                    }

                    KernelTools.SyncThreads();

                    if (pRow < paRows && pCol < pbCols)
                    {
                        for (int kd = 0; kd < TileSize16; kd++)
                        {
                            int k = tileOffset + kd;
                            if (k < aCols_bRows)
                            {
                                sum += sA[ty, kd] * sB[kd, tx];
                            }
                        }
                    }
                    KernelTools.SyncThreads();
                }
                if (pRow < paRows && pCol < pbCols)
                {
                    p[pRow, pCol] = sum;
                }
            }
        }
    }

    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void MatrixVectorProduct<T>(T[] p, T[,] a, T[] b, int paRows, int aCols_bRows)
        where T : INumber<T>, new()
    {
        int tx = (int)KernelTools.ThreadIndex.X;
        int ty = (int)KernelTools.ThreadIndex.Y;
        var totalBlocksY = (paRows + TileSize16 - 1) / TileSize16;

        for (int by = (int)KernelTools.BlockIndex.Y; by < totalBlocksY; by += (int)KernelTools.GridDimension.Y)
        {
            int pRow = by * TileSize16 + ty;
            var sA = new T[TileSize16, TileSize16];
            var sB = new T[TileSize16, TileSize16];

            T sum = new T();
            for (int tile = 0; tile < (aCols_bRows + TileSize16 - 1) / TileSize16; tile++)
            {
                int tileOffset = tile * TileSize16;
                if (pRow < paRows && tileOffset + tx < aCols_bRows)
                {
                    sA[ty, tx] = a[pRow, tileOffset + tx];
                }

                if (tileOffset + ty < aCols_bRows)
                {
                    sB[ty, tx] = b[tileOffset + ty];
                }

                KernelTools.SyncThreads();

                if (pRow < paRows)
                {
                    for (int kd = 0; kd < TileSize16; kd++)
                    {
                        int k = tileOffset + kd;
                        if (k < aCols_bRows)
                        {
                            sum += sA[ty, kd] * sB[kd, tx];
                        }
                    }
                }

                KernelTools.SyncThreads();
            }

            if (pRow < paRows)
            {
                p[pRow] = sum;
            }
        }
    }
}