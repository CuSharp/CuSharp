using System.Drawing;
using System.Drawing.Imaging;

namespace CuSharp.PerformanceEvaluation;

public class MandelbrotGenerator
{
    public static void Generate()
    {
        int n = 10000;
        uint gridDim = (uint) (n % 32 == 0 ? n / 32 : n / 32 + 1);
        uint blockDim = (uint) (n > 32 ? 32 : n);
        int size = n * n;
        int maxIterations = 1000;
        var mandelbrot = new float[size];

        CuSharp.EnableOptimizer = true;
        var dev = CuSharp.GetDefaultDevice();

        var devMandelbrot = dev.Copy(mandelbrot);
        
        float zoom = 10000.0f;
        float deltaX = 2.0f;
        float deltaY = 0.5f;
        
        dev.Launch<float[], int, int, float,float,float >(Kernels.MandelBrot, (gridDim,gridDim,1), (blockDim, blockDim,1), devMandelbrot, maxIterations, n, zoom, deltaX, deltaY);
        mandelbrot = dev.Copy(devMandelbrot);
        SaveAsBitMap(mandelbrot, n, maxIterations, zoom.ToString() + ".png");
        /*for (int i = 0; i < 100; i++)
        {
            SaveAsBitMap(mandelbrot, n, maxIterations, i.ToString() + ".png");
            rFreq = r.NextDouble();
            rPhase = r.NextDouble();
            gFreq = r.NextDouble();
            gPhase = r.NextDouble();
            bFreq = r.NextDouble();
            bPhase = r.NextDouble();
            Console.WriteLine($"{i}: {rFreq},{rPhase},{gFreq},{gPhase},{bFreq},{bPhase}");
        }*/
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

    /*private static Random r = new Random();
    private static double rFreq = r.NextDouble();
    private static double rPhase = r.NextDouble();
    private static double gFreq = r.NextDouble();
    private static double gPhase = r.NextDouble();
    private static double bFreq = r.NextDouble();
    private static double bPhase = r.NextDouble();*/
    private static (int R, int G, int B) GetColor(float m, int maxIter)
    {
        if (m == maxIter)
        {
            return (0, 0, 0);
        }
        double rFreq = 0.2836873750601605;
        double rPhase = 0.8125687812576259;
        double gFreq = 0.27972962651164823;
        double gPhase = 0.394258208198057;
        double bFreq = 0.17974138093825753;
        double bPhase = 0.8435839954173746;

        int red = (int) Math.Abs(Math.Sin( rFreq * m + rPhase) * 255);
        int green = (int) Math.Abs(Math.Sin( gFreq * m + gPhase) * 255);
        int blue = (int) Math.Abs(Math.Sin( bFreq * m + bPhase) * 255);
        //int red =(int)( 255 / m);
        return (red, green, blue);
    }
}