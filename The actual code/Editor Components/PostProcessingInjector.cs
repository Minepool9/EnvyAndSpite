using System;
using UnityEngine;

namespace DoomahLevelLoader
{
	public class PostProccessingInjector : MonoBehaviour
	{
		private void Update()
		{
			Camera[] foundCameraObjects = Object.FindObjectsOfType<Camera>();
			if (foundCameraObjects.Length != 0)
			{
				Component[] effects = base.GetComponents(typeof(PostProcessingEffect));
				for (int i = 0; i < effects.Length; i++)
				{
					foundCameraObjects[0].gameObject.AddComponent<PostProcessingEffect>().shaderMaterial = ((PostProcessingEffect)effects[i]).shaderMaterial;
					Object.Destroy(effects[i]);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public DepthTextureMode SETTHISVARIABLETOEVERYTHING;
	}
}
