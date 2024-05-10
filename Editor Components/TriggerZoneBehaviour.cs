using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

	public class TriggerZoneBehavior : MonoBehaviour
	{
		public float delay = 2f;
		public float activationDelay = 0.1f;
		private bool hasActivated = false;
		private List<Transform> ignoreList = new List<Transform>(); 

		private void Start()
		{
			Collider collider = GetComponent<Collider>();
			if (collider != null)
			{
				collider.isTrigger = true;
			}
			
			DelayedInitialization();
		}

		private void DelayedInitialization()
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}

		private IEnumerator ActivateChildrenWithDelay()
		{
			while (true)
			{
				bool allChildrenActivated = true;
				foreach (Transform child in transform)
				{
					if (child == null)
						continue;

					if (!ignoreList.Contains(child) && !child.name.Contains("Gore Zone") && !child.gameObject.activeSelf)
					{
						allChildrenActivated = false; 
						if (child.name != null && (child.name != "NoPass(Clone)" || child.GetComponent<DoomahLevelLoader.UnityComponents.AddressableReplacer>() == null))
						{
							yield return new WaitForSeconds(activationDelay);
						}
						child.gameObject.SetActive(true);
						Debug.Log("Trigger Activated child:  " + child.name);
					}
					else
					{
						ignoreList.Add(child);
					}
				}

				if (allChildrenActivated)
				{
					break;
				}
			}

			hasActivated = true;
		}



		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name == "Player" && !hasActivated)
			{
				StartCoroutine(ActivateChildrenWithDelay());
			}
		}
	}