using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

namespace DoomahLevelLoader
{
	public class SpawnableManager
	{	
		private Dictionary<string, string> prefabPaths = new Dictionary<string, string>
		{
			{ "Wave", "Placeholder" },
			{ "TriggerZone", "Placeholder" },
			{ "PlayerStart", "Placeholder" },
			{ "Blocker", "Assets/Prefabs/Decals/NoPass.prefab" },
			{ "Filth", "Assets/Prefabs/Enemies/Zombie.prefab" },
			{ "Virtue", "Assets/Prefabs/Enemies/Virtue.prefab" },
			{ "Cerberus", "Assets/Prefabs/Enemies/StatueEnemy.prefab" },
			{ "Drone", "Assets/Prefabs/Enemies/Drone.prefab" },
			{ "Maurice", "Assets/Prefabs/Enemies/Spider.prefab" },
			{ "Stray", "Assets/Prefabs/Enemies/Projectile Zombie.prefab" },
			{ "Schism", "Assets/Prefabs/Enemies/Super Projectile Zombie.prefab" },
			{ "Soldier", "Assets/Prefabs/Enemies/ShotgunHusk.prefab" },
			{ "SwordsMachine", "Assets/Prefabs/Enemies/SwordsMachineNonboss.prefab" },
			{ "Sisyphus_Insurrectionist", "Assets/Prefabs/Enemies/Sisyphus.prefab" },
			{ "Hideous_Mass", "Assets/Prefabs/Enemies/Mass.prefab" },
			{ "StreetCleaner", "Assets/Prefabs/Enemies/Streetcleaner.prefab" },
			{ "Health_Power_Up", "Assets/Prefabs/Levels/BonusGhostSuperCharge.prefab" },
			{ "Duel_Power_Up", "Assets/Prefabs/Levels/DualWieldPowerup.prefab" },
			{ "Stalker", "Assets/Prefabs/Enemies/Stalker.prefab" },
			{ "Ferryman", "Assets/Prefabs/Enemies/Ferryman.prefab" },
			{ "Mannequin", "Assets/Prefabs/Enemies/Mannequin.prefab" },
			{ "Mindflayer", "Assets/Prefabs/Enemies/Mindflayer.prefab" },
			{ "Sentry", "Assets/Prefabs/Enemies/Turret.prefab" },
			{ "V2", "Assets/Prefabs/Enemies/V2.prefab" },
			{ "Gabe_First", "Assets/Prefabs/Enemies/Gabriel.prefab" },
			{ "Gabe_Second", "Assets/Prefabs/Enemies/Gabriel 2nd Variant.prefab" },
			{ "GutterTank", "Assets/Prefabs/Enemies/Guttertank.prefab" },
			{ "GutterMan", "Assets/Prefabs/Enemies/Gutterman.prefab" },
			{ "Puppet", "Assets/Prefabs/Enemies/Puppet.prefab" },
			{ "Green_Grapple", "Assets/Prefabs/Levels/Interactive/GrapplePoint.prefab" },
			{ "Blue_Grapple", "Assets/Prefabs/Levels/Interactive/GrapplePointSlingshot Variant.prefab" },
			{ "CheckPoint", "Assets/Prefabs/Levels/Checkpoint.prefab" },
			{ "CheckPoint_Invisible", "Assets/Prefabs/Levels/Checkpoint.prefab" },
			{ "Yellow_Shop", "Assets/Prefabs/Levels/Shop.prefab" },
			// Add more mappings for other suffixes if needed
		};
		
