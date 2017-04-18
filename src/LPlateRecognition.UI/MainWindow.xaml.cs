using LPlateRecognition.UI.ViewModels;
using System.Windows;

namespace LPlateRecognition.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = (_vm = new MainViewModel());

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            string imageLoc = ((string[]) e.Data.GetData(DataFormats.FileDrop, false))[0];

            _vm.Init(imageLoc);
        }
    }
}
