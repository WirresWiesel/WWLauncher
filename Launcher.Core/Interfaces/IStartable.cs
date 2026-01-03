using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Launcher.Core.Models;

namespace Launcher.Core.Interfaces
{
    public interface IStartable
    {
        public string Name { get; set; }
        public string ExePath { get; set; }
        public bool IsLauncher { get; set; }
        public string ProcessName { get; set; }
        public Process? ProcessInstance { get; set; }
        public ProcessObject? ProcessObject { get; set; }
        public void SetProcess(Process process);
        public void SetProcessObject(Process process);
        public void SetCurrentState(string state);
    }
}
