using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CuSharp.MandelbrotExample;

public class Image : INotifyPropertyChanged
{
    private ImageSource _img;
    public static int Width { get; } = 1400;
    private MandelbrotGenerator _generator = new MandelbrotGenerator(Width);
    private double _currentZoom = 1.0;
    private double _measurement = 0.0;
    private const double ZoomFactor = 2;
    private bool _isGenerating = false;
    private Dispatcher _dispatcher;
    public Image(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn, _dispatcher);
        ResetZoomCommand = new RelayCommand(OnResetZoom, CanResetZoom, _dispatcher);
        RandomizeColorCommand = new RelayCommand(OnRandomizeColor, CanRandomizeColor, _dispatcher);
        generateMandelbrot();
    }

    public double Measurement
    {
        get => _measurement;
        private set
        {
            value = Math.Round(value, 2);
            _measurement = value;
            OnPropertyChanged(nameof(Measurement));
        }
    }
    public double CurrentZoom
    {
        get => _currentZoom;
        private set
        {
            value = Math.Round(value, 2);
            _currentZoom = value;
            OnPropertyChanged(nameof(CurrentZoom));
        }
    } 
    public ImageSource ImageData
    {
        get => _img;
        private set
        {
            _img = value;
            OnPropertyChanged(nameof(ImageData));
        }
    }
    
    public ICommand ResetZoomCommand { get; }

    private bool CanResetZoom(object param)
    {
        return !_isGenerating;
    }

    private void OnResetZoom(object param)
    {
        CurrentZoom = 1.0;
        _generator = new MandelbrotGenerator(Width);
        generateMandelbrot();
    }
    
    public ICommand RandomizeColorCommand { get; }

    private bool CanRandomizeColor(object param)
    {
        return !_isGenerating;
    }

    private void OnRandomizeColor(object param)
    {
        Random r = new Random();
        _generator.RFreq = r.NextDouble();
        _generator.RPhase = r.NextDouble();
        _generator.GFreq = r.NextDouble();
        _generator.GPhase = r.NextDouble();
        _generator.BFreq = r.NextDouble();
        _generator.BPhase = r.NextDouble();
        generateMandelbrot();
    }
    
    public ICommand ZoomInCommand { get; }
    private bool CanZoomIn(object point)
    {
        return !_isGenerating;
    }
    public void OnZoomIn(object param)
    {
        CurrentZoom *= ZoomFactor;
        var point = Mouse.GetPosition((IInputElement) param);
        _generator.UpdatePosition(point, ZoomFactor);
        generateMandelbrot();
        
    }

    private void generateMandelbrot()
    {
        _isGenerating = true;
        (ZoomInCommand as RelayCommand)!.RaiseCanExecuteChanged();
        (ResetZoomCommand as RelayCommand)!.RaiseCanExecuteChanged();
        (RandomizeColorCommand as RelayCommand)!.RaiseCanExecuteChanged();
        Task.Run(() =>
        {
            var result = _generator.Generate(CurrentZoom);
            _dispatcher.Invoke(() =>
            {
                ImageData = BitmapToImageSource(result.bmp);
                Measurement = result.time;
            });
        }).ContinueWith(_ =>
        {
             _isGenerating = false;
             (ZoomInCommand as RelayCommand)!.RaiseCanExecuteChanged();
             (ResetZoomCommand as RelayCommand)!.RaiseCanExecuteChanged();
             (RandomizeColorCommand as RelayCommand)!.RaiseCanExecuteChanged();           
        });
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



    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        _dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
    }

}