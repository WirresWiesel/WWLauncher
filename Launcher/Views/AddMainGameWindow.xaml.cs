using System.Windows;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for AddProgramWindow.xaml
    /// </summary>
    public partial class AddMainGameWindow : Window
    {
        public Programinfo? Result { get; private set; }
        public AddMainGameWindow()
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
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = new Programinfo()
            {
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
