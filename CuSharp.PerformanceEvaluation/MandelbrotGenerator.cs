using System.Drawing;
using System.Drawing.Imaging;

namespace CuSharp.PerformanceEvaluation;

public class MandelbrotGenerator
{
    public static void Generate()
    {
        int N = 2000;
        uint gridDim = (uint) (N % 32 == 0 ? N / 32 : N / 32 + 1);
        uint blockDim = (uint) (N > 32 ? 32 : N);
        int size = N * N;
        int maxIterations = 1000;
        var mandelbrot = new float[size];

        CuSharp.EnableOptimizer = true;
        var dev = CuSharp.GetDefaultDevice();

        var devMandelbrot = dev.Copy(mandelbrot);
        var devN = dev.Copy(N);
        var devMaxIterations = dev.Copy(maxIterations);
        
        float zoom = 1000.0f;
        var devZoom = dev.Copy(zoom);

        float deltaX = 2.0f;
        var devDeltaX = dev.Copy(deltaX);
        float deltaY = 1.0f;
        var devDeltaY = dev.Copy(deltaY);
        
        
        dev.Launch(Kernels.MandelBrot, (gridDim,gridDim,1), (blockDim, blockDim,1), devMandelbrot, devMaxIterations, devN, devZoom, devDeltaX, devDeltaY);
        mandelbrot = dev.Copy(devMandelbrot);
        Bitmap bmp = new Bitmap(N,N, PixelFormat.Format32bppPArgb);
        for (int x = 0; x < N; x++)
        {
            for (int y = 0; y < N; y++)
            {
                var m = mandelbrot[y * N + x];
                var color = GetColor(m, maxIterations);
                bmp.SetPixel(x, y, Color.FromArgb(255, color.Item1,color.Item2,color.Item3));
            }
        }
        
        bmp.Save("test.png", ImageFormat.Png);
    }

    private static (int,int,int) GetColor(float m, int maxIter)
    {
        if (m == maxIter)
        {
            return (0, 0, 0);
        }
        float rFreq = 0.2f;
        float rPhase = 0;
        float gFreq = 0.5f;
        float gPhase = 0;
        float bFreq = 0.3f;
        float bPhase = 0;
        int red = (int) Math.Abs(Math.Sin( rFreq * m + rPhase) * 255);
        int green = (int) Math.Abs(Math.Sin( gFreq * m + gPhase) * 255);
        int blue = (int) Math.Abs(Math.Sin( bFreq * m + bPhase) * 255);
        //int red =(int)( 255 / m);
        return (red, green, blue);
    }
}