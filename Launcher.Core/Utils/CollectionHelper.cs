using Launcher.Core.ViewModels;
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

        public static List<T> ToList<T>(ObservableCollection<T> observableCollection)
        {
            return observableCollection.ToList();
        }

        public static ObservableCollection<AssetViewModel> ToAssetViewModelList (ObservableCollection<Models.Asset> assets)
        {
            ObservableCollection<AssetViewModel> assetViewModels = new();
            foreach (var asset in assets)
            {
                assetViewModels.Add(new AssetViewModel(asset));
            }
            return assetViewModels;
        }

        public static ObservableCollection<Models.Asset> ToAssetModelList (ObservableCollection<AssetViewModel> assetViewModels)
        {
            ObservableCollection<Models.Asset> assets = new();
            foreach (var assetViewModel in assetViewModels)
            {
                assets.Add(assetViewModel.ToModel());
            }
            return assets;
        }
    }
}
