using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for RequestWindow.xaml
    /// </summary>
    public partial class RequestWindow : Window
    {
        public string Message { get; set; } = string.Empty;

        public RequestWindow(string message)
        {
            InitializeComponent();

            Message = message;
            DataContext = this;
        }

        private void BtnClick_Delete(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnClick_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
