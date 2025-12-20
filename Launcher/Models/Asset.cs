
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Launcher.Models
{
    // Defines what is an Asset and which data it holds
    class Asset : Programinfo
    {
        public ObservableCollection<Programinfo> ProgramList { get; set; } = new();
    }
}
