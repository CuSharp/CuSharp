using CuSharp.Kernel;

namespace CuSharp.PerformanceEvaluation;

public static class Kernels
{
    
        public static void IntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth)
        {
            var row = KernelTools.BlockDimensions.Item2 * KernelTools.BlockIndex.Item2 + KernelTools.ThreadIndex.Item2;
            var col = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
            int result = 0;
            if (row < matrixWidth && col < matrixWidth)
            {
                for (int i = 0; i < matrixWidth; i++)
                {
                    result += a[matrixWidth * row + i] * b[i * matrixWidth + col];
                }

                c[row * matrixWidth + col] = result;
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

    public static void MandelBrot(float[] light,  int maxIterations, int N, float zoom, float deltaX, float deltaY)
    {
        
        var row = KernelTools.BlockDimensions.Item2 * KernelTools.BlockIndex.Item2 + KernelTools.ThreadIndex.Item2;
        var col = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;

        if (row < N && col < N)
        {
            float fromX = col / zoom - deltaX;
            float fromY = row / zoom - deltaY;
            float x = 0.0f;
            float y = 0.0f;
            int iteration = 0;
            while (x * x + y * y <= 2 * 2 && iteration < maxIterations)
            {
                var xtemp = x * x - y * y + fromX;
                y = 2 * x * y + fromY;
                x = xtemp;
                iteration++;
            }
       
            light[row * N + col] = iteration;     
        }
        
    }
}