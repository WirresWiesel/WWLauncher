using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Launcher.Core.Models
{
    // define what is a program and which data it holds
    public class Programinfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly string Running = "Running";
        public static readonly string NotRunning = "Not Running";
        public static readonly string Unknown = "Unknown";

        [JsonIgnore]
        public ProcessObject? ProcessObject { get; set; }

        [JsonIgnore]
        public string? _state = null;
        [JsonIgnore]
        public string? _name = null;
        [JsonIgnore]
        public string? _exePath = null;

        public string ProcessName { get; set; } = string.Empty;

        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }
        public string EXEPath
        {
            get => _exePath ?? string.Empty;
            set
            {
                if (_exePath != value)
                {
                    _exePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EXEPath)));
                }
            }
        }

        [JsonIgnore]
        public string State
        {
            get => _state ?? Programinfo.Unknown;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
                }
            }
        }


        [JsonIgnore]
        public Process? ProcessInstance { get; set; }

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

        public virtual Programinfo Clone()
        {
            Debug.WriteLine($"[Info] Program: \"{this.Name}\"; cloned");
            return new Programinfo
            {
                Name = this.Name,
                EXEPath = this.EXEPath,
                ProcessName = this.ProcessName
            };
        }

        public void CopyFrom(Programinfo proginfo)
        {
            //integrate this in the programinfo class as copyfrom-method
            this.ProcessInstance = proginfo.ProcessInstance;
            this.ProcessObject = proginfo.ProcessObject;
            this.ProcessName = proginfo.ProcessName;
            this.EXEPath = proginfo.EXEPath;
            this.Name = proginfo.Name;
            this.State = proginfo.State;
        }
    }
}
