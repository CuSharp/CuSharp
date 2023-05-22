using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Kernel;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CuSharp.MandelbrotExample;

public class Image : INotifyPropertyChanged
{
    private ImageSource _img;
    public ImageSource ImageData
    {
        get => _img;
        private set
        {
            _img = value;
            OnPropertyChanged(nameof(ImageData));
        }
    }

    public void Generate(double zoom, System.Windows.Point pos)
    {
        int n = 1000;
        uint gridDim = (uint) (n % 32 == 0 ? n / 32 : n / 32 + 1);
        uint blockDim = (uint) (n > 32 ? 32 : n);
        int size = n * n;
        int maxIterations = 1000;
        var mandelbrot = new double[size];
        
        CuSharp.EnableOptimizer = true;
        var dev = CuSharp.GetDefaultDevice();

        var devMandelbrot = dev.Copy(mandelbrot);
        
        double deltaX =  1.40006849; //<<-- cool cooridnate
        double deltaY =  0.119594f; //<<-- cool coordinate

        dev.Launch<double[], int, int, double,double,double >(MandelBrot, (gridDim,gridDim,1), (blockDim, blockDim,1), devMandelbrot, maxIterations, n, zoom, deltaX, deltaY);
        mandelbrot = dev.Copy(devMandelbrot);

        var bmp = new Bitmap(n,n, PixelFormat.Format32bppArgb);
        
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                var m = mandelbrot[y * n + x];
                var color = GetColor(m, maxIterations);
                bmp.SetPixel(x, y, Color.FromArgb(255, color.R,color.G,color.B));
            }
        }
        
        ImageData = BitmapToImageSource(bmp);
    }
    
    BitmapImage BitmapToImageSource(Bitmap bitmap)
    {
        using (MemoryStream memory = new MemoryStream())
        {
            bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }
    }
    private static (int R, int G, int B) GetColor(double m, int maxIter)
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
    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void MandelBrot(double[] light,  int maxIterations, int N, double zoom, double deltaX,  double deltaY)
    {
        var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;

        if (row < N && col < N)
        {
            double fromX = (col) / (N * zoom ) - (0.5f/zoom) - deltaX; 
            double fromY = (row) / (N * zoom ) - (0.5f/zoom) - deltaY; 
            double x = 0.0f; 
            double y = 0.0f;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}