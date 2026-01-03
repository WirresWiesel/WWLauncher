using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Launcher.Core.Models
{
    // define what is a program and which data it holds
    public class Programinfo
    {
        public static readonly string Running = "Running";
        public static readonly string NotRunning = "Not Running";
        public static readonly string Unknown = "Unknown";

        public string ProcessName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string ExePath { get; set; } = string.Empty;
        public bool IsLauncher { get; set; } = false;

        [JsonIgnore]
        public ProcessObject? ProcessObject { get; set; }

        [JsonIgnore]
        public string State { get; set; } = Unknown;

        [JsonIgnore]
        public Process? ProcessInstance { get; set; }

        public virtual Programinfo Clone()
        {
            Debug.WriteLine($"[Info] Program: \"{this.Name}\"; cloned");
            return new Programinfo
            {
                Name = this.Name,
                ExePath = this.ExePath,
                ProcessName = this.ProcessName
            };
        }

        public void CopyFrom(Programinfo proginfo)
        {
            //integrate this in the programinfo class as copyfrom-method
            this.ProcessInstance = proginfo.ProcessInstance;
            this.ProcessObject = proginfo.ProcessObject;
            this.ProcessName = proginfo.ProcessName;
            this.ExePath = proginfo.ExePath;
            this.Name = proginfo.Name;
            this.State = proginfo.State;
            this.IsLauncher = proginfo.IsLauncher;
        }
    }
}
