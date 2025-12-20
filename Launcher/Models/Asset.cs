
using System.Collections.ObjectModel;

namespace Launcher.Models
{
    // Defines what is an Asset and which data it holds
    class Asset : Programinfo
    {
        public ObservableCollection<Programinfo> ProgramList { get; set; } = new();
    }
}
