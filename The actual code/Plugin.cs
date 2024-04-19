using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HarmonyLib;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Logic;

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
		
        public static Plugin Instance => _instance;

        private async Task Awake()
        {
            Logger.LogInfo("doomahreal.ultrakill.levelloader is loaded!");
            Loaderscene.Setup();
            terminal = Loader.LoadTerminal();
			
            _instance = this;

            Harmony val = new Harmony("doomahreal.ultrakill.levelloader");
            val.PatchAll();
			
			Loaderscene.ExtractSceneName();
			
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;		
			await ShaderManager.LoadShaders();
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
				Debug.Log("Is scene alright parthner");
            }
            if (scene.name == Loaderscene.LoadedSceneName)
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
				ShaderManager.ApplyShaders(SceneManager.GetActiveScene().GetRootGameObjects());
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
		
        public static void FixVariables()
        {
            MapVarManager.Instance.ReloadMapVars();
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
    }
}
