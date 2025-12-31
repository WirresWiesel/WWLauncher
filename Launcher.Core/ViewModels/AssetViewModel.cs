using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.ViewModels
{
    public partial class AssetViewModel : ObservableObject
    {
        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string exePath;

        [ObservableProperty]
        public string state;

        public string processName;
        public Process? processInstance;
        public ProcessObject? processObject;

        public ObservableCollection<ProgramViewModel> programlist;

        public AssetViewModel(Asset asset)
        {
            this.ExePath = asset.ExePath;
            this.Name = asset.Name;
            this.State = asset.State;
            this.processName = asset.ProcessName;
            this.programlist = new ObservableCollection<ProgramViewModel>(asset.ProgramList.Select(p => new ProgramViewModel(p)));
        }

        public AssetViewModel(AssetViewModel asset)
        {
            this.ExePath = asset.ExePath;
            this.Name = asset.Name;
            this.State = asset.State;
            this.processName = asset.processName;
            this.programlist = new ObservableCollection<ProgramViewModel>(asset.programlist.Select(p => new ProgramViewModel(p)));
        }

        public Asset ToModel()
        {
            return new Asset
            {
                //integrate this in the programinfo class as copyfrom-method
                ProcessInstance = this.processInstance,
                ProcessObject = this.processObject,
                ProcessName = this.processName,
                ExePath = this.ExePath,
                Name = this.Name,
                State = this.State
            };
        }

        public AssetViewModel CopyFrom(AssetViewModel assetViewModel)
        {
            return new AssetViewModel(assetViewModel);
        }
    }
}
