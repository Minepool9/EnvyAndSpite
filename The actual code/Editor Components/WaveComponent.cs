using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

public class WaveComponent : MonoBehaviour
{
    public float checkInterval = 1f;
    private float timer = 0f;
    private float activationDelay = 0.1f;
    private bool hasActivated = false;
    private List<Transform> activatedChildren = new List<Transform>();
    private List<Transform> ignoreList = new List<Transform>();

    private void Start()
    {
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
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;

            CheckChildren();
        }
    }

	private IEnumerator ActivateChildrenWithDelay(Transform[] childrenToActivate)
	{
		foreach (Transform child in childrenToActivate)
		{
			if (child.name != "NoPass(Clone)" || child.GetComponent<DoomahLevelLoader.UnityComponents.AddressableReplacer>() == null)
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


    private void CheckChildren()
    {
        GameObject parentObject = transform.parent.gameObject;

        List<GameObject> childrenOfWaves = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childrenOfWaves.Add(child.gameObject);
        }

        List<Transform> siblingsAndGrandchildren = new List<Transform>();
        siblingsAndGrandchildren.AddRange(parentObject.GetComponentsInChildren<Transform>());
        siblingsAndGrandchildren.Remove(transform); // Remove the object itself

        List<Transform> enemies = new List<Transform>();

        foreach (Transform obj in siblingsAndGrandchildren)
        {
            EnemyIdentifier enemyIdentifier = obj.GetComponent<EnemyIdentifier>();
            if (enemyIdentifier != null && !enemyIdentifier.dead)
            {
                enemies.Add(obj);
            }
            else if (enemyIdentifier != null && enemyIdentifier.dead && enemies.Contains(obj))
            {
                enemies.Remove(obj);
            }
        }
		Transform[] transformsToActivate = new Transform[childrenOfWaves.Count];
		for (int i = 0; i < childrenOfWaves.Count; i++)
		{
			transformsToActivate[i] = childrenOfWaves[i].transform;
		}
		
		
		
        if (enemies.Count == 0 && !hasActivated)
        {
            hasActivated = true;
			StartCoroutine(ActivateChildrenWithDelay(transformsToActivate));
        }
    }
}