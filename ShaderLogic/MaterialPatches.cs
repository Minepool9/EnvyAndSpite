using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace meshwhatever
{
	[HarmonyPatch(typeof(Material))]
	static class MaterialPatches
	{
		public static void Process(Material material)
		{
			if (material.shader == null)
				return;

			if (!ShaderManager.shaderDictionary.TryGetValue(material.shader.name, out Shader realShader))
				return;

			if (material.shader == realShader)
				return;

			material.shader = realShader;
		}

		[HarmonyPatch(MethodType.Constructor, new Type[] {typeof(Shader)})]
		[HarmonyPostfix]
		public static void CtorPatch1(Material __instance)
		{
			Process(__instance);
		}

		[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(Material) })]
		[HarmonyPostfix]
		public static void CtorPatch2(Material __instance)
		{
			Process(__instance);
		}

		[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(string) })]
		[HarmonyPostfix]
		public static void CtorPatch3(Material __instance)
		{
			Process(__instance);
		}
	}
}
