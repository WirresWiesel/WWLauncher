using Launcher.Logic;
using Launcher.Core.Models;
using Launcher.Core.Services;
using Launcher.Utils;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;

namespace Launcher.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LauncherLogic? _launcherLogic;
        private readonly AssetService _assetService;
        private readonly SettingsService _settingsService;

        public MainWindow()
        {
            InitializeComponent();

            _launcherLogic = new LauncherLogic();
            _assetService = new AssetService();
            _settingsService = new SettingsService();
            
            Loaded += (_, _) =>
            {
                InitializeLauncher();
            };
        }

        private void InitializeLauncher()
        {
            _settingsService.GetSettings();
            ComboAssets.ItemsSource = _assetService.Assets;
            _launcherLogic?.SetTheme(_settingsService.Settings);
            _launcherLogic?.SetStatusUpdateTimerInterval(_settingsService.Settings.StatusUpdateInterval);
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
                BtnDeleteAsset.IsEnabled = true;
                BtnStartProgramAll.IsEnabled = true;
                BtnStopProgramAll.IsEnabled = true;
                BtnEditAsset.IsEnabled = true;
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
                _assetService.SaveInternalAssetList();
            }
        }

        private void BtnClick_RemoveProgram(object sender, RoutedEventArgs e)
        { 
            var selectedProgram = LstProgram.SelectedItem as Programinfo;
            var confirmWindow = new RequestWindow($"Do you realy want to delete the program?\n -->{selectedProgram!.Name}");
            if (confirmWindow.ShowDialog() == true)
            {
                var currentAsset = ComboAssets.SelectedItem as Asset;
                if (selectedProgram != null && currentAsset != null)
                {
                    AssetService.RemoveProgramFromAsset(currentAsset, selectedProgram);
                    _launcherLogic?.UpdateLstProgram(LstProgram);
                    CheckProgramButtons();
                    _assetService.SaveInternalAssetList();
                }
            }
        }

        private void LstSelectionChanged_LstProgram(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CheckProgramButtons();
        }

        private void BtnClick_StartProgramSingle(object sender, RoutedEventArgs e)
        {
            var program = LstProgram.SelectedItem as Programinfo;
            //_launcherLogic?.UpdateLstProgram(LstProgram);       // Refresh the List before executing command
            _launcherLogic?.UpdateProgramStates();       // Refresh the State before executing command

            if (program != null && program.State != "Running")
                _launcherLogic?.StartProgram(program);

            _assetService.SaveInternalAssetList();      // needs to be saved because to save the ProcessName to the .json
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

            _assetService.SaveInternalAssetList();      // needs to be saved because to save the ProcessName to the .json
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
                _booleanState = _assetService.IsSetMainGamePath(_asset!);
            }
            else
            {
                _StringState = string.Empty;
            }

            EllipseMainGameSet.Fill = StatusVisualizer.VisualizeBoolean(_booleanState);

            if (_booleanState)
            {
                TxtBlock_StateMaingamePath.Text = "Main Game is set";
                ChkBoxStartMaingame.IsEnabled = true;
                ChkBoxStartMaingame.Opacity = 1;
            }
            else
            {
                TxtBlock_StateMaingamePath.Text = "Main Game is not set";
                ChkBoxStartMaingame.IsEnabled = false;
                ChkBoxStartMaingame.Opacity = 0.3;
            }
            this.SortListView(LstProgram);
            this.SortComboBox(ComboAssets);
        }

        private void BtnClick_CreateNewAsset(object sender, RoutedEventArgs e)
        {
            string assetName = string.Empty;
            string ExePath = string.Empty;
            Asset newAsset;

            var editAssetWindow = new EditWindow(assetName, null)
            {
                Owner = this
            };

            if (editAssetWindow.ShowDialog() == true)
            {
                assetName = editAssetWindow.NewName;
                ExePath = editAssetWindow.NewPath;
            }

            if (!(assetName == string.Empty))
            { 
                newAsset = _assetService.CreateAsset(assetName);
                _assetService.EditAsset(newAsset, assetName, ExePath);
                this.InitializeLauncher();

                // Make the ComboBox select the last created Asset
                Asset? _asset = _assetService.Assets.FirstOrDefault(p => p.Name == assetName);
                if (_asset != null)
                {
                    ComboAssets.SelectedItem = _asset;
                }
            }
            UpdateVisuals();
        }

        private void BtnClick_DeleteAsset(object sender, RoutedEventArgs e)
        {
            string currentAssetName = string.Empty;
            Asset? currentAsset = ComboAssets.SelectedItem as Asset;
            if (currentAsset != null)
            {
                currentAssetName = currentAsset.Name;
            }

            var requestWindow = new RequestWindow($"Do you realy want to delete this asset?\n --> {currentAssetName}");
            if (requestWindow.ShowDialog() == true)
            {
                _assetService.DeleteAsset(currentAsset);
                this.InitializeLauncher();
            }
        }

        private void BtnClick_OpenMenuSettings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new MenuSettingsWindow(_settingsService.Settings)
            {
                Owner = this
            };

            if (settingsWindow.ShowDialog() == true)
            {
                _settingsService.Settings = settingsWindow.Result;
                _settingsService.SaveSettings();
                _launcherLogic?.SetTheme(_settingsService.Settings);
                _launcherLogic?.SetStatusUpdateTimerInterval(_settingsService.Settings.StatusUpdateInterval);
            }
        }

        private void CheckProgramButtons()
        {
            if (LstProgram.Items.Count > 0)
            {
                BtnDeleteProgram.IsEnabled = true;
                BtnStartProgramSingle.IsEnabled = true;
                BtnStopProgramSingle.IsEnabled = true;
                BtnEditProgram.IsEnabled = true;
            }
            else
            {
                BtnDeleteProgram.IsEnabled = false;
                BtnStartProgramSingle.IsEnabled = false;
                BtnStopProgramSingle.IsEnabled = false;
                BtnEditProgram.IsEnabled = false;
            }
        }

        private void BtnClick_EditAsset(object sender, RoutedEventArgs e)
        {
            var asset = ComboAssets.SelectedItem as Asset;
            var index = ComboAssets.SelectedIndex;
            var editWindow = new EditWindow(asset!.Name, asset!.ExePath);
            if (editWindow.ShowDialog() == true)
            {
                asset.ExePath = editWindow.NewPath;
                asset.Name = editWindow.NewName;
                ComboAssets.Text = asset.Name;
            }
            UpdateVisuals();
        }

        private void BtnClick_EditProgram(object sender, RoutedEventArgs e)
        {
            var program = LstProgram.SelectedItem as Programinfo;
            var editWindow = new EditWindow(program!.Name, program!.ExePath);
            if (editWindow.ShowDialog() == true)
            {
                if (program != null)
                {
                    program.ExePath = editWindow.NewPath;
                    program.Name = editWindow.NewName;
                }
            }
            _assetService.SaveInternalAssetList();
            UpdateVisuals();
        }

        private void SortListView(ListView listView)
        {
            var _lstProgramView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            if (_lstProgramView != null)
            {
                _lstProgramView.SortDescriptions.Clear();
                _lstProgramView.SortDescriptions.Add(new SortDescription(nameof(Programinfo.Name), ListSortDirection.Ascending));
            }
        }

        private void SortComboBox(ComboBox comboBox)
        {
            var _comboBox = CollectionViewSource.GetDefaultView(ComboAssets.ItemsSource);
            if (_comboBox != null)
            {
                _comboBox.SortDescriptions.Clear();
                _comboBox.SortDescriptions.Add(new SortDescription(nameof(Asset.Name), ListSortDirection.Ascending));
            }
        }
    }
}