using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Launcher
{
    // define what is a program and which data it holds
    public class Programinfo : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string EXEPath { get; set; } = string.Empty;

        [JsonIgnore]
        public string State
        {
            get => _state ?? "Unknown";
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
        public string? _state = null;
        [JsonIgnore]
        public Process? ProcessInstance { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
