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
    public partial class ProgramViewModel : ObservableObject
    {
        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string exePath;

        [ObservableProperty]
        public string state;

        public Process? processInstance;
        public ProcessObject? processObject;
        public string processName;

        public ProgramViewModel(Programinfo programinfo)
        {
            this.processInstance = programinfo.ProcessInstance;
            this.processObject = programinfo.ProcessObject;
            this.processName = programinfo.ProcessName;
            this.exePath = programinfo.ExePath;
            this.name = programinfo.Name;
            this.state = programinfo.State;
        }
        public ProgramViewModel(ProgramViewModel programViewModel)
        {
            this.processInstance = programViewModel.processInstance;
            this.processObject = programViewModel.processObject;
            this.processName = programViewModel.processName;
            this.ExePath = programViewModel.ExePath;
            this.Name = programViewModel.Name;
            this.state = programViewModel.state;
        }
        public void SetProcessName(Process? process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Program: \"{this.Name}\"; setting ProcessName: \"{process.ProcessName}\"");
                this.processName = process.ProcessName ?? string.Empty;
            }
        }

        public void SetProcessInstance(Process? process)
        {
            if (process != null)
            {
                Debug.WriteLine($"[Info] Program: \"{this.Name}\"; setting ProcessInstance");
                this.processInstance = process;
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
                ProcessName = this.processName
            };
        }

        public ProgramViewModel CopyFrom(ProgramViewModel programViewModel)
        {
            return new ProgramViewModel(programViewModel);
        }
    }
}
