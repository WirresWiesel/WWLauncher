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

namespace Launcher.Core.Utils
{
    public class CommandHelper : ObservableObject
    {
        public void StartProgram(object programinfo)
        {
            Debug.WriteLine("[Info] CommandHelper: Start Program");

            Programinfo _program = (Programinfo)programinfo;
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
            Programinfo program = (Programinfo)programinfo;
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
