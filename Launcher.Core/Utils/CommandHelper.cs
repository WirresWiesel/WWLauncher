using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Launcher.Core.Models;
using Launcher.Core.Utils;
using Launcher.Core.ViewModels;
using Launcher.Core.Interfaces;

namespace Launcher.Core.Utils
{
    public class CommandHelper : ObservableObject
    {
        public void StartProgram(IStartable programinfo)
        {
            Debug.WriteLine("[Info] CommandHelper: Start Program");
            Process? _proc = null;

            _proc = ProcessHandler.StartProgram(programinfo);
        }

        public void StopProgram(IStartable programinfo)
        {
            if (ProcessHandler.StopProgram(programinfo) == false)
            {
                Debug.WriteLine($"[Info] CommandHelper: Could not stop program");
            }
            else
            {
                Debug.WriteLine("[Info] CommandHelper: Program stopped");
            }
            programinfo.SetCurrentState(Programinfo.NotRunning);
        }
    }
}
