using Launcher.Core.Models;
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

        public static ObservableCollection<Models.Asset> ToAssetModelList (ObservableCollection<AssetViewModel> assetViewModels)
        {
            ObservableCollection<Models.Asset> assets = new();
            foreach (var assetViewModel in assetViewModels)
            {
                assets.Add(assetViewModel.ToModel());
            }
            return assets;
        }

        /// <summary>
        /// Returns the index of the specified asset in the given asset list.
        /// If not found and a backup asset list is provided, attempts to find the index there.
        /// Returns -1 if the asset cannot be found in either list.
        /// </summary>
        public static int GetIndexOfAssetFromAssetlist(AssetViewModel? asset, ObservableCollection<AssetViewModel> assetlist, ObservableCollection<Asset>? backupAssetlist = null)
        {
            int _index = -1;
            _index = assetlist.IndexOf(asset!);
            if (_index == -1 && backupAssetlist != null)
            {
                _index = backupAssetlist.IndexOf(asset!.ToModel());
            }

            return _index;
        }

        public static List<T> ToList<T>(ObservableCollection<T> observableCollection)
        {
            return observableCollection.ToList();
        }

        public static ObservableCollection<AssetViewModel> ToAssetViewModelList(ObservableCollection<Models.Asset> assets)
        {
            ObservableCollection<AssetViewModel> assetViewModels = new();
            foreach (var asset in assets)
            {
                assetViewModels.Add(new AssetViewModel(asset));
            }
            return assetViewModels;
        }

        /// <summary>
        /// Retrieves an asset from the given asset list that matches the name of the specified asset.
        /// Returns null if no matching asset is found.
        /// </summary>
        public static AssetViewModel? GetAssetInAssetlist(ObservableCollection<AssetViewModel> assetList, AssetViewModel asset)
        {
            AssetViewModel? _targetAsset = assetList.FirstOrDefault(p => p.Name == asset.Name);
            return _targetAsset;
        }

        /// <summary>
        /// Updates a target asset in the given asset list by copying the values from the editable asset.
        /// The target asset is identified based on the provided root asset.
        /// </summary>
        public static void EditAssetInAssetlist(ObservableCollection<AssetViewModel> assetList, AssetViewModel? rootAsset, AssetViewModel editableAsset)
        {
            if (editableAsset != null && rootAsset != null)
            {
                var _targetAsset = CollectionHelper.GetAssetInAssetlist(assetList, rootAsset);
                if (_targetAsset == null)
                    return;

                //_targetAsset.CopyFrom(editableAsset);
                _targetAsset.CopyFrom(editableAsset);
            }
        }
    }
}
