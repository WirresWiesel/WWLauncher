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
    /// Interaction logic for AddAssetWindow.xaml
    /// </summary>
    public partial class AddAssetWindow : Window
    {
        public string AssetName { get; private set; } = string.Empty;

        public AddAssetWindow()
        {
            InitializeComponent();

            TxtAssetName.Focus();
        }

        private void BtnClick_OK(object sender, RoutedEventArgs e)
        {
            AssetName = TxtAssetName.Text.Trim();
            if (string.IsNullOrEmpty(AssetName))
            {
                MessageBox.Show("Please enter a valid asset name.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnClick_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
