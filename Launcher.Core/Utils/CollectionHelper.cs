using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.Utils
{
    public static class CollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(List<T> list)
        {
            return new ObservableCollection<T>(list);
        }
    }
}
