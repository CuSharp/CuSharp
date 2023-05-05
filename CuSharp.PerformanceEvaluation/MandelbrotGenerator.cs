using System.Drawing;
using System.Drawing.Imaging;

namespace CuSharp.PerformanceEvaluation;

public class MandelbrotGenerator
{
    public static void Generate()
    {
        int n = 2000;
        uint gridDim = (uint) (n % 32 == 0 ? n / 32 : n / 32 + 1);
        uint blockDim = (uint) (n > 32 ? 32 : n);
        int size = n * n;
        int maxIterations = 100;
        var mandelbrot = new float[size];

        CuSharp.EnableOptimizer = true;
        var dev = CuSharp.GetDefaultDevice();

        var devMandelbrot = dev.Copy(mandelbrot);
        
        //float zoom = 1000.0f;
        float deltaX = 2.0f;
        float deltaY = 1.0f;

        for (float zoom = 1000.0f; zoom < 10000.0f; zoom += 1000)
        {
            dev.Launch<float[], int, int, float,float,float >(Kernels.MandelBrot, (gridDim,gridDim,1), (blockDim, blockDim,1), devMandelbrot, maxIterations, n, zoom, deltaX, deltaY);
            mandelbrot = dev.Copy(devMandelbrot);
            SaveAsBitMap(mandelbrot, n, maxIterations, zoom.ToString() + ".png");
        }
        
    }

    private static void SaveAsBitMap(float[] mandelbrot, int width, int maxIterations, string filename)
    {
        
        Bitmap bmp = new Bitmap(width,width, PixelFormat.Format32bppPArgb);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                var m = mandelbrot[y * width + x];
                var color = GetColor(m, maxIterations);
                bmp.SetPixel(x, y, Color.FromArgb(255, color.R,color.G,color.B));
            }
        }
        
        bmp.Save(filename, ImageFormat.Png);
    }
    private static (int R, int G, int B) GetColor(float m, int maxIter)
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