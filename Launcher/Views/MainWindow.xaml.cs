using Launcher.Logic;
using Launcher.Models;
using Launcher.Services;
using Launcher.Utils;
using Microsoft.VisualBasic;
using System.Windows;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LauncherLogic? _launcherLogic;
        private readonly AssetService _assetService;

        public MainWindow()
        {
            InitializeComponent();

            _launcherLogic = new LauncherLogic();
            _assetService = new AssetService();
            this.InitializeLauncher();
        }

        private void InitializeLauncher()
        {
            ComboAssets.ItemsSource = _assetService.GetAssetList();
            this.UpdateVisuals();
        }

        private void ComboAssets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Asset? _tmpAsset = ComboAssets.SelectedItem as Asset;
            _launcherLogic?.OnAssetSelected(_tmpAsset!);
            _launcherLogic?.UpdateLstProgram(LstProgram);
            _launcherLogic?.UpdateProgramStates();
            this.UpdateVisuals();

            if (_tmpAsset != null)
            {
                BtnAddProgram.IsEnabled = true;
            }
        }

        private void BtnClick_AddProgram(object sender, RoutedEventArgs e)
        {
            var window = new Views.AddProgramWindow();
            window.ShowDialog();

            if (window.DialogResult == true && window.Result != null)
            {
                _launcherLogic?.AddProgramToCurrentAsset(window.Result);
                _launcherLogic?.UpdateLstProgram(LstProgram);
                _launcherLogic?.UpdateProgramStates();
                _assetService.SaveAssetList();
            }
        }

        private void BtnClick_AddMaingame(object sender, RoutedEventArgs e)
        {
            var window = new Views.AddProgramWindow();
            window.ShowDialog();

            if (window.DialogResult == true && window.Result != null)
            {
                _launcherLogic?.AddMaingameToCurrentAsset(window.Result);
                _assetService.SaveAssetList();
            }
        }

        private void BtnClick_RemoveProgram(object sender, RoutedEventArgs e)
        {
            var selectedProgram = LstProgram.SelectedItem as Programinfo;
            var currentAsset = ComboAssets.SelectedItem as Asset;
            if (selectedProgram != null && currentAsset != null)
            {
                AssetService.RemoveProgramFromAsset(currentAsset, selectedProgram);
                _launcherLogic?.UpdateLstProgram(LstProgram);
                _assetService.SaveAssetList();
            }
        }

        private void LstSelectionChanged_LstProgram(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            BtnDeleteProgram.IsEnabled = true;
            BtnStartProgramSingle.IsEnabled = true;
            BtnStopProgramSingle.IsEnabled = true;
        }

        private void BtnClick_StartProgramSingle(object sender, RoutedEventArgs e)
        {
            var program = LstProgram.SelectedItem as Programinfo;
            //_launcherLogic?.UpdateLstProgram(LstProgram);       // Refresh the List before executing command
            _launcherLogic?.UpdateProgramStates();       // Refresh the State before executing command

            if (program != null && program.State != "Running")
                _launcherLogic?.StartProgram(program);

            _assetService.SaveAssetList();      // needs to be saved because to save the ProcessName to the .json
        }

        private void BtnClick_StopProgramSingle(object sender, RoutedEventArgs e)
        {
            var program = LstProgram.SelectedItem as Programinfo;
            //_launcherLogic?.UpdateLstProgram(LstProgram);       // Refresh the List before executing command
            _launcherLogic?.UpdateProgramStates();       // Refresh the State before executing command

            if (program != null && program.State == "Running")
                _launcherLogic?.StopProgram(program);
        }

        private void BtnClick_StartProgramAll(object sender, RoutedEventArgs e)
        {
            //_launcherLogic?.UpdateLstProgram(LstProgram);       // Refresh the List before executing command
            _launcherLogic?.UpdateProgramStates();       // Refresh the State before executing command
            Asset? _asset = ComboAssets.SelectedItem as Asset;

            if (ChkBoxStartMaingame.IsChecked == true && _asset != null)
                _launcherLogic?.StartProgram(_asset);

            foreach (Programinfo program in LstProgram.Items)
            {
                if(program.State != "Running")
                    _launcherLogic?.StartProgram(program);
            }

            _assetService.SaveAssetList();      // needs to be saved because to save the ProcessName to the .json
        }

        private void BtnClick_StopProgramAll(object sender, RoutedEventArgs e)
        {
            //_launcherLogic?.UpdateLstProgram(LstProgram);       // Refresh the List before executing command
            _launcherLogic?.UpdateProgramStates();       // Refresh the State before executing command
            Asset? _asset = ComboAssets.SelectedItem as Asset;

            if (ChkBoxStartMaingame.IsChecked == true && _asset != null)
                _launcherLogic?.StopProgram(_asset);

            foreach (Programinfo program in LstProgram.Items)
            {
                if (program.State == "Running")
                    _launcherLogic?.StopProgram(program);
            }
        }

        private void UpdateVisuals()
        {
            Asset? _asset = ComboAssets.SelectedItem as Asset;
            Programinfo? _programinfo = _asset;
            string _StringState;
            bool _booleanState = false;

            if (_asset != null)
            {
                _StringState = _asset.State;
                _booleanState = _assetService.IsSetMaingamePath(_asset!);
            }
            else
            {
                _StringState = string.Empty;
            }

            EllipseMainGameSet.Fill = StatusVisualizer.VisualizeBoolean(_booleanState);

            if (_booleanState)
            {
                TxtBlock_StateMaingamePath.Text = "Maingame is set";
            }
            else
            {
                TxtBlock_StateMaingamePath.Text = "Maingame is not set";
            }
        }

        private void BtnClick_CreateNewAsset(object sender, RoutedEventArgs e)
        {
            string assetName = Interaction.InputBox(
                "Name des neuen Assets:",
                "Neues Asset erstellen",
                "Neues Asset");

            _assetService.CreateAsset(assetName);
            this.InitializeLauncher();

            // Make the ComboBox select the last created Asset
            Asset? _asset = _assetService.Assets.FirstOrDefault(p => p.Name == assetName);
            if ( _asset != null)
            {
                ComboAssets.SelectedItem = _asset;
            }
        }
    }
}