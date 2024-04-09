using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace DoomahLevelLoader.UnityComponents
{
    public class CheckpointReplacer : AddressableReplacer
    {
        protected override void PostInstantiate(GameObject instantiatedObject)
        {
            Debug.Log("Replacing checkpoint");

            CheckPoint realCheckpoint = GetComponentInParent<CheckPoint>();
            CheckPoint templateCheckpoint = instantiatedObject.GetComponent<CheckPoint>();

            foreach (Collider hitbox in instantiatedObject.GetComponentsInChildren<Collider>().Where(col => col.isTrigger))
                Destroy(hitbox.gameObject);

            foreach (Transform child in Enumerable.Range(0, instantiatedObject.transform.childCount).Select(i => instantiatedObject.transform.GetChild(i)).ToArray())
            {
                Debug.Log(child.name);
                child.SetParent(transform.parent, true);
            }

            realCheckpoint.graphic = templateCheckpoint.graphic;
            realCheckpoint.activateEffect = templateCheckpoint.activateEffect;
        }
    }
}