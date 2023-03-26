﻿namespace CuSharp.PerformanceEvaluation;

public static class Kernels
{
    
        public static void IntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth)
        {
            var row = KernelTools.BlockDimensions.Item2 * KernelTools.BlockIndex.Item2 + KernelTools.ThreadIndex.Item2;
            var col = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
            int result = 0;
            if (row < matrixWidth)
            {
                if (col < matrixWidth)
                {
                    for (int i = 0; i < matrixWidth; i = i + 1)
                    {
                        result = result + a[matrixWidth * row + i] * b[i * matrixWidth + col];
                    }

                    c[row * matrixWidth + col] = result;
                }
            }
        }

    public static void IntMatrixMultiplicationSequential(int[] a, int[] b, int[] c, int matrixWidth, int gridDim, int blockDim) {
        
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

                    c[row * matrixWidth + col] = result;
                }
            }
        }

    }
}