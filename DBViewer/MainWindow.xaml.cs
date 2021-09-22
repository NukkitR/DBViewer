using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<KeyItem> keysDataSource { get; set; }

        private readonly OpenFileDialog dialog = new OpenFileDialog();

        private KeyItem chunks = new KeyItem { Name = "Chunks" };
        private KeyItem generalKeys = new KeyItem { Name = "General Keys" };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            keysDataSource = new ObservableCollection<KeyItem>();
            keysDataSource.Add(chunks);
            keysDataSource.Add(generalKeys);
        }

        private void OnMenuOpenClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("OnMenuOpenClick");
            dialog.FileName = "test file name blablabla...";
            dialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void OnControlsSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Console.WriteLine("OnControlsSearchBoxQuerySubmitted");
            // TODO:
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Console.WriteLine("OnControlsSearchBoxQuerySubmitted");
            // TODO:
        }
    }
}
