using System;
using System.Drawing;
using System.Drawing.Imaging;
using CuSharp.CudaCompiler.LLVMConfiguration;
using CuSharp.Kernel;
using Point = System.Windows.Point;

namespace CuSharp.MandelbrotExample;

public class MandelbrotGenerator
{
    
    private double _currentX = 2;
    private double _currentY = .5 ;
    private double _lastZoom = 1;

    private readonly int _n;
    private readonly uint _gridDim;
    private readonly uint _blockDim; 

    public MandelbrotGenerator(int width)
    {
        _n = width;
        _gridDim = (uint) (_n % 32 == 0 ? _n / 32 : _n / 32 + 1);
        _blockDim = (uint) (_n > 32 ? 32 : _n);
    }

    public void UpdatePosition(Point pos, double zoomFactor)
    {
        _currentX -= (pos.X - _n/ (zoomFactor / 0.5)) / _n / _lastZoom;
        _currentY -= (pos.Y - _n/ (zoomFactor / 0.5)) / _n / _lastZoom;
    }
    
    public (Bitmap bmp, double time) Generate(double zoom)
    {
        int maxIterations = 1000;
        var mandelbrot = new double[_n * _n];
        _lastZoom = zoom;
        
        Cu.EnableOptimizer = true;
        var dev = Cu.GetDefaultDevice();
        var devMandelbrot = dev.Copy(mandelbrot);

        var before = Cu.CreateEvent();
        var after = Cu.CreateEvent();
        before.Record();
        dev.Launch<double[], int, int, double,double,double >(MandelBrot, (_gridDim,_gridDim,1), (_blockDim, _blockDim,1), devMandelbrot, maxIterations, _n, zoom, _currentX, _currentY);
        after.Record();
        mandelbrot = dev.Copy(devMandelbrot);

        var bmp = new Bitmap(_n,_n, PixelFormat.Format32bppArgb);
        
        for (int x = 0; x < _n; x++)
        {
            for (int y = 0; y < _n; y++)
            {
                var m = mandelbrot[y * _n + x];
                var color = GetColor(m, maxIterations);
                bmp.SetPixel(x, y, Color.FromArgb(255, color.R,color.G,color.B));
            }
        }
        
        devMandelbrot.Dispose();
        return (bmp, before.GetDeltaTo(after));
    }
    
    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void MandelBrot(double[] light,  int maxIterations, int N, double zoom, double deltaX,  double deltaY)
    {
        var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;

        if (row < N && col < N)
        {
            double fromX = col / (double) N / zoom - deltaX; 
            double fromY = row / (double) N / zoom - deltaY; 
            double x = 0.0; 
            double y = 0.0;
            int iteration = 0;
            while (x * x + y * y <= 4 && iteration < maxIterations)
            {
                var xtemp = x * x - y * y + fromX;
                y = 2 * x * y + fromY;
                x = xtemp;
                iteration++;
            }
       
            light[row * N + col] = iteration;     
        }
    }

    public double RFreq = 0.2836873750601605;
    public double RPhase = 0.8125687812576259;
    public double GFreq = 0.27972962651164823;
    public double GPhase = 0.394258208198057;
    public double BFreq = 0.17974138093825753;
    public double BPhase = 0.8435839954173746;
    private (int R, int G, int B) GetColor(double m, int maxIter)
    {
        if (Math.Abs(m - maxIter) < 0.1)
        {
            return (0, 0, 0);
        }

        int red = (int) Math.Abs(Math.Sin( RFreq * m + RPhase) * 255);
        int green = (int) Math.Abs(Math.Sin( GFreq * m + GPhase) * 255);
        int blue = (int) Math.Abs(Math.Sin( BFreq * m + BPhase) * 255);
        return (red, green, blue);
    }
}