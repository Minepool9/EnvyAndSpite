using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
using meshwhatever;

public static class ShaderManager
{
    public static Dictionary<string, Shader> shaderDictionary = new Dictionary<string, Shader>();

    public static void LoadShaders()
    {
		Addressables.InitializeAsync().WaitForCompletion();
		foreach (string addressEntry in ((ResourceLocationMap)Addressables.ResourceLocators.First()).Keys)
		{
			if (!addressEntry.EndsWith(".shader"))
				continue;

			Shader ingameShader = Addressables.LoadAssetAsync<Shader>(addressEntry).WaitForCompletion();
			if (ingameShader == null || ingameShader.name == "ULTRAKILL/PostProcessV2")
				continue;

			shaderDictionary[ingameShader.name] = ingameShader;
		}
	}

    public static void ApplyShaders(GameObject[] allGameObjects)
    {
        foreach (GameObject go in allGameObjects)
        {
            Renderer[] meshRenderers = go.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in meshRenderers)
            {
                if (renderer == null)
                    continue;

                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    Material sharedMat = renderer.sharedMaterials[i];
                    newMaterials[i] = sharedMat;

					if (sharedMat == null || sharedMat.shader == null)
                        continue;

                    // PostProcessV2 material should be the shared one
                    if (sharedMat.shader.name == "ULTRAKILL/PostProcessV2")
                        continue;

                    newMaterials[i] = renderer.materials[i];
					if (!shaderDictionary.TryGetValue(sharedMat.shader.name, out Shader realShader))
						continue;

					newMaterials[i].shader = realShader;
				}
                renderer.materials = newMaterials;
            }
        }
    }
}
