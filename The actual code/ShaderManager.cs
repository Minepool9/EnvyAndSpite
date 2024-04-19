using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using HarmonyLib;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;

public class ShaderManager
{
	internal static Dictionary<string, Shader> shaderNameMap = new Dictionary<string, Shader>();

	public static void Something()
	{
		Addressables.InitializeAsync().WaitForCompletion();
		foreach (string addressEntry in ((ResourceLocationMap)Addressables.ResourceLocators.First()).Keys)
		{
			  if (!addressEntry.EndsWith(".shader"))
				continue;

			  Shader ingameShader = Addressables.LoadAsset<Shader>(addressEntry).WaitForCompletion();
			  if (ingameShader == null)
				continue;

			  shaderNameMap[ingameShader.name] = ingameShader;
		}
		Debug.Log("Something?");
	}

	[HarmonyPatch(typeof(Material), MethodType.Constructor, new Type[] { typeof(Material) })]
	[HarmonyPostfix]
	private static void SwapShader(Material __instance)
	{
		if (__instance.shader == null)
			return;

		if (!shaderNameMap.TryGetValue(__instance.shader.name, out Shader ingameShader) || __instance.shader == ingameShader)
			return;

		__instance.shader = ingameShader;
	}
}
