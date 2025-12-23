using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Launcher
{
    // define what is a program and which data it holds
    public class Programinfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonIgnore]
        public string? _state = null;
        [JsonIgnore]
        public string? _name = null;
        [JsonIgnore]
        public string? _exePath = null;

        public string ProcessName { get; set; } = string.Empty;

        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }
        public string EXEPath
        {
            get => _exePath ?? string.Empty;
            set
            {
                if (_exePath != value)
                {
                    _exePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EXEPath)));
                }
            }
        }

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
        public Process? ProcessInstance { get; set; }


    }
}
