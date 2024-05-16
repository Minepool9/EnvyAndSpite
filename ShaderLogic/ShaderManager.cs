using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Linq;
using meshwhatever;
using UnityEngine.SceneManagement;

public static class ShaderManager
{
    public static Dictionary<string, Shader> shaderDictionary = new Dictionary<string, Shader>();
    private static HashSet<Material> modifiedMaterials = new HashSet<Material>();

    public static IEnumerator LoadShadersAsync()
    {
        AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IResourceLocator locator = handle.Result;
            foreach (string addressEntry in ((ResourceLocationMap)locator).Keys)
            {
                if (!addressEntry.EndsWith(".shader"))
                    continue;

                AsyncOperationHandle<Shader> shaderHandle = Addressables.LoadAssetAsync<Shader>(addressEntry);
                yield return shaderHandle;

                if (shaderHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Shader ingameShader = shaderHandle.Result;
                    if (ingameShader != null && ingameShader.name != "ULTRAKILL/PostProcessV2")
                    {
                        shaderDictionary[ingameShader.name] = ingameShader;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Addressables initialization failed: " + handle.OperationException);
        }
    }

    public static IEnumerator ApplyShadersAsync(GameObject[] allGameObjects)
    {
        if (allGameObjects == null)
        {
            yield break;
        }

        foreach (GameObject go in allGameObjects)
        {
            if (go == null)
                continue;

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

                    if (sharedMat == null || sharedMat.shader == null || modifiedMaterials.Contains(sharedMat))
                        continue;

                    if (sharedMat.shader.name == "ULTRAKILL/PostProcessV2")
                        continue;

                    if (!shaderDictionary.TryGetValue(sharedMat.shader.name, out Shader realShader))
                    {
                        continue;
                    }

                    newMaterials[i].shader = realShader;
                    modifiedMaterials.Add(sharedMat);
                }
                renderer.materials = newMaterials;
            }
            yield return null;
        }
    }

    public static IEnumerator ApplyShadersAsyncContinuously()
    {
        GameObject shaderManagerObject = new GameObject("ShaderManagerObject");
        ShaderManagerRunner shaderManagerRunner = shaderManagerObject.AddComponent<ShaderManagerRunner>();

        shaderManagerRunner.StartApplyingShaders();
        yield return null;
    }
}

public class ShaderManagerRunner : MonoBehaviour
{
    public void StartApplyingShaders()
    {
        StartCoroutine(ApplyShadersContinuously());
    }

    private IEnumerator ApplyShadersContinuously()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);

            yield return ShaderManager.ApplyShadersAsync(SceneManager.GetActiveScene().GetRootGameObjects());
        }
    }
}