		public void ManageSpawnables(Transform parent, Func<GameObject, Vector3, Quaternion, GameObject> instantiateMethod, Action<GameObject> destroyMethod, Action onCompleteCallback)		{
			// Find all children with names starting with "Spawnable_"
			foreach (Transform child in parent)
			{
				if (child.gameObject.name.StartsWith("Spawnable_"))
				{
					// Get the suffix after "Spawnable_"
					string suffix = child.gameObject.name.Substring("Spawnable_".Length);

					// Remove any spaces and anything after them from the suffix
					int spaceIndex = suffix.IndexOf(' ');
					if (spaceIndex != -1)
					{
						suffix = suffix.Substring(0, spaceIndex);
					}
					
					// Check if the suffix exists in the prefabPaths dictionary
					if (prefabPaths.ContainsKey(suffix))
					{
						// Load and instantiate prefabs for all suffixes except "TriggerZone"
                        if (suffix != "TriggerZone" && suffix != "Wave" && suffix != "PlayerStart" && suffix != "CheckPoint")
						{
							Addressables.LoadAssetAsync<GameObject>(prefabPaths[suffix]).Completed += op =>
							{
								GameObject prefab = op.Result;
								GameObject instance = instantiateMethod(prefab, child.position, child.rotation);

								// Handle special cases for other suffixes
								switch (suffix)
								{
									case "Duel_Power_Up":
									case "Health_Power_Up":
									case "Green_Grapple":
									case "Blue_Grapple":
										// Set the instantiated prefab inactive
										instance.SetActive(true);
										// Set the parent of the instantiated prefab to the parent of the original game object
										instance.transform.SetParent(child.parent);
										// Destroy the original game object
										destroyMethod(child.gameObject);
										break;
									case "CheckPoint_Invisible":
										// Set the instantiated prefab inactive
										instance.SetActive(true);
										// Set the parent of the instantiated prefab to the parent of the original game object
										instance.transform.SetParent(child.parent);
										// Get the CheckPoint component from the instantiated prefab
										CheckPoint checkpointComponent = instance.GetComponent<CheckPoint>();
										// Check if the CheckPoint component exists
										if (checkpointComponent != null)
										{
											// Set the 'invisible' bool to true
											checkpointComponent.invisible = true;
										}
										// Destroy the original game object
										destroyMethod(child.gameObject);
										break;
									case "Yellow_Shop":
										// Set the instantiated prefab inactive
										instance.SetActive(true);
										// Set the parent of the instantiated prefab to the parent of the original game object
										instance.transform.SetParent(child.parent);
										// Rotate the Terminal
										instance.transform.Rotate(0f, 0f, 180f);

										// Load the shader asynchronously
										Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit.shader").Completed += shaderOp =>
										{
											if (shaderOp.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
											{
												Shader loadedShader = shaderOp.Result;

												// Find the "Cube" game object under the instantiated prefab
												Transform cubeTransform = instance.transform.Find("Cube");
												if (cubeTransform != null)
												{
													// Get the MeshRenderer component of the "Cube" game object
													MeshRenderer cubeRenderer = cubeTransform.GetComponent<MeshRenderer>();
													if (cubeRenderer != null)
													{
														// Iterate through each material in the MeshRenderer
														foreach (Material material in cubeRenderer.materials)
														{
															// Set the shader for each material to the loaded shader
															material.shader = loadedShader;
														}
													}
												}
											}
										};

										// Destroy the original game object
										destroyMethod(child.gameObject);
										break;
									case "Blocker":
										// Retrieve the size of the original GameObject's BoxCollider
										Vector3 originalColliderSize = child.GetComponent<BoxCollider>().size;

										// Set the instantiated prefab inactive
										instance.SetActive(false);
										// Set the parent of the instantiated prefab to the parent of the original game object
										instance.transform.SetParent(child.parent);
										
										instance.transform.Rotate(0f, 90f, 0f);


										// Get all children of the instantiated prefab
										foreach (Transform blockChild in instance.transform)
										{
											// Add a BoxCollider component to each child
											BoxCollider childCollider = blockChild.gameObject.AddComponent<BoxCollider>();
											// Retrieve the size of the original GameObject's BoxCollider
											childCollider.size = originalColliderSize;
										}
										
										instance.gameObject.AddComponent<BlockerComponent>();
										
										// Destroy the original game object
										destroyMethod(child.gameObject);
										break;
									default:
										// Set the instantiated prefab inactive
										instance.SetActive(false);
										// Set the parent of the instantiated prefab to the parent of the original game object
										instance.transform.SetParent(child.parent);
										// Destroy the original game object
										destroyMethod(child.gameObject);
										break;
								}


								// Check if this is the last child
								if (child == parent.GetChild(parent.childCount - 1))
								{
									// Invoke the onCompleteCallback
									onCompleteCallback?.Invoke();
								}
							};
						}
                        else if (suffix == "TriggerZone")
                        {
                            // Add TriggerZone behavior directly without instantiating any prefab
                            child.gameObject.AddComponent<TriggerZoneBehavior>();
                        }
                        else if (suffix == "Wave")
                        {
                            // Add WaveComponent behavior directly without instantiating any prefab
                            child.gameObject.AddComponent<WaveComponent>();
                        }
						else if (suffix == "PlayerStart")
						{	
							// Teleport the player to well.... the gameObject...
							NewMovement.Instance.transform.position = child.gameObject.transform.position;
						}
						else if (suffix == "CheckPoint")
						{

						}
					}
					else
					{
						Debug.LogWarning($"No prefab path found for suffix: {suffix}");

						// Check if this is the last child
						if (child == parent.GetChild(parent.childCount - 1))
						{
							// Invoke the onCompleteCallback
							onCompleteCallback?.Invoke();
						}
					}
				}

				// Recursively check children
				ManageSpawnables(child, instantiateMethod, destroyMethod, onCompleteCallback);
			}
		}


	}
}