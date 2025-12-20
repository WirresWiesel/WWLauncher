using Launcher.Models;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace Launcher.Services
{
    // Defines methods for Asset handling
    class AssetService
    {
        string Filepath = "Assets/Assetlist.json";
        public List<Models.Asset> Assets = new List<Models.Asset>();

    // Reads the asset list from a JSON file and returns it as a list of Asset objects
        public List<Models.Asset> GetAssetList()
        {
            if (!File.Exists(Filepath))
            {
                Debug.WriteLine("No Assetfile found");
            }
            else
            {
                Debug.WriteLine("Loading Assets from " + Filepath);
                string fileString = File.ReadAllText(Filepath);
                Assets = JsonSerializer.Deserialize<List<Models.Asset>>(fileString) ?? Assets;
            }
            return Assets;
        }

        public void CreateAsset(string assetName)
        {
            Asset asset = new Asset();
            asset.Name = assetName;
            Assets.Add(asset);
            this.SaveAssetList();
        }

        public void SaveAssetList()
        {
            JsonSerializerOptions JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Change the edited asset in the asset list and save it back to the JSON file
            string fileString = JsonSerializer.Serialize(Assets, JsonOptions);
            File.WriteAllText("Assets/Assetlist.json", fileString);
        }

        public static void AddProgramToAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
                asset.ProgramList.Add(proginfo);
            else
                Debug.WriteLine("Asset is null, cannot add program");
        }

        public static void RemoveProgramFromAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
                asset.ProgramList.Remove(proginfo);
            else
                Debug.WriteLine("Asset is null, cannot remove program");
        }
        public static void AddMaingameToAsset(Asset asset, Programinfo proginfo)
        {
            if (asset != null)
            {
                asset.Name = proginfo.Name;
                asset.EXEPath = proginfo.EXEPath;
            }
            else
                Debug.WriteLine("Asset is null, cannot add program");
        }

        public bool IsSetMaingamePath(Asset asset)
        {
            if (!string.IsNullOrEmpty(asset.EXEPath))
                return true;
            else
                return false;
        }
    }
}
