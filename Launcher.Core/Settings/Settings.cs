using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Settings
{
    public class Settings
    {
        public bool IsDarkMode { get; set; }
        public int StatusUpdateInterval { get; set; }

        public Settings Clone()
        {
            return new Settings
            {
                IsDarkMode = this.IsDarkMode,
                StatusUpdateInterval = this.StatusUpdateInterval
            };
        }
    }
}
