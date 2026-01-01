using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Core.Models;
using Launcher.Core.ViewModels;
using Launcher.Core.Services;
using Launcher.Core.Utils;
using Launcher.WinUI.Pages;
using Microsoft.UI.Input.DragDrop;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Media.AppBroadcasting;
using Windows.Media.ContentRestrictions;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using WinRT.Interop;
using Windows.UI.ViewManagement;

namespace Launcher.WinUI.Logic
{
    public partial class UILogic : ObservableObject
    {
        private readonly AssetService _assetService = new();
        private CommandHelper _commandHelper = new();
        public ObservableCollection<Asset> Assetlist { get; private set; } = new();
        public ObservableCollection<ProgramViewModel> Programlist { get; private set; } = new();

        public ICommand StartProgram { get; }
        public ICommand StopProgram { get; }
        public ICommand StartAsset { get; }
        public ICommand StopAsset { get; }
        public ICommand Browse { get; }
        public ICommand Delete { get; }
        public ICommand AddProgram { get; }
        public ICommand Save { get; }
        public ICommand AddAsset { get; }
        public ICommand DeleteAsset { get; }
        public ICommand DeleteMainGamePath { get;  }

        private AssetViewModel? _editableSelectedAsset = new AssetViewModel(new Asset());
        public AssetViewModel? EditableSelectedAsset
        {
            get => _editableSelectedAsset;
            set
            {
                if (SetProperty(ref _editableSelectedAsset!, value))
                {
                    Debug.WriteLine($"[Info] UILogic: Property changed EditableSelectedAsset");
                    _editableSelectedAsset.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);
                }
            }
        }

        private ObservableCollection<AssetViewModel> _editableAssetlist = new();
        public ObservableCollection<AssetViewModel> EditableAssetlist
        {
            get => _editableAssetlist;
            set
            {
                if (SetProperty(ref _editableAssetlist, value))
                {
                    Debug.WriteLine($"[Info] UILogic: Property changed EditableAssetlist");
                }
            }
        }

        private AssetViewModel _selectedAsset = new AssetViewModel(new Asset());

