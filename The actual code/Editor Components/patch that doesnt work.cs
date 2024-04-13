﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoomahLevelLoader
{
	[HarmonyPatch(typeof(StockMapInfo), nameof(StockMapInfo.Awake))]
	public static class StockMapInfoPatch
	{
		[HarmonyPostfix]
		public static void Postfix()
		{
			if (!Plugin.IsCustomLevel)
			{
				return;
			}
			
            StatsManager sman = GameObject.FindObjectOfType<StatsManager>();
			if (sman != null)
				sman.levelNumber = -1;

			string currentPath = SceneManager.GetActiveScene().path;
			foreach (ExecuteOnSceneLoad obj in Resources.FindObjectsOfTypeAll<ExecuteOnSceneLoad>().Where(o => o.gameObject.scene.path == currentPath).OrderBy(exe => exe.relativeExecutionOrder))
			{
				try
				{
					obj.Execute();
				}
				catch (Exception e)
				{
					Debug.LogError($"Error while executing OnSceneLoad script for {obj.gameObject.name}: {e}");
				}
			}
		}
	}
}