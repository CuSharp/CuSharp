namespace CuSharp.PerformanceEvaluation;

public class MandelbrotGenerator
{
    public static void Generate()
    {
        int N = 1024;
        int size = N * N;
        int maxIterations = 100;
        var mandelbrot = new float[size];
        float fromX = 0, fromY = 0, h = 1;
        

        var dev = CuSharp.GetDefaultDevice();

        var devMandelbrot = dev.Copy(mandelbrot);
        var devN = dev.Copy(N);
        var devFromX = dev.Copy(fromX);
        var devFromY = dev.Copy(fromY);
        var devH = dev.Copy(h);
        var devMaxIterations = dev.Copy(maxIterations);
        
        dev.Launch(Kernels.MandelBrot, (1,1,1), ((uint) N, (uint)N,0), devMandelbrot, devMaxIterations, devN, devH, devFromX, devFromY);
        mandelbrot = dev.Copy(devMandelbrot);
        
    }
}