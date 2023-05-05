using CuSharp.Kernel;

namespace CuSharp.PerformanceEvaluation;

public static class Kernels
{
    
    public static void IntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth)
    {
        var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;
        int result = 0;
        if (row < matrixWidth && col < matrixWidth)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                //KernelTools.SyncThreads();
                result += a[matrixWidth * row + i] * b[i * matrixWidth + col];
            }

            c[row * matrixWidth + col] = result;
        }
    }

    public static void TiledIntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth, int tileWidth, int nofTiles)
    {
        var tx = KernelTools.ThreadIndex.X;
        var ty = KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockIndex.X * tileWidth + tx;
        var row = KernelTools.BlockIndex.Y * tileWidth + ty;
        
        var aSub = new int[1024];
        var bSub = new int[1024];

        var sum = 0;
        for (int tile = 0; tile < nofTiles; tile++)
        {
            if (row < matrixWidth && tile * tileWidth + tx < matrixWidth)
            {
                aSub[ty * tileWidth + tx] = a[row * matrixWidth + tile * tileWidth + tx];
            }

            if (col < matrixWidth && tile * tileWidth + ty < matrixWidth)
            {
                bSub[ty * tileWidth + tx] = b[(tile * tileWidth + ty) * matrixWidth + col];
            }

            KernelTools.SyncThreads();

            if (row < matrixWidth && col < matrixWidth)
            {
                for (int ksub = 0; ksub < tileWidth; ksub++)
                {
                    if (tile * tileWidth + ksub < matrixWidth)
                    {
                        sum += aSub[ty * tileWidth + ksub] * bSub[ksub * tileWidth + tx];
                    }
                }
            }
            KernelTools.SyncThreads();
        }
        if (row < matrixWidth && col < matrixWidth)
        {
            c[row * matrixWidth + col] = sum;
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
        
        var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;

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