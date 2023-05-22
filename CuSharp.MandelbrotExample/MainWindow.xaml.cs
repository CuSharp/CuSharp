
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace CuSharp.MandelbrotExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image image;
        private float zoom = 1.0f;
        public MainWindow()
        {
            image = new Image();
            image.Generate(zoom, new Point(501,0));
            InitializeComponent();
            this.DataContext = image;
        }

        private void MandelbrotImg_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            zoom *= 1.5f;
            var pos = e.GetPosition(this);
            image.Generate(zoom, pos);
        }
    }
}