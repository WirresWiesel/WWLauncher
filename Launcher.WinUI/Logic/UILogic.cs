using System;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Core.Utils;
using Launcher.Core.Models;
using Launcher.Core.Services;
using Launcher.Core.Interfaces;
using Launcher.Core.ViewModels;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Launcher.WinUI.Pages;
using Microsoft.UI.Input.DragDrop;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Media.AppBroadcasting;
using Windows.Media.ContentRestrictions;
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
                        IStartable? _program = null;
                        if (TypeHelper.IsIStartable(program))
                        {
                            _program = program as IStartable;
                        }

                        if (_program != null)
                        {
                            _program!.ExePath = file.Path;
                            this.SetDirtyFlag(true);
                        }
                        else
                            Debug.WriteLine("[Error] UILogic: Program is null");
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
                int _index = CollectionHelper.GetIndexOfAssetFromAssetlist(SelectedAsset, EditableAssetlist, Assetlist);  //GetIndexOfAssetFromAssetlist(SelectedAsset!, EditableAssetlist, Assetlist);
                AssetViewModel _AssetToDelete = EditableAssetlist[_index];
                EditableAssetlist.Remove(_AssetToDelete);
                SelectedAsset = EditableAssetlist[_index - 1];
                this.SyncAndSave();
            });
        }

        /// <summary>
        /// Initializes the UI logic by retrieving assets and preparing the UI state.
        /// </summary>
        public void StartUILogic()
        {
            Debug.WriteLine("[Info] UILogic: Starting UILogic");
            this.GetAssets();
        }

        /// <summary>
        /// Retrieves all assets from the asset service and populates the observable collections.
        /// Each asset is wrapped in an AssetViewModel, with PropertyChanged events subscribed.
        /// If any asset has a valid executable path, the mode selector is updated accordingly.
        /// The first asset in the list is set as the selected asset if assets exist.
        /// </summary>
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
        /// Updates the program list of the currently selected asset.
        /// All programs are reloaded, their PropertyChanged events are subscribed,
        /// their states are updated, and the list is assigned to the bound collection.
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

        /// <summary>
        /// Updates the current state of the given program based on whether its process is running.
        /// Sets the state to Running if the process is active, otherwise to NotRunning.
        /// </summary>
        private void UpdateProgramStates(IStartable program)
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

        /// <summary>
        /// Handles property changes for programs or assets, specifically the "Name" and "ExePath" properties.
        /// If the sender is an AssetViewModel, updates the mode selector based on whether ExePath is set.
        /// Marks the state as dirty whenever these properties change.
        /// </summary>
        private void OnPropertyChanged_ProgramOrAsset(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Name" ||
                args.PropertyName == "ExePath")
            {
                Debug.WriteLine($"[UILogic] UILogic: Property changed {args.PropertyName}");
                if (TypeHelper.IsAssetViewModel(sender!))
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
            else if (args.PropertyName == "IsLauncher")
            {
                Debug.WriteLine($"[UILogic] UILogic: Property changed {args.PropertyName}");
                this.SetDirtyFlag(true);
            }
        }

        /// <summary>
        /// Sets the ModeSelectorActive flag to the specified state if it differs from the current state.
        /// Logs the change for debugging purposes.
        /// </summary>
        private void SetModeSelector(bool state)
        {
            if (this.ModeSelectorActive != state)
            {
                Debug.WriteLine($"[UILogic] UILogic: Set ModeSelectorActive from \"{ModeSelectorActive}\" to \"{state}\"");
                this.ModeSelectorActive = state;
            }
        }

        /// <summary>
        /// Updates the DirtyFlag to the specified state if it has changed,
        /// and logs the change for debugging purposes.
        /// </summary>
        private void SetDirtyFlag(bool state)
        {
            if (this.DirtyFlag != state)
            {
                Debug.WriteLine($"[UILogic] UILogic: Set DirtyFlag from \"{DirtyFlag}\" to \"{state}\"");
                this.DirtyFlag = state;
            }
        }

        /// <summary>
        /// Synchronizes the currently editable asset with the asset list and saves all assets.
        /// </summary>
        private void SyncAndSave()
        {
            Debug.WriteLine("[UILogic] UILogic: Sync and Save");
            CollectionHelper.EditAssetInAssetlist(EditableAssetlist, SelectedAsset, EditableSelectedAsset!);
            this.SaveAssets();
        }

        /// <summary>
        /// Saves the current list of editable assets using the asset service
        /// and resets the DirtyFlag to indicate no unsaved changes.
        /// </summary>
        private void SaveAssets()
        {
            Debug.WriteLine("[UILogic] UILogic: Save Assetlist");
            _assetService.SaveObservableAssetList(CollectionHelper.ToAssetModelList(EditableAssetlist));
            this.SetDirtyFlag(false);
        }
    }
}
