using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx;
using HarmonyLib;

public class BlockerComponent : MonoBehaviour
{
    private float checkInterval = 1f; // Adjust the interval as needed
    private float timer = 0f;

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

    private void DeactivateGameobject()
    {
        gameObject.SetActive(false);
    }


    private void ActivateChildrenIfNeeded()
    {
        // Get the parent GameObject
        GameObject parentObject = transform.parent.gameObject;

        // Get the immediate children of the parent
        Transform[] immediateChildren = parentObject.GetComponentsInChildren<Transform>();

        bool allChildrenDead = true;

        // Check each immediate child for the "EnemyIdentifier" component and its "dead" flag
        foreach (Transform immediateChild in immediateChildren)
        {
            // Skip the immediate child of the GameObject the script is attached to
            if (immediateChild == transform)
                continue;

            // Check the immediate child for the "EnemyIdentifier" component
            EnemyIdentifier immediateChildEnemyIdentifier = immediateChild.GetComponent<EnemyIdentifier>();
            if (immediateChildEnemyIdentifier != null && !immediateChildEnemyIdentifier.dead)
            {
                allChildrenDead = false;
                break;
            }

            // Check the immediate children of the immediate child
            Transform[] childrenOfImmediateChild = immediateChild.GetComponentsInChildren<Transform>();
            foreach (Transform childOfImmediateChild in childrenOfImmediateChild)
            {
                // Skip the immediate child of the immediate child
                if (childOfImmediateChild == immediateChild)
                    continue;

                // Check the immediate child's immediate children for the "EnemyIdentifier" component
                EnemyIdentifier childEnemyIdentifier = childOfImmediateChild.GetComponent<EnemyIdentifier>();
                if (childEnemyIdentifier != null && !childEnemyIdentifier.dead)
                {
                    allChildrenDead = false;
                    break;
                }
            }

            if (!allChildrenDead)
                break;
        }

        // If all immediate children and their immediate children with "EnemyIdentifier" components are dead, deactivate own children with delay
        if (allChildrenDead || immediateChildren.Length == 2)
        {
            DeactivateGameobject();
        }
    }
}
