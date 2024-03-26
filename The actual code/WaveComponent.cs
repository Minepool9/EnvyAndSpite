using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

public class WaveComponent : MonoBehaviour
{
    public float checkInterval = 1f; // Adjust the interval as needed
    private float timer = 0f;
    private float activationDelay = 0.1f; // Delay between activating each child
    private bool hasActivated = false; // Flag to track if activation has occurred
    private List<Transform> activatedChildren = new List<Transform>(); // List to store activated children
    private List<Transform> ignoreList = new List<Transform>(); // List to store ignored children

    private void Start()
    {
        // Disable all children of the GameObject
        DisableAllChildren();
    }

    private void DisableAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Increment the timer with delta time
        timer += Time.deltaTime;

        // Check if the timer has exceeded the check interval
        if (timer >= checkInterval)
        {
            // Reset the timer
            timer = 0f;

            // Check the state of child objects and activate children accordingly
            ActivateChildrenIfNeeded();
        }
    }

    private IEnumerator ActivateChildrenWithDelay(Transform[] childrenToActivate)
    {
        foreach (Transform child in childrenToActivate)
        {
            if (child.name != "NoPass(Clone)")
            {
                child.gameObject.SetActive(true);
                yield return new WaitForSeconds(activationDelay);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void ActivateChildrenIfNeeded()
    {
        // Get the parent GameObject
        GameObject parentObject = transform.parent.gameObject;

        // Get the immediate children of the parent
        List<GameObject> childrenOfWaves = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Gore Zone"))
            {
                childrenOfWaves.Add(child.gameObject);
            }
        }

        // Get all siblings and grandchildren
        List<Transform> siblingsAndGrandchildren = new List<Transform>();
        siblingsAndGrandchildren.AddRange(parentObject.GetComponentsInChildren<Transform>());
        siblingsAndGrandchildren.Remove(transform); // Remove the object itself

        List<Transform> enemies = new List<Transform>();

        // Filter out GameObjects without the "EnemyIdentifier" component or with the "dead" flag set to false
        foreach (Transform obj in siblingsAndGrandchildren)
        {
            EnemyIdentifier enemyIdentifier = obj.GetComponent<EnemyIdentifier>();
            if (enemyIdentifier != null && !enemyIdentifier.dead)
            {
                enemies.Add(obj);
            }
            else if (enemyIdentifier != null && enemyIdentifier.dead && enemies.Contains(obj))
            {
                enemies.Remove(obj); // Remove the enemy from the list if it's dead
            }
        }
		// Convert GameObject array to Transform array
		Transform[] transformsToActivate = new Transform[childrenOfWaves.Count];
		for (int i = 0; i < childrenOfWaves.Count; i++)
		{
			transformsToActivate[i] = childrenOfWaves[i].transform;
		}
		
		
		
        // If all identified enemies are dead, activate the children with a delay
        if (enemies.Count == 0 && !hasActivated)
        {
            hasActivated = true; // Mark as activated to prevent repeated activations
			StartCoroutine(ActivateChildrenWithDelay(transformsToActivate));
        }
    }
}
