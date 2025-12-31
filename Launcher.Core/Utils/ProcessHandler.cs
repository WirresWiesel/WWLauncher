using Launcher.Core.Models;
using Launcher.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.Utils
{
    public static class ProcessHandler
    {
        private static ProcessObject? _processObject = new();
        public static Process? StartProgram(ProgramViewModel program)
        {
            Debug.WriteLine("[Info] ProcessHandler: Start Program");
            var startinfo = new ProcessStartInfo
            {
                FileName = program.ExePath,
                WorkingDirectory = Path.GetDirectoryName(program.ExePath),
                UseShellExecute = false
            };

            Process? _process = Process.Start(startinfo);
            if (_process != null)
            {
                _processObject ??= new ProcessObject();
                _processObject.Initialize();
                _processObject.AddProcess(_process);
                program.processObject = _processObject;
            }
            return _process;
        }

        public static bool StopProgram(ProgramViewModel program)
        {
            if (program.processObject != null)
            {
                Debug.WriteLine($"[Info] ProcessHandler: Stopping program via ProcessObject \"{program.Name}\"");
                StopProcessObject(program);
                return true;
            }
            else if (program.processInstance != null)
            {
                Debug.WriteLine($"[Info] ProcessHandler: Stopping program via ProcessInstance \"{program.Name}\"");
                StopProcess(program.processInstance);
                return true;
            }
            else
            {
                Debug.WriteLine($"[Info] ProcessHandler: No ProcessObject, no ProcessInstance");
                if (program != null)
                {
                    List<Process> processes = FindRunningProcess(program)!;
                    if (processes != null)
                    {
                        foreach (Process process in processes)
                        {
                            StopProcess(process);
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
        }

        public static bool StopProcess(Process process)
        {
            Debug.WriteLine($"[Info] ProcessHandler: Stopping process \"{process.ProcessName}\"");
            if (process != null)
            {
                Debug.WriteLine($"[Info] ProcessHandler: Try Softclose \"{process.ProcessName}\"");
                process!.CloseMainWindow();

                if (!process.WaitForExit(500))
                {
                    Debug.WriteLine($"[Info] ProcessHandler: Kill \"{process.Id}\"");
                    process!.Kill(true);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StopProcessObject(ProgramViewModel program)
        {
            try
            {
                program.processObject!.Dispose();
                program.processObject = null;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Error] _processObject close failed: " + ex.Message);
                return false;
            }
        }

        private static bool IsProcessRunning(Process processInstance)
        {
            try
            {
                if (processInstance.HasExited)
                {
                    Debug.WriteLine($"[Info] ProcessHandler: Process is not running \"{processInstance.ProcessName}\"; ID \"{processInstance.Id}\"");
                    return false;
                }
                else
                {
                    Debug.WriteLine($"[Info] ProcessHandler: Process is running \"{processInstance.ProcessName}\"; ID \"{processInstance.Id}\"");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Error] ProcessHandler: Failure while checking process state: " + ex.Message);
                return false;
            }
        }

        // Have to return a list of Processes because of multi threaded programs
        // Process.GetProcesses <- executed by every program??? get List every change of asset???
        public static List<Process>? FindRunningProcess(ProgramViewModel program)
        {
            Debug.WriteLine($"[Info] ProcessHandler: Search for running Process by Name \"{program.processName}\"");
            List<Process> processes = Process.GetProcesses().ToList();
            List<Process>? _processes = new();
            foreach (Process process in processes)
            {
                if (process.ProcessName == program.processName)
                {
                    _processes.Add(process);
                    Debug.WriteLine($"[Info] ProcessHandler: Process found \"{process.ProcessName}\"; ID \"{process.Id}\"");
                }
            }

            if (_processes.Count <= 0)
            {
                Debug.WriteLine("[Error] ProcessHandler: No process found");
                return null;
            }
            return _processes;
        }
    }

}
