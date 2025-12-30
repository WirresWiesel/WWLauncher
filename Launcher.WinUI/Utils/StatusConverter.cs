using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;

namespace Launcher.WinUI.Utils
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // targetType & language werden ignoriert
            if (value is string state)
            {
                return state switch
                {
                    "Running" => new SolidColorBrush(Colors.LimeGreen),
                    "Not Running" => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
