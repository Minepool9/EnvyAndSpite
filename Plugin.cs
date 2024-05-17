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
using System.Reflection;

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

        private void Awake()
        {
            StartCoroutine(ShaderManager.LoadShadersAsync());

            Logger.LogInfo("If you see this, dont panick! because everything is fine :)");
            terminal = Loader.LoadTerminal();
			
            _instance = this;

            Harmony val = new Harmony("doomahreal.ultrakill.levelloader");
            val.PatchAll();
			
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;		
			_ = Loaderscene.Setup();
			Loaderscene.ExtractSceneName();
        }

        private async void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
			await Loaderscene.DeleteUnpackedLevelsFolder();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneHelper.CurrentScene == "uk_construct")
            {
				terminalInstantiated = false;
				Loaderscene.currentAssetBundleIndex = 0;
            }
			if (SceneHelper.CurrentScene == "Main Menu")
			{
				ShaderManager.CreateShaderDictionary();
                InstantiateEnvyScreen();
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
                StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());
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
			if (SceneHelper.CurrentScene == "Main Menu")
			{
				InstantiateEnvyScreen();
                ShaderManager.CreateShaderDictionary();
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
		
		private void InstantiateEnvyScreen()
		{
			GameObject envyScreenPrefab = terminal.LoadAsset<GameObject>("assets/envyscreen.prefab");

			if (envyScreenPrefab == null)
			{
				Debug.LogError("EnvyScreen prefab not found in the terminal bundle.");
				return;
			}

			GameObject canvasObject = GameObject.Find("/Canvas/Main Menu (1)");
			if (canvasObject == null)
			{
				return;
			}

			GameObject instantiatedObject = Instantiate(envyScreenPrefab);

			instantiatedObject.transform.SetParent(canvasObject.transform, false);
			instantiatedObject.transform.localPosition = Vector3.zero;
			instantiatedObject.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
		}

		private void InstantiateTerminal()
		{
			var shaderHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader");
			shaderHandle.WaitForCompletion();

			if (shaderHandle.Status != AsyncOperationStatus.Succeeded)
				return;

			loadedShader = shaderHandle.Result;

			GameObject instantiatedterminal = terminal.LoadAsset<GameObject>("assets/levelloadterminal.prefab");
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
