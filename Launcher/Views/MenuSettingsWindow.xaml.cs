using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for MenuSettingsWindow.xaml
    /// </summary>
    public partial class MenuSettingsWindow : Window
    {
        public Settings.Settings Result = new Settings.Settings();
        private Settings.Settings _settingsCopy = new Settings.Settings();

        public MenuSettingsWindow(Settings.Settings currentSettings)
        {
            InitializeComponent();

            _settingsCopy = currentSettings.Clone();
            ChkBox_EnableDarkMode.IsChecked = currentSettings.IsDarkMode;
            SldStatusUpdateInterval.Value = currentSettings.StatusUpdateInterval;
            TxtStatusUpdateIntervalValue.Text = currentSettings.StatusUpdateInterval.ToString();
        }

        private void BtnClick_OK(object sender, RoutedEventArgs e)
        {
            Result = _settingsCopy;
            DialogResult = true;
            Close();
        }

        private void BtnClick_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChkBox_EnableDarkMode_Click(object sender, RoutedEventArgs e)
        {
            if (ChkBox_EnableDarkMode.IsChecked == true)
                _settingsCopy.IsDarkMode = true;
            else
                _settingsCopy.IsDarkMode = false;
        }

        private void SldChanged_StatusUpdateInterval(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _settingsCopy.StatusUpdateInterval = (int)SldStatusUpdateInterval.Value;
            UpdateTxtStatusUpdateInterval();
        }

        private void UpdateTxtStatusUpdateInterval()
        {
            //Debug.WriteLine("Sld changed");
            string _interval = _settingsCopy.StatusUpdateInterval.ToString();
            if (TxtStatusUpdateIntervalValue != null)
                TxtStatusUpdateIntervalValue.Text = _interval;
        }
    }
}
