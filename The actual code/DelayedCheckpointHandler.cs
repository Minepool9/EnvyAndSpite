using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DoomahLevelLoader
{
    public class DelayedCheckpointHandler : MonoBehaviour
    {
        private GameObject originalGameObject;
        private GameObject instantiatedPrefab;
        private Action<GameObject> destroyMethod;

        public void Initialize(GameObject originalGameObject, GameObject instantiatedPrefab, Action<GameObject> destroyMethod)
        {
            this.originalGameObject = originalGameObject;
            this.instantiatedPrefab = instantiatedPrefab;
            this.destroyMethod = destroyMethod;
            StartCoroutine(DelayedCheckPoint());
			// Set the instantiated prefab inactive
            instantiatedPrefab.SetActive(true);
        }

        private IEnumerator DelayedCheckPoint()
        {
            // Wait for 0.2 seconds
            yield return new WaitForSeconds(0.2f);
			
            // Set the parent of the instantiated prefab to the parent of the original game object
            instantiatedPrefab.transform.SetParent(originalGameObject.transform.parent);
            // Retrieve the RoomManager component from the original game object
            ReallyCustomRoomManager roomManager = originalGameObject.GetComponent<ReallyCustomRoomManager>();
            if (roomManager != null)
            {
                // Store the public arrays from the RoomManager component
                List<GameObject> activeRooms = new List<GameObject>(roomManager.ActiveRooms);
                // Retrieve the CheckPoint component from the instantiated prefab
                CheckPoint checkpoint = instantiatedPrefab.GetComponent<CheckPoint>();
                if (checkpoint != null)
                {
                    // Set the defaultRooms list to match the originalRooms list
                    checkpoint.defaultRooms = activeRooms;
                }
            }
        }
    }
}
