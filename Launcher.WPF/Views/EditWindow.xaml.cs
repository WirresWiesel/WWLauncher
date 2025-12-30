using Launcher.Core.Models;
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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        string _name = string.Empty;
        string _path = string.Empty;
        public string NewName = string.Empty;
        public string NewPath = string.Empty;
        public EditWindow(string name, string? path)
        {
            InitializeComponent();

            GetData(name, path);
            SetFields();
            TxtBoxCustomName.Focus();
            TxtBoxCustomName.SelectAll();
        }

        public void GetData(string name, string? path)
        {
            _name = name;
            if (path != null)
            {
                _path = path;
            }
        }

        public void SetFields()
        {
            TxtPath.Text = _path;
            TxtBoxCustomName.Text = _name;
        }

        private void BtnClick_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnClick_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            NewPath = TxtPath.Text;
            NewName = TxtBoxCustomName.Text;
            Close();
        }

        private void BtnClick_ChoosePath(object sender, RoutedEventArgs e)
        {
            string FileExePath = string.Empty;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                FileExePath = openFileDialog.FileName;
            }

            TxtPath.Text = FileExePath;

            if (string.IsNullOrEmpty(TxtBoxCustomName.Text))
                TxtBoxCustomName.Text = System.IO.Path.GetFileNameWithoutExtension(FileExePath);
        }
    }
}
