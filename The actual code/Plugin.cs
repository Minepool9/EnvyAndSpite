using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace DoomahLevelLoader
{
    [BepInPlugin("doomahreal.ultrakill.levelloader", "DoomahLevelLoader", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private GameObject instantiatedPrefab;
		private GameObject instantiatedterminal;
        public static AssetBundle assetBundle;
		private AssetBundle terminal;
		private bool terminalInstantiated = false;
		public Shader loadedShader;
		
		// Static instance variable to hold the instance of the class
        private static Plugin _instance;

        // Static property to access the instance
        public static Plugin Instance
        {
            get { return _instance; }
        }


        private void Awake()
        {
            Logger.LogInfo("doomahreal.ultrakill.levelloader is loaded!");
			
            Loader.LoadDoomahFiles(); // Load .doomah files

            // Load the asset bundle during Awake
            assetBundle = Loader.LoadBundle();
			terminal = Loader.LoadTerminal();
			
			_instance = this;
			
			Harmony val = new Harmony("doomahreal.ultrakill.levelloader");
			val.PatchAll();
			
			// Register for scene events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
		
        private void OnDestroy()
        {
            // Unregister scene events to avoid memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneHelper.CurrentScene == "uk_construct")
            {
                terminalInstantiated = false; // Reset the flag when the target scene is loaded
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (SceneHelper.CurrentScene == "uk_construct")
            {
                terminalInstantiated = false; // Reset the flag when the target scene is unloaded
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
			var shaderHandle = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader");
			shaderHandle.WaitForCompletion();

			if (shaderHandle.Status != AsyncOperationStatus.Succeeded)
				return;

			Shader loadedShader = shaderHandle.Result;

			instantiatedterminal = terminal.LoadAsset<GameObject>("assets/custom/levelloadterminal.prefab");
			if (instantiatedterminal == null)
				return;

			GameObject instantiatedObject = Instantiate(instantiatedterminal, new Vector3(-36f, -10f, 335f), Quaternion.Euler(0f, 0f, 180f));

			Transform cubeTransform = instantiatedObject.transform.Find("Cube");
			if (cubeTransform == null)
				return;

			Renderer renderer = cubeTransform.GetComponent<Renderer>();
			if (renderer == null)
				return;

			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				material.shader = loadedShader;
			}
			renderer.materials = materials;
		}
		
		public void InstantiatePrefab()
		{
			// Find any GameObject with the name "MapBase(Clone)" in the scene
			GameObject existingMapBase = GameObject.Find("MapBase(Clone)");

			if (existingMapBase != null)
			{
				// If found, destroy the existing one
				Destroy(existingMapBase);
			}

			// Instantiate prefab named "MapBase" from the asset bundle at position (0, 300, 0) in the currently loaded scene
			instantiatedPrefab = assetBundle.LoadAsset<GameObject>("MapBase");
			if (instantiatedPrefab != null)
			{
				GameObject instantiatedObject = Instantiate(instantiatedPrefab, new Vector3(0, 300, 0), Quaternion.identity);
				ApplyCustomShaderToStandardMaterials(instantiatedObject); // Call method to apply custom shader
				NotifyPlayer();
			}
			else
			{
				Logger.LogError("Failed to load prefab from asset bundle.");
			}
		}
		
		private void NotifyPlayer()
		{
			HudMessageReceiver.Instance.SendHudMessage("<color=red>Please spawn an enemy in the created map and rebuild navmesh and delete him for the level to work!</color>", "", "", 1, false);
			SandboxNavmesh.Instance.isDirty = true;
			CheatsManager.Instance.RenderCheatsInfo();
		}
		private void ApplyCustomShaderToStandardMaterials(GameObject instantiatedObject)
		{
			// Get all MeshRenderers in the instantiated object and its children
			MeshRenderer[] meshRenderers = instantiatedObject.GetComponentsInChildren<MeshRenderer>(true);

			// Load the custom shader
			loadedShader = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader").WaitForCompletion();

			if (loadedShader == null)
			{
				Logger.LogError("Failed to load custom shader.");
				return;
			}

			foreach (MeshRenderer renderer in meshRenderers)
			{
				// Check each material of the renderer
				foreach (Material material in renderer.materials)
				{
					// Check if the material's shader is named "Standard"
					if (material.shader.name == "Standard")
					{
						// Apply the custom shader
						material.shader = loadedShader;
					}
				}
			}
		}
    }
}

