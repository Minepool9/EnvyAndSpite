using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HarmonyLib;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DoomahLevelLoader
{
    [BepInPlugin("doomahreal.ultrakill.levelloader", "DoomahLevelLoader", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private AssetBundle terminal;
        private bool terminalInstantiated = false;
        private Shader loadedShader;
		public static bool IsCustomLevel = false;
        private static Plugin _instance;
		private Shader unlitShader;
		private Shader vertexLitShader;
		
        public static Plugin Instance => _instance;

        private void Awake()
        {
            Logger.LogInfo("doomahreal.ultrakill.levelloader is loaded!");

            Loaderscene.LoadAssetBundles();

            terminal = Loader.LoadTerminal();
            _instance = this;

            Harmony val = new Harmony("doomahreal.ultrakill.levelloader");
            val.PatchAll();

            Loaderscene.scenePath = Loaderscene.ExtractScene();
			LoadShaders();
			
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneHelper.CurrentScene == "uk_construct")
            {
                terminalInstantiated = false; 
            }
            if (scene.path == Loaderscene.scenePath)
            {
                SceneHelper.CurrentScene = SceneManager.GetActiveScene().name;
                Camera mainCamera = Camera.main;
				IsCustomLevel = true;
                if (mainCamera != null)
                {
                    mainCamera.clearFlags = CameraClearFlags.Skybox;
                }
                else
                {
                    Debug.LogWarning("Main camera not found in the scene.");
                }		
				ApplyShaders();
            }
			else
			{
				IsCustomLevel = false;
			}
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (SceneHelper.CurrentScene == "uk_construct")
            {
                terminalInstantiated = false; 
            }
        }

        private void Update()
        {
            if (SceneHelper.CurrentScene == "uk_construct" && terminal != null && !terminalInstantiated)
            {
                InstantiateTerminal();
                terminalInstantiated = true;
            }
        }

		private void InstantiateTerminal()
		{
			var shaderHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader");
			shaderHandle.WaitForCompletion();

			if (shaderHandle.Status != AsyncOperationStatus.Succeeded)
				return;

			loadedShader = shaderHandle.Result;

			GameObject instantiatedterminal = terminal.LoadAsset<GameObject>("assets/custom/levelloadterminal.prefab");
			if (instantiatedterminal == null)
				return;

			GameObject instantiatedObject = Instantiate(instantiatedterminal, new Vector3(-36f, -10f, 335f), Quaternion.Euler(0f, 0f, 180f));

			Transform cubeTransform = instantiatedObject.transform.Find("Cube");
			if (cubeTransform == null)
				return;

			GameObject[] cubeArray = new GameObject[1];
			cubeArray[0] = cubeTransform.gameObject; 

			Renderer renderer = cubeTransform.GetComponent<Renderer>();
			if (renderer == null)
				return;

			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				material.shader = loadedShader;
			}
			renderer.materials = materials;

			var outdoorsChecker = instantiatedObject.AddComponent<OutdoorsChecker>();

			if (outdoorsChecker != null)
			{
				outdoorsChecker.targets = cubeArray;
				outdoorsChecker.nonSolid = false;
			}
		}
		
		private async void LoadShaders()
		{
			// Load the unlit shader
			AsyncOperationHandle<Shader> unlitHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader");
			await unlitHandle.Task;
			unlitShader = unlitHandle.Result;

			// Load the vertex lit shader
			AsyncOperationHandle<Shader> vertexLitHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader");
			await vertexLitHandle.Task;
			vertexLitShader = vertexLitHandle.Result;
		}

		private void ApplyShaders()
		{
			// Find all GameObjects in the scene
			GameObject[] allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

			// Iterate through all GameObjects
			foreach (GameObject go in allGameObjects)
			{
				// Get all MeshRenderers, including inactive ones
				MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);

				// Apply shaders to each MeshRenderer
				foreach (MeshRenderer renderer in meshRenderers)
				{
					if (renderer.gameObject.name == "Quad")
					{
						continue;
					}
					if (renderer.material.shader.name == "ULTRAKILL/VERTEXLIT")
					{
						ApplyShaderToRenderer(renderer, vertexLitShader);
					}
					else if (renderer.material.shader.name == "Standard" || renderer.material.shader.name == "ULTRAKILL/UNLIT")
					{
						ApplyShaderToRenderer(renderer, unlitShader);
					}
					// You can add more conditions for other shaders if needed
				}
			}
		}


		private void ApplyShaderToRenderer(MeshRenderer renderer, Shader shader)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				material.shader = shader;
			}
			renderer.materials = materials;
		}
    }
}
