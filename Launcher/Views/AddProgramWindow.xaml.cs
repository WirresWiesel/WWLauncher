using System.Windows;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for AddProgramWindow.xaml
    /// </summary>
    public partial class AddProgramWindow : Window
    {
        public Programinfo? Result { get; private set; }
        public AddProgramWindow()
        {
            InitializeComponent();
        }

        private void BtnChoosePath_Click(object sender, RoutedEventArgs e)
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
            TxtBoxCustomName.Text = System.IO.Path.GetFileNameWithoutExtension(FileExePath);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = new Programinfo()
            {
                Name = TxtBoxCustomName.Text,
                EXEPath = TxtPath.Text ?? string.Empty
            };

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
