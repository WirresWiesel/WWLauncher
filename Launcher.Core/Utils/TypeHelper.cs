using Launcher.Core.Interfaces;
using Launcher.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.Utils
{
    public static class TypeHelper
    {
        /// <summary>
        /// Determines whether the specified object is of type AssetViewModel.
        /// </summary>
        public static bool IsAssetViewModel(object obj)
        {
            return obj is AssetViewModel;
        }

        /// <summary>
        /// Determines whether the specified object is of type ProgramViewModel.
        /// </summary>
        public static bool IsProgramViewModel(object obj)
        {
            return obj is ProgramViewModel;
        }

        /// <summary>
        /// Determines whether the specified object is of type IStartable.
        /// </summary>
        public static bool IsIStartable(object obj)
        {
            return obj is IStartable;
        }
    }
}
