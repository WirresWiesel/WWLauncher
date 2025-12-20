using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Launcher.Utils
{
    public static class StatusVisualizer
    {
        public static SolidColorBrush UpdateProgramStatus(string state)
        {
            SolidColorBrush _brushColor = Brushes.Gray;
            switch (state)
            {
                case "Running":
                    _brushColor = Brushes.Green;
                    break;
                case "not Running":
                    _brushColor = Brushes.Red;
                    break;
                default:
                    _brushColor = Brushes.Gray;
                    break;
            }
            return _brushColor;
        }

        public static SolidColorBrush VisualizeBoolean(bool state)
        {
            if (state)
                return Brushes.Green;
            else
                return Brushes.Red;
        }
    }
}
