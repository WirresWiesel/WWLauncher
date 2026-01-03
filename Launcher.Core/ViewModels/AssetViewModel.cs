using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Interfaces;
using Launcher.Core.Models;
using Launcher.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.ViewModels
{
    public partial class AssetViewModel : ObservableObject, IStartable
    {
        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string exePath;

        [ObservableProperty]
        public string state;

        [ObservableProperty]
        public bool isLauncher;

        public string ProcessName { get; set; }
        public Process? ProcessInstance { get; set; }
        public ProcessObject? ProcessObject { get; set; }

        public ObservableCollection<ProgramViewModel> programlist;

        public AssetViewModel(Asset asset)
        {
            this.ExePath = asset.ExePath;
            this.Name = asset.Name;
            this.State = asset.State;
            this.isLauncher = asset.IsLauncher;
            this.ProcessName = asset.ProcessName;
            this.programlist = new ObservableCollection<ProgramViewModel>(asset.ProgramList.Select(p => new ProgramViewModel(p)));
        }

        public AssetViewModel(AssetViewModel asset)
        {
            this.ExePath = asset.ExePath;
            this.Name = asset.Name;
            this.State = asset.State;
            this.isLauncher = asset.IsLauncher;
            this.ProcessName = asset.ProcessName;
            this.programlist = new ObservableCollection<ProgramViewModel>(asset.programlist.Select(p => new ProgramViewModel(p)));
        }

        public void SetProcessName(Process? process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Program: \"{this.Name}\"; setting ProcessName: \"{process.ProcessName}\"");
                this.ProcessName = process.ProcessName ?? string.Empty;
            }
        }

        public void SetProcessInstance(Process? process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Program: \"{this.Name}\"; setting ProcessInstance");
                this.ProcessInstance = process;
            }
        }

        public void SetCurrentState(string state)
        {
            Debug.WriteLine($"[Info] Program: \"{this.Name}\"; setting state to: \"{state}\"");
            this.State = state;
        }

        public Asset ToModel()
        {
            return new Asset
            {
                ProcessInstance = this.ProcessInstance,
                ProcessObject = this.ProcessObject,
                ProcessName = this.ProcessName,
                ExePath = this.ExePath,
                Name = this.Name,
                State = this.State,
                IsLauncher = this.IsLauncher,
                ProgramList = new ObservableCollection<Programinfo>(this.programlist.Select(p => p.ToModel()))
            };
        }

        public void CopyFrom(AssetViewModel assetViewModel)
        {
            this.ExePath = assetViewModel.ExePath;
            this.Name = assetViewModel.Name;
            this.State = assetViewModel.State;
            this.ProcessName = assetViewModel.ProcessName;
            this.IsLauncher = assetViewModel.IsLauncher;
            this.programlist = new ObservableCollection<ProgramViewModel>(assetViewModel.programlist.Select(p => new ProgramViewModel(p)));
        }

        public bool IsSetExePath()
        {
            return !string.IsNullOrEmpty(this.ExePath);
        }

        public void SetProcessObject(Process process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Asset: \"{this.Name}\"; setting ProcessInstance");
                ProcessObject _processObject = new ProcessObject();
                _processObject.Initialize();
                _processObject.AddProcess(process);
                this.ProcessObject = _processObject;
                this.SetProcessName(process);
                this.SetProcessInstance(process);
                this.SetCurrentState(Programinfo.Running);
            }
        }

        public void SetProcess(Process process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Asset: \"{this.Name}\"; setting ProcessInstance");
                this.SetProcessName(process);
                this.SetProcessInstance(process);
                this.SetCurrentState(Programinfo.Running);
            }
        }
    }
}
