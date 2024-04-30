using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using UnityEngine.UI;
using TMPro;

namespace DoomahLevelLoader
{
    public static class Loaderscene
    {
        public static List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();
        public static int currentAssetBundleIndex = 0;
		public static List<string> bundleFolderPaths = new List<string>();

        public static string LoadedSceneName { get; private set; }

		public static async Task Setup()
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

				try
				{
					if (!Directory.Exists(levelFolderPath))
					{
						await Task.Run(() => ZipFile.ExtractToDirectory(doomahFile, levelFolderPath));
					}

					_ = LoadAssetBundle(levelFolderPath);
				}
				catch
				{
					string fileName = Path.GetFileName(doomahFile);
					UnityEngine.Debug.LogError($"Failed to extract {fileName}! Please Uninstall map or ask creator to update to 1.3.0!");
				}
			}
		}
		
        public static async Task DeleteUnpackedLevelsFolder()
        {
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(executablePath);
            string unpackedLevelsPath = Path.Combine(directoryPath, "UnpackedLevels");

            if (Directory.Exists(unpackedLevelsPath))
            {
                try
                {
                    await Task.Run(() => Directory.Delete(unpackedLevelsPath, true));
                    UnityEngine.Debug.Log("UnpackedLevels folder deleted successfully.");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Failed to delete UnpackedLevels folder: {ex.Message}");
                }
            }
            else
            {
                UnityEngine.Debug.Log("UnpackedLevels folder does not exist.");
            }
        }
		
		// dear fucking god what is this logic
		public static async Task Refresh()
		{
			GameObject parentOfFuckingPleaseWait = null;

			EnvyandSpiteterimal envyScript = GameObject.FindObjectOfType<EnvyandSpiteterimal>();
			if (envyScript != null && envyScript.FuckingPleaseWait != null)
			{
				parentOfFuckingPleaseWait = envyScript.FuckingPleaseWait.transform.parent.gameObject;
				envyScript.FuckingPleaseWait.gameObject.SetActive(true);
			}
			else
			{
				EnvyLoaderMenu envyLoaderMenuScript = GameObject.FindObjectOfType<EnvyLoaderMenu>();
				if (envyLoaderMenuScript != null && envyLoaderMenuScript.FuckingPleaseWait != null)
				{
					parentOfFuckingPleaseWait = envyLoaderMenuScript.FuckingPleaseWait.transform.parent.gameObject;
					envyLoaderMenuScript.FuckingPleaseWait.gameObject.SetActive(true);
				}
			}

			foreach (var bundle in loadedAssetBundles)
			{
				bundle.Unload(true);
			}
			loadedAssetBundles.Clear();
			bundleFolderPaths.Clear();

			await DeleteUnpackedLevelsFolder();

			await Setup();

			if (parentOfFuckingPleaseWait != null)
			{
				parentOfFuckingPleaseWait.SetActive(false);
			}
		}


		public static async Task LoadAssetBundle(string folderPath)
		{
			string[] bundleFiles = Directory.GetFiles(folderPath, "*.bundle");

			foreach (string bundleFile in bundleFiles)
			{
				await Task.Run(() =>
				{
					AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleFile);
					if (assetBundle != null)
					{
						loadedAssetBundles.Add(assetBundle);
						bundleFolderPaths.Add(Path.GetDirectoryName(bundleFile));
					}
				});
			}
		}

		public static string GetCurrentBundleFolderPath()
		{
			if (currentAssetBundleIndex >= 0 && currentAssetBundleIndex < bundleFolderPaths.Count)
			{
				return bundleFolderPaths[currentAssetBundleIndex];
			}
			return null;
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
		
		public static void OpenFilesFolder()
		{
			string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string directoryPath = Path.GetDirectoryName(executablePath);

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					Process.Start("explorer.exe", directoryPath.Replace("/", "\\"));
					break;
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					Process.Start("open", directoryPath);
					break;
				case RuntimePlatform.LinuxEditor:
				case RuntimePlatform.LinuxPlayer:
					Process.Start("xdg-open", directoryPath);
					break;
				default:
					UnityEngine.Debug.LogWarning("BROTHER WHAT IS YOUR OS?????");
					break;
			}
		}
		
		public static void UpdateLevelPicture(Image levelPicture, TextMeshProUGUI frownyFace, bool getFirstBundle = true, string bundlePath = "")
		{
			string bundleFolderPath = "";

			if (getFirstBundle)
			{
				bundleFolderPath = GetCurrentBundleFolderPath();
			}
			else
			{
				if (!string.IsNullOrEmpty(bundlePath))
				{
					bundleFolderPath = bundlePath;
				}
				else
				{
					return;
				}
			}

			if (!string.IsNullOrEmpty(bundleFolderPath))
			{
				string[] imageFiles = Directory.GetFiles(bundleFolderPath, "*.png");
				if (imageFiles.Length > 0)
				{
					string imagePath = imageFiles[0];
					Texture2D tex = LoadTextureFromFile(imagePath);
					if (tex != null)
					{
						tex.filterMode = FilterMode.Point;
						levelPicture.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
						frownyFace.gameObject.SetActive(false);
						levelPicture.color = Color.white;
					}
				}
				else
				{
					frownyFace.gameObject.SetActive(true);
					levelPicture.color = new Color(1f, 1f, 1f, 0f);
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Bundle folder path is null or empty.");
			}
		}

		private static Texture2D LoadTextureFromFile(string path)
		{
			byte[] fileData = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(fileData);
			return texture;
		}	
    }
}
