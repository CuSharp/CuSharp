// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using CuSharp;
using CuSharp.CudaCompiler;
using CuSharp.PerformanceEvaluation;

public class Program
{

    static void Main()
    {

        Test(1000, 5000, 1000, false);
    }

    static void Test(int min, int max, int step, bool verify)
    {
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
    }

    static double Launch(int matrixWidth, CuDevice dev, bool verify)
    {

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


        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        var devC = dev.Copy(c);
        var devWidth = dev.Copy(matrixWidth);
        CuSharp.CuSharp.StartTimer();
        dev.Launch(Kernels.IntMatrixMultiplication, (gridDim, gridDim, 1), (blockDim, blockDim, 1), devA, devB, devC,
            devWidth);
        var result = CuSharp.CuSharp.GetTimeMS();
        c = dev.Copy(devC);
        if (verify)
        {
            int[] expectedC = new int[matrixWidth * matrixWidth];
            Kernels.IntMatrixMultiplicationSequential(a, b, expectedC, matrixWidth, (int) gridDim, (int) blockDim);
            if (!c.SequenceEqual(expectedC))
            {
                throw new Exception("Result mismatch");
            }
        }
        devA.Dispose();
        devB.Dispose();
        devC.Dispose();
        devWidth.Dispose();
        return result;
    }
}
