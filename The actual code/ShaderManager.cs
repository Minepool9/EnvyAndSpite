using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public static class ShaderManager
{
    private static Shader unlitShader;
    private static Shader vertexLitShader;
    private static Shader transparentShader;

    private static Dictionary<string, Shader> shaderDictionary;

    public static async Task LoadShaders()
    {
        AsyncOperationHandle<Shader> unlitHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader");
        await unlitHandle.Task;
        unlitShader = unlitHandle.Result;

        AsyncOperationHandle<Shader> vertexLitHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader");
        await vertexLitHandle.Task;
        vertexLitShader = vertexLitHandle.Result;
        
        AsyncOperationHandle<Shader> transparentHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Transparent/ULTRAKILL-vertexlit-transparent-zwrite.shader");
        await transparentHandle.Task;
        transparentShader = transparentHandle.Result;

        shaderDictionary = new Dictionary<string, Shader>
        {
            { "ULTRAKILL/VERTEXLIT", vertexLitShader },
            { "Standard", unlitShader },
            { "ULTRAKILL/UNLIT", unlitShader },
            { "ULTRAKILL/TRANSPARENT", transparentShader },
        };
    }

    public static void ApplyShaders(GameObject[] allGameObjects)
    {
        foreach (GameObject go in allGameObjects)
        {
            MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer renderer in meshRenderers)
            {
                if (renderer.gameObject.name == "Quad")
                {
                    continue;
                }

                if (shaderDictionary.ContainsKey(renderer.material.shader.name))
                {
                    Shader shader = shaderDictionary[renderer.material.shader.name];
                    ApplyShaderToRenderer(renderer, shader);
                }
            }
        }
    }

    private static void ApplyShaderToRenderer(MeshRenderer renderer, Shader shader)
    {
        Material[] materials = renderer.materials;
        foreach (Material material in materials)
        {
            material.shader = shader;
        }
        renderer.materials = materials;
    }
}
