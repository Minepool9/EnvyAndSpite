using System;
using UnityEngine;

namespace DoomahLevelLoader
{
	public class PostProcessingEffect : MonoBehaviour
	{
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, this.shaderMaterial);
		}

		public Material shaderMaterial;
	}
}
