using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BepInEx;

namespace DoomahLevelLoader
{
    public static class Loaderscene
    {
        private static List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();
        private static int currentAssetBundleIndex = 0;

        public static string LoadedSceneName { get; private set; }

		public static void Setup()
		{
			string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string directoryPath = Path.GetDirectoryName(executablePath);
			string unpackedLevelsPath = Path.Combine(directoryPath, "UnpackedLevels");

			if (!Directory.Exists(unpackedLevelsPath))
			{
				Directory.CreateDirectory(unpackedLevelsPath);
			}

			string[] doomahFiles = Directory.GetFiles(directoryPath, "*.doomah");
			foreach (string doomahFile in doomahFiles)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(doomahFile);
				string levelFolderPath = Path.Combine(unpackedLevelsPath, fileNameWithoutExtension);

				if (!Directory.Exists(levelFolderPath))
				{
					ZipFile.ExtractToDirectory(doomahFile, levelFolderPath);
				}

				LoadAssetBundle(levelFolderPath);
			}
		}

        private static void LoadAssetBundle(string folderPath)
        {
            string[] bundleFiles = Directory.GetFiles(folderPath, "*.bundle");

            foreach (string bundleFile in bundleFiles)
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleFile);
                if (assetBundle != null)
                {
                    loadedAssetBundles.Add(assetBundle);
                }
            }
        }

        public static void SelectAssetBundle(int index)
        {
            if (index >= 0 && index < loadedAssetBundles.Count)
            {
                currentAssetBundleIndex = index;
            }
        }

        public static void ExtractSceneName()
        {
            if (loadedAssetBundles.Count > 0)
            {
                string[] scenePaths = loadedAssetBundles[currentAssetBundleIndex].GetAllScenePaths();
                if (scenePaths.Length > 0)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
                    LoadedSceneName = sceneName;
                }
            }
        }
		
		public static void Loadscene()
		{
			if (!string.IsNullOrEmpty(LoadedSceneName))
			{
				SceneManager.LoadSceneAsync(LoadedSceneName).completed += OnSceneLoadComplete;
				SceneHelper.ShowLoadingBlocker();
			}
		}

		private static void OnSceneLoadComplete(AsyncOperation asyncOperation)
		{
			SceneHelper.DismissBlockers();
		}

        public static void MoveToNextAssetBundle()
        {
            currentAssetBundleIndex = (currentAssetBundleIndex + 1) % loadedAssetBundles.Count;
        }

        public static void MoveToPreviousAssetBundle()
        {
            currentAssetBundleIndex = (currentAssetBundleIndex - 1 + loadedAssetBundles.Count) % loadedAssetBundles.Count;
        }
    }
}
