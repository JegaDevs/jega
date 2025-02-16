using System.Diagnostics;
using UnityEditor;

namespace JegaCore
{
    public class StaticPaths
    {
        public const string CoreAssetsFolderName = "JegaCoreAssets";
        public const string CoreAssetsPath = "Assets/Resources/" + CoreAssetsFolderName + "/";

        /// <summary>
        /// Returns whether the CoreAssetsFolder exists in the project. In build runtime, always returns false.
        /// </summary>
        public static bool CoreAssetsFolderExists
        {
            get
            {
#if UNITY_EDITOR
                return AssetDatabase.IsValidFolder(CoreAssetsPath);
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Creates the Resources and the CoreAssets folders in the project, if they dont exist yet. Does nothing in
        /// build runtime.Build-safe call.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void CreateCoreAssetsFoldersIfMissing()
        {
#if UNITY_EDITOR
            if (AssetDatabase.IsValidFolder("Assets/Resources/") == false)
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (AssetDatabase.IsValidFolder(CoreAssetsPath) == false)
                AssetDatabase.CreateFolder("Assets/Resources", CoreAssetsFolderName);
#endif
        }

        /// <summary>
        /// Creates and saves the Asset received in the CoreAssets folder. Does nothing in build runtime. Build-safe call.
        /// </summary>
        /// <param name="asset">UnityEngine.Object to be saved to disk.</param>
        [Conditional("UNITY_EDITOR")]
        public static void CreateAssetInCoreFolderThenSave(UnityEngine.Object asset)
        {
#if UNITY_EDITOR
            CreateCoreAssetsFoldersIfMissing();

            string fullAssetPath = $"{CoreAssetsPath}{asset.name}.asset";
            AssetDatabase.CreateAsset(asset, fullAssetPath);
            AssetDatabase.SaveAssetIfDirty(asset);
#endif
        }

        /// <summary>
        /// Shortcut method to load an asset from the CoreAssets folder.
        /// </summary>
        /// <param name="assetName">Name of the asset, without the '.asset' at the end.</param>
        /// <typeparam name="T">Any UnityEngine.Object</typeparam>
        public static T LoadAssetFromCoreFolder<T>(string assetName) where T : UnityEngine.Object
        {
            return UnityEngine.Resources.Load<T>(CoreAssetsFolderName + "/" + assetName);
        }

        /// <summary>
        /// Method to load a ScriptableObject asset from the CoreAssets folder. If the asset is not found there, it is
        /// created and saved.
        /// </summary>
        /// <param name="assetName">Name of the asset, without the '.asset' at the end.</param>
        /// <typeparam name="T">Any ScriptableObject or child class object.</typeparam>
        public static T LoadScriptableOrCreateIfMissing<T>(string assetName) where T : UnityEngine.ScriptableObject
        {
            T asset = LoadAssetFromCoreFolder<T>(assetName);
            if (asset != null) return asset;

            asset = UnityEngine.ScriptableObject.CreateInstance<T>();
            asset.name = assetName;
            CreateAssetInCoreFolderThenSave(asset);

            return asset;
        }
    }
}
