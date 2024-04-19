using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Logic;

namespace DoomahLevelLoader
{
    public static class Loaderscene
    {
        public static AssetBundle[] loadedBundles;
        private static int currentBundleIndex = 0;
        public static string scenePath;

        public static void LoadAssetBundles()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] bundlePaths = Directory.GetFiles(directory, "*.doomah");

            loadedBundles = new AssetBundle[bundlePaths.Length];

            for (int i = 0; i < bundlePaths.Length; i++)
            {
                loadedBundles[i] = AssetBundle.LoadFromFile(bundlePaths[i]);
            }
        }

		public static string ExtractScene()
		{
			if (loadedBundles.Length == 0)
				return "";

			currentBundleIndex = Mathf.Clamp(currentBundleIndex, 0, loadedBundles.Length - 1);

			while (true)
			{
				AssetBundle currentBundle = loadedBundles[currentBundleIndex];

				if (currentBundle.isStreamedSceneAssetBundle && currentBundle.GetAllScenePaths().Length != 0)
				{
					return currentBundle.GetAllScenePaths().First();
				}

				NextBundle();

				if (currentBundleIndex == 0)
				{
					Debug.LogError("No scenes found in any loaded bundles.");
					return "";
				}
			}
		}


        public static void LoadScene()
        {
            if (!string.IsNullOrEmpty(scenePath))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(scenePath);
            }
            else
            {
                Debug.LogError("Scene not found or loaded.");
            }
        }

        public static void NextBundle()
        {
            currentBundleIndex = (currentBundleIndex + 1) % loadedBundles.Length;
            scenePath = ExtractScene();
        }

        public static void PreviousBundle()
        {
            currentBundleIndex = (currentBundleIndex - 1 + loadedBundles.Length) % loadedBundles.Length;
            scenePath = ExtractScene();
        }

        public static void FixVariables()
        {
            MonoSingleton<MapVarManager>.Instance.ReloadMapVars();
        }

        [HarmonyPatch(typeof(SceneHelper), "RestartScene")]
        public static class SceneHelper_RestartScene_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Plugin.IsCustomLevel)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(scenePath);
                    Loaderscene.FixVariables();
                    return false;
                }
                return true;
            }
        }
    }
}