
using System.Collections.ObjectModel;

namespace Launcher.Core.Models
{
    // Defines what is an Asset and which data it holds
    public class Asset : Programinfo
    {
        public override Asset Clone()
        {
            var clone = new Asset
            {
                Name = this.Name,
                EXEPath = this.EXEPath,
                ProcessName = this.ProcessName
            };

            foreach (var program in ProgramList)
                clone.ProgramList.Add(program.Clone());

            return clone;
        }

        public void CopyFrom(Asset proginfo)
        {
            //integrate this in the programinfo class as copyfrom-method
            this.ProcessInstance = proginfo.ProcessInstance;
            this.ProcessObject = proginfo.ProcessObject;
            this.ProcessName = proginfo.ProcessName;
            this.EXEPath = proginfo.EXEPath;
            this.Name = proginfo.Name;
            this.State = proginfo.State;
            this.ProgramList = new ObservableCollection<Programinfo>(proginfo.ProgramList.Select(p => p.Clone()));
        }

        public ObservableCollection<Programinfo> ProgramList { get; set; } = new();
    }
}
