using Launcher.Core.Interfaces;
using Launcher.Core.Models;
using Launcher.Core.ViewModels;
using System.ComponentModel;
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
        public static Process? StartProgram(IStartable program)
        {
            Debug.WriteLine("[Info] ProcessHandler: Start Program");
            var startinfo = new ProcessStartInfo
            {
                FileName = program.ExePath,
                WorkingDirectory = Path.GetDirectoryName(program.ExePath),
                UseShellExecute = false
            };

            Process? _process = Process.Start(startinfo);
            if (program.IsLauncher)
            {
                program.SetProcess(_process!);
            }
            else
            {
                program.SetProcessObject(_process!);
            }
            return _process;
        }

        public static bool StopProgram(IStartable program)
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
                    try
                    {
                        process!.Kill();
                    }
                    catch (Win32Exception ex)
                    {
                        Debug.WriteLine("[Error] ProcessHandler: Failure while killing process: " + ex.Message);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StopProcessObject(IStartable program)
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
        public static List<Process>? FindRunningProcess(IStartable program)
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
