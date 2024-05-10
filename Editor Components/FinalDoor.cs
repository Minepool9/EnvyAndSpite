using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace DoomahLevelLoader.UnityComponents
{
    public class FinalDoorFixer : MonoBehaviour
    {
        public bool oneTime = true;
        public bool moveToParent = true;
        public BoxCollider OpenTrigger;
        private FinalDoor FD;
        private GameObject instantiatedObject;
        private bool isOpened = false;

        private void OnEnable()
        {
            Activate();
        }

        private bool _activated = false;

        public void Activate()
        {
            if (oneTime && _activated)
                return;

            _activated = true;

            GameObject targetObject = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Levels/Special Rooms/FinalRoom.prefab").WaitForCompletion();
            if (targetObject == null)
            {
                Debug.LogWarning($"Tried to load asset, but it does not exist");
                enabled = false;
                return;
            }

            instantiatedObject = Instantiate(targetObject, transform.position, transform.rotation, transform);

            if (moveToParent)
                instantiatedObject.transform.SetParent(transform.parent, true);

            Debug.Log("FinalDoorFixer: Final door game object loaded successfully.");

            FinalDoor fdComponent = instantiatedObject.transform.Find("FinalDoor")?.GetComponent<FinalDoor>();
            if (fdComponent != null)
            {
                FD = fdComponent;
                Debug.Log("FinalDoorFixer: Final door component found successfully.");
            }
            else
            {
                Debug.LogWarning("FinalDoorFixer: Final door component not found.");
            }

            PostInstantiate(instantiatedObject);

        }

        protected virtual void PostInstantiate(GameObject instantiatedObject) { }

        private void OnTriggerEnter(Collider other)
        {
            if (!isOpened && other.CompareTag("Player") && FD != null)
            {
                FD.Open();
                isOpened = true;
            }
        }
    }
}
