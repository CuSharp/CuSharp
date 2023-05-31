
using System.Windows;


namespace CuSharp.MandelbrotExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image image;
        public MainWindow()
        {
            image = new Image(this.Dispatcher);
            InitializeComponent();
            DataContext = image;
        }
    }
}