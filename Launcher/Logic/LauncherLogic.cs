using Launcher.Models;
using Launcher.Services;
using System.Diagnostics;
using Launcher.Utils;
using System.IO;

namespace Launcher.Logic
{
    // Defines logic functions for the Launcher
    class LauncherLogic
    {
        private Asset? _currentAsset;
        private UpdateTimer _statusTimer;
        private ThemeHandler _themeHandler;

        public LauncherLogic()
        {
            _themeHandler = new ThemeHandler();
            _statusTimer = new UpdateTimer(TimeSpan.FromSeconds(30), UpdateProgramStates);
            _statusTimer.StartTimer();
        }

        public void OnAssetSelected(Asset asset)
        {
            _currentAsset = asset;
        }

        public void AddProgramToCurrentAsset(Programinfo programinfo)
        {
            AssetService.AddProgramToAsset(_currentAsset!, programinfo);
        }

        public void AddMaingameToCurrentAsset(Programinfo programinfo)
        {
            AssetService.AddMainGameToAsset(_currentAsset!, programinfo);
        }

        public void UpdateLstProgram(System.Windows.Controls.ListBox lstProgram)
        {
            lstProgram.ItemsSource = _currentAsset?.ProgramList;
            
        }

        public void StartProgram(Programinfo programinfo)
        {
            try
            {
                var startinfo = new ProcessStartInfo
                {
                    FileName = programinfo.EXEPath,
                    WorkingDirectory = Path.GetDirectoryName(programinfo.EXEPath),
                    UseShellExecute = true
                };

                programinfo.ProcessInstance = Process.Start(startinfo);

                if (programinfo.ProcessInstance != null)
                    programinfo.ProcessName = programinfo.ProcessInstance.ProcessName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start program: " + ex.Message);
            }
            UpdateProgramStates();
        }

        public void StopProgram(Programinfo programinfo)
        {
            try
            {
                if (programinfo.ProcessInstance != null && IsRunning(programinfo.ProcessInstance))
                {
                    programinfo.ProcessInstance.Kill();
                    programinfo.State = "Not Running";
                    Debug.WriteLine($"Program {programinfo.Name} has been stopped.");
                }
                else
                {
                    Debug.WriteLine($"Program {programinfo.Name} is not running, cannot stop.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop program: " + ex.Message);
            }
        }

        public void UpdateProgramStates()
        {
            if (_currentAsset != null)
            {
                foreach (var program in _currentAsset.ProgramList)
                {
                    program.ProcessInstance = FindProcess(program.ProcessName);
                    if (program.ProcessInstance != null && IsRunning(program.ProcessInstance))
                    {
                        program.State = "Running";
                        Debug.WriteLine($"Program {program.Name} is running.");
                    }
                    else
                    {
                        program.State = "Not Running";
                        Debug.WriteLine($"Program {program.Name} is not running.");
                    }

                    Debug.WriteLine($"Checking state for program: {program.Name}");
                    Debug.WriteLine($"Executable Path: {program.EXEPath}");
                    Debug.WriteLine($"Programinstance: {program.ProcessInstance}");
                }
            }
            else
            {
                Debug.WriteLine("No asset selected, cannot update program states");
            }
            Debug.WriteLine($"Launcher Logic Timestamp: {DateTime.Now}");
        }

        private Process? FindProcess(string processName)
        {
            List<Process> processes = Process.GetProcesses().ToList();
            foreach (Process process in processes)
            {
                if (process.ProcessName == processName)
                {
                    Debug.WriteLine($"Running process found {process.ProcessName}");
                    return process;
                }
            }
            return null;
        }

        private bool IsRunning(Process processInstance)
        {
            try
            {
                if (processInstance.HasExited)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while checking process state: " + ex.Message);
                return false;
            }
        }

        public void SetTheme(Settings.Settings settings)
        {
            if (settings.IsDarkMode)
                _themeHandler.SetDarkMode();
            else
                _themeHandler.SetLightMode();
        }

        public void SetStatusUpdateTimerInterval(int intervalInSeconds)
        {
            _statusTimer.SetInterval(TimeSpan.FromSeconds(intervalInSeconds));
        }
    }
}
