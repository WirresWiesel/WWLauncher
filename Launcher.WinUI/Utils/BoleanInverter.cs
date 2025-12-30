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
    public class BoleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // targetType & language werden ignoriert
            if (value is true)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
