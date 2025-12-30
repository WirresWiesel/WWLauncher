using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Launcher.Core.Models;

namespace Launcher.Core.Utils
{
    public static class ProcessHandler
    {
        private static ProcessObject? _processObject = new();
        public static Process? StartProgram(Programinfo program)
        {
            Debug.WriteLine("[Info] ProcessHandler: Start Program");
            var startinfo = new ProcessStartInfo
            {
                FileName = program.EXEPath,
                WorkingDirectory = Path.GetDirectoryName(program.EXEPath),
                UseShellExecute = false
            };

            Process? _process = Process.Start(startinfo);
            if (_process != null)
            {
                _processObject ??= new ProcessObject();
                _processObject.Initialize();
                _processObject.AddProcess(_process);
                program.ProcessObject = _processObject;
            }
            return _process;
        }

        public static bool StopProgram(Programinfo program)
        {
            if (program.ProcessObject != null)
            {
                Debug.WriteLine($"[Info] ProcessHandler: Stopping program via ProcessObject \"{program.Name}\"");
                StopProcessObject(program);
                return true;
            }
            else if (program.ProcessInstance != null)
            {
                Debug.WriteLine($"[Info] ProcessHandler: Stopping program via ProcessInstance \"{program.Name}\"");
                StopProcess(program.ProcessInstance);
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

        public static bool StopProcessObject(Programinfo program)
        {
            try
            {
                program.ProcessObject!.Dispose();
                program.ProcessObject = null;
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
        public static List<Process>? FindRunningProcess(Programinfo program)
        {
            Debug.WriteLine($"[Info] ProcessHandler: Search for running Process by Name \"{program.ProcessName}\"");
            List<Process> processes = Process.GetProcesses().ToList();
            List<Process>? _processes = new();
            foreach (Process process in processes)
            {
                if (process.ProcessName == program.ProcessName)
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
