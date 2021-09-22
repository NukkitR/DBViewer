using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBViewer
{
    public partial class OpenFileDialog : INotifyPropertyChanged
    {

        private string fileName;

        public OpenFileDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string FileName {
            get { return fileName; }
            set
            {
                if (string.Equals(value, fileName))
                    return;
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO:
        }

        private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO:
        }

        private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.GetDeferral().Complete();
        }

        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            // TODO:
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
