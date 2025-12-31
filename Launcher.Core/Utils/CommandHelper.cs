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

namespace Launcher.Core.Utils
{
    public class CommandHelper : ObservableObject
    {
        public void StartProgram(ProgramViewModel programinfo)
        {
            Debug.WriteLine("[Info] CommandHelper: Start Program");

            ProgramViewModel _program = (ProgramViewModel)programinfo;
            Process? _proc = ProcessHandler.StartProgram(_program);

            if (_proc != null)
            {
                _program.SetProcessName(_proc);
                _program.SetProcessInstance(_proc);
                _program.SetCurrentState(Programinfo.Running);
            }
        }

        // Not nice yet
        public void StopProgram(object programinfo)
        {
            ProgramViewModel program = (ProgramViewModel)programinfo;
            Debug.WriteLine($"[Info] CommandHelper: Try to stop Program \"{program.Name}\"");
            if(ProcessHandler.StopProgram(program) == false)
            {
                Debug.WriteLine($"[Info] CommandHelper: Could not stop program: \"{program.Name}\"");
            }
            else
            {
                Debug.WriteLine("[Info] CommandHelper: Program stopped");
            }
            program.SetCurrentState(Programinfo.NotRunning);
        }
    }
}