        public AssetViewModel? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                if (SetProperty(ref _selectedAsset!, value))
                {
                    if (value != null)
                    {
                        Debug.WriteLine($"[Info] UILogic: Property changed \"{SelectedAsset?.Name}\"");
                        EditableSelectedAsset = new AssetViewModel(value);
                        //EditableAssetName = EditableSelectedAsset!.Name;
                        if (EditableSelectedAsset.IsSetExePath())
                        {
                            this.SetModeSelector(true);
                        }
                        else
                        {
                            this.SetModeSelector(false);
                        }
                        this.SetDirtyFlag(false);
                    }
                    this.UpdateProgramlists();
                }
            }
        }

        /*
        private string _editableAssetName = string.Empty;
        public string? EditableAssetName
        {
            get => _editableAssetName;
            set
            {
                if (SetProperty(ref _editableAssetName!, value))
                {
                    if (EditableAssetName != null)
                    {
                        Debug.WriteLine($"[Info] UILogic: Property changed \"{EditableAssetName}\"");
                        EditableSelectedAsset!.Name = EditableAssetName;
                        this.SetDirtyFlag(true);
                    }
                }
            }
        }
        */

        public bool _dirtyFlag;
        public bool DirtyFlag
        {
            get => _dirtyFlag;
            set => SetProperty(ref _dirtyFlag, value);
        }

        public bool _assetStartMode;
        public bool AssetStartMode
        {
            get => _assetStartMode;
            set
            {
                if (SetProperty(ref _assetStartMode, value))
                {
                    Debug.WriteLine($"[Info] UILogic: Property changed AssetStartMode to \"{value}\"");
                };
            }
        }

        public bool _modeSelectorActive;
        public bool ModeSelectorActive
        {
            get => _modeSelectorActive;
            set
            {
                if (SetProperty(ref _modeSelectorActive, value))
                {
                    Debug.WriteLine($"[Info] UILogic: Property changed ModeSelectorActive to \"{value}\"");
                };
            }
        }

        public UILogic()
        {
            StartProgram = new RelayCommand<object>(program =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Program Start\"");
                if (program != null)
                {
                    ProgramViewModel _program = program as ProgramViewModel ?? new ProgramViewModel(new Programinfo());
                    _commandHelper.StartProgram(_program);
                    this.SyncAndSave();
                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No program to start");
                }
            });

            StopProgram = new RelayCommand<object>(program =>
            {
                Debug.WriteLine("[Info] UILogic: Botton clicked \"Program Stop\"");
                if (program != null)
                {
                    ProgramViewModel _program = program as ProgramViewModel ?? new ProgramViewModel(new Programinfo());
                    _commandHelper.StopProgram(_program);
                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No program to stop");
                }
            });

            StartAsset = new RelayCommand<object>(asset =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Asset Start\"");
                if (asset != null)
                {
                    AssetViewModel? _asset = asset as AssetViewModel;
                    ObservableCollection<ProgramViewModel> _programlist = _asset!.programlist;
                    foreach (var program in _programlist)
                    {
                        _commandHelper.StartProgram(program);
                    }

                    if (this.AssetStartMode)
                    {
                        _commandHelper.StartProgram(_asset);
                    }
                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No asset to start");
                }
            });

            StopAsset = new RelayCommand<object>(asset =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Asset Stop\"");
                if (asset != null)
                {
                    AssetViewModel? _asset = asset as AssetViewModel;
                    ObservableCollection<ProgramViewModel> _programlist = _asset!.programlist;
                    foreach (var program in _programlist)
                    {
                        _commandHelper.StopProgram(program);
                    }

                    if (this.AssetStartMode)
                    {
                        _commandHelper.StopProgram(_asset);
                    }
                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No asset to stop");
                }
            });

            Browse = new RelayCommand<object>(async program =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Browse\"");

                var picker = new FileOpenPicker();

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(((App)Application.Current)._window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.FileTypeFilter.Add(".exe");

                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    Debug.WriteLine($"{file.Path}");
                    if (program != null)
                    {
                        ProgramViewModel? _program = null;
                        AssetViewModel? _asset = null;

                        if (program.GetType() == typeof(ProgramViewModel))
                        {
                            Debug.WriteLine("[Info] UILogic: Setting ExePath in ProgramViewModel");
                            _program = program as ProgramViewModel;
                        }
                        else if (program.GetType() == typeof(AssetViewModel))
                        {
                            Debug.WriteLine("[Info] UILogic: Setting ExePath in AssetViewModel's selected Program");
                            _asset = program as AssetViewModel;
                        }
                        else
                            Debug.WriteLine("[Error] UILogic: Unknown type in Browse command");

                        if (_program != null)
                        {
                            _program!.ExePath = file.Path;
                            this.SetDirtyFlag(true);
                        }
                        else if (_asset != null)
                        {
                            _asset!.ExePath = file.Path;
                            this.SetDirtyFlag(true);
                        }
                        else
                            Debug.WriteLine("[Error] UILogic: ProgramViewModel is null");
                    }
                }
            });

            Delete = new RelayCommand<object>(program =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Delete\"");
                if (program != null)
                {
                    ProgramViewModel? _program = program as ProgramViewModel;
                    _assetService.RemoveProgramFromObservableAsset(EditableSelectedAsset!.programlist, _program!);
                    this.SetDirtyFlag(true);
                }
            });

            DeleteMainGamePath = new RelayCommand<object>(asset =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"DeleteMainGamePath\"");
                if (asset != null)
                {
                    AssetViewModel? _asset = asset as AssetViewModel;
                    _asset!.ExePath = string.Empty;
                    this.SetDirtyFlag(true);
                }
            });

            AddProgram = new RelayCommand<object>(p =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"AddProgram\"");
                ProgramViewModel _program = new ProgramViewModel(new Programinfo());
                EditableSelectedAsset!.programlist.Add(_program);
                this.SetDirtyFlag(true);
            });

            Save = new RelayCommand<object>(p =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Save\"");
                this.SyncAndSave();
                this.SetDirtyFlag(false);
            });

            AddAsset = new RelayCommand<object>(p =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"AddAsset\"");
                AssetViewModel _asset = new AssetViewModel(new Asset());
                _asset.Name = "New Asset";
                _asset.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);
                EditableAssetlist.Add(_asset);
                SelectedAsset = EditableAssetlist[EditableAssetlist.Count - 1];
                this.SyncAndSave();
            });

            DeleteAsset = new RelayCommand<object>(p =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"DeleteAsset\"");
                int _index = GetIndexOfAssetFromAssetlist(SelectedAsset!, EditableAssetlist, Assetlist);
                AssetViewModel _AssetToDelete = EditableAssetlist[_index];
                EditableAssetlist.Remove(_AssetToDelete);
                SelectedAsset = EditableAssetlist[_index - 1];
                this.SyncAndSave();
            });
        }

        public void StartUILogic()
        {
            Debug.WriteLine("[Info] UILogic: Starting UILogic");
            this.GetAssets();
        }

        private void GetAssets()
        {
            Debug.WriteLine("[Info] UILogic: Getting Assets");
            Assetlist = CollectionHelper.ToObservableCollection(_assetService.Assets);
            EditableAssetlist.Clear();

            if (Assetlist.Count > 0)
            {
                foreach (Asset asset in Assetlist)
                {
                    AssetViewModel _assetViewModel = new AssetViewModel(asset);
                    _assetViewModel.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);
                    if (_assetViewModel.IsSetExePath())
                    {
                        this.SetModeSelector(true);
                    }
                    else
                    {
                        this.SetModeSelector(false);
                    }
                    EditableAssetlist.Add(_assetViewModel);
                }
                SelectedAsset = EditableAssetlist[0];
            }
            else
            {
                Debug.WriteLine("[Info] UILogic: No Assets saved in .json");
            }
        }

        /// <summary>
        /// Reading the ProgramList of the Asset and creating a new instance named Programlist
        /// </summary>
        private void UpdateProgramlists()
        {
            Debug.WriteLine("[Info] UILogic: Updating EditableProgramList");
            Programlist.Clear();
            foreach (var program in EditableSelectedAsset!.programlist)
            {
                //Debug.WriteLine($"{program.Name}");
                program.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);
                this.UpdateProgramStates(program);
                Programlist.Add(program);
            }
            EditableSelectedAsset.programlist = Programlist;
        }

        //Funktion zum erstmaligen initialisieren aller States
        private void UpdateProgramStates(ProgramViewModel program)
        {
            if (ProcessHandler.FindRunningProcess(program) != null)
            {
                program.SetCurrentState(Programinfo.Running);
                return;
            }
            else
            {
                program.SetCurrentState(Programinfo.NotRunning);
                return;
            }
        }

        private void OnPropertyChanged_ProgramOrAsset(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Name" ||
                args.PropertyName == "ExePath")
            {
                Debug.WriteLine($"[UILogic] UILogic: Property changed {args.PropertyName}");
                if (this.IsAssetViewModel(sender!))
                {
                    AssetViewModel? _asset = sender as AssetViewModel;
                    Debug.WriteLine($"[UILogic] UILogic: Sender is {sender!.GetType()}");
                    if (!String.IsNullOrEmpty(_asset!.ExePath))
                    {
                        Debug.WriteLine($"[UILogic] UILogic: Setting ModeSelector");
                        this.SetModeSelector(true);
                    }
                    else
                    {
                        this.SetModeSelector(false);
                    }
                }
                this.SetDirtyFlag(true);
            }
        }

        private void SetModeSelector(bool state)
        {
            if (this.ModeSelectorActive != state)
            {
                Debug.WriteLine($"[UILogic] UILogic: Set ModeSelectorActive from \"{ModeSelectorActive}\" to \"{state}\"");
                this.ModeSelectorActive = state;
            }
        }

        private void SetDirtyFlag(bool state)
        {
            if (this.DirtyFlag != state)
            {
                Debug.WriteLine($"[UILogic] UILogic: Set DirtyFlag from \"{DirtyFlag}\" to \"{state}\"");
                this.DirtyFlag = state;
            }
        }

        private void SyncAndSave()
        {
            Debug.WriteLine("[UILogic] UILogic: Sync and Save");
            this.EditAssetInAssetlist(EditableAssetlist, SelectedAsset, EditableSelectedAsset!);
            this.SaveAssets();
        }

        private void SaveAssets()
        {
            Debug.WriteLine("[UILogic] UILogic: Save Assetlist");
            _assetService.SaveObservableAssetList(CollectionHelper.ToAssetModelList(EditableAssetlist));
            this.SetDirtyFlag(false);
        }

        private void EditAssetInAssetlist(ObservableCollection<AssetViewModel> assetList, AssetViewModel? rootAsset, AssetViewModel editableAsset)
        {
            if (editableAsset != null && rootAsset != null)
            {
                var _targetAsset = GetAssetInAssetlist(assetList, rootAsset);
                if (_targetAsset == null)
                    return;

                //_targetAsset.CopyFrom(editableAsset);
                _targetAsset.CopyFrom(editableAsset);
            }
        }

        private AssetViewModel? GetAssetInAssetlist(ObservableCollection<AssetViewModel> assetList, AssetViewModel asset)
        {
            AssetViewModel? _targetAsset = assetList.FirstOrDefault(p => p.Name == asset.Name);
            return _targetAsset;
        }

        private int GetIndexOfAssetFromAssetlist(AssetViewModel asset, ObservableCollection<AssetViewModel> assetlist, ObservableCollection<Asset>? backupAssetlist = null)
        {
            int _index = -1;
            _index = assetlist.IndexOf(SelectedAsset!);
            if (_index == -1 && backupAssetlist != null)
            {
                _index = backupAssetlist.IndexOf(SelectedAsset!.ToModel());
            }

            return _index;
        }

        private bool IsAssetViewModel(object obj)
        {
            return obj is AssetViewModel;
        }
    }
}
