using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Interfaces;
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
    public partial class ProgramViewModel : ObservableObject, IStartable
    {
        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string exePath;

        [ObservableProperty]
        public string state;

        public Process? ProcessInstance { get; set; }
        public ProcessObject? ProcessObject { get; set; }
        public string ProcessName { get; set; }

        public ProgramViewModel(Programinfo programinfo)
        {
            this.ProcessInstance = programinfo.ProcessInstance;
            this.ProcessObject = programinfo.ProcessObject;
            this.ProcessName = programinfo.ProcessName;
            this.exePath = programinfo.ExePath;
            this.name = programinfo.Name;
            this.state = programinfo.State;
        }
        public ProgramViewModel(ProgramViewModel programViewModel)
        {
            this.ProcessInstance = programViewModel.ProcessInstance;
            this.ProcessObject = programViewModel.ProcessObject;
            this.ProcessName = programViewModel.ProcessName;
            this.ExePath = programViewModel.ExePath;
            this.Name = programViewModel.Name;
            this.state = programViewModel.state;
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

        public Programinfo ToModel()
        {
            return new Programinfo
            {
                Name = this.Name,
                ExePath = this.ExePath,
                ProcessName = this.ProcessName
            };
        }

        public void SetProcess(Process process)
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

        public void CopyFrom(ProgramViewModel programViewModel)
        {
            this.ProcessInstance = programViewModel.ProcessInstance;
            this.ProcessObject = programViewModel.ProcessObject;
            this.ProcessName = programViewModel.ProcessName;
            this.ExePath = programViewModel.ExePath;
            this.Name = programViewModel.Name;
            this.State = programViewModel.State;
        }
        public void CopyFrom(AssetViewModel assetViewModel)
        {
            this.ProcessInstance = assetViewModel.ProcessInstance;
            this.ProcessObject = assetViewModel.ProcessObject;
            this.ProcessName = assetViewModel.ProcessName;
            this.ExePath = assetViewModel.ExePath;
            this.Name = assetViewModel.Name;
            this.State = assetViewModel.State;
        }
    }
}
