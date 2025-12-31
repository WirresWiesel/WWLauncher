using Launcher.Core.Models;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Launcher.Core.ViewModels;

namespace Launcher.Core.Services
{
    // Defines methods for Asset handling
    public class AssetService
    {
        private static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WWLauncher", "Assetlist");
        private static string Filepath = Path.Combine(BasePath, "Assetlist.json");
        public List<Models.Asset> Assets = new List<Models.Asset>();

        public AssetService()
        {
            Debug.WriteLine("[Info] AssetService: Starting AssetService");
            this.GetAssetList();
        }


        // Reads the asset list from a JSON file and returns it as a list of Asset objects
        private List<Models.Asset> GetAssetList()
        {
            if (!File.Exists(Filepath))
            {
                Debug.WriteLine("[Error] AssetService: No Assetfile found");
                this.CreateDefaultAsset();
            }
            else
            {
                Debug.WriteLine("[Info] AssetService: Loading Assets from " + Filepath);
                string fileString = File.ReadAllText(Filepath);
                Assets = JsonSerializer.Deserialize<List<Models.Asset>>(fileString) ?? Assets;
            }
            return Assets;
        }

        private void CreateDefaultAsset()
        {
            Debug.WriteLine($"[Info] AssetService: Writing new Assetlist to {Filepath}");
            Directory.CreateDirectory(BasePath);
            File.WriteAllText(Filepath, "[]");
        }

        public Asset CreateAsset(string assetName)
        {
            Asset asset = new Asset();
            asset.Name = assetName;
            Assets.Add(asset);
            this.SaveInternalAssetList();
            return asset;
        }

        public void EditAsset(Asset asset, string? assetName, string? exePath)
        {
            if (asset != null)
            {
                if (assetName != null)
                {
                    asset.Name = assetName;
                }

                if (exePath != null)
                {
                    asset.ExePath = exePath;
                }
            }
            this.SaveInternalAssetList();
        }

        public void DeleteAsset(Asset? asset)
        {
            if (asset != null)
            {
                Assets.Remove(asset);
                this.SaveInternalAssetList();
            }
        }

        public void SaveInternalAssetList()
        {
            JsonSerializerOptions JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Change the edited asset in the asset list and save it back to the JSON file
            string fileString = JsonSerializer.Serialize(Assets, JsonOptions);
            File.WriteAllText(Filepath, fileString);
        }

        public void SaveObservableAssetList(ObservableCollection<Asset> assets)
        {
            Debug.WriteLine("[Info] AssetService: Save the ObservableAssetlist somehow");

            JsonSerializerOptions JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string fileString = JsonSerializer.Serialize(assets, JsonOptions);
            File.WriteAllText(Filepath, fileString);
            Debug.WriteLine("[Info] AssetService: File saved");
        }

        public static void AddProgramToAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
                asset.ProgramList.Add(proginfo);
            else
                Debug.WriteLine("[Error] AssetService: Asset is null, cannot add program");
        }

        public static void RemoveProgramFromAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
                asset.ProgramList.Remove(proginfo);
            else
                Debug.WriteLine("[Error] AssetService: Asset is null, can not remove program");
        }

        public void RemoveProgramFromObservableAsset(ObservableCollection<ProgramViewModel> programlist, ProgramViewModel program)
        {
            Debug.WriteLine($"[Info] AssetService: Remove {program.Name} from ObservableProgramList somehow");
            if (programlist != null)
                programlist.Remove(program);
            else
                Debug.WriteLine("[Error] AssetService: Asset is null, can not remove program");
        }

        public static void AddMainGameToAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
            {
                asset.ExePath = proginfo.ExePath;
            }
            else
                Debug.WriteLine("[Error] AssetService: Asset is null, cannot add program");
        }

        public bool IsSetMainGamePath(Asset asset)
        {
            if (!string.IsNullOrEmpty(asset.ExePath))
                return true;
            else
                return false;
        }
    }
}
