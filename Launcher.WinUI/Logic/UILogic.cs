using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Core.Models;
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

namespace Launcher.WinUI.Logic
{
    public class UILogic : ObservableObject
    {
        private readonly AssetService _assetService = new();
        private CommandHelper _commandHelper = new();
        public ObservableCollection<Asset> Assetlist { get; private set; } = new();
        public ObservableCollection<Programinfo> Programlist{ get; private set; } = new();
        public Asset? EditableSelectedAsset { get; private set; } = null;
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

        private ObservableCollection<Asset> _editableAssetlist = new();
        public ObservableCollection<Asset> EditableAssetlist
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

        private Asset _selectedAsset = new();

        public Asset? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                if (SetProperty(ref _selectedAsset!, value))
                {
                    if (SelectedAsset != null)
                    {
                        Debug.WriteLine($"[Info] UILogic: Property changed \"{SelectedAsset.Name}\"");
                        EditableSelectedAsset = value?.Clone();
                        EditableAssetName = EditableSelectedAsset!.Name;
                        this.SetDirtyFlag(false);
                    }
                    this.UpdateProgramlists();
                }
            }
        }

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

        public bool _dirtyFlag;
        public bool DirtyFlag
        {
            get => _dirtyFlag;
            set => SetProperty(ref _dirtyFlag, value);
        }

        public UILogic()
        {
            StartProgram = new RelayCommand<object>(program =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Program Start\"");
                if (program != null)
                {
                    _commandHelper.StartProgram(program);
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
                    _commandHelper.StopProgram(program);
                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No program to stop");
                }
            });

            StartAsset = new RelayCommand<object>(programlist =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Asset Start\"");
                if (programlist != null)
                {
                    ObservableCollection<Programinfo> _programlist = programlist as ObservableCollection<Programinfo> ?? new();
                    foreach (var program in _programlist)
                    {
                        _commandHelper.StartProgram(program);
                    }

                }
                else
                {
                    Debug.WriteLine("[Error] UILogic: No asset to start");
                }
            });

            StopAsset = new RelayCommand<object>(programlist =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Asset Stop\"");
                if (programlist != null)
                {
                    ObservableCollection<Programinfo> _programlist = programlist as ObservableCollection<Programinfo> ?? new();
                    foreach (var program in _programlist)
                    {
                        _commandHelper.StopProgram(program);
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
                        Programinfo? _program = program as Programinfo;
                        _program!.EXEPath = file.Path;
                        this.SetDirtyFlag(true);
                    }
                }
            });

            Delete = new RelayCommand<object>(program =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"Delete\"");
                if (program != null)
                {
                    Programinfo? _program = program as Programinfo;
                    _assetService.RemoveProgramFromObservableAsset(EditableSelectedAsset!.ProgramList, _program!);
                    this.SetDirtyFlag(true);
                }
            });

            AddProgram = new RelayCommand<object>(p =>
            {
                Debug.WriteLine("[Info] UILogic: Button clicked \"AddProgram\"");
                Programinfo _program = new();
                EditableSelectedAsset!.ProgramList.Add(_program);
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
                Asset _asset = new();
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
                Asset _AssetToDelete = EditableAssetlist[_index];
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
                    asset.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);
                    EditableAssetlist.Add(asset);
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
            foreach (var program in EditableSelectedAsset!.ProgramList)
            {
                //Debug.WriteLine($"{program.Name}");
                program.PropertyChanged += (sender, e) => OnPropertyChanged_ProgramOrAsset(sender, e);

                this.UpdateProgramStates(program);
                Programlist.Add(program);
            }
            EditableSelectedAsset.ProgramList = Programlist;
        }

        //Funktion zum erstmaligen initialisieren aller States
        private void UpdateProgramStates(Programinfo program)
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
                args.PropertyName == "EXEPath")
            {
                Debug.WriteLine($"[UILogic] UILogic: Property changed {args.PropertyName}");
                this.SetDirtyFlag(true);
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
            this.EditAssetInAssetlist(EditableAssetlist, SelectedAsset, EditableSelectedAsset);
            this.SaveAssets();
        }

        private void SaveAssets()
        {
            Debug.WriteLine("[UILogic] UILogic: Save Assetlist");
            _assetService.SaveObservableAssetList(EditableAssetlist);
            this.SetDirtyFlag(false);
        }

        private void EditAssetInAssetlist(ObservableCollection<Asset> assetList, Asset? rootAsset, Asset? editableAsset)
        {
            if (editableAsset != null && rootAsset != null)
            {
                var _targetAsset = GetAssetInAssetlist(assetList, rootAsset);
                if (_targetAsset == null)
                    return;

                _targetAsset.CopyFrom(editableAsset);
            }
        }

        private Asset? GetAssetInAssetlist(ObservableCollection<Asset> assetList, Asset asset)
        {
            Asset? _targetAsset = assetList.FirstOrDefault(p => p.Name == asset.Name);
            return _targetAsset;
        }

        private int GetIndexOfAssetFromAssetlist(Asset asset, ObservableCollection<Asset> assetlist, ObservableCollection<Asset>? backupAssetlist = null)
        {
            int _index = -1;
            _index = assetlist.IndexOf(SelectedAsset!);
            if (_index == -1 && backupAssetlist != null)
            {
                _index = backupAssetlist.IndexOf(SelectedAsset!);
            }

            return _index;
        }
    }
}
