using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher.Utils
{
    public class ThemeHandler
    {
        public void SetDarkMode()
        {
            var Theme = "DarkTheme.xaml";
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"/Launcher;component/Themes/{Theme}", UriKind.Relative) });
        }

        public void SetLightMode()
        {
            var Theme = "LightTheme.xaml";
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"/Launcher;component/Themes/{Theme}", UriKind.Relative) });
        }
    }
}
