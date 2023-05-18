using System.Text.Json.Serialization;
using CuSharp;
using CuSharp.PerformanceEvaluation;

public class Program
{

    static void Main()
    {

        Test(1000,2000, 100, false);
    }

    static void Test(int min, int max, int step, bool verify)
    {
        CuSharp.CuSharp.EnableOptimizer = true;
        var dev = CuSharp.CuSharp.GetDefaultDevice();

        for (int i = min; i <= max; i += step)
        {
            Launch(i, dev, verify);
            double result = 0;
            for (int j = 0; j < 10; j++)
            {
                result += Launch(i, dev, verify);
            }

            result /= 10;
            Console.WriteLine($"Width: {i},Size: {i * i},\t Avg: {result}");
        }
        dev.Dispose();
    }

    static double Launch(int matrixWidth, CuDevice dev, bool verify)
    {

        uint gridDim = (uint) (matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint) (matrixWidth > 32 ? 32 : matrixWidth);
        double[] a = new double [matrixWidth * matrixWidth];
        double[] b = new double [matrixWidth * matrixWidth];
        double[] c = new double [matrixWidth * matrixWidth];
        for (double i = 0.0; i < matrixWidth * matrixWidth; i++)
        {
            a[(int) i] = i;
            b[(int) i] = matrixWidth * matrixWidth - i;
        }


        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        //var devWidth = dev.Copy(matrixWidth);
        var before = CuSharp.CuSharp.CreateEvent();
        var after = CuSharp.CuSharp.CreateEvent();
        
        before.Record();
        dev.Launch<double[], double[], double[], int>(Kernels.MatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC,matrixWidth);
        //dev.Launch<double[],double[],double[],int,int,int>(Kernels.TiledIntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim,1), devA, devB, devC, matrixWidth, 32, (int) (blockDim));
        
        after.Record();
        c = dev.Copy(devC);
        if (verify)
        {
            double[] expectedC = new double[matrixWidth * matrixWidth];
            Kernels.IntMatrixMultiplicationSequential(a, b, expectedC, matrixWidth, (int) gridDim, (int) blockDim);
            if (!c.SequenceEqual(expectedC))
            {
                throw new Exception("Result mismatch");
            }
        }
        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        var result = before.GetDeltaTo(after);
        before.Dispose();
        after.Dispose();
        return result;
    }
    
    static double LaunchMultiDim(int matrixWidth, CuDevice dev, bool verify)
    {

        uint gridDim = (uint) (matrixWidth % 32 == 0 ? matrixWidth / 32 : matrixWidth / 32 + 1);
        uint blockDim = (uint) (matrixWidth > 32 ? 32 : matrixWidth);
        double[,] a = new double [matrixWidth , matrixWidth];
        double[,] b = new double [matrixWidth , matrixWidth];
        double[,] c = new double [matrixWidth , matrixWidth];
        for (int i = 0; i < matrixWidth; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                a[i, j] = i * j;
                b[i, j] = (matrixWidth - i) * (matrixWidth - j);
            }
        }

        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        //var devWidth = dev.Copy(matrixWidth);
        var before = CuSharp.CuSharp.CreateEvent();
        var after = CuSharp.CuSharp.CreateEvent();
        
        before.Record();
        dev.Launch<double[,], double[,], double[,], int>(Kernels.MultiDimMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC,matrixWidth);
        
        after.Record();
        c = dev.Copy(devC);
        if (verify)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixWidth; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < matrixWidth; k++)
                    {
                        sum += a[k, j] * b[i, k];
                    }

                    if (Math.Abs(sum - c[i, j]) > ((double)matrixWidth / 10))
                    {
                        throw new Exception("Result mismatch");
                    }
                }
            }
        }
        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        var result = before.GetDeltaTo(after);
        before.Dispose();
        after.Dispose();
        return result;
    }
}
