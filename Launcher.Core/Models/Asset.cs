
using System.Collections.ObjectModel;

namespace Launcher.Core.Models
{
    // Defines what is an Asset and which data it holds
    public class Asset : Programinfo
    {
        public ObservableCollection<Programinfo> ProgramList { get; set; } = new();
    }
}
