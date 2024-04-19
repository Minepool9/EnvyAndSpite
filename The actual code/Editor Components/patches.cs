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
    [HarmonyPatch(typeof(SceneHelper), "RestartScene")]
    public static class SceneHelper_RestartScene_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
			if (Plugin.IsCustomLevel)
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(Loaderscene.LoadedSceneName);
				Plugin.FixVariables();
				return false;
			}
			return true;
		}
    }
}