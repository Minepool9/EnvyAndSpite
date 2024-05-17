using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.SceneManagement;

public static class ShaderManager
{
    public static Dictionary<string, Shader> shaderDictionary = new Dictionary<string, Shader>();
    private static HashSet<Material> modifiedMaterials = new HashSet<Material>();

    public static IEnumerator LoadShadersAsync()
    {
        AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
        while (!handle.IsDone)
        {
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IResourceLocator locator = handle.Result;
            foreach (string addressEntry in ((ResourceLocationMap)locator).Keys)
            {
                if (!addressEntry.EndsWith(".shader"))
                    continue;

                AsyncOperationHandle<Shader> shaderHandle = Addressables.LoadAssetAsync<Shader>(addressEntry);
                while (!shaderHandle.IsDone)
                {
                    yield return null;
                }

                if (shaderHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Shader ingameShader = shaderHandle.Result;
                    if (ingameShader != null && ingameShader.name != "ULTRAKILL/PostProcessV2")
                    {
                        if (!shaderDictionary.ContainsKey(ingameShader.name))
                        {
                            shaderDictionary[ingameShader.name] = ingameShader;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to load shader: " + shaderHandle.OperationException);
                }
            }
        }
        else
        {
            Debug.LogError("Addressables initialization failed: " + handle.OperationException);
        }
    }

    public static string ModPath()
    {
        return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
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

    public static void CreateShaderDictionary()
    {
        var shaderList = new List<ShaderInfo>();

        foreach (var shader in Resources.FindObjectsOfTypeAll<Shader>())
        {
            if (!shaderList.Any(s => s.Name == shader.name))
            {
                shaderList.Add(new ShaderInfo { Name = shader.name });
            }
        }

        string json = JsonConvert.SerializeObject(shaderList, Formatting.Indented);
        File.WriteAllText(Path.Combine(ModPath(), "ShaderList.json"), json);
    }

    public class ShaderInfo
    {
        public string Name { get; set; }
    }

    public static IEnumerator LoadShadersFromDictionaryAsync()
    {
        string shaderListPath = Path.Combine(ModPath(), "ShaderList.json");
        if (File.Exists(shaderListPath))
        {
            var shaderDataTask = ReadAllTextAsync(shaderListPath);
            while (!shaderDataTask.IsCompleted)
            {
                yield return null;
            }

            string json = shaderDataTask.Result;
            var shaderData = JsonConvert.DeserializeObject<List<ShaderInfo>>(json);
            var shaderLookup = new Dictionary<string, Shader>();
            foreach (var shaderInfo in shaderData)
            {
                Shader foundShader = Shader.Find(shaderInfo.Name);
                if (foundShader != null)
                {
                    if (!shaderLookup.ContainsKey(shaderInfo.Name))
                    {
                        shaderLookup[shaderInfo.Name] = foundShader;
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate shader name found: {shaderInfo.Name}");
                    }
                }
            }

            foreach (var material in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (shaderLookup.TryGetValue(material.shader.name, out Shader newShader))
                {
                    if (material.shader != newShader)
                    {
                        material.shader = newShader;
                    }
                }
            }
        }
    }

    private static async Task<string> ReadAllTextAsync(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            return await reader.ReadToEndAsync();
        }
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
            yield return ShaderManager.LoadShadersFromDictionaryAsync();
        }
    }
}
