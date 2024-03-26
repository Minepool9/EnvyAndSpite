using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

	public class TriggerZoneBehavior : MonoBehaviour
	{
		public float delay = 2f; // Adjust the delay time as needed
		public float activationDelay = 0.1f; // Delay between activating each child
		private bool hasActivated = false; // Flag to track if activation has occurred
		private List<Transform> ignoreList = new List<Transform>(); // List to store ignored children

		private void Start()
		{
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
				bool allChildrenActivated = true; // Flag to track if all eligible children are activated
				foreach (Transform child in transform)
				{
					if (!ignoreList.Contains(child) && !child.name.Contains("Gore Zone") && !child.gameObject.activeSelf)
					{
						allChildrenActivated = false; // Set flag to false if there are still eligible children to activate
						if (child.name != "NoPass(Clone)")
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
				// If all eligible children are activated, break the loop
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