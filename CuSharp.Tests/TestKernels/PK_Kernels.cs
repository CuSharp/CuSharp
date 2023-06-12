using System;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Kernel;
using System.Numerics;

namespace CuSharp.Tests.TestKernels;

public static class PK_Kernels
{
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
                //T sum = default(T); // TODO: Remove or implement
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
            //T sum = default(T); // TODO: Remove or implement
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