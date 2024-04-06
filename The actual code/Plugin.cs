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
        private const KeyCode instantiateKey = KeyCode.U;

        private GameObject instantiatedPrefab;
		private GameObject instantiatedterminal;
        private AssetBundle assetBundle;
		private AssetBundle terminal;
		private bool terminalInstantiated = false;
		public bool firstTimeFlag = true;

		
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

				// Unload the asset bundle
				assetBundle.Unload(true);
				
				// Wait for 0.3 seconds
				StartCoroutine(ReloadAssetBundle());
			}
			else
			{
				// Instantiate prefab named "MapBase" from the asset bundle at position (0, 300, 0) in the currently loaded scene
				instantiatedPrefab = assetBundle.LoadAsset<GameObject>("MapBase");
				if (instantiatedPrefab != null)
				{
					GameObject instantiatedObject = Instantiate(instantiatedPrefab, new Vector3(0, 300, 0), Quaternion.identity);
					AddFloorTagToChildrenWithCollider(instantiatedObject.transform);
					ApplyCustomShaderToStandardMaterials(instantiatedObject); // Call method to apply custom shader
					SpawnableManager spawnableManager = new SpawnableManager();
					// Call ManageSpawnables and pass the line of code as onCompleteCallback
					spawnableManager.ManageSpawnables(instantiatedObject.transform, Instantiate, Destroy, () =>
					{

					});
					
					if (firstTimeFlag) // Check the flag before executing the lines
					{
						// Execute the lines only if firstTimeFlag is true
						MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage("<color=red>Please spawn an enemy in the created map and rebuild navmesh and delete him for the level to work!</color>", "", "", 2, false);
						MonoSingleton<SandboxNavmesh>.Instance.isDirty = true;
						MonoSingleton<CheatsManager>.Instance.RenderCheatsInfo();

						// Set the flag to false after executing the lines
						firstTimeFlag = false;
					}
				}
				else
				{
					Logger.LogError("Failed to load prefab from asset bundle.");
				}
			}
		}
		
		private void ApplyCustomShaderToStandardMaterials(GameObject instantiatedObject)
		{
			// Get all MeshRenderers in the instantiated object and its children
			MeshRenderer[] meshRenderers = instantiatedObject.GetComponentsInChildren<MeshRenderer>(true);

			// Load the custom shader
			Shader loadedShader = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader").WaitForCompletion();

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
		
        private IEnumerator ReloadAssetBundle()
        {
            yield return new WaitForSeconds(0.1f);

            // Reload the asset bundle
            assetBundle = Loader.LoadBundle();

            yield return new WaitForSeconds(0.1f);
            InstantiatePrefab();
        }

		private void AddFloorTagToChildrenWithCollider(Transform parent)
		{
			foreach (Transform child in parent)
			{
				Collider collider = child.GetComponent<Collider>();
				if (collider != null)
				{
					// Check if the collider is not a trigger before adding the tag
					if (!collider.isTrigger)
					{
						child.gameObject.tag = "Floor";
						child.gameObject.layer = 8;
					}
				}
				// Recursively check children
				AddFloorTagToChildrenWithCollider(child);
			}
		}

    }
}

